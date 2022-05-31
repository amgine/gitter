#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.Controls;

using System;
using System.Drawing;
using System.Windows.Forms;

using Resources = gitter.Framework.Properties.Resources;

public partial class WebBrowserView : ViewBase
{
	private readonly WebBrowser _webBrowser;
	private readonly WebBrowserViewToolbar _toolbar;

	public WebBrowserView(Guid guid, IWorkingEnvironment environment)
		: base(guid, environment)
	{
		SuspendLayout();
		_webBrowser = new();
		_webBrowser.Dock = DockStyle.Fill;
		_webBrowser.MinimumSize = new(20, 20);
		_webBrowser.Name = nameof(_webBrowser);
		_webBrowser.ScriptErrorsSuppressed = true;
		_webBrowser.Size = new(555, 362);
		_webBrowser.TabIndex = 0;
		_webBrowser.Parent = this;
		Name = nameof(WebBrowserView);
		ResumeLayout(false);

		Text = Resources.StrWebBrowser;

		//_webBrowser.DocumentCompleted += OnWebBrowserDocumentCompleted;

		AddTopToolStrip(_toolbar = new WebBrowserViewToolbar(this));
	}

	private void OnWebBrowserDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
	{
		Text = _webBrowser.DocumentTitle;
	}

	public override IImageProvider ImageProvider => CommonIcons.WebBrowser;

	internal protected WebBrowser WebBrowser => _webBrowser;

	protected override void AttachViewModel(object viewModel)
	{
		if(viewModel is WebBrowserViewModel vm)
		{
			_webBrowser.Navigate(vm.Url);
		}
	}
}
