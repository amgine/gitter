namespace gitter.Git.Gui.Views
{
	partial class RemoteView
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
				DataSource = null;
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
			this._lstRemoteReferences = new gitter.Git.Gui.Controls.RemoteReferencesListBox();
			this.SuspendLayout();
			// 
			// _lstRemoteReferences
			// 
			this._lstRemoteReferences.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lstRemoteReferences.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._lstRemoteReferences.ForeColor = System.Drawing.SystemColors.WindowText;
			this._lstRemoteReferences.Location = new System.Drawing.Point(0, 0);
			this._lstRemoteReferences.Name = "_lstRemoteReferences";
			this._lstRemoteReferences.ShowTreeLines = true;
			this._lstRemoteReferences.Size = new System.Drawing.Size(555, 362);
			this._lstRemoteReferences.TabIndex = 0;
			// 
			// RemoteView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lstRemoteReferences);
			this.Name = "RemoteView";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Controls.RemoteReferencesListBox _lstRemoteReferences;
	}
}
