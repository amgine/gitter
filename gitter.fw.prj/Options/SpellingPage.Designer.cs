namespace gitter.Framework.Options
{
	partial class SpellingPage
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpellingPage));
			this.label1 = new System.Windows.Forms.Label();
			this._lstDictionaries = new gitter.Framework.Controls.CustomListBox();
			this._lnkDownload = new System.Windows.Forms.LinkLabel();
			this._lblPoweredBy = new System.Windows.Forms.Label();
			this._picLogo = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this._picLogo)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(-3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 15);
			this.label1.TabIndex = 1;
			this.label1.Text = "Dictionaries:";
			// 
			// _lstDictionaries
			// 
			this._lstDictionaries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lstDictionaries.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._lstDictionaries.Location = new System.Drawing.Point(0, 18);
			this._lstDictionaries.Name = "_lstDictionaries";
			this._lstDictionaries.ShowCheckBoxes = true;
			this._lstDictionaries.Size = new System.Drawing.Size(521, 310);
			this._lstDictionaries.TabIndex = 2;
			this._lstDictionaries.Text = "No dictionaries found";
			// 
			// _lnkDownload
			// 
			this._lnkDownload.AutoSize = true;
			this._lnkDownload.Location = new System.Drawing.Point(105, 0);
			this._lnkDownload.Name = "_lnkDownload";
			this._lnkDownload.Size = new System.Drawing.Size(91, 15);
			this._lnkDownload.TabIndex = 3;
			this._lnkDownload.TabStop = true;
			this._lnkDownload.Text = "download more";
			this._lnkDownload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._lnkDownload_LinkClicked);
			// 
			// _lblPoweredBy
			// 
			this._lblPoweredBy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._lblPoweredBy.AutoSize = true;
			this._lblPoweredBy.Location = new System.Drawing.Point(369, 0);
			this._lblPoweredBy.Name = "_lblPoweredBy";
			this._lblPoweredBy.Size = new System.Drawing.Size(72, 15);
			this._lblPoweredBy.TabIndex = 4;
			this._lblPoweredBy.Text = "Powered by:";
			// 
			// _picLogo
			// 
			this._picLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._picLogo.Cursor = System.Windows.Forms.Cursors.Hand;
			this._picLogo.Image = ((System.Drawing.Image)(resources.GetObject("_picLogo.Image")));
			this._picLogo.Location = new System.Drawing.Point(447, 0);
			this._picLogo.Name = "_picLogo";
			this._picLogo.Size = new System.Drawing.Size(71, 15);
			this._picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this._picLogo.TabIndex = 5;
			this._picLogo.TabStop = false;
			this._picLogo.Click += new System.EventHandler(this._picLogo_Click);
			// 
			// SpellingPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._picLogo);
			this.Controls.Add(this._lblPoweredBy);
			this.Controls.Add(this._lnkDownload);
			this.Controls.Add(this._lstDictionaries);
			this.Controls.Add(this.label1);
			this.Name = "SpellingPage";
			this.Size = new System.Drawing.Size(521, 328);
			((System.ComponentModel.ISupportInitialize)(this._picLogo)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private Controls.CustomListBox _lstDictionaries;
		private System.Windows.Forms.LinkLabel _lnkDownload;
		private System.Windows.Forms.Label _lblPoweredBy;
		private System.Windows.Forms.PictureBox _picLogo;
	}
}
