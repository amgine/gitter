namespace gitter.Git.Gui.Views
{
	partial class StashView
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
			this._lstStash = new gitter.Git.Gui.Controls.StashListBox();
			this.SuspendLayout();
			// 
			// _lstStash
			// 
			this._lstStash.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._lstStash.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lstStash.Location = new System.Drawing.Point(0, 0);
			this._lstStash.Name = "_lstStash";
			this._lstStash.Size = new System.Drawing.Size(555, 160);
			this._lstStash.TabIndex = 1;
			this._lstStash.Text = "Nothing stashed";
			// 
			// StashTool
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._lstStash);
			this.Name = "StashTool";
			this.Size = new System.Drawing.Size(555, 160);
			this.ResumeLayout(false);

		}

		#endregion

		private gitter.Git.Gui.Controls.StashListBox _lstStash;
	}
}
