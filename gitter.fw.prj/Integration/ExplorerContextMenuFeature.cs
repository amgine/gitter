namespace gitter.Framework
{
	using System;
	using System.Drawing;
	using gitter.Framework.Configuration;

	using Resources = gitter.Framework.Properties.Resources;

	sealed class ExplorerContextMenuFeature : IIntegrationFeature
	{
		public event EventHandler IsEnabledChanged;

		private void OnIsEnabledChanged()
		{
			var handler = IsEnabledChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		public string Name
		{
			get { return "ExplorerContextMenu"; }
		}

		public string DisplayText
		{
			get { return Resources.StrsExplorerContextMenuFeature; }
		}

		public Bitmap Icon
		{
			get { return CachedResources.Bitmaps["ImgFolder"]; }
		}

		public bool IsEnabled
		{
			get { return GlobalOptions.IsIntegratedInExplorerContextMenu; }
			set
			{
				if(value)
				{
					GlobalOptions.IntegrateInExplorerContextMenu();
				}
				else
				{
					GlobalOptions.RemoveFromExplorerContextMenu();
				}
			}
		}

		public bool AdministratorRightsRequired
		{
			get { return true; }
		}

		public Action GetEnableAction(bool enable)
		{
			return enable ?
				(Action)GlobalOptions.IntegrateInExplorerContextMenu :
				(Action)GlobalOptions.RemoveFromExplorerContextMenu;
		}

		bool IIntegrationFeature.HasConfiguration
		{
			get { return false; }
		}

		void IIntegrationFeature.SaveTo(Section section)
		{
		}

		void IIntegrationFeature.LoadFrom(Section section)
		{
		}
	}
}
