namespace gitter.Git
{
	partial class IntegrationOptionsPage
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
			this._lstFeatures = new gitter.Framework.Controls.CustomListBox();
			this.SuspendLayout();
			// 
			// _lstFeatures
			// 
			this._lstFeatures.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lstFeatures.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._lstFeatures.Location = new System.Drawing.Point(0, 0);
			this._lstFeatures.Name = "_lstFeatures";
			this._lstFeatures.ShowCheckBoxes = true;
			this._lstFeatures.Size = new System.Drawing.Size(447, 354);
			this._lstFeatures.TabIndex = 0;
			// 
			// IntegrationOptionsPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lstFeatures);
			this.Name = "IntegrationOptionsPage";
			this.Size = new System.Drawing.Size(447, 354);
			this.ResumeLayout(false);

		}

		#endregion

		private gitter.Framework.Controls.CustomListBox _lstFeatures;

	}
}
