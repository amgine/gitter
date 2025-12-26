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

namespace gitter.Git.Gui.Views;

using System;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Services;
using gitter.Framework.Controls;
using gitter.Framework.Configuration;

using gitter.Git.Gui.Controls;
using gitter.Framework.Layout;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
partial class CommitView : GitViewBase
{
	readonly struct ViewControls
	{
		public readonly CommitToolbar _toolbar;
		public readonly TextBox _txtMessage;
		public readonly TableLayoutPanel _tableChanges;
		public readonly LabelControl _lblUnstaged;
		public readonly LabelControl _lblStaged;
		public readonly TreeListBox _lstUnstaged;
		public readonly TreeListBox _lstStaged;
		public readonly LabelControl _lblMessage;
		public readonly ICheckBoxWidget _chkAmend;
		public readonly IButtonWidget _btnCommit;

		public ViewControls(IGitterStyle? style, CommitView parent)
		{
			style ??= GitterApplication.Style;

			_toolbar = new(parent)
			{
				GripStyle = ToolStripGripStyle.Hidden,
				Stretch = true,
				Padding = new Padding(2),
				Dock = DockStyle.Top,
				Renderer = GitterApplication.Style.ToolStripRenderer,
			};
			_toolbar.MouseDown += (_, _) => parent.Focus();

			_tableChanges = new();
			_lblUnstaged = new();
			_lblStaged = new();
			_lstUnstaged  = new()
			{
				Style = style,
				ShowTreeLines = true,
				HeaderStyle = HeaderStyle.Hidden,
				Multiselect = true,
			};
			_lstStaged = new()
			{
				Style = style,
				ShowTreeLines = true,
				HeaderStyle = HeaderStyle.Hidden,
				Multiselect = true,
			};
			_lblMessage   = new();
			_txtMessage = new()
			{
				AcceptsReturn = true,
				AcceptsTab = true,
				Multiline = true,
				ScrollBars = ScrollBars.None,
			};
			_chkAmend  = style.CheckBoxFactory.Create();
			_btnCommit = style.ButtonFactory.Create();

			GitterApplication.FontManager.InputFont.Apply(_txtMessage);

			if(style.Type == GitterStyleType.DarkBackground)
			{
				_txtMessage.BorderStyle = BorderStyle.FixedSingle;
			}

			_txtMessage.BackColor = style.Colors.Window;
			_txtMessage.ForeColor = style.Colors.WindowText;

			for(int i = 0; i < _lstUnstaged.Columns.Count; ++i)
			{
				var column = _lstUnstaged.Columns[i];
				column.IsVisible = column.Id == (int)ColumnId.Name;
				if(column.IsVisible)
				{
					column.SizeMode = ColumnSizeMode.Auto;
				}
			}

			for(int i = 0; i < _lstStaged.Columns.Count; ++i)
			{
				var column = _lstStaged.Columns[i];
				column.IsVisible = column.Id == (int)ColumnId.Name;
				if(column.IsVisible)
				{
					column.SizeMode = ColumnSizeMode.Auto;
				}
			}
		}

		public void Localize()
		{
			_lblStaged.Text = Resources.StrsStagedChanges.AddColon();
			_lstStaged.Text = Resources.StrsNoStagedChanges;
			_lblUnstaged.Text = Resources.StrsUnstagedChanges.AddColon();
			_lstUnstaged.Text = Resources.StrsNoUnstagedChanges;
			_lblMessage.Text = Resources.StrMessage.AddColon();
			_chkAmend.Text = Resources.StrAmend;
			_btnCommit.Text = Resources.StrCommit;
		}

		public void Layout(Control parent)
		{
			var decMessage = new TextBoxDecorator(_txtMessage);
			Panel grip;
			Grid grid;

			_ = new ControlLayout(parent)
			{
				Content = grid = new Grid(
					rows:
					[
						/* 0 */ LayoutConstants.ToolbarHeight,
						/* 1 */ SizeSpec.Absolute(2),
						/* 2 */ SizeSpec.Everything(),
						/* 3 */ SizeSpec.Absolute(4),
						/* 4 */ LayoutConstants.LabelRowHeight,
						/* 5 */ LayoutConstants.LabelRowSpacing,
						/* 6 */ SizeSpec.Absolute(100),
					],
					content:
					[
						new GridContent(new ControlContent(_toolbar, marginOverride: LayoutConstants.NoMargin), row: 0),
						new GridContent(new Grid(
							padding: DpiBoundValue.Padding(new(2, 0, 2, 0)),
							rows:
							[
								LayoutConstants.LabelRowHeight,
								LayoutConstants.LabelRowSpacing,
								SizeSpec.Everything(),
							],
							columns:
							[
								SizeSpec.Relative(0.5f),
								SizeSpec.Absolute(4),
								SizeSpec.RelativeToLeftover(1),
							],
							content:
							[
								new GridContent(new ControlContent(_lblUnstaged, marginOverride: LayoutConstants.NoMargin), column: 0, row: 0),
								new GridContent(new ControlContent(_lblStaged,   marginOverride: LayoutConstants.NoMargin), column: 2, row: 0),
								new GridContent(new ControlContent(_lstUnstaged, marginOverride: LayoutConstants.NoMargin), column: 0, row: 2),
								new GridContent(new ControlContent(_lstStaged,   marginOverride: LayoutConstants.NoMargin), column: 2, row: 2),
							]), row: 2),
						new GridContent(new ControlContent(grip = new()
						{
							Cursor = Cursors.SizeNS,
						}, marginOverride: LayoutConstants.NoMargin), row: 3),
						new GridContent(new ControlContent(_lblMessage, marginOverride: LayoutConstants.NoMargin), row: 4),
						new GridContent(new Grid(
							padding: DpiBoundValue.Padding(new(2, 0, 2, 2)),
							columns:
							[
								SizeSpec.Everything(),
								SizeSpec.Absolute(8),
								SizeSpec.Absolute(74),
							],
							rows:
							[
								SizeSpec.Everything(),
								LayoutConstants.CheckBoxRowHeight,
								LayoutConstants.RowSpacing,
								SizeSpec.Absolute(22),
							],
							content:
							[
								new GridContent(new ControlContent(decMessage, marginOverride: LayoutConstants.NoMargin), row: 0, rowSpan: 4, column: 0),
								new GridContent(new WidgetContent(_chkAmend,  marginOverride: LayoutConstants.NoMargin), row: 1, column: 2),
								new GridContent(new WidgetContent(_btnCommit, marginOverride: LayoutConstants.NoMargin), row: 3, column: 2),
							]), row: 6),
					]),
			};

			_toolbar.Parent     = parent;
			_lblUnstaged.Parent = parent;
			_lblStaged.Parent   = parent;
			_lstUnstaged.Parent = parent;
			_lstStaged.Parent   = parent;
			grip.Parent         = parent;
			_lblMessage.Parent  = parent;
			decMessage.Parent   = parent;
			_chkAmend.Parent    = parent;
			_btnCommit.Parent   = parent;

			var tabIndex = 0;
			_toolbar.TabIndex     = tabIndex++;
			_lblUnstaged.TabIndex = tabIndex++;
			_lblStaged.TabIndex   = tabIndex++;
			_lstUnstaged.TabIndex = tabIndex++;
			_lstStaged.TabIndex   = tabIndex++;
			grip.TabIndex         = tabIndex++;
			_lblMessage.TabIndex  = tabIndex++;
			decMessage.TabIndex   = tabIndex++;
			_chkAmend.TabIndex    = tabIndex++;
			_btnCommit.TabIndex   = tabIndex++;

			_ = new VerticalResizer(grip, grid.Rows[6], -1, DpiBoundValue.ScaleY(100));
		}

		public void Dispose()
		{
			_chkAmend.Dispose();
			_btnCommit.Dispose();
		}
	}

