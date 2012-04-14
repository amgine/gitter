namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;

	/// <summary>Dock markers of <see cref="ViewHost"/>.</summary>
	sealed class ViewHostDockMarkers : DockMarkers<ViewHostDockMarker>
	{
		private readonly ViewHost _dockHost;

		/// <summary>Initializes a new instance of the <see cref="ViewHostDockMarkers"/> class.</summary>
		/// <param name="dockHost"><see cref="ViewHost"/> which is the source of dock markers.</param>
		public ViewHostDockMarkers(ViewHost dockHost)
		{
			if(dockHost == null)
				throw new ArgumentNullException("dockHost");

			_dockHost = dockHost;
		}

		/// <summary><see cref="ViewHost"/> which is the source of dock markers.</summary>
		/// <value>Source of dock markers.</value>
		public ViewHost ViewHost
		{
			get { return _dockHost; }
		}

		/// <summary>Creates the markers.</summary>
		/// <param name="dockClient">The dock client.</param>
		/// <returns>Created markers.</returns>
		protected override ViewHostDockMarker[] CreateMarkers(ViewHost dockClient)
		{
			if(_dockHost.IsDocumentWell || (!dockClient.IsDocumentWell && !(dockClient.ViewsCount == 1 && dockClient.GetView(0).IsDocument)))
			{
				return new ViewHostDockMarker[] { new ViewHostDockMarker(_dockHost, dockClient) };
			}
			else
			{
				return null;
			}
		}
	}
}
