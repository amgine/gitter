namespace gitter.Controls
{
	partial class AddIssueTrackerDialog
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
				if(_setupControlCache != null)
				{
					foreach(var ctl in _setupControlCache.Values)
					{
						if(ctl != null)
						{
							ctl.Dispose();
						}
					}
					_setupControlCache.Clear();
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
			this._issueTracker = new gitter.Controls.IssueTrackerPicker();
			this._lblProvider = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _issueTracker
			// 
			this._issueTracker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._issueTracker.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._issueTracker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._issueTracker.FormattingEnabled = true;
			this._issueTracker.Location = new System.Drawing.Point(100, 3);
			this._issueTracker.Name = "_issueTracker";
			this._issueTracker.SelectedIssueTracker = null;
			this._issueTracker.Size = new System.Drawing.Size(273, 24);
			this._issueTracker.TabIndex = 0;
			// 
			// _lblProvider
			// 
			this._lblProvider.AutoSize = true;
			this._lblProvider.Location = new System.Drawing.Point(3, 6);
			this._lblProvider.Name = "_lblProvider";
			this._lblProvider.Size = new System.Drawing.Size(54, 15);
			this._lblProvider.TabIndex = 1;
			this._lblProvider.Text = "Provider:";
			// 
			// AddIssueTrackerDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lblProvider);
			this.Controls.Add(this._issueTracker);
			this.Name = "AddIssueTrackerDialog";
			this.Size = new System.Drawing.Size(376, 30);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private IssueTrackerPicker _issueTracker;
		private System.Windows.Forms.Label _lblProvider;
	}
}
