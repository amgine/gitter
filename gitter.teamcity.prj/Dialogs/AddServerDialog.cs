#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.TeamCity;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;

using Resources = gitter.TeamCity.Properties.Resources;

partial class AddServerDialog : DialogBase, IAsyncExecutableDialog, IAddServerView
{
	readonly struct DialogControls
	{
		private readonly LabelControl _lblName;
		private readonly LabelControl _lblServiceUrl;
		private readonly LabelControl _lblAPIKey;
		public  readonly TextBox _txtName;
		public  readonly TextBox _txtServiceUrl;
		public  readonly TextBox _txtAPIKey;
		public  readonly LinkLabel _lnkTokens;

		public DialogControls()
		{
			_lblName = new();
			_lblServiceUrl = new();
			_txtName = new();
			_txtServiceUrl = new();
			_txtAPIKey = new();
			_lblAPIKey = new();
			_lnkTokens = new()
			{
				LinkColor = GitterApplication.Style.Colors.HyperlinkText,
				ActiveLinkColor = GitterApplication.Style.Colors.HyperlinkTextHotTrack,
				DisabledLinkColor = GitterApplication.Style.Colors.GrayText,
				TextAlign = ContentAlignment.MiddleLeft,
				Padding = default,
			};
			_lnkTokens.Links[0].Enabled = false;

			GitterApplication.FontManager.InputFont.Apply(_txtName, _txtServiceUrl, _txtAPIKey);
		}

		public void Localize()
		{
			_lblName.Text       = Resources.StrName.AddColon();
			_lblAPIKey.Text     = Resources.StrApiKey.AddColon();
			_lblServiceUrl.Text = Resources.StrServiceUri.AddColon();
			_lnkTokens.Text     = Resources.StrsManageAccessTokens;
		}

		public void Layout(Control parent)
		{
			var nameDec = new TextBoxDecorator(_txtName);
			var urlDec  = new TextBoxDecorator(_txtServiceUrl);
			var keyDec  = new TextBoxDecorator(_txtAPIKey);

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
						LayoutConstants.LabelRowHeight,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblName,       marginOverride: LayoutConstants.NoMargin), row: 0),
						new GridContent(new ControlContent(nameDec,        marginOverride: LayoutConstants.TextBoxMargin), row: 0, column: 1),
						new GridContent(new ControlContent(_lblServiceUrl, marginOverride: LayoutConstants.NoMargin), row: 1),
						new GridContent(new ControlContent(urlDec,         marginOverride: LayoutConstants.TextBoxMargin), row: 1, column: 1),
						new GridContent(new ControlContent(_lblAPIKey,     marginOverride: LayoutConstants.NoMargin), row: 2),
						new GridContent(new ControlContent(keyDec,         marginOverride: LayoutConstants.TextBoxMargin), row: 2, column: 1),
						new GridContent(new ControlContent(_lnkTokens,     marginOverride: DpiBoundValue.Padding(new(-3, 0, 0, 0))), row: 3, column: 1),
					]),
			};

			var tabIndex = 0;
			_lblName.TabIndex = tabIndex++;
			nameDec.TabIndex = tabIndex++;
			_lblServiceUrl.TabIndex = tabIndex++;
			urlDec.TabIndex = tabIndex++;
			_lblAPIKey.TabIndex = tabIndex++;
			keyDec.TabIndex = tabIndex++;
			_lnkTokens.TabIndex = tabIndex++;

			_lblName.Parent = parent;
			nameDec.Parent = parent;
			_lblServiceUrl.Parent = parent;
			urlDec.Parent = parent;
			_lblAPIKey.Parent = parent;
			keyDec.Parent = parent;
			_lnkTokens.Parent = parent;
		}
	}

	private readonly DialogControls _controls;

	public AddServerDialog(HttpMessageInvoker httpMessageInvoker, IList<ServerInfo> servers)
	{
		Verify.Argument.IsNotNull(httpMessageInvoker);
		Verify.Argument.IsNotNull(servers);

		Name = nameof(AddServerDialog);
		Text = Resources.StrAddServer;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new();
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		var inputs = new IUserInputSource[]
		{
			ServerName = new TextBoxInputSource(_controls._txtName),
			ServiceUrl = new TextBoxInputSource(_controls._txtServiceUrl),
			APIKey     = new TextBoxInputSource(_controls._txtAPIKey),
		};
		UserInputErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		_controls._lnkTokens.LinkClicked += OnManageAccessTokensClick;
		_controls._txtServiceUrl.TextChanged += (_, _) => _controls._lnkTokens.Links[0].Enabled = _controls._txtServiceUrl.TextLength != 0;

		Controller = new AddServerController(httpMessageInvoker, servers) { View = this, };
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(356, 102));

	public IUserInputSource<string?> ServerName { get; }

	public IUserInputSource<string?> ServiceUrl { get; }

	public IUserInputSource<string?> APIKey { get; }

	public IUserInputErrorNotifier UserInputErrorNotifier { get; }

	private AddServerController Controller { get; }

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrAdd;

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_controls._txtName.Focus);
	}

	private void OnManageAccessTokensClick(object? sender, LinkLabelLinkClickedEventArgs e)
		=> UrlHelper.OpenPersonalAccessTokensPage(_controls._txtServiceUrl.Text.Trim());

	/// <inheritdoc/>
	public async Task<bool> ExecuteAsync()
	{
		_controls._txtName.Enabled       = false;
		_controls._txtServiceUrl.Enabled = false;
		_controls._txtAPIKey.Enabled     = false;
		try
		{
			return await Controller.TryAddServerAsync();
		}
		finally
		{
			_controls._txtName.Enabled       = true;
			_controls._txtServiceUrl.Enabled = true;
			_controls._txtAPIKey.Enabled     = true;
		}
	}
}
