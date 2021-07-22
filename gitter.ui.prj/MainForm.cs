#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.IO;
	using System.Text;
	using System.Windows.Forms;
	using System.Xml;

	using gitter.Controls;
	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Options;
	using gitter.Framework.Services;

	using Resources = gitter.Properties.Resources;

	/// <summary>Main application form.</summary>
	public partial class MainForm : FormEx, IWorkingEnvironment
	{
		static class Icons
		{
			const int Size = 16;

			public static readonly IDpiBoundValue<Bitmap> Init           = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"init",            Size);
			public static readonly IDpiBoundValue<Bitmap> Clone          = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"clone",           Size);
			public static readonly IDpiBoundValue<Bitmap> Repository     = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"repository",      Size);
			public static readonly IDpiBoundValue<Bitmap> RepositoryOpen = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"repository.open", Size);
		}

		#region Data

		private INotificationService _notificationService;
		private readonly ViewDockService _viewDockService;

		private readonly Dictionary<string, IRepositoryProvider> _repositoryProviders;
		private readonly Dictionary<string, IRepositoryServiceProvider> _repositoryServiceProviders;
		private IRepositoryProvider _currentProvider;
		private HashSet<IRepositoryServiceProvider> _activeIssueTrackerProviders;
		private IRepository _repository;
		private IRepositoryGuiProvider _repositoryGui;
		private LinkedList<IGuiProvider> _additionalGui;

		private string _recentRepositoryPath;

		private readonly RepositoryExplorerViewFactory _repositoryExplorerFactory;
		private readonly StartPageViewFactory _startPageFactory;
		private readonly LogViewFactory _logFactory;

		private readonly ConfigurationService _configurationService;
		private readonly RepositoryManagerService _repositoryManagerService;
		private readonly DpiBindings _bindings;

		#endregion

		#region Constants

		private const int SavedRecentRepositories = 25;

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="MainForm"/> class.</summary>
		public MainForm()
		{
			InitializeComponent();

			_configurationService     = GitterApplication.ConfigurationService;
			_repositoryManagerService = new RepositoryManagerService(SavedRecentRepositories);
			_viewDockService          = new ViewDockService(this, _toolDockGrid, _configurationService.ViewsSection);
			_notificationService      = new BalloonNotificationService();
			_bindings                 = new DpiBindings(this);

			_repositoryManagerService.RecentRepositories.Changed += OnRecentRepositoriesChanged;

			ProvideToolbar(new StandardToolbar(this));

			var init  = new ToolStripMenuItem(Resources.StrInit.AddEllipsis(),  default, OnInitRepositoryClick);
			var clone = new ToolStripMenuItem(Resources.StrClone.AddEllipsis(), default, OnCloneRepositoryClick);

			_bindings.BindImage(init,               Icons.Init);
			_bindings.BindImage(clone,              Icons.Clone);
			_bindings.BindImage(_mnuOpenRepository, Icons.RepositoryOpen);

			_mnuRepository.DropDownItems.Insert(0, init);
			_mnuRepository.DropDownItems.Insert(1, clone);
			_mnuRepository.DropDownItems.Insert(2, new ToolStripSeparator());

			_viewDockService = new ViewDockService(this, _toolDockGrid, _configurationService.ViewsSection);
			_viewDockService.RegisterFactory(
				_startPageFactory = new StartPageViewFactory());
			_viewDockService.RegisterFactory(
				_repositoryExplorerFactory = new RepositoryExplorerViewFactory(this));
			_viewDockService.RegisterFactory(
				_logFactory = new LogViewFactory());

			LoadOptions();
			LoadRecentRepositories();

			_viewDockService.ShowView(Guids.RepositoryExplorerView);

			_repositoryProviders         = new Dictionary<string, IRepositoryProvider>();
			_repositoryServiceProviders  = new Dictionary<string, IRepositoryServiceProvider>();
			_activeIssueTrackerProviders = new HashSet<IRepositoryServiceProvider>();
			_additionalGui               = new LinkedList<IGuiProvider>();

			LoadProviders();

			_mnuView.DropDownItems.Insert(0, new ViewMenuItem(_repositoryExplorerFactory) { Environment = this });
			_mnuView.DropDownItems.Insert(1, new ViewMenuItem(_startPageFactory) { Environment = this });
			_mnuView.DropDownItems.Insert(2, new ToolStripSeparator());

			_mnuView.DropDownItems.Add(new ToolStripSeparator());
			_mnuView.DropDownItems.Add(new ViewMenuItem(_logFactory) { Environment = this });

			_mnuRepository.Text         = Resources.StrRepository;
			_mnuExit.Text               = Resources.StrExit;
			_mnuOpenRepository.Text     = Resources.StrOpen.AddEllipsis();
			_mnuRecentRepositories.Text = Resources.StrRecent;

			_mnuView.Text     = Resources.StrView;
			_mnuToolbars.Text = Resources.StrToolbars;

			_mnuTools.Text   = Resources.StrTools;
			_mnuOptions.Text = Resources.StrOptions.AddEllipsis();

			_mnuHelp.Text  = Resources.StrHelp;
			_mnuAbout.Text = Resources.StrAbout.AddEllipsis();

			_recentRepositoryPath = string.Empty;
		}

		#endregion

		public IEnumerable<IRepositoryProvider> RepositoryProviders
			=> _repositoryProviders.Values;

		public T GetRepositoryProvider<T>() where T : class, IRepositoryProvider
		{
			foreach(var prov in RepositoryProviders)
			{
				if(prov is T p) return p;
			}
			return default;
		}

		public IEnumerable<IRepositoryServiceProvider> IssueTrackerProviders
			=> _repositoryServiceProviders.Values;

		public IEnumerable<IRepositoryServiceProvider> ActiveIssueTrackerProviders
			=> _activeIssueTrackerProviders;

		public bool TryLoadIssueTracker(IRepositoryServiceProvider provider)
		{
			Verify.Argument.IsNotNull(provider, nameof(provider));
			Verify.State.IsTrue(_repository != null);

			if(!_activeIssueTrackerProviders.Contains(provider) && provider.TryCreateGuiProvider(_repository, out var gui))
			{
				gui.AttachToEnvironment(this);
				_additionalGui.AddLast(gui);
				_activeIssueTrackerProviders.Add(provider);
				return true;
			}
			else
			{
				return false;
			}
		}

		private void LoadProviders()
		{
			int menuid = 0;
			var git = new gitter.Git.RepositoryProvider();
			LoadRepositoryProvider(git, ref menuid);
			if(menuid != 0)
			{
				_mnuRepository.DropDownItems.Insert(menuid, new ToolStripSeparator());
			}
			LoadIssueTrackerProvider(new Redmine.RedmineServiceProvider());
			LoadIssueTrackerProvider(new TeamCity.TeamCityServiceProvider());
			LoadIssueTrackerProvider(new GitLab.GitLabServiceProvider());
		}

		private void LoadRepositoryProvider(IRepositoryProvider provider, ref int menuid)
		{
			if(provider.LoadFor(this, _configurationService.GetSectionForProvider(provider)))
			{
				_repositoryProviders.Add(provider.Name, provider);
			}
		}

		private void LoadIssueTrackerProvider(IRepositoryServiceProvider provider)
		{
			if(provider.LoadFor(this, _configurationService.GetSectionForProvider(provider)))
			{
				_repositoryServiceProviders.Add(provider.Name, provider);
			}
		}

		private void OnInitRepositoryClick(object sender, EventArgs e)
		{
			using var dlg = new InitRepositoryDialog(this);
			dlg.Run(this);
		}

		private void OnCloneRepositoryClick(object sender, EventArgs e)
		{
			using var dlg = new CloneRepositoryDialog(this);
			dlg.Run(this);
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			var cd = Directory.GetCurrentDirectory().ToLower();
			var appPath = Path.GetDirectoryName(Application.ExecutablePath).ToLower();
			string[] args = null;
			try
			{
				args = Environment.GetCommandLineArgs();
			}
			catch(NotSupportedException)
			{
			}
			if(args is not { Length: > 1 })
			{
				if(!cd.EndsWith("\\")) cd += "\\";
				if(!appPath.EndsWith("\\")) appPath += "\\";

				if(appPath != cd)
				{
					if(!OpenRepository(cd, true))
					{
						ShowStartPageOnStartup();
					}
				}
				else
				{
					ShowStartPageOnStartup();
				}
			}
			else
			{
				if(!OpenRepository(args[1], true))
				{
					ShowStartPageOnStartup();
				}
			}
		}

		private void ShowStartPageOnStartup()
		{
			if(_startPageFactory.ShowOnStartup)
			{
				_viewDockService.ShowView(Guids.StartPageView);
			}
		}

		public string RecentRepositoryPath
			=> _recentRepositoryPath;

		public RepositoryManagerService RepositoryManagerService
			=> _repositoryManagerService;

		public INotificationService NotificationService
			=> _notificationService;

		private void _mnuExit_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void _mnuAbout_Click(object sender, EventArgs e)
		{
			using var dlg = new AboutDialog();
			dlg.Run(this);
		}

		private void StartOptionsDialog()
		{
			using var d = new OptionsDialog(this);
			d.Run(this);
		}

		private void _mnuOptions_Click(object sender, EventArgs e)
		{
			StartOptionsDialog();
		}

		private void _mnuOpen_Click(object sender, EventArgs e)
		{
			var path = Utility.ShowPickFolderDialog(this);
			if(path != null)
			{
				OpenRepository(path);
			}
		}

		private static void SaveXml(XmlDocument doc, Stream stream)
		{
			using var writer = XmlWriter.Create(stream, new XmlWriterSettings()
			{
				Encoding = Encoding.UTF8,
				Indent = true,
				IndentChars = "\t",
			});
			doc.Save(writer);
		}

		private void LoadGuiView(IRepositoryGuiProvider gui)
		{
			gui.LoadFrom(_configurationService.GetSectionForProviderGui(_currentProvider));
		}

		private void SaveRepositoryProvider(IRepositoryProvider provider)
		{
			provider.SaveTo(_configurationService.GetSectionForProvider(provider));
		}

		private void SaveRepositoryServiceProvider(IRepositoryServiceProvider provider)
		{
			provider.SaveTo(_configurationService.GetSectionForProvider(provider));
		}

		private void SaveGuiView(IRepositoryGuiProvider gui)
		{
			gui.SaveTo(_configurationService.GetSectionForProviderGui(_currentProvider));
		}

		private void LoadOptions()
		{
			_repositoryManagerService.LoadFrom(_configurationService.RepositoryManagerSection);
			var mainWindowNode = _configurationService.GuiSection.TryGetSection("MainWindow");
			if(mainWindowNode != null)
			{
				StartPosition = FormStartPosition.Manual;
				Bounds = mainWindowNode.GetValue("Bounds", Bounds);
				WindowState = mainWindowNode.GetValue("State", FormWindowState.Normal);
			}
			var startPageNode = _configurationService.GuiSection.TryGetSection("StartPage");
			if(startPageNode != null)
			{
				_startPageFactory.ShowOnStartup = startPageNode.GetValue("ShowOnStartup", true);
				_startPageFactory.CloseAfterRepositoryLoad = startPageNode.GetValue("CloseAfterRepositoryLoad", false);
			}
		}

		private void SaveOptions()
		{
			var state = WindowState;
			var bounds = state!=FormWindowState.Normal?RestoreBounds:Bounds;
			if(state == FormWindowState.Minimized) state = FormWindowState.Normal;

			var mainWindowNode = _configurationService.GuiSection.GetCreateSection("MainWindow");
			mainWindowNode.SetValue("Bounds", bounds);
			mainWindowNode.SetValue("State", state);
			var startPageNode = _configurationService.GuiSection.GetCreateSection("StartPage");
			startPageNode.SetValue("ShowOnStartup", _startPageFactory.ShowOnStartup);
			startPageNode.SetValue("CloseAfterRepositoryLoad", _startPageFactory.CloseAfterRepositoryLoad);

			_repositoryManagerService.SaveTo(_configurationService.RepositoryManagerSection);
			_viewDockService.SaveSettings();
		}

		private void LoadRecentRepositories()
		{
			_repositoryManagerService.RecentRepositories.Changed -= OnRecentRepositoriesChanged;
			_repositoryManagerService.RecentRepositories.Clear();
			var cfgName = "recent.xml";
			if(_configurationService.FileExists(cfgName))
			{
				try
				{
					var doc = new XmlDocument();
					using(var stream = _configurationService.OpenFile(cfgName))
					{
						doc.Load(stream);
					}
					var node = doc["Recent"];
					foreach(XmlNode repoNode in node.ChildNodes)
					{
						if(repoNode.Name == "Repository")
						{
							var path = repoNode.Attributes["Path"].Value;
							_repositoryManagerService.RecentRepositories.Add(new RepositoryLink(path, @""));
						}
					}
				}
				catch(Exception exc) when(!exc.IsCritical())
				{
				}
			}
			_repositoryManagerService.RecentRepositories.Changed += OnRecentRepositoriesChanged;
			UpdateRecentRepositoriesMenu();
		}

		private void SaveRecentRepositories()
		{
			var cfgName = "recent.xml";
			var newdoc = new XmlDocument();
			var rootnode = newdoc.AppendChild(newdoc.CreateElement("Recent"));
			for(int i = 0; i < _repositoryManagerService.RecentRepositories.Count; ++i)
			{
				rootnode.AppendChild(newdoc.CreateElement("Repository")).Attributes.Append(newdoc.CreateAttribute("Path")).Value =
					_repositoryManagerService.RecentRepositories[i].Path;
			}
			try
			{
				using var stream = _configurationService.CreateFile(cfgName);
				SaveXml(newdoc, stream);
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
			}
		}

		private void UpdateRecentRepositoriesMenu()
		{
			while(_mnuRecentRepositories.DropDownItems.Count > 0)
			{
				var index = _mnuRecentRepositories.DropDownItems.Count - 1;
				var item  = _mnuRecentRepositories.DropDownItems[index];
				_mnuRecentRepositories.DropDownItems.RemoveAt(index);
				_bindings.BindImage(item, null);
				item.Dispose();
			}
			if(_repositoryManagerService.RecentRepositories.Count == 0)
			{
				_mnuRecentRepositories.DropDownItems.Add(new ToolStripMenuItem(Resources.StrlNoAvailable.SurroundWith("<", ">"))
				{
					Enabled = false,
				});
			}
			else
			{
				foreach(var repo in _repositoryManagerService.RecentRepositories)
				{
					var item = new ToolStripMenuItem(
						repo.Path, default, OnRecentRepositoryClick)
					{
						Tag = repo,
					};
					_bindings.BindImage(item, Icons.Repository);
					_mnuRecentRepositories.DropDownItems.Add(item);
				}
			}
		}

		private void OnRecentRepositoryClick(object sender, EventArgs e)
		{
			var repo = (RepositoryLink)((ToolStripItem)sender).Tag;
			OpenRepository(repo.Path);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			foreach(var provider in _repositoryProviders.Values)
			{
				SaveRepositoryProvider(provider);
			}
			foreach(var provider in _repositoryServiceProviders.Values)
			{
				SaveRepositoryServiceProvider(provider);
			}
			if(_repositoryGui != null)
			{
				SaveGuiView(_repositoryGui);
				_repositoryGui.Repository = null;
			}
			SaveOptions();
			SaveRecentRepositories();
			if(_repository != null)
			{
				_currentProvider.CloseRepository(_repository);
			}
			base.OnClosing(e);
		}

		#region IWorkingEnvironment

		public ViewDockService ViewDockService => _viewDockService;

		public IRepositoryProvider ActiveRepositoryProvider => _currentProvider;

		public IRepository ActiveRepository => _repository;

		public IRepositoryProvider FindProviderForDirectory(string workingDirectory)
		{
			foreach(var prov in _repositoryProviders.Values)
			{
				if(prov.IsValidFor(workingDirectory))
				{
					return prov;
				}
			}
			return null;
		}

		public bool OpenRepository(string path, IRepositoryProvider repositoryProvider)
		{
			if(_currentProvider != repositoryProvider)
			{
				CloseRepository();
				_currentProvider = repositoryProvider;
			}
			else
			{
				DetachRepository();
				foreach(var gui in _additionalGui)
				{
					gui.DetachFromEnvironment(this);
				}
				_additionalGui.Clear();
				_activeIssueTrackerProviders.Clear();
			}

			try
			{
				_repository = null;
				_repository = ProgressForm.MonitorTaskAsModalWindow(
					this,
					Resources.StrLoadingRepository,
					(p, c) => _currentProvider.OpenRepositoryAsync(path, p, c));
			}
			catch(OperationCanceledException)
			{
				return false;
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToOpenRepository,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
			if(_repository == null)
			{
				return false;
			}
			_repository.Deleted += OnRepositoryDeleted;
			if(_repositoryGui != null)
			{
				_repositoryGui.Repository = _repository;
			}
			else
			{
				_repositoryGui = _currentProvider.GuiProvider;
				_repositoryGui.Repository = _repository;
				LoadGuiView(_repositoryGui);
				_repositoryGui.AttachToEnvironment(this);
			}
			_repositoryGui.ActivateDefaultView();

			Text = _repository.WorkingDirectory + " - " + Application.ProductName;

			repositoryProvider.OnRepositoryLoaded(_repository);

			OpenIssueTrackers();

			return true;
		}

		private void OnRepositoryDeleted(object sender, EventArgs e)
		{
			var repository = (IRepository)sender;
			BeginInvoke(new MethodInvoker(
				() =>
				{
					GitterApplication.MessageBoxService.Show(
						this,
						"Repository was removed externally and will be closed.",
						repository.WorkingDirectory,
						MessageBoxButton.Close,
						MessageBoxIcon.Warning);
					CloseRepository();
				}), null);
		}

		private void OnRecentRepositoriesChanged(object sender, NotifyCollectionEventArgs e)
		{
			UpdateRecentRepositoriesMenu();
		}

		private void OpenIssueTrackers()
		{
			foreach(var prov in _repositoryServiceProviders.Values)
			{
				if(prov.TryCreateGuiProvider(_repository, out var gui))
				{
					gui.AttachToEnvironment(this);
					_additionalGui.AddLast(gui);
					_activeIssueTrackerProviders.Add(prov);
				}
			}
		}

		public bool OpenRepository(string path)
		{
			return OpenRepository(path, false);
		}

		public bool OpenRepository(string path, bool allowRecursiveSearch)
		{
			path = Path.GetFullPath(path);
			if(_repository != null && _repository.WorkingDirectory == path)
			{
				_repositoryGui.ActivateDefaultView();
				return true;
			}
			try
			{
				_recentRepositoryPath = path;
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
				_recentRepositoryPath = string.Empty;
			}
			var prov = FindProviderForDirectory(path);
			if(prov == null && allowRecursiveSearch)
			{
				var di = new DirectoryInfo(path);
				if(di.Exists)
				{
					while(di.Parent != null)
					{
						di = di.Parent;
						prov = FindProviderForDirectory(di.FullName);
						if(prov != null)
						{
							path = di.FullName;
							break;
						}
					}
				}
			}
			if(prov == null)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					string.Format(Resources.ErrPathIsNotValidRepository, path),
					Resources.ErrFailedToOpenRepository,
					MessageBoxButton.Close,
					MessageBoxIcon.Information);
				return false;
			}
			else
			{
				var wasOpened = OpenRepository(path, prov);
				if(wasOpened)
				{
					RepositoryManagerService.RegisterRecentRepository(path);
					_repositoryExplorerFactory.RootItem.RepositoryDisplayName =
						Path.GetFileName(path.EndsWithOneOf(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) ?
										 path.Substring(0, path.Length - 1) : path);
				}
				return wasOpened;
			}
		}

		public void CloseRepository()
		{
			if(_repositoryGui != null)
			{
				SaveGuiView(_repositoryGui);
				_repositoryGui.DetachFromEnvironment(this);
				(_repositoryGui as IDisposable)?.Dispose();
				_repositoryGui = null;
			}
			if(_currentProvider != null)
			{
				DetachRepository();
				_currentProvider = null;
			}
			_repositoryExplorerFactory.RootItem.RepositoryDisplayName = null;
			foreach(var gui in _additionalGui)
			{
				gui.DetachFromEnvironment(this);
				(gui as IDisposable)?.Dispose();
			}
			_additionalGui.Clear();
			_activeIssueTrackerProviders.Clear();

			Text = Application.ProductName;
		}

		private void DetachRepository()
		{
			if(_repository != null)
			{
				_repository.Deleted -= OnRepositoryDeleted;
				//var layout = new ViewLayout(_viewDockService);
				//layout.SaveTo(_repository.ConfigSection.GetCreateEmptySection("Layout"));
				_currentProvider.CloseRepository(_repository);
				_repository = null;
			}
		}

		public void ProvideMainMenuItem(ToolStripMenuItem item)
		{
			Verify.Argument.IsNotNull(item, nameof(item));

			_menuStrip.Items.Insert(_menuStrip.Items.IndexOf(_mnuTools), item);
		}

		public void ProvideViewMenuItem(ToolStripMenuItem item)
		{
			Verify.Argument.IsNotNull(item, nameof(item));

			_mnuView.DropDownItems.Insert(_mnuView.DropDownItems.Count - 4, item);
			if(_mnuView.DropDownItems.Count == 7)
			{
				_mnuView.DropDownItems.Insert(2, new ToolStripSeparator());
			}
		}

		public void ProvideRepositoryExplorerItem(CustomListBoxItem item)
		{
			Verify.Argument.IsNotNull(item, nameof(item));

			_repositoryExplorerFactory.AddItem(item);
		}

		public void ProvideToolbar(ToolStrip toolStrip)
		{
			Verify.Argument.IsNotNull(toolStrip, nameof(toolStrip));

			if(_toolStripContainer.TopToolStripPanel.Rows.Length > 1)
			{
				var row = _toolStripContainer.TopToolStripPanel.Rows[1];
				int xOffset = 0;
				var yOffset = 0;
				foreach(var c in row.Controls)
				{
					if(c.Right > xOffset)
					{
						xOffset = c.Right;
					}
					if(c.Top > yOffset)
					{
						yOffset = c.Top;
					}
				}
				var p = new Point(xOffset, yOffset);
				_toolStripContainer.TopToolStripPanel.Join(
					toolStrip, xOffset, yOffset);
			}
			else
			{
				_toolStripContainer.TopToolStripPanel.Join(
					toolStrip, 1);
			}

			_mnuToolbars.DropDownItems.Add(
				new ToolStripMenuItem(
					toolStrip.Text,
					null,
					(sender, e) =>
					{
						var item = ((ToolStripMenuItem)sender);
						var strip = (ToolStrip)item.Tag;
						strip.Visible = !strip.Visible;
						item.Checked = strip.Visible;
					})
			{
				Checked = true,
				Tag = toolStrip,
			});
			if(_mnuToolbars.DropDownItems.Count == 1)
			{
				_mnuToolbars.Enabled = true;
			}
		}

		public void ProvideStatusBarObject(ToolStripItem item, bool leftAlign)
		{
			Verify.Argument.IsNotNull(item, nameof(item));

			if(leftAlign)
			{
				var index = _statusStrip.Items.IndexOf(_statusSeparator);
				_statusStrip.Items.Insert(index, item);
			}
			else
			{
				item.Alignment = ToolStripItemAlignment.Right;
				var index = _statusStrip.Items.IndexOf(_statusSeparator);
				_statusStrip.Items.Insert(index + 1, item);
			}
		}

		public void RemoveMainMenuItem(ToolStripMenuItem item)
		{
			Verify.Argument.IsNotNull(item, nameof(item));

			_menuStrip.Items.Remove(item);
		}

		public void RemoveViewMenuItem(ToolStripMenuItem item)
		{
			Verify.Argument.IsNotNull(item, nameof(item));

			_mnuView.DropDownItems.Remove(item);
			if(_mnuView.DropDownItems.Count == 8)
			{
				_mnuView.DropDownItems.RemoveAt(2);
			}
		}

		public void RemoveRepositoryExplorerItem(CustomListBoxItem item)
		{
			Verify.Argument.IsNotNull(item, nameof(item));

			_repositoryExplorerFactory.RemoveItem(item);
		}

		public void RemoveToolbar(ToolStrip toolStrip)
		{
			Verify.Argument.IsNotNull(toolStrip, nameof(toolStrip));

			toolStrip.Parent = null;
			foreach(ToolStripItem item in _mnuToolbars.DropDownItems)
			{
				if(item.Tag == toolStrip)
				{
					_mnuToolbars.DropDownItems.Remove(item);
					break;
				}
			}
			if(_mnuToolbars.DropDownItems.Count == 0)
			{
				_mnuToolbars.Enabled = false;
			}
		}

		public void RemoveStatusBarObject(ToolStripItem item)
		{
			Verify.Argument.IsNotNull(item, nameof(item));

			_statusStrip.Items.Remove(item);
		}

		Form IWorkingEnvironment.MainForm => this;

		DpiBindings IWorkingEnvironment.MainFormDpiBindings => _bindings;

		#endregion
	}
}
