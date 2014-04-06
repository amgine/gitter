namespace gitter.Git.Gui.Dialogs
{
	partial class RenameRemoteDialog
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
			this._lblNewName = new System.Windows.Forms.Label();
			this._lblOldName = new System.Windows.Forms.Label();
			this._txtNewName = new System.Windows.Forms.TextBox();
			this._txtOldName = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// _lblNewName
			// 
			this._lblNewName.AutoSize = true;
			this._lblNewName.Location = new System.Drawing.Point(0, 32);
			this._lblNewName.Name = "_lblNewName";
			this._lblNewName.Size = new System.Drawing.Size(79, 13);
			this._lblNewName.TabIndex = 7;
			this._lblNewName.Text = "%New Name%:";
			// 
			// _lblOldName
			// 
			this._lblOldName.AutoSize = true;
			this._lblOldName.Location = new System.Drawing.Point(0, 6);
			this._lblOldName.Name = "_lblOldName";
			this._lblOldName.Size = new System.Drawing.Size(63, 13);
			this._lblOldName.TabIndex = 6;
			this._lblOldName.Text = "%Remote%:";
			// 
			// _txtNewName
			// 
			this._txtNewName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._txtNewName.Location = new System.Drawing.Point(94, 29);
			this._txtNewName.Name = "_txtNewName";
			this._txtNewName.Size = new System.Drawing.Size(288, 20);
			this._txtNewName.TabIndex = 4;
			// 
			// _txtOldName
			// 
			this._txtOldName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._txtOldName.Location = new System.Drawing.Point(94, 3);
			this._txtOldName.Name = "_txtOldName";
			this._txtOldName.ReadOnly = true;
			this._txtOldName.Size = new System.Drawing.Size(288, 20);
			this._txtOldName.TabIndex = 5;
			// 
			// RenameRemoteDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lblNewName);
			this.Controls.Add(this._lblOldName);
			this.Controls.Add(this._txtNewName);
			this.Controls.Add(this._txtOldName);
			this.Name = "RenameRemoteDialog";
			this.Size = new System.Drawing.Size(385, 53);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _lblNewName;
		private System.Windows.Forms.Label _lblOldName;
		private System.Windows.Forms.TextBox _txtNewName;
		private System.Windows.Forms.TextBox _txtOldName;
	}
}
