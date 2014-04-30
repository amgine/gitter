#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Configuration;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	partial class PathHistoryView : HistoryViewBase
	{
		#region Data

		private PathHistoryToolbar _toolBar;

		#endregion

		#region .ctor

		public PathHistoryView(GuiProvider gui)
			: base(Guids.PathHistoryViewGuid, gui)
		{
			RemoveGraphColumn();
			RevisionListBox.PreviewKeyDown += OnKeyDown;
			AddTopToolStrip(_toolBar = new PathHistoryToolbar(this));
		}

		#endregion

		private void RemoveGraphColumn()
		{
			for(int i = 0; i < RevisionListBox.Columns.Count; ++i)
			{
				if(RevisionListBox.Columns[i].Id == (int)ColumnId.Graph)
				{
					RevisionListBox.Columns.RemoveAt(i);
					break;
				}
			}
		}

		protected override void AttachViewModel(object viewModel)
		{
			base.AttachViewModel(viewModel);

			var vm = viewModel as HistoryViewModel;
			if(vm != null)
			{
				LogSource = vm.LogSource as PathLogSource;
				if(LogSource != null)
				{
					Text = Resources.StrHistory + ": " + LogSource.ToString();
				}
				else
				{
					Text = Resources.StrHistory;
				}
			}
		}

		protected override void DetachViewModel(object viewModel)
		{
			base.DetachViewModel(viewModel);

			var vm = viewModel as HistoryViewModel;
			if(vm != null)
			{
				LogSource = null;
				Text      = Resources.StrHistory;
			}
		}

		protected new PathLogSource LogSource
		{
			get { return (PathLogSource)base.LogSource; }
			set { base.LogSource = value; }
		}

		public override Image Image
		{
			get
			{
				if(LogSource != null)
				{
					if(LogSource.Path.EndsWith('/'))
					{
						return CachedResources.Bitmaps["ImgFolderHistory"];
					}
				}
				return CachedResources.Bitmaps["ImgFileHistory"];
			}
		}

		protected override void DetachFromRepository(Repository repository)
		{
			base.DetachFromRepository(repository);

			LogSource = null;
			LogOptions.Reset();
		}

		/// <summary>Refreshes the content.</summary>
		public override void RefreshContent()
		{
			if(InvokeRequired)
			{
				BeginInvoke(new MethodInvoker(ReloadRevisionLog));
			}
			else
			{
				ReloadRevisionLog();
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
				case Keys.F:
					if(e.Modifiers == Keys.Control)
					{
						ShowSearchToolBar();
						e.IsInputKey = true;
					}
					break;
				case Keys.F5:
					if(e.Modifiers == Keys.None)
					{
						RefreshContent();
					}
					break;
			}
		}

		protected override void SaveMoreViewTo(Section section)
		{
			base.SaveMoreViewTo(section);
			var layoutNode = section.GetCreateSection("Layout");
			layoutNode.SetValue("ShowDetails", ShowDetails);
			var listNode = section.GetCreateSection("RevisionList");
			RevisionListBox.SaveViewTo(listNode);
		}

		protected override void LoadMoreViewFrom(Section section)
		{
			base.LoadMoreViewFrom(section);
			var layoutNode = section.TryGetSection("Layout");
			if(layoutNode != null)
			{
				//_toolbar.ShowDiffButton.Checked = ShowDetails = layoutNode.GetValue("ShowDetails", ShowDetails);
			}
			var listNode = section.TryGetSection("RevisionList");
			if(listNode != null)
			{
				RevisionListBox.LoadViewFrom(listNode);
			}
		}
	}
}
