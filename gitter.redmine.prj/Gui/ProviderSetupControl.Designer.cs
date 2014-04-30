namespace gitter.Redmine.Gui
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
			this._lblApiKey = new System.Windows.Forms.Label();
			this._lblProject = new System.Windows.Forms.Label();
			this._cmbProject = new System.Windows.Forms.ComboBox();
			this._txtApiKey = new System.Windows.Forms.TextBox();
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
			// _lblApiKey
			// 
			this._lblApiKey.AutoSize = true;
			this._lblApiKey.Location = new System.Drawing.Point(0, 35);
			this._lblApiKey.Name = "_lblApiKey";
			this._lblApiKey.Size = new System.Drawing.Size(50, 15);
			this._lblApiKey.TabIndex = 2;
			this._lblApiKey.Text = "API Key:";
			// 
			// _lblProject
			// 
			this._lblProject.AutoSize = true;
			this._lblProject.Location = new System.Drawing.Point(0, 64);
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
			this._cmbProject.Location = new System.Drawing.Point(94, 61);
			this._cmbProject.Name = "_cmbProject";
			this._cmbProject.Size = new System.Drawing.Size(303, 23);
			this._cmbProject.TabIndex = 2;
			// 
			// _txtApiKey
			// 
			this._txtApiKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtApiKey.Location = new System.Drawing.Point(94, 32);
			this._txtApiKey.MinimumSize = new System.Drawing.Size(4, 23);
			this._txtApiKey.Name = "_txtApiKey";
			this._txtApiKey.Size = new System.Drawing.Size(303, 23);
			this._txtApiKey.TabIndex = 1;
			// 
			// ProviderSetupControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this._cmbProject);
			this.Controls.Add(this._lblProject);
			this.Controls.Add(this._lblApiKey);
			this.Controls.Add(this._txtApiKey);
			this.Controls.Add(this._txtServiceUri);
			this.Controls.Add(this._lblServiceUri);
			this.Name = "ProviderSetupControl";
			this.Size = new System.Drawing.Size(400, 87);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _lblServiceUri;
		private System.Windows.Forms.TextBox _txtServiceUri;
		private System.Windows.Forms.Label _lblApiKey;
		private System.Windows.Forms.Label _lblProject;
		private System.Windows.Forms.ComboBox _cmbProject;
		private System.Windows.Forms.TextBox _txtApiKey;
	}
}
