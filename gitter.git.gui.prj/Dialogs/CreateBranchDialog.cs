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

namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Controls;
	using gitter.Framework.Options;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Dialog for creating <see cref="Branch"/> object.</summary>
	[ToolboxItem(false)]
	public partial class CreateBranchDialog : GitDialogBase, IExecutableDialog
	{
		#region Data

		private Repository _repository;
		private bool _branchNameEdited;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="CreateBranchDialog"/>.</summary>
		/// <param name="repository"><see cref="Repository"/> to create <see cref="Branch"/> in.</param>
		/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		public CreateBranchDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();
			Localize();

			SetupReferenceNameInputBox(_txtName, ReferenceType.LocalBranch);

			var logallrefupdates = _repository.Configuration.TryGetParameterValue(GitConstants.CoreLogAllRefUpdatesParameter);
			if(logallrefupdates != null && logallrefupdates == "true")
			{
				_chkCreateReflog.Checked = true;
				_chkCreateReflog.Enabled = false;
			}

			ToolTipService.Register(_chkCheckoutAfterCreation, Resources.TipCheckoutAfterCreation);
			ToolTipService.Register(_chkOrphan, Resources.TipOrphan);
			ToolTipService.Register(_chkCreateReflog, Resources.TipReflog);
			ToolTipService.Register(_trackingTrack, Resources.TipTrack);

			_txtRevision.References.LoadData(
				_repository,
				ReferenceType.Reference,
				GlobalBehavior.GroupReferences,
				GlobalBehavior.GroupRemoteBranches);
			_txtRevision.References.Items[0].IsExpanded = true;

			GitterApplication.FontManager.InputFont.Apply(_txtName, _txtRevision);
			GlobalBehavior.SetupAutoCompleteSource(_txtRevision, _repository, ReferenceType.Branch);
		}

		#endregion

		#region Methods

		private void Localize()
		{
			Text = Resources.StrCreateBranch;

			_lblName.Text = Resources.StrName.AddColon();
			_lblRevision.Text = Resources.StrRevision.AddColon();

			_grpOptions.Text = Resources.StrOptions;
			_chkCheckoutAfterCreation.Text = Resources.StrCheckoutAfterCreation;
			if(GitFeatures.CheckoutOrphan.IsAvailableFor(_repository))
			{
				_chkOrphan.Text = Resources.StrlMakeOrphanBranch;
			}
			else
			{
				_chkOrphan.Text = Resources.StrlMakeOrphanBranch + " " +
					Resources.StrfVersionRequired.UseAsFormat(GitFeatures.CheckoutOrphan.RequiredVersion).SurroundWithBraces();
			}
			_chkCreateReflog.Text = Resources.StrCreateBranchReflog;

			_grpTracking.Text = Resources.StrsTrackingMode;
			_trackingDefault.Text = Resources.StrDefault;
			_trackingDoNotTrack.Text = Resources.StrlDoNotTrack;
			_trackingTrack.Text = Resources.StrTrack;
		}

		#endregion

		#region Properties

		protected override string ActionVerb
		{
			get { return Resources.StrCreate; }
		}

		/// <summary>Starting revision for new <see cref="Branch"/>.</summary>
		public string StartingRevision
		{
			get { return _txtRevision.Text.Trim(); }
			set { _txtRevision.Text = value.Trim(); }
		}

		/// <summary>Allow user to change <see cref="M:StartingRevision"/> property.</summary>
		public bool AllowChangingStartingRevision
		{
			get { return _txtRevision.Enabled; }
			set { _txtRevision.Enabled = value; }
		}

		/// <summary>Branch tracking mode.</summary>
		public BranchTrackingMode TrackingMode
		{
			get
			{
				if(_trackingTrack.Checked)
				{
					return BranchTrackingMode.Tracking;
				}
				if(_trackingDoNotTrack.Checked)
				{
					return BranchTrackingMode.NotTracking;
				}
				return BranchTrackingMode.Default;
			}
			set
			{
				switch(value)
				{
					case BranchTrackingMode.Default:
						_trackingDefault.Checked = true;
						break;
					case BranchTrackingMode.NotTracking:
						_trackingDoNotTrack.Checked = true;
						break;
					case BranchTrackingMode.Tracking:
						_trackingTrack.Checked = true;
						break;
					default:
						throw new ArgumentException("value");
				}
			}
		}

		/// <summary>Create a new orphan branch.</summary>
		public bool Orphan
		{
			get { return _chkOrphan.Checked; }
			set { _chkOrphan.Checked = value; }
		}

		/// <summary>Branch name.</summary>
		public string BranchName
		{
			get { return _txtName.Text; }
			set
			{
				_txtName.Text = value;
				_branchNameEdited = !string.IsNullOrEmpty(_txtName.Text);
			}
		}

		/// <summary>Allow user to change <see cref="M:BranchName"/> property.</summary>
		public bool AllowChangingBranchName
		{
			get { return !_txtName.ReadOnly; }
			set { _txtName.ReadOnly = !value; }
		}

		#endregion

		#region Event Handlers

		private void OnBranchNameChanged(object sender, EventArgs e)
		{
			_branchNameEdited = !string.IsNullOrEmpty(_txtName.Text);
		}

		private void OnRevisionChanged(object sender, EventArgs e)
		{
			if(!_branchNameEdited)
			{
				var branchName = _txtRevision.Text.Trim();
				var branch = _repository.Refs.Remotes.TryGetItem(branchName);
				if(branch != null)
				{
					_txtName.Text = branch.Name.Substring(branch.Name.LastIndexOf('/')+1);
					_branchNameEdited = false;
				}
			}
		}

		private void OnCheckoutAfterCreationCheckedChanged(object sender, EventArgs e)
		{
			if(_chkCheckoutAfterCreation.Checked)
			{
				_chkOrphan.Enabled = GitFeatures.CheckoutOrphan.IsAvailableFor(_repository);
			}
			else
			{
				_chkOrphan.Enabled = false;
			}
		}

		#endregion

		#region IExecutableDialog

		public bool Execute()
		{
			var branchName	= _txtName.Text.Trim();
			var refspec		= _txtRevision.Text.Trim();
			var checkout	= _chkCheckoutAfterCreation.Checked;
			var orphan		= checkout && _chkOrphan.Checked && GitFeatures.CheckoutOrphan.IsAvailableFor(_repository);
			var reflog		= _chkCreateReflog.Checked;
			var existent	= _repository.Refs.Heads.TryGetItem(branchName);

			if(!ValidateBranchName(branchName, _txtName))
			{
				return false;
			}
			if(!ValidateRefspec(refspec, _txtRevision))
			{
				return false;
			}
			if(existent != null)
			{
				if(GitterApplication.MessageBoxService.Show(
					this,
					Resources.StrAskBranchExists.UseAsFormat(branchName),
					Resources.StrBranch,
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Question) == DialogResult.Yes)
				{
					var ptr = _repository.GetRevisionPointer(refspec);
					try
					{
						if(existent.IsCurrent)
						{
							ResetMode mode;
							using(var dlg = new SelectResetModeDialog())
							{
								if(dlg.Run(this) != DialogResult.OK)
								{
									return false;
								}
								mode = dlg.ResetMode;
							}
							using(this.ChangeCursor(Cursors.WaitCursor))
							{
								_repository.Head.Reset(ptr, mode);
							}
						}
						else
						{
							using(this.ChangeCursor(Cursors.WaitCursor))
							{
								existent.Reset(ptr);
								if(checkout)
								{
									existent.Checkout(true);
								}
							}
						}
					}
					catch(UnknownRevisionException)
					{
						NotificationService.NotifyInputError(
							_txtRevision,
							Resources.ErrInvalidRevisionExpression,
							Resources.ErrRevisionIsUnknown);
						return false;
					}
					catch(GitException exc)
					{
						GitterApplication.MessageBoxService.Show(
							this,
							exc.Message,
							Resources.ErrFailedToReset,
							MessageBoxButton.Close,
							MessageBoxIcon.Error);
						return false;
					}
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				var trackingMode = TrackingMode;
				try
				{
					using(this.ChangeCursor(Cursors.WaitCursor))
					{
						var ptr = _repository.GetRevisionPointer(refspec);
						if(orphan)
						{
							_repository.Refs.Heads.CreateOrphan(
								branchName,
								ptr,
								trackingMode,
								reflog);
						}
						else
						{
							_repository.Refs.Heads.Create(
								branchName,
								ptr,
								trackingMode,
								checkout,
								reflog);
						}
					}
				}
				catch(UnknownRevisionException)
				{
					NotificationService.NotifyInputError(
						_txtRevision,
						Resources.ErrInvalidRevisionExpression,
						Resources.ErrRevisionIsUnknown);
					return false;
				}
				catch(BranchAlreadyExistsException)
				{
					NotificationService.NotifyInputError(
						_txtName,
						Resources.ErrInvalidBranchName,
						Resources.ErrBranchAlreadyExists);
					return false;
				}
				catch(InvalidBranchNameException exc)
				{
					NotificationService.NotifyInputError(
						_txtName,
						Resources.ErrInvalidBranchName,
						exc.Message);
					return false;
				}
				catch(GitException exc)
				{
					GitterApplication.MessageBoxService.Show(
						this,
						exc.Message,
						string.Format(Resources.ErrFailedToCreateBranch, branchName),
						MessageBoxButton.Close,
						MessageBoxIcon.Error);
					return false;
				}
				return true;
			}
		}

		#endregion
	}
}
