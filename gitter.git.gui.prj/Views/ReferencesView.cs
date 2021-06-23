#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Configuration;
	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	partial class ReferencesView : GitViewBase, ISearchableView<ReferencesSearchOptions>
	{
		private ReferencesToolbar _toolbar;
		private ISearchToolBarController _searchToolbar;

		public ReferencesView(GuiProvider gui)
			: base(Guids.ReferencesViewGuid, gui)
		{
			InitializeComponent();
			_lstReferences.Columns.ShowAll((c) => c.Id != (int)ColumnId.TreeHash);
			_lstReferences.PreviewKeyDown += OnKeyDown;

			Text   = Resources.StrReferences;
			Search = new ReferencesSearch(_lstReferences);

			_searchToolbar = CreateSearchToolbarController<ReferencesView, ReferencesSearchToolBar, ReferencesSearchOptions>(this);

			AddTopToolStrip(_toolbar = new ReferencesToolbar(this));
		}

		public override IImageProvider ImageProvider { get; } = new ScaledImageProvider(CachedResources.ScaledBitmaps, @"branch");

		protected override void AttachToRepository(Repository repository)
		{
			Assert.IsNotNull(repository);

			_lstReferences.LoadData(Repository);
			_lstReferences.Items[0].IsExpanded = true;
			_lstReferences.Items[1].ExpandAll();
			_lstReferences.Items[2].IsExpanded = true;
		}

		protected override void DetachFromRepository(Repository repository)
		{
			Assert.IsNotNull(repository);

			_lstReferences.LoadData(null);
		}

		public IRevisionPointer SelectedReference
		{
			get
			{
				if(_lstReferences.SelectedItems.Count == 0) return null;
				return (_lstReferences.SelectedItems[0] as IRevisionPointerListItem)?.RevisionPointer;
			}
		}

		public override void RefreshContent()
		{
			if(InvokeRequired)
			{
				try
				{
					BeginInvoke(new MethodInvoker(RefreshContent));
				}
				catch
				{
				}
			}
			else
			{
				if(IsDisposed) return;
				if(Repository is not null)
				{
					using(this.ChangeCursor(Cursors.WaitCursor))
					{
						_lstReferences.BeginUpdate();
						Repository.Refs.Refresh();
						_lstReferences.EndUpdate();
					}
				}
			}
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
					e.IsInputKey = true;
					break;
			}
		}

		public ISearch<ReferencesSearchOptions> Search { get; }

		public bool SearchToolBarVisible
		{
			get => _searchToolbar.IsVisible;
			set => _searchToolbar.IsVisible = value;
		}

		protected override void SaveMoreViewTo(Section section)
		{
			base.SaveMoreViewTo(section);
			var listNode = section.GetCreateSection("ReferenceList");
			_lstReferences.SaveViewTo(listNode);
		}

		protected override void LoadMoreViewFrom(Section section)
		{
			base.LoadMoreViewFrom(section);
			var listNode = section.TryGetSection("ReferenceList");
			if(listNode is not null)
			{
				_lstReferences.LoadViewFrom(listNode);
			}
		}
	}
}
