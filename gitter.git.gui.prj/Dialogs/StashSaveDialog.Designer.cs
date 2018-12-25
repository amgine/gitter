namespace gitter.Git.Gui.Dialogs
{
	partial class StashSaveDialog
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
				if(_speller != null)
				{
					_speller.Dispose();
					_speller = null;
				}
			}
			Repository = null;
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._lblMessage = new System.Windows.Forms.Label();
			this._txtMessage = new System.Windows.Forms.TextBox();
			this._chkKeepIndex = new System.Windows.Forms.CheckBox();
			this._chkIncludeUntrackedFiles = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _lblMessage
			// 
			this._lblMessage.AutoSize = true;
			this._lblMessage.Location = new System.Drawing.Point(0, 0);
			this._lblMessage.Name = "_lblMessage";
			this._lblMessage.Size = new System.Drawing.Size(76, 15);
			this._lblMessage.TabIndex = 0;
			this._lblMessage.Text = "%Message%:";
			// 
			// _txtMessage
			// 
			this._txtMessage.AcceptsReturn = true;
			this._txtMessage.AcceptsTab = true;
			this._txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtMessage.Location = new System.Drawing.Point(3, 18);
			this._txtMessage.Multiline = true;
			this._txtMessage.Name = "_txtMessage";
			this._txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this._txtMessage.Size = new System.Drawing.Size(394, 113);
			this._txtMessage.TabIndex = 1;
			// 
			// _chkKeepIndex
			// 
			this._chkKeepIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._chkKeepIndex.AutoSize = true;
			this._chkKeepIndex.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkKeepIndex.Location = new System.Drawing.Point(3, 136);
			this._chkKeepIndex.Name = "_chkKeepIndex";
			this._chkKeepIndex.Size = new System.Drawing.Size(109, 20);
			this._chkKeepIndex.TabIndex = 2;
			this._chkKeepIndex.Text = "%Keep index%";
			this._chkKeepIndex.UseVisualStyleBackColor = true;
			// 
			// _chkIncludeUntrackedFiles
			// 
			this._chkIncludeUntrackedFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._chkIncludeUntrackedFiles.AutoSize = true;
			this._chkIncludeUntrackedFiles.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkIncludeUntrackedFiles.Location = new System.Drawing.Point(153, 136);
			this._chkIncludeUntrackedFiles.Name = "_chkIncludeUntrackedFiles";
			this._chkIncludeUntrackedFiles.Size = new System.Drawing.Size(171, 20);
			this._chkIncludeUntrackedFiles.TabIndex = 3;
			this._chkIncludeUntrackedFiles.Text = "%Include untracked files%";
			this._chkIncludeUntrackedFiles.UseVisualStyleBackColor = true;
			// 
			// StashSaveDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._chkIncludeUntrackedFiles);
			this.Controls.Add(this._chkKeepIndex);
			this.Controls.Add(this._txtMessage);
			this.Controls.Add(this._lblMessage);
			this.Name = "StashSaveDialog";
			this.Size = new System.Drawing.Size(400, 156);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _lblMessage;
		private System.Windows.Forms.TextBox _txtMessage;
		private System.Windows.Forms.CheckBox _chkKeepIndex;
		private System.Windows.Forms.CheckBox _chkIncludeUntrackedFiles;
	}
}
