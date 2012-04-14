namespace gitter.Git.Gui.Dialogs
{
	partial class ApplyPatchesDialog
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
			this._grpApplyTo = new gitter.Framework.Controls.GroupSeparator();
			this._grpOptions = new gitter.Framework.Controls.GroupSeparator();
			this._radWorkingDirectory = new System.Windows.Forms.RadioButton();
			this._radIndexAndWorkingDirectory = new System.Windows.Forms.RadioButton();
			this._radIndexOnly = new System.Windows.Forms.RadioButton();
			this._chkReverse = new System.Windows.Forms.CheckBox();
			this._lblPatches = new System.Windows.Forms.Label();
			this._btnAddFiles = new System.Windows.Forms.Button();
			this._btnAddFromClipboard = new System.Windows.Forms.Button();
			this._lstPatches = new gitter.Framework.Controls.CustomListBox();
			this.SuspendLayout();
			// 
			// _grpApplyTo
			// 
			this._grpApplyTo.Location = new System.Drawing.Point(0, 189);
			this._grpApplyTo.Name = "_grpApplyTo";
			this._grpApplyTo.Size = new System.Drawing.Size(397, 19);
			this._grpApplyTo.TabIndex = 1;
			this._grpApplyTo.Text = "%Apply To%";
			// 
			// _grpOptions
			// 
			this._grpOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._grpOptions.Location = new System.Drawing.Point(0, 277);
			this._grpOptions.Name = "_grpOptions";
			this._grpOptions.Size = new System.Drawing.Size(397, 19);
			this._grpOptions.TabIndex = 2;
			this._grpOptions.Text = "%Options%";
			// 
			// _radWorkingDirectory
			// 
			this._radWorkingDirectory.AutoSize = true;
			this._radWorkingDirectory.Checked = true;
			this._radWorkingDirectory.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radWorkingDirectory.Location = new System.Drawing.Point(14, 214);
			this._radWorkingDirectory.Name = "_radWorkingDirectory";
			this._radWorkingDirectory.Size = new System.Drawing.Size(147, 20);
			this._radWorkingDirectory.TabIndex = 3;
			this._radWorkingDirectory.TabStop = true;
			this._radWorkingDirectory.Text = "%Working Directory%";
			this._radWorkingDirectory.UseVisualStyleBackColor = true;
			// 
			// _radIndexAndWorkingDirectory
			// 
			this._radIndexAndWorkingDirectory.AutoSize = true;
			this._radIndexAndWorkingDirectory.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radIndexAndWorkingDirectory.Location = new System.Drawing.Point(14, 254);
			this._radIndexAndWorkingDirectory.Name = "_radIndexAndWorkingDirectory";
			this._radIndexAndWorkingDirectory.Size = new System.Drawing.Size(201, 20);
			this._radIndexAndWorkingDirectory.TabIndex = 4;
			this._radIndexAndWorkingDirectory.Text = "%Index and Working Directory%";
			this._radIndexAndWorkingDirectory.UseVisualStyleBackColor = true;
			// 
			// _radIndexOnly
			// 
			this._radIndexOnly.AutoSize = true;
			this._radIndexOnly.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radIndexOnly.Location = new System.Drawing.Point(14, 234);
			this._radIndexOnly.Name = "_radIndexOnly";
			this._radIndexOnly.Size = new System.Drawing.Size(79, 20);
			this._radIndexOnly.TabIndex = 5;
			this._radIndexOnly.Text = "%Index%";
			this._radIndexOnly.UseVisualStyleBackColor = true;
			// 
			// _chkReverse
			// 
			this._chkReverse.AutoSize = true;
			this._chkReverse.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkReverse.Location = new System.Drawing.Point(14, 302);
			this._chkReverse.Name = "_chkReverse";
			this._chkReverse.Size = new System.Drawing.Size(92, 20);
			this._chkReverse.TabIndex = 6;
			this._chkReverse.Text = "%Reverse%";
			this._chkReverse.UseVisualStyleBackColor = true;
			// 
			// _lblPatches
			// 
			this._lblPatches.AutoSize = true;
			this._lblPatches.Location = new System.Drawing.Point(0, 0);
			this._lblPatches.Name = "_lblPatches";
			this._lblPatches.Size = new System.Drawing.Size(71, 15);
			this._lblPatches.TabIndex = 7;
			this._lblPatches.Text = "%Patches%:";
			// 
			// _btnAddFiles
			// 
			this._btnAddFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnAddFiles.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnAddFiles.Location = new System.Drawing.Point(144, 160);
			this._btnAddFiles.Name = "_btnAddFiles";
			this._btnAddFiles.Size = new System.Drawing.Size(93, 23);
			this._btnAddFiles.TabIndex = 8;
			this._btnAddFiles.Text = "%Add Files%...";
			this._btnAddFiles.UseVisualStyleBackColor = true;
			this._btnAddFiles.Click += new System.EventHandler(this.OnAddFilesClick);
			// 
			// _btnAddFromClipboard
			// 
			this._btnAddFromClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnAddFromClipboard.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnAddFromClipboard.Location = new System.Drawing.Point(243, 160);
			this._btnAddFromClipboard.Name = "_btnAddFromClipboard";
			this._btnAddFromClipboard.Size = new System.Drawing.Size(154, 23);
			this._btnAddFromClipboard.TabIndex = 8;
			this._btnAddFromClipboard.Text = "%Add From Clipboard%";
			this._btnAddFromClipboard.UseVisualStyleBackColor = true;
			this._btnAddFromClipboard.Click += new System.EventHandler(this.OnAddFromClipboardClick);
			// 
			// _lstPatches
			// 
			this._lstPatches.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lstPatches.Location = new System.Drawing.Point(3, 18);
			this._lstPatches.Name = "_lstPatches";
			this._lstPatches.Size = new System.Drawing.Size(394, 136);
			this._lstPatches.TabIndex = 9;
			this._lstPatches.Text = "%No patches to apply%";
			// 
			// ApplyPatchesDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lstPatches);
			this.Controls.Add(this._btnAddFromClipboard);
			this.Controls.Add(this._btnAddFiles);
			this.Controls.Add(this._lblPatches);
			this.Controls.Add(this._chkReverse);
			this.Controls.Add(this._radIndexOnly);
			this.Controls.Add(this._radIndexAndWorkingDirectory);
			this.Controls.Add(this._radWorkingDirectory);
			this.Controls.Add(this._grpOptions);
			this.Controls.Add(this._grpApplyTo);
			this.Name = "ApplyPatchesDialog";
			this.Size = new System.Drawing.Size(400, 325);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Framework.Controls.GroupSeparator _grpApplyTo;
		private Framework.Controls.GroupSeparator _grpOptions;
		private System.Windows.Forms.RadioButton _radWorkingDirectory;
		private System.Windows.Forms.RadioButton _radIndexAndWorkingDirectory;
		private System.Windows.Forms.RadioButton _radIndexOnly;
		private System.Windows.Forms.CheckBox _chkReverse;
		private System.Windows.Forms.Label _lblPatches;
		private System.Windows.Forms.Button _btnAddFiles;
		private System.Windows.Forms.Button _btnAddFromClipboard;
		private Framework.Controls.CustomListBox _lstPatches;
	}
}
