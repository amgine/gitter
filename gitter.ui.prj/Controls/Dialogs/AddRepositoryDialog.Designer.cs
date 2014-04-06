namespace gitter
{
	partial class AddRepositoryDialog
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
			this._btnSelectDirectory = new System.Windows.Forms.Button();
			this._lblPath = new System.Windows.Forms.Label();
			this._txtPath = new System.Windows.Forms.TextBox();
			this._txtDescription = new System.Windows.Forms.TextBox();
			this._lblDescription = new System.Windows.Forms.Label();
			this.SuspendLayout();
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
			this._lblPath.TabIndex = 15;
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
			// _txtDescription
			// 
			this._txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._txtDescription.Location = new System.Drawing.Point(94, 32);
			this._txtDescription.Name = "_txtDescription";
			this._txtDescription.Size = new System.Drawing.Size(288, 23);
			this._txtDescription.TabIndex = 2;
			// 
			// _lblDescription
			// 
			this._lblDescription.AutoSize = true;
			this._lblDescription.Location = new System.Drawing.Point(0, 35);
			this._lblDescription.Name = "_lblDescription";
			this._lblDescription.Size = new System.Drawing.Size(90, 15);
			this._lblDescription.TabIndex = 15;
			this._lblDescription.Text = "%Description%:";
			// 
			// AddRepositoryDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._btnSelectDirectory);
			this.Controls.Add(this._lblDescription);
			this.Controls.Add(this._lblPath);
			this.Controls.Add(this._txtDescription);
			this.Controls.Add(this._txtPath);
			this.Name = "AddRepositoryDialog";
			this.Size = new System.Drawing.Size(385, 58);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _btnSelectDirectory;
		private System.Windows.Forms.Label _lblPath;
		private System.Windows.Forms.TextBox _txtPath;
		private System.Windows.Forms.TextBox _txtDescription;
		private System.Windows.Forms.Label _lblDescription;
	}
}
