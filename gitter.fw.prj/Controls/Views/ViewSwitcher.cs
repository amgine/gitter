namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;
	using System.Text;

	class ViewSwitcher : Form
	{
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// ViewSwitcher
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(733, 386);
			this.ControlBox = false;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ViewSwitcher";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.ResumeLayout(false);

		}
	}
}
