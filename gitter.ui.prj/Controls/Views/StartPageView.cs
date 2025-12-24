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

namespace gitter;

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
using gitter.Framework.Layout;
using gitter.Framework.Services;

using Resources = gitter.Properties.Resources;

[System.ComponentModel.DesignerCategory("")]
internal partial class StartPageView : ViewBase
{
	private readonly struct ViewControls
	{
		public readonly LocalRepositoriesListBox _lstLocalRepositories;
		public readonly RecentRepositoriesListBox _lstRecentRepositories;
		public readonly LinkButton _btnAddLocalRepo;
		public readonly LinkButton _btnInitLocalRepo;
		public readonly LinkButton _btnCloneRemoteRepo;
		public readonly Panel _separator1;
		public readonly LogoControl _logo;
		public readonly HintTextBox _txtFilter;
		public readonly GroupSeparator _lblRecentRepositories;
		public readonly LabelControl _lblLocalRepositories;
		public readonly ICheckBoxWidget _chkShowPageAtStartup;
		public readonly ICheckBoxWidget _chkClosePageAfterRepositoryLoad;

		public ViewControls(IGitterStyle? style)
		{
			style ??= GitterApplication.Style;

			_lstLocalRepositories = new()
			{
				AllowDrop   = true,
				BorderStyle = BorderStyle.None,
				HeaderStyle = HeaderStyle.Hidden,
				Text        = "No repositories found",
			};
			_lstRecentRepositories = new()
			{
				BorderStyle = BorderStyle.None,
				HeaderStyle = HeaderStyle.Hidden,
			};

			_btnAddLocalRepo = new();
			_btnInitLocalRepo = new();
			_btnCloneRemoteRepo = new();

			_btnAddLocalRepo.Text = "Add Local Repository...";
			_btnInitLocalRepo.Text = "Init Local Repository...";
			_btnCloneRemoteRepo.Text = "Clone Remote Repository...";

			_separator1 = new() { BackColor = style.Colors.Separator };
			_logo = new();
			_txtFilter = new()
			{
				HintForeColor = style.Colors.GrayText,
				Hint = "filter",
			};
			var headerFont = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			_lblRecentRepositories = new()
			{
				Style = style,
				SeparatorColor = style.Colors.Separator,
				Font = headerFont,
				Text = "Recent repositories",
			};
			_lblLocalRepositories = new()
			{
				Font = headerFont,
				Text = "Local repositories",
			};
			_chkClosePageAfterRepositoryLoad = style.CheckBoxFactory.Create();
			_chkClosePageAfterRepositoryLoad.Text = Resources.StrsClosePageAfterRepositoryLoad;
			_chkShowPageAtStartup = style.CheckBoxFactory.Create();
			_chkShowPageAtStartup.Text = Resources.StrsShowPageOnStartup;
		}

		public void Layout(Control parent)
		{
			var decFilter = new TextBoxDecorator(_txtFilter);

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					rows:
					[
						SizeSpec.Absolute(90),
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_logo, marginOverride: LayoutConstants.NoMargin), row: 0),
						new GridContent(new Grid(
							columns:
							[
								SizeSpec.Absolute(2),
								SizeSpec.Absolute(225),
								SizeSpec.Absolute(2),
								SizeSpec.Absolute(DpiBoundValue.ScaleX(1)),
								SizeSpec.Absolute(2),
								SizeSpec.Everything(),
								SizeSpec.Absolute(2),
							],
							content:
							[
								new GridContent(new Grid(
									padding: DpiBoundValue.Padding(new(0, 26, 0, 4)),
									rows:
									[
										SizeSpec.Absolute(30),
										SizeSpec.Absolute(30),
										SizeSpec.Absolute(30),
										SizeSpec.Absolute(12),
										SizeSpec.Absolute(28),
										SizeSpec.Everything(),
										LayoutConstants.CheckBoxRowHeight,
										LayoutConstants.CheckBoxRowHeight,
									],
									content:
									[
										new GridContent(new ControlContent(_btnAddLocalRepo, sizeOverride: DpiBoundValue.Size(new(0, 28)), marginOverride: DpiBoundValue.Padding(new(20, 0, 0, 0)), verticalContentAlignment: VerticalContentAlignment.Center), row: 0),
										new GridContent(new ControlContent(_btnInitLocalRepo, sizeOverride: DpiBoundValue.Size(new(0, 28)), marginOverride: DpiBoundValue.Padding(new(20, 0, 0, 0)), verticalContentAlignment: VerticalContentAlignment.Center), row: 1),
										new GridContent(new ControlContent(_btnCloneRemoteRepo, sizeOverride: DpiBoundValue.Size(new(0, 28)), marginOverride: DpiBoundValue.Padding(new(20, 0, 0, 0)), verticalContentAlignment: VerticalContentAlignment.Center), row: 2),
										new GridContent(new ControlContent(_lblRecentRepositories, marginOverride: LayoutConstants.NoMargin), row: 4),
										new GridContent(new ControlContent(_lstRecentRepositories, marginOverride: LayoutConstants.NoMargin), row: 5),
										new GridContent(new WidgetContent(_chkClosePageAfterRepositoryLoad, marginOverride: DpiBoundValue.Padding(new(4, 0, 0, 0))), row: 6),
										new GridContent(new WidgetContent(_chkShowPageAtStartup, marginOverride: DpiBoundValue.Padding(new(4, 0, 0, 0))), row: 7),
									]), column: 1),
								new GridContent(new ControlContent(_separator1, marginOverride: DpiBoundValue.Padding(new(0, 10, 0, 10))), column: 3),
								new GridContent(new Grid(
									columns:
									[
										SizeSpec.Everything(),
										SizeSpec.Absolute(4),
										SizeSpec.Absolute(188).CollapseIfTooSmall(),
									],
									rows:
									[
										SizeSpec.Absolute(28),
										SizeSpec.Everything(),
									],
									content:
									[
										new GridContent(new ControlContent(_lblLocalRepositories, marginOverride: LayoutConstants.NoMargin), row: 0, column: 0),
										new GridContent(new ControlContent(decFilter, marginOverride: DpiBoundValue.Padding(new(0, 2, 0, 2))), row: 0, column: 2),
										new GridContent(new ControlContent(_lstLocalRepositories, marginOverride: LayoutConstants.NoMargin), row: 1, columnSpan: 3),
									]), column: 5),
							]), row: 1),
					]),
			};

			var dpiBindings = new DpiBindings(parent);
			dpiBindings.BindImage(_btnAddLocalRepo,    Icons.RepositoryAdd);
			dpiBindings.BindImage(_btnInitLocalRepo,   Icons.RepositoryInit);
			dpiBindings.BindImage(_btnCloneRemoteRepo, Icons.RepositoryClone);

			_logo.Parent               = parent;

			_btnAddLocalRepo.Parent    = parent;
			_btnInitLocalRepo.Parent   = parent;
			_btnCloneRemoteRepo.Parent = parent;

			_lblRecentRepositories.Parent           = parent;
			_lstRecentRepositories.Parent           = parent;
			_chkClosePageAfterRepositoryLoad.Parent = parent;
			_chkShowPageAtStartup.Parent            = parent;

			_separator1.Parent           = parent;
			_lblLocalRepositories.Parent = parent;
			decFilter.Parent             = parent;
			_lstLocalRepositories.Parent = parent;
		}
	}

	#region Data

	private readonly StartPageViewFactory _factory;
	private readonly NotifyCollectionBinding<RepositoryLink> _recentRepositoriesBinding;
	private readonly List<RepositoryListItem> _repositories;
	private readonly ViewControls _controls;

	#endregion

	#region .ctor

	public StartPageView(IWorkingEnvironment environment, StartPageViewFactory factory)
		: base(Guids.StartPageView, environment)
	{
		Verify.Argument.IsNotNull(factory);

		SuspendLayout();

		AutoScaleMode       = AutoScaleMode.Dpi;
		AutoScaleDimensions = Dpi.Default;
		Name = nameof(StartPageView);
		Text = Resources.StrStartPage;

		if(GitterApplication.Style.Type == GitterStyleType.DarkBackground)
		{
			BackColor = Color.FromArgb(37, 37, 37);
		}

		_controls = new(GitterApplication.Style);
		_controls.Layout(this);

		ResumeLayout(performLayout: false);
		ResumeLayout();

		_controls._logo.Click += OnLogoClick;

		_factory = factory;
		_repositories = [];
		_controls._lstLocalRepositories.FullList = _repositories;

		_controls._txtFilter.TextChanged += OnFilterTextChanged;

		_controls._lstLocalRepositories.ItemActivated  += OnLocalRepositoriesListItemActivated;
		_controls._lstRecentRepositories.ItemActivated += OnRecentRepositoriesListItemActivated;

		_controls._lstLocalRepositories.DragEnter += OnLocalRepositoriesDragEnter;
		_controls._lstLocalRepositories.DragDrop  += OnLocalRepositoriesDragDrop;

		_controls._lstLocalRepositories.KeyDown  += OnLocalRepositoriesKeyDown;
		_controls._lstRecentRepositories.KeyDown += OnRecentRepositoriesKeyDown;

		var conv = new DpiConverter(this);

		_controls._chkClosePageAfterRepositoryLoad.IsChecked = _factory.CloseAfterRepositoryLoad;
		_controls._chkClosePageAfterRepositoryLoad.IsCheckedChanged += _chkClosePageAfterRepositoryLoad_CheckedChanged;

		_controls._chkShowPageAtStartup.IsChecked = _factory.ShowOnStartup;
		_controls._chkShowPageAtStartup.IsCheckedChanged += _chkShowPageAtStartup_CheckedChanged;

		_controls._btnAddLocalRepo.LinkClicked    += _btnAddLocalRepo_LinkClicked;
		_controls._btnInitLocalRepo.LinkClicked   += _btnInitLocalRepo_LinkClicked;
		_controls._btnCloneRemoteRepo.LinkClicked += _btnCloneRemoteRepo_LinkClicked;

		_recentRepositoriesBinding = new NotifyCollectionBinding<RepositoryLink>(
			_controls._lstRecentRepositories.Items,
			WorkingEnvironment.RepositoryManagerService.RecentRepositories,
			repo => new RecentRepositoryListItem(repo));
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_recentRepositoriesBinding.Dispose();
		}
		base.Dispose(disposing);
	}

	#endregion

	#region Properties

	public override bool IsDocument => true;

	public override IImageProvider ImageProvider => Icons.StartPage;

	#endregion

	#region Methods

	private void OnLocalRepositoriesKeyDown(object? sender, KeyEventArgs e)
	{
		switch(e.KeyCode)
		{
			case Keys.Delete:
				while(_controls._lstLocalRepositories.SelectedItems.Count != 0)
				{
					var item = (RepositoryListItem)_controls._lstLocalRepositories.SelectedItems[0];
					_repositories.Remove(item);
					item.Remove();
				}
				break;
		}
	}

	private void OnRecentRepositoriesKeyDown(object? sender, KeyEventArgs e)
	{
		switch(e.KeyCode)
		{
			case Keys.Delete:
				while(_controls._lstRecentRepositories.SelectedItems.Count != 0)
				{
					var item = (RecentRepositoryListItem)_controls._lstRecentRepositories.SelectedItems[0];
					WorkingEnvironment.RepositoryManagerService.RecentRepositories.Remove(item.DataContext);
					if(item.ListBox != null)
					{
						item.Remove();
					}
				}
				break;
		}
	}

	private void OnLocalRepositoriesDragEnter(object? sender, DragEventArgs e)
	{
		if(e.Data is null) return;

		if(e.Data.GetDataPresent(DataFormats.FileDrop)
			&& e.Data.GetData(DataFormats.FileDrop) is string[] files)
		{
			for(int i = 0; i < files.Length; ++i)
			{
				if(Directory.Exists(files[i]))
				{
					e.Effect = DragDropEffects.Link;
				}
			}
		}
		else if(e.Data.GetDataPresent(typeof(RepositoryListItem)))
		{
			if(string.IsNullOrWhiteSpace(_controls._txtFilter.Text))
			{
				e.Effect = DragDropEffects.Move;
			}
		}
		else if(e.Data.TryGetData<RecentRepositoryListItem>(out var item))
		{
			if(!IsPresentInLocalRepositoryList(item.DataContext.Path))
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

	private void OnLocalRepositoriesDragDrop(object? sender, DragEventArgs e)
	{
		if(e.Data is null) return;
		if(e.Effect == DragDropEffects.None) return;

		if(e.Data.GetDataPresent(DataFormats.FileDrop)
			&& e.Data.GetData(DataFormats.FileDrop) is string[] { Length: not 0 } files)
		{
			foreach(var entry in files)
			{
				var di = new DirectoryInfo(entry);
				if(!di.Exists) continue;

				var path = di.FullName;
				if(IsPresentInLocalRepositoryList(path)) continue;

				var provider = WorkingEnvironment.FindProviderForDirectory(path);
				if(provider is null) continue;

				var link = new RepositoryLink(path, provider.Name);
				var point = _controls._lstLocalRepositories.PointToClient(new Point(e.X, e.Y));
				var index = _controls._lstLocalRepositories.GetInsertIndexFormPoint(point.X, point.Y, false, out var itemsCollection);
				if(index != -1)
				{
					var itemToInsert = new RepositoryListItem(link);
					InsertRepositoryItem(index, itemsCollection, itemToInsert);
				}
			}
		}
		else if(e.Data.TryGetData<RepositoryListItem>(out var itemToMove))
		{
			if(!string.IsNullOrWhiteSpace(_controls._txtFilter.Text))
			{
				return;
			}
			var point = _controls._lstLocalRepositories.PointToClient(new Point(e.X, e.Y));
			var index = _controls._lstLocalRepositories.GetInsertIndexFormPoint(point.X, point.Y, false, out var itemsCollection);
			if(index == -1) return;
			var currentIndex = _controls._lstLocalRepositories.Items.IndexOf(itemToMove);
			if(index == currentIndex) return;
			if(currentIndex == -1)
			{
				_repositories.Insert(index, itemToMove);
				itemsCollection.Insert(index, itemToMove);
			}
			else
			{
				if(index > _controls._lstLocalRepositories.Items.Count - 1)
				{
					--index;
				}
				_repositories.Remove(itemToMove);
				itemToMove.Remove();
				_repositories.Insert(index, itemToMove);
				itemsCollection.Insert(index, itemToMove);
			}
		}
		else if(e.Data.TryGetData<RecentRepositoryListItem>(out var itemToAdd))
		{
			var path = itemToAdd.DataContext.Path;
			if(IsPresentInLocalRepositoryList(path)) return;
			var point = _controls._lstLocalRepositories.PointToClient(new Point(e.X, e.Y));
			var index = _controls._lstLocalRepositories.GetInsertIndexFormPoint(point.X, point.Y, false, out var itemsCollection);
			if(index != -1)
			{
				var itemToInsert = new RepositoryListItem(new RepositoryLink(path, string.Empty));
				InsertRepositoryItem(index, itemsCollection, itemToInsert);
			}
		}
	}

	private void InsertRepositoryItem(int index, CustomListBoxItemsCollection target, RepositoryListItem itemToInsert)
	{
		if(!string.IsNullOrWhiteSpace(_controls._txtFilter.Text))
		{
			if(index == 0)
			{
				_repositories.Insert(0, itemToInsert);
			}
			else
			{
				var dst = _repositories.IndexOf((RepositoryListItem)_controls._lstLocalRepositories.Items[index - 1]);
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

	private void OnLocalRepositoriesListItemActivated(object? sender, ItemEventArgs e)
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

	private void OnRecentRepositoriesListItemActivated(object? sender, ItemEventArgs e)
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
		_controls._lstLocalRepositories.SaveViewTo(listNode);
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
			_controls._lstLocalRepositories.LoadViewFrom(listNode);
			_controls._lstLocalRepositories.BeginUpdate();
			_controls._lstLocalRepositories.Items.Clear();
			_controls._lstLocalRepositories.LoadViewFrom(listNode);
			_repositories.Clear();
			var itemsNode = listNode.TryGetSection("Items");
			if(itemsNode != null)
			{
				foreach(var s in itemsNode.Sections)
				{
					var link = new RepositoryLink(s);
					var item = new RepositoryListItem(link);
					_repositories.Add(item);
					_controls._lstLocalRepositories.Items.Add(item);
				}
			}
			_controls._lstLocalRepositories.EndUpdate();
		}
	}

	public LocalRepositoriesListBox Repositories => _controls._lstLocalRepositories;

	private void _btnAddLocalRepo_LinkClicked(object? sender, EventArgs e)
	{
		var path = Utility.ShowPickFolderDialog(this);
		if(!string.IsNullOrWhiteSpace(path))
		{
			var prov = WorkingEnvironment.FindProviderForDirectory(path!);
			if(prov is null)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					Resources.ErrPathIsNotValidRepository.UseAsFormat(path!),
					Resources.ErrInvalidPath,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return;
			}
			var item = new RepositoryListItem(new RepositoryLink(path!, prov.Name));
			_repositories.Add(item);
			_controls._lstLocalRepositories.Items.Add(item);
		}
	}

	private void _btnInitLocalRepo_LinkClicked(object? sender, EventArgs e)
	{
		using var dialog = new InitRepositoryDialog(WorkingEnvironment);
		dialog.Run(WorkingEnvironment.MainForm);
	}

	private void _btnCloneRemoteRepo_LinkClicked(object? sender, EventArgs e)
	{
		using var dialog = new CloneRepositoryDialog(WorkingEnvironment);
		dialog.Run(WorkingEnvironment.MainForm);
	}

	private void _chkClosePageAfterRepositoryLoad_CheckedChanged(object? sender, EventArgs e)
		=> _factory.CloseAfterRepositoryLoad = sender is ICheckBoxWidget { IsChecked: true };

	private void _chkShowPageAtStartup_CheckedChanged(object? sender, EventArgs e)
		=> _factory.ShowOnStartup = sender is ICheckBoxWidget { IsChecked: true };

	private void OnLogoClick(object? sender, EventArgs e)
	{
		Focus();
	}

	private bool FilterItem(RepositoryListItem item) => FilterItem(item, _controls._txtFilter.Text);

	private static bool FilterItem(RepositoryListItem item, string filter)
	{
		if(string.IsNullOrWhiteSpace(filter)) return true;
		var index = item.DataContext.Path.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase);
		return index >= 0;
	}

	private void RefillLocalRepositories()
	{
		var filter = _controls._txtFilter.Text;
		_controls._lstLocalRepositories.BeginUpdate();
		int index1 = 0;
		for(int index = 0; index < _repositories.Count; ++index)
		{
			var item = _repositories[index];
			if(FilterItem(item, filter))
			{
				if(!item.IsAttachedToListBox)
				{
					_controls._lstLocalRepositories.Items.Insert(index1, item);
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
		_controls._lstLocalRepositories.EndUpdate();
	}

	private void OnFilterTextChanged(object? sender, EventArgs e)
	{
		RefillLocalRepositories();
	}

	#endregion
}
