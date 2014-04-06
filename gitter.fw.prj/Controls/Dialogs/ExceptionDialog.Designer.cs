namespace gitter.Framework
{
	partial class ExceptionDialog
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
			this._lblExceptionName = new System.Windows.Forms.Label();
			this._lblMessage = new System.Windows.Forms.Label();
			this._txtStack = new System.Windows.Forms.TextBox();
			this._lblSTack = new System.Windows.Forms.Label();
			this._lnkCopyToClipboard = new System.Windows.Forms.LinkLabel();
			this._lnkSendBugReport = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// _lblExceptionName
			// 
			this._lblExceptionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lblExceptionName.Location = new System.Drawing.Point(0, 0);
			this._lblExceptionName.Name = "_lblExceptionName";
			this._lblExceptionName.Size = new System.Drawing.Size(490, 32);
			this._lblExceptionName.TabIndex = 0;
			this._lblExceptionName.Text = "%EXCEPTION_NAME%";
			this._lblExceptionName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _lblMessage
			// 
			this._lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lblMessage.Location = new System.Drawing.Point(0, 32);
			this._lblMessage.Name = "_lblMessage";
			this._lblMessage.Size = new System.Drawing.Size(490, 65);
			this._lblMessage.TabIndex = 1;
			this._lblMessage.Text = "%Message%";
			// 
			// _txtStack
			// 
			this._txtStack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._txtStack.BackColor = System.Drawing.SystemColors.Window;
			this._txtStack.Location = new System.Drawing.Point(0, 115);
			this._txtStack.Multiline = true;
			this._txtStack.Name = "_txtStack";
			this._txtStack.ReadOnly = true;
			this._txtStack.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this._txtStack.Size = new System.Drawing.Size(490, 268);
			this._txtStack.TabIndex = 2;
			this._txtStack.WordWrap = false;
			// 
			// _lblSTack
			// 
			this._lblSTack.AutoSize = true;
			this._lblSTack.Location = new System.Drawing.Point(0, 97);
			this._lblSTack.Name = "_lblSTack";
			this._lblSTack.Size = new System.Drawing.Size(58, 15);
			this._lblSTack.TabIndex = 3;
			this._lblSTack.Text = "%Stack%:";
			// 
			// _lnkCopyToClipboard
			// 
			this._lnkCopyToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._lnkCopyToClipboard.AutoSize = true;
			this._lnkCopyToClipboard.Location = new System.Drawing.Point(249, 386);
			this._lnkCopyToClipboard.Name = "_lnkCopyToClipboard";
			this._lnkCopyToClipboard.Size = new System.Drawing.Size(122, 15);
			this._lnkCopyToClipboard.TabIndex = 4;
			this._lnkCopyToClipboard.TabStop = true;
			this._lnkCopyToClipboard.Text = "%Copy to clipboard%";
			this._lnkCopyToClipboard.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnCopyToClipboardLinkClicked);
			// 
			// _lnkSendBugReport
			// 
			this._lnkSendBugReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._lnkSendBugReport.AutoSize = true;
			this._lnkSendBugReport.Location = new System.Drawing.Point(378, 386);
			this._lnkSendBugReport.Name = "_lnkSendBugReport";
			this._lnkSendBugReport.Size = new System.Drawing.Size(109, 15);
			this._lnkSendBugReport.TabIndex = 5;
			this._lnkSendBugReport.TabStop = true;
			this._lnkSendBugReport.Text = "%Send bugreport%";
			this._lnkSendBugReport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnSendBugReportLinkClicked);
			// 
			// ExceptionDialog
			// 
			this.Controls.Add(this._lnkSendBugReport);
			this.Controls.Add(this._lnkCopyToClipboard);
			this.Controls.Add(this._lblSTack);
			this.Controls.Add(this._txtStack);
			this.Controls.Add(this._lblMessage);
			this.Controls.Add(this._lblExceptionName);
			this.Name = "ExceptionDialog";
			this.Size = new System.Drawing.Size(490, 401);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _lblExceptionName;
		private System.Windows.Forms.Label _lblMessage;
		private System.Windows.Forms.TextBox _txtStack;
		private System.Windows.Forms.Label _lblSTack;
		private System.Windows.Forms.LinkLabel _lnkCopyToClipboard;
		private System.Windows.Forms.LinkLabel _lnkSendBugReport;
	}
}
