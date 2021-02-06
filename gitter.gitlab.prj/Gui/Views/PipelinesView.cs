#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.GitLab.Gui
{
	using System;
	using System.Drawing;
	using System.Globalization;
	using System.Windows.Forms;
	
	using gitter.Framework;
	using gitter.Framework.Configuration;
	using gitter.Framework.Controls;
	using gitter.GitLab.Api;
	using gitter.GitLab.Gui.ListBoxes;

	using Resources = gitter.GitLab.Properties.Resources;

	partial class PipelinesView : GitLabViewBase, ISearchableView<PipelinesSearchOptions>
	{
		#region Data

		private readonly PipelinesToolbar _toolbar;
		private PipelinesListBinding _dataSource;
		private ISearchToolBarController _searchToolbar;

		#endregion

		#region .ctor

		public PipelinesView(IWorkingEnvironment environment)
			: base(Guids.PipelinesViewGuid, environment)
		{
			InitializeComponent();
			_lstPipelines.Text = Resources.StrsNoPipelinesToDisplay;

			Text   = Resources.StrPipelines;
			Search = new PipelinesSearch(_lstPipelines);

			_searchToolbar = CreateSearchToolbarController<PipelinesView, PipelinesSearchToolBar, PipelinesSearchOptions>(this);

			AddTopToolStrip(_toolbar = new PipelinesToolbar(this));

			_lstPipelines.ItemActivated  += OnItemActivated;
			_lstPipelines.PreviewKeyDown += OnKeyDown;

			_toolbar.RefreshButton.Click += (s, e) => DataSource?.ReloadData();
		}

		#endregion

		#region Properties

		public override Image Image => CachedResources.Bitmaps["ImgPipelines"];

		private PipelinesListBinding DataSource
		{
			get => _dataSource;
			set
			{
				if(_dataSource != value)
				{
					_dataSource?.Dispose();
					_dataSource = value;
					_dataSource?.ReloadData();
				}
			}
		}

		#endregion

		#region Methods

		protected override void OnContextDetached(GitLabServiceContext serviceContext)
		{
			DataSource = default;
		}

		protected override void OnContextAttached(GitLabServiceContext serviceContext)
		{
			DataSource = new PipelinesListBinding(serviceContext, _lstPipelines);
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			if(e.Item is PipelineListItem item)
			{
				ShowPipelineDetails(item.DataContext);
			}
		}

		private void ShowPipelineDetails(Pipeline pipeline)
		{
			Assert.IsNotNull(pipeline);

			Utility.OpenUrl(pipeline.WebUrl.ToString());
		}

		protected override void SaveMoreViewTo(Section section)
		{
			var listNode = section.GetCreateSection("PipelinesList");
			_lstPipelines.SaveViewTo(listNode);
		}

		protected override void LoadMoreViewFrom(Section section)
		{
			var listNode = section.TryGetSection("PipelinesList");
			if(listNode != null)
			{
				_lstPipelines.LoadViewFrom(listNode);
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
				case Keys.F when e.Modifiers == Keys.Control:
					_searchToolbar.Show();
					e.IsInputKey = true;
					break;
				case Keys.F5:
					RefreshContent();
					break;
			}
		}

		public void RefreshContent() => DataSource?.ReloadData();

		#endregion

		#region ISearchableView

		public ISearch<PipelinesSearchOptions> Search { get; }

		public bool SearchToolBarVisible
		{
			get => _searchToolbar.IsVisible;
			set => _searchToolbar.IsVisible = value;
		}

		#endregion
	}
}