	private readonly ViewControls _controls;
	private readonly TextBoxSpellChecker? _speller;
	private bool _treeMode;
	private bool _suppressDiffUpdate;

	public CommitView(GuiProvider gui)
		: base(Guids.CommitViewGuid, gui)
	{
		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Name                = nameof(CommitView);
		Text                = Resources.StrCommit;
		_controls = new ViewControls(GitterApplication.Style, this);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		_treeMode = true;

		_controls._lstStaged.PreviewKeyDown += OnKeyDown;
		_controls._lstUnstaged.PreviewKeyDown += OnKeyDown;
		_controls._txtMessage.PreviewKeyDown += OnKeyDown;
		_controls._chkAmend.Control.PreviewKeyDown += OnKeyDown;
		_controls._btnCommit.Control.PreviewKeyDown += OnKeyDown;

		_controls._lstStaged.ItemActivated += OnStagedItemActivated;
		_controls._lstUnstaged.ItemActivated += OnUnstagedItemActivated;

		_controls._lstStaged.GotFocus += OnStagedGotFocus;
		_controls._lstUnstaged.GotFocus += OnUnstagedGotFocus;

		_controls._chkAmend.IsCheckedChanged += OnAmendIsCheckedChanged;
		_controls._btnCommit.Click += OnCommitClick;

		if(SpellingService.Enabled)
		{
			_speller = new TextBoxSpellChecker(_controls._txtMessage, true);
		}
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_controls.Dispose();
			_speller?.Dispose();
		}
		base.Dispose(disposing);
	}

	private void LoadCommitMessage()
	{
		if(Repository is not null)
		{
			Message = Repository.Status.LoadCommitMessage();
		}
	}

	private void SaveCommitMessage()
	{
		Repository?.Status.SaveCommitMessage(Message);
	}

	public override bool IsDocument => true;

	protected override void AttachToRepository(Repository repository)
	{
		Assert.IsNotNull(repository);

		lock(repository.Status.SyncRoot)
		{
			_controls._lstUnstaged.SetTree(repository.Status.UnstagedRoot, _treeMode ?
				TreeListBoxMode.ShowFullTree : TreeListBoxMode.ShowPlainFileList);
			_controls._lstStaged.SetTree(repository.Status.StagedRoot, _treeMode ?
				TreeListBoxMode.ShowFullTree : TreeListBoxMode.ShowPlainFileList);
		}

		_controls._lstStaged.SelectionChanged += OnSelectedStagedItemsChanged;
		_controls._lstUnstaged.SelectionChanged += OnSelectedUnstagedItemsChanged;
		LoadCommitMessage();
	}

	protected override void DetachFromRepository(Repository repository)
	{
		Assert.IsNotNull(repository);

		SaveCommitMessage();
		_controls._lstStaged.Clear();
		_controls._lstUnstaged.Clear();
		_controls._lstStaged.SelectionChanged -= OnSelectedStagedItemsChanged;
		_controls._lstUnstaged.SelectionChanged -= OnSelectedUnstagedItemsChanged;
		_controls._txtMessage.Text = string.Empty;
	}

	public bool TreeMode
	{
		get => _treeMode;
		set
		{
			if(_treeMode == value) return;

			_treeMode = value;

			if(Repository is null) return;

			lock(Repository.Status.SyncRoot)
			{
				_controls._lstUnstaged.SetTree(Repository.Status.UnstagedRoot, _treeMode ?
					TreeListBoxMode.ShowFullTree : TreeListBoxMode.ShowPlainFileList);
				_controls._lstStaged.SetTree(Repository.Status.StagedRoot, _treeMode?
					TreeListBoxMode.ShowFullTree : TreeListBoxMode.ShowPlainFileList);
			}
		}
	}

	public bool Amend
	{
		get => _controls._chkAmend.IsChecked;
		set => _controls._chkAmend.IsChecked = value;
	}

	public bool HasMessage => _controls._txtMessage.TextLength != 0;

	public string Message
	{
		get => _controls._txtMessage.Text;
		set => _controls._txtMessage.Text = value;
	}

	public override IImageProvider ImageProvider => Icons.Commit;

	public override void OnActivated()
	{
		_controls._chkAmend.Control.Enabled = Repository is { Head.IsEmpty: false };
	}

	protected override void OnClosing()
	{
		SaveCommitMessage();
		base.OnClosing();
	}

	public override void RefreshContent()
	{
		if(IsDisposed) return;
		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new MethodInvoker(RefreshContentSync));
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			RefreshContentSync();
		}
	}

	private void RefreshContentSync()
	{
		if(IsDisposed) return;
		if(Repository is null) return;

		using(this.ChangeCursor(Cursors.WaitCursor))
		{
			_controls._lstStaged.BeginUpdate();
			_controls._lstUnstaged.BeginUpdate();
			Repository.Status.Refresh();
			_controls._lstStaged.EndUpdate();
			_controls._lstUnstaged.EndUpdate();
		}
	}

	private void OnStagedGotFocus(object? sender, EventArgs e)
	{
		if(_controls._lstUnstaged.SelectedItems.Count != 0)
		{
			_suppressDiffUpdate = true;
			_controls._lstUnstaged.SelectedItems.Clear();
			_suppressDiffUpdate = false;
			ShowContextualDiffView(null);
		}
	}

	private void OnUnstagedGotFocus(object? sender, EventArgs e)
	{
		if(_controls._lstStaged.SelectedItems.Count != 0)
		{
			_suppressDiffUpdate = true;
			_controls._lstStaged.SelectedItems.Clear();
			_suppressDiffUpdate = false;
			ShowContextualDiffView(null);
		}
	}

	private void OnSelectedUnstagedItemsChanged(object? sender, EventArgs e)
	{
		if(Repository is null) return;

		if(_suppressDiffUpdate) return;
		var items = ((CustomListBox)sender!).SelectedItems;
		if(items.Count == 0)
		{
			ShowContextualDiffView(null);
		}
		else
		{
			var paths = new string[items.Count];
			for(int i = 0; i < paths.Length; ++i)
			{
				paths[i] = ((ITreeItemListItem)items[i]).TreeItem.RelativePath;
			}

			ShowContextualDiffView(Repository.Status.GetDiffSource(false, paths));
		}
	}

	private void OnSelectedStagedItemsChanged(object? sender, EventArgs e)
	{
		if(Repository is null) return;

		if(_suppressDiffUpdate) return;
		var items = ((CustomListBox)sender!).SelectedItems;
		if(items.Count == 0)
		{
			ShowContextualDiffView(null);
		}
		else
		{
			var paths = new string[items.Count];
			for(int i = 0; i < paths.Length; ++i)
			{
				paths[i] = ((ITreeItemListItem)items[i]).TreeItem.RelativePath;
			}

			ShowContextualDiffView(Repository.Status.GetDiffSource(true, paths));
		}
	}

	private void OnAmendIsCheckedChanged(object? sender, EventArgs e)
	{
		if(!Amend || HasMessage) return;
		LoadHeadCommitMessage();
	}

	private bool LoadHeadCommitMessage()
	{
		if(Repository is null) return true;

		var rev = Repository.Head.Revision;
		if(rev is null) return false;

		var textBox = _controls._txtMessage;
		textBox.AppendText(Utility.ExpandNewLineCharacters(rev.Subject));
		if(!string.IsNullOrEmpty(rev.Body))
		{
			textBox.AppendText(Environment.NewLine);
			textBox.AppendText(Environment.NewLine);
			textBox.AppendText(Utility.ExpandNewLineCharacters(rev.Body));
		}
		textBox.SelectAll();
		return true;
	}

	protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
	{
		Assert.IsNotNull(e);

		OnKeyDown(this, e);
		base.OnPreviewKeyDown(e);
	}

	private void OnKeyDown(object? sender, PreviewKeyDownEventArgs e)
	{
		Assert.IsNotNull(e);

		switch(e.KeyCode)
		{
			case Keys.F5:
				RefreshContent();
				e.IsInputKey = true;
				break;
		}
	}

	private void OnStagedItemActivated(object? sender, ItemEventArgs e)
	{
		Assert.IsNotNull(e);

		if(e.Item is not TreeFileListItem item) return;

		try
		{
			using(this.ChangeCursor(Cursors.WaitCursor))
			{
				item.DataContext.Unstage();
			}
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				this,
				exc.Message,
				Resources.ErrFailedToUnstage,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
		}
	}

	private void OnUnstagedItemActivated(object? sender, ItemEventArgs e)
	{
		Assert.IsNotNull(e);

		if(e.Item is not TreeFileListItem item) return;

		try
		{
			using(this.ChangeCursor(Cursors.WaitCursor))
			{
				item.DataContext.Stage();
			}
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				this,
				exc.Message,
				Resources.ErrFailedToStage,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
		}
	}

	private void OnCommitClick(object? sender, EventArgs e)
	{
		if(Repository is null) return;

		string message = _controls._txtMessage.Text.Trim();
		bool amend = _controls._chkAmend.Control.Enabled && _controls._chkAmend.IsChecked;
		bool merging = Repository.State == RepositoryState.Merging;
		if(_controls._lstStaged.Items.Count == 0 && !merging && !amend)
		{
			NotificationService.NotifyInputError(
				_controls._lstStaged,
				Resources.ErrNothingToCommit,
				Resources.ErrNofilesStagedForCommit);
			return;
		}
		if(message.Length == 0)
		{
			NotificationService.NotifyInputError(
				_controls._txtMessage,
				Resources.ErrEmptyCommitMessage,
				Resources.ErrEnterCommitMessage);
			return;
		}
		if(message.Length < 2)
		{
			NotificationService.NotifyInputError(
				_controls._txtMessage,
				Resources.ErrShortCommitMessage,
				Resources.ErrEnterLongerCommitMessage.UseAsFormat(2));
			return;
		}
		try
		{
			using(this.ChangeCursor(Cursors.WaitCursor))
			{
				Repository.Status.Commit(message, amend);
			}
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				this,
				exc.Message,
				Resources.ErrFailedToCommit,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			return;
		}
		_controls._txtMessage.Text = string.Empty;
		_controls._chkAmend.IsChecked = false;
	}

	protected override void SaveMoreViewTo(Section section)
	{
		base.SaveMoreViewTo(section);

		section.SetValue<bool>("TreeMode", _treeMode);
		var stagedListSection = section.GetCreateSection("StagedList");
		_controls._lstStaged.SaveViewTo(stagedListSection);
		var unstagedListSection = section.GetCreateSection("UnstagedList");
		_controls._lstUnstaged.SaveViewTo(unstagedListSection);
	}

	protected override void LoadMoreViewFrom(Section section)
	{
		base.LoadMoreViewFrom(section);

		_controls._toolbar.TreeModeButton.Checked = _treeMode = section.GetValue<bool>("TreeMode", true);
		var stagedListSection = section.TryGetSection("StagedList");
		if(stagedListSection is not null)
		{
			_controls._lstStaged.LoadViewFrom(stagedListSection);
		}
		var unstagedListSection = section.TryGetSection("UnstagedList");
		if(unstagedListSection is not null)
		{
			_controls._lstUnstaged.LoadViewFrom(unstagedListSection);
		}
	}
}
