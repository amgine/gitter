namespace gitter.Git.Gui.Views
{
	partial class ConfigView
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
			this._lstConfig = new gitter.Git.Gui.Controls.ConfigListBox();
			this.SuspendLayout();
			// 
			// _lstConfig
			// 
			this._lstConfig.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._lstConfig.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lstConfig.Location = new System.Drawing.Point(0, 0);
			this._lstConfig.Name = "_lstConfig";
			this._lstConfig.Size = new System.Drawing.Size(555, 362);
			this._lstConfig.TabIndex = 1;
			this._lstConfig.Text = "No parameters found";
			// 
			// ConfigView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lstConfig);
			this.Name = "ConfigView";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private gitter.Git.Gui.Controls.ConfigListBox _lstConfig;
	}
}
