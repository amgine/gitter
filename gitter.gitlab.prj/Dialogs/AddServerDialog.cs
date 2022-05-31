#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2020  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;

using gitter.Framework;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;

using Resources = gitter.GitLab.Properties.Resources;

partial class AddServerDialog : DialogBase, IAsyncExecutableDialog, IAddServerView
{
	public AddServerDialog(HttpMessageInvoker httpMessageInvoker, IList<ServerInfo> servers)
	{
		Verify.Argument.IsNotNull(httpMessageInvoker);
		Verify.Argument.IsNotNull(servers);

		InitializeComponent();

		Text = Resources.StrAddServer;

		_lblName.Text       = Resources.StrName.AddColon();
		_lblAPIKey.Text     = Resources.StrAPIKey.AddColon();
		_lblServiceUrl.Text = Resources.StrServiceUri.AddColon();

		var inputs = new IUserInputSource[]
		{
			ServerName = new TextBoxInputSource(_txtName),
			ServiceUrl = new TextBoxInputSource(_txtServiceUrl),
			APIKey     = new TextBoxInputSource(_txtAPIKey),
		};
		UserInputErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		GitterApplication.FontManager.InputFont.Apply(_txtName, _txtServiceUrl, _txtAPIKey);

		Controller = new AddServerController(httpMessageInvoker, servers) { View = this, };
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(356, 88));

	public IUserInputSource<string> ServerName { get; }

	public IUserInputSource<string> ServiceUrl { get; }

	public IUserInputSource<string> APIKey { get; }

	public IUserInputErrorNotifier UserInputErrorNotifier { get; }

	private AddServerController Controller { get; }

	protected override string ActionVerb => Resources.StrAdd;

	/// <inheritdoc/>
	public async Task<bool> ExecuteAsync()
	{
		_txtName.Enabled       = false;
		_txtServiceUrl.Enabled = false;
		_txtAPIKey.Enabled     = false;
		try
		{
			return await Controller.TryAddServerAsync();
		}
		finally
		{
			_txtName.Enabled       = true;
			_txtServiceUrl.Enabled = true;
			_txtAPIKey.Enabled     = true;
		}
	}
}
