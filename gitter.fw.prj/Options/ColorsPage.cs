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
	public partial class ColorsPage : PropertyPage
	{
		public static readonly new Guid Guid = new Guid("AD2A7C07-6E10-4F0D-B471-F6DA58638660");

		public ColorsPage()
			: base(Guid)
		{
			InitializeComponent();
		}
	}
}
