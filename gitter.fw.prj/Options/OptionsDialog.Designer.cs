namespace gitter.Framework.Options
{
	partial class OptionsDialog
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
				if (components != null)
				{
					components.Dispose();
				}
				_activePage = null;
				foreach(var page in _propertyPages.Values)
				{
					if(page != null)
					{
						page.Dispose();
					}
				}
				_propertyPages.Clear();
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
			this._lstOptions = new gitter.Framework.Options.OptionsListBox();
			this._pnlPageContainer = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// _lstOptions
			// 
			this._lstOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this._lstOptions.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._lstOptions.ItemActivation = gitter.Framework.Controls.ItemActivation.SingleClick;
			this._lstOptions.Location = new System.Drawing.Point(3, 3);
			this._lstOptions.Name = "_lstOptions";
			this._lstOptions.ShowTreeLines = true;
			this._lstOptions.Size = new System.Drawing.Size(162, 375);
			this._lstOptions.TabIndex = 0;
			this._lstOptions.ItemActivated += new System.EventHandler<gitter.Framework.Controls.ItemEventArgs>(this.OnItemActivated);
			// 
			// _pnlPageContainer
			// 
			this._pnlPageContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._pnlPageContainer.Location = new System.Drawing.Point(171, 3);
			this._pnlPageContainer.Name = "_pnlPageContainer";
			this._pnlPageContainer.Size = new System.Drawing.Size(445, 375);
			this._pnlPageContainer.TabIndex = 1;
			// 
			// OptionsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._pnlPageContainer);
			this.Controls.Add(this._lstOptions);
			this.Name = "OptionsDialog";
			this.Size = new System.Drawing.Size(619, 381);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private gitter.Framework.Options.OptionsListBox _lstOptions;
		private System.Windows.Forms.Panel _pnlPageContainer;

	}
}
