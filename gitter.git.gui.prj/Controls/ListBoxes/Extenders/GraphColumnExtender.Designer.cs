﻿namespace gitter.Git.Gui.Controls
{
	partial class GraphColumnExtender
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
				if(_chkShowColors is not null)
				{
					_chkShowColors.Dispose();
					_chkShowColors = null;
				}
				UnsubscribeFromColumnEvents();
				components?.Dispose();
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
			this.SuspendLayout();
			// 
			// GraphColumnExtender
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Name = "GraphColumnExtender";
			this.Size = new System.Drawing.Size(148, 28);
			this.ResumeLayout(false);

		}

		#endregion

	}
}
