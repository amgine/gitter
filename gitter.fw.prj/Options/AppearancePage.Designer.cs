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
			this.panel1.SuspendLayout();
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
			// AppearancePage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupSeparator1);
			this.Controls.Add(this.panel1);
			this.Name = "AppearancePage";
			this.Size = new System.Drawing.Size(391, 305);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton _radGdiPlus;
		private System.Windows.Forms.RadioButton _radGdi;
		private Controls.GroupSeparator groupSeparator1;
	}
}
