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

namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
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
				get { return _multiselect; }
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
					var revPointerlistItem = item as IRevisionPointerListItem;
					if(revPointerlistItem != null)
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
						var item = Control.SelectedItems[0] as IRevisionPointerListItem;
						if(item == null)
						{
							return null;
						}
						else
						{
							var revision = item.RevisionPointer;
							if(revision == null)
							{
								return null;
							}
							return new IRevisionPointer[] { revision };
						}
					}
				}
			}

			private static void SelItemCheckedState(CustomListBoxItem item, IList<IRevisionPointer> list)
			{
				if(item.CheckedState != CheckedState.Unavailable)
				{
					var revPointerlistItem = item as IRevisionPointerListItem;
					if(revPointerlistItem != null)
					{
						if(list == null || !list.Contains(revPointerlistItem.RevisionPointer))
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
				if(revisionPointer == null)
				{
					return null;
				}
				foreach(var item in items)
				{
					var revItem = item as IRevisionPointerListItem;
					if(revItem != null)
					{
						if(revItem.RevisionPointer == revisionPointer)
						{
							return item;
						}
					}
					else
					{
						var res = TryFindItem(item.Items, revisionPointer);
						if(res != null)
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
					if(value == null || value.Count == 0)
					{
						Control.SelectedItems.Clear();
					}
					else
					{
						Control.SelectedItems.Clear();
						var item = TryFindItem(Control.Items, value[0]);
						if(item != null)
						{
							item.FocusAndSelect();
						}
					}
				}
			}
		}

		#endregion

		#region Data

		private readonly Repository _repository;
		private readonly IList<BranchBase> _unmergedBranches;
		private TextBoxSpellChecker _speller;
		private readonly RevisionsInput _revisionsInput;
		private readonly IUserInputSource<string> _messageInput;
		private readonly IUserInputSource<bool> _noFastForwardInput;
		private readonly IUserInputSource<bool> _noCommitInput;
		private readonly IUserInputSource<bool> _squashInput;
		private readonly IUserInputErrorNotifier _errorNotifier;
		private readonly IMergeController _controller;

		#endregion

		#region .ctor

		public MergeDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();
			Localize();
			SetupTooltips();

			var inputs = new IUserInputSource[]
			{
				_revisionsInput     = new RevisionsInput(_references),
				_messageInput       = new TextBoxInputSource(_txtMessage),
				_noFastForwardInput = new CheckBoxInputSource(_chkNoFF),
				_noCommitInput      = new CheckBoxInputSource(_chkNoCommit),
				_squashInput        = new CheckBoxInputSource(_chkSquash),
			};
			_errorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

			if(SpellingService.Enabled)
			{
				_speller = new TextBoxSpellChecker(_txtMessage, true);
			}

			GitterApplication.FontManager.InputFont.Apply(_txtMessage);

			_unmergedBranches = _repository.Refs.GetUnmergedBranches();
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
			if(revisions == null || revisions.Count == 0)
			{
				return;
			}
			if(revisions.Count > 1)
			{
				try
				{
					_txtMessage.Text = _repository.Head.FormatMergeMessage(revisions);
				}
				catch(Exception exc)
				{
					if(exc.IsCritical())
					{
						throw;
					}
				}
			}
			else
			{
				try
				{
					_txtMessage.Text = _repository.Head.FormatMergeMessage(revisions[0]);
				}
				catch(Exception exc)
				{
					if(exc.IsCritical())
					{
						throw;
					}
				}
			}
		}

		private void OnAutoFormatLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			AutoFormatMessage();
		}

		#endregion

		#region Properties

		protected override string ActionVerb
		{
			get { return Resources.StrMerge; }
		}

		public IUserInputSource<IList<IRevisionPointer>> Revisions
		{
			get { return _revisionsInput; }
		}

		public IUserInputSource<string> Message
		{
			get { return _messageInput; }
		}

		public IUserInputSource<bool> NoFastForward
		{
			get { return _noFastForwardInput; }
		}

		public IUserInputSource<bool> NoCommit
		{
			get { return _noCommitInput; }
		}

		public IUserInputSource<bool> Squash
		{
			get { return _squashInput; }
		}

		public IUserInputErrorNotifier ErrorNotifier
		{
			get { return _errorNotifier; }
		}

		#endregion

		#region IExecutableDialog Members

		public bool Execute()
		{
			return _controller.TryMerge();
		}

		#endregion
	}
}
