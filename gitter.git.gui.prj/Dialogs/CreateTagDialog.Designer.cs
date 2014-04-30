namespace gitter.Git.Gui.Dialogs
{
	partial class CreateTagDialog
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
			_repository = null;
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panel2 = new System.Windows.Forms.Panel();
			this._txtKeyId = new System.Windows.Forms.TextBox();
			this._grpSigning = new gitter.Framework.Controls.GroupSeparator();
			this._radUseKeyId = new System.Windows.Forms.RadioButton();
			this._radUseDefaultEmailKey = new System.Windows.Forms.RadioButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this._grpMessage = new gitter.Framework.Controls.GroupSeparator();
			this._txtMessage = new System.Windows.Forms.TextBox();
			this._pnlOptions = new System.Windows.Forms.Panel();
			this._radSigned = new System.Windows.Forms.RadioButton();
			this._grpOptions = new gitter.Framework.Controls.GroupSeparator();
			this._radSimple = new System.Windows.Forms.RadioButton();
			this._radAnnotated = new System.Windows.Forms.RadioButton();
			this._lblRevision = new System.Windows.Forms.Label();
			this._txtName = new System.Windows.Forms.TextBox();
			this._lblName = new System.Windows.Forms.Label();
			this._txtRevision = new gitter.Git.Gui.Controls.RevisionPicker();
			this.panel2.SuspendLayout();
			this.panel1.SuspendLayout();
			this._pnlOptions.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this._txtKeyId);
			this.panel2.Controls.Add(this._grpSigning);
			this.panel2.Controls.Add(this._radUseKeyId);
			this.panel2.Controls.Add(this._radUseDefaultEmailKey);
			this.panel2.Location = new System.Drawing.Point(0, 289);
			this.panel2.Margin = new System.Windows.Forms.Padding(0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(397, 75);
			this.panel2.TabIndex = 15;
			// 
			// _txtKeyId
			// 
			this._txtKeyId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtKeyId.Enabled = false;
			this._txtKeyId.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._txtKeyId.Location = new System.Drawing.Point(142, 44);
			this._txtKeyId.Name = "_txtKeyId";
			this._txtKeyId.Size = new System.Drawing.Size(255, 23);
			this._txtKeyId.TabIndex = 8;
			// 
			// _grpSigning
			// 
			this._grpSigning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._grpSigning.Location = new System.Drawing.Point(0, 0);
			this._grpSigning.Name = "_grpSigning";
			this._grpSigning.Size = new System.Drawing.Size(397, 19);
			this._grpSigning.TabIndex = 0;
			this._grpSigning.Text = "%Signing%";
			// 
			// _radUseKeyId
			// 
			this._radUseKeyId.AutoSize = true;
			this._radUseKeyId.Enabled = false;
			this._radUseKeyId.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radUseKeyId.Location = new System.Drawing.Point(15, 45);
			this._radUseKeyId.Name = "_radUseKeyId";
			this._radUseKeyId.Size = new System.Drawing.Size(109, 20);
			this._radUseKeyId.TabIndex = 7;
			this._radUseKeyId.Text = "%Use key-id%:";
			this._radUseKeyId.UseVisualStyleBackColor = true;
			this._radUseKeyId.CheckedChanged += new System.EventHandler(this._radUseKeyId_CheckedChanged);
			// 
			// _radUseDefaultEmailKey
			// 
			this._radUseDefaultEmailKey.AutoSize = true;
			this._radUseDefaultEmailKey.Checked = true;
			this._radUseDefaultEmailKey.Enabled = false;
			this._radUseDefaultEmailKey.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radUseDefaultEmailKey.Location = new System.Drawing.Point(15, 25);
			this._radUseDefaultEmailKey.Name = "_radUseDefaultEmailKey";
			this._radUseDefaultEmailKey.Size = new System.Drawing.Size(213, 20);
			this._radUseDefaultEmailKey.TabIndex = 6;
			this._radUseDefaultEmailKey.TabStop = true;
			this._radUseDefaultEmailKey.Text = "%Use default EMail adresse\'s key%";
			this._radUseDefaultEmailKey.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._grpMessage);
			this.panel1.Controls.Add(this._txtMessage);
			this.panel1.Location = new System.Drawing.Point(0, 109);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(397, 180);
			this.panel1.TabIndex = 14;
			// 
			// _grpMessage
			// 
			this._grpMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._grpMessage.Location = new System.Drawing.Point(0, 0);
			this._grpMessage.Name = "_grpMessage";
			this._grpMessage.Size = new System.Drawing.Size(397, 19);
			this._grpMessage.TabIndex = 0;
			this._grpMessage.Text = "%Message%";
			// 
			// _txtMessage
			// 
			this._txtMessage.AcceptsReturn = true;
			this._txtMessage.AcceptsTab = true;
			this._txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtMessage.Enabled = false;
			this._txtMessage.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._txtMessage.Location = new System.Drawing.Point(15, 25);
			this._txtMessage.Multiline = true;
			this._txtMessage.Name = "_txtMessage";
			this._txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this._txtMessage.Size = new System.Drawing.Size(382, 152);
			this._txtMessage.TabIndex = 5;
			this._txtMessage.WordWrap = false;
			// 
			// _pnlOptions
			// 
			this._pnlOptions.Controls.Add(this._radSigned);
			this._pnlOptions.Controls.Add(this._grpOptions);
			this._pnlOptions.Controls.Add(this._radSimple);
			this._pnlOptions.Controls.Add(this._radAnnotated);
			this._pnlOptions.Location = new System.Drawing.Point(0, 61);
			this._pnlOptions.Margin = new System.Windows.Forms.Padding(0);
			this._pnlOptions.Name = "_pnlOptions";
			this._pnlOptions.Size = new System.Drawing.Size(397, 48);
			this._pnlOptions.TabIndex = 11;
			// 
			// _radSigned
			// 
			this._radSigned.AutoSize = true;
			this._radSigned.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radSigned.Location = new System.Drawing.Point(236, 25);
			this._radSigned.Name = "_radSigned";
			this._radSigned.Size = new System.Drawing.Size(87, 20);
			this._radSigned.TabIndex = 4;
			this._radSigned.Text = "%Signed%";
			this._radSigned.UseVisualStyleBackColor = true;
			this._radSigned.CheckedChanged += new System.EventHandler(this._radSigned_CheckedChanged);
			// 
			// _grpOptions
			// 
			this._grpOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._grpOptions.Location = new System.Drawing.Point(0, 0);
			this._grpOptions.Name = "_grpOptions";
			this._grpOptions.Size = new System.Drawing.Size(397, 19);
			this._grpOptions.TabIndex = 0;
			this._grpOptions.Text = "%Type%";
			// 
			// _radSimple
			// 
			this._radSimple.AutoSize = true;
			this._radSimple.Checked = true;
			this._radSimple.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radSimple.Location = new System.Drawing.Point(15, 25);
			this._radSimple.Name = "_radSimple";
			this._radSimple.Size = new System.Drawing.Size(114, 20);
			this._radSimple.TabIndex = 2;
			this._radSimple.TabStop = true;
			this._radSimple.Text = "%Lightweight%";
			this._radSimple.UseVisualStyleBackColor = true;
			this._radSimple.CheckedChanged += new System.EventHandler(this._radSimple_CheckedChanged);
			// 
			// _radAnnotated
			// 
			this._radAnnotated.AutoSize = true;
			this._radAnnotated.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radAnnotated.Location = new System.Drawing.Point(129, 25);
			this._radAnnotated.Name = "_radAnnotated";
			this._radAnnotated.Size = new System.Drawing.Size(107, 20);
			this._radAnnotated.TabIndex = 3;
			this._radAnnotated.Text = "%Annotated%";
			this._radAnnotated.UseVisualStyleBackColor = true;
			this._radAnnotated.CheckedChanged += new System.EventHandler(this._radAnnotated_CheckedChanged);
			// 
			// _lblRevision
			// 
			this._lblRevision.AutoSize = true;
			this._lblRevision.Location = new System.Drawing.Point(0, 35);
			this._lblRevision.Name = "_lblRevision";
			this._lblRevision.Size = new System.Drawing.Size(74, 15);
			this._lblRevision.TabIndex = 8;
			this._lblRevision.Text = "%Revision%:";
			// 
			// _txtName
			// 
			this._txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtName.Location = new System.Drawing.Point(94, 3);
			this._txtName.Name = "_txtName";
			this._txtName.Size = new System.Drawing.Size(303, 23);
			this._txtName.TabIndex = 0;
			// 
			// _lblName
			// 
			this._lblName.AutoSize = true;
			this._lblName.Location = new System.Drawing.Point(0, 6);
			this._lblName.Name = "_lblName";
			this._lblName.Size = new System.Drawing.Size(62, 15);
			this._lblName.TabIndex = 5;
			this._lblName.Text = "%Name%:";
			// 
			// _txtRevision
			// 
			this._txtRevision.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtRevision.FormattingEnabled = true;
			this._txtRevision.Location = new System.Drawing.Point(94, 32);
			this._txtRevision.Name = "_txtRevision";
			this._txtRevision.Size = new System.Drawing.Size(303, 23);
			this._txtRevision.TabIndex = 1;
			// 
			// CreateTagDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this._txtRevision);
			this.Controls.Add(this._pnlOptions);
			this.Controls.Add(this._lblRevision);
			this.Controls.Add(this._txtName);
			this.Controls.Add(this._lblName);
			this.Name = "CreateTagDialog";
			this.Size = new System.Drawing.Size(400, 364);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this._pnlOptions.ResumeLayout(false);
			this._pnlOptions.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _txtName;
		private System.Windows.Forms.RadioButton _radAnnotated;
		private System.Windows.Forms.RadioButton _radSimple;
		private System.Windows.Forms.Label _lblRevision;
		private System.Windows.Forms.Label _lblName;
		private System.Windows.Forms.Panel _pnlOptions;
		private gitter.Framework.Controls.GroupSeparator _grpOptions;
		private gitter.Git.Gui.Controls.RevisionPicker _txtRevision;
		private System.Windows.Forms.RadioButton _radSigned;
		private System.Windows.Forms.TextBox _txtKeyId;
		private System.Windows.Forms.RadioButton _radUseKeyId;
		private System.Windows.Forms.RadioButton _radUseDefaultEmailKey;
		private System.Windows.Forms.TextBox _txtMessage;
		private System.Windows.Forms.Panel panel1;
		private Framework.Controls.GroupSeparator _grpMessage;
		private System.Windows.Forms.Panel panel2;
		private Framework.Controls.GroupSeparator _grpSigning;

	}
}
