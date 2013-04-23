namespace gitter.Git.Gui.Controls
{
	partial class SubjectColumnExtender
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
				UnsubscribeFromColumnEvents();
				if(components != null)
				{
					components.Dispose();
				}
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
			this.SuspendLayout();
			// 
			// SubjectColumnExtender
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Name = "SubjectColumnExtender";
			this.Size = new System.Drawing.Size(214, 143);
			this.ResumeLayout(false);

		}

		#endregion

	}
}
