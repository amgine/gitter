namespace gitter.Framework.Options
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	[ToolboxItem(false)]
	public partial class AppearancePage : PropertyPage, IExecutableDialog
	{
		public AppearancePage()
			: base(PropertyPageFactory.AppearanceGroupGuid)
		{
			InitializeComponent();

			if(GitterApplication.TextRenderer == GitterApplication.GdiPlusTextRenderer)
			{
				_radGdiPlus.Checked = true;
			}
			else if(GitterApplication.TextRenderer == GitterApplication.GdiTextRenderer)
			{
				_radGdi.Checked = true;
			}
		}

		public bool Execute()
		{
			if(_radGdi.Checked)
			{
				GitterApplication.TextRenderer = GitterApplication.GdiTextRenderer;
			}
			else if(_radGdiPlus.Checked)
			{
				GitterApplication.TextRenderer = GitterApplication.GdiPlusTextRenderer;
			}
			return true;
		}
	}
}
