namespace gitter.Git.Gui.Dialogs
{
	partial class CloneDialog
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
			this._lblRealPath = new System.Windows.Forms.Label();
			this._lblWillBeClonedInto = new System.Windows.Forms.Label();
			this._chkAppendRepositoryNameFromUrl = new System.Windows.Forms.CheckBox();
			this._pnlOptions = new System.Windows.Forms.Panel();
			this._lblDepth = new System.Windows.Forms.Label();
			this._chkNoCheckout = new System.Windows.Forms.CheckBox();
			this._chkRecursive = new System.Windows.Forms.CheckBox();
			this._numDepth = new System.Windows.Forms.NumericUpDown();
			this._chkShallowClone = new System.Windows.Forms.CheckBox();
			this._txtRemoteName = new System.Windows.Forms.TextBox();
			this._lblRemoteName = new System.Windows.Forms.Label();
			this._chkMirror = new System.Windows.Forms.CheckBox();
			this._btnSelectTemplate = new System.Windows.Forms.Button();
			this._grpOptions = new gitter.Framework.Controls.GroupSeparator();
			this._chkBare = new System.Windows.Forms.CheckBox();
			this._chkUseTemplate = new System.Windows.Forms.CheckBox();
			this._txtTemplate = new System.Windows.Forms.TextBox();
			this._lblUrl = new System.Windows.Forms.Label();
			this._txtUrl = new System.Windows.Forms.TextBox();
			this._btnSelectDirectory = new System.Windows.Forms.Button();
			this._lblPath = new System.Windows.Forms.Label();
			this._txtPath = new System.Windows.Forms.TextBox();
			this._pnlOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._numDepth)).BeginInit();
			this.SuspendLayout();
			// 
			// _lblRealPath
			// 
			this._lblRealPath.AutoSize = true;
			this._lblRealPath.Location = new System.Drawing.Point(0, 98);
			this._lblRealPath.Name = "_lblRealPath";
			this._lblRealPath.Size = new System.Drawing.Size(0, 15);
			this._lblRealPath.TabIndex = 22;
			// 
			// _lblWillBeClonedInto
			// 
			this._lblWillBeClonedInto.AutoSize = true;
			this._lblWillBeClonedInto.Location = new System.Drawing.Point(0, 83);
			this._lblWillBeClonedInto.Name = "_lblWillBeClonedInto";
			this._lblWillBeClonedInto.Size = new System.Drawing.Size(129, 15);
			this._lblWillBeClonedInto.TabIndex = 21;
			this._lblWillBeClonedInto.Text = "%Will be cloned into%:";
			// 
			// _chkAppendRepositoryNameFromUrl
			// 
			this._chkAppendRepositoryNameFromUrl.AutoSize = true;
			this._chkAppendRepositoryNameFromUrl.Checked = true;
			this._chkAppendRepositoryNameFromUrl.CheckState = System.Windows.Forms.CheckState.Checked;
			this._chkAppendRepositoryNameFromUrl.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkAppendRepositoryNameFromUrl.Location = new System.Drawing.Point(94, 58);
			this._chkAppendRepositoryNameFromUrl.Name = "_chkAppendRepositoryNameFromUrl";
			this._chkAppendRepositoryNameFromUrl.Size = new System.Drawing.Size(277, 20);
			this._chkAppendRepositoryNameFromUrl.TabIndex = 3;
			this._chkAppendRepositoryNameFromUrl.Text = "%Append repository name from URL to path%";
			this._chkAppendRepositoryNameFromUrl.UseVisualStyleBackColor = true;
			// 
			// _pnlOptions
			// 
			this._pnlOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._pnlOptions.Controls.Add(this._lblDepth);
			this._pnlOptions.Controls.Add(this._chkNoCheckout);
			this._pnlOptions.Controls.Add(this._chkRecursive);
			this._pnlOptions.Controls.Add(this._numDepth);
			this._pnlOptions.Controls.Add(this._chkShallowClone);
			this._pnlOptions.Controls.Add(this._txtRemoteName);
			this._pnlOptions.Controls.Add(this._lblRemoteName);
			this._pnlOptions.Controls.Add(this._chkMirror);
			this._pnlOptions.Controls.Add(this._btnSelectTemplate);
			this._pnlOptions.Controls.Add(this._grpOptions);
			this._pnlOptions.Controls.Add(this._chkBare);
			this._pnlOptions.Controls.Add(this._chkUseTemplate);
			this._pnlOptions.Controls.Add(this._txtTemplate);
			this._pnlOptions.Location = new System.Drawing.Point(0, 115);
			this._pnlOptions.Name = "_pnlOptions";
			this._pnlOptions.Size = new System.Drawing.Size(397, 181);
			this._pnlOptions.TabIndex = 19;
			// 
			// _lblDepth
			// 
			this._lblDepth.AutoSize = true;
			this._lblDepth.Enabled = false;
			this._lblDepth.Location = new System.Drawing.Point(114, 108);
			this._lblDepth.Name = "_lblDepth";
			this._lblDepth.Size = new System.Drawing.Size(62, 15);
			this._lblDepth.TabIndex = 9;
			this._lblDepth.Text = "%Depth%:";
			// 
			// _chkNoCheckout
			// 
			this._chkNoCheckout.AutoSize = true;
			this._chkNoCheckout.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkNoCheckout.Location = new System.Drawing.Point(12, 156);
			this._chkNoCheckout.Name = "_chkNoCheckout";
			this._chkNoCheckout.Size = new System.Drawing.Size(122, 20);
			this._chkNoCheckout.TabIndex = 13;
			this._chkNoCheckout.Text = "%No Checkout%";
			this._chkNoCheckout.UseVisualStyleBackColor = true;
			// 
			// _chkRecursive
			// 
			this._chkRecursive.AutoSize = true;
			this._chkRecursive.Checked = true;
			this._chkRecursive.CheckState = System.Windows.Forms.CheckState.Checked;
			this._chkRecursive.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkRecursive.Location = new System.Drawing.Point(12, 131);
			this._chkRecursive.Name = "_chkRecursive";
			this._chkRecursive.Size = new System.Drawing.Size(102, 20);
			this._chkRecursive.TabIndex = 12;
			this._chkRecursive.Text = "%Recursive%";
			this._chkRecursive.UseVisualStyleBackColor = true;
			// 
			// _numDepth
			// 
			this._numDepth.Enabled = false;
			this._numDepth.Location = new System.Drawing.Point(182, 106);
			this._numDepth.Maximum = new decimal(new int[] {
            -727379969,
            232,
            0,
            0});
			this._numDepth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._numDepth.Name = "_numDepth";
			this._numDepth.Size = new System.Drawing.Size(71, 23);
			this._numDepth.TabIndex = 11;
			this._numDepth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._numDepth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _chkShallowClone
			// 
			this._chkShallowClone.AutoSize = true;
			this._chkShallowClone.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkShallowClone.Location = new System.Drawing.Point(12, 106);
			this._chkShallowClone.Name = "_chkShallowClone";
			this._chkShallowClone.Size = new System.Drawing.Size(127, 20);
			this._chkShallowClone.TabIndex = 10;
			this._chkShallowClone.Text = "%Shallow Clone%";
			this._chkShallowClone.UseVisualStyleBackColor = true;
			this._chkShallowClone.CheckedChanged += new System.EventHandler(this._chkShallowClone_CheckedChanged);
			// 
			// _txtRemoteName
			// 
			this._txtRemoteName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtRemoteName.Location = new System.Drawing.Point(117, 25);
			this._txtRemoteName.Name = "_txtRemoteName";
			this._txtRemoteName.Size = new System.Drawing.Size(280, 23);
			this._txtRemoteName.TabIndex = 4;
			// 
			// _lblRemoteName
			// 
			this._lblRemoteName.AutoSize = true;
			this._lblRemoteName.Location = new System.Drawing.Point(9, 28);
			this._lblRemoteName.Name = "_lblRemoteName";
			this._lblRemoteName.Size = new System.Drawing.Size(104, 15);
			this._lblRemoteName.TabIndex = 7;
			this._lblRemoteName.Text = "%Remote name%:";
			// 
			// _chkMirror
			// 
			this._chkMirror.AutoSize = true;
			this._chkMirror.Enabled = false;
			this._chkMirror.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkMirror.Location = new System.Drawing.Point(117, 81);
			this._chkMirror.Name = "_chkMirror";
			this._chkMirror.Size = new System.Drawing.Size(85, 20);
			this._chkMirror.TabIndex = 9;
			this._chkMirror.Text = "%Mirror%";
			this._chkMirror.UseVisualStyleBackColor = true;
			// 
			// _btnSelectTemplate
			// 
			this._btnSelectTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnSelectTemplate.Enabled = false;
			this._btnSelectTemplate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnSelectTemplate.Location = new System.Drawing.Point(368, 54);
			this._btnSelectTemplate.Name = "_btnSelectTemplate";
			this._btnSelectTemplate.Size = new System.Drawing.Size(29, 23);
			this._btnSelectTemplate.TabIndex = 7;
			this._btnSelectTemplate.Text = "...";
			this._btnSelectTemplate.UseVisualStyleBackColor = true;
			this._btnSelectTemplate.Click += new System.EventHandler(this._btnSelectTemplate_Click);
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
			// _chkBare
			// 
			this._chkBare.AutoSize = true;
			this._chkBare.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkBare.Location = new System.Drawing.Point(12, 81);
			this._chkBare.Name = "_chkBare";
			this._chkBare.Size = new System.Drawing.Size(131, 20);
			this._chkBare.TabIndex = 8;
			this._chkBare.Text = "%Bare repository%";
			this._chkBare.UseVisualStyleBackColor = true;
			this._chkBare.CheckedChanged += new System.EventHandler(this._chkBare_CheckedChanged);
			// 
			// _chkUseTemplate
			// 
			this._chkUseTemplate.AutoSize = true;
			this._chkUseTemplate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkUseTemplate.Location = new System.Drawing.Point(12, 56);
			this._chkUseTemplate.Name = "_chkUseTemplate";
			this._chkUseTemplate.Size = new System.Drawing.Size(105, 20);
			this._chkUseTemplate.TabIndex = 5;
			this._chkUseTemplate.Text = "%Template%:";
			this._chkUseTemplate.UseVisualStyleBackColor = true;
			this._chkUseTemplate.CheckedChanged += new System.EventHandler(this._chkUseTemplate_CheckedChanged);
			// 
			// _txtTemplate
			// 
			this._txtTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtTemplate.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this._txtTemplate.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
			this._txtTemplate.Enabled = false;
			this._txtTemplate.Location = new System.Drawing.Point(117, 54);
			this._txtTemplate.Name = "_txtTemplate";
			this._txtTemplate.Size = new System.Drawing.Size(251, 23);
			this._txtTemplate.TabIndex = 6;
			// 
			// _lblUrl
			// 
			this._lblUrl.AutoSize = true;
			this._lblUrl.Location = new System.Drawing.Point(0, 6);
			this._lblUrl.Name = "_lblUrl";
			this._lblUrl.Size = new System.Drawing.Size(51, 15);
			this._lblUrl.TabIndex = 18;
			this._lblUrl.Text = "%URL%:";
			// 
			// _txtUrl
			// 
			this._txtUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtUrl.Location = new System.Drawing.Point(94, 3);
			this._txtUrl.Name = "_txtUrl";
			this._txtUrl.Size = new System.Drawing.Size(303, 23);
			this._txtUrl.TabIndex = 0;
			// 
			// _btnSelectDirectory
			// 
			this._btnSelectDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnSelectDirectory.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnSelectDirectory.Location = new System.Drawing.Point(368, 32);
			this._btnSelectDirectory.Name = "_btnSelectDirectory";
			this._btnSelectDirectory.Size = new System.Drawing.Size(29, 23);
			this._btnSelectDirectory.TabIndex = 2;
			this._btnSelectDirectory.Text = "...";
			this._btnSelectDirectory.UseVisualStyleBackColor = true;
			this._btnSelectDirectory.Click += new System.EventHandler(this._btnSelectDirectory_Click);
			// 
			// _lblPath
			// 
			this._lblPath.AutoSize = true;
			this._lblPath.Location = new System.Drawing.Point(0, 35);
			this._lblPath.Name = "_lblPath";
			this._lblPath.Size = new System.Drawing.Size(54, 15);
			this._lblPath.TabIndex = 15;
			this._lblPath.Text = "%Path%:";
			// 
			// _txtPath
			// 
			this._txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this._txtPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
			this._txtPath.Location = new System.Drawing.Point(94, 32);
			this._txtPath.Name = "_txtPath";
			this._txtPath.Size = new System.Drawing.Size(274, 23);
			this._txtPath.TabIndex = 1;
			// 
			// CloneDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lblRealPath);
			this.Controls.Add(this._lblWillBeClonedInto);
			this.Controls.Add(this._chkAppendRepositoryNameFromUrl);
			this.Controls.Add(this._pnlOptions);
			this.Controls.Add(this._lblUrl);
			this.Controls.Add(this._txtUrl);
			this.Controls.Add(this._btnSelectDirectory);
			this.Controls.Add(this._lblPath);
			this.Controls.Add(this._txtPath);
			this.Name = "CloneDialog";
			this.Size = new System.Drawing.Size(400, 297);
			this._pnlOptions.ResumeLayout(false);
			this._pnlOptions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._numDepth)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _btnSelectDirectory;
		private System.Windows.Forms.Label _lblPath;
		private System.Windows.Forms.TextBox _txtPath;
		private System.Windows.Forms.Label _lblUrl;
		private System.Windows.Forms.TextBox _txtUrl;
		private System.Windows.Forms.Panel _pnlOptions;
		private System.Windows.Forms.Button _btnSelectTemplate;
		private gitter.Framework.Controls.GroupSeparator _grpOptions;
		private System.Windows.Forms.CheckBox _chkBare;
		private System.Windows.Forms.CheckBox _chkUseTemplate;
		private System.Windows.Forms.TextBox _txtTemplate;
		private System.Windows.Forms.CheckBox _chkMirror;
		private System.Windows.Forms.TextBox _txtRemoteName;
		private System.Windows.Forms.Label _lblRemoteName;
		private System.Windows.Forms.CheckBox _chkShallowClone;
		private System.Windows.Forms.Label _lblDepth;
		private System.Windows.Forms.NumericUpDown _numDepth;
		private System.Windows.Forms.CheckBox _chkRecursive;
		private System.Windows.Forms.CheckBox _chkNoCheckout;
		private System.Windows.Forms.CheckBox _chkAppendRepositoryNameFromUrl;
		private System.Windows.Forms.Label _lblWillBeClonedInto;
		private System.Windows.Forms.Label _lblRealPath;
	}
}
