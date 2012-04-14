namespace gitter.Git.Gui.Dialogs
{
	partial class AddNoteDialog
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
					_speller.Dispose();
				if(components != null)
					components.Dispose();
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
			this._txtMessage = new System.Windows.Forms.TextBox();
			this._lblMessage = new System.Windows.Forms.Label();
			this._txtRevision = new gitter.Git.Gui.Controls.RevisionPicker();
			this._lblRevision = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _txtMessage
			// 
			this._txtMessage.AcceptsReturn = true;
			this._txtMessage.AcceptsTab = true;
			this._txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._txtMessage.Location = new System.Drawing.Point(3, 49);
			this._txtMessage.Multiline = true;
			this._txtMessage.Name = "_txtMessage";
			this._txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this._txtMessage.Size = new System.Drawing.Size(394, 189);
			this._txtMessage.TabIndex = 1;
			// 
			// _lblMessage
			// 
			this._lblMessage.AutoSize = true;
			this._lblMessage.Location = new System.Drawing.Point(0, 31);
			this._lblMessage.Name = "_lblMessage";
			this._lblMessage.Size = new System.Drawing.Size(76, 15);
			this._lblMessage.TabIndex = 2;
			this._lblMessage.Text = "%Message%:";
			// 
			// _txtRevision
			// 
			this._txtRevision.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._txtRevision.FormattingEnabled = true;
			this._txtRevision.Location = new System.Drawing.Point(94, 3);
			this._txtRevision.Name = "_txtRevision";
			this._txtRevision.Size = new System.Drawing.Size(303, 23);
			this._txtRevision.TabIndex = 0;
			// 
			// _lblRevision
			// 
			this._lblRevision.AutoSize = true;
			this._lblRevision.Location = new System.Drawing.Point(0, 6);
			this._lblRevision.Name = "_lblRevision";
			this._lblRevision.Size = new System.Drawing.Size(74, 15);
			this._lblRevision.TabIndex = 3;
			this._lblRevision.Text = "%Revision%:";
			// 
			// AddNoteDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lblRevision);
			this.Controls.Add(this._txtRevision);
			this.Controls.Add(this._txtMessage);
			this.Controls.Add(this._lblMessage);
			this.Name = "AddNoteDialog";
			this.Size = new System.Drawing.Size(400, 241);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _txtMessage;
		private System.Windows.Forms.Label _lblMessage;
		private Controls.RevisionPicker _txtRevision;
		private System.Windows.Forms.Label _lblRevision;
	}
}
