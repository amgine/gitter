namespace gitter.Framework.Options
{
	partial class AppearancePage
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
			this.panel1 = new System.Windows.Forms.Panel();
			this._radGdiPlus = new System.Windows.Forms.RadioButton();
			this._radGdi = new System.Windows.Forms.RadioButton();
			this.groupSeparator1 = new gitter.Framework.Controls.GroupSeparator();
			this.groupSeparator2 = new gitter.Framework.Controls.GroupSeparator();
			this._pnlThemesContainer = new System.Windows.Forms.Panel();
			this._pnlRestartRequiredWarning = new System.Windows.Forms.Panel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this._pnlRestartRequiredWarning.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this._radGdiPlus);
			this.panel1.Controls.Add(this._radGdi);
			this.panel1.Location = new System.Drawing.Point(3, 20);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(388, 48);
			this.panel1.TabIndex = 0;
			// 
			// _radGdiPlus
			// 
			this._radGdiPlus.AutoSize = true;
			this._radGdiPlus.Checked = true;
			this._radGdiPlus.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radGdiPlus.Location = new System.Drawing.Point(3, 25);
			this._radGdiPlus.Name = "_radGdiPlus";
			this._radGdiPlus.Size = new System.Drawing.Size(58, 20);
			this._radGdiPlus.TabIndex = 1;
			this._radGdiPlus.TabStop = true;
			this._radGdiPlus.Text = "GDI+";
			this._radGdiPlus.UseVisualStyleBackColor = true;
			// 
			// _radGdi
			// 
			this._radGdi.AutoSize = true;
			this._radGdi.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radGdi.Location = new System.Drawing.Point(3, 3);
			this._radGdi.Name = "_radGdi";
			this._radGdi.Size = new System.Drawing.Size(50, 20);
			this._radGdi.TabIndex = 0;
			this._radGdi.Text = "GDI";
			this._radGdi.UseVisualStyleBackColor = true;
			// 
			// groupSeparator1
			// 
			this.groupSeparator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupSeparator1.Location = new System.Drawing.Point(0, 0);
			this.groupSeparator1.Name = "groupSeparator1";
			this.groupSeparator1.Size = new System.Drawing.Size(388, 19);
			this.groupSeparator1.TabIndex = 1;
			this.groupSeparator1.Text = "Preferred text renderer";
			// 
			// groupSeparator2
			// 
			this.groupSeparator2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupSeparator2.Location = new System.Drawing.Point(0, 74);
			this.groupSeparator2.Name = "groupSeparator2";
			this.groupSeparator2.Size = new System.Drawing.Size(388, 19);
			this.groupSeparator2.TabIndex = 2;
			this.groupSeparator2.Text = "Application theme";
			// 
			// _pnlThemesContainer
			// 
			this._pnlThemesContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._pnlThemesContainer.Location = new System.Drawing.Point(3, 94);
			this._pnlThemesContainer.Name = "_pnlThemesContainer";
			this._pnlThemesContainer.Size = new System.Drawing.Size(385, 182);
			this._pnlThemesContainer.TabIndex = 3;
			// 
			// _pnlRestartRequiredWarning
			// 
			this._pnlRestartRequiredWarning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._pnlRestartRequiredWarning.Controls.Add(this.pictureBox1);
			this._pnlRestartRequiredWarning.Controls.Add(this.label1);
			this._pnlRestartRequiredWarning.Location = new System.Drawing.Point(3, 282);
			this._pnlRestartRequiredWarning.Name = "_pnlRestartRequiredWarning";
			this._pnlRestartRequiredWarning.Size = new System.Drawing.Size(385, 20);
			this._pnlRestartRequiredWarning.TabIndex = 4;
			this._pnlRestartRequiredWarning.Visible = false;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::gitter.Framework.Properties.Resources.ImgLogWarning;
			this.pictureBox1.Location = new System.Drawing.Point(0, 3);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(16, 16);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(256, 15);
			this.label1.TabIndex = 1;
			this.label1.Text = "Application must be restarted to apply changes";
			// 
			// AppearancePage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._pnlRestartRequiredWarning);
			this.Controls.Add(this._pnlThemesContainer);
			this.Controls.Add(this.groupSeparator2);
			this.Controls.Add(this.groupSeparator1);
			this.Controls.Add(this.panel1);
			this.Name = "AppearancePage";
			this.Size = new System.Drawing.Size(391, 305);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this._pnlRestartRequiredWarning.ResumeLayout(false);
			this._pnlRestartRequiredWarning.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton _radGdiPlus;
		private System.Windows.Forms.RadioButton _radGdi;
		private Controls.GroupSeparator groupSeparator1;
		private Controls.GroupSeparator groupSeparator2;
		private System.Windows.Forms.Panel _pnlThemesContainer;
		private System.Windows.Forms.Panel _pnlRestartRequiredWarning;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
	}
}
