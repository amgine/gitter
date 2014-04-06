namespace gitter.Git.Gui.Views
{
	partial class SubmodulesView
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._lstSubmodules = new gitter.Git.Gui.Controls.SubmodulesListBox();
			this.SuspendLayout();
			// 
			// _lstSubmodules
			// 
			this._lstSubmodules.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._lstSubmodules.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lstSubmodules.Location = new System.Drawing.Point(0, 0);
			this._lstSubmodules.Margin = new System.Windows.Forms.Padding(0);
			this._lstSubmodules.Name = "_lstSubmodules";
			this._lstSubmodules.Size = new System.Drawing.Size(555, 160);
			this._lstSubmodules.TabIndex = 1;
			this._lstSubmodules.Text = "No Submodules";
			// 
			// SubmodulesView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lstSubmodules);
			this.Name = "SubmodulesView";
			this.Size = new System.Drawing.Size(555, 160);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private gitter.Git.Gui.Controls.SubmodulesListBox _lstSubmodules;
	}
}
