namespace gitter.Git.Gui.Dialogs
{
	partial class MergeDialog
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
			this._pnlOptions = new System.Windows.Forms.Panel();
			this._chkNoFF = new System.Windows.Forms.CheckBox();
			this._chkNoCommit = new System.Windows.Forms.CheckBox();
			this._grpOptions = new gitter.Framework.Controls.GroupSeparator();
			this._chkSquash = new System.Windows.Forms.CheckBox();
			this._lnkAutoFormat = new System.Windows.Forms.LinkLabel();
			this._lblMessage = new System.Windows.Forms.Label();
			this._txtMessage = new System.Windows.Forms.TextBox();
			this._lblMergeWith = new System.Windows.Forms.Label();
			this._references = new gitter.Git.Gui.Controls.ReferencesListBox();
			this._pnlOptions.SuspendLayout();
			this.SuspendLayout();
			// 
			// _pnlOptions
			// 
			this._pnlOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._pnlOptions.Controls.Add(this._chkNoFF);
			this._pnlOptions.Controls.Add(this._chkNoCommit);
			this._pnlOptions.Controls.Add(this._grpOptions);
			this._pnlOptions.Controls.Add(this._chkSquash);
			this._pnlOptions.Location = new System.Drawing.Point(228, 270);
			this._pnlOptions.Name = "_pnlOptions";
			this._pnlOptions.Size = new System.Drawing.Size(411, 86);
			this._pnlOptions.TabIndex = 14;
			// 
			// _chkNoFF
			// 
			this._chkNoFF.AutoSize = true;
			this._chkNoFF.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkNoFF.Location = new System.Drawing.Point(13, 25);
			this._chkNoFF.Name = "_chkNoFF";
			this._chkNoFF.Size = new System.Drawing.Size(136, 20);
			this._chkNoFF.TabIndex = 4;
			this._chkNoFF.Text = "%No fast-forward%";
			this._chkNoFF.UseVisualStyleBackColor = true;
			// 
			// _chkNoCommit
			// 
			this._chkNoCommit.AutoSize = true;
			this._chkNoCommit.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkNoCommit.Location = new System.Drawing.Point(13, 45);
			this._chkNoCommit.Name = "_chkNoCommit";
			this._chkNoCommit.Size = new System.Drawing.Size(113, 20);
			this._chkNoCommit.TabIndex = 5;
			this._chkNoCommit.Text = "%No commit%";
			this._chkNoCommit.UseVisualStyleBackColor = true;
			// 
			// _grpOptions
			// 
			this._grpOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._grpOptions.Location = new System.Drawing.Point(0, 0);
			this._grpOptions.Name = "_grpOptions";
			this._grpOptions.Size = new System.Drawing.Size(411, 19);
			this._grpOptions.TabIndex = 0;
			this._grpOptions.Text = "%Options%";
			// 
			// _chkSquash
			// 
			this._chkSquash.AutoSize = true;
			this._chkSquash.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkSquash.Location = new System.Drawing.Point(13, 65);
			this._chkSquash.Name = "_chkSquash";
			this._chkSquash.Size = new System.Drawing.Size(90, 20);
			this._chkSquash.TabIndex = 6;
			this._chkSquash.Text = "%Squash%";
			this._chkSquash.UseVisualStyleBackColor = true;
			// 
			// _lnkAutoFormat
			// 
			this._lnkAutoFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._lnkAutoFormat.Location = new System.Drawing.Point(484, 0);
			this._lnkAutoFormat.Name = "_lnkAutoFormat";
			this._lnkAutoFormat.Size = new System.Drawing.Size(155, 15);
			this._lnkAutoFormat.TabIndex = 3;
			this._lnkAutoFormat.TabStop = true;
			this._lnkAutoFormat.Text = "%Auto%";
			this._lnkAutoFormat.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._lnkAutoFormat.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnAutoFormatLinkClicked);
			// 
			// _lblMessage
			// 
			this._lblMessage.AutoSize = true;
			this._lblMessage.Location = new System.Drawing.Point(228, 0);
			this._lblMessage.Name = "_lblMessage";
			this._lblMessage.Size = new System.Drawing.Size(76, 15);
			this._lblMessage.TabIndex = 11;
			this._lblMessage.Text = "%Message%:";
			// 
			// _txtMessage
			// 
			this._txtMessage.AcceptsReturn = true;
			this._txtMessage.AcceptsTab = true;
			this._txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtMessage.Location = new System.Drawing.Point(231, 18);
			this._txtMessage.Multiline = true;
			this._txtMessage.Name = "_txtMessage";
			this._txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this._txtMessage.Size = new System.Drawing.Size(408, 246);
			this._txtMessage.TabIndex = 2;
			this._txtMessage.WordWrap = false;
			// 
			// _lblMergeWith
			// 
			this._lblMergeWith.AutoSize = true;
			this._lblMergeWith.Location = new System.Drawing.Point(0, 0);
			this._lblMergeWith.Name = "_lblMergeWith";
			this._lblMergeWith.Size = new System.Drawing.Size(90, 15);
			this._lblMergeWith.TabIndex = 7;
			this._lblMergeWith.Text = "%Merge with%:";
			// 
			// _references
			// 
			this._references.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this._references.ForeColor = System.Drawing.SystemColors.WindowText;
			this._references.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._references.ItemActivation = gitter.Framework.Controls.ItemActivation.SingleClick;
			this._references.Location = new System.Drawing.Point(3, 18);
			this._references.Name = "_references";
			this._references.ShowTreeLines = true;
			this._references.Size = new System.Drawing.Size(222, 338);
			this._references.TabIndex = 1;
			// 
			// MergeDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._pnlOptions);
			this.Controls.Add(this._lblMessage);
			this.Controls.Add(this._txtMessage);
			this.Controls.Add(this._lblMergeWith);
			this.Controls.Add(this._references);
			this.Controls.Add(this._lnkAutoFormat);
			this.Name = "MergeDialog";
			this.Size = new System.Drawing.Size(642, 359);
			this._pnlOptions.ResumeLayout(false);
			this._pnlOptions.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private gitter.Git.Gui.Controls.ReferencesListBox _references;
		private System.Windows.Forms.Label _lblMergeWith;
		private System.Windows.Forms.CheckBox _chkNoFF;
		private System.Windows.Forms.CheckBox _chkNoCommit;
		private System.Windows.Forms.CheckBox _chkSquash;
		private System.Windows.Forms.Label _lblMessage;
		private System.Windows.Forms.TextBox _txtMessage;
		private System.Windows.Forms.LinkLabel _lnkAutoFormat;
		private System.Windows.Forms.Panel _pnlOptions;
		private gitter.Framework.Controls.GroupSeparator _grpOptions;
	}
}
