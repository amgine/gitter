namespace gitter.Git.Gui.Views
{
	partial class TreeView
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
			this._treeContent = new gitter.Git.Gui.Controls.TreeListBox();
			this._splitContainer = new System.Windows.Forms.SplitContainer();
			this._directoryTree = new gitter.Git.Gui.Controls.TreeListBox();
			((System.ComponentModel.ISupportInitialize)(this._splitContainer)).BeginInit();
			this._splitContainer.Panel1.SuspendLayout();
			this._splitContainer.Panel2.SuspendLayout();
			this._splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// _treeContent
			// 
			this._treeContent.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._treeContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this._treeContent.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._treeContent.Location = new System.Drawing.Point(0, 0);
			this._treeContent.Margin = new System.Windows.Forms.Padding(0);
			this._treeContent.Name = "_treeContent";
			this._treeContent.ShowTreeLines = true;
			this._treeContent.Size = new System.Drawing.Size(366, 362);
			this._treeContent.TabIndex = 1;
			this._treeContent.Text = "Tree is empty";
			this._treeContent.ItemActivated += new System.EventHandler<gitter.Framework.Controls.ItemEventArgs>(this.OnItemActivated);
			// 
			// _splitContainer
			// 
			this._splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this._splitContainer.Location = new System.Drawing.Point(0, 0);
			this._splitContainer.Name = "_splitContainer";
			// 
			// _splitContainer.Panel1
			// 
			this._splitContainer.Panel1.Controls.Add(this._directoryTree);
			// 
			// _splitContainer.Panel2
			// 
			this._splitContainer.Panel2.Controls.Add(this._treeContent);
			this._splitContainer.Size = new System.Drawing.Size(555, 362);
			this._splitContainer.SplitterDistance = 185;
			this._splitContainer.TabIndex = 2;
			// 
			// _directoryTree
			// 
			this._directoryTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._directoryTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this._directoryTree.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._directoryTree.Location = new System.Drawing.Point(0, 0);
			this._directoryTree.Margin = new System.Windows.Forms.Padding(0);
			this._directoryTree.Name = "_directoryTree";
			this._directoryTree.ShowTreeLines = true;
			this._directoryTree.Size = new System.Drawing.Size(185, 362);
			this._directoryTree.TabIndex = 2;
			this._directoryTree.Text = "Tree is empty";
			// 
			// TreeView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._splitContainer);
			this.Name = "TreeView";
			this._splitContainer.Panel1.ResumeLayout(false);
			this._splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._splitContainer)).EndInit();
			this._splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private gitter.Git.Gui.Controls.TreeListBox _treeContent;
		private System.Windows.Forms.SplitContainer _splitContainer;
		private Controls.TreeListBox _directoryTree;
	}
}
