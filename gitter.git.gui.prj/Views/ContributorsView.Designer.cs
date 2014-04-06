namespace gitter.Git.Gui.Views
{
	partial class ContributorsView
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
			this._lstUsers = new gitter.Git.Gui.Controls.UsresListBox();
			this.SuspendLayout();
			// 
			// _lstUsers
			// 
			this._lstUsers.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._lstUsers.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lstUsers.Location = new System.Drawing.Point(0, 0);
			this._lstUsers.Margin = new System.Windows.Forms.Padding(0);
			this._lstUsers.Name = "_lstUsers";
			this._lstUsers.Size = new System.Drawing.Size(555, 362);
			this._lstUsers.TabIndex = 1;
			this._lstUsers.Text = "No Users";
			// 
			// ContributorsView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lstUsers);
			this.Name = "ContributorsView";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private gitter.Git.Gui.Controls.UsresListBox _lstUsers;
	}
}
