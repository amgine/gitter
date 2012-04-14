namespace gitter.Framework.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Options;

	/// <summary>Base class or column header extenders.</summary>
	[ToolboxItem(false)]
	public partial class BaseExtender : UserControl
	{
		/// <summary>Create <see cref="BaseExtender"/>.</summary>
		public BaseExtender()
		{
			AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			BackColor = System.Drawing.SystemColors.Window;
			BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			if(LicenseManager.UsageMode == LicenseUsageMode.Designtime)
				Font = SystemFonts.MessageBoxFont;
			else
				Font = GitterApplication.FontManager.UIFont;
		}
	}
}
