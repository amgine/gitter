namespace gitter.Git.Gui.Views
{
	partial class RemotesView
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
			this._lstRemotes = new gitter.Git.Gui.Controls.RemoteListBox();
			this.SuspendLayout();
			// 
			// _lstRemotes
			// 
			this._lstRemotes.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._lstRemotes.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lstRemotes.Location = new System.Drawing.Point(0, 0);
			this._lstRemotes.Margin = new System.Windows.Forms.Padding(0);
			this._lstRemotes.Name = "_lstRemotes";
			this._lstRemotes.Size = new System.Drawing.Size(555, 160);
			this._lstRemotes.TabIndex = 1;
			this._lstRemotes.Text = "No Remotes";
			// 
			// RemotesView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lstRemotes);
			this.Name = "RemotesView";
			this.Size = new System.Drawing.Size(555, 160);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private gitter.Git.Gui.Controls.RemoteListBox _lstRemotes;
	}
}
