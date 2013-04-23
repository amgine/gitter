namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Configuration;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	partial class ContributorsView : GitViewBase
	{
		private ContributorsToolBar _toolbar;

		public ContributorsView(IDictionary<string, object> parameters, GuiProvider gui)
			: base(Guids.ContributorsViewGuid, gui, parameters)
		{
			InitializeComponent();

			Text = Resources.StrContributors;

			_lstUsers.Text = Resources.StrsNoContributorsToDisplay;
			_lstUsers.PreviewKeyDown += OnKeyDown;

			AddTopToolStrip(_toolbar = new ContributorsToolBar(this));
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgUser"]; }
		}

		protected override void AttachToRepository(Repository repository)
		{
			_lstUsers.Load(repository);
		}

		protected override void DetachFromRepository(Repository repository)
		{
			_lstUsers.Load(null);
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
					using(this.ChangeCursor(Cursors.WaitCursor))
					{
						Repository.Users.Refresh();
					}
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
			var listNode = section.GetCreateSection("UsersList");
			_lstUsers.SaveViewTo(listNode);
		}

		protected override void LoadMoreViewFrom(Section section)
		{
			base.LoadMoreViewFrom(section);
			var listNode = section.TryGetSection("UsersList");
			if(listNode != null)
			{
				_lstUsers.LoadViewFrom(listNode);
			}
		}
	}
}
