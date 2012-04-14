namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class HistoryToolbar : ToolStrip
	{
		#region Data

		private readonly HistoryView _view;
		private ToolStripButton _btnShowDetails;
		private ToolStripButton _btnRefresh;
		private ToolStripButton _btnDateOrder;
		private ToolStripButton _btnTopoOrder;
		private ToolStripDropDownButton _btnLimit;
		private ToolStripDropDownButton _btnFilter;
		private HistoryFilterDropDown _filterDropDown;

		#endregion

		/// <summary>Initializes a new instance of the <see cref="HistoryToolbar"/> class.</summary>
		/// <param name="view">Host history view.</param>
		public HistoryToolbar(HistoryView view)
		{
			_view = view;

			_view.LogOptionsChanged += OnLogOptionsChanged;
			_view.RepositoryChanged += OnRepositoryChanged;

			Items.Add(_btnRefresh = new ToolStripButton(Resources.StrRefresh, CachedResources.Bitmaps["ImgRefresh"],
				(sender, e) =>
				{
					_view.RefreshContent();
				})
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
			});

			Items.Add(new ToolStripSeparator());

			Items.Add(_btnDateOrder = new ToolStripButton(Resources.StrDateOrder, CachedResources.Bitmaps["ImgDateOrder"],
				(sender, e) =>
				{
					_view.LogOptions.Order = AccessLayer.RevisionQueryOrder.DateOrder;
				})
			{
				Checked = _view.LogOptions.Order == AccessLayer.RevisionQueryOrder.DateOrder,
				DisplayStyle = ToolStripItemDisplayStyle.Image,
			});

			Items.Add(_btnTopoOrder = new ToolStripButton(Resources.StrTopoOrder, CachedResources.Bitmaps["ImgTopoOrder"],
				(sender, e) =>
				{
					_view.LogOptions.Order = AccessLayer.RevisionQueryOrder.TopoOrder;
				})
			{
				Checked = _view.LogOptions.Order == AccessLayer.RevisionQueryOrder.TopoOrder,
				DisplayStyle = ToolStripItemDisplayStyle.Image,
			});

			Items.Add(new ToolStripSeparator());

			Items.Add(_btnFilter = new ToolStripDropDownButton(Resources.StrFilter, CachedResources.Bitmaps["ImgFilter"])
				{
					DropDown = new gitter.Framework.Controls.Popup(
						_filterDropDown = new HistoryFilterDropDown()
						{
							LogOptions = _view.LogOptions,
							Repository = _view.Repository,
						}),
				});

			Items.Add(_btnLimit = new ToolStripDropDownButton());
			_btnLimit.DropDownItems.AddRange(
				new[]
				{
					new ToolStripMenuItem(Resources.StrlUnlimited, null, OnLimitOptionClick) { Tag = 0 },
					new ToolStripMenuItem( "100 " + Resources.StrlCommits, null, OnLimitOptionClick) { Tag = 100 },
					new ToolStripMenuItem( "500 " + Resources.StrlCommits, null, OnLimitOptionClick) { Tag = 500 },
					new ToolStripMenuItem("1000 " + Resources.StrlCommits, null, OnLimitOptionClick) { Tag = 1000 },
					new ToolStripMenuItem("2000 " + Resources.StrlCommits, null, OnLimitOptionClick) { Tag = 2000 },
					new ToolStripMenuItem("5000 " + Resources.StrlCommits, null, OnLimitOptionClick) { Tag = 5000 },
				});
			UpdateLimitButtonText();

			Items.Add(_btnShowDetails = new ToolStripButton(Resources.StrAutoShowDiff, CachedResources.Bitmaps["ImgDiff"],
				(sender, e) =>
				{
					var btn = (ToolStripButton)sender;
					btn.Checked = !btn.Checked;
					_view.ShowDetails = btn.Checked;
				})
			{
				Checked = _view.ShowDetails,
				DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
				Alignment = ToolStripItemAlignment.Right,
			});
		}

		private void OnRepositoryChanged(object sender, EventArgs e)
		{
			_filterDropDown.Repository = _view.Repository;
		}

		private void OnLogOptionsChanged(object sender, EventArgs e)
		{
			_filterDropDown.LogOptions = _view.LogOptions;
			_btnDateOrder.Checked = _view.LogOptions.Order == AccessLayer.RevisionQueryOrder.DateOrder;
			_btnTopoOrder.Checked = _view.LogOptions.Order == AccessLayer.RevisionQueryOrder.TopoOrder;
			UpdateLimitButtonText();
		}

		private void OnLimitOptionClick(object sender, EventArgs e)
		{
			_view.LogOptions.MaxCount = (int)((ToolStripItem)sender).Tag;
		}

		private void UpdateLimitButtonText()
		{
			foreach(ToolStripMenuItem item in _btnLimit.DropDownItems)
			{
				if(item.Tag is int && (int)item.Tag == _view.LogOptions.MaxCount)
				{
					_btnLimit.Text = item.Text;
					item.Checked = true;
				}
				else
				{
					item.Checked = false;
				}
			}
		}

		public ToolStripButton RefreshButton
		{
			get { return _btnRefresh; }
		}

		public ToolStripButton ShowDiffButton
		{
			get { return _btnShowDetails; }
		}
	}
}
