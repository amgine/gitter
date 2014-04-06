namespace gitter
{
	partial class AboutDialog
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
			this.label1 = new System.Windows.Forms.Label();
			this._logoPictureBox = new System.Windows.Forms.PictureBox();
			this.textBoxDescription = new System.Windows.Forms.Label();
			this.labelCopyright = new System.Windows.Forms.Label();
			this.labelCompanyName = new System.Windows.Forms.Label();
			this.labelVersion = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.label2 = new System.Windows.Forms.Label();
			this._btnCheckForUpdates = new System.Windows.Forms.Button();
			this._pnlUpdates = new System.Windows.Forms.Panel();
			this._lblUpdateStatus = new System.Windows.Forms.Label();
			this._btnUpdate = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this._logoPictureBox)).BeginInit();
			this._pnlUpdates.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.Location = new System.Drawing.Point(8, 214);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(397, 35);
			this.label1.TabIndex = 24;
			this.label1.Text = "Some Icons are Copyright © Yusuke Kamiyamane. All rights reserved. Licensed under" +
    " a Creative Commons Attribution 3.0 license.";
			// 
			// _logoPictureBox
			// 
			this._logoPictureBox.Dock = System.Windows.Forms.DockStyle.Top;
			this._logoPictureBox.Image = global::gitter.Properties.Resources.ImgStartPageLogo;
			this._logoPictureBox.Location = new System.Drawing.Point(0, 0);
			this._logoPictureBox.Name = "_logoPictureBox";
			this._logoPictureBox.Size = new System.Drawing.Size(500, 90);
			this._logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this._logoPictureBox.TabIndex = 13;
			this._logoPictureBox.TabStop = false;
			// 
			// textBoxDescription
			// 
			this.textBoxDescription.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.textBoxDescription.Location = new System.Drawing.Point(8, 93);
			this.textBoxDescription.Name = "textBoxDescription";
			this.textBoxDescription.Size = new System.Drawing.Size(362, 17);
			this.textBoxDescription.TabIndex = 25;
			this.textBoxDescription.Text = "Graphical interface for git content tracker.";
			this.textBoxDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelCopyright
			// 
			this.labelCopyright.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelCopyright.Location = new System.Drawing.Point(8, 145);
			this.labelCopyright.Name = "labelCopyright";
			this.labelCopyright.Size = new System.Drawing.Size(158, 17);
			this.labelCopyright.TabIndex = 25;
			this.labelCopyright.Text = "Written by: Popovskiy Maxim";
			this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelCompanyName
			// 
			this.labelCompanyName.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelCompanyName.Location = new System.Drawing.Point(8, 171);
			this.labelCompanyName.Name = "labelCompanyName";
			this.labelCompanyName.Size = new System.Drawing.Size(427, 33);
			this.labelCompanyName.TabIndex = 25;
			this.labelCompanyName.Text = "gitter is free software; you can redistribute it and/or modify it under the terms" +
    " of the GNU General Public License.";
			this.labelCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelVersion
			// 
			this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelVersion.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelVersion.Location = new System.Drawing.Point(69, 119);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(70, 17);
			this.labelVersion.TabIndex = 26;
			this.labelVersion.Text = "[Version]";
			this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// linkLabel1
			// 
			this.linkLabel1.Location = new System.Drawing.Point(172, 145);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(149, 17);
			this.linkLabel1.TabIndex = 27;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "amgine.gitter@gmail.com";
			this.linkLabel1.LinkClicked += OnEmailLinkClicked;
			// 
			// label2
			// 
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label2.Location = new System.Drawing.Point(8, 119);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(52, 17);
			this.label2.TabIndex = 25;
			this.label2.Text = "Version:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _btnCheckForUpdates
			// 
			this._btnCheckForUpdates.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnCheckForUpdates.Location = new System.Drawing.Point(0, 0);
			this._btnCheckForUpdates.Name = "_btnCheckForUpdates";
			this._btnCheckForUpdates.Size = new System.Drawing.Size(123, 23);
			this._btnCheckForUpdates.TabIndex = 28;
			this._btnCheckForUpdates.Text = "Check For Updates";
			this._btnCheckForUpdates.UseVisualStyleBackColor = true;
			this._btnCheckForUpdates.Click += new System.EventHandler(this.OnCheckForUpdatesClick);
			// 
			// _pnlUpdates
			// 
			this._pnlUpdates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._pnlUpdates.Controls.Add(this._lblUpdateStatus);
			this._pnlUpdates.Controls.Add(this._btnUpdate);
			this._pnlUpdates.Controls.Add(this._btnCheckForUpdates);
			this._pnlUpdates.Location = new System.Drawing.Point(175, 116);
			this._pnlUpdates.Name = "_pnlUpdates";
			this._pnlUpdates.Size = new System.Drawing.Size(322, 23);
			this._pnlUpdates.TabIndex = 29;
			// 
			// _lblUpdateStatus
			// 
			this._lblUpdateStatus.AutoSize = true;
			this._lblUpdateStatus.Location = new System.Drawing.Point(-3, 4);
			this._lblUpdateStatus.Name = "_lblUpdateStatus";
			this._lblUpdateStatus.Size = new System.Drawing.Size(57, 15);
			this._lblUpdateStatus.TabIndex = 29;
			this._lblUpdateStatus.Text = "[STATUS]";
			this._lblUpdateStatus.Visible = false;
			// 
			// _btnUpdate
			// 
			this._btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnUpdate.Location = new System.Drawing.Point(190, 0);
			this._btnUpdate.Name = "_btnUpdate";
			this._btnUpdate.Size = new System.Drawing.Size(88, 23);
			this._btnUpdate.TabIndex = 28;
			this._btnUpdate.Text = "Update";
			this._btnUpdate.UseVisualStyleBackColor = true;
			this._btnUpdate.Visible = false;
			this._btnUpdate.Click += new System.EventHandler(this.OnUpdateClick);
			// 
			// AboutDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._pnlUpdates);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.labelVersion);
			this.Controls.Add(this.labelCompanyName);
			this.Controls.Add(this.labelCopyright);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxDescription);
			this.Controls.Add(this._logoPictureBox);
			this.Controls.Add(this.label1);
			this.Name = "AboutDialog";
			this.Size = new System.Drawing.Size(500, 252);
			((System.ComponentModel.ISupportInitialize)(this._logoPictureBox)).EndInit();
			this._pnlUpdates.ResumeLayout(false);
			this._pnlUpdates.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox _logoPictureBox;
		private System.Windows.Forms.Label textBoxDescription;
		private System.Windows.Forms.Label labelCopyright;
		private System.Windows.Forms.Label labelCompanyName;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button _btnCheckForUpdates;
		private System.Windows.Forms.Panel _pnlUpdates;
		private System.Windows.Forms.Button _btnUpdate;
		private System.Windows.Forms.Label _lblUpdateStatus;
	}
}
