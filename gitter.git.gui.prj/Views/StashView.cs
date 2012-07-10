namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;
	using System.Xml;

	using gitter.Framework.Controls;
	using gitter.Framework.Configuration;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	partial class StashView : GitViewBase
	{
		private StashToolbar _toolBar;

		public StashView(IDictionary<string, object> parameters, GuiProvider gui)
			: base(Guids.StashViewGuid, gui, parameters)
		{
			InitializeComponent();

			Text = Resources.StrStash;

			_lstStash.Text = Resources.StrNothingStashed;
			_lstStash.PreviewKeyDown += OnKeyDown;

			AddTopToolStrip(_toolBar = new StashToolbar(this));
		}

		protected override void AttachToRepository(Repository repository)
		{
			_lstStash.LoadData(repository);
		}

		protected override void DetachFromRepository(Repository repository)
		{
			_lstStash.LoadData(null);
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgStash"]; }
		}

		public override void RefreshContent()
		{
			if(InvokeRequired)
			{
				BeginInvoke(new MethodInvoker(RefreshContent));
			}
			else
			{
				if(Repository != null)
				{
					Repository.Stash.Refresh();
				}
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
			var listNode = section.GetCreateSection("StashList");
			_lstStash.SaveViewTo(listNode);
		}

		protected override void LoadMoreViewFrom(Section section)
		{
			base.LoadMoreViewFrom(section);
			var listNode = section.TryGetSection("StashList");
			if(listNode != null)
				_lstStash.LoadViewFrom(listNode);
		}
	}
}
