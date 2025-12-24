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
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;

using Resources = gitter.GitLab.Properties.Resources;

partial class SetApiKeyDialog : DialogBase, IAsyncExecutableDialog, ISetApiKeyView
{
	readonly struct DialogControls
	{
		private readonly LabelControl _lblAPIKey;
		public  readonly TextBox _txtAPIKey;
		public  readonly LinkLabel _lnkTokens;

		public DialogControls()
		{
			_txtAPIKey = new();
			_lblAPIKey = new();
			_lnkTokens = new()
			{
				LinkColor         = GitterApplication.Style.Colors.HyperlinkText,
				ActiveLinkColor   = GitterApplication.Style.Colors.HyperlinkTextHotTrack,
				DisabledLinkColor = GitterApplication.Style.Colors.GrayText,
				TextAlign         = ContentAlignment.MiddleLeft,
				Padding = default,
			};

			GitterApplication.FontManager.InputFont.Apply(_txtAPIKey);
		}

		public void Localize()
		{
			_lblAPIKey.Text = Resources.StrAPIKey.AddColon();
			_lnkTokens.Text = Resources.StrsManageAccessTokens;
		}

		public void Layout(Control parent)
		{
			var keyDec = new TextBoxDecorator(_txtAPIKey);

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
						LayoutConstants.LabelRowHeight,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblAPIKey, marginOverride: LayoutConstants.NoMargin), row: 0),
						new GridContent(new ControlContent(keyDec,     marginOverride: LayoutConstants.TextBoxMargin), row: 0, column: 1),
						new GridContent(new ControlContent(_lnkTokens, marginOverride: DpiBoundValue.Padding(new(-3, 0, 0, 0))), row: 1, column: 1),
					]),
			};

			var tabIndex = 0;
			_lblAPIKey.TabIndex = tabIndex++;
			keyDec.TabIndex     = tabIndex++;
			_lnkTokens.TabIndex = tabIndex++;

			_lblAPIKey.Parent = parent;
			keyDec.Parent     = parent;
			_lnkTokens.Parent = parent;
		}
	}

	private readonly DialogControls _controls;

	public SetApiKeyDialog(HttpMessageInvoker httpMessageInvoker, IList<ServerInfo> servers, ServerInfo server)
	{
		Verify.Argument.IsNotNull(httpMessageInvoker);
		Verify.Argument.IsNotNull(servers);
		Verify.Argument.IsNotNull(server);

		Name = nameof(SetApiKeyDialog);
		Text = Resources.StrSetApiKey;

		ServiceUri = server.ServiceUri;

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
			APIKey = new TextBoxInputSource(_controls._txtAPIKey),
		};
		UserInputErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		_controls._lnkTokens.LinkClicked += OnManageAccessTokensClick;

		Controller = new SetApiKeyController(httpMessageInvoker, servers) { View = this, };
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(356, 44));

	public Uri ServiceUri { get; }

	public IUserInputSource<string?> APIKey { get; }

	public IUserInputErrorNotifier UserInputErrorNotifier { get; }

	private SetApiKeyController Controller { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_controls._txtAPIKey.Focus);
	}

	private void OnManageAccessTokensClick(object? sender, LinkLabelLinkClickedEventArgs e)
		=> UrlHelper.OpenPersonalAccessTokensPage(ServiceUri.ToString());

	/// <inheritdoc/>
	public async Task<bool> ExecuteAsync()
	{
		_controls._txtAPIKey.Enabled = false;
		try
		{
			return await Controller.TrySetApiKeyAsync();
		}
		finally
		{
			_controls._txtAPIKey.Enabled = true;
		}
	}
}
