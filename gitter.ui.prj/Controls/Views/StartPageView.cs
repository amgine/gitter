namespace gitter
{
	using System;
	using System.Linq;
	using System.IO;
	using System.Globalization;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Services;
	using gitter.Framework.Configuration;

	using Resources = gitter.Properties.Resources;

	internal partial class StartPageView : ViewBase
	{
		private readonly StartPageViewFactory _factory;
		private readonly NotifyCollectionBinding<string> _recentRepositoriesBinding;

		public StartPageView(IWorkingEnvironment environment, IDictionary<string, object> parameters, StartPageViewFactory factory)
			: base(Guids.StartPageView, environment, parameters)
		{
			Verify.Argument.IsNotNull(factory, "factory");
			
			InitializeComponent();

			_factory = factory;

			_chkShowPageAtStartup.Checked = _factory.ShowOnStartup;
			_chkClosePageAfterRepositoryLoad.Checked = _factory.CloseAfterRepositoryLoad;

			Text = Resources.StrStartPage;

			_lstLocalRepositories.ItemActivated += OnLocalRepositoriesListItemActivated;
			_lstRecentRepositories.ItemActivated += OnRecentRepositoriesListItemActivated;

			_lstLocalRepositories.DragEnter += OnLocalRepositoriesDragEnter;
			_lstLocalRepositories.DragDrop += OnLocalRepositoriesDragDrop;

			_recentRepositoriesBinding = new NotifyCollectionBinding<string>(
				_lstRecentRepositories.Items,
				WorkingEnvironment.RecentRepositories,
				repo => new RecentRepositoryListItem(repo));
		}

		private void OnLocalRepositoriesDragEnter(object sender, DragEventArgs e)
		{
			if(e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				var data = (string[])(e.Data.GetData(DataFormats.FileDrop));
				for(int i = 0; i < data.Length; ++i)
				{
					if(Directory.Exists(data[i]))
					{
						e.Effect = DragDropEffects.Link;
					}
				}
			}
		}

		private bool IsPresentInLocalRepositoryList(string path)
		{
			foreach(RepositoryListItem item in _lstLocalRepositories.Items)
			{
				if(item.DataContext.Path == path)
				{
					return true;
				}
			}
			return false;
		}

		private void OnLocalRepositoriesDragDrop(object sender, DragEventArgs e)
		{
			if(e.Effect != DragDropEffects.None)
			{
				if(e.Data.GetDataPresent(DataFormats.FileDrop))
				{
					var data = (string[])(e.Data.GetData(DataFormats.FileDrop));
					for(int i = 0; i < data.Length; ++i)
					{
						var di =  new DirectoryInfo(data[i]);
						if(di.Exists)
						{
							var path = di.FullName;
							if(!IsPresentInLocalRepositoryList(path))
							{
								var provider = WorkingEnvironment.FindProviderForDirectory(data[i]);
								if(provider != null)
								{
									var link = new RepositoryLink(path, provider.Name);
									_lstLocalRepositories.Items.Add(new RepositoryListItem(link));
								}
							}
						}
					}
				}
				/*
				else if(e.Data.GetDataPresent(typeof(RepositoryLink)))
				{
					var data = (RepositoryLink)e.Data.GetData(typeof(RepositoryLink));

				}
				*/
			}
		}

		public override bool IsDocument
		{
			get { return true; }
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgStartPage"]; }
		}

		private void OnLocalRepositoriesListItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as RepositoryListItem;
			if(item != null)
			{
				if(WorkingEnvironment.OpenRepository(item.DataContext.Path))
				{
					if(_factory.CloseAfterRepositoryLoad)
					{
						Close();
					}
				}
			}
		}

		private void OnRecentRepositoriesListItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as RecentRepositoryListItem;
			if(item != null)
			{
				if(WorkingEnvironment.OpenRepository(item.DataContext))
				{
					if(_factory.CloseAfterRepositoryLoad)
					{
						Close();
					}
				}
			}
		}

		protected override void SaveMoreViewTo(Section section)
		{
			base.SaveMoreViewTo(section);
			var listNode = section.GetCreateSection("RepositoryList");
			_lstLocalRepositories.SaveViewTo(listNode);
			var itemsNode = listNode.GetCreateEmptySection("Items");
			int id = 0;
			foreach(var item in _lstLocalRepositories.Items)
			{
				var repoItem = item as RepositoryListItem;
				if(repoItem != null)
				{
					var link = repoItem.DataContext;
					link.SaveTo(itemsNode.GetCreateSection("Repository" + id.ToString(CultureInfo.InvariantCulture)));
					++id;
				}
			}
		}

		protected override void LoadMoreViewFrom(Section section)
		{
			base.LoadMoreViewFrom(section);
			var listNode = section.TryGetSection("RepositoryList");
			if(listNode != null)
			{
				_lstLocalRepositories.LoadViewFrom(listNode);
				_lstLocalRepositories.BeginUpdate();
				_lstLocalRepositories.Items.Clear();
				_lstLocalRepositories.LoadViewFrom(listNode);
				var itemsNode = listNode.TryGetSection("Items");
				if(itemsNode != null)
				{
					foreach(var s in itemsNode.Sections)
					{
						var link = new RepositoryLink(s);
						var item = new RepositoryListItem(link);
						_lstLocalRepositories.Items.Add(item);
					}
				}
				_lstLocalRepositories.EndUpdate();
			}
		}

		public LocalRepositoriesListBox Repositories
		{
			get { return _lstLocalRepositories; }
		}

		private void _btnAddLocalRepo_LinkClicked(object sender, EventArgs e)
		{
			using(var dlg = new AddRepositoryDialog(WorkingEnvironment, Repositories))
			{
				dlg.Run(this);
			}
		}

		private void _btnInitLocalRepo_LinkClicked(object sender, EventArgs e)
		{
			var provider = WorkingEnvironment.RepositoryProviders
											 .OfType<gitter.Git.IGitRepositoryProvider>()
											 .FirstOrDefault();
			if(provider != null)
			{
				provider.RunInitDialog();
			}
		}

		private void _btnCloneRemoteRepo_LinkClicked(object sender, EventArgs e)
		{
			var provider = WorkingEnvironment.RepositoryProviders
											 .OfType<gitter.Git.IGitRepositoryProvider>()
											 .FirstOrDefault();
			if(provider != null)
			{
				provider.RunCloneDialog();
			}
		}

		private void _chkClosePageAfterRepositoryLoad_CheckedChanged(object sender, EventArgs e)
		{
			_factory.CloseAfterRepositoryLoad = _chkClosePageAfterRepositoryLoad.Checked;
		}

		private void _chkShowPageAtStartup_CheckedChanged(object sender, EventArgs e)
		{
			_factory.ShowOnStartup = _chkShowPageAtStartup.Checked;
		}

		private void OnLogoClick(object sender, EventArgs e)
		{
			Focus();
		}

		private void _btnScanLocalRepo_LinkClicked(object sender, EventArgs e)
		{
			//var x = new NotificationForm();
			//var n = new gitter.Git.Gui.FetchNotification();
			//x.ClientSize = n.Size;
			//n.Parent = x;
			//x.Show();
		}
	}
}
