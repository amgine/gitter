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
			if(repository == null) throw new ArgumentNullException("repository");
			_repository = repository;

			InitializeComponent();

			Text = Resources.StrCreateBranch;

			SetupReferenceNameInputBox(_txtName, ReferenceType.LocalBranch);

			_lblName.Text = Resources.StrName.AddColon();
			_lblRevision.Text = Resources.StrRevision.AddColon();
			_grpOptions.Text = Resources.StrOptions;
			_chkCheckoutAfterCreation.Text = Resources.StrCheckoutAfterCreation;
			if(GitFeatures.CheckoutOrphan.IsAvailableFor(RepositoryProvider.Git))
			{
				_chkOrphan.Text = Resources.StrlMakeOrphanBranch;
			}
			else
			{
				_chkOrphan.Text = Resources.StrlMakeOrphanBranch + " " +
					Resources.StrfVersionRequired.UseAsFormat(GitFeatures.CheckoutOrphan.RequiredVersion).SurroundWithBraces();
			}
			_chkCreateReflog.Text = Resources.StrCreateBranchReflog;

			_grpTracking.Text = Resources.StrTrackingMode;

			_trackingDefault.Text = Resources.StrDefault;
			_trackingDoNotTrack.Text = Resources.StrlDoNotTrack;
			_trackingTrack.Text = Resources.StrTrack;

			ToolTipService.Register(_chkCheckoutAfterCreation, Resources.TipCheckoutAfterCreation);
			ToolTipService.Register(_chkOrphan, Resources.TipOrphan);
			ToolTipService.Register(_chkCreateReflog, Resources.TipReflog);

			var logallrefupdates = _repository.Configuration.TryGetParameterValue(GitConstants.CoreLogAllRefUpdatesParameter);
			if(logallrefupdates != null && logallrefupdates == "true")
			{
				_chkCreateReflog.Checked = true;
				_chkCreateReflog.Enabled = false;
			}

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

		#region Properties

		protected override string ActionVerb
		{
			get { return Resources.StrCreate; }
		}

		/// <summary>Starting revision for new <see cref="Branch"/>.</summary>
		public string StartingRevision
		{
			get { return _txtRevision.Text; }
			set { _txtRevision.Text = value; }
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
				_chkOrphan.Enabled = GitFeatures.CheckoutOrphan.IsAvailableFor(RepositoryProvider.Git);
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
			var name		= _txtName.Text.Trim();
			var refspec		= _txtRevision.Text.Trim();
			var checkout	= _chkCheckoutAfterCreation.Checked;
			var orphan		= checkout && _chkOrphan.Checked && GitFeatures.CheckoutOrphan.IsAvailableFor(RepositoryProvider.Git);
			var reflog		= _chkCreateReflog.Checked;

			if(!ValidateNewBranchName(name, _txtName, _repository))
			{
				return false;
			}
			if(!ValidateRefspec(refspec, _txtRevision))
			{
				return false;
			}
			var trackingMode = TrackingMode;
			try
			{
				Cursor = Cursors.WaitCursor;
				var ptr = _repository.CreateRevisionPointer(refspec);
				if(orphan)
				{
					_repository.Refs.Heads.CreateOrphan(
						name,
						ptr,
						trackingMode,
						reflog);
				}
				else
				{
					_repository.Refs.Heads.Create(
						name,
						ptr,
						trackingMode,
						checkout,
						reflog);
				}
				Cursor = Cursors.Default;
			}
			catch(UnknownRevisionException)
			{
				Cursor = Cursors.Default;
				NotificationService.NotifyInputError(
					_txtRevision,
					Resources.ErrInvalidRevisionExpression,
					Resources.ErrRevisionIsUnknown);
				return false;
			}
			catch(BranchAlreadyExistsException)
			{
				Cursor = Cursors.Default;
				NotificationService.NotifyInputError(
					_txtName,
					Resources.ErrInvalidBranchName,
					Resources.ErrBranchAlreadyExists);
				return false;
			}
			catch(InvalidBranchNameException exc)
			{
				Cursor = Cursors.Default;
				NotificationService.NotifyInputError(
					_txtName,
					Resources.ErrInvalidBranchName,
					exc.Message);
				return false;
			}
			catch(GitException exc)
			{
				Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					string.Format(Resources.ErrFailedToCreateBranch, name),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		#endregion
	}
}
