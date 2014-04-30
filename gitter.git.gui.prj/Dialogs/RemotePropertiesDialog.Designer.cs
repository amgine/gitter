namespace gitter.Git.Gui.Dialogs
{
	partial class RemotePropertiesDialog
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._radFetchNone = new System.Windows.Forms.RadioButton();
			this._radFetchAll = new System.Windows.Forms.RadioButton();
			this._radNormal = new System.Windows.Forms.RadioButton();
			this._lblFetchTags = new System.Windows.Forms.Label();
			this._lblUploadPack = new System.Windows.Forms.Label();
			this._lblReceivePack = new System.Windows.Forms.Label();
			this._lblVCS = new System.Windows.Forms.Label();
			this._chkSkipFetchAll = new System.Windows.Forms.CheckBox();
			this._chkMirror = new System.Windows.Forms.CheckBox();
			this._grpOptions = new gitter.Framework.Controls.GroupSeparator();
			this._grpUpdatedReferences = new gitter.Framework.Controls.GroupSeparator();
			this._txtUploadPack = new System.Windows.Forms.TextBox();
			this._txtReceivePack = new System.Windows.Forms.TextBox();
			this._txtVCS = new System.Windows.Forms.TextBox();
			this._txtProxy = new System.Windows.Forms.TextBox();
			this._txtPushURL = new System.Windows.Forms.TextBox();
			this._txtFetchURL = new System.Windows.Forms.TextBox();
			this._lblProxy = new System.Windows.Forms.Label();
			this._lblPushURL = new System.Windows.Forms.Label();
			this._lblFetchURL = new System.Windows.Forms.Label();
			this._lstUpdatedReferences = new gitter.Framework.Controls.CustomListBox();
			this._btnAddRefspec = new System.Windows.Forms.Button();
			this._lblRefspec = new System.Windows.Forms.Label();
			this._txtRefspec = new System.Windows.Forms.TextBox();
			this._radFetch = new System.Windows.Forms.RadioButton();
			this._radPush = new System.Windows.Forms.RadioButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _radFetchNone
			// 
			this._radFetchNone.AutoSize = true;
			this._radFetchNone.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radFetchNone.Location = new System.Drawing.Point(272, 202);
			this._radFetchNone.Name = "_radFetchNone";
			this._radFetchNone.Size = new System.Drawing.Size(80, 20);
			this._radFetchNone.TabIndex = 10;
			this._radFetchNone.Text = "%None%";
			this._radFetchNone.UseVisualStyleBackColor = true;
			// 
			// _radFetchAll
			// 
			this._radFetchAll.AutoSize = true;
			this._radFetchAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radFetchAll.Location = new System.Drawing.Point(201, 202);
			this._radFetchAll.Name = "_radFetchAll";
			this._radFetchAll.Size = new System.Drawing.Size(65, 20);
			this._radFetchAll.TabIndex = 9;
			this._radFetchAll.Text = "%All%";
			this._radFetchAll.UseVisualStyleBackColor = true;
			// 
			// _radNormal
			// 
			this._radNormal.AutoSize = true;
			this._radNormal.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radNormal.Location = new System.Drawing.Point(106, 202);
			this._radNormal.Name = "_radNormal";
			this._radNormal.Size = new System.Drawing.Size(89, 20);
			this._radNormal.TabIndex = 8;
			this._radNormal.Text = "%Default%";
			this._radNormal.UseVisualStyleBackColor = true;
			// 
			// _lblFetchTags
			// 
			this._lblFetchTags.AutoSize = true;
			this._lblFetchTags.Location = new System.Drawing.Point(0, 204);
			this._lblFetchTags.Name = "_lblFetchTags";
			this._lblFetchTags.Size = new System.Drawing.Size(87, 15);
			this._lblFetchTags.TabIndex = 10;
			this._lblFetchTags.Text = "%Fetch Tags%:";
			// 
			// _lblUploadPack
			// 
			this._lblUploadPack.AutoSize = true;
			this._lblUploadPack.Location = new System.Drawing.Point(0, 176);
			this._lblUploadPack.Name = "_lblUploadPack";
			this._lblUploadPack.Size = new System.Drawing.Size(96, 15);
			this._lblUploadPack.TabIndex = 9;
			this._lblUploadPack.Text = "%Upload Pack%:";
			// 
			// _lblReceivePack
			// 
			this._lblReceivePack.AutoSize = true;
			this._lblReceivePack.Location = new System.Drawing.Point(0, 147);
			this._lblReceivePack.Name = "_lblReceivePack";
			this._lblReceivePack.Size = new System.Drawing.Size(98, 15);
			this._lblReceivePack.TabIndex = 9;
			this._lblReceivePack.Text = "%Receive Pack%:";
			// 
			// _lblVCS
			// 
			this._lblVCS.AutoSize = true;
			this._lblVCS.Location = new System.Drawing.Point(0, 93);
			this._lblVCS.Name = "_lblVCS";
			this._lblVCS.Size = new System.Drawing.Size(51, 15);
			this._lblVCS.TabIndex = 8;
			this._lblVCS.Text = "%VCS%:";
			// 
			// _chkSkipFetchAll
			// 
			this._chkSkipFetchAll.AutoSize = true;
			this._chkSkipFetchAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkSkipFetchAll.Location = new System.Drawing.Point(3, 251);
			this._chkSkipFetchAll.Name = "_chkSkipFetchAll";
			this._chkSkipFetchAll.Size = new System.Drawing.Size(119, 20);
			this._chkSkipFetchAll.TabIndex = 12;
			this._chkSkipFetchAll.Text = "%Skip fetch all%";
			this._chkSkipFetchAll.UseVisualStyleBackColor = true;
			// 
			// _chkMirror
			// 
			this._chkMirror.AutoSize = true;
			this._chkMirror.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkMirror.Location = new System.Drawing.Point(3, 226);
			this._chkMirror.Name = "_chkMirror";
			this._chkMirror.Size = new System.Drawing.Size(85, 20);
			this._chkMirror.TabIndex = 11;
			this._chkMirror.Text = "%Mirror%";
			this._chkMirror.UseVisualStyleBackColor = true;
			// 
			// _grpOptions
			// 
			this._grpOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._grpOptions.Location = new System.Drawing.Point(0, 119);
			this._grpOptions.Name = "_grpOptions";
			this._grpOptions.Size = new System.Drawing.Size(397, 19);
			this._grpOptions.TabIndex = 5;
			this._grpOptions.Text = "%Default Bahaviour%";
			// 
			// _grpUpdatedReferences
			// 
			this._grpUpdatedReferences.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._grpUpdatedReferences.Location = new System.Drawing.Point(0, 0);
			this._grpUpdatedReferences.Name = "_grpUpdatedReferences";
			this._grpUpdatedReferences.Size = new System.Drawing.Size(397, 19);
			this._grpUpdatedReferences.TabIndex = 4;
			this._grpUpdatedReferences.Text = "%Updated References%";
			// 
			// _txtUploadPack
			// 
			this._txtUploadPack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtUploadPack.Location = new System.Drawing.Point(106, 173);
			this._txtUploadPack.Name = "_txtUploadPack";
			this._txtUploadPack.Size = new System.Drawing.Size(291, 23);
			this._txtUploadPack.TabIndex = 7;
			// 
			// _txtReceivePack
			// 
			this._txtReceivePack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtReceivePack.Location = new System.Drawing.Point(106, 144);
			this._txtReceivePack.Name = "_txtReceivePack";
			this._txtReceivePack.Size = new System.Drawing.Size(291, 23);
			this._txtReceivePack.TabIndex = 6;
			// 
			// _txtVCS
			// 
			this._txtVCS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtVCS.Location = new System.Drawing.Point(106, 90);
			this._txtVCS.Name = "_txtVCS";
			this._txtVCS.Size = new System.Drawing.Size(291, 23);
			this._txtVCS.TabIndex = 5;
			// 
			// _txtProxy
			// 
			this._txtProxy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtProxy.Location = new System.Drawing.Point(106, 61);
			this._txtProxy.Name = "_txtProxy";
			this._txtProxy.Size = new System.Drawing.Size(291, 23);
			this._txtProxy.TabIndex = 4;
			// 
			// _txtPushURL
			// 
			this._txtPushURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtPushURL.Location = new System.Drawing.Point(106, 32);
			this._txtPushURL.Name = "_txtPushURL";
			this._txtPushURL.Size = new System.Drawing.Size(291, 23);
			this._txtPushURL.TabIndex = 3;
			// 
			// _txtFetchURL
			// 
			this._txtFetchURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtFetchURL.Location = new System.Drawing.Point(106, 3);
			this._txtFetchURL.Name = "_txtFetchURL";
			this._txtFetchURL.Size = new System.Drawing.Size(291, 23);
			this._txtFetchURL.TabIndex = 2;
			// 
			// _lblProxy
			// 
			this._lblProxy.AutoSize = true;
			this._lblProxy.Location = new System.Drawing.Point(0, 64);
			this._lblProxy.Name = "_lblProxy";
			this._lblProxy.Size = new System.Drawing.Size(59, 15);
			this._lblProxy.TabIndex = 1;
			this._lblProxy.Text = "%Proxy%:";
			// 
			// _lblPushURL
			// 
			this._lblPushURL.AutoSize = true;
			this._lblPushURL.Location = new System.Drawing.Point(0, 35);
			this._lblPushURL.Name = "_lblPushURL";
			this._lblPushURL.Size = new System.Drawing.Size(80, 15);
			this._lblPushURL.TabIndex = 1;
			this._lblPushURL.Text = "%Push URL%:";
			// 
			// _lblFetchURL
			// 
			this._lblFetchURL.AutoSize = true;
			this._lblFetchURL.Location = new System.Drawing.Point(0, 6);
			this._lblFetchURL.Name = "_lblFetchURL";
			this._lblFetchURL.Size = new System.Drawing.Size(83, 15);
			this._lblFetchURL.TabIndex = 0;
			this._lblFetchURL.Text = "%Fetch URL%:";
			// 
			// _lstUpdatedReferences
			// 
			this._lstUpdatedReferences.AllowColumnReorder = false;
			this._lstUpdatedReferences.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lstUpdatedReferences.ForeColor = System.Drawing.SystemColors.WindowText;
			this._lstUpdatedReferences.Location = new System.Drawing.Point(3, 25);
			this._lstUpdatedReferences.Multiselect = true;
			this._lstUpdatedReferences.Name = "_lstUpdatedReferences";
			this._lstUpdatedReferences.Size = new System.Drawing.Size(394, 100);
			this._lstUpdatedReferences.TabIndex = 13;
			// 
			// _btnAddRefspec
			// 
			this._btnAddRefspec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._btnAddRefspec.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnAddRefspec.Location = new System.Drawing.Point(322, 132);
			this._btnAddRefspec.Name = "_btnAddRefspec";
			this._btnAddRefspec.Size = new System.Drawing.Size(75, 23);
			this._btnAddRefspec.TabIndex = 15;
			this._btnAddRefspec.Text = "%Add%";
			this._btnAddRefspec.UseVisualStyleBackColor = true;
			this._btnAddRefspec.Click += new System.EventHandler(this._btnAddRefspec_Click);
			// 
			// _lblRefspec
			// 
			this._lblRefspec.AutoSize = true;
			this._lblRefspec.Location = new System.Drawing.Point(0, 135);
			this._lblRefspec.Name = "_lblRefspec";
			this._lblRefspec.Size = new System.Drawing.Size(71, 15);
			this._lblRefspec.TabIndex = 14;
			this._lblRefspec.Text = "%Refspec%:";
			// 
			// _txtRefspec
			// 
			this._txtRefspec.Location = new System.Drawing.Point(106, 132);
			this._txtRefspec.Name = "_txtRefspec";
			this._txtRefspec.Size = new System.Drawing.Size(210, 23);
			this._txtRefspec.TabIndex = 14;
			// 
			// _radFetch
			// 
			this._radFetch.AutoSize = true;
			this._radFetch.Checked = true;
			this._radFetch.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radFetch.Location = new System.Drawing.Point(106, 156);
			this._radFetch.Name = "_radFetch";
			this._radFetch.Size = new System.Drawing.Size(80, 20);
			this._radFetch.TabIndex = 16;
			this._radFetch.TabStop = true;
			this._radFetch.Text = "%Fetch%";
			this._radFetch.UseVisualStyleBackColor = true;
			// 
			// _radPush
			// 
			this._radPush.AutoSize = true;
			this._radPush.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radPush.Location = new System.Drawing.Point(186, 156);
			this._radPush.Name = "_radPush";
			this._radPush.Size = new System.Drawing.Size(77, 20);
			this._radPush.TabIndex = 17;
			this._radPush.TabStop = true;
			this._radPush.Text = "%Push%";
			this._radPush.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this._grpUpdatedReferences);
			this.panel1.Controls.Add(this._lstUpdatedReferences);
			this.panel1.Controls.Add(this._btnAddRefspec);
			this.panel1.Controls.Add(this._txtRefspec);
			this.panel1.Controls.Add(this._lblRefspec);
			this.panel1.Controls.Add(this._radPush);
			this.panel1.Controls.Add(this._radFetch);
			this.panel1.Location = new System.Drawing.Point(0, 274);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(397, 177);
			this.panel1.TabIndex = 19;
			// 
			// RemotePropertiesDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this._radFetchNone);
			this.Controls.Add(this._radFetchAll);
			this.Controls.Add(this._radNormal);
			this.Controls.Add(this._lblFetchTags);
			this.Controls.Add(this._lblUploadPack);
			this.Controls.Add(this._lblReceivePack);
			this.Controls.Add(this._lblVCS);
			this.Controls.Add(this._chkSkipFetchAll);
			this.Controls.Add(this._chkMirror);
			this.Controls.Add(this._grpOptions);
			this.Controls.Add(this._txtUploadPack);
			this.Controls.Add(this._txtReceivePack);
			this.Controls.Add(this._txtVCS);
			this.Controls.Add(this._txtProxy);
			this.Controls.Add(this._txtPushURL);
			this.Controls.Add(this._txtFetchURL);
			this.Controls.Add(this._lblProxy);
			this.Controls.Add(this._lblPushURL);
			this.Controls.Add(this._lblFetchURL);
			this.Name = "RemotePropertiesDialog";
			this.Size = new System.Drawing.Size(400, 452);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _lblFetchURL;
		private System.Windows.Forms.Label _lblPushURL;
		private System.Windows.Forms.TextBox _txtFetchURL;
		private System.Windows.Forms.TextBox _txtPushURL;
		private System.Windows.Forms.TextBox _txtProxy;
		private System.Windows.Forms.Label _lblProxy;
		private gitter.Framework.Controls.GroupSeparator _grpUpdatedReferences;
		private gitter.Framework.Controls.GroupSeparator _grpOptions;
		private System.Windows.Forms.CheckBox _chkMirror;
		private System.Windows.Forms.CheckBox _chkSkipFetchAll;
		private System.Windows.Forms.Label _lblVCS;
		private System.Windows.Forms.TextBox _txtVCS;
		private System.Windows.Forms.Label _lblReceivePack;
		private System.Windows.Forms.Label _lblUploadPack;
		private System.Windows.Forms.TextBox _txtReceivePack;
		private System.Windows.Forms.TextBox _txtUploadPack;
		private System.Windows.Forms.Label _lblFetchTags;
		private System.Windows.Forms.RadioButton _radNormal;
		private System.Windows.Forms.RadioButton _radFetchAll;
		private System.Windows.Forms.RadioButton _radFetchNone;
		private gitter.Framework.Controls.CustomListBox _lstUpdatedReferences;
		private System.Windows.Forms.Button _btnAddRefspec;
		private System.Windows.Forms.Label _lblRefspec;
		private System.Windows.Forms.TextBox _txtRefspec;
		private System.Windows.Forms.RadioButton _radFetch;
		private System.Windows.Forms.RadioButton _radPush;
		private System.Windows.Forms.Panel panel1;
	}
}
