namespace gitter.Git
{
	partial class CliOptionsPage
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
			this._lblPathToGitExe = new System.Windows.Forms.Label();
			this._txtmSysGitPath = new System.Windows.Forms.TextBox();
			this._cmdAutoDetect = new System.Windows.Forms.Button();
			this._lblFoundVersion = new System.Windows.Forms.Label();
			this._cmdBrowse = new System.Windows.Forms.Button();
			this._lblVersion = new System.Windows.Forms.Label();
			this._radAlwaysAutodetect = new System.Windows.Forms.RadioButton();
			this._radSpecifyManually = new System.Windows.Forms.RadioButton();
			this._lblLatestVersionValue = new System.Windows.Forms.Label();
			this._lblLatestVersion = new System.Windows.Forms.Label();
			this._btnDownload = new System.Windows.Forms.Button();
			this._btnRefreshLatestVersion = new System.Windows.Forms.Button();
			this._chkFallbackToAnsi = new System.Windows.Forms.CheckBox();
			this._chkLogCLICalls = new System.Windows.Forms.CheckBox();
			this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// _lblPathToGitExe
			// 
			this._lblPathToGitExe.AutoSize = true;
			this._lblPathToGitExe.Location = new System.Drawing.Point(0, 0);
			this._lblPathToGitExe.Name = "_lblPathToGitExe";
			this._lblPathToGitExe.Size = new System.Drawing.Size(105, 15);
			this._lblPathToGitExe.TabIndex = 1;
			this._lblPathToGitExe.Text = "%Path to git.exe%:";
			// 
			// _txtmSysGitPath
			// 
			this._txtmSysGitPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtmSysGitPath.Location = new System.Drawing.Point(3, 24);
			this._txtmSysGitPath.Name = "_txtmSysGitPath";
			this._txtmSysGitPath.Size = new System.Drawing.Size(473, 23);
			this._txtmSysGitPath.TabIndex = 2;
			this._txtmSysGitPath.TextChanged += new System.EventHandler(this._txtmSysGitPath_TextChanged);
			// 
			// _cmdAutoDetect
			// 
			this._cmdAutoDetect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._cmdAutoDetect.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._cmdAutoDetect.Location = new System.Drawing.Point(270, 53);
			this._cmdAutoDetect.Name = "_cmdAutoDetect";
			this._cmdAutoDetect.Size = new System.Drawing.Size(100, 23);
			this._cmdAutoDetect.TabIndex = 3;
			this._cmdAutoDetect.Text = "%Autodetect%";
			this._cmdAutoDetect.UseVisualStyleBackColor = true;
			this._cmdAutoDetect.Click += new System.EventHandler(this._cmdAutoDetect_Click);
			// 
			// _lblFoundVersion
			// 
			this._lblFoundVersion.AutoSize = true;
			this._lblFoundVersion.Location = new System.Drawing.Point(0, 57);
			this._lblFoundVersion.Name = "_lblFoundVersion";
			this._lblFoundVersion.Size = new System.Drawing.Size(106, 15);
			this._lblFoundVersion.TabIndex = 7;
			this._lblFoundVersion.Text = "%Found Version%:";
			// 
			// _cmdBrowse
			// 
			this._cmdBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._cmdBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._cmdBrowse.Location = new System.Drawing.Point(376, 53);
			this._cmdBrowse.Name = "_cmdBrowse";
			this._cmdBrowse.Size = new System.Drawing.Size(100, 23);
			this._cmdBrowse.TabIndex = 9;
			this._cmdBrowse.Text = "%Browse%...";
			this._cmdBrowse.UseVisualStyleBackColor = true;
			this._cmdBrowse.Click += new System.EventHandler(this._cmdBrowse_Click);
			// 
			// _lblVersion
			// 
			this._lblVersion.AutoSize = true;
			this._lblVersion.Location = new System.Drawing.Point(114, 57);
			this._lblVersion.Name = "_lblVersion";
			this._lblVersion.Size = new System.Drawing.Size(83, 15);
			this._lblVersion.TabIndex = 11;
			this._lblVersion.Text = "<unavailable>";
			// 
			// _radAlwaysAutodetect
			// 
			this._radAlwaysAutodetect.AutoSize = true;
			this._radAlwaysAutodetect.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radAlwaysAutodetect.Location = new System.Drawing.Point(117, -2);
			this._radAlwaysAutodetect.Name = "_radAlwaysAutodetect";
			this._radAlwaysAutodetect.Size = new System.Drawing.Size(148, 20);
			this._radAlwaysAutodetect.TabIndex = 12;
			this._radAlwaysAutodetect.TabStop = true;
			this._radAlwaysAutodetect.Text = "%Always autodetect%";
			this._radAlwaysAutodetect.UseVisualStyleBackColor = true;
			this._radAlwaysAutodetect.CheckedChanged += new System.EventHandler(this._radAlwaysAutodetect_CheckedChanged);
			// 
			// _radSpecifyManually
			// 
			this._radSpecifyManually.AutoSize = true;
			this._radSpecifyManually.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radSpecifyManually.Location = new System.Drawing.Point(270, -2);
			this._radSpecifyManually.Name = "_radSpecifyManually";
			this._radSpecifyManually.Size = new System.Drawing.Size(141, 20);
			this._radSpecifyManually.TabIndex = 13;
			this._radSpecifyManually.TabStop = true;
			this._radSpecifyManually.Text = "%Specify manually%";
			this._radSpecifyManually.UseVisualStyleBackColor = true;
			// 
			// _lblLatestVersionValue
			// 
			this._lblLatestVersionValue.AutoSize = true;
			this._lblLatestVersionValue.Location = new System.Drawing.Point(114, 86);
			this._lblLatestVersionValue.Name = "_lblLatestVersionValue";
			this._lblLatestVersionValue.Size = new System.Drawing.Size(81, 15);
			this._lblLatestVersionValue.TabIndex = 21;
			this._lblLatestVersionValue.Text = "%serching%...";
			// 
			// _lblLatestVersion
			// 
			this._lblLatestVersion.AutoSize = true;
			this._lblLatestVersion.Location = new System.Drawing.Point(0, 86);
			this._lblLatestVersion.Name = "_lblLatestVersion";
			this._lblLatestVersion.Size = new System.Drawing.Size(103, 15);
			this._lblLatestVersion.TabIndex = 20;
			this._lblLatestVersion.Text = "%Latest Version%:";
			// 
			// _btnDownload
			// 
			this._btnDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnDownload.Enabled = false;
			this._btnDownload.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnDownload.Location = new System.Drawing.Point(376, 82);
			this._btnDownload.Name = "_btnDownload";
			this._btnDownload.Size = new System.Drawing.Size(100, 23);
			this._btnDownload.TabIndex = 19;
			this._btnDownload.Text = "%Download%";
			this._btnDownload.UseVisualStyleBackColor = true;
			this._btnDownload.Click += new System.EventHandler(this._btnDownload_Click);
			// 
			// _btnRefreshLatestVersion
			// 
			this._btnRefreshLatestVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnRefreshLatestVersion.Enabled = false;
			this._btnRefreshLatestVersion.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnRefreshLatestVersion.Location = new System.Drawing.Point(270, 82);
			this._btnRefreshLatestVersion.Name = "_btnRefreshLatestVersion";
			this._btnRefreshLatestVersion.Size = new System.Drawing.Size(100, 23);
			this._btnRefreshLatestVersion.TabIndex = 18;
			this._btnRefreshLatestVersion.Text = "%Refresh%";
			this._btnRefreshLatestVersion.UseVisualStyleBackColor = true;
			this._btnRefreshLatestVersion.Click += new System.EventHandler(this._btnRefreshLatestVersion_Click);
			// 
			// _chkFallbackToAnsi
			// 
			this._chkFallbackToAnsi.AutoSize = true;
			this._chkFallbackToAnsi.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkFallbackToAnsi.Location = new System.Drawing.Point(3, 133);
			this._chkFallbackToAnsi.Name = "_chkFallbackToAnsi";
			this._chkFallbackToAnsi.Size = new System.Drawing.Size(372, 20);
			this._chkFallbackToAnsi.TabIndex = 17;
			this._chkFallbackToAnsi.Text = "%Fallback to Ansi codepage if UTF-8 fails to decode characters%";
			this._chkFallbackToAnsi.UseVisualStyleBackColor = true;
			// 
			// _chkLogCLICalls
			// 
			this._chkLogCLICalls.AutoSize = true;
			this._chkLogCLICalls.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkLogCLICalls.Location = new System.Drawing.Point(3, 111);
			this._chkLogCLICalls.Name = "_chkLogCLICalls";
			this._chkLogCLICalls.Size = new System.Drawing.Size(227, 20);
			this._chkLogCLICalls.TabIndex = 17;
			this._chkLogCLICalls.Text = "%Log command line interface calls%";
			this._chkLogCLICalls.UseVisualStyleBackColor = true;
			// 
			// CliOptionsPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lblLatestVersionValue);
			this.Controls.Add(this._lblLatestVersion);
			this.Controls.Add(this._lblPathToGitExe);
			this.Controls.Add(this._btnDownload);
			this.Controls.Add(this._cmdBrowse);
			this.Controls.Add(this._btnRefreshLatestVersion);
			this.Controls.Add(this._lblFoundVersion);
			this.Controls.Add(this._chkFallbackToAnsi);
			this.Controls.Add(this._lblVersion);
			this.Controls.Add(this._chkLogCLICalls);
			this.Controls.Add(this._cmdAutoDetect);
			this.Controls.Add(this._radAlwaysAutodetect);
			this.Controls.Add(this._radSpecifyManually);
			this.Controls.Add(this._txtmSysGitPath);
			this.Name = "CliOptionsPage";
			this.Size = new System.Drawing.Size(479, 158);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label _lblPathToGitExe;
		private System.Windows.Forms.TextBox _txtmSysGitPath;
		private System.Windows.Forms.Button _cmdAutoDetect;
		private System.Windows.Forms.Label _lblFoundVersion;
		private System.Windows.Forms.Button _cmdBrowse;
		private System.Windows.Forms.Label _lblVersion;
		private System.Windows.Forms.RadioButton _radAlwaysAutodetect;
		private System.Windows.Forms.RadioButton _radSpecifyManually;
		private System.Windows.Forms.OpenFileDialog _openFileDialog;
		private System.Windows.Forms.CheckBox _chkLogCLICalls;
		private System.Windows.Forms.Label _lblLatestVersionValue;
		private System.Windows.Forms.Label _lblLatestVersion;
		private System.Windows.Forms.Button _btnDownload;
		private System.Windows.Forms.Button _btnRefreshLatestVersion;
		private System.Windows.Forms.CheckBox _chkFallbackToAnsi;
	}
}
