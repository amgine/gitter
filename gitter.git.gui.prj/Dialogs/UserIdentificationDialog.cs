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
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Options;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class UserIdentificationDialog : GitDialogBase, IExecutableDialog
	{
		#region Data

		private readonly IWorkingEnvironment _environment;
		private RepositoryProvider _repositoryProvider;
		private Repository _repository;
		private string _oldUserName;
		private string _oldUserEmail;

		#endregion

		public UserIdentificationDialog(IWorkingEnvironment environment, Repository repository)
		{
			Verify.Argument.IsNotNull(environment, "environment");

			_environment = environment;
			_repository = repository;
			_repositoryProvider = environment.GetRepositoryProvider<RepositoryProvider>();

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
								catch(GitException)
								{
								}
							}
							var cpUserEmail = _repository.Configuration.TryGetParameter(GitConstants.UserEmailParameter);
							if(cpUserEmail != null)
							{
								try
								{
									cpUserEmail.Unset();
								}
								catch(GitException)
								{
								}
							}
						}
						try
						{
							_repositoryProvider.GitAccessor.SetConfigValue.Invoke(
								new SetConfigValueParameters(GitConstants.UserEmailParameter, userEmail)
								{
									ConfigFile = ConfigFile.User,
								});
						}
						catch(ConfigParameterDoesNotExistException)
						{
						}
						try
						{
							_repositoryProvider.GitAccessor.SetConfigValue.Invoke(
								new SetConfigValueParameters(GitConstants.UserNameParameter, userName)
								{
									ConfigFile = ConfigFile.User,
								});
						}
						catch(ConfigParameterDoesNotExistException)
						{
						}
						if(_repository != null)
						{
							_repository.Configuration.Refresh();
						}
					}
					if(_repository != null)
					{
						_repository.OnUserIdentityChanged();
					}
				}
				else
				{
					_repository.Configuration.SetUserIdentity(userName, userEmail);
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
