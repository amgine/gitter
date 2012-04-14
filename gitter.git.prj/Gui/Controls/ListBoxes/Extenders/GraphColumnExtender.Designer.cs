namespace gitter.Git.Gui.Controls
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
				UnsubscribeFromColumnEvents();
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
			this._chkShowColors = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _chkShowColors
			// 
			this._chkShowColors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._chkShowColors.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._chkShowColors.Location = new System.Drawing.Point(6, 0);
			this._chkShowColors.Name = "_chkShowColors";
			this._chkShowColors.Size = new System.Drawing.Size(137, 27);
			this._chkShowColors.TabIndex = 1;
			this._chkShowColors.Text = "%Show Colors%";
			this._chkShowColors.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._chkShowColors.UseVisualStyleBackColor = true;
			this._chkShowColors.CheckedChanged += new System.EventHandler(this.OnShowColorsCheckedChanged);
			// 
			// GraphColumnExtender
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._chkShowColors);
			this.Name = "GraphColumnExtender";
			this.Size = new System.Drawing.Size(148, 28);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox _chkShowColors;
	}
}
