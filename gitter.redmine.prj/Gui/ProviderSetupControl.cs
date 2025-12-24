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

namespace gitter.Redmine.Gui;

#nullable enable

using System;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;

using Resources = gitter.Redmine.Properties.Resources;

public partial class ProviderSetupControl : DialogBase, IExecutableDialog
{
	readonly struct DialogControls
	{
		private readonly LabelControl _lblServiceUri;
		public  readonly TextBox _txtServiceUri;
		private readonly LabelControl _lblProject;
		public  readonly TextBox _cmbProject;
		public  readonly TextBox _txtApiKey;
		private readonly LabelControl _lblApiKey;

		public DialogControls(IGitterStyle style)
		{
			style ??= GitterApplication.Style;

			_lblServiceUri = new();
			_txtServiceUri = new();
			_lblProject    = new();
			_cmbProject    = new();
			_txtApiKey     = new();
			_lblApiKey     = new();

			GitterApplication.FontManager.InputFont.Apply(_txtServiceUri, _txtApiKey, _cmbProject);
		}

		public void Localize()
		{
			_lblServiceUri.Text = Resources.StrServiceUri.AddColon();
			_lblApiKey.Text     = Resources.StrApiKey.AddColon();
			_lblProject.Text    = Resources.StrProject.AddColon();
		}

		public void Layout(Control parent)
		{
			var urlDec = new TextBoxDecorator(_txtServiceUri);
			var keyDec = new TextBoxDecorator(_txtApiKey);
			var projectDec = new TextBoxDecorator(_cmbProject);

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(94),
						SizeSpec.Everything(),
					],
					rows:
					[
						LayoutConstants.TextInputRowHeight,
						LayoutConstants.TextInputRowHeight,
						LayoutConstants.TextInputRowHeight,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblServiceUri, marginOverride: LayoutConstants.NoMargin),      row: 0, column: 0),
						new GridContent(new ControlContent(urlDec,         marginOverride: LayoutConstants.TextBoxMargin), row: 0, column: 1),
						new GridContent(new ControlContent(_lblApiKey,     marginOverride: LayoutConstants.NoMargin),      row: 1, column: 0),
						new GridContent(new ControlContent(keyDec,         marginOverride: LayoutConstants.TextBoxMargin), row: 1, column: 1),
						new GridContent(new ControlContent(_lblProject,    marginOverride: LayoutConstants.NoMargin),      row: 2, column: 0),
						new GridContent(new ControlContent(projectDec,     marginOverride: LayoutConstants.TextBoxMargin), row: 2, column: 1),
					]),
			};

			var tabIndex = 0;
			_lblServiceUri.TabIndex = tabIndex++;
			urlDec.TabIndex = tabIndex++;
			_lblApiKey.TabIndex = tabIndex++;
			keyDec.TabIndex = tabIndex++;
			_lblProject.TabIndex = tabIndex++;
			projectDec.TabIndex = tabIndex++;

			_lblServiceUri.Parent = parent;
			urlDec.Parent = parent;
			_lblApiKey.Parent = parent;
			keyDec.Parent = parent;
			_lblProject.Parent = parent;
			projectDec.Parent = parent;
		}
	}

	private readonly DialogControls _controls;
	private IRepository _repository;

	public ProviderSetupControl(IRepository repository)
	{
		Name = nameof(ProviderSetupControl);
		Text = Resources.StrRedmine;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		_repository = repository;

		var section = _repository.ConfigSection.TryGetSection("IssueTrackers")?.TryGetSection("Redmine");
		if(section is not null)
		{
			_controls._txtServiceUri.Text = section.GetValue<string>("ServiceUri", string.Empty);
			_controls._txtApiKey.Text     = section.GetValue<string>("ApiKey", string.Empty);
			_controls._cmbProject.Text    = section.GetValue<string>("ProjectId", string.Empty);
		}
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(400, 87));

	public string ServiceUri
	{
		get => _controls._txtServiceUri.Text.Trim();
		set => _controls._txtServiceUri.Text = value;
	}

	public string ApiKey
	{
		get => _controls._txtApiKey.Text.Trim();
		set => _controls._txtApiKey.Text = value;
	}

	public string ProjectId
	{
		get => _controls._cmbProject.Text.Trim();
		set => _controls._cmbProject.Text = value;
	}

	private bool ValidateServiceUri(string uri)
	{
		if(string.IsNullOrWhiteSpace(uri))
		{
			NotificationService.NotifyInputError(
				_controls._txtServiceUri,
				Resources.ErrNoServiceUriSpecified,
				Resources.ErrServiceUriCannotBeEmpty);
			return false;
		}
		if(!Uri.TryCreate(uri, UriKind.Absolute, out _))
		{
			NotificationService.NotifyInputError(
				_controls._txtServiceUri,
				Resources.ErrInvalidServiceUri,
				Resources.ErrServiceUriIsNotValid);
		}
		return true;
	}

	private bool ValidateApiKey(string apiKey)
	{
		if(string.IsNullOrWhiteSpace(apiKey))
		{
			NotificationService.NotifyInputError(
				_controls._txtApiKey,
				Resources.ErrNoApiKeySpecified,
				Resources.ErrApiKeyCannotBeEmpty);
			return false;
		}
		if(apiKey.Length != 40)
		{
			NotificationService.NotifyInputError(
				_controls._txtApiKey,
				Resources.ErrInvalidApiKey,
				Resources.ErrApiKeyMustContain40Characters);
			return false;
		}
		for(int i = 0; i < apiKey.Length; ++i)
		{
			if(!Uri.IsHexDigit(apiKey[i]))
			{
				NotificationService.NotifyInputError(
					_controls._txtApiKey,
					Resources.ErrInvalidApiKey,
					Resources.ErrApiKeyContainsInvalidCharacters);
				return false;
			}
		}
		return true;
	}

	private bool ValidateProjectId(string projectId)
	{
		if(string.IsNullOrWhiteSpace(projectId))
		{
			NotificationService.NotifyInputError(
				_controls._cmbProject,
				Resources.ErrNoProjectNameSpecified,
				Resources.ErrProjectNameCannotBeEmpty);
			return false;
		}
		return true;
	}

	public bool Execute()
	{
		var uri = ServiceUri;
		var key = ApiKey;
		var pid = ProjectId;

		if(!ValidateServiceUri(uri)) return false;
		if(!ValidateApiKey(key)) return false;
		if(!ValidateProjectId(pid)) return false;

		var section = _repository.ConfigSection
								 .GetCreateSection("IssueTrackers")
								 .GetCreateSection("Redmine");
		section.SetValue<string>("ServiceUri", uri);
		section.SetValue<string>("ApiKey", key);
		section.SetValue<string>("ProjectId", pid);

		return true;
	}
}
