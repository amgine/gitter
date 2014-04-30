namespace gitter.Git.Gui.Dialogs
{
	partial class PushDialog
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
			this._pnlOptions = new System.Windows.Forms.Panel();
			this._pnlWarning = new System.Windows.Forms.Panel();
			this._lblUseWithCaution = new System.Windows.Forms.Label();
			this._picWarning = new System.Windows.Forms.PictureBox();
			this._grpOptions = new gitter.Framework.Controls.GroupSeparator();
			this._chkUseThinPack = new System.Windows.Forms.CheckBox();
			this._chkForceOverwriteBranches = new System.Windows.Forms.CheckBox();
			this._chkSendTags = new System.Windows.Forms.CheckBox();
			this._lblBranches = new System.Windows.Forms.Label();
			this._remotePicker = new gitter.Git.Gui.Controls.RemotePicker();
			this._lstReferences = new gitter.Git.Gui.Controls.ReferencesListBox();
			this._grpPushTo = new gitter.Framework.Controls.GroupSeparator();
			this._pnlPushTo = new System.Windows.Forms.Panel();
			this._txtUrl = new System.Windows.Forms.TextBox();
			this._radUrl = new System.Windows.Forms.RadioButton();
			this._radRemote = new System.Windows.Forms.RadioButton();
			this._pnlOptions.SuspendLayout();
			this._pnlWarning.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._picWarning)).BeginInit();
			this._pnlPushTo.SuspendLayout();
			this.SuspendLayout();
			// 
			// _pnlOptions
			// 
			this._pnlOptions.Controls.Add(this._pnlWarning);
			this._pnlOptions.Controls.Add(this._grpOptions);
			this._pnlOptions.Controls.Add(this._chkUseThinPack);
			this._pnlOptions.Controls.Add(this._chkForceOverwriteBranches);
			this._pnlOptions.Controls.Add(this._chkSendTags);
			this._pnlOptions.Location = new System.Drawing.Point(0, 290);
			this._pnlOptions.Name = "_pnlOptions";
			this._pnlOptions.Size = new System.Drawing.Size(382, 89);
			this._pnlOptions.TabIndex = 20;
			// 
			// _pnlWarning
			// 
			this._pnlWarning.BackColor = System.Drawing.SystemColors.Info;
			this._pnlWarning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._pnlWarning.Controls.Add(this._lblUseWithCaution);
			this._pnlWarning.Controls.Add(this._picWarning);
			this._pnlWarning.ForeColor = System.Drawing.SystemColors.InfoText;
			this._pnlWarning.Location = new System.Drawing.Point(229, 24);
			this._pnlWarning.Name = "_pnlWarning";
			this._pnlWarning.Size = new System.Drawing.Size(153, 20);
			this._pnlWarning.TabIndex = 6;
			this._pnlWarning.Visible = false;
			// 
			// _lblUseWithCaution
			// 
			this._lblUseWithCaution.AutoSize = true;
			this._lblUseWithCaution.Location = new System.Drawing.Point(17, 3);
			this._lblUseWithCaution.Name = "_lblUseWithCaution";
			this._lblUseWithCaution.Size = new System.Drawing.Size(115, 15);
			this._lblUseWithCaution.TabIndex = 1;
			this._lblUseWithCaution.Text = "%Use with caution%";
			// 
			// _picWarning
			// 
			this._picWarning.Location = new System.Drawing.Point(1, 1);
			this._picWarning.Name = "_picWarning";
			this._picWarning.Size = new System.Drawing.Size(16, 16);
			this._picWarning.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this._picWarning.TabIndex = 0;
			this._picWarning.TabStop = false;
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
			// _chkUseThinPack
			// 
			this._chkUseThinPack.AutoSize = true;
			this._chkUseThinPack.Checked = true;
			this._chkUseThinPack.CheckState = System.Windows.Forms.CheckState.Checked;
			this._chkUseThinPack.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkUseThinPack.Location = new System.Drawing.Point(13, 45);
			this._chkUseThinPack.Name = "_chkUseThinPack";
			this._chkUseThinPack.Size = new System.Drawing.Size(123, 20);
			this._chkUseThinPack.TabIndex = 6;
			this._chkUseThinPack.Text = "%Use thin pack%";
			this._chkUseThinPack.UseVisualStyleBackColor = true;
			// 
			// _chkForceOverwriteBranches
			// 
			this._chkForceOverwriteBranches.AutoSize = true;
			this._chkForceOverwriteBranches.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkForceOverwriteBranches.Location = new System.Drawing.Point(13, 25);
			this._chkForceOverwriteBranches.Name = "_chkForceOverwriteBranches";
			this._chkForceOverwriteBranches.Size = new System.Drawing.Size(225, 20);
			this._chkForceOverwriteBranches.TabIndex = 5;
			this._chkForceOverwriteBranches.Text = "%Force overwrite remote branches%";
			this._chkForceOverwriteBranches.UseVisualStyleBackColor = true;
			this._chkForceOverwriteBranches.CheckedChanged += new System.EventHandler(this.OnForceOverwriteCheckedChanged);
			// 
			// _chkSendTags
			// 
			this._chkSendTags.AutoSize = true;
			this._chkSendTags.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkSendTags.Location = new System.Drawing.Point(13, 65);
			this._chkSendTags.Name = "_chkSendTags";
			this._chkSendTags.Size = new System.Drawing.Size(103, 20);
			this._chkSendTags.TabIndex = 7;
			this._chkSendTags.Text = "%Send tags%";
			this._chkSendTags.UseVisualStyleBackColor = true;
			// 
			// _lblBranches
			// 
			this._lblBranches.AutoSize = true;
			this._lblBranches.Location = new System.Drawing.Point(0, 0);
			this._lblBranches.Name = "_lblBranches";
			this._lblBranches.Size = new System.Drawing.Size(121, 15);
			this._lblBranches.TabIndex = 2;
			this._lblBranches.Text = "%Branches to push%:";
			// 
			// _remotePicker
			// 
			this._remotePicker.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._remotePicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._remotePicker.FormattingEnabled = true;
			this._remotePicker.Location = new System.Drawing.Point(104, 24);
			this._remotePicker.Name = "_remotePicker";
			this._remotePicker.Size = new System.Drawing.Size(278, 23);
			this._remotePicker.TabIndex = 2;
			this._remotePicker.SelectedIndexChanged += new System.EventHandler(this.OnRemotePickerSelectedIndexChanged);
			// 
			// _lstReferences
			// 
			this._lstReferences.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lstReferences.DisableContextMenus = true;
			this._lstReferences.ForeColor = System.Drawing.SystemColors.WindowText;
			this._lstReferences.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._lstReferences.Location = new System.Drawing.Point(3, 18);
			this._lstReferences.Name = "_lstReferences";
			this._lstReferences.ShowTreeLines = true;
			this._lstReferences.Size = new System.Drawing.Size(379, 183);
			this._lstReferences.TabIndex = 0;
			// 
			// _grpPushTo
			// 
			this._grpPushTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._grpPushTo.Location = new System.Drawing.Point(0, 0);
			this._grpPushTo.Name = "_grpPushTo";
			this._grpPushTo.Size = new System.Drawing.Size(382, 19);
			this._grpPushTo.TabIndex = 0;
			this._grpPushTo.Text = "%Push to%";
			// 
			// _pnlPushTo
			// 
			this._pnlPushTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._pnlPushTo.Controls.Add(this._txtUrl);
			this._pnlPushTo.Controls.Add(this._radUrl);
			this._pnlPushTo.Controls.Add(this._remotePicker);
			this._pnlPushTo.Controls.Add(this._radRemote);
			this._pnlPushTo.Controls.Add(this._grpPushTo);
			this._pnlPushTo.Location = new System.Drawing.Point(0, 207);
			this._pnlPushTo.Name = "_pnlPushTo";
			this._pnlPushTo.Size = new System.Drawing.Size(382, 80);
			this._pnlPushTo.TabIndex = 10;
			// 
			// _txtUrl
			// 
			this._txtUrl.Location = new System.Drawing.Point(104, 53);
			this._txtUrl.Name = "_txtUrl";
			this._txtUrl.Size = new System.Drawing.Size(278, 23);
			this._txtUrl.TabIndex = 4;
			this._txtUrl.TextChanged += new System.EventHandler(this.OnUrlTextChanged);
			// 
			// _radUrl
			// 
			this._radUrl.AutoSize = true;
			this._radUrl.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radUrl.Location = new System.Drawing.Point(12, 54);
			this._radUrl.Name = "_radUrl";
			this._radUrl.Size = new System.Drawing.Size(72, 20);
			this._radUrl.TabIndex = 3;
			this._radUrl.Text = "%URL%";
			this._radUrl.UseVisualStyleBackColor = true;
			// 
			// _radRemote
			// 
			this._radRemote.AutoSize = true;
			this._radRemote.Checked = true;
			this._radRemote.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radRemote.Location = new System.Drawing.Point(12, 26);
			this._radRemote.Name = "_radRemote";
			this._radRemote.Size = new System.Drawing.Size(92, 20);
			this._radRemote.TabIndex = 1;
			this._radRemote.TabStop = true;
			this._radRemote.Text = "%Remote%";
			this._radRemote.UseVisualStyleBackColor = true;
			// 
			// PushDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._pnlPushTo);
			this.Controls.Add(this._pnlOptions);
			this.Controls.Add(this._lblBranches);
			this.Controls.Add(this._lstReferences);
			this.Name = "PushDialog";
			this.Size = new System.Drawing.Size(385, 379);
			this._pnlOptions.ResumeLayout(false);
			this._pnlOptions.PerformLayout();
			this._pnlWarning.ResumeLayout(false);
			this._pnlWarning.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._picWarning)).EndInit();
			this._pnlPushTo.ResumeLayout(false);
			this._pnlPushTo.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private gitter.Git.Gui.Controls.ReferencesListBox _lstReferences;
		private System.Windows.Forms.Label _lblBranches;
		private System.Windows.Forms.CheckBox _chkSendTags;
		private System.Windows.Forms.CheckBox _chkUseThinPack;
		private System.Windows.Forms.CheckBox _chkForceOverwriteBranches;
		private System.Windows.Forms.Panel _pnlWarning;
		private System.Windows.Forms.Label _lblUseWithCaution;
		private System.Windows.Forms.PictureBox _picWarning;
		private System.Windows.Forms.Panel _pnlOptions;
		private gitter.Framework.Controls.GroupSeparator _grpOptions;
		private gitter.Git.Gui.Controls.RemotePicker _remotePicker;
		private Framework.Controls.GroupSeparator _grpPushTo;
		private System.Windows.Forms.Panel _pnlPushTo;
		private System.Windows.Forms.TextBox _txtUrl;
		private System.Windows.Forms.RadioButton _radUrl;
		private System.Windows.Forms.RadioButton _radRemote;
	}
}
