namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	partial class MaintenanceView : GitViewBase
	{
		public MaintenanceView(IDictionary<string, object> parameters, GuiProvider gui)
			: base(Guids.MaintenanceViewGuid, gui, parameters)
		{
			InitializeComponent();

			Text = Resources.StrMaintenance;
		}
	}
}
