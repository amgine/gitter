namespace gitter.Git.Gui.Dialogs
{
	partial class StashToBranchDialog
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
			this._lblStash = new System.Windows.Forms.Label();
			this._txtStashName = new System.Windows.Forms.TextBox();
			this._lblBranchName = new System.Windows.Forms.Label();
			this._txtBranchName = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// _lblStash
			// 
			this._lblStash.AutoSize = true;
			this._lblStash.Location = new System.Drawing.Point(0, 6);
			this._lblStash.Name = "_lblStash";
			this._lblStash.Size = new System.Drawing.Size(53, 13);
			this._lblStash.TabIndex = 4;
			this._lblStash.Text = "%Stash%:";
			// 
			// _txtStashName
			// 
			this._txtStashName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._txtStashName.Location = new System.Drawing.Point(94, 3);
			this._txtStashName.Name = "_txtStashName";
			this._txtStashName.ReadOnly = true;
			this._txtStashName.Size = new System.Drawing.Size(288, 20);
			this._txtStashName.TabIndex = 1;
			// 
			// _lblBranchName
			// 
			this._lblBranchName.AutoSize = true;
			this._lblBranchName.Location = new System.Drawing.Point(0, 32);
			this._lblBranchName.Name = "_lblBranchName";
			this._lblBranchName.Size = new System.Drawing.Size(60, 13);
			this._lblBranchName.TabIndex = 6;
			this._lblBranchName.Text = "%Branch%:";
			// 
			// _txtBranchName
			// 
			this._txtBranchName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._txtBranchName.Location = new System.Drawing.Point(94, 29);
			this._txtBranchName.Name = "_txtBranchName";
			this._txtBranchName.Size = new System.Drawing.Size(288, 20);
			this._txtBranchName.TabIndex = 0;
			// 
			// StashToBranchDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lblBranchName);
			this.Controls.Add(this._txtBranchName);
			this.Controls.Add(this._lblStash);
			this.Controls.Add(this._txtStashName);
			this.Name = "StashToBranchDialog";
			this.Size = new System.Drawing.Size(385, 53);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _lblStash;
		private System.Windows.Forms.TextBox _txtStashName;
		private System.Windows.Forms.Label _lblBranchName;
		private System.Windows.Forms.TextBox _txtBranchName;
	}
}
