namespace gitter.GitLab.Gui;

partial class PipelinesView
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
			DataSource = null; 
			if(components != null)
			{
				components.Dispose();
			}
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
		this._lstPipelines = new gitter.GitLab.Gui.ListBoxes.PipelinesListBox();
		this.SuspendLayout();
		// 
		// _lstIssues
		// 
		this._lstPipelines.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
		this._lstPipelines.Location = new System.Drawing.Point(0, 0);
		this._lstPipelines.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this._lstPipelines.Name = "_lstPipelines";
		this._lstPipelines.Size = new System.Drawing.Size(615, 407);
		this._lstPipelines.TabIndex = 0;
		// 
		// IssuesView
		// 
		this.Controls.Add(this._lstPipelines);
		this.Name = "PipelinesView";
		this.Size = new System.Drawing.Size(615, 407);
		this.ResumeLayout(false);
	}

	#endregion

	private ListBoxes.PipelinesListBox _lstPipelines;
}
