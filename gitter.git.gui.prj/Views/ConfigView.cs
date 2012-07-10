namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Configuration;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	partial class ConfigView : GitViewBase
	{
		private readonly ConfigToolBar _toolBar;

		public ConfigView(IDictionary<string, object> parameters, GuiProvider gui)
			: base(Guids.ConfigViewGuid, gui, parameters)
		{
			InitializeComponent();

			_lstConfig.PreviewKeyDown += OnKeyDown;

			Text = Resources.StrConfig;

			AddTopToolStrip(_toolBar = new ConfigToolBar(this));
		}

		public override bool IsDocument
		{
			get { return true; }
		}

		protected override void AttachToRepository(Repository repository)
		{
			_lstConfig.LoadData(repository);
		}

		protected override void DetachFromRepository(Repository repository)
		{
			_lstConfig.Clear();
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgConfig"]; }
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
					Cursor = Cursors.WaitCursor;
					Repository.Configuration.Refresh();
					Cursor = Cursors.Default;
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
			var listSection = section.GetCreateSection("ConfigParameterList");
			_lstConfig.SaveViewTo(listSection);
		}

		protected override void LoadMoreViewFrom(Section section)
		{
			base.LoadMoreViewFrom(section);
			var listSection = section.TryGetSection("ConfigParameterList");
			if(listSection != null)
			{
				_lstConfig.LoadViewFrom(listSection);
			}
		}
	}
}
