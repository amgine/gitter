namespace gitter.Git.Gui.Dialogs
{
	partial class VersionCheckDialog
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
			this._lblInstalledVersion = new System.Windows.Forms.Label();
			this._lblInstalledVersionValue = new System.Windows.Forms.Label();
			this._lblRequiredVersionValue = new System.Windows.Forms.Label();
			this._lblRequiredVersion = new System.Windows.Forms.Label();
			this._lnkRefresh = new System.Windows.Forms.LinkLabel();
			this._lnkGoToDownloadPage = new System.Windows.Forms.LinkLabel();
			this._picWarning = new System.Windows.Forms.PictureBox();
			this._lblMessage = new System.Windows.Forms.Label();
			this._lblAdditionalMessage = new System.Windows.Forms.Label();
			this._lnkConfigure = new System.Windows.Forms.LinkLabel();
			this._lblLatestVersion = new System.Windows.Forms.Label();
			this._lblLatestVersionValue = new System.Windows.Forms.Label();
			this._lnkRefreshLatestVersion = new System.Windows.Forms.LinkLabel();
			this._lnkDownload = new System.Windows.Forms.LinkLabel();
			((System.ComponentModel.ISupportInitialize)(this._picWarning)).BeginInit();
			this.SuspendLayout();
			// 
			// _lblInstalledVersion
			// 
			this._lblInstalledVersion.AutoSize = true;
			this._lblInstalledVersion.Location = new System.Drawing.Point(0, 60);
			this._lblInstalledVersion.Name = "_lblInstalledVersion";
			this._lblInstalledVersion.Size = new System.Drawing.Size(132, 15);
			this._lblInstalledVersion.TabIndex = 0;
			this._lblInstalledVersion.Text = "%Installed git version%:";
			// 
			// _lblInstalledVersionValue
			// 
			this._lblInstalledVersionValue.AutoSize = true;
			this._lblInstalledVersionValue.Location = new System.Drawing.Point(141, 60);
			this._lblInstalledVersionValue.Name = "_lblInstalledVersionValue";
			this._lblInstalledVersionValue.Size = new System.Drawing.Size(40, 15);
			this._lblInstalledVersionValue.TabIndex = 1;
			this._lblInstalledVersionValue.Text = "0.0.0.0";
			// 
			// _lblRequiredVersionValue
			// 
			this._lblRequiredVersionValue.AutoSize = true;
			this._lblRequiredVersionValue.Location = new System.Drawing.Point(141, 80);
			this._lblRequiredVersionValue.Name = "_lblRequiredVersionValue";
			this._lblRequiredVersionValue.Size = new System.Drawing.Size(40, 15);
			this._lblRequiredVersionValue.TabIndex = 3;
			this._lblRequiredVersionValue.Text = "0.0.0.0";
			// 
			// _lblRequiredVersion
			// 
			this._lblRequiredVersion.AutoSize = true;
			this._lblRequiredVersion.Location = new System.Drawing.Point(0, 80);
			this._lblRequiredVersion.Name = "_lblRequiredVersion";
			this._lblRequiredVersion.Size = new System.Drawing.Size(135, 15);
			this._lblRequiredVersion.TabIndex = 2;
			this._lblRequiredVersion.Text = "%Required git version%:";
			// 
			// _lnkRefresh
			// 
			this._lnkRefresh.AutoSize = true;
			this._lnkRefresh.Location = new System.Drawing.Point(228, 60);
			this._lnkRefresh.Name = "_lnkRefresh";
			this._lnkRefresh.Size = new System.Drawing.Size(63, 15);
			this._lnkRefresh.TabIndex = 4;
			this._lnkRefresh.TabStop = true;
			this._lnkRefresh.Text = "%refresh%";
			this._lnkRefresh.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnRefreshLoacalVersion);
			// 
			// _lnkGoToDownloadPage
			// 
			this._lnkGoToDownloadPage.AutoSize = true;
			this._lnkGoToDownloadPage.Location = new System.Drawing.Point(228, 80);
			this._lnkGoToDownloadPage.Name = "_lnkGoToDownloadPage";
			this._lnkGoToDownloadPage.Size = new System.Drawing.Size(80, 15);
			this._lnkGoToDownloadPage.TabIndex = 5;
			this._lnkGoToDownloadPage.TabStop = true;
			this._lnkGoToDownloadPage.Text = "%download%";
			this._lnkGoToDownloadPage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnGoToDownloadPage);
			// 
			// _picWarning
			// 
			this._picWarning.Image = global::gitter.Git.Properties.Resources.ImgWarning48;
			this._picWarning.Location = new System.Drawing.Point(3, 3);
			this._picWarning.Name = "_picWarning";
			this._picWarning.Size = new System.Drawing.Size(48, 48);
			this._picWarning.TabIndex = 6;
			this._picWarning.TabStop = false;
			// 
			// _lblMessage
			// 
			this._lblMessage.Location = new System.Drawing.Point(62, 3);
			this._lblMessage.Name = "_lblMessage";
			this._lblMessage.Size = new System.Drawing.Size(320, 48);
			this._lblMessage.TabIndex = 7;
			this._lblMessage.Text = "%Message%";
			// 
			// _lblAdditionalMessage
			// 
			this._lblAdditionalMessage.Location = new System.Drawing.Point(0, 129);
			this._lblAdditionalMessage.Name = "_lblAdditionalMessage";
			this._lblAdditionalMessage.Size = new System.Drawing.Size(382, 33);
			this._lblAdditionalMessage.TabIndex = 8;
			this._lblAdditionalMessage.Text = "%git support will be disabled until minimum required version is installed.%";
			// 
			// _lnkConfigure
			// 
			this._lnkConfigure.AutoSize = true;
			this._lnkConfigure.Location = new System.Drawing.Point(297, 60);
			this._lnkConfigure.Name = "_lnkConfigure";
			this._lnkConfigure.Size = new System.Drawing.Size(78, 15);
			this._lnkConfigure.TabIndex = 9;
			this._lnkConfigure.TabStop = true;
			this._lnkConfigure.Text = "%configure%";
			this._lnkConfigure.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnConfigureClick);
			// 
			// _lblLatestVersion
			// 
			this._lblLatestVersion.AutoSize = true;
			this._lblLatestVersion.Location = new System.Drawing.Point(0, 100);
			this._lblLatestVersion.Name = "_lblLatestVersion";
			this._lblLatestVersion.Size = new System.Drawing.Size(103, 15);
			this._lblLatestVersion.TabIndex = 10;
			this._lblLatestVersion.Text = "%Latest Version%:";
			// 
			// _lblLatestVersionValue
			// 
			this._lblLatestVersionValue.AutoSize = true;
			this._lblLatestVersionValue.Location = new System.Drawing.Point(141, 100);
			this._lblLatestVersionValue.Name = "_lblLatestVersionValue";
			this._lblLatestVersionValue.Size = new System.Drawing.Size(67, 15);
			this._lblLatestVersionValue.TabIndex = 10;
			this._lblLatestVersionValue.Text = "searching...";
			// 
			// _lnkRefreshLatestVersion
			// 
			this._lnkRefreshLatestVersion.AutoSize = true;
			this._lnkRefreshLatestVersion.Location = new System.Drawing.Point(228, 100);
			this._lnkRefreshLatestVersion.Name = "_lnkRefreshLatestVersion";
			this._lnkRefreshLatestVersion.Size = new System.Drawing.Size(63, 15);
			this._lnkRefreshLatestVersion.TabIndex = 4;
			this._lnkRefreshLatestVersion.TabStop = true;
			this._lnkRefreshLatestVersion.Text = "%refresh%";
			this._lnkRefreshLatestVersion.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnRefreshLatestVersionClick);
			// 
			// _lnkDownload
			// 
			this._lnkDownload.AutoSize = true;
			this._lnkDownload.Location = new System.Drawing.Point(297, 100);
			this._lnkDownload.Name = "_lnkDownload";
			this._lnkDownload.Size = new System.Drawing.Size(80, 15);
			this._lnkDownload.TabIndex = 5;
			this._lnkDownload.TabStop = true;
			this._lnkDownload.Text = "%download%";
			this._lnkDownload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnDownloadClick);
			// 
			// VersionCheckDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lblLatestVersionValue);
			this.Controls.Add(this._lblLatestVersion);
			this.Controls.Add(this._lnkConfigure);
			this.Controls.Add(this._lblAdditionalMessage);
			this.Controls.Add(this._lblMessage);
			this.Controls.Add(this._picWarning);
			this.Controls.Add(this._lnkDownload);
			this.Controls.Add(this._lnkGoToDownloadPage);
			this.Controls.Add(this._lnkRefreshLatestVersion);
			this.Controls.Add(this._lnkRefresh);
			this.Controls.Add(this._lblRequiredVersionValue);
			this.Controls.Add(this._lblRequiredVersion);
			this.Controls.Add(this._lblInstalledVersionValue);
			this.Controls.Add(this._lblInstalledVersion);
			this.Name = "VersionCheckDialog";
			this.Size = new System.Drawing.Size(385, 162);
			((System.ComponentModel.ISupportInitialize)(this._picWarning)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _lblInstalledVersion;
		private System.Windows.Forms.Label _lblInstalledVersionValue;
		private System.Windows.Forms.Label _lblRequiredVersionValue;
		private System.Windows.Forms.Label _lblRequiredVersion;
		private System.Windows.Forms.LinkLabel _lnkRefresh;
		private System.Windows.Forms.LinkLabel _lnkGoToDownloadPage;
		private System.Windows.Forms.PictureBox _picWarning;
		private System.Windows.Forms.Label _lblMessage;
		private System.Windows.Forms.Label _lblAdditionalMessage;
		private System.Windows.Forms.LinkLabel _lnkConfigure;
		private System.Windows.Forms.Label _lblLatestVersion;
		private System.Windows.Forms.Label _lblLatestVersionValue;
		private System.Windows.Forms.LinkLabel _lnkRefreshLatestVersion;
		private System.Windows.Forms.LinkLabel _lnkDownload;
	}
}
