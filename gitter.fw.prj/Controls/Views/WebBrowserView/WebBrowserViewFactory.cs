namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;

	using Resources = gitter.Framework.Properties.Resources;

	public class WebBrowserViewFactory : ViewFactory
	{
		public static readonly new Guid Guid = new Guid("BF80569F-4544-4B0F-8C5B-213215E053AA");

		public WebBrowserViewFactory()
			: base(Guid, Resources.StrWebBrowser, Resources.ImgWebBrowser, true)
		{
			this.DefaultViewPosition = ViewPosition.SecondaryDocumentHost;
		}

		protected override ViewBase CreateViewCore(IDictionary<string, object> parameters)
		{
			return new WebBrowserView(Guid, parameters);
		}
	}
}
