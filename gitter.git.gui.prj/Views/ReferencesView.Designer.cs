namespace gitter.Git.Gui.Views
{
	partial class ReferencesView
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
			this._lstReferences = new gitter.Git.Gui.Controls.ReferencesListBox();
			this.SuspendLayout();
			// 
			// _lstReferences
			// 
			this._lstReferences.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._lstReferences.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lstReferences.Location = new System.Drawing.Point(0, 0);
			this._lstReferences.Name = "_lstReferences";
			this._lstReferences.ShowTreeLines = true;
			this._lstReferences.Size = new System.Drawing.Size(555, 362);
			this._lstReferences.TabIndex = 1;
			this._lstReferences.Text = "referencesListBox1";
			// 
			// ReferencesView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lstReferences);
			this.Name = "ReferencesView";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private gitter.Git.Gui.Controls.ReferencesListBox _lstReferences;
	}
}
