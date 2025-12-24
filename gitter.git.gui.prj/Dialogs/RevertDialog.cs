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

namespace gitter.Git.Gui.Dialogs;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Services;

using gitter.Git.Gui.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class RevertDialog : GitDialogBase, IExecutableDialog
{
	readonly struct DialogControls
	{
		public  readonly TextBox           _txtRevision;
		private readonly LabelControl      _lblRevision;
		private readonly GroupSeparator    _grpMainlineParentCommit;
		public  readonly FlowLayoutControl _lstCommits;
		public  readonly ICheckBoxWidget   _chkNoCommit;
		private readonly GroupSeparator    _grpOptions;
		private readonly bool _showCommitSelector;

		public DialogControls(IGitterStyle? style, bool showCommitSelector)
		{
			style ??= GitterApplication.Style;

			_showCommitSelector = showCommitSelector;

			_txtRevision = new() { ReadOnly = true };
			_lblRevision = new();
			if(_showCommitSelector)
			{
				_grpMainlineParentCommit = new();
				_lstCommits = new() { Style = style };
			}
			else
			{
				_grpMainlineParentCommit = default!;
				_lstCommits = default!;
			}
			_chkNoCommit = style.CheckBoxFactory.Create();
			_grpOptions  = new();
		}

		public void Localize()
		{
			if(_showCommitSelector)
			{
				_grpMainlineParentCommit.Text = Resources.StrMainlineParentCommit;
			}
			_lblRevision.Text = Resources.StrRevision.AddColon();
			_grpOptions.Text = Resources.StrOptions;
			_chkNoCommit.Text = Resources.StrsNoCommit;
		}

		public void Layout(Control parent)
		{
			var decRevision = new TextBoxDecorator(_txtRevision);

			if(_showCommitSelector)
			{
				_ = new ControlLayout(parent)
				{
					Content = new Grid(
						rows:
						[
							LayoutConstants.TextInputRowHeight,
							LayoutConstants.GroupSeparatorRowHeight,
							SizeSpec.Everything(),
							LayoutConstants.GroupSeparatorRowHeight,
							LayoutConstants.CheckBoxRowHeight,
						],
						content:
						[
							new GridContent(new Grid(
							columns:
							[
								SizeSpec.Absolute(100),
								SizeSpec.Everything(),
							],
							content:
							[
								new GridContent(new ControlContent(_lblRevision, marginOverride: LayoutConstants.TextBoxLabelMargin), column: 0),
								new GridContent(new ControlContent(decRevision,  marginOverride: LayoutConstants.TextBoxMargin),      column: 1),
							]), row: 0),
							new GridContent(new ControlContent(_grpMainlineParentCommit, marginOverride: LayoutConstants.NoMargin), row: 1),
							new GridContent(new ControlContent(_lstCommits,  marginOverride: LayoutConstants.GroupPadding), row: 2),
							new GridContent(new ControlContent(_grpOptions,  marginOverride: LayoutConstants.NoMargin),     row: 3),
							new GridContent(new WidgetContent (_chkNoCommit, marginOverride: LayoutConstants.GroupPadding), row: 4),
						]),
				};
			}
			else
			{
				_ = new ControlLayout(parent)
				{
					Content = new Grid(
						rows:
						[
							LayoutConstants.TextInputRowHeight,
							LayoutConstants.GroupSeparatorRowHeight,
							LayoutConstants.CheckBoxRowHeight,
						],
						content:
						[
							new GridContent(new Grid(
							columns:
							[
								SizeSpec.Absolute(100),
								SizeSpec.Everything(),
							],
							content:
							[
								new GridContent(new ControlContent(_lblRevision, marginOverride: LayoutConstants.TextBoxLabelMargin), column: 0),
								new GridContent(new ControlContent(decRevision,  marginOverride: LayoutConstants.TextBoxMargin),      column: 1),
							]), row: 0),
							new GridContent(new ControlContent(_grpOptions,  marginOverride: LayoutConstants.NoMargin),     row: 1),
							new GridContent(new WidgetContent (_chkNoCommit, marginOverride: LayoutConstants.GroupPadding), row: 2),
						]),
				};
			}

			decRevision.Parent = parent;
			_lblRevision.Parent = parent;
			if(_showCommitSelector)
			{
				_grpMainlineParentCommit.Parent = parent;
				_lstCommits.Parent = parent;
			}
			_chkNoCommit.Parent = parent;
			_grpOptions.Parent = parent;
		}
	}

	private static readonly IDpiBoundValue<Size> _sizeWithCommitSelector    = DpiBoundValue.Size(new(480, 327));
	private static readonly IDpiBoundValue<Size> _sizeWithoutCommitSelector = DpiBoundValue.Size(new(385, 78));

	private readonly DialogControls _controls;
	private readonly bool _showCommitSelector;

	private static Revision GetRevision(IRevisionPointer revisionPointer)
	{
		var revision = revisionPointer.Dereference()
			?? throw new ArgumentException($"Unable to dereference '{revisionPointer.Pointer}'", nameof(revisionPointer));
		if(!revision.IsLoaded) revision.Load();
		return revision;
	}

	public RevertDialog(IRevisionPointer revisionPointer)
	{
		Verify.Argument.IsValidRevisionPointer(revisionPointer);

		RevisionPointer = revisionPointer;
		var revision = GetRevision(revisionPointer);
		_showCommitSelector = revision.Parents.Count > 1;

		SuspendLayout();
		AutoScaleMode = AutoScaleMode.Dpi;
		AutoScaleDimensions = Dpi.Default;
		Name = nameof(RevertDialog);
		Text = Resources.StrRevertCommit;
		Size = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style, _showCommitSelector);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		ToolTipService.Register(_controls._chkNoCommit.Control, Resources.TipRevertNoCommit);

		_controls._txtRevision.Text = revisionPointer.Pointer;

		GitterApplication.FontManager.InputFont.Apply(_controls._txtRevision);

		if(_showCommitSelector)
		{
			bool first = true;
			foreach(var parent in revision.Parents)
			{
				if(!parent.IsLoaded)
				{
					parent.Load();
				}
				if(first)
				{
					first = false;
				}
				else
				{
					_controls._lstCommits.Panels.Add(new FlowPanelSeparator() { Height = 10 });
				}
				_controls._lstCommits.Panels.Add(
					new RevisionHeaderPanel()
					{
						Revision = parent,
						IsSelectable = true,
						IsSelected = _controls._lstCommits.Panels.Count == 0,
					});
			}
		}
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize
		=> _showCommitSelector
			? _sizeWithCommitSelector
			: _sizeWithoutCommitSelector;

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrRevert;

	public IRevisionPointer RevisionPointer { get; }

	public bool NoCommit
	{
		get => _controls._chkNoCommit.IsChecked;
		set => _controls._chkNoCommit.IsChecked = value;
	}

	public int MainlineParentCommit
	{
		get
		{
			if(!_showCommitSelector) return 0;

			int index = 0;
			foreach(var p in _controls._lstCommits.Panels)
			{
				if(p is RevisionHeaderPanel rhp)
				{
					++index;
					if(rhp.IsSelected)
					{
						return index;
					}
				}
			}
			return 0;
		}
		set
		{
			Verify.Argument.IsNotNegative(value);
			Verify.State.IsTrue(_showCommitSelector);

			if(value == 0)
			{
				foreach(var p in _controls._lstCommits.Panels)
				{
					if(p is RevisionHeaderPanel rhp)
					{
						rhp.IsSelected = false;
					}
				}
			}
			else
			{
				int index = 0;
				foreach(var p in _controls._lstCommits.Panels)
				{
					if(p is RevisionHeaderPanel rhp)
					{
						++index;
						if(index == value)
						{
							rhp.IsSelected = true;
							break;
						}
					}
				}
			}
		}
	}

	public bool Execute()
	{
		int mainline  = MainlineParentCommit;
		bool noCommit = NoCommit;
		try
		{
			using(this.ChangeCursor(Cursors.WaitCursor))
			{
				RevisionPointer.Revert(mainline, noCommit);
			}
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				this,
				exc.Message,
				Resources.ErrFailedToRevert,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			return false;
		}
		return true;
	}
}
