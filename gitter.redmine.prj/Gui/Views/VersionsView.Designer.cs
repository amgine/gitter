namespace gitter.Redmine.Gui
{
	partial class VersionsView
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._lstVersions = new gitter.Redmine.Gui.ListBoxes.VersionsListBox();
			this.SuspendLayout();
			// 
			// _lstIssues
			// 
			this._lstVersions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lstVersions.Location = new System.Drawing.Point(0, 0);
			this._lstVersions.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._lstVersions.Name = "_lstVersions";
			this._lstVersions.Size = new System.Drawing.Size(615, 407);
			this._lstVersions.TabIndex = 0;
			this._lstVersions.Text = "No versions to display";
			// 
			// IssuesView
			// 
			this.Controls.Add(this._lstVersions);
			this.Name = "NewsView";
			this.Size = new System.Drawing.Size(615, 407);
			this.ResumeLayout(false);
		}

		#endregion

		private ListBoxes.VersionsListBox _lstVersions;
	}
}
