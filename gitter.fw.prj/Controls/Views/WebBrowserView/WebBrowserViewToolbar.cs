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

namespace gitter.Framework.Controls;

using System;
using System.ComponentModel;
using System.Windows.Forms;

using Resources = gitter.Framework.Properties.Resources;

[DesignerCategory("")]
sealed class WebBrowserViewToolbar : ToolStrip
{
	private readonly WebBrowserView _view;

	private readonly ToolStripButton _btnGoBack;
	private readonly ToolStripButton _btnGoForward;
	private readonly ToolStripTextBox _addressBox;
	//private readonly ToolStripButton _btnStop;
	//private readonly ToolStripButton _btnRefresh;

	public WebBrowserViewToolbar(WebBrowserView view)
	{
		Verify.Argument.IsNotNull(view);

		_view = view;
		Items.Add(_btnGoBack = new ToolStripButton(
			Resources.StrBack, null,
			(_, _) =>
			{
				if(_view.WebBrowser.CanGoBack)
				{
					_view.WebBrowser.GoBack();
				}
			})
			{
				Enabled = _view.WebBrowser.CanGoBack,
			});

		Items.Add(_btnGoForward = new ToolStripButton(
			Resources.StrForward, null,
			(_, _) =>
			{
				if(_view.WebBrowser.CanGoForward)
				{
					_view.WebBrowser.GoForward();
				}
			})
			{
				Enabled = _view.WebBrowser.CanGoForward,
				DisplayStyle = ToolStripItemDisplayStyle.Image,
			});

		Items.Add(_addressBox = new ToolStripTextBox("AddressBox")
			{
				AutoSize = false,
				Width = 250,
			});

		var dpiBindings = new DpiBindings(this);
		dpiBindings.BindImage(_btnGoBack,    CommonIcons.NavBack);
		dpiBindings.BindImage(_btnGoForward, CommonIcons.NavForward);

		_view.WebBrowser.CanGoBackChanged += OnWebBrowserCanGoBackChanged;
		_view.WebBrowser.CanGoForwardChanged += OnWebBrowserCanGoForwardChanged;
		_view.WebBrowser.Navigating += OnWebBrowserNavigating;
	}

	protected override void OnResize(EventArgs e)
	{
		_addressBox.Width = Width - 120;
		base.OnResize(e);
	}

	private void OnWebBrowserNavigating(object sender, WebBrowserNavigatingEventArgs e)
	{
		if(e.TargetFrameName == string.Empty)
		{
			_addressBox.Text = e.Url.ToString();
		}
	}

	private void OnWebBrowserCanGoBackChanged(object sender, EventArgs e)
	{
		_btnGoBack.Enabled = _view.WebBrowser.CanGoBack;
	}

	private void OnWebBrowserCanGoForwardChanged(object sender, EventArgs e)
	{
		_btnGoForward.Enabled = _view.WebBrowser.CanGoForward;
	}

	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_view.WebBrowser.CanGoBackChanged -= OnWebBrowserCanGoBackChanged;
			_view.WebBrowser.CanGoForwardChanged -= OnWebBrowserCanGoForwardChanged;
			_view.WebBrowser.Navigating -= OnWebBrowserNavigating;
		}
		base.Dispose(disposing);
	}
}
