namespace gitter.Framework.Controls
{
	partial class GroupSeparator
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
			this._line = new System.Windows.Forms.Panel();
			this._lblText = new System.Windows.Forms.Label();
			this._picChevron = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this._picChevron)).BeginInit();
			this.SuspendLayout();
			// 
			// _line
			// 
			this._line.BackColor = System.Drawing.SystemColors.ControlDark;
			this._line.Name = "_line";
			this._line.Size = new System.Drawing.Size(340, 1);
			this._line.TabIndex = 1;
			this._line.Click += new System.EventHandler(this.OnClick);
			this._line.MouseEnter += new System.EventHandler(this.OnHostedControlMouseEnter);
			this._line.MouseLeave += new System.EventHandler(this.OnHostedControlMouseLeave);
			// 
			// _lblText
			// 
			this._lblText.AutoSize = true;
			this._lblText.Location = new System.Drawing.Point(0, 3);
			this._lblText.Name = "_lblText";
			this._lblText.Size = new System.Drawing.Size(49, 15);
			this._lblText.TabIndex = 0;
			this._lblText.Text = "%Text%";
			this._lblText.Click += new System.EventHandler(this.OnClick);
			this._lblText.MouseEnter += new System.EventHandler(this.OnHostedControlMouseEnter);
			this._lblText.MouseLeave += new System.EventHandler(this.OnHostedControlMouseLeave);
			this._lblText.Resize += new System.EventHandler(this._lblText_Resize);
			// 
			// _picChevron
			// 
			this._picChevron.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._picChevron.Cursor = System.Windows.Forms.Cursors.Hand;
			this._picChevron.Location = new System.Drawing.Point(388, 0);
			this._picChevron.Name = "_picChevron";
			this._picChevron.Size = new System.Drawing.Size(19, 19);
			this._picChevron.TabIndex = 2;
			this._picChevron.TabStop = false;
			this._picChevron.Visible = false;
			this._picChevron.Click += new System.EventHandler(this.OnClick);
			this._picChevron.MouseEnter += new System.EventHandler(this.OnHostedControlMouseEnter);
			// 
			// GroupSeparator
			// 
			this.Controls.Add(this._picChevron);
			this.Controls.Add(this._lblText);
			this.Controls.Add(this._line);
			this.MaximumSize = new System.Drawing.Size(9999, System.Windows.Forms.SystemInformation.SmallIconSize.Height + 3);
			this.MinimumSize = new System.Drawing.Size(0, System.Windows.Forms.SystemInformation.SmallIconSize.Height + 3);
			this.Name = "GroupSeparator";
			this.Size = new System.Drawing.Size(407, 19);
			this.Click += new System.EventHandler(this.OnClick);
			((System.ComponentModel.ISupportInitialize)(this._picChevron)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _lblText;
		private System.Windows.Forms.Panel _line;
		private System.Windows.Forms.PictureBox _picChevron;
	}
}
