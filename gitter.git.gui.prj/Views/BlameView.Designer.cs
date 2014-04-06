namespace gitter.Git.Gui.Views
{
	partial class BlameView
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
			if(disposing)
			{
				BlameFileBinding = null;
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._blamePanel = new gitter.Git.Gui.Controls.BlameViewer();
			this.SuspendLayout();
			// 
			// _blamePanel
			// 
			this._blamePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._blamePanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._blamePanel.Location = new System.Drawing.Point(0, 0);
			this._blamePanel.Name = "_blamePanel";
			this._blamePanel.Size = new System.Drawing.Size(555, 362);
			this._blamePanel.TabIndex = 0;
			// 
			// BlameView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._blamePanel);
			this.Name = "BlameView";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private gitter.Git.Gui.Controls.BlameViewer _blamePanel;
	}
}
