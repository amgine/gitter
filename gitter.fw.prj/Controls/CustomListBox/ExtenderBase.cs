namespace gitter.Framework.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Options;

	/// <summary>Base class or column header extenders.</summary>
	[ToolboxItem(false)]
	public partial class ExtenderBase : UserControl
	{
		/// <summary>Create <see cref="ExtenderBase"/>.</summary>
		public ExtenderBase()
		{
			AutoScaleDimensions	= new System.Drawing.SizeF(96F, 96F);
			AutoScaleMode		= System.Windows.Forms.AutoScaleMode.Dpi;
			BorderStyle	= System.Windows.Forms.BorderStyle.FixedSingle;
			if(LicenseManager.UsageMode == LicenseUsageMode.Designtime)
			{
				Font		= SystemFonts.MessageBoxFont;
				BackColor	= System.Drawing.SystemColors.Window;
			}
			else
			{
				Font		= GitterApplication.FontManager.UIFont;
				BackColor	= GitterApplication.Style.Colors.Window;
				ForeColor	= GitterApplication.Style.Colors.WindowText;
			}
		}
	}
}
