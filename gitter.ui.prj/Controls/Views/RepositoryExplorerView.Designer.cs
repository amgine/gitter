namespace gitter
{
	partial class RepositoryExplorerView
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
			this._lstRepositoryExplorer = new gitter.RepositoryExplorerListBox();
			this.SuspendLayout();
			// 
			// _lstTools
			// 
			this._lstRepositoryExplorer.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._lstRepositoryExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lstRepositoryExplorer.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._lstRepositoryExplorer.Location = new System.Drawing.Point(0, 0);
			this._lstRepositoryExplorer.Name = "_lstRepositoryExplorer";
			this._lstRepositoryExplorer.ShowTreeLines = true;
			this._lstRepositoryExplorer.Size = new System.Drawing.Size(153, 449);
			this._lstRepositoryExplorer.TabIndex = 0;
			this._lstRepositoryExplorer.ItemActivated += new System.EventHandler<gitter.Framework.Controls.ItemEventArgs>(this.OnItemActivated);
			// 
			// ToolboxTool
			// 
			this.Controls.Add(this._lstRepositoryExplorer);
			this.Name = "ToolboxTool";
			this.Size = new System.Drawing.Size(153, 449);
			this.ResumeLayout(false);

		}

		#endregion

		private RepositoryExplorerListBox _lstRepositoryExplorer;
	}
}
