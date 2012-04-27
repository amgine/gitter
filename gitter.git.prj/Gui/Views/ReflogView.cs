namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Properties.Resources;

	internal partial class ReflogView : GitViewBase
	{
		public ReflogView(IDictionary<string, object> parameters, GuiProvider gui)
			: base(Guids.ReflogViewGuid, gui, parameters)
		{
			InitializeComponent();

			Text = Resources.StrReflog;

			_lstReflog.SelectionChanged += (sender, e) =>
				{
					ShowSelectedCommitDetails();
				};

			_lstReflog.ItemActivated += (sender, e) =>
				{
					ShowDiffTool(new RevisionChangesDiffSource(((ReflogRecordListItem)e.Item).DataContext.Revision));
				};

			_lstReflog.PreviewKeyDown += OnKeyDown;

			ApplyParameters(parameters);
		}

		public override bool IsDocument
		{
			get { return true; }
		}

		public override Image Image
		{
			get
			{
				if(_lstReflog.Reflog != null)
				{
					if(_lstReflog.Reflog.Reference.Type == ReferenceType.RemoteBranch)
						return CachedResources.Bitmaps["ImgViewReflogRemote"];
				}
				return CachedResources.Bitmaps["ImgViewReflog"];
			}
		}

		private void ShowSelectedCommitDetails()
		{
			switch(_lstReflog.SelectedItems.Count)
			{
				case 1:
					{
						var item = _lstReflog.SelectedItems[0] as ReflogRecordListItem;
						if(item != null)
						{
							ShowContextualDiffTool(new RevisionChangesDiffSource(item.DataContext.Revision));
						}
					}
					break;
			}
		}

		public override void RefreshContent()
		{
			if(_lstReflog.Reflog != null)
			{
				_lstReflog.Reflog.Refresh();
			}
		}

		public override void ApplyParameters(IDictionary<string, object> parameters)
		{
			base.ApplyParameters(parameters);

			var reflog = (Reflog)parameters["reflog"];
			_lstReflog.Load(reflog);
			Text = Resources.StrReflog + ": " + reflog.Reference.Name;
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
	}
}
