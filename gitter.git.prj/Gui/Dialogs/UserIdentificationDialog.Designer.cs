namespace gitter.Git.Gui.Dialogs
{
	partial class UserIdentificationDialog
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
			this._txtUsername = new System.Windows.Forms.TextBox();
			this._txtEmail = new System.Windows.Forms.TextBox();
			this._lblUser = new System.Windows.Forms.Label();
			this._lblEmail = new System.Windows.Forms.Label();
			this._radSetUserGlobally = new System.Windows.Forms.RadioButton();
			this._radSetUserForRepositoryOnly = new System.Windows.Forms.RadioButton();
			this._lblUseThisUserNameAndEmail = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _txtUsername
			// 
			this._txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._txtUsername.Location = new System.Drawing.Point(68, 3);
			this._txtUsername.Name = "_txtUsername";
			this._txtUsername.Size = new System.Drawing.Size(258, 23);
			this._txtUsername.TabIndex = 0;
			// 
			// _txtEmail
			// 
			this._txtEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._txtEmail.Location = new System.Drawing.Point(68, 32);
			this._txtEmail.Name = "_txtEmail";
			this._txtEmail.Size = new System.Drawing.Size(258, 23);
			this._txtEmail.TabIndex = 1;
			// 
			// _lblUser
			// 
			this._lblUser.AutoSize = true;
			this._lblUser.Location = new System.Drawing.Point(0, 6);
			this._lblUser.Name = "_lblUser";
			this._lblUser.Size = new System.Drawing.Size(62, 15);
			this._lblUser.TabIndex = 2;
			this._lblUser.Text = "%Name%:";
			// 
			// _lblEmail
			// 
			this._lblEmail.AutoSize = true;
			this._lblEmail.Location = new System.Drawing.Point(0, 35);
			this._lblEmail.Name = "_lblEmail";
			this._lblEmail.Size = new System.Drawing.Size(59, 15);
			this._lblEmail.TabIndex = 3;
			this._lblEmail.Text = "%EMail%:";
			// 
			// _radSetUserGlobally
			// 
			this._radSetUserGlobally.AutoSize = true;
			this._radSetUserGlobally.Checked = true;
			this._radSetUserGlobally.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radSetUserGlobally.Location = new System.Drawing.Point(13, 83);
			this._radSetUserGlobally.Name = "_radSetUserGlobally";
			this._radSetUserGlobally.Size = new System.Drawing.Size(186, 20);
			this._radSetUserGlobally.TabIndex = 4;
			this._radSetUserGlobally.TabStop = true;
			this._radSetUserGlobally.Text = "%For current Windows user%";
			this._radSetUserGlobally.UseVisualStyleBackColor = true;
			// 
			// _radSetUserForRepositoryOnly
			// 
			this._radSetUserForRepositoryOnly.AutoSize = true;
			this._radSetUserForRepositoryOnly.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radSetUserForRepositoryOnly.Location = new System.Drawing.Point(13, 103);
			this._radSetUserForRepositoryOnly.Name = "_radSetUserForRepositoryOnly";
			this._radSetUserForRepositoryOnly.Size = new System.Drawing.Size(191, 20);
			this._radSetUserForRepositoryOnly.TabIndex = 5;
			this._radSetUserForRepositoryOnly.Text = "%For current repository only%";
			this._radSetUserForRepositoryOnly.UseVisualStyleBackColor = true;
			// 
			// _lblUseThisUserNameAndEmail
			// 
			this._lblUseThisUserNameAndEmail.AutoSize = true;
			this._lblUseThisUserNameAndEmail.Location = new System.Drawing.Point(0, 65);
			this._lblUseThisUserNameAndEmail.Name = "_lblUseThisUserNameAndEmail";
			this._lblUseThisUserNameAndEmail.Size = new System.Drawing.Size(184, 15);
			this._lblUseThisUserNameAndEmail.TabIndex = 6;
			this._lblUseThisUserNameAndEmail.Text = "%Use this user name and email%:";
			// 
			// UserIdentificationDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lblUseThisUserNameAndEmail);
			this.Controls.Add(this._radSetUserForRepositoryOnly);
			this.Controls.Add(this._radSetUserGlobally);
			this.Controls.Add(this._lblEmail);
			this.Controls.Add(this._lblUser);
			this.Controls.Add(this._txtEmail);
			this.Controls.Add(this._txtUsername);
			this.Name = "UserIdentificationDialog";
			this.Size = new System.Drawing.Size(329, 128);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _txtUsername;
		private System.Windows.Forms.TextBox _txtEmail;
		private System.Windows.Forms.Label _lblUser;
		private System.Windows.Forms.Label _lblEmail;
		private System.Windows.Forms.RadioButton _radSetUserGlobally;
		private System.Windows.Forms.RadioButton _radSetUserForRepositoryOnly;
		private System.Windows.Forms.Label _lblUseThisUserNameAndEmail;
	}
}
