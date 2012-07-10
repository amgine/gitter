namespace gitter.Git.Gui.Views
{
	partial class PathHistoryView
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
			this._lstRevisions = new gitter.Git.Gui.Controls.RevisionListBox();
			this.SuspendLayout();
			// 
			// _lstRevisions
			// 
			this._lstRevisions.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._lstRevisions.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lstRevisions.Location = new System.Drawing.Point(0, 0);
			this._lstRevisions.Name = "_lstRevisions";
			this._lstRevisions.ShowStatusItems = true;
			this._lstRevisions.Size = new System.Drawing.Size(555, 362);
			this._lstRevisions.TabIndex = 1;
			// 
			// HistoryTool
			// 
			this.Controls.Add(this._lstRevisions);
			this.Name = "HistoryTool";
			this.ResumeLayout(false);
		}

		#endregion

		private gitter.Git.Gui.Controls.RevisionListBox _lstRevisions;
	}
}
