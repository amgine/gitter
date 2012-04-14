namespace gitter.Framework.Controls
{
	partial class WebBrowserView
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
			this._webBrowser = new System.Windows.Forms.WebBrowser();
			this.SuspendLayout();
			// 
			// _webBrowser
			// 
			this._webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this._webBrowser.Location = new System.Drawing.Point(0, 0);
			this._webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this._webBrowser.Name = "_webBrowser";
			this._webBrowser.ScriptErrorsSuppressed = true;
			this._webBrowser.Size = new System.Drawing.Size(555, 362);
			this._webBrowser.TabIndex = 0;
			// 
			// WebBrowserView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._webBrowser);
			this.Name = "WebBrowserView";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.WebBrowser _webBrowser;
	}
}
