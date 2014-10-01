namespace gitter.Git.Gui.Views
{
	partial class DiffView
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
			if(disposing)
			{
				DiffSource = null;
				if(components != null)
				{
					components.Dispose();
				}
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
			this._diffViewer = new gitter.Git.Gui.Controls.DiffViewer();
			this.SuspendLayout();
			// 
			// _diffViewer
			// 
			this._diffViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._diffViewer.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._diffViewer.Location = new System.Drawing.Point(0, 0);
			this._diffViewer.Name = "_diffViewer";
			this._diffViewer.Size = new System.Drawing.Size(555, 362);
			this._diffViewer.TabIndex = 0;
			this._diffViewer.Text = "diffViewer1";
			// 
			// DiffView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._diffViewer);
			this.Name = "DiffView";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Controls.DiffViewer _diffViewer;
	}
}
