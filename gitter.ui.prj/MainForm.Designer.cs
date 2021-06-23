namespace gitter
{
	partial class MainForm
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
			System.Windows.Forms.ToolStripSeparator _separator1;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this._toolStripContainer = new System.Windows.Forms.ToolStripContainer();
			this._statusStrip = new System.Windows.Forms.StatusStrip();
			this._statusSeparator = new System.Windows.Forms.ToolStripStatusLabel();
			this._toolDockGrid = new gitter.Framework.Controls.ViewDockGrid();
			this._menuStrip = new System.Windows.Forms.MenuStrip();
			this._mnuRepository = new System.Windows.Forms.ToolStripMenuItem();
			this._mnuOpenRepository = new System.Windows.Forms.ToolStripMenuItem();
			this._mnuRecentRepositories = new System.Windows.Forms.ToolStripMenuItem();
			this._mnuDummy = new System.Windows.Forms.ToolStripMenuItem();
			this._mnuExit = new System.Windows.Forms.ToolStripMenuItem();
			this._mnuView = new System.Windows.Forms.ToolStripMenuItem();
			this._mnuToolbars = new System.Windows.Forms.ToolStripMenuItem();
			this._mnuTools = new System.Windows.Forms.ToolStripMenuItem();
			this._mnuOptions = new System.Windows.Forms.ToolStripMenuItem();
			this._mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
			this._mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
			_separator1 = new System.Windows.Forms.ToolStripSeparator();
			this._toolStripContainer.BottomToolStripPanel.SuspendLayout();
			this._toolStripContainer.ContentPanel.SuspendLayout();
			this._toolStripContainer.TopToolStripPanel.SuspendLayout();
			this._toolStripContainer.SuspendLayout();
			this._statusStrip.SuspendLayout();
			this._menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// _separator1
			// 
			_separator1.Name = "_separator1";
			_separator1.Size = new System.Drawing.Size(172, 6);
			// 
			// _toolStripContainer
			// 
			// 
			// _toolStripContainer.BottomToolStripPanel
			// 
			this._toolStripContainer.BottomToolStripPanel.Controls.Add(this._statusStrip);
			// 
			// _toolStripContainer.ContentPanel
			// 
			this._toolStripContainer.ContentPanel.Controls.Add(this._toolDockGrid);
			this._toolStripContainer.ContentPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
			this._toolStripContainer.ContentPanel.Size = new System.Drawing.Size(795, 450);
			this._toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._toolStripContainer.Location = new System.Drawing.Point(0, 0);
			this._toolStripContainer.Name = "_toolStripContainer";
			this._toolStripContainer.Size = new System.Drawing.Size(795, 496);
			this._toolStripContainer.TabIndex = 6;
			this._toolStripContainer.Text = "toolStripContainer1";
			// 
			// _toolStripContainer.TopToolStripPanel
			// 
			this._toolStripContainer.TopToolStripPanel.Controls.Add(this._menuStrip);
			// 
			// _statusStrip
			// 
			this._statusStrip.Dock = System.Windows.Forms.DockStyle.None;
			this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusSeparator});
			this._statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this._statusStrip.Location = new System.Drawing.Point(0, 0);
			this._statusStrip.Name = "_statusStrip";
			this._statusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
			this._statusStrip.Size = new System.Drawing.Size(795, 22);
			this._statusStrip.TabIndex = 0;
			// 
			// _statusSeparator
			// 
			this._statusSeparator.Name = "_statusSeparator";
			this._statusSeparator.Size = new System.Drawing.Size(0, 17);
			this._statusSeparator.Spring = true;
			// 
			// _toolDockGrid
			// 
			this._toolDockGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this._toolDockGrid.Location = new System.Drawing.Point(0, 0);
			this._toolDockGrid.Name = "_toolDockGrid";
			this._toolDockGrid.Size = new System.Drawing.Size(795, 450);
			this._toolDockGrid.TabIndex = 0;
			// 
			// _menuStrip
			// 
			this._menuStrip.Dock = System.Windows.Forms.DockStyle.None;
			this._menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mnuRepository,
            this._mnuView,
            this._mnuTools,
            this._mnuHelp});
			this._menuStrip.Location = new System.Drawing.Point(0, 0);
			this._menuStrip.Name = "_menuStrip";
			this._menuStrip.Size = new System.Drawing.Size(795, 24);
			this._menuStrip.TabIndex = 0;
			// 
			// _mnuRepository
			// 
			this._mnuRepository.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mnuOpenRepository,
            this._mnuRecentRepositories,
            _separator1,
            this._mnuExit});
			this._mnuRepository.Name = "_mnuRepository";
			this._mnuRepository.Size = new System.Drawing.Size(95, 20);
			this._mnuRepository.Text = "%Repository%";
			// 
			// _mnuOpenRepository
			// 
			this._mnuOpenRepository.Name = "_mnuOpenRepository";
			this._mnuOpenRepository.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this._mnuOpenRepository.Size = new System.Drawing.Size(175, 22);
			this._mnuOpenRepository.Text = "%Open%...";
			this._mnuOpenRepository.Click += new System.EventHandler(this._mnuOpen_Click);
			// 
			// _mnuRecentRepositories
			// 
			this._mnuRecentRepositories.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mnuDummy});
			this._mnuRecentRepositories.Name = "_mnuRecentRepositories";
			this._mnuRecentRepositories.Size = new System.Drawing.Size(175, 22);
			this._mnuRecentRepositories.Text = "%Recent%";
			// 
			// _mnuDummy
			// 
			this._mnuDummy.Enabled = false;
			this._mnuDummy.Name = "_mnuDummy";
			this._mnuDummy.Size = new System.Drawing.Size(153, 22);
			this._mnuDummy.Text = "<no available>";
			// 
			// _mnuExit
			// 
			this._mnuExit.Name = "_mnuExit";
			this._mnuExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this._mnuExit.Size = new System.Drawing.Size(175, 22);
			this._mnuExit.Text = "%Exit%";
			this._mnuExit.Click += new System.EventHandler(this._mnuExit_Click);
			// 
			// _mnuView
			// 
			this._mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mnuToolbars});
			this._mnuView.Name = "_mnuView";
			this._mnuView.Size = new System.Drawing.Size(64, 20);
			this._mnuView.Text = "%View%";
			// 
			// _mnuToolbars
			// 
			this._mnuToolbars.Enabled = false;
			this._mnuToolbars.Name = "_mnuToolbars";
			this._mnuToolbars.Size = new System.Drawing.Size(140, 22);
			this._mnuToolbars.Text = "%Toolbars%";
			// 
			// _mnuTools
			// 
			this._mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mnuOptions});
			this._mnuTools.Name = "_mnuTools";
			this._mnuTools.Size = new System.Drawing.Size(68, 20);
			this._mnuTools.Text = "%Tools%";
			// 
			// _mnuOptions
			// 
			this._mnuOptions.Name = "_mnuOptions";
			this._mnuOptions.Size = new System.Drawing.Size(145, 22);
			this._mnuOptions.Text = "%Options%...";
			this._mnuOptions.Click += new System.EventHandler(this._mnuOptions_Click);
			// 
			// _mnuHelp
			// 
			this._mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mnuAbout});
			this._mnuHelp.Name = "_mnuHelp";
			this._mnuHelp.Size = new System.Drawing.Size(64, 20);
			this._mnuHelp.Text = "%Help%";
			// 
			// _mnuAbout
			// 
			this._mnuAbout.Name = "_mnuAbout";
			this._mnuAbout.Size = new System.Drawing.Size(136, 22);
			this._mnuAbout.Text = "%About%...";
			this._mnuAbout.Click += new System.EventHandler(this._mnuAbout_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(795, 496);
			this.Controls.Add(this._toolStripContainer);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this._menuStrip;
			this.Name = "MainForm";
			this.Text = "gitter";
			this._toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
			this._toolStripContainer.BottomToolStripPanel.PerformLayout();
			this._toolStripContainer.ContentPanel.ResumeLayout(false);
			this._toolStripContainer.TopToolStripPanel.ResumeLayout(false);
			this._toolStripContainer.TopToolStripPanel.PerformLayout();
			this._toolStripContainer.ResumeLayout(false);
			this._toolStripContainer.PerformLayout();
			this._statusStrip.ResumeLayout(false);
			this._statusStrip.PerformLayout();
			this._menuStrip.ResumeLayout(false);
			this._menuStrip.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStripContainer _toolStripContainer;
		private System.Windows.Forms.MenuStrip _menuStrip;
		private System.Windows.Forms.ToolStripMenuItem _mnuRepository;
		private System.Windows.Forms.ToolStripMenuItem _mnuExit;
		private System.Windows.Forms.ToolStripMenuItem _mnuHelp;
		private System.Windows.Forms.ToolStripMenuItem _mnuAbout;
		private System.Windows.Forms.ToolStripMenuItem _mnuTools;
		private System.Windows.Forms.ToolStripMenuItem _mnuOptions;
		private System.Windows.Forms.ToolStripMenuItem _mnuOpenRepository;
		private System.Windows.Forms.ToolStripMenuItem _mnuRecentRepositories;
		private System.Windows.Forms.ToolStripMenuItem _mnuDummy;
		private System.Windows.Forms.StatusStrip _statusStrip;
		private System.Windows.Forms.ToolStripMenuItem _mnuView;
		private System.Windows.Forms.ToolStripStatusLabel _statusSeparator;
		private System.Windows.Forms.ToolStripMenuItem _mnuToolbars;
		private Framework.Controls.ViewDockGrid _toolDockGrid;
	}
}
