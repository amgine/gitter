namespace gitter.Git.Gui.Dialogs
{
	partial class InitDialog
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
			this._btnSelectTemplate = new System.Windows.Forms.Button();
			this._grpOptions = new gitter.Framework.Controls.GroupSeparator();
			this._chkBare = new System.Windows.Forms.CheckBox();
			this._chkUseTemplate = new System.Windows.Forms.CheckBox();
			this._txtTemplate = new System.Windows.Forms.TextBox();
			this._btnSelectDirectory = new System.Windows.Forms.Button();
			this._lblPath = new System.Windows.Forms.Label();
			this._txtPath = new System.Windows.Forms.TextBox();
			this._pnlOptions.SuspendLayout();
			this.SuspendLayout();
			// 
			// _pnlOptions
			// 
			this._pnlOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._pnlOptions.Controls.Add(this._btnSelectTemplate);
			this._pnlOptions.Controls.Add(this._grpOptions);
			this._pnlOptions.Controls.Add(this._chkBare);
			this._pnlOptions.Controls.Add(this._txtTemplate);
			this._pnlOptions.Controls.Add(this._chkUseTemplate);
			this._pnlOptions.Location = new System.Drawing.Point(0, 29);
			this._pnlOptions.Name = "_pnlOptions";
			this._pnlOptions.Size = new System.Drawing.Size(382, 69);
			this._pnlOptions.TabIndex = 14;
			// 
			// _btnSelectTemplate
			// 
			this._btnSelectTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnSelectTemplate.Enabled = false;
			this._btnSelectTemplate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnSelectTemplate.Location = new System.Drawing.Point(353, 23);
			this._btnSelectTemplate.Name = "_btnSelectTemplate";
			this._btnSelectTemplate.Size = new System.Drawing.Size(29, 23);
			this._btnSelectTemplate.TabIndex = 4;
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
			this._grpOptions.Size = new System.Drawing.Size(382, 19);
			this._grpOptions.TabIndex = 0;
			this._grpOptions.Text = "%Options%";
			// 
			// _chkBare
			// 
			this._chkBare.AutoSize = true;
			this._chkBare.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkBare.Location = new System.Drawing.Point(12, 45);
			this._chkBare.Name = "_chkBare";
			this._chkBare.Size = new System.Drawing.Size(131, 20);
			this._chkBare.TabIndex = 5;
			this._chkBare.Text = "%Bare repository%";
			this._chkBare.UseVisualStyleBackColor = true;
			// 
			// _chkUseTemplate
			// 
			this._chkUseTemplate.AutoSize = true;
			this._chkUseTemplate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkUseTemplate.Location = new System.Drawing.Point(12, 25);
			this._chkUseTemplate.Name = "_chkUseTemplate";
			this._chkUseTemplate.Size = new System.Drawing.Size(105, 20);
			this._chkUseTemplate.TabIndex = 2;
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
			this._txtTemplate.Location = new System.Drawing.Point(94, 23);
			this._txtTemplate.Name = "_txtTemplate";
			this._txtTemplate.Size = new System.Drawing.Size(259, 23);
			this._txtTemplate.TabIndex = 3;
			// 
			// _btnSelectDirectory
			// 
			this._btnSelectDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnSelectDirectory.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnSelectDirectory.Location = new System.Drawing.Point(353, 3);
			this._btnSelectDirectory.Name = "_btnSelectDirectory";
			this._btnSelectDirectory.Size = new System.Drawing.Size(29, 23);
			this._btnSelectDirectory.TabIndex = 1;
			this._btnSelectDirectory.Text = "...";
			this._btnSelectDirectory.UseVisualStyleBackColor = true;
			this._btnSelectDirectory.Click += new System.EventHandler(this._btnSelectDirectory_Click);
			// 
			// _lblPath
			// 
			this._lblPath.AutoSize = true;
			this._lblPath.Location = new System.Drawing.Point(0, 6);
			this._lblPath.Name = "_lblPath";
			this._lblPath.Size = new System.Drawing.Size(54, 15);
			this._lblPath.TabIndex = 10;
			this._lblPath.Text = "%Path%:";
			// 
			// _txtPath
			// 
			this._txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this._txtPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
			this._txtPath.Location = new System.Drawing.Point(94, 3);
			this._txtPath.Name = "_txtPath";
			this._txtPath.Size = new System.Drawing.Size(259, 23);
			this._txtPath.TabIndex = 0;
			// 
			// InitDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._pnlOptions);
			this.Controls.Add(this._btnSelectDirectory);
			this.Controls.Add(this._lblPath);
			this.Controls.Add(this._txtPath);
			this.Name = "InitDialog";
			this.Size = new System.Drawing.Size(385, 98);
			this._pnlOptions.ResumeLayout(false);
			this._pnlOptions.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _lblPath;
		private System.Windows.Forms.TextBox _txtTemplate;
		private System.Windows.Forms.TextBox _txtPath;
		private System.Windows.Forms.CheckBox _chkUseTemplate;
		private System.Windows.Forms.CheckBox _chkBare;
		private System.Windows.Forms.Button _btnSelectDirectory;
		private System.Windows.Forms.Button _btnSelectTemplate;
		private System.Windows.Forms.Panel _pnlOptions;
		private gitter.Framework.Controls.GroupSeparator _grpOptions;
	}
}
