namespace gitter.Git.Gui.Controls
{
	partial class HashColumnExtender
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
			this._chkAbbreviate = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _chkAbbreviate
			// 
			this._chkAbbreviate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._chkAbbreviate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._chkAbbreviate.Location = new System.Drawing.Point(6, 0);
			this._chkAbbreviate.Name = "_chkAbbreviate";
			this._chkAbbreviate.Size = new System.Drawing.Size(129, 27);
			this._chkAbbreviate.TabIndex = 1;
			this._chkAbbreviate.Text = "%Abbreviate%";
			this._chkAbbreviate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._chkAbbreviate.UseVisualStyleBackColor = true;
			this._chkAbbreviate.CheckedChanged += new System.EventHandler(this.OnAbbreviateCheckedChanged);
			// 
			// HashColumnExtender
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._chkAbbreviate);
			this.Name = "HashColumnExtender";
			this.Size = new System.Drawing.Size(140, 28);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox _chkAbbreviate;
	}
}
