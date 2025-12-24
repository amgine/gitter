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

namespace gitter.Git.Gui.Dialogs;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;
using gitter.Framework.Services;

using gitter.Git.Gui.Controllers;
using gitter.Git.Gui.Controls;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class MergeDialog : GitDialogBase, IExecutableDialog, IMergeView
{
	private sealed class RevisionsInput(ReferencesListBox referencesListBox)
		: ControlInputSource<ReferencesListBox, Many<IRevisionPointer>>(referencesListBox)
	{
		private bool _multiselect;

		public bool Multiselect
		{
			get => _multiselect;
			set
			{
				if(_multiselect != value)
				{
					UnsubscribeFromValueChangeEvent();
					_multiselect = value;
					SubscribeToValueChangeEvent();
					if(value)
					{
						Control.EnableCheckboxes();
					}
					else
					{
						Control.DisableCheckboxes();
					}
					InvalidateValue();
				}
			}
		}

		protected override void SubscribeToValueChangeEvent()
		{
			if(Multiselect)
			{
				Control.ItemCheckedChanged += OnControlValueChanged;
			}
			else
			{
				Control.SelectionChanged += OnControlValueChanged;
			}
		}

		protected override void UnsubscribeFromValueChangeEvent()
		{
			if(Multiselect)
			{
				Control.ItemCheckedChanged -= OnControlValueChanged;
			}
			else
			{
				Control.SelectionChanged -= OnControlValueChanged;
			}
		}

		private static void InspectItem(CustomListBoxItem item, List<IRevisionPointer> list)
		{
			if(item.CheckedState == CheckedState.Checked)
			{
				if(item is IRevisionPointerListItem revPointerlistItem)
				{
					list.Add(revPointerlistItem.RevisionPointer);
				}
			}
			foreach(var i in item.Items)
			{
				InspectItem(i, list);
			}
		}

		protected override Many<IRevisionPointer> FetchValue()
		{
			if(Multiselect)
			{
				var selectedRevisions = new List<IRevisionPointer>();
				foreach(var item in Control.Items)
				{
					InspectItem(item, selectedRevisions);
				}
				return selectedRevisions;
			}
			if(Control.SelectedItems.Count == 0)
			{
				return default;
			}
			if(Control.SelectedItems[0] is not IRevisionPointerListItem revItem)
			{
				return default;
			}
			var revision = revItem.RevisionPointer;
			if(revision is null) return default;
			return new(revision);
		}

		private static void SetItemCheckedState(CustomListBoxItem item, Many<IRevisionPointer> list)
		{
			if(item.CheckedState != CheckedState.Unavailable)
			{
				if(item is IRevisionPointerListItem revPointerlistItem)
				{
					item.CheckedState = list.Contains(revPointerlistItem.RevisionPointer)
						? CheckedState.Checked
						: CheckedState.Unchecked;
				}
			}
			foreach(var child in item.Items)
			{
				SetItemCheckedState(child, list);
			}
		}

		private static CustomListBoxItem? TryFindItem(CustomListBoxItemsCollection items, IRevisionPointer revisionPointer)
		{
			if(revisionPointer is null)
			{
				return null;
			}
			foreach(var item in items)
			{
				if(item is IRevisionPointerListItem revItem)
				{
					if(revItem.RevisionPointer == revisionPointer)
					{
						return item;
					}
				}
				else
				{
					var res = TryFindItem(item.Items, revisionPointer);
					if(res is not null)
					{
						return res;
					}
				}
			}
			return null;
		}

		protected override void SetValue(Many<IRevisionPointer> value)
		{
			if(Multiselect)
			{
				foreach(var item in Control.Items)
				{
					SetItemCheckedState(item, value);
				}
			}
			else
			{
				Control.SelectedItems.Clear();
				if(value is {Count : not 0 })
				{
					var item = TryFindItem(Control.Items, value[0]);
					item?.FocusAndSelect();
				}
			}
		}
	}

	readonly struct DialogControls
	{
		public  readonly ReferencesListBox _references;
		private readonly LabelControl _lblMergeWith;
		public  readonly ICheckBoxWidget _chkNoFF;
		public  readonly ICheckBoxWidget _chkNoCommit;
		public  readonly ICheckBoxWidget _chkSquash;
		private readonly LabelControl _lblMessage;
		public  readonly TextBox _txtMessage;
		public  readonly LinkLabel _lnkAutoFormat;
		public  readonly GroupSeparator _grpOptions;

		public DialogControls(IGitterStyle style)
		{
			style ??= GitterApplication.Style;

			var cbf = style.CheckBoxFactory;
			_references = new()
			{
				ShowTreeLines  = true,
				HeaderStyle    = HeaderStyle.Hidden,
				ItemActivation = gitter.Framework.Controls.ItemActivation.SingleClick,
				Style          = style,
			};
			_lblMergeWith = new();
			_chkNoFF = cbf.Create();
			_chkNoCommit = cbf.Create();
			_chkSquash = cbf.Create();
			_lblMessage = new();
			_txtMessage = new()
			{
				Multiline = true,
				AcceptsReturn = true,
				AcceptsTab = true,
				WordWrap = false,
				//ScrollBars = ScrollBars.Vertical,
			};
			_lnkAutoFormat = new()
			{
				TextAlign       = ContentAlignment.MiddleRight,
				LinkColor       = style.Colors.HyperlinkText,
				ActiveLinkColor = style.Colors.HyperlinkTextHotTrack,
			};
			_grpOptions = new();

			GitterApplication.FontManager.InputFont.Apply(_txtMessage);
		}

		public void Localize()
		{
			_references.Text = Resources.StrNoBranchesToMergeWith;
			_lblMergeWith.Text = Resources.StrMergeWith.AddColon();
			_grpOptions.Text = Resources.StrOptions;
			_lblMessage.Text = Resources.StrMessage.AddColon();
			_lnkAutoFormat.Text = Resources.StrAutoFormat;
			_chkNoFF.Text = Resources.StrsNoFastForward;
			_chkNoCommit.Text = Resources.StrsNoCommit;
			_chkSquash.Text = Resources.StrSquash;
		}

		public void Layout(Control parent)
		{
			var messageDec = new TextBoxDecorator(_txtMessage);

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(230),
						LayoutConstants.RowSpacing,
						SizeSpec.Everything(),
					],
					rows:
					[
						LayoutConstants.LabelRowHeight,
						LayoutConstants.LabelRowSpacing,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblMergeWith,  marginOverride: LayoutConstants.NoMargin), column: 0, row: 0),
						new	GridContent(new ControlContent(_references,    marginOverride: LayoutConstants.NoMargin), column: 0, row: 2),
						new GridContent(new Grid(
							columns:
							[
								SizeSpec.Everything(),
								SizeSpec.Absolute(50),
							],
							content:
							[
								new GridContent(new ControlContent(_lblMessage,    marginOverride: LayoutConstants.NoMargin), column: 0),
								new GridContent(new ControlContent(_lnkAutoFormat, marginOverride: LayoutConstants.NoMargin), column: 1),
							]), column: 2),
						new GridContent(new Grid(
							rows:
							[
								SizeSpec.Everything(),
								LayoutConstants.GroupSeparatorRowHeight,
								LayoutConstants.CheckBoxRowHeight,
								LayoutConstants.CheckBoxRowHeight,
								LayoutConstants.CheckBoxRowHeight,
							],
							content:
							[
								new GridContent(new ControlContent(messageDec,   marginOverride: LayoutConstants.NoMargin), row: 0),
								new GridContent(new ControlContent(_grpOptions,  marginOverride: LayoutConstants.NoMargin), row: 1),
								new GridContent(new WidgetContent (_chkNoFF,     marginOverride: LayoutConstants.GroupPadding), row: 2),
								new GridContent(new WidgetContent (_chkNoCommit, marginOverride: LayoutConstants.GroupPadding), row: 3),
								new GridContent(new WidgetContent (_chkSquash,   marginOverride: LayoutConstants.GroupPadding), row: 4),
							]), column: 2, row: 2),
					]),
			};

			var tabIndex = 0;
			_lblMergeWith.TabIndex = tabIndex++;
			_references.TabIndex = tabIndex++;
			_lblMessage.TabIndex = tabIndex++;
			_lnkAutoFormat.TabIndex = tabIndex++;
			messageDec.TabIndex = tabIndex++;
			_grpOptions.TabIndex = tabIndex++;
			_chkNoFF.TabIndex = tabIndex++;
			_chkNoCommit.TabIndex = tabIndex++;
			_chkSquash.TabIndex = tabIndex++;

			_lblMergeWith.Parent = parent;
			_references.Parent = parent;
			_lblMessage.Parent = parent;
			_lnkAutoFormat.Parent = parent;
			messageDec.Parent = parent;
			_grpOptions.Parent = parent;
			_chkNoFF.Parent = parent;
			_chkNoCommit.Parent = parent;
			_chkSquash.Parent = parent;
		}
	}

	private readonly DialogControls _controls;
	private readonly Repository _repository;
	private readonly HashSet<BranchBase> _unmergedBranches;
	private readonly TextBoxSpellChecker? _speller;
	private readonly RevisionsInput _revisionsInput;
	private readonly IMergeController _controller;

	public MergeDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		_repository = repository;

		Name = nameof(MergeDialog);
		Text = Resources.StrMergeBranches;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		SetupTooltips();

		var inputs = new IUserInputSource[]
		{
			_revisionsInput = new RevisionsInput(_controls._references),
			Message         = new TextBoxInputSource(_controls._txtMessage),
			NoFastForward   = new CheckBoxWidgetInputSource(_controls._chkNoFF),
			NoCommit        = new CheckBoxWidgetInputSource(_controls._chkNoCommit),
			Squash          = new CheckBoxWidgetInputSource(_controls._chkSquash),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		if(SpellingService.Enabled)
		{
			_speller = new TextBoxSpellChecker(_controls._txtMessage, true);
		}

		_unmergedBranches = [.. _repository.Refs.GetUnmergedBranches()];
		_controls._references.DisableContextMenus = true;
		_controls._references.LoadData(_repository, ReferenceType.Branch, false, GlobalBehavior.GroupRemoteBranches,
			reference => reference is BranchBase branch && _unmergedBranches.Contains(branch));

		_controls._lnkAutoFormat.LinkClicked += OnAutoFormatLinkClicked;

		_controller = new MergeController(repository) { View = this };
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_speller?.Dispose();
		}
		base.Dispose(disposing);
	}

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_controls._txtMessage.Focus);
	}

	private void SetupTooltips()
	{
		ToolTipService.Register(_controls._chkNoFF.Control, Resources.TipNoFF);
		ToolTipService.Register(_controls._chkNoCommit.Control, Resources.TipMergeNoCommit);
		ToolTipService.Register(_controls._chkSquash.Control, Resources.TipSquash);
	}

	public void EnableMultipleBrunchesMerge()
	{
		_revisionsInput.Multiselect = true;
	}

	private void AutoFormatMessage()
	{
		var revisions = Revisions.Value;
		if(revisions.Count == 0)
		{
			return;
		}
		try
		{
			_controls._txtMessage.Text = _repository.Head.FormatMergeMessage(revisions);
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
		}
	}

	private void OnAutoFormatLinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
	{
		AutoFormatMessage();
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(642, 359));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrMerge;

	public IUserInputSource<Many<IRevisionPointer>> Revisions => _revisionsInput;

	public IUserInputSource<string?> Message { get; }

	public IUserInputSource<bool> NoFastForward { get; }

	public IUserInputSource<bool> NoCommit { get; }

	public IUserInputSource<bool> Squash { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	public bool Execute() => _controller.TryMerge();
}
