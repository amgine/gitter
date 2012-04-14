namespace gitter.Controls
{
	partial class LinkButton
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
				if(components != null)
				{
					components.Dispose();
				}
				if(_underlineFont != null)
				{
					_underlineFont.Dispose();
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
			this._lblText = new System.Windows.Forms.Label();
			this._picImage = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this._picImage)).BeginInit();
			this.SuspendLayout();
			// 
			// _lblText
			// 
			this._lblText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lblText.AutoSize = true;
			this._lblText.Cursor = System.Windows.Forms.Cursors.Hand;
			this._lblText.Location = new System.Drawing.Point(27, 0);
			this._lblText.Name = "_lblText";
			this._lblText.Size = new System.Drawing.Size(41, 13);
			this._lblText.TabIndex = 1;
			this._lblText.Text = "[TEXT]";
			this._lblText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._lblText.Click += new System.EventHandler(this._lblText_Click);
			this._lblText.MouseEnter += new System.EventHandler(this.OnInteractivePartMouseEnter);
			this._lblText.MouseLeave += new System.EventHandler(this.OnInteractivePartMouseLeave);
			// 
			// _picImage
			// 
			this._picImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this._picImage.Cursor = System.Windows.Forms.Cursors.Hand;
			this._picImage.Location = new System.Drawing.Point(0, 0);
			this._picImage.Margin = new System.Windows.Forms.Padding(0);
			this._picImage.Name = "_picImage";
			this._picImage.Size = new System.Drawing.Size(24, 26);
			this._picImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this._picImage.TabIndex = 0;
			this._picImage.TabStop = false;
			this._picImage.Click += new System.EventHandler(this._picImage_Click);
			this._picImage.MouseEnter += new System.EventHandler(this.OnInteractivePartMouseEnter);
			this._picImage.MouseLeave += new System.EventHandler(this.OnInteractivePartMouseLeave);
			// 
			// LinkButton
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this._lblText);
			this.Controls.Add(this._picImage);
			this.Name = "LinkButton";
			this.Size = new System.Drawing.Size(256, 26);
			((System.ComponentModel.ISupportInitialize)(this._picImage)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _lblText;
		private System.Windows.Forms.PictureBox _picImage;
	}
}
