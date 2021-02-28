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
	using System.Drawing;
	using System.Globalization;
	using System.IO;
	using System.Windows.Forms;

	using gitter.Controls;
	using gitter.Framework;
	using gitter.Framework.Configuration;
	using gitter.Framework.Controls;
	using gitter.Framework.Services;

	using Resources = gitter.Properties.Resources;

	internal partial class StartPageView : ViewBase
	{
		#region Data

		private readonly StartPageViewFactory _factory;
		private readonly NotifyCollectionBinding<RepositoryLink> _recentRepositoriesBinding;
		private readonly ICheckBoxWidget _chkShowPageAtStartup;
		private readonly ICheckBoxWidget _chkClosePageAfterRepositoryLoad;
		private readonly List<RepositoryListItem> _repositories;

		#endregion

		#region .ctor

		public StartPageView(IWorkingEnvironment environment, StartPageViewFactory factory)
			: base(Guids.StartPageView, environment)
		{
			Verify.Argument.IsNotNull(factory, nameof(factory));
			
			InitializeComponent();

			_picLogo.Image = GetLogo();
			_picLogo2.Image = GetGradient();

			_factory = factory;
			_repositories = new List<RepositoryListItem>();
			_lstLocalRepositories.FullList = _repositories;

			Text = Resources.StrStartPage;

			_txtFilter.BackColor     = GitterApplication.Style.Colors.Window;
			_txtFilter.HintForeColor = GitterApplication.Style.Colors.GrayText;
			_txtFilter.TextForeColor = GitterApplication.Style.Colors.WindowText;

			_lstLocalRepositories.SizeChanged += (s, e) =>
			{
				var x = _lstLocalRepositories.Width + _lstLocalRepositories.Left - _txtFilter.Width;
				if(x > _lblLocalRepositories.Left + _lblLocalRepositories.Width)
				{
					_txtFilter.Left = x;
					_txtFilter.Visible = true;
				}
				else
				{
					_txtFilter.Visible = false;
				}
			};

			_txtFilter.TextChanged += OnFilterTextChanged;

			_lstLocalRepositories.ItemActivated += OnLocalRepositoriesListItemActivated;
			_lstRecentRepositories.ItemActivated += OnRecentRepositoriesListItemActivated;

			_lstLocalRepositories.DragEnter += OnLocalRepositoriesDragEnter;
			_lstLocalRepositories.DragDrop += OnLocalRepositoriesDragDrop;

			_lstLocalRepositories.KeyDown += OnLocalRepositoriesKeyDown;
			_lstRecentRepositories.KeyDown += OnRecentRepositoriesKeyDown;

			_chkClosePageAfterRepositoryLoad = GitterApplication.Style.CreateCheckBox();
			_chkClosePageAfterRepositoryLoad.Text = Resources.StrsClosePageAfterRepositoryLoad;
			_chkClosePageAfterRepositoryLoad.Control.Bounds = new Rectangle(9, 491, 199, 20);
			_chkClosePageAfterRepositoryLoad.Control.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
			_chkClosePageAfterRepositoryLoad.Control.Parent = this;
			_chkClosePageAfterRepositoryLoad.IsChecked = _factory.CloseAfterRepositoryLoad;
			_chkClosePageAfterRepositoryLoad.IsCheckedChanged += _chkClosePageAfterRepositoryLoad_CheckedChanged;

			_chkShowPageAtStartup = GitterApplication.Style.CreateCheckBox();
			_chkShowPageAtStartup.Text = Resources.StrsShowPageOnStartup;
			_chkShowPageAtStartup.Control.Bounds = new Rectangle(9, 511, 199, 20);
			_chkShowPageAtStartup.Control.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
			_chkShowPageAtStartup.Control.Parent = this;
			_chkShowPageAtStartup.IsChecked = _factory.ShowOnStartup;
			_chkShowPageAtStartup.IsCheckedChanged += _chkShowPageAtStartup_CheckedChanged;

			_separator1.BackColor = GitterApplication.Style.Colors.Separator;
			_separator2.BackColor = GitterApplication.Style.Colors.Separator;

			_recentRepositoriesBinding = new NotifyCollectionBinding<RepositoryLink>(
				_lstRecentRepositories.Items,
				WorkingEnvironment.RepositoryManagerService.RecentRepositories,
				repo => new RecentRepositoryListItem(repo));
		}

		#endregion

		#region Properties

		public override bool IsDocument => true;

		public override Image Image => CachedResources.Bitmaps["ImgStartPage"];

		#endregion

		#region Methods

		private static Bitmap GetLogo() => GitterApplication.Style.Type == GitterStyleType.DarkBackground
			? Resources.ImgStartPageLogoDark
			: Resources.ImgStartPageLogo;

		private static Bitmap GetGradient() => GitterApplication.Style.Type == GitterStyleType.DarkBackground
			? Resources.ImgStartPageLogoGradientDark
			: Resources.ImgStartPageLogoGradient;

		private void OnLocalRepositoriesKeyDown(object sender, KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Delete:
					while(_lstLocalRepositories.SelectedItems.Count != 0)
					{
						var item = (RepositoryListItem)_lstLocalRepositories.SelectedItems[0];
						_repositories.Remove(item);
						item.Remove();
					}
					break;
			}
		}

		private void OnRecentRepositoriesKeyDown(object sender, KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Delete:
					while(_lstRecentRepositories.SelectedItems.Count != 0)
					{
						var item = (RecentRepositoryListItem)_lstRecentRepositories.SelectedItems[0];
						WorkingEnvironment.RepositoryManagerService.RecentRepositories.Remove(item.DataContext);
						if(item.ListBox != null)
						{
							item.Remove();
						}
					}
					break;
			}
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
			else if(e.Data.GetDataPresent(typeof(RepositoryListItem)))
			{
				if(string.IsNullOrWhiteSpace(_txtFilter.Text))
				{
					e.Effect = DragDropEffects.Move;
				}
			}
			else if(e.Data.GetDataPresent(typeof(RecentRepositoryListItem)))
			{
				var data = (RecentRepositoryListItem)e.Data.GetData(typeof(RecentRepositoryListItem));
				if(!IsPresentInLocalRepositoryList(data.DataContext.Path))
				{
					e.Effect = DragDropEffects.Copy;
				}
			}
		}

		private bool IsPresentInLocalRepositoryList(string path)
		{
			foreach(var item in _repositories)
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
						var di = new DirectoryInfo(data[i]);
						if(di.Exists)
						{
							var path = di.FullName;
							if(!IsPresentInLocalRepositoryList(path))
							{
								var provider = WorkingEnvironment.FindProviderForDirectory(data[i]);
								if(provider != null)
								{
									var link = new RepositoryLink(path, provider.Name);
									var point = _lstLocalRepositories.PointToClient(new Point(e.X, e.Y));
									var index = _lstLocalRepositories.GetInsertIndexFormPoint(point.X, point.Y, false, out var itemsCollection);
									if(index != -1)
									{
										var itemToInsert = new RepositoryListItem(link);
										InsertRepositoryItem(index, itemsCollection, itemToInsert);
									}
								}
							}
						}
					}
				}
				else if(e.Data.GetDataPresent(typeof(RepositoryListItem)))
				{
					if(!string.IsNullOrWhiteSpace(_txtFilter.Text))
					{
						return;
					}
					var itemToMove = (RepositoryListItem)e.Data.GetData(typeof(RepositoryListItem));
					var point = _lstLocalRepositories.PointToClient(new Point(e.X, e.Y));
					var index = _lstLocalRepositories.GetInsertIndexFormPoint(point.X, point.Y, false, out var itemsCollection);
					if(index == -1) return;
					var currentIndex = _lstLocalRepositories.Items.IndexOf(itemToMove);
					if(index == currentIndex) return;
					if(currentIndex == -1)
					{
						_repositories.Insert(index, itemToMove);
						itemsCollection.Insert(index, itemToMove);
					}
					else
					{
						if(index > _lstLocalRepositories.Items.Count - 1)
						{
							--index;
						}
						_repositories.Remove(itemToMove);
						itemToMove.Remove();
						_repositories.Insert(index, itemToMove);
						itemsCollection.Insert(index, itemToMove);
					}
				}
				else if(e.Data.GetDataPresent(typeof(RecentRepositoryListItem)))
				{
					var itemToMove = (RecentRepositoryListItem)e.Data.GetData(typeof(RecentRepositoryListItem));
					var path = itemToMove.DataContext.Path;
					if(IsPresentInLocalRepositoryList(path)) return;
					var point = _lstLocalRepositories.PointToClient(new Point(e.X, e.Y));
					var index = _lstLocalRepositories.GetInsertIndexFormPoint(point.X, point.Y, false, out var itemsCollection);
					if(index != -1)
					{
						var itemToInsert = new RepositoryListItem(new RepositoryLink(path, string.Empty));
						InsertRepositoryItem(index, itemsCollection, itemToInsert);
					}
				}
			}
		}

		private void InsertRepositoryItem(int index, CustomListBoxItemsCollection target, RepositoryListItem itemToInsert)
		{
			if(!string.IsNullOrWhiteSpace(_txtFilter.Text))
			{
				if(index == 0)
				{
					_repositories.Insert(0, itemToInsert);
				}
				else
				{
					var dst = _repositories.IndexOf((RepositoryListItem)_lstLocalRepositories.Items[index - 1]);
					_repositories.Insert(dst + 1, itemToInsert);
				}
				if(FilterItem(itemToInsert))
				{
					target.Insert(index, itemToInsert);
				}
			}
			else
			{
				_repositories.Insert(index, itemToInsert);
				target.Insert(index, itemToInsert);
			}
		}

		private void OnLocalRepositoriesListItemActivated(object sender, ItemEventArgs e)
		{
			if(e.Item is RepositoryListItem item)
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
			if(e.Item is RecentRepositoryListItem item)
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

		protected override void SaveMoreViewTo(Section section)
		{
			base.SaveMoreViewTo(section);
			var listNode = section.GetCreateSection("RepositoryList");
			_lstLocalRepositories.SaveViewTo(listNode);
			var itemsNode = listNode.GetCreateEmptySection("Items");
			int id = 0;
			foreach(var item in _repositories)
			{
				var link = item.DataContext;
				link.SaveTo(itemsNode.GetCreateSection("Repository" + id.ToString(CultureInfo.InvariantCulture)));
				++id;
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
				_repositories.Clear();
				var itemsNode = listNode.TryGetSection("Items");
				if(itemsNode != null)
				{
					foreach(var s in itemsNode.Sections)
					{
						var link = new RepositoryLink(s);
						var item = new RepositoryListItem(link);
						_repositories.Add(item);
						_lstLocalRepositories.Items.Add(item);
					}
				}
				_lstLocalRepositories.EndUpdate();
			}
		}

		public LocalRepositoriesListBox Repositories => _lstLocalRepositories;

		private void _btnAddLocalRepo_LinkClicked(object sender, EventArgs e)
		{
			var path = Utility.ShowPickFolderDialog(this);
			if(!string.IsNullOrWhiteSpace(path))
			{
				var prov = WorkingEnvironment.FindProviderForDirectory(path);
				if(prov == null)
				{
					GitterApplication.MessageBoxService.Show(
						this,
						Resources.ErrPathIsNotValidRepository.UseAsFormat(path),
						Resources.ErrInvalidPath,
						MessageBoxButton.Close,
						MessageBoxIcon.Error);
					return;
				}
				var item = new RepositoryListItem(new RepositoryLink(path, prov.Name));
				_repositories.Add(item);
				_lstLocalRepositories.Items.Add(item);
			}
		}

		private void _btnInitLocalRepo_LinkClicked(object sender, EventArgs e)
		{
			using var dlg = new InitRepositoryDialog(WorkingEnvironment);
			dlg.Run(WorkingEnvironment.MainForm);
		}

		private void _btnCloneRemoteRepo_LinkClicked(object sender, EventArgs e)
		{
			using var dlg = new CloneRepositoryDialog(WorkingEnvironment);
			dlg.Run(WorkingEnvironment.MainForm);
		}

		private void _chkClosePageAfterRepositoryLoad_CheckedChanged(object sender, EventArgs e)
		{
			_factory.CloseAfterRepositoryLoad = _chkClosePageAfterRepositoryLoad.IsChecked;
		}

		private void _chkShowPageAtStartup_CheckedChanged(object sender, EventArgs e)
		{
			_factory.ShowOnStartup = _chkShowPageAtStartup.IsChecked;
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

		private bool FilterItem(RepositoryListItem item) => FilterItem(item, _txtFilter.Text);

		private static bool FilterItem(RepositoryListItem item, string filter)
		{
			if(string.IsNullOrWhiteSpace(filter)) return true;
			var index = item.DataContext.Path.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase);
			return index >= 0;
		}

		private void RefillLocalRepositories()
		{
			var filter = _txtFilter.Text;
			_lstLocalRepositories.BeginUpdate();
			int index1 = 0;
			for(int index = 0; index < _repositories.Count; ++index)
			{
				var item = _repositories[index];
				if(FilterItem(item, filter))
				{
					if(!item.IsAttachedToListBox)
					{
						_lstLocalRepositories.Items.Insert(index1, item);
					}
					++index1;
				}
				else
				{
					if(item.IsAttachedToListBox)
					{
						item.Remove();
					}
				}
			}
			_lstLocalRepositories.EndUpdate();
		}

		private void OnFilterTextChanged(object sender, EventArgs e)
		{
			RefillLocalRepositories();
		}

		#endregion
	}
}
