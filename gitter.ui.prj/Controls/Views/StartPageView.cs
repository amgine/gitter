#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
		private ICheckBoxWidget _chkShowPageAtStartup;
		private ICheckBoxWidget _chkClosePageAfterRepositoryLoad;

		public StartPageView(IWorkingEnvironment environment, IDictionary<string, object> parameters, StartPageViewFactory factory)
			: base(Guids.StartPageView, environment, parameters)
		{
			Verify.Argument.IsNotNull(factory, "factory");
			
			InitializeComponent();

			_picLogo.Image = GetLogo();
			_picLogo2.Image = GetGradient();

			_factory = factory;

			Text = Resources.StrStartPage;

			_lstLocalRepositories.ItemActivated += OnLocalRepositoriesListItemActivated;
			_lstRecentRepositories.ItemActivated += OnRecentRepositoriesListItemActivated;

			_lstLocalRepositories.DragEnter += OnLocalRepositoriesDragEnter;
			_lstLocalRepositories.DragDrop += OnLocalRepositoriesDragDrop;

			_lstLocalRepositories.KeyDown += OnLocalRepositoriesKeyDown;
			_lstRecentRepositories.KeyDown += OnRecentRepositoriesKeyDown;

			_chkClosePageAfterRepositoryLoad = GitterApplication.Style.CreateCheckBox();
			_chkClosePageAfterRepositoryLoad.Text = "Close page after repository load";
			_chkClosePageAfterRepositoryLoad.Control.Bounds = new Rectangle(9, 491, 199, 20);
			_chkClosePageAfterRepositoryLoad.Control.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
			_chkClosePageAfterRepositoryLoad.Control.Parent = this;
			_chkClosePageAfterRepositoryLoad.IsChecked = _factory.CloseAfterRepositoryLoad;
			_chkClosePageAfterRepositoryLoad.IsCheckedChanged += _chkClosePageAfterRepositoryLoad_CheckedChanged;

			_chkShowPageAtStartup = GitterApplication.Style.CreateCheckBox();
			_chkShowPageAtStartup.Text = "Show page on startup";
			_chkShowPageAtStartup.Control.Bounds = new Rectangle(9, 511, 199, 20);
			_chkShowPageAtStartup.Control.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
			_chkShowPageAtStartup.Control.Parent = this;
			_chkShowPageAtStartup.IsChecked = _factory.ShowOnStartup;
			_chkShowPageAtStartup.IsCheckedChanged += _chkShowPageAtStartup_CheckedChanged;

			_separator1.BackColor = GitterApplication.Style.Colors.Separator;
			_separator2.BackColor = GitterApplication.Style.Colors.Separator;

			_recentRepositoriesBinding = new NotifyCollectionBinding<string>(
				_lstRecentRepositories.Items,
				WorkingEnvironment.RecentRepositories,
				repo => new RecentRepositoryListItem(repo));
		}

		private static Bitmap GetLogo()
		{
			if(GitterApplication.Style.Type == GitterStyleType.DarkBackground)
			{
				return Resources.ImgStartPageLogoDark;
			}
			else
			{
				return Resources.ImgStartPageLogo;
			}
		}

		private static Bitmap GetGradient()
		{
			if(GitterApplication.Style.Type == GitterStyleType.DarkBackground)
			{
				return Resources.ImgStartPageLogoGradientDark;
			}
			else
			{
				return Resources.ImgStartPageLogoGradient;
			}
		}

		private void OnLocalRepositoriesKeyDown(object sender, KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Delete:
					while(_lstLocalRepositories.SelectedItems.Count != 0)
					{
						_lstLocalRepositories.SelectedItems[0].Remove();
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
						WorkingEnvironment.RecentRepositories.Remove(item.DataContext);
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
				e.Effect = DragDropEffects.Move;
			}
			else if(e.Data.GetDataPresent(typeof(RecentRepositoryListItem)))
			{
				var data = (RecentRepositoryListItem)e.Data.GetData(typeof(RecentRepositoryListItem));
				if(!IsPresentInLocalRepositoryList(data.DataContext))
				{
					e.Effect = DragDropEffects.Copy;
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
									var point = _lstLocalRepositories.PointToClient(new Point(e.X, e.Y));
									CustomListBoxItemsCollection itemsCollection;
									var index = _lstLocalRepositories.GetInsertIndexFormPoint(point.X, point.Y, false, out itemsCollection);
									if(index != -1)
									{
										itemsCollection.Insert(index, new RepositoryListItem(link));
									}
								}
							}
						}
					}
				}
				else if(e.Data.GetDataPresent(typeof(RepositoryListItem)))
				{
					var data = (RepositoryListItem)e.Data.GetData(typeof(RepositoryListItem));
					var point = _lstLocalRepositories.PointToClient(new Point(e.X, e.Y));
					CustomListBoxItemsCollection itemsCollection;
					var index = _lstLocalRepositories.GetInsertIndexFormPoint(point.X, point.Y, false, out itemsCollection);
					if(index == -1) return;
					var currentIndex = _lstLocalRepositories.Items.IndexOf(data);
					if(index == currentIndex) return;
					if(currentIndex == -1)
					{
						itemsCollection.Insert(index, data);
					}
					else
					{
						if(index > _lstLocalRepositories.Items.Count - 1)
						{
							--index;
						}
						data.Remove();
						itemsCollection.Insert(index, data);
					}
				}
				else if(e.Data.GetDataPresent(typeof(RecentRepositoryListItem)))
				{
					var data = (RecentRepositoryListItem)e.Data.GetData(typeof(RecentRepositoryListItem));
					var path = data.DataContext;
					if(IsPresentInLocalRepositoryList(path)) return;
					var point = _lstLocalRepositories.PointToClient(new Point(e.X, e.Y));
					CustomListBoxItemsCollection itemsCollection;
					var index = _lstLocalRepositories.GetInsertIndexFormPoint(point.X, point.Y, false, out itemsCollection);
					if(index == -1) return;
					var item = new RepositoryListItem(new RepositoryLink(path, string.Empty));
					itemsCollection.Insert(index, item);
				}
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
				_lstLocalRepositories.Items.Add(item);
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
	}
}
