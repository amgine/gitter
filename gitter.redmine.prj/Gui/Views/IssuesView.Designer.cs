namespace gitter.Redmine.Gui
{
	partial class IssuesView
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
			this._lstIssues = new gitter.Redmine.Gui.ListBoxes.IssuesListBox();
			this.SuspendLayout();
			// 
			// _lstIssues
			// 
			this._lstIssues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lstIssues.Location = new System.Drawing.Point(0, 0);
			this._lstIssues.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._lstIssues.Name = "_lstIssues";
			this._lstIssues.Size = new System.Drawing.Size(615, 407);
			this._lstIssues.TabIndex = 0;
			this._lstIssues.Text = "No issues to display";
			// 
			// IssuesView
			// 
			this.Controls.Add(this._lstIssues);
			this.Name = "IssuesView";
			this.Size = new System.Drawing.Size(615, 407);
			this.ResumeLayout(false);
		}

		#endregion

		private ListBoxes.IssuesListBox _lstIssues;
	}
}
