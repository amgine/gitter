namespace gitter.Redmine.Gui
{
	partial class NewsView
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
			this._lstNews = new gitter.Redmine.Gui.ListBoxes.NewsListBox();
			this.SuspendLayout();
			// 
			// _lstIssues
			// 
			this._lstNews.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lstNews.Location = new System.Drawing.Point(0, 0);
			this._lstNews.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._lstNews.Name = "_lstNews";
			this._lstNews.Size = new System.Drawing.Size(615, 407);
			this._lstNews.TabIndex = 0;
			this._lstNews.Text = "No news to display";
			// 
			// IssuesView
			// 
			this.Controls.Add(this._lstNews);
			this.Name = "NewsView";
			this.Size = new System.Drawing.Size(615, 407);
			this.ResumeLayout(false);
		}

		#endregion

		private ListBoxes.NewsListBox _lstNews;
	}
}
