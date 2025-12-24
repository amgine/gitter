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

using gitter.Git.AccessLayer;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
partial class UserIdentificationDialog : GitDialogBase, IExecutableDialog
{
	readonly struct DialogControls
	{
		public  readonly TextBox _txtUsername;
		public  readonly TextBox _txtEmail;
		private readonly LabelControl _lblUser;
		private readonly LabelControl _lblEmail;
		public  readonly IRadioButtonWidget _radSetUserGlobally;
		public  readonly IRadioButtonWidget _radSetUserForRepositoryOnly;
		private readonly LabelControl _lblUseThisUserNameAndEmail;

		public DialogControls(IGitterStyle? style = default)
		{
			style ??= GitterApplication.Style;

			_lblUser = new();
			_txtUsername = new();
			_lblEmail = new();
			_txtEmail = new();
			_lblUseThisUserNameAndEmail = new();
			_radSetUserGlobally = style.RadioButtonFactory.Create();
			_radSetUserForRepositoryOnly = style.RadioButtonFactory.Create();

			_radSetUserGlobally.IsChecked = true;

			GitterApplication.FontManager.InputFont.Apply(_txtUsername, _txtEmail);
		}

		public void Localize()
		{
			_lblUser.Text = Resources.StrUsername.AddColon();
			_lblEmail.Text = Resources.StrEmail.AddColon();
			_lblUseThisUserNameAndEmail.Text = Resources.StrsUseThisUserNameAndEmail.AddColon();
			_radSetUserGlobally.Text = Resources.StrsForCurrentWindowsUser;
			_radSetUserForRepositoryOnly.Text = Resources.StrsForCurrentRepositoryOnly;
		}

		public void Layout(Control parent)
		{
			var nameDec = new TextBoxDecorator(_txtUsername);
			var mailDec = new TextBoxDecorator(_txtEmail);

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(70),
						SizeSpec.Everything(),
					],
					rows:
					[
						/* 0 */ LayoutConstants.TextInputRowHeight,
						/* 1 */ LayoutConstants.TextInputRowHeight,
						/* 2 */ LayoutConstants.LabelRowSpacing,
						/* 3 */ LayoutConstants.LabelRowHeight,
						/* 4 */ LayoutConstants.RadioButtonRowHeight,
						/* 5 */ LayoutConstants.RadioButtonRowHeight,
						/* 6 */ SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblUser,  marginOverride: LayoutConstants.NoMargin), row: 0),
						new GridContent(new ControlContent(nameDec,   marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 0),
						new GridContent(new ControlContent(_lblEmail, marginOverride: LayoutConstants.NoMargin), row: 1),
						new GridContent(new ControlContent(mailDec,   marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 1),
						new GridContent(new ControlContent(_lblUseThisUserNameAndEmail, marginOverride: LayoutConstants.NoMargin), row: 3, columnSpan: 2),
						new GridContent(new WidgetContent(_radSetUserGlobally, marginOverride: LayoutConstants.GroupPadding), row: 4, columnSpan: 2),
						new GridContent(new WidgetContent(_radSetUserForRepositoryOnly, marginOverride: LayoutConstants.GroupPadding), row: 5, columnSpan: 2),
					]),
			};

			var tabIndex = 0;
			_lblUser.TabIndex = tabIndex++;
			nameDec.TabIndex = tabIndex++;
			_lblEmail.TabIndex = tabIndex++;
			mailDec.TabIndex = tabIndex++;
			_lblUseThisUserNameAndEmail.TabIndex = tabIndex++;
			_radSetUserGlobally.TabIndex = tabIndex++;
			_radSetUserForRepositoryOnly.TabIndex = tabIndex++;

			_lblUser.Parent = parent;
			nameDec.Parent = parent;
			_lblEmail.Parent = parent;
			mailDec.Parent = parent;
			_lblUseThisUserNameAndEmail.Parent = parent;
			_radSetUserGlobally.Parent = parent;
			_radSetUserForRepositoryOnly.Parent = parent;
		}
	}

	private readonly DialogControls _controls;
	private readonly RepositoryProvider _repositoryProvider;
	private readonly string? _oldUserName;
	private readonly string? _oldUserEmail;
	private readonly Repository? _repository;

	public UserIdentificationDialog(RepositoryProvider provider, Repository? repository)
	{
		Verify.Argument.IsNotNull(provider);

		_repository         = repository;
		_repositoryProvider = provider;

		Name = nameof(UserIdentificationDialog);

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		Text = Resources.StrUserIdentification;

		if(repository is not null)
		{
			var userName = repository.Configuration.TryGetParameter(GitConstants.UserNameParameter);
			if(userName is not null)
			{
				_controls._txtUsername.Text = _oldUserName = userName.Value;
			}
			else
			{
				_controls._txtUsername.Text = Environment.UserName;
			}
			var userEmail = repository.Configuration.TryGetParameter(GitConstants.UserEmailParameter);
			if(userEmail is not null)
			{
				_controls._txtEmail.Text = _oldUserEmail = userEmail.Value;
			}
			else
			{
				_controls._txtEmail.Text = string.Format("{0}@{1}", Environment.UserName, Environment.UserDomainName);
			}
		}
		else
		{
			_controls._radSetUserForRepositoryOnly.Enabled = false;
		}
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(DefaultWidth, 116));

	public string Username
	{
		get => _controls._txtUsername.Text;
		set => _controls._txtUsername.Text = value;
	}

	public string Email
	{
		get => _controls._txtEmail.Text;
		set => _controls._txtEmail.Text = value;
	}

	public bool SetGlobally
	{
		get => _controls._radSetUserGlobally.IsChecked;
		set
		{
			if(value)
			{
				_controls._radSetUserGlobally.IsChecked = true;
			}
			else
			{
				if(!_controls._radSetUserForRepositoryOnly.Enabled)
				{
					throw new InvalidOperationException();
				}
				_controls._radSetUserForRepositoryOnly.IsChecked = true;
			}
		}
	}

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_controls._txtUsername.Focus);
	}

	public void SetDefaults()
	{
		_controls._txtUsername.Text = Environment.UserName;
		_controls._txtEmail.Text = string.Format("{0}@{1}", Environment.UserName, Environment.UserDomainName);
	}

	public bool Execute()
	{
		var userName  = _controls._txtUsername.Text.Trim();
		var userEmail = _controls._txtEmail.Text.Trim();
		if(userName.Length == 0)
		{
			NotificationService.NotifyInputError(
				_controls._txtUsername,
				Resources.ErrInvalidUserName,
				Resources.ErrUserNameCannotBeEmpty);
			return false;
		}
		if(userEmail.Length == 0)
		{
			NotificationService.NotifyInputError(
				_controls._txtEmail,
				Resources.ErrInvalidEmail,
				Resources.ErrEmailCannotBeEmpty);
			return false;
		}
		try
		{
			if(_controls._radSetUserGlobally.IsChecked)
			{
				if(_oldUserName != userName || _oldUserEmail != userEmail)
				{
					if(_repository is not null)
					{
						static void Unset(ConfigParametersCollection cfg, string name)
						{
							if(!cfg.TryGetParameter(name, out var p)) return;
							try
							{
								p.Unset();
							}
							catch(GitException)
							{
							}
						}

						var cfg = _repository.Configuration;
						Unset(cfg, GitConstants.UserNameParameter);
						Unset(cfg, GitConstants.UserEmailParameter);
					}
					var accessor = _repositoryProvider.GitAccessor;
					if(accessor is not null)
					{
						static void SetValue(IGitAccessor accessor, string name, string value)
						{
							try
							{
							accessor.SetConfigValue.Invoke(
								new SetConfigValueRequest(name, value)
								{
									ConfigFile = ConfigFile.CurrentUser,
								});
							}
							catch(ConfigParameterDoesNotExistException)
							{
							}
						}

						SetValue(accessor, GitConstants.UserEmailParameter, userEmail);
						SetValue(accessor, GitConstants.UserNameParameter,  userName);
					}
					_repository?.Configuration.Refresh();
				}
				if(_repository is not null)
				{
					_repository.OnUserIdentityChanged();
				}
			}
			else
			{
				_repository?.Configuration.SetUserIdentity(userName, userEmail);
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
}
