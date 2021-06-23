﻿#region Copyright Notice
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

namespace gitter.TeamCity.Gui
{
	using System;
	using System.Text;

	using gitter.Framework;

	using Resources = gitter.TeamCity.Properties.Resources;

	public partial class ProviderSetupControl : DialogBase, IExecutableDialog
	{
		private IRepository _repository;

		public ProviderSetupControl(IRepository repository)
		{
			InitializeComponent();

			GitterApplication.FontManager.InputFont.Apply(_txtUsername);

			_repository = repository;

			Text = Resources.StrTeamCity;
			_lblServiceUri.Text = Resources.StrServiceUri.AddColon();
			_lblUsername.Text   = Resources.StrUsername.AddColon();
			_lblPassword.Text   = Resources.StrPassword.AddColon();
			_lblProject.Text    = Resources.StrProject.AddColon();

			var section = _repository.ConfigSection.TryGetSection("IssueTrackers");
			if(section is not null)
			{
				section = section.TryGetSection("TeamCity");
			}
			if(section is not null)
			{
				_txtServiceUri.Text = section.GetValue<string>("ServiceUri", string.Empty);
				_txtUsername.Text   = Unmask(section.GetValue<string>("Username", string.Empty));
				_txtPassword.Text   = Unmask(section.GetValue<string>("Password", string.Empty));
				_cmbProject.Text    = section.GetValue<string>("ProjectId", string.Empty);
			}
		}

		private static string Mask(string str)
		{
			if(str == string.Empty) return string.Empty;

			return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
		}

		private static string Unmask(string str)
		{
			if(str == string.Empty) return string.Empty;

			return Encoding.UTF8.GetString(Convert.FromBase64String(str));
		}

		public string ServiceUri
		{
			get => _txtServiceUri.Text.Trim();
			set => _txtServiceUri.Text = value;
		}

		public string Username
		{
			get => _txtUsername.Text.Trim();
			set => _txtUsername.Text = value;
		}

		public string Password
		{
			get => _txtPassword.Text;
			set => _txtPassword.Text = value;
		}

		public string ProjectId
		{
			get => _cmbProject.Text.Trim();
			set => _cmbProject.Text = value;
		}

		private bool ValidateServiceUri(string uri)
		{
			if(string.IsNullOrWhiteSpace(uri))
			{
				NotificationService.NotifyInputError(
					_txtServiceUri,
					Resources.ErrNoServiceUriSpecified,
					Resources.ErrServiceUriCannotBeEmpty);
				return false;
			}
			Uri dummy;
			if(!Uri.TryCreate(uri, UriKind.Absolute, out dummy))
			{
				NotificationService.NotifyInputError(
					_txtServiceUri,
					Resources.ErrInvalidServiceUri,
					Resources.ErrServiceUriIsNotValid);
			}
			return true;
		}

		private bool ValidateUsername(string username)
		{
			if(string.IsNullOrWhiteSpace(username))
			{
				NotificationService.NotifyInputError(
					_txtUsername,
					Resources.ErrNoUsernameSpecified,
					Resources.ErrUsernameCannotBeEmpty);
				return false;
			}
			return true;
		}

		private bool ValidateProjectId(string projectId)
		{
			if(string.IsNullOrWhiteSpace(projectId))
			{
				NotificationService.NotifyInputError(
					_cmbProject,
					Resources.ErrNoProjectNameSpecified,
					Resources.ErrProjectNameCannotBeEmpty);
				return false;
			}
			return true;
		}

		public bool Execute()
		{
			var uri      = ServiceUri;
			var username = Username;
			var password = Password;
			var pid      = ProjectId;

			if(!ValidateServiceUri(uri))    return false;
			if(!ValidateUsername(username)) return false;
			if(!ValidateProjectId(pid))     return false;

			var section = _repository.ConfigSection
									 .GetCreateSection("IssueTrackers")
									 .GetCreateSection("TeamCity");
			section.SetValue<string>("ServiceUri", uri);
			section.SetValue<string>("Username", Mask(username));
			section.SetValue<string>("Password", Mask(password));
			section.SetValue<string>("ProjectId", pid);

			return true;
		}
	}
}
