namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework.Configuration;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	partial class SubmodulesView : GitViewBase
	{
		private readonly SubmodulesToolbar _toolBar;

		#region .ctor

		public SubmodulesView(IDictionary<string, object> parameters, GuiProvider gui)
			: base(Guids.SubmodulesViewGuid, gui, parameters)
		{
			InitializeComponent();

			Text = Resources.StrSubmodules;
			_lstSubmodules.Text = Resources.StrsNoSubmodules;
			_lstSubmodules.PreviewKeyDown += OnKeyDown;

			AddTopToolStrip(_toolBar = new SubmodulesToolbar(this));
		}

		#endregion

		#region Overrides

		protected override void AttachToRepository(Repository repository)
		{
			_lstSubmodules.Load(repository);
			base.AttachToRepository(repository);
		}

		protected override void DetachFromRepository(Repository repository)
		{
			_lstSubmodules.Load(null);
			base.DetachFromRepository(repository);
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgSubmodule"]; }
		}

		public override void RefreshContent()
		{
			if(Repository != null)
			{
				Repository.Submodules.Refresh();
			}
		}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			OnKeyDown(this, e);
			base.OnPreviewKeyDown(e);
		}

		private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.F5:
					RefreshContent();
					e.IsInputKey = true;
					break;
			}
		}

		protected override void SaveMoreViewTo(Section section)
		{
			base.SaveMoreViewTo(section);
			var listNode = section.GetCreateSection("SubmodulesList");
			_lstSubmodules.SaveViewTo(listNode);
		}

		protected override void LoadMoreViewFrom(Section section)
		{
			base.LoadMoreViewFrom(section);
			var listNode = section.TryGetSection("SubmodulesList");
			if(listNode != null)
			{
				_lstSubmodules.LoadViewFrom(listNode);
			}
		}

		#endregion
	}
}
