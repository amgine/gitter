namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	public partial class WebBrowserView : ViewBase
	{
		private static readonly Image _image = Resources.ImgWebBrowser;

		private readonly WebBrowserViewToolbar _toolbar;

		public WebBrowserView()
		{
			InitializeComponent();
		}

		public WebBrowserView(Guid guid, IDictionary<string, object> parameters)
			: base(guid, parameters)
		{
			InitializeComponent();

			Text = Resources.StrWebBrowser;

			//_webBrowser.DocumentCompleted += OnWebBrowserDocumentCompleted;

			AddTopToolStrip(_toolbar = new WebBrowserViewToolbar(this));

			ApplyParameters(parameters);
		}

		private void OnWebBrowserDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			Text = _webBrowser.DocumentTitle;
		}

		public override Image Image
		{
			get { return _image; }
		}

		internal protected WebBrowser WebBrowser
		{
			get { return _webBrowser; }
		}

		public override void ApplyParameters(IDictionary<string, object> parameters)
		{
			 base.ApplyParameters(parameters);

			 var url = parameters["url"].ToString();
			 _webBrowser.Navigate(url);
		}
	}
}
