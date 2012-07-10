namespace gitter.Git.Gui.Dialogs
{
	partial class AddRemoteDialog
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
			this._lblUrl = new System.Windows.Forms.Label();
			this._lblName = new System.Windows.Forms.Label();
			this._txtUrl = new System.Windows.Forms.TextBox();
			this._txtName = new System.Windows.Forms.TextBox();
			this._chkFetch = new System.Windows.Forms.CheckBox();
			this._pnlOptions = new System.Windows.Forms.Panel();
			this._chkMirror = new System.Windows.Forms.CheckBox();
			this._grpOptions = new gitter.Framework.Controls.GroupSeparator();
			this.panel1 = new System.Windows.Forms.Panel();
			this._tagFetchAll = new System.Windows.Forms.RadioButton();
			this._tagFetchNone = new System.Windows.Forms.RadioButton();
			this._tagFetchDefault = new System.Windows.Forms.RadioButton();
			this._grpTagImport = new gitter.Framework.Controls.GroupSeparator();
			this.panel2 = new System.Windows.Forms.Panel();
			this._trackSpecified = new System.Windows.Forms.RadioButton();
			this._trackAllBranches = new System.Windows.Forms.RadioButton();
			this._grpBranches = new gitter.Framework.Controls.GroupSeparator();
			this._pnlOptions.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// _lblUrl
			// 
			this._lblUrl.AutoSize = true;
			this._lblUrl.Location = new System.Drawing.Point(0, 32);
			this._lblUrl.Name = "_lblUrl";
			this._lblUrl.Size = new System.Drawing.Size(51, 15);
			this._lblUrl.TabIndex = 7;
			this._lblUrl.Text = "%URL%:";
			// 
			// _lblName
			// 
			this._lblName.AutoSize = true;
			this._lblName.Location = new System.Drawing.Point(0, 6);
			this._lblName.Name = "_lblName";
			this._lblName.Size = new System.Drawing.Size(62, 15);
			this._lblName.TabIndex = 6;
			this._lblName.Text = "%Name%:";
			// 
			// _txtUrl
			// 
			this._txtUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._txtUrl.Location = new System.Drawing.Point(94, 29);
			this._txtUrl.Name = "_txtUrl";
			this._txtUrl.Size = new System.Drawing.Size(288, 23);
			this._txtUrl.TabIndex = 1;
			// 
			// _txtName
			// 
			this._txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._txtName.Location = new System.Drawing.Point(94, 3);
			this._txtName.Name = "_txtName";
			this._txtName.Size = new System.Drawing.Size(288, 23);
			this._txtName.TabIndex = 0;
			// 
			// _chkFetch
			// 
			this._chkFetch.AutoSize = true;
			this._chkFetch.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkFetch.Location = new System.Drawing.Point(12, 25);
			this._chkFetch.Name = "_chkFetch";
			this._chkFetch.Size = new System.Drawing.Size(125, 20);
			this._chkFetch.TabIndex = 2;
			this._chkFetch.Text = "%Fetch Remote%";
			this._chkFetch.UseVisualStyleBackColor = true;
			// 
			// _pnlOptions
			// 
			this._pnlOptions.Controls.Add(this._chkMirror);
			this._pnlOptions.Controls.Add(this._grpOptions);
			this._pnlOptions.Controls.Add(this._chkFetch);
			this._pnlOptions.Location = new System.Drawing.Point(0, 58);
			this._pnlOptions.Margin = new System.Windows.Forms.Padding(0);
			this._pnlOptions.Name = "_pnlOptions";
			this._pnlOptions.Size = new System.Drawing.Size(382, 49);
			this._pnlOptions.TabIndex = 10;
			// 
			// _chkMirror
			// 
			this._chkMirror.AutoSize = true;
			this._chkMirror.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkMirror.Location = new System.Drawing.Point(177, 25);
			this._chkMirror.Name = "_chkMirror";
			this._chkMirror.Size = new System.Drawing.Size(85, 20);
			this._chkMirror.TabIndex = 3;
			this._chkMirror.Text = "%Mirror%";
			this._chkMirror.UseVisualStyleBackColor = true;
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
			// panel1
			// 
			this.panel1.Controls.Add(this._tagFetchAll);
			this.panel1.Controls.Add(this._tagFetchNone);
			this.panel1.Controls.Add(this._tagFetchDefault);
			this.panel1.Controls.Add(this._grpTagImport);
			this.panel1.Location = new System.Drawing.Point(0, 107);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(382, 48);
			this.panel1.TabIndex = 11;
			// 
			// _tagFetchAll
			// 
			this._tagFetchAll.AutoSize = true;
			this._tagFetchAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._tagFetchAll.Location = new System.Drawing.Point(248, 25);
			this._tagFetchAll.Name = "_tagFetchAll";
			this._tagFetchAll.Size = new System.Drawing.Size(93, 20);
			this._tagFetchAll.TabIndex = 9;
			this._tagFetchAll.Text = "%All Tags%";
			this._tagFetchAll.UseVisualStyleBackColor = true;
			// 
			// _tagFetchNone
			// 
			this._tagFetchNone.AutoSize = true;
			this._tagFetchNone.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._tagFetchNone.Location = new System.Drawing.Point(127, 25);
			this._tagFetchNone.Name = "_tagFetchNone";
			this._tagFetchNone.Size = new System.Drawing.Size(95, 20);
			this._tagFetchNone.TabIndex = 8;
			this._tagFetchNone.Text = "%No Tags%";
			this._tagFetchNone.UseVisualStyleBackColor = true;
			// 
			// _tagFetchDefault
			// 
			this._tagFetchDefault.AutoSize = true;
			this._tagFetchDefault.Checked = true;
			this._tagFetchDefault.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._tagFetchDefault.Location = new System.Drawing.Point(11, 25);
			this._tagFetchDefault.Name = "_tagFetchDefault";
			this._tagFetchDefault.Size = new System.Drawing.Size(89, 20);
			this._tagFetchDefault.TabIndex = 7;
			this._tagFetchDefault.TabStop = true;
			this._tagFetchDefault.Text = "%Default%";
			this._tagFetchDefault.UseVisualStyleBackColor = true;
			// 
			// _grpTagImport
			// 
			this._grpTagImport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._grpTagImport.Location = new System.Drawing.Point(0, 0);
			this._grpTagImport.Name = "_grpTagImport";
			this._grpTagImport.Size = new System.Drawing.Size(382, 19);
			this._grpTagImport.TabIndex = 0;
			this._grpTagImport.Text = "%Tag Import Mode%";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this._trackSpecified);
			this.panel2.Controls.Add(this._trackAllBranches);
			this.panel2.Controls.Add(this._grpBranches);
			this.panel2.Location = new System.Drawing.Point(294, 196);
			this.panel2.Margin = new System.Windows.Forms.Padding(0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(382, 207);
			this.panel2.TabIndex = 12;
			this.panel2.Visible = false;
			// 
			// _trackSpecified
			// 
			this._trackSpecified.AutoSize = true;
			this._trackSpecified.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._trackSpecified.Location = new System.Drawing.Point(11, 45);
			this._trackSpecified.Name = "_trackSpecified";
			this._trackSpecified.Size = new System.Drawing.Size(134, 20);
			this._trackSpecified.TabIndex = 5;
			this._trackSpecified.Text = "%Track Specified%:";
			this._trackSpecified.UseVisualStyleBackColor = true;
			// 
			// _trackAllBranches
			// 
			this._trackAllBranches.AutoSize = true;
			this._trackAllBranches.Checked = true;
			this._trackAllBranches.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._trackAllBranches.Location = new System.Drawing.Point(11, 25);
			this._trackAllBranches.Name = "_trackAllBranches";
			this._trackAllBranches.Size = new System.Drawing.Size(97, 20);
			this._trackAllBranches.TabIndex = 4;
			this._trackAllBranches.TabStop = true;
			this._trackAllBranches.Text = "%Track All%";
			this._trackAllBranches.UseVisualStyleBackColor = true;
			// 
			// _grpBranches
			// 
			this._grpBranches.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._grpBranches.Location = new System.Drawing.Point(0, 0);
			this._grpBranches.Name = "_grpBranches";
			this._grpBranches.Size = new System.Drawing.Size(382, 19);
			this._grpBranches.TabIndex = 0;
			this._grpBranches.Text = "%Tracking Branches%";
			// 
			// AddRemoteDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this._pnlOptions);
			this.Controls.Add(this._lblUrl);
			this.Controls.Add(this._lblName);
			this.Controls.Add(this._txtUrl);
			this.Controls.Add(this._txtName);
			this.Name = "AddRemoteDialog";
			this.Size = new System.Drawing.Size(385, 156);
			this._pnlOptions.ResumeLayout(false);
			this._pnlOptions.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _lblUrl;
		private System.Windows.Forms.Label _lblName;
		private System.Windows.Forms.TextBox _txtUrl;
		private System.Windows.Forms.TextBox _txtName;
		private System.Windows.Forms.CheckBox _chkFetch;
		private System.Windows.Forms.Panel _pnlOptions;
		private gitter.Framework.Controls.GroupSeparator _grpOptions;
		private System.Windows.Forms.CheckBox _chkMirror;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton _tagFetchAll;
		private System.Windows.Forms.RadioButton _tagFetchNone;
		private System.Windows.Forms.RadioButton _tagFetchDefault;
		private Framework.Controls.GroupSeparator _grpTagImport;
		private System.Windows.Forms.Panel panel2;
		private Framework.Controls.GroupSeparator _grpBranches;
		private System.Windows.Forms.RadioButton _trackSpecified;
		private System.Windows.Forms.RadioButton _trackAllBranches;
	}
}
