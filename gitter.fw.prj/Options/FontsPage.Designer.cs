namespace gitter.Framework.Options
{
	partial class FontsPage
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
			this._lblFonts = new System.Windows.Forms.Label();
			this._lstFonts = new gitter.Framework.Options.FontsListBox();
			this._lblName = new System.Windows.Forms.Label();
			this._lblSize = new System.Windows.Forms.Label();
			this._lblStyle = new System.Windows.Forms.Label();
			this._lblSample = new System.Windows.Forms.Label();
			this._cmbFonts = new System.Windows.Forms.ComboBox();
			this._numSize = new System.Windows.Forms.NumericUpDown();
			this._cmbStyle = new System.Windows.Forms.ComboBox();
			this._pnlSelectedFont = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this._numSize)).BeginInit();
			this._pnlSelectedFont.SuspendLayout();
			this.SuspendLayout();
			// 
			// _lblFonts
			// 
			this._lblFonts.AutoSize = true;
			this._lblFonts.Location = new System.Drawing.Point(-3, 0);
			this._lblFonts.Name = "_lblFonts";
			this._lblFonts.Size = new System.Drawing.Size(59, 15);
			this._lblFonts.TabIndex = 0;
			this._lblFonts.Text = "%Fonts%:";
			// 
			// _lstFonts
			// 
			this._lstFonts.AllowColumnReorder = false;
			this._lstFonts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lstFonts.ForeColor = System.Drawing.SystemColors.WindowText;
			this._lstFonts.ItemActivation = gitter.Framework.Controls.ItemActivation.SingleClick;
			this._lstFonts.Location = new System.Drawing.Point(0, 18);
			this._lstFonts.Name = "_lstFonts";
			this._lstFonts.Size = new System.Drawing.Size(448, 245);
			this._lstFonts.TabIndex = 1;
			// 
			// _lblName
			// 
			this._lblName.AutoSize = true;
			this._lblName.Location = new System.Drawing.Point(-3, 3);
			this._lblName.Name = "_lblName";
			this._lblName.Size = new System.Drawing.Size(62, 15);
			this._lblName.TabIndex = 2;
			this._lblName.Text = "%Name%:";
			// 
			// _lblSize
			// 
			this._lblSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._lblSize.AutoSize = true;
			this._lblSize.Location = new System.Drawing.Point(305, 3);
			this._lblSize.Name = "_lblSize";
			this._lblSize.Size = new System.Drawing.Size(50, 15);
			this._lblSize.TabIndex = 3;
			this._lblSize.Text = "%Size%:";
			// 
			// _lblStyle
			// 
			this._lblStyle.AutoSize = true;
			this._lblStyle.Location = new System.Drawing.Point(-3, 30);
			this._lblStyle.Name = "_lblStyle";
			this._lblStyle.Size = new System.Drawing.Size(55, 15);
			this._lblStyle.TabIndex = 4;
			this._lblStyle.Text = "%Style%:";
			// 
			// _lblSample
			// 
			this._lblSample.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lblSample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._lblSample.Location = new System.Drawing.Point(0, 56);
			this._lblSample.Name = "_lblSample";
			this._lblSample.Size = new System.Drawing.Size(448, 50);
			this._lblSample.TabIndex = 5;
			this._lblSample.Text = "%Sample%";
			this._lblSample.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _cmbFonts
			// 
			this._cmbFonts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._cmbFonts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cmbFonts.FormattingEnabled = true;
			this._cmbFonts.Location = new System.Drawing.Point(82, 0);
			this._cmbFonts.Name = "_cmbFonts";
			this._cmbFonts.Size = new System.Drawing.Size(217, 23);
			this._cmbFonts.TabIndex = 6;
			this._cmbFonts.SelectedIndexChanged += new System.EventHandler(this.OnFontFamilySelected);
			// 
			// _numSize
			// 
			this._numSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._numSize.Location = new System.Drawing.Point(361, 0);
			this._numSize.Name = "_numSize";
			this._numSize.Size = new System.Drawing.Size(87, 23);
			this._numSize.TabIndex = 7;
			this._numSize.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
			this._numSize.ValueChanged += new System.EventHandler(this.OnFontSizeChanged);
			// 
			// _cmbStyle
			// 
			this._cmbStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._cmbStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cmbStyle.FormattingEnabled = true;
			this._cmbStyle.Location = new System.Drawing.Point(82, 27);
			this._cmbStyle.Name = "_cmbStyle";
			this._cmbStyle.Size = new System.Drawing.Size(217, 23);
			this._cmbStyle.TabIndex = 6;
			this._cmbStyle.SelectedIndexChanged += new System.EventHandler(this.OnFontStyleChanged);
			// 
			// _pnlSelectedFont
			// 
			this._pnlSelectedFont.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._pnlSelectedFont.Controls.Add(this._lblName);
			this._pnlSelectedFont.Controls.Add(this._numSize);
			this._pnlSelectedFont.Controls.Add(this._lblSize);
			this._pnlSelectedFont.Controls.Add(this._cmbStyle);
			this._pnlSelectedFont.Controls.Add(this._lblStyle);
			this._pnlSelectedFont.Controls.Add(this._cmbFonts);
			this._pnlSelectedFont.Controls.Add(this._lblSample);
			this._pnlSelectedFont.Enabled = false;
			this._pnlSelectedFont.Location = new System.Drawing.Point(0, 269);
			this._pnlSelectedFont.Name = "_pnlSelectedFont";
			this._pnlSelectedFont.Size = new System.Drawing.Size(448, 106);
			this._pnlSelectedFont.TabIndex = 8;
			// 
			// FontsPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._pnlSelectedFont);
			this.Controls.Add(this._lstFonts);
			this.Controls.Add(this._lblFonts);
			this.Name = "FontsPage";
			this.Size = new System.Drawing.Size(448, 375);
			((System.ComponentModel.ISupportInitialize)(this._numSize)).EndInit();
			this._pnlSelectedFont.ResumeLayout(false);
			this._pnlSelectedFont.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _lblFonts;
		private gitter.Framework.Options.FontsListBox _lstFonts;
		private System.Windows.Forms.Label _lblName;
		private System.Windows.Forms.Label _lblSize;
		private System.Windows.Forms.Label _lblStyle;
		private System.Windows.Forms.Label _lblSample;
		private System.Windows.Forms.ComboBox _cmbFonts;
		private System.Windows.Forms.NumericUpDown _numSize;
		private System.Windows.Forms.ComboBox _cmbStyle;
		private System.Windows.Forms.Panel _pnlSelectedFont;

	}
}
