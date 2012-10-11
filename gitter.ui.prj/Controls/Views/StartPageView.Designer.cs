namespace gitter
{
	partial class StartPageView
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
				_recentRepositoriesBinding.Dispose();
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
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.PictureBox _picLogo;
			System.Windows.Forms.PictureBox _picLogo2;
			this._chkShowPageAtStartup = new System.Windows.Forms.CheckBox();
			this._chkClosePageAfterRepositoryLoad = new System.Windows.Forms.CheckBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this._lstRecentRepositories = new gitter.RecentRepositoriesListBox();
			this._lstLocalRepositories = new gitter.LocalRepositoriesListBox();
			this._btnAddLocalRepo = new gitter.Controls.LinkButton();
			this._btnScanLocalRepo = new gitter.Controls.LinkButton();
			this._btnInitLocalRepo = new gitter.Controls.LinkButton();
			this._btnCloneRemoteRepo = new gitter.Controls.LinkButton();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			_picLogo = new System.Windows.Forms.PictureBox();
			_picLogo2 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(_picLogo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(_picLogo2)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			label1.Location = new System.Drawing.Point(239, 93);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(171, 23);
			label1.TabIndex = 2;
			label1.Text = "Local repositories";
			// 
			// label2
			// 
			label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			label2.Location = new System.Drawing.Point(9, 221);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(219, 23);
			label2.TabIndex = 2;
			label2.Text = "Recent repositories";
			// 
			// _picLogo
			// 
			_picLogo.Image = global::gitter.Properties.Resources.ImgStartPageLogo;
			_picLogo.Location = new System.Drawing.Point(0, 0);
			_picLogo.Margin = new System.Windows.Forms.Padding(0);
			_picLogo.Name = "_picLogo";
			_picLogo.Size = new System.Drawing.Size(527, 90);
			_picLogo.TabIndex = 6;
			_picLogo.TabStop = false;
			_picLogo.Click += new System.EventHandler(this.OnLogoClick);
			// 
			// _picLogo2
			// 
			_picLogo2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			_picLogo2.Image = global::gitter.Properties.Resources.ImgStartPageLogoGradient;
			_picLogo2.Location = new System.Drawing.Point(527, 0);
			_picLogo2.Margin = new System.Windows.Forms.Padding(0);
			_picLogo2.Name = "_picLogo2";
			_picLogo2.Size = new System.Drawing.Size(164, 90);
			_picLogo2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			_picLogo2.TabIndex = 7;
			_picLogo2.TabStop = false;
			_picLogo2.Click += new System.EventHandler(this.OnLogoClick);
			// 
			// _chkShowPageAtStartup
			// 
			this._chkShowPageAtStartup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._chkShowPageAtStartup.AutoSize = true;
			this._chkShowPageAtStartup.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkShowPageAtStartup.Location = new System.Drawing.Point(9, 511);
			this._chkShowPageAtStartup.Margin = new System.Windows.Forms.Padding(0);
			this._chkShowPageAtStartup.Name = "_chkShowPageAtStartup";
			this._chkShowPageAtStartup.Size = new System.Drawing.Size(147, 20);
			this._chkShowPageAtStartup.TabIndex = 5;
			this._chkShowPageAtStartup.Text = "Show page on startup";
			this._chkShowPageAtStartup.UseVisualStyleBackColor = true;
			this._chkShowPageAtStartup.CheckedChanged += new System.EventHandler(this._chkShowPageAtStartup_CheckedChanged);
			// 
			// _chkClosePageAfterRepositoryLoad
			// 
			this._chkClosePageAfterRepositoryLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._chkClosePageAfterRepositoryLoad.AutoSize = true;
			this._chkClosePageAfterRepositoryLoad.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkClosePageAfterRepositoryLoad.Location = new System.Drawing.Point(9, 491);
			this._chkClosePageAfterRepositoryLoad.Margin = new System.Windows.Forms.Padding(0);
			this._chkClosePageAfterRepositoryLoad.Name = "_chkClosePageAfterRepositoryLoad";
			this._chkClosePageAfterRepositoryLoad.Size = new System.Drawing.Size(199, 20);
			this._chkClosePageAfterRepositoryLoad.TabIndex = 5;
			this._chkClosePageAfterRepositoryLoad.Text = "Close page after repository load";
			this._chkClosePageAfterRepositoryLoad.UseVisualStyleBackColor = true;
			this._chkClosePageAfterRepositoryLoad.CheckedChanged += new System.EventHandler(this._chkClosePageAfterRepositoryLoad_CheckedChanged);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(226)))), ((int)(((byte)(252)))));
			this.panel1.Location = new System.Drawing.Point(232, 103);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1, 418);
			this.panel1.TabIndex = 9;
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(226)))), ((int)(((byte)(252)))));
			this.panel2.Location = new System.Drawing.Point(147, 234);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(80, 1);
			this.panel2.TabIndex = 10;
			// 
			// _lstRecentRepositories
			// 
			this._lstRecentRepositories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this._lstRecentRepositories.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._lstRecentRepositories.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._lstRecentRepositories.Location = new System.Drawing.Point(3, 247);
			this._lstRecentRepositories.MaximumSize = new System.Drawing.Size(500, 9000);
			this._lstRecentRepositories.Name = "_lstRecentRepositories";
			this._lstRecentRepositories.Size = new System.Drawing.Size(225, 241);
			this._lstRecentRepositories.TabIndex = 4;
			// 
			// _lstLocalRepositories
			// 
			this._lstLocalRepositories.AllowDrop = true;
			this._lstLocalRepositories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lstLocalRepositories.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._lstLocalRepositories.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._lstLocalRepositories.Location = new System.Drawing.Point(234, 119);
			this._lstLocalRepositories.MaximumSize = new System.Drawing.Size(500, 9000);
			this._lstLocalRepositories.Name = "_lstLocalRepositories";
			this._lstLocalRepositories.Size = new System.Drawing.Size(457, 417);
			this._lstLocalRepositories.TabIndex = 1;
			this._lstLocalRepositories.Text = "No repositories found";
			// 
			// _btnAddLocalRepo
			// 
			this._btnAddLocalRepo.Font = new System.Drawing.Font("Segoe UI", 9F);
			this._btnAddLocalRepo.Image = global::gitter.Properties.Resources.ImgRepoAddMedium;
			this._btnAddLocalRepo.Location = new System.Drawing.Point(20, 121);
			this._btnAddLocalRepo.Margin = new System.Windows.Forms.Padding(1);
			this._btnAddLocalRepo.Name = "_btnAddLocalRepo";
			this._btnAddLocalRepo.Size = new System.Drawing.Size(208, 28);
			this._btnAddLocalRepo.TabIndex = 8;
			this._btnAddLocalRepo.Text = "Add Local Repository...";
			this._btnAddLocalRepo.LinkClicked += new System.EventHandler(this._btnAddLocalRepo_LinkClicked);
			// 
			// _btnScanLocalRepo
			// 
			this._btnScanLocalRepo.Font = new System.Drawing.Font("Segoe UI", 9F);
			this._btnScanLocalRepo.Image = global::gitter.Properties.Resources.ImgRepoScanMedium;
			this._btnScanLocalRepo.Location = new System.Drawing.Point(20, 93);
			this._btnScanLocalRepo.Margin = new System.Windows.Forms.Padding(1);
			this._btnScanLocalRepo.Name = "_btnScanLocalRepo";
			this._btnScanLocalRepo.Size = new System.Drawing.Size(208, 28);
			this._btnScanLocalRepo.TabIndex = 8;
			this._btnScanLocalRepo.Text = "Scan Local Repositories...";
			this._btnScanLocalRepo.Visible = false;
			this._btnScanLocalRepo.LinkClicked += new System.EventHandler(this._btnScanLocalRepo_LinkClicked);
			// 
			// _btnInitLocalRepo
			// 
			this._btnInitLocalRepo.Font = new System.Drawing.Font("Segoe UI", 9F);
			this._btnInitLocalRepo.Image = global::gitter.Properties.Resources.ImgRepoInitMedium;
			this._btnInitLocalRepo.Location = new System.Drawing.Point(20, 151);
			this._btnInitLocalRepo.Margin = new System.Windows.Forms.Padding(1);
			this._btnInitLocalRepo.Name = "_btnInitLocalRepo";
			this._btnInitLocalRepo.Size = new System.Drawing.Size(208, 28);
			this._btnInitLocalRepo.TabIndex = 8;
			this._btnInitLocalRepo.Text = "Init Local Repository...";
			this._btnInitLocalRepo.LinkClicked += new System.EventHandler(this._btnInitLocalRepo_LinkClicked);
			// 
			// _btnCloneRemoteRepo
			// 
			this._btnCloneRemoteRepo.Font = new System.Drawing.Font("Segoe UI", 9F);
			this._btnCloneRemoteRepo.Image = global::gitter.Properties.Resources.ImgRepoCloneMedium;
			this._btnCloneRemoteRepo.Location = new System.Drawing.Point(20, 181);
			this._btnCloneRemoteRepo.Margin = new System.Windows.Forms.Padding(1);
			this._btnCloneRemoteRepo.Name = "_btnCloneRemoteRepo";
			this._btnCloneRemoteRepo.Size = new System.Drawing.Size(208, 28);
			this._btnCloneRemoteRepo.TabIndex = 8;
			this._btnCloneRemoteRepo.Text = "Clone Remote Repository...";
			this._btnCloneRemoteRepo.LinkClicked += new System.EventHandler(this._btnCloneRemoteRepo_LinkClicked);
			// 
			// StartPageView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel2);
			this.Controls.Add(_picLogo);
			this.Controls.Add(this._lstRecentRepositories);
			this.Controls.Add(this._lstLocalRepositories);
			this.Controls.Add(this._chkShowPageAtStartup);
			this.Controls.Add(this.panel1);
			this.Controls.Add(_picLogo2);
			this.Controls.Add(this._chkClosePageAfterRepositoryLoad);
			this.Controls.Add(this._btnAddLocalRepo);
			this.Controls.Add(label1);
			this.Controls.Add(this._btnScanLocalRepo);
			this.Controls.Add(this._btnInitLocalRepo);
			this.Controls.Add(this._btnCloneRemoteRepo);
			this.Controls.Add(label2);
			this.Name = "StartPageView";
			this.Size = new System.Drawing.Size(691, 536);
			((System.ComponentModel.ISupportInitialize)(_picLogo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(_picLogo2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private LocalRepositoriesListBox _lstLocalRepositories;
		private RecentRepositoriesListBox _lstRecentRepositories;
		private System.Windows.Forms.CheckBox _chkShowPageAtStartup;
		private System.Windows.Forms.CheckBox _chkClosePageAfterRepositoryLoad;
		private Controls.LinkButton _btnAddLocalRepo;
		private Controls.LinkButton _btnScanLocalRepo;
		private Controls.LinkButton _btnInitLocalRepo;
		private Controls.LinkButton _btnCloneRemoteRepo;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
	}
}
