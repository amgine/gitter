namespace gitter.Git.Gui.Views
{
	partial class ReflogView
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
				if(components != null)
				{
					components.Dispose();
				}
				if(_reference != null)
				{
					var branch = _reference as Branch;
					if(branch != null)
					{
						branch.Renamed -= OnBranchRenamed;
					}
				}
				_lstReflog.Load(null);
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
			this._lstReflog = new gitter.Git.Gui.Controls.ReflogListBox();
			this.SuspendLayout();
			// 
			// _lstReflog
			// 
			this._lstReflog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lstReflog.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._lstReflog.Location = new System.Drawing.Point(0, 0);
			this._lstReflog.Name = "_lstReflog";
			this._lstReflog.Size = new System.Drawing.Size(555, 362);
			this._lstReflog.TabIndex = 0;
			this._lstReflog.Text = "Reflog is empty";
			// 
			// ReflogView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lstReflog);
			this.Name = "ReflogView";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Controls.ReflogListBox _lstReflog;
	}
}
