namespace gitter.TeamCity.Gui
{
	partial class ProviderSetupControl
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
			this._lblServiceUri = new System.Windows.Forms.Label();
			this._txtServiceUri = new System.Windows.Forms.TextBox();
			this._lblUsername = new System.Windows.Forms.Label();
			this._lblProject = new System.Windows.Forms.Label();
			this._cmbProject = new System.Windows.Forms.ComboBox();
			this._txtUsername = new System.Windows.Forms.TextBox();
			this._txtPassword = new System.Windows.Forms.TextBox();
			this._lblPassword = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _lblServiceUri
			// 
			this._lblServiceUri.AutoSize = true;
			this._lblServiceUri.Location = new System.Drawing.Point(0, 6);
			this._lblServiceUri.Name = "_lblServiceUri";
			this._lblServiceUri.Size = new System.Drawing.Size(71, 15);
			this._lblServiceUri.TabIndex = 0;
			this._lblServiceUri.Text = "Service URL:";
			// 
			// _txtServiceUri
			// 
			this._txtServiceUri.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtServiceUri.Location = new System.Drawing.Point(94, 3);
			this._txtServiceUri.MinimumSize = new System.Drawing.Size(4, 23);
			this._txtServiceUri.Name = "_txtServiceUri";
			this._txtServiceUri.Size = new System.Drawing.Size(303, 23);
			this._txtServiceUri.TabIndex = 0;
			// 
			// _lblUsername
			// 
			this._lblUsername.AutoSize = true;
			this._lblUsername.Location = new System.Drawing.Point(0, 35);
			this._lblUsername.Name = "_lblUsername";
			this._lblUsername.Size = new System.Drawing.Size(63, 15);
			this._lblUsername.TabIndex = 2;
			this._lblUsername.Text = "Username:";
			// 
			// _lblProject
			// 
			this._lblProject.AutoSize = true;
			this._lblProject.Location = new System.Drawing.Point(0, 93);
			this._lblProject.Name = "_lblProject";
			this._lblProject.Size = new System.Drawing.Size(47, 15);
			this._lblProject.TabIndex = 3;
			this._lblProject.Text = "Project:";
			// 
			// _cmbProject
			// 
			this._cmbProject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._cmbProject.FormattingEnabled = true;
			this._cmbProject.Location = new System.Drawing.Point(94, 90);
			this._cmbProject.Name = "_cmbProject";
			this._cmbProject.Size = new System.Drawing.Size(303, 23);
			this._cmbProject.TabIndex = 3;
			// 
			// _txtUsername
			// 
			this._txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtUsername.Location = new System.Drawing.Point(94, 32);
			this._txtUsername.MinimumSize = new System.Drawing.Size(4, 23);
			this._txtUsername.Name = "_txtUsername";
			this._txtUsername.Size = new System.Drawing.Size(303, 23);
			this._txtUsername.TabIndex = 1;
			// 
			// _txtPassword
			// 
			this._txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtPassword.Location = new System.Drawing.Point(94, 61);
			this._txtPassword.MinimumSize = new System.Drawing.Size(4, 23);
			this._txtPassword.Name = "_txtPassword";
			this._txtPassword.Size = new System.Drawing.Size(303, 23);
			this._txtPassword.TabIndex = 2;
			this._txtPassword.UseSystemPasswordChar = true;
			// 
			// _lblPassword
			// 
			this._lblPassword.AutoSize = true;
			this._lblPassword.Location = new System.Drawing.Point(0, 64);
			this._lblPassword.Name = "_lblPassword";
			this._lblPassword.Size = new System.Drawing.Size(60, 15);
			this._lblPassword.TabIndex = 2;
			this._lblPassword.Text = "Password:";
			// 
			// ProviderSetupControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this._cmbProject);
			this.Controls.Add(this._lblProject);
			this.Controls.Add(this._lblPassword);
			this.Controls.Add(this._lblUsername);
			this.Controls.Add(this._txtPassword);
			this.Controls.Add(this._txtUsername);
			this.Controls.Add(this._txtServiceUri);
			this.Controls.Add(this._lblServiceUri);
			this.Name = "ProviderSetupControl";
			this.Size = new System.Drawing.Size(400, 117);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _lblServiceUri;
		private System.Windows.Forms.TextBox _txtServiceUri;
		private System.Windows.Forms.Label _lblUsername;
		private System.Windows.Forms.Label _lblProject;
		private System.Windows.Forms.ComboBox _cmbProject;
		private System.Windows.Forms.TextBox _txtUsername;
		private System.Windows.Forms.TextBox _txtPassword;
		private System.Windows.Forms.Label _lblPassword;
	}
}
