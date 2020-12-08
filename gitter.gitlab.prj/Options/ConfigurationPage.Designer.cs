
namespace gitter.GitLab.Options
{
	partial class ConfigurationPage
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._lstServers = new gitter.Framework.Controls.CustomListBox();
			this._lblServers = new System.Windows.Forms.Label();
			this._btnAdd = new System.Windows.Forms.Button();
			this._btnRemove = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _lstServers
			// 
			this._lstServers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lstServers.ForeColor = System.Drawing.SystemColors.WindowText;
			this._lstServers.ItemHeight = 21;
			this._lstServers.Location = new System.Drawing.Point(0, 18);
			this._lstServers.Name = "_lstServers";
			this._lstServers.Size = new System.Drawing.Size(617, 495);
			this._lstServers.TabIndex = 0;
			// 
			// _lblServers
			// 
			this._lblServers.AutoSize = true;
			this._lblServers.Location = new System.Drawing.Point(-3, 0);
			this._lblServers.Name = "_lblServers";
			this._lblServers.Size = new System.Drawing.Size(64, 15);
			this._lblServers.TabIndex = 1;
			this._lblServers.Text = "%Servers%";
			// 
			// _btnAdd
			// 
			this._btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnAdd.Location = new System.Drawing.Point(461, 519);
			this._btnAdd.Name = "_btnAdd";
			this._btnAdd.Size = new System.Drawing.Size(75, 23);
			this._btnAdd.TabIndex = 2;
			this._btnAdd.Text = "%Add%";
			this._btnAdd.UseVisualStyleBackColor = true;
			this._btnAdd.Click += new System.EventHandler(this.OnAddServerClick);
			// 
			// _btnRemove
			// 
			this._btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnRemove.Enabled = false;
			this._btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnRemove.Location = new System.Drawing.Point(542, 519);
			this._btnRemove.Name = "_btnRemove";
			this._btnRemove.Size = new System.Drawing.Size(75, 23);
			this._btnRemove.TabIndex = 2;
			this._btnRemove.Text = "%Remove%";
			this._btnRemove.UseVisualStyleBackColor = true;
			this._btnRemove.Click += new System.EventHandler(this.OnRemoveServerClick);
			// 
			// ConfigurationPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this._btnRemove);
			this.Controls.Add(this._btnAdd);
			this.Controls.Add(this._lblServers);
			this.Controls.Add(this._lstServers);
			this.Name = "ConfigurationPage";
			this.Size = new System.Drawing.Size(617, 542);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Framework.Controls.CustomListBox _lstServers;
		private System.Windows.Forms.Label _lblServers;
		private System.Windows.Forms.Button _btnAdd;
		private System.Windows.Forms.Button _btnRemove;
	}
}
