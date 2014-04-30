namespace gitter.Git.Gui.Dialogs
{
	partial class CreateBranchDialog
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
				if(components != null)
				{
					components.Dispose();
				}
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
			this._pnlOptions = new System.Windows.Forms.Panel();
			this._chkOrphan = new System.Windows.Forms.CheckBox();
			this._chkCheckoutAfterCreation = new System.Windows.Forms.CheckBox();
			this._grpOptions = new gitter.Framework.Controls.GroupSeparator();
			this._chkCreateReflog = new System.Windows.Forms.CheckBox();
			this._txtName = new System.Windows.Forms.TextBox();
			this._lblRevision = new System.Windows.Forms.Label();
			this._lblName = new System.Windows.Forms.Label();
			this._txtRevision = new gitter.Git.Gui.Controls.RevisionPicker();
			this.panel1 = new System.Windows.Forms.Panel();
			this._trackingDoNotTrack = new System.Windows.Forms.RadioButton();
			this._trackingTrack = new System.Windows.Forms.RadioButton();
			this._trackingDefault = new System.Windows.Forms.RadioButton();
			this._grpTracking = new gitter.Framework.Controls.GroupSeparator();
			this._pnlOptions.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _pnlOptions
			// 
			this._pnlOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._pnlOptions.Controls.Add(this._chkOrphan);
			this._pnlOptions.Controls.Add(this._chkCheckoutAfterCreation);
			this._pnlOptions.Controls.Add(this._grpOptions);
			this._pnlOptions.Controls.Add(this._chkCreateReflog);
			this._pnlOptions.Location = new System.Drawing.Point(0, 61);
			this._pnlOptions.Margin = new System.Windows.Forms.Padding(0);
			this._pnlOptions.Name = "_pnlOptions";
			this._pnlOptions.Size = new System.Drawing.Size(400, 90);
			this._pnlOptions.TabIndex = 8;
			// 
			// _chkOrphan
			// 
			this._chkOrphan.AutoSize = true;
			this._chkOrphan.Enabled = false;
			this._chkOrphan.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkOrphan.Location = new System.Drawing.Point(13, 45);
			this._chkOrphan.Name = "_chkOrphan";
			this._chkOrphan.Size = new System.Drawing.Size(162, 20);
			this._chkOrphan.TabIndex = 3;
			this._chkOrphan.Text = "%Make orphan branch%";
			this._chkOrphan.UseVisualStyleBackColor = true;
			// 
			// _chkCheckoutAfterCreation
			// 
			this._chkCheckoutAfterCreation.AutoSize = true;
			this._chkCheckoutAfterCreation.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkCheckoutAfterCreation.Location = new System.Drawing.Point(13, 25);
			this._chkCheckoutAfterCreation.Name = "_chkCheckoutAfterCreation";
			this._chkCheckoutAfterCreation.Size = new System.Drawing.Size(176, 20);
			this._chkCheckoutAfterCreation.TabIndex = 2;
			this._chkCheckoutAfterCreation.Text = "%Checkout after creation%";
			this._chkCheckoutAfterCreation.UseVisualStyleBackColor = true;
			this._chkCheckoutAfterCreation.CheckedChanged += new System.EventHandler(this.OnCheckoutAfterCreationCheckedChanged);
			// 
			// _grpOptions
			// 
			this._grpOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._grpOptions.Location = new System.Drawing.Point(0, 0);
			this._grpOptions.Name = "_grpOptions";
			this._grpOptions.Size = new System.Drawing.Size(397, 19);
			this._grpOptions.TabIndex = 0;
			this._grpOptions.Text = "%Options%";
			// 
			// _chkCreateReflog
			// 
			this._chkCreateReflog.AutoSize = true;
			this._chkCreateReflog.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkCreateReflog.Location = new System.Drawing.Point(13, 65);
			this._chkCreateReflog.Name = "_chkCreateReflog";
			this._chkCreateReflog.Size = new System.Drawing.Size(160, 20);
			this._chkCreateReflog.TabIndex = 4;
			this._chkCreateReflog.Text = "%Create branch reflog%";
			this._chkCreateReflog.UseVisualStyleBackColor = true;
			// 
			// _txtName
			// 
			this._txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtName.Location = new System.Drawing.Point(94, 3);
			this._txtName.Name = "_txtName";
			this._txtName.Size = new System.Drawing.Size(303, 23);
			this._txtName.TabIndex = 0;
			this._txtName.TextChanged += new System.EventHandler(this.OnBranchNameChanged);
			// 
			// _lblRevision
			// 
			this._lblRevision.AutoSize = true;
			this._lblRevision.Location = new System.Drawing.Point(0, 35);
			this._lblRevision.Name = "_lblRevision";
			this._lblRevision.Size = new System.Drawing.Size(74, 15);
			this._lblRevision.TabIndex = 4;
			this._lblRevision.Text = "%Revision%:";
			// 
			// _lblName
			// 
			this._lblName.AutoSize = true;
			this._lblName.Location = new System.Drawing.Point(0, 6);
			this._lblName.Name = "_lblName";
			this._lblName.Size = new System.Drawing.Size(62, 15);
			this._lblName.TabIndex = 1;
			this._lblName.Text = "%Name%:";
			// 
			// _txtRevision
			// 
			this._txtRevision.FormattingEnabled = true;
			this._txtRevision.Location = new System.Drawing.Point(94, 32);
			this._txtRevision.Name = "_txtRevision";
			this._txtRevision.Size = new System.Drawing.Size(303, 23);
			this._txtRevision.TabIndex = 1;
			this._txtRevision.TextChanged += new System.EventHandler(this.OnRevisionChanged);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this._trackingDoNotTrack);
			this.panel1.Controls.Add(this._trackingTrack);
			this.panel1.Controls.Add(this._trackingDefault);
			this.panel1.Controls.Add(this._grpTracking);
			this.panel1.Location = new System.Drawing.Point(0, 151);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(400, 49);
			this.panel1.TabIndex = 9;
			// 
			// _trackingDoNotTrack
			// 
			this._trackingDoNotTrack.AutoSize = true;
			this._trackingDoNotTrack.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._trackingDoNotTrack.Location = new System.Drawing.Point(108, 25);
			this._trackingDoNotTrack.Name = "_trackingDoNotTrack";
			this._trackingDoNotTrack.Size = new System.Drawing.Size(116, 20);
			this._trackingDoNotTrack.TabIndex = 6;
			this._trackingDoNotTrack.Text = "%Do not track%";
			this._trackingDoNotTrack.UseVisualStyleBackColor = true;
			// 
			// _trackingTrack
			// 
			this._trackingTrack.AutoSize = true;
			this._trackingTrack.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._trackingTrack.Location = new System.Drawing.Point(230, 25);
			this._trackingTrack.Name = "_trackingTrack";
			this._trackingTrack.Size = new System.Drawing.Size(80, 20);
			this._trackingTrack.TabIndex = 7;
			this._trackingTrack.Text = "%Track%";
			this._trackingTrack.UseVisualStyleBackColor = true;
			// 
			// _trackingDefault
			// 
			this._trackingDefault.AutoSize = true;
			this._trackingDefault.Checked = true;
			this._trackingDefault.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._trackingDefault.Location = new System.Drawing.Point(13, 25);
			this._trackingDefault.Name = "_trackingDefault";
			this._trackingDefault.Size = new System.Drawing.Size(89, 20);
			this._trackingDefault.TabIndex = 5;
			this._trackingDefault.TabStop = true;
			this._trackingDefault.Text = "%Default%";
			this._trackingDefault.UseVisualStyleBackColor = true;
			// 
			// _grpTracking
			// 
			this._grpTracking.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._grpTracking.Location = new System.Drawing.Point(0, 0);
			this._grpTracking.Name = "_grpTracking";
			this._grpTracking.Size = new System.Drawing.Size(397, 19);
			this._grpTracking.TabIndex = 0;
			this._grpTracking.Text = "%Tracking mode%";
			// 
			// CreateBranchDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this._txtRevision);
			this.Controls.Add(this._pnlOptions);
			this.Controls.Add(this._txtName);
			this.Controls.Add(this._lblRevision);
			this.Controls.Add(this._lblName);
			this.Name = "CreateBranchDialog";
			this.Size = new System.Drawing.Size(400, 200);
			this._pnlOptions.ResumeLayout(false);
			this._pnlOptions.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox _chkCheckoutAfterCreation;
		private System.Windows.Forms.Label _lblName;
		private System.Windows.Forms.TextBox _txtName;
		private System.Windows.Forms.Label _lblRevision;
		private System.Windows.Forms.CheckBox _chkCreateReflog;
		private System.Windows.Forms.Panel _pnlOptions;
		private gitter.Framework.Controls.GroupSeparator _grpOptions;
		private gitter.Git.Gui.Controls.RevisionPicker _txtRevision;
		private System.Windows.Forms.CheckBox _chkOrphan;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton _trackingDoNotTrack;
		private System.Windows.Forms.RadioButton _trackingTrack;
		private System.Windows.Forms.RadioButton _trackingDefault;
		private Framework.Controls.GroupSeparator _grpTracking;
	}
}
