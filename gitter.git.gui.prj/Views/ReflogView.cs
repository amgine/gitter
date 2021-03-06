﻿#region Copyright Notice
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
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	internal partial class ReflogView : GitViewBase, ISearchableView<ReflogSearchOptions>
	{
		#region Data

		private readonly ReflogToolbar _toolbar;
		private ISearchToolBarController _searchToolbar;
		private Reflog _reflog;
		private Reference _reference;

		#endregion

		#region .ctor

		public ReflogView(GuiProvider gui)
			: base(Guids.ReflogViewGuid, gui)
		{
			InitializeComponent();

			_lstReflog.SelectionChanged += OnReflogSelectionChanged;
			_lstReflog.ItemActivated += OnReflogItemActivated;
			_lstReflog.PreviewKeyDown += OnKeyDown;

			Search = new ReflogSearch<ReflogSearchOptions>(_lstReflog);
			_searchToolbar = CreateSearchToolbarController<ReflogView, ReflogSearchToolBar, ReflogSearchOptions>(this);

			AddTopToolStrip(_toolbar = new ReflogToolbar(this));
		}

		#endregion

		#region Properties

		public override bool IsDocument => true;

		private static readonly IImageProvider _branchReflogIcon = new ScaledImageProvider(CachedResources.ScaledBitmaps, @"branch.reflog");
		private static readonly IImageProvider _remoteReflogIcon = new ScaledImageProvider(CachedResources.ScaledBitmaps, @"rbranch.reflog");

		public override IImageProvider ImageProvider
			=> Reflog is not null && Reflog.Reference.Type == ReferenceType.RemoteBranch
				? _remoteReflogIcon
				: _branchReflogIcon;

		public Reflog Reflog
		{
			get => _reflog;
			private set
			{
				if(_reflog != value)
				{
					_reflog = value;
					_lstReflog.Load(value);
					Reference = value?.Reference;
				}
			}
		}

		public Reference Reference
		{
			get => _reference;
			private set
			{
				if(_reference != value)
				{
					if(_reference != null)
					{
						if(_reference is Branch branch)
						{
							branch.Renamed -= OnBranchRenamed;
						}
					}
					_reference = value;
					if(_reference != null)
					{
						if(_reference is Branch branch)
						{
							branch.Renamed += OnBranchRenamed;
						}
					}
					UpdateText();
				}
			}
		}

		public ISearch<ReflogSearchOptions> Search { get; }

		public bool SearchToolBarVisible
		{
			get => _searchToolbar.IsVisible;
			set => _searchToolbar.IsVisible = value;
		}

		#endregion

		private void ShowSelectedCommitDetails()
		{
			switch(_lstReflog.SelectedItems.Count)
			{
				case 1:
					{
						if(_lstReflog.SelectedItems[0] is ReflogRecordListItem item)
						{
							ShowContextualDiffView(item.DataContext.Revision.GetDiffSource());
						}
					}
					break;
			}
		}

		public override void RefreshContent()
		{
			if(Reflog != null)
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					Reflog.Refresh();
				}
			}
		}

		protected override void AttachViewModel(object viewModel)
		{
			base.AttachViewModel(viewModel);

			if(viewModel is ReflogViewModel vm)
			{
				Reflog = vm.Reflog;
			}
		}

		protected override void DetachViewModel(object viewModel)
		{
			base.DetachViewModel(viewModel);

			if(viewModel is ReflogViewModel)
			{
				Reflog = null;
			}
		}

		private void UpdateText()
			=> Text = Reference != null
				? Resources.StrReflog + ": " + Reference.Name
				: Resources.StrReflog;

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			OnKeyDown(this, e);
			base.OnPreviewKeyDown(e);
		}

		#region Event Handlers

		private void OnReflogSelectionChanged(object sender, EventArgs e)
		{
			ShowSelectedCommitDetails();
		}

		private void OnReflogItemActivated(object sender, ItemEventArgs e)
		{
			if(e.Item is ReflogRecordListItem item)
			{
				var reflogRecord = item.DataContext;
				ShowDiffView(reflogRecord.Revision.GetDiffSource());
			}
		}

		private void OnBranchRenamed(object sender, NameChangeEventArgs e)
		{
			if(!IsDisposed)
			{
				if(InvokeRequired)
				{
					try
					{
						BeginInvoke(new MethodInvoker(UpdateText));
					}
					catch(ObjectDisposedException)
					{
					}
				}
				else
				{
					UpdateText();
				}
			}
		}

		private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			Assert.IsNotNull(e);

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

		#endregion
	}
}
