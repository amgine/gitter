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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;
	using gitter.Framework.Controls;

	sealed class SplitDiffBinding : AsyncDataBinding<Diff>
	{
		#region Data

		private readonly IDiffSource _diffSource;
		private readonly DiffViewer _diffViewerHeaders;
		private readonly DiffViewer _diffViewerFiles;
		private DiffOptions _diffOptions;
		private readonly List<FileDiffPanel> _allDiffPanels;
		private readonly FlowProgressPanel _progressPanel;
		private int _scrollPosAfterReload;

		#endregion

		#region .ctor

		public SplitDiffBinding(IDiffSource diffSource, DiffViewer diffViewerHeaders, DiffViewer diffViewerFiles, DiffOptions diffOptions)
		{
			Verify.Argument.IsNotNull(diffSource, "diffSource");
			Verify.Argument.IsNotNull(diffViewerHeaders, "diffViewerHeaders");
			Verify.Argument.IsNotNull(diffViewerFiles, "diffViewerFiles");
			Verify.Argument.IsNotNull(diffOptions, "diffOptions");

			_diffSource = diffSource;
			_diffViewerHeaders = diffViewerHeaders;
			_diffViewerFiles = diffViewerFiles;
			_diffOptions = diffOptions;

			_allDiffPanels = new List<FileDiffPanel>();
			_progressPanel = new FlowProgressPanel();
			Progress = _progressPanel.ProgressMonitor;
		}

		#endregion

		#region Properties

		public IDiffSource DiffSource
		{
			get { return _diffSource; }
		}

		public DiffViewer DiffViewerHeaders
		{
			get { return _diffViewerHeaders; }
		}

		public DiffViewer DiffViewerFiles
		{
			get { return _diffViewerFiles; }
		}

		public DiffOptions DiffOptions
		{
			get { return _diffOptions; }
			set
			{
				Verify.Argument.IsNotNull(value, "value");

				_diffOptions = value;
			}
		}

		#endregion

		#region Methods

		private void AddSourceSpecificPanels()
		{
			var revisionSource = DiffSource as IRevisionDiffSource;
			if(revisionSource != null)
			{
				DiffViewerHeaders.Panels.Add(new RevisionHeaderPanel() { Revision = revisionSource.Revision.Dereference() });
				DiffViewerHeaders.Panels.Add(new FlowPanelSeparator() { SeparatorStyle = FlowPanelSeparatorStyle.Line });
				return;
			}
			var indexSource = DiffSource as IIndexDiffSource;
			if(indexSource != null && !indexSource.Cached)
			{
				var panel = new UntrackedFilesPanel(indexSource.Repository.Status);
				if(panel.Count != 0)
				{
					DiffViewerHeaders.Panels.Add(panel);
					DiffViewerHeaders.Panels.Add(new FlowPanelSeparator() { Height = 5 });
				}
				return;
			}
		}

		protected override Task<Diff> FetchDataAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.State.IsFalse(IsDisposed, "DiffBinding is disposed.");

			if(!DiffViewerHeaders.Created)
			{
				DiffViewerHeaders.CreateControl();
			}
			if(!DiffViewerFiles.Created)
			{
				DiffViewerFiles.CreateControl();
			}
			_scrollPosAfterReload = DiffViewerFiles.VScrollPos;
			DiffViewerFiles.Panels.Clear();
			DiffViewerHeaders.BeginUpdate();
			DiffViewerHeaders.Panels.Clear();
			DiffViewerHeaders.ScrollToTopLeft();
			AddSourceSpecificPanels();
			DiffViewerHeaders.Panels.Add(_progressPanel);
			DiffViewerHeaders.EndUpdate();
			_allDiffPanels.Clear();
			return DiffSource.GetDiffAsync(DiffOptions, progress, cancellationToken);
		}

		protected override void OnFetchCompleted(Diff diff)
		{
			if(DiffViewerHeaders.IsDisposed || DiffViewerFiles.IsDisposed)
			{
				return;
			}

			DiffViewerFiles.BeginUpdate();
			_allDiffPanels.Clear();
			_progressPanel.Remove();
			if(diff != null)
			{
				FlowPanelSeparator separator = null;
				var changedFilesPanel = new ChangedFilesPanel() { Diff = diff };
				changedFilesPanel.FileNavigationRequested +=
					(s, e) =>
					{
						foreach(var panel in DiffViewerFiles.Panels)
						{
							var diffpanel = panel as FileDiffPanel;
							if(diffpanel != null && diffpanel.DiffFile == e.DiffFile)
							{
								diffpanel.ScrollIntoView();
								break;
							}
						}
					};
				changedFilesPanel.StatusFilterChanged += OnStatusFilterChanged;
				DiffViewerHeaders.Panels.Add(changedFilesPanel);
				foreach(var file in diff)
				{
					var fileDiffPanel = new FileDiffPanel(DiffSource.Repository, file, diff.Type);
					_allDiffPanels.Add(fileDiffPanel);
					DiffViewerFiles.Panels.Add(fileDiffPanel);
					DiffViewerFiles.Panels.Add(separator = new FlowPanelSeparator() { SeparatorStyle = FlowPanelSeparatorStyle.Simple });
				}
				if(separator != null)
				{
					separator.Height = 6;
				}
			}
			DiffViewerFiles.EndUpdate();
			if(_scrollPosAfterReload != 0)
			{
				DiffViewerFiles.BeginInvoke(new Action<int>(SetScrollPos), _scrollPosAfterReload);
			}
		}

		private void SetScrollPos(int scrollPos)
		{
			if(scrollPos > DiffViewerFiles.MaxVScrollPos)
			{
				scrollPos = DiffViewerFiles.MaxVScrollPos;
			}
			DiffViewerFiles.VScrollBar.Value = scrollPos;
		}

		private void OnStatusFilterChanged(object sender, EventArgs e)
		{
			var changedFilesPanel = (ChangedFilesPanel)sender;
			var index = 0;
			DiffViewerFiles.BeginUpdate();
			if(index < DiffViewerFiles.Panels.Count)
			{
				DiffViewerFiles.Panels.RemoveRange(index, DiffViewerFiles.Panels.Count - index);
			}
			FlowPanelSeparator separator = null;
			for(int i = 0; i < _allDiffPanels.Count; ++i)
			{
				if((_allDiffPanels[i].DiffFile.Status & changedFilesPanel.StatusFilter) != FileStatus.Unknown)
				{
					DiffViewerFiles.Panels.Add(_allDiffPanels[i]);
					DiffViewerFiles.Panels.Add(separator = new FlowPanelSeparator() { SeparatorStyle = FlowPanelSeparatorStyle.Simple });
				}
			}
			if(separator != null)
			{
				separator.Height = 6;
			}
			DiffViewerFiles.EndUpdate();
		}

		protected override void OnFetchFailed(Exception exception)
		{
			if(DiffViewerHeaders.IsDisposed || DiffViewerFiles.IsDisposed)
			{
				return;
			}
			DiffViewerHeaders.BeginUpdate();
			_progressPanel.Remove();
			if(exception != null && !string.IsNullOrWhiteSpace(exception.Message))
			{
				DiffViewerHeaders.Panels.Add(new FlowProgressPanel() { Message = exception.Message });
			}
			DiffViewerHeaders.EndUpdate();
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(!DiffViewerHeaders.IsDisposed)
				{
					DiffViewerHeaders.Panels.Clear();
				}
				if(!DiffViewerFiles.IsDisposed)
				{
					DiffViewerFiles.Panels.Clear();
				}
			}
			base.Dispose(disposing);
		}

		#endregion
	}
}
