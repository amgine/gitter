namespace gitter.Framework.Options
{
	partial class BehaviorPage
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
			this._chkIntergateInFolderMenu = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _chkIntergateInFolderMenu
			// 
			this._chkIntergateInFolderMenu.AutoSize = true;
			this._chkIntergateInFolderMenu.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkIntergateInFolderMenu.Location = new System.Drawing.Point(0, 3);
			this._chkIntergateInFolderMenu.Name = "_chkIntergateInFolderMenu";
			this._chkIntergateInFolderMenu.Size = new System.Drawing.Size(338, 20);
			this._chkIntergateInFolderMenu.TabIndex = 0;
			this._chkIntergateInFolderMenu.Text = "Add \"Run gitter\" item to folder menu in Windows Explorer";
			this._chkIntergateInFolderMenu.UseVisualStyleBackColor = true;
			// 
			// BehaviorPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._chkIntergateInFolderMenu);
			this.Name = "BehaviorPage";
			this.Size = new System.Drawing.Size(351, 215);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox _chkIntergateInFolderMenu;
	}
}
