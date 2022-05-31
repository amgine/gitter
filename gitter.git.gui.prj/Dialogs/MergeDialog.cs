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
	#region Helpers

	private sealed class RevisionsInput : ControlInputSource<ReferencesListBox, IList<IRevisionPointer>>
	{
		private bool _multiselect;

		public RevisionsInput(ReferencesListBox referencesListBox)
			: base(referencesListBox)
		{
		}

		public bool Multiselect
		{
			get => _multiselect;
			set
			{
				if(_multiselect != value)
				{
					UnsubscribeToValueChangeEvent();
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

		protected override void UnsubscribeToValueChangeEvent()
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

		protected override IList<IRevisionPointer> FetchValue()
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
			else
			{
				if(Control.SelectedItems.Count == 0)
				{
					return null;
				}
				else
				{
					if(Control.SelectedItems[0] is not IRevisionPointerListItem item)
					{
						return null;
					}
					var revision = item.RevisionPointer;
					if(revision is null)
					{
						return null;
					}
					return new IRevisionPointer[] { revision };
				}
			}
		}

		private static void SelItemCheckedState(CustomListBoxItem item, IList<IRevisionPointer> list)
		{
			if(item.CheckedState != CheckedState.Unavailable)
			{
				if(item is IRevisionPointerListItem revPointerlistItem)
				{
					if(list is null || !list.Contains(revPointerlistItem.RevisionPointer))
					{
						item.CheckedState = CheckedState.Unchecked;
					}
					else
					{
						item.CheckedState = CheckedState.Checked;
					}
				}
			}
			foreach(var i in item.Items)
			{
				SelItemCheckedState(i, list);
			}
		}

		private static CustomListBoxItem TryFindItem(CustomListBoxItemsCollection items, IRevisionPointer revisionPointer)
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

		protected override void SetValue(IList<IRevisionPointer> value)
		{
			if(Multiselect)
			{
				foreach(var item in Control.Items)
				{
					SelItemCheckedState(item, value);
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

	#endregion

	#region Data

	private readonly Repository _repository;
	private readonly HashSet<BranchBase> _unmergedBranches;
	private TextBoxSpellChecker _speller;
	private readonly RevisionsInput _revisionsInput;
	private readonly IMergeController _controller;

	#endregion

	#region .ctor

	public MergeDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		_repository = repository;

		InitializeComponent();
		Localize();
		SetupTooltips();

		var inputs = new IUserInputSource[]
		{
			_revisionsInput = new RevisionsInput(_references),
			Message         = new TextBoxInputSource(_txtMessage),
			NoFastForward   = new CheckBoxInputSource(_chkNoFF),
			NoCommit        = new CheckBoxInputSource(_chkNoCommit),
			Squash          = new CheckBoxInputSource(_chkSquash),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		if(SpellingService.Enabled)
		{
			_speller = new TextBoxSpellChecker(_txtMessage, true);
		}

		GitterApplication.FontManager.InputFont.Apply(_txtMessage);

		_unmergedBranches = new HashSet<BranchBase>(_repository.Refs.GetUnmergedBranches());
		_references.DisableContextMenus = true;
		_references.Style = GitterApplication.DefaultStyle;
		_references.LoadData(_repository, ReferenceType.Branch, false, GlobalBehavior.GroupRemoteBranches,
			reference => _unmergedBranches.Contains(reference as BranchBase));

		_txtMessage.Height = _pnlOptions.Top - _txtMessage.Top - 6;

		_controller = new MergeController(repository) { View = this };
	}

	#endregion

	#region Methods

	private void Localize()
	{
		Text = Resources.StrMergeBranches;

		_references.Text    = Resources.StrNoBranchesToMergeWith;
		_lblMergeWith.Text  = Resources.StrMergeWith.AddColon();
		_grpOptions.Text    = Resources.StrOptions;
		_lblMessage.Text    = Resources.StrMessage.AddColon();
		_lnkAutoFormat.Text = Resources.StrAutoFormat;
		_chkNoFF.Text       = Resources.StrsNoFastForward;
		_chkNoCommit.Text   = Resources.StrsNoCommit;
		_chkSquash.Text     = Resources.StrSquash;
	}

	private void SetupTooltips()
	{
		ToolTipService.Register(_chkNoFF, Resources.TipNoFF);
		ToolTipService.Register(_chkNoCommit, Resources.TipMergeNoCommit);
		ToolTipService.Register(_chkSquash, Resources.TipSquash);
	}

	public void EnableMultipleBrunchesMerge()
	{
		_revisionsInput.Multiselect = true;
	}

	private void AutoFormatMessage()
	{
		var revisions = Revisions.Value;
		if(revisions is null || revisions.Count == 0)
		{
			return;
		}
		if(revisions.Count > 1)
		{
			try
			{
				_txtMessage.Text = _repository.Head.FormatMergeMessage(revisions);
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
			}
		}
		else
		{
			try
			{
				_txtMessage.Text = _repository.Head.FormatMergeMessage(revisions[0]);
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
			}
		}
	}

	private void OnAutoFormatLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		AutoFormatMessage();
	}

	#endregion

	#region Properties

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(642, 359));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrMerge;

	public IUserInputSource<IList<IRevisionPointer>> Revisions => _revisionsInput;

	public IUserInputSource<string> Message { get; }

	public IUserInputSource<bool> NoFastForward { get; }

	public IUserInputSource<bool> NoCommit { get; }

	public IUserInputSource<bool> Squash { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	#endregion

	#region IExecutableDialog Members

	/// <inheritdoc/>
	public bool Execute() => _controller.TryMerge();

	#endregion
}
