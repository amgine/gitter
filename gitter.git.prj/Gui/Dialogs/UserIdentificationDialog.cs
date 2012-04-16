namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Options;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	public partial class UserIdentificationDialog : GitDialogBase, IExecutableDialog
	{
		private Repository _repository;
		private string _oldUserName;
		private string _oldUserEmail;

		public UserIdentificationDialog(Repository repository)
		{
			_repository = repository;

			InitializeComponent();

			Text = Resources.StrUserIdentification;

			_lblUser.Text = Resources.StrUsername.AddColon();
			_lblEmail.Text = Resources.StrEmail.AddColon();
			_lblUseThisUserNameAndEmail.Text = Resources.StrsUseThisUserNameAndEmail.AddColon();
			_radSetUserGlobally.Text = Resources.StrsForCurrentWindowsUser;
			_radSetUserForRepositoryOnly.Text = Resources.StrsForCurrentRepositoryOnly;

			if(repository != null)
			{
				var userName = repository.Configuration.TryGetParameter(GitConstants.UserNameParameter);
				if(userName != null)
				{
					_txtUsername.Text = _oldUserName = userName.Value;
				}
				else
				{
					_txtUsername.Text = Environment.UserName;
				}
				var userEmail = repository.Configuration.TryGetParameter(GitConstants.UserEmailParameter);
				if(userEmail != null)
				{
					_txtEmail.Text = _oldUserEmail = userEmail.Value;
				}
				else
				{
					_txtEmail.Text = string.Format("{0}@{1}", Environment.UserName, Environment.UserDomainName);
				}
			}
			else
			{
				_radSetUserForRepositoryOnly.Enabled = false;
			}

			GitterApplication.FontManager.InputFont.Apply(_txtUsername, _txtEmail);
		}

		public string Username
		{
			get { return _txtUsername.Text; }
			set { _txtUsername.Text = value; }
		}

		public string Email
		{
			get { return _txtEmail.Text; }
			set { _txtEmail.Text = value; }
		}

		public bool SetGlobally
		{
			get { return _radSetUserGlobally.Checked; }
			set
			{
				if(value)
				{
					_radSetUserGlobally.Checked = true;
				}
				else
				{
					if(!_radSetUserForRepositoryOnly.Enabled)
					{
						throw new InvalidOperationException();
					}
					_radSetUserForRepositoryOnly.Checked = true;
				}
			}
		}

		public void SetDefaults()
		{
			_txtUsername.Text = Environment.UserName;
			_txtEmail.Text = string.Format("{0}@{1}", Environment.UserName, Environment.UserDomainName);
		}

		#region IExecutableDialog Members

		public bool Execute()
		{
			string userName = _txtUsername.Text.Trim();
			string userEmail = _txtEmail.Text.Trim();
			if(userName.Length == 0)
			{
				NotificationService.NotifyInputError(
					_txtUsername,
					Resources.ErrInvalidUserName,
					Resources.ErrUserNameCannotBeEmpty);
				return false;
			}
			if(userEmail.Length == 0)
			{
				NotificationService.NotifyInputError(
					_txtEmail,
					Resources.ErrInvalidEmail,
					Resources.ErrEmailCannotBeEmpty);
				return false;
			}
			try
			{
				if(_radSetUserGlobally.Checked)
				{
					if(_oldUserName != userName || _oldUserEmail != userEmail)
					{
						if(_repository != null)
						{
							var cpUserName = _repository.Configuration.TryGetParameter(GitConstants.UserNameParameter);
							if(cpUserName != null)
							{
								try
								{
									cpUserName.Unset();
								}
								catch(GitException) { }
							}
							var cpUserEmail = _repository.Configuration.TryGetParameter(GitConstants.UserEmailParameter);
							if(cpUserEmail != null)
							{
								try
								{
									cpUserEmail.Unset();
								}
								catch(GitException) { }
							}
						}
						RepositoryProvider.Git.SetConfigValue(
							new SetConfigValueParameters(GitConstants.UserNameParameter, userName)
							{
								ConfigFile = ConfigFile.User,
							});
						RepositoryProvider.Git.SetConfigValue(
							new SetConfigValueParameters(GitConstants.UserEmailParameter, userEmail)
							{
								ConfigFile = ConfigFile.User,
							});
						if(_repository != null)
						{
							_repository.Configuration.Refresh();
						}
					}
				}
				else
				{
					using(_repository.Monitor.BlockNotifications(
						RepositoryNotifications.ConfigUpdatedNotification))
					{
						_repository.Configuration.SetValue(GitConstants.UserNameParameter, userName);
						_repository.Configuration.SetValue(GitConstants.UserEmailParameter, userEmail);
					}
				}
				if(_repository != null)
				{
					_repository.InvokeUserIdentityChanged();
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToSetParameter,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		#endregion
	}
}
