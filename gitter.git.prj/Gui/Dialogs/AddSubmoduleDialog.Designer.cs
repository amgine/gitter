namespace gitter.Git.Gui.Dialogs
{
	partial class AddSubmoduleDialog
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
			this._txtRepository = new System.Windows.Forms.TextBox();
			this._txtPath = new System.Windows.Forms.TextBox();
			this._txtBranch = new System.Windows.Forms.TextBox();
			this._lblPath = new System.Windows.Forms.Label();
			this._lblUrl = new System.Windows.Forms.Label();
			this._chkBranch = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _txtRepository
			// 
			this._txtRepository.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtRepository.Location = new System.Drawing.Point(100, 29);
			this._txtRepository.Name = "_txtRepository";
			this._txtRepository.Size = new System.Drawing.Size(311, 23);
			this._txtRepository.TabIndex = 7;
			// 
			// _txtPath
			// 
			this._txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtPath.Location = new System.Drawing.Point(100, 3);
			this._txtPath.Name = "_txtPath";
			this._txtPath.Size = new System.Drawing.Size(311, 23);
			this._txtPath.TabIndex = 7;
			// 
			// _txtBranch
			// 
			this._txtBranch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtBranch.Enabled = false;
			this._txtBranch.Location = new System.Drawing.Point(100, 55);
			this._txtBranch.Name = "_txtBranch";
			this._txtBranch.Size = new System.Drawing.Size(311, 23);
			this._txtBranch.TabIndex = 7;
			// 
			// _lblPath
			// 
			this._lblPath.AutoSize = true;
			this._lblPath.Location = new System.Drawing.Point(0, 6);
			this._lblPath.Name = "_lblPath";
			this._lblPath.Size = new System.Drawing.Size(54, 15);
			this._lblPath.TabIndex = 8;
			this._lblPath.Text = "%Path%:";
			// 
			// _lblUrl
			// 
			this._lblUrl.AutoSize = true;
			this._lblUrl.Location = new System.Drawing.Point(0, 32);
			this._lblUrl.Name = "_lblUrl";
			this._lblUrl.Size = new System.Drawing.Size(51, 15);
			this._lblUrl.TabIndex = 9;
			this._lblUrl.Text = "%URL%:";
			// 
			// _chkBranch
			// 
			this._chkBranch.AutoSize = true;
			this._chkBranch.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkBranch.Location = new System.Drawing.Point(3, 57);
			this._chkBranch.Name = "_chkBranch";
			this._chkBranch.Size = new System.Drawing.Size(92, 20);
			this._chkBranch.TabIndex = 10;
			this._chkBranch.Text = "%Branch%:";
			this._chkBranch.UseVisualStyleBackColor = true;
			this._chkBranch.CheckedChanged += new System.EventHandler(this._chkBranch_CheckedChanged);
			// 
			// AddSubmoduleDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._chkBranch);
			this.Controls.Add(this._lblUrl);
			this.Controls.Add(this._lblPath);
			this.Controls.Add(this._txtPath);
			this.Controls.Add(this._txtBranch);
			this.Controls.Add(this._txtRepository);
			this.Name = "AddSubmoduleDialog";
			this.Size = new System.Drawing.Size(414, 82);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _txtRepository;
		private System.Windows.Forms.TextBox _txtPath;
		private System.Windows.Forms.TextBox _txtBranch;
		private System.Windows.Forms.Label _lblPath;
		private System.Windows.Forms.Label _lblUrl;
		private System.Windows.Forms.CheckBox _chkBranch;
	}
}
