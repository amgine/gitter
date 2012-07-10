namespace gitter.Git.Gui.Dialogs
{
	partial class RevertDialog
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
			this._lblRevision = new System.Windows.Forms.Label();
			this._txtRevision = new System.Windows.Forms.TextBox();
			this._chkNoCommit = new System.Windows.Forms.CheckBox();
			this._pnlOptions = new System.Windows.Forms.Panel();
			this._grpOptions = new gitter.Framework.Controls.GroupSeparator();
			this._pnlOptions.SuspendLayout();
			this.SuspendLayout();
			// 
			// _lblRevision
			// 
			this._lblRevision.AutoSize = true;
			this._lblRevision.Location = new System.Drawing.Point(0, 6);
			this._lblRevision.Name = "_lblRevision";
			this._lblRevision.Size = new System.Drawing.Size(74, 15);
			this._lblRevision.TabIndex = 0;
			this._lblRevision.Text = "%Revision%:";
			// 
			// _txtRevision
			// 
			this._txtRevision.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._txtRevision.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._txtRevision.Location = new System.Drawing.Point(94, 3);
			this._txtRevision.Name = "_txtRevision";
			this._txtRevision.ReadOnly = true;
			this._txtRevision.Size = new System.Drawing.Size(288, 20);
			this._txtRevision.TabIndex = 1;
			// 
			// _chkNoCommit
			// 
			this._chkNoCommit.AutoSize = true;
			this._chkNoCommit.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkNoCommit.Location = new System.Drawing.Point(12, 25);
			this._chkNoCommit.Name = "_chkNoCommit";
			this._chkNoCommit.Size = new System.Drawing.Size(113, 20);
			this._chkNoCommit.TabIndex = 10;
			this._chkNoCommit.Text = "%No commit%";
			this._chkNoCommit.UseVisualStyleBackColor = true;
			// 
			// _pnlOptions
			// 
			this._pnlOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._pnlOptions.Controls.Add(this._chkNoCommit);
			this._pnlOptions.Controls.Add(this._grpOptions);
			this._pnlOptions.Location = new System.Drawing.Point(0, 29);
			this._pnlOptions.Name = "_pnlOptions";
			this._pnlOptions.Size = new System.Drawing.Size(382, 47);
			this._pnlOptions.TabIndex = 3;
			// 
			// _grpOptions
			// 
			this._grpOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._grpOptions.Location = new System.Drawing.Point(0, 0);
			this._grpOptions.Name = "_grpOptions";
			this._grpOptions.Size = new System.Drawing.Size(382, 19);
			this._grpOptions.TabIndex = 0;
			this._grpOptions.Text = "%Options%";
			// 
			// RevertDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._pnlOptions);
			this.Controls.Add(this._txtRevision);
			this.Controls.Add(this._lblRevision);
			this.Name = "RevertDialog";
			this.Size = new System.Drawing.Size(385, 76);
			this._pnlOptions.ResumeLayout(false);
			this._pnlOptions.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _lblRevision;
		private System.Windows.Forms.TextBox _txtRevision;
		private System.Windows.Forms.CheckBox _chkNoCommit;
		private System.Windows.Forms.Panel _pnlOptions;
		private gitter.Framework.Controls.GroupSeparator _grpOptions;
	}
}
