namespace gitter.Git.Gui.Dialogs
{
	partial class CommitDialog
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
				if(_speller != null)
				{
					_speller.Dispose();
				}
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
			this._lstStaged = new gitter.Git.Gui.Controls.TreeListBox();
			this._txtMessage = new System.Windows.Forms.TextBox();
			this._lblStagedFiles = new System.Windows.Forms.Label();
			this._lblMessage = new System.Windows.Forms.Label();
			this._chkAmend = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _lstStaged
			// 
			this._lstStaged.DisableContextMenus = true;
			this._lstStaged.ForeColor = System.Drawing.SystemColors.WindowText;
			this._lstStaged.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._lstStaged.Location = new System.Drawing.Point(3, 18);
			this._lstStaged.Name = "_lstStaged";
			this._lstStaged.ShowTreeLines = true;
			this._lstStaged.Size = new System.Drawing.Size(222, 338);
			this._lstStaged.TabIndex = 2;
			// 
			// _txtMessage
			// 
			this._txtMessage.AcceptsReturn = true;
			this._txtMessage.AcceptsTab = true;
			this._txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtMessage.Location = new System.Drawing.Point(231, 18);
			this._txtMessage.Multiline = true;
			this._txtMessage.Name = "_txtMessage";
			this._txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this._txtMessage.Size = new System.Drawing.Size(408, 316);
			this._txtMessage.TabIndex = 0;
			// 
			// _lblStagedFiles
			// 
			this._lblStagedFiles.AutoSize = true;
			this._lblStagedFiles.Location = new System.Drawing.Point(0, 0);
			this._lblStagedFiles.Name = "_lblStagedFiles";
			this._lblStagedFiles.Size = new System.Drawing.Size(113, 15);
			this._lblStagedFiles.TabIndex = 2;
			this._lblStagedFiles.Text = "%Staged changes%:";
			// 
			// _lblMessage
			// 
			this._lblMessage.AutoSize = true;
			this._lblMessage.Location = new System.Drawing.Point(228, 0);
			this._lblMessage.Name = "_lblMessage";
			this._lblMessage.Size = new System.Drawing.Size(76, 15);
			this._lblMessage.TabIndex = 3;
			this._lblMessage.Text = "%Message%:";
			// 
			// _chkAmend
			// 
			this._chkAmend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._chkAmend.AutoSize = true;
			this._chkAmend.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkAmend.Location = new System.Drawing.Point(231, 336);
			this._chkAmend.Name = "_chkAmend";
			this._chkAmend.Size = new System.Drawing.Size(91, 20);
			this._chkAmend.TabIndex = 1;
			this._chkAmend.Text = "%Amend%";
			this._chkAmend.UseVisualStyleBackColor = true;
			this._chkAmend.CheckedChanged += new System.EventHandler(this.OnAmendCheckedChanged);
			// 
			// CommitDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lblMessage);
			this.Controls.Add(this._lblStagedFiles);
			this.Controls.Add(this._txtMessage);
			this.Controls.Add(this._lstStaged);
			this.Controls.Add(this._chkAmend);
			this.Name = "CommitDialog";
			this.Size = new System.Drawing.Size(642, 359);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private gitter.Git.Gui.Controls.TreeListBox _lstStaged;
		private System.Windows.Forms.TextBox _txtMessage;
		private System.Windows.Forms.Label _lblStagedFiles;
		private System.Windows.Forms.Label _lblMessage;
		private System.Windows.Forms.CheckBox _chkAmend;
	}
}
