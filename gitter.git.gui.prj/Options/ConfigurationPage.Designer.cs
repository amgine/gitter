namespace gitter.Git
{
	partial class ConfigurationPage
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
			this._lstUserConfig = new gitter.Git.Gui.Controls.ConfigListBox();
			this._tabs = new System.Windows.Forms.TabControl();
			this._pageUser = new System.Windows.Forms.TabPage();
			this._btnAddUserParameter = new System.Windows.Forms.Button();
			this._pageSystem = new System.Windows.Forms.TabPage();
			this._btnAddSystemParameter = new System.Windows.Forms.Button();
			this._lstSystemConfig = new gitter.Git.Gui.Controls.ConfigListBox();
			this._tabs.SuspendLayout();
			this._pageUser.SuspendLayout();
			this._pageSystem.SuspendLayout();
			this.SuspendLayout();
			// 
			// _lstUserConfig
			// 
			this._lstUserConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lstUserConfig.ForeColor = System.Drawing.SystemColors.WindowText;
			this._lstUserConfig.ItemHeight = 21;
			this._lstUserConfig.Location = new System.Drawing.Point(0, 0);
			this._lstUserConfig.Name = "_lstUserConfig";
			this._lstUserConfig.Size = new System.Drawing.Size(520, 311);
			this._lstUserConfig.TabIndex = 0;
			// 
			// _tabs
			// 
			this._tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._tabs.Controls.Add(this._pageUser);
			this._tabs.Controls.Add(this._pageSystem);
			this._tabs.Location = new System.Drawing.Point(0, 0);
			this._tabs.Name = "_tabs";
			this._tabs.SelectedIndex = 0;
			this._tabs.Size = new System.Drawing.Size(528, 374);
			this._tabs.TabIndex = 1;
			// 
			// _pageUser
			// 
			this._pageUser.Controls.Add(this._btnAddUserParameter);
			this._pageUser.Controls.Add(this._lstUserConfig);
			this._pageUser.Location = new System.Drawing.Point(4, 24);
			this._pageUser.Name = "_pageUser";
			this._pageUser.Padding = new System.Windows.Forms.Padding(3);
			this._pageUser.Size = new System.Drawing.Size(520, 346);
			this._pageUser.TabIndex = 0;
			this._pageUser.Text = "%Current User%";
			this._pageUser.UseVisualStyleBackColor = true;
			// 
			// _btnAddUserParameter
			// 
			this._btnAddUserParameter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnAddUserParameter.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnAddUserParameter.Location = new System.Drawing.Point(384, 317);
			this._btnAddUserParameter.Name = "_btnAddUserParameter";
			this._btnAddUserParameter.Size = new System.Drawing.Size(130, 23);
			this._btnAddUserParameter.TabIndex = 1;
			this._btnAddUserParameter.Text = "%Add Parameter%";
			this._btnAddUserParameter.UseVisualStyleBackColor = true;
			this._btnAddUserParameter.Click += new System.EventHandler(this._addUserParameter_Click);
			// 
			// _pageSystem
			// 
			this._pageSystem.Controls.Add(this._btnAddSystemParameter);
			this._pageSystem.Controls.Add(this._lstSystemConfig);
			this._pageSystem.Location = new System.Drawing.Point(4, 24);
			this._pageSystem.Name = "_pageSystem";
			this._pageSystem.Padding = new System.Windows.Forms.Padding(3);
			this._pageSystem.Size = new System.Drawing.Size(520, 346);
			this._pageSystem.TabIndex = 1;
			this._pageSystem.Text = "%System%";
			this._pageSystem.UseVisualStyleBackColor = true;
			// 
			// _btnAddSystemParameter
			// 
			this._btnAddSystemParameter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnAddSystemParameter.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnAddSystemParameter.Location = new System.Drawing.Point(384, 317);
			this._btnAddSystemParameter.Name = "_btnAddSystemParameter";
			this._btnAddSystemParameter.Size = new System.Drawing.Size(130, 23);
			this._btnAddSystemParameter.TabIndex = 2;
			this._btnAddSystemParameter.Text = "%Add Parameter%";
			this._btnAddSystemParameter.UseVisualStyleBackColor = true;
			this._btnAddSystemParameter.Click += new System.EventHandler(this._addSystemParameter_Click);
			// 
			// _lstSystemConfig
			// 
			this._lstSystemConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lstSystemConfig.ForeColor = System.Drawing.SystemColors.WindowText;
			this._lstSystemConfig.ItemHeight = 21;
			this._lstSystemConfig.Location = new System.Drawing.Point(0, 0);
			this._lstSystemConfig.Name = "_lstSystemConfig";
			this._lstSystemConfig.Size = new System.Drawing.Size(520, 311);
			this._lstSystemConfig.TabIndex = 1;
			// 
			// ConfigurationPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._tabs);
			this.Name = "ConfigurationPage";
			this.Size = new System.Drawing.Size(528, 374);
			this._tabs.ResumeLayout(false);
			this._pageUser.ResumeLayout(false);
			this._pageSystem.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private gitter.Git.Gui.Controls.ConfigListBox _lstUserConfig;
		private System.Windows.Forms.TabControl _tabs;
		private System.Windows.Forms.TabPage _pageUser;
		private System.Windows.Forms.TabPage _pageSystem;
		private gitter.Git.Gui.Controls.ConfigListBox _lstSystemConfig;
		private System.Windows.Forms.Button _btnAddUserParameter;
		private System.Windows.Forms.Button _btnAddSystemParameter;
	}
}
