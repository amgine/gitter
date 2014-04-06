namespace gitter.Git.Gui.Dialogs
{
	partial class CherryPickDialog
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
			this._lstCommits = new gitter.Framework.Controls.FlowLayoutControl();
			this._grpOptions = new gitter.Framework.Controls.GroupSeparator();
			this._pnlOptions = new System.Windows.Forms.Panel();
			this._chkNoCommit = new System.Windows.Forms.CheckBox();
			this._grpMainlineParentCommit = new gitter.Framework.Controls.GroupSeparator();
			this._pnlMainlineParentCommit = new System.Windows.Forms.Panel();
			this._txtRevision = new System.Windows.Forms.TextBox();
			this._lblRevision = new System.Windows.Forms.Label();
			this._pnlOptions.SuspendLayout();
			this._pnlMainlineParentCommit.SuspendLayout();
			this.SuspendLayout();
			// 
			// _lstCommits
			// 
			this._lstCommits.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lstCommits.ForeColor = System.Drawing.SystemColors.WindowText;
			this._lstCommits.Location = new System.Drawing.Point(13, 25);
			this._lstCommits.Name = "_lstCommits";
			this._lstCommits.Size = new System.Drawing.Size(464, 218);
			this._lstCommits.TabIndex = 1;
			// 
			// _grpOptions
			// 
			this._grpOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._grpOptions.Location = new System.Drawing.Point(0, 0);
			this._grpOptions.Name = "_grpOptions";
			this._grpOptions.Size = new System.Drawing.Size(477, 19);
			this._grpOptions.TabIndex = 1;
			this._grpOptions.Text = "%Options%";
			// 
			// _pnlOptions
			// 
			this._pnlOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._pnlOptions.Controls.Add(this._chkNoCommit);
			this._pnlOptions.Controls.Add(this._grpOptions);
			this._pnlOptions.Location = new System.Drawing.Point(0, 278);
			this._pnlOptions.Name = "_pnlOptions";
			this._pnlOptions.Size = new System.Drawing.Size(480, 49);
			this._pnlOptions.TabIndex = 2;
			// 
			// _chkNoCommit
			// 
			this._chkNoCommit.AutoSize = true;
			this._chkNoCommit.Location = new System.Drawing.Point(13, 25);
			this._chkNoCommit.Name = "_chkNoCommit";
			this._chkNoCommit.Size = new System.Drawing.Size(107, 19);
			this._chkNoCommit.TabIndex = 2;
			this._chkNoCommit.Text = "%No commit%";
			this._chkNoCommit.UseVisualStyleBackColor = true;
			// 
			// _grpMainlineParentCommit
			// 
			this._grpMainlineParentCommit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._grpMainlineParentCommit.Location = new System.Drawing.Point(0, 0);
			this._grpMainlineParentCommit.Name = "_grpMainlineParentCommit";
			this._grpMainlineParentCommit.Size = new System.Drawing.Size(477, 19);
			this._grpMainlineParentCommit.TabIndex = 1;
			this._grpMainlineParentCommit.Text = "%Mainline Parent Commit%";
			// 
			// _pnlMainlineParentCommit
			// 
			this._pnlMainlineParentCommit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._pnlMainlineParentCommit.Controls.Add(this._grpMainlineParentCommit);
			this._pnlMainlineParentCommit.Controls.Add(this._lstCommits);
			this._pnlMainlineParentCommit.Location = new System.Drawing.Point(0, 29);
			this._pnlMainlineParentCommit.Name = "_pnlMainlineParentCommit";
			this._pnlMainlineParentCommit.Size = new System.Drawing.Size(480, 243);
			this._pnlMainlineParentCommit.TabIndex = 3;
			// 
			// _txtRevision
			// 
			this._txtRevision.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtRevision.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._txtRevision.Location = new System.Drawing.Point(94, 3);
			this._txtRevision.Name = "_txtRevision";
			this._txtRevision.ReadOnly = true;
			this._txtRevision.Size = new System.Drawing.Size(383, 20);
			this._txtRevision.TabIndex = 0;
			// 
			// _lblRevision
			// 
			this._lblRevision.AutoSize = true;
			this._lblRevision.Location = new System.Drawing.Point(0, 6);
			this._lblRevision.Name = "_lblRevision";
			this._lblRevision.Size = new System.Drawing.Size(74, 15);
			this._lblRevision.TabIndex = 4;
			this._lblRevision.Text = "%Revision%:";
			// 
			// CherryPickDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._txtRevision);
			this.Controls.Add(this._lblRevision);
			this.Controls.Add(this._pnlMainlineParentCommit);
			this.Controls.Add(this._pnlOptions);
			this.Name = "CherryPickDialog";
			this.Size = new System.Drawing.Size(480, 327);
			this._pnlOptions.ResumeLayout(false);
			this._pnlOptions.PerformLayout();
			this._pnlMainlineParentCommit.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Framework.Controls.FlowLayoutControl _lstCommits;
		private Framework.Controls.GroupSeparator _grpOptions;
		private System.Windows.Forms.Panel _pnlOptions;
		private System.Windows.Forms.CheckBox _chkNoCommit;
		private Framework.Controls.GroupSeparator _grpMainlineParentCommit;
		private System.Windows.Forms.Panel _pnlMainlineParentCommit;
		private System.Windows.Forms.TextBox _txtRevision;
		private System.Windows.Forms.Label _lblRevision;
	}
}
