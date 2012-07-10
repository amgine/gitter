namespace gitter.Git
{
	partial class GitOptionsPage
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
			this._grpRepositoryAccessor = new gitter.Framework.Controls.GroupSeparator();
			this._cmbAccessMethod = new System.Windows.Forms.ComboBox();
			this._lblAccessmethod = new System.Windows.Forms.Label();
			this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// _grpRepositoryAccessor
			// 
			this._grpRepositoryAccessor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._grpRepositoryAccessor.Location = new System.Drawing.Point(0, 0);
			this._grpRepositoryAccessor.Name = "_grpRepositoryAccessor";
			this._grpRepositoryAccessor.Size = new System.Drawing.Size(476, 19);
			this._grpRepositoryAccessor.TabIndex = 0;
			this._grpRepositoryAccessor.Text = "%Repository access method%";
			// 
			// _cmbAccessMethod
			// 
			this._cmbAccessMethod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._cmbAccessMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cmbAccessMethod.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._cmbAccessMethod.FormattingEnabled = true;
			this._cmbAccessMethod.Items.AddRange(new object[] {
            "MSysGit command line interface"});
			this._cmbAccessMethod.Location = new System.Drawing.Point(117, 25);
			this._cmbAccessMethod.Name = "_cmbAccessMethod";
			this._cmbAccessMethod.Size = new System.Drawing.Size(359, 23);
			this._cmbAccessMethod.TabIndex = 4;
			// 
			// _lblAccessmethod
			// 
			this._lblAccessmethod.AutoSize = true;
			this._lblAccessmethod.Location = new System.Drawing.Point(0, 28);
			this._lblAccessmethod.Name = "_lblAccessmethod";
			this._lblAccessmethod.Size = new System.Drawing.Size(111, 15);
			this._lblAccessmethod.TabIndex = 8;
			this._lblAccessmethod.Text = "%Access Method%:";
			// 
			// GitOptionsPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._lblAccessmethod);
			this.Controls.Add(this._cmbAccessMethod);
			this.Controls.Add(this._grpRepositoryAccessor);
			this.Name = "GitOptionsPage";
			this.Size = new System.Drawing.Size(479, 218);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private gitter.Framework.Controls.GroupSeparator _grpRepositoryAccessor;
		private System.Windows.Forms.ComboBox _cmbAccessMethod;
		private System.Windows.Forms.Label _lblAccessmethod;
		private System.Windows.Forms.OpenFileDialog _openFileDialog;
	}
}
