namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	partial class GitView : GitViewBase
	{
		public GitView(IDictionary<string, object> parameters, GuiProvider gui)
			: base(Guids.GitViewGuid, parameters, gui)
		{
			InitializeComponent();

			Text = Resources.StrGit;
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgGit"]; }
		}

		public override bool IsDocument
		{
			get { return true; }
		}
	}
}
