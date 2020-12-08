namespace gitter.GitLab
{
	partial class AddServerDialog
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
			this._lblName = new System.Windows.Forms.Label();
			this._lblServiceUrl = new System.Windows.Forms.Label();
			this._txtName = new System.Windows.Forms.TextBox();
			this._txtServiceUrl = new System.Windows.Forms.TextBox();
			this._txtAPIKey = new System.Windows.Forms.TextBox();
			this._lblAPIKey = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _lblName
			// 
			this._lblName.AutoSize = true;
			this._lblName.Location = new System.Drawing.Point(0, 6);
			this._lblName.Name = "_lblName";
			this._lblName.Size = new System.Drawing.Size(62, 15);
			this._lblName.TabIndex = 0;
			this._lblName.Text = "%Name:%";
			// 
			// _lblServiceUrl
			// 
			this._lblServiceUrl.AutoSize = true;
			this._lblServiceUrl.Location = new System.Drawing.Point(0, 35);
			this._lblServiceUrl.Name = "_lblServiceUrl";
			this._lblServiceUrl.Size = new System.Drawing.Size(91, 15);
			this._lblServiceUrl.TabIndex = 1;
			this._lblServiceUrl.Text = "%Service URL:%";
			// 
			// _txtName
			// 
			this._txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtName.Location = new System.Drawing.Point(93, 3);
			this._txtName.Name = "_txtName";
			this._txtName.Size = new System.Drawing.Size(260, 23);
			this._txtName.TabIndex = 2;
			// 
			// _txtServiceUrl
			// 
			this._txtServiceUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtServiceUrl.Location = new System.Drawing.Point(93, 32);
			this._txtServiceUrl.Name = "_txtServiceUrl";
			this._txtServiceUrl.Size = new System.Drawing.Size(260, 23);
			this._txtServiceUrl.TabIndex = 3;
			// 
			// _txtAPIKey
			// 
			this._txtAPIKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtAPIKey.Location = new System.Drawing.Point(93, 61);
			this._txtAPIKey.Name = "_txtAPIKey";
			this._txtAPIKey.Size = new System.Drawing.Size(260, 23);
			this._txtAPIKey.TabIndex = 4;
			// 
			// _lblAPIKey
			// 
			this._lblAPIKey.AutoSize = true;
			this._lblAPIKey.Location = new System.Drawing.Point(0, 64);
			this._lblAPIKey.Name = "_lblAPIKey";
			this._lblAPIKey.Size = new System.Drawing.Size(70, 15);
			this._lblAPIKey.TabIndex = 5;
			this._lblAPIKey.Text = "%API Key:%";
			// 
			// AddServerDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this._lblAPIKey);
			this.Controls.Add(this._txtAPIKey);
			this.Controls.Add(this._txtServiceUrl);
			this.Controls.Add(this._txtName);
			this.Controls.Add(this._lblServiceUrl);
			this.Controls.Add(this._lblName);
			this.Name = "AddServerDialog";
			this.Size = new System.Drawing.Size(356, 88);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _lblName;
		private System.Windows.Forms.Label _lblServiceUrl;
		private System.Windows.Forms.TextBox _txtName;
		private System.Windows.Forms.TextBox _txtServiceUrl;
		private System.Windows.Forms.TextBox _txtAPIKey;
		private System.Windows.Forms.Label _lblAPIKey;
	}
}
