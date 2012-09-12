namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

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
			Verify.Argument.IsNotNull(view, "view");

			_view = view;

			Items.Add(_btnGoBack = new ToolStripButton(
				Resources.StrBack,
				Resources.ImgGoBack,
				(s, e) =>
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
				Resources.StrForward,
				Resources.ImgGoForward,
				(s, e) =>
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
}
