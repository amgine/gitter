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

namespace gitter.Git.Gui.Controls;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;
using gitter.Framework.Controls;

sealed class DiffBinding : AsyncDataBinding<Diff>
{
	#region Data

	private DiffOptions _diffOptions;
	private readonly List<FileDiffPanel> _allDiffPanels;
	private readonly FlowProgressPanel _progressPanel;
	private int _scrollPosAfterReload;

	#endregion

	#region .ctor

	public DiffBinding(IDiffSource diffSource, DiffViewer diffViewer, DiffOptions diffOptions)
	{
		Verify.Argument.IsNotNull(diffSource);
		Verify.Argument.IsNotNull(diffViewer);
		Verify.Argument.IsNotNull(diffOptions);

		DiffSource = diffSource;
		DiffViewer = diffViewer;
		_diffOptions = diffOptions;

		_allDiffPanels = new List<FileDiffPanel>();
		_progressPanel = new FlowProgressPanel();
		Progress = _progressPanel.ProgressMonitor;
	}

	#endregion

	#region Properties

	public IDiffSource DiffSource { get; }

	public DiffViewer DiffViewer { get; }

	public DiffOptions DiffOptions
	{
		get => _diffOptions;
		set
		{
			Verify.Argument.IsNotNull(value);

			_diffOptions = value;
		}
	}

	#endregion

	#region Methods

	private void AddSourceSpecificPanels()
	{
		var panels = DiffHeaderPanelsProvider.GetSourceSpecificPanels(DiffSource);
		DiffViewer.Panels.AddRange(panels);
	}

	protected override Task<Diff> FetchDataAsync(
		IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
	{
		Verify.State.IsFalse(IsDisposed, "DiffBinding is disposed.");

		if(!DiffViewer.Created)
		{
			DiffViewer.CreateControl();
		}
		_scrollPosAfterReload = DiffViewer.VScrollPos;
		DiffViewer.BeginUpdate();
		DiffViewer.Panels.Clear();
		DiffViewer.ScrollToTopLeft();
		AddSourceSpecificPanels();
		DiffViewer.Panels.Add(_progressPanel);
		DiffViewer.EndUpdate();
		_allDiffPanels.Clear();
		return DiffSource.GetDiffAsync(DiffOptions, progress, cancellationToken);
	}

	protected override void OnFetchCompleted(Diff diff)
	{
		if(DiffViewer.IsDisposed)
		{
			return;
		}

		DiffViewer.BeginUpdate();
		_allDiffPanels.Clear();
		_progressPanel.Remove();
		if(diff is not null)
		{
			FlowPanelSeparator separator = null;
			var changedFilesPanel = new ChangedFilesPanel() { Diff = diff };
			changedFilesPanel.StatusFilterChanged += OnStatusFilterChanged;
			DiffViewer.Panels.Add(changedFilesPanel);
			DiffViewer.Panels.Add(new FlowPanelSeparator() { SeparatorStyle = FlowPanelSeparatorStyle.Line });
			foreach(var file in diff)
			{
				var fileDiffPanel = new FileDiffPanel(DiffSource.Repository, file, diff.Type);
				_allDiffPanels.Add(fileDiffPanel);
				DiffViewer.Panels.Add(fileDiffPanel);
				DiffViewer.Panels.Add(separator = new FlowPanelSeparator() { SeparatorStyle = FlowPanelSeparatorStyle.Simple });
			}
			if(separator != null)
			{
				separator.Height = 6;
			}
		}
		DiffViewer.EndUpdate();
		if(_scrollPosAfterReload != 0)
		{
			DiffViewer.BeginInvoke(new Action<int>(SetScrollPos), _scrollPosAfterReload);
		}
	}

	private void SetScrollPos(int scrollPos)
	{
		if(scrollPos > DiffViewer.MaxVScrollPos)
		{
			scrollPos = DiffViewer.MaxVScrollPos;
		}
		DiffViewer.VScrollBar.Value = scrollPos;
	}

	private void OnStatusFilterChanged(object sender, EventArgs e)
	{
		var changedFilesPanel = (ChangedFilesPanel)sender;
		var index = DiffViewer.Panels.IndexOf(changedFilesPanel) + 2;
		DiffViewer.BeginUpdate();
		if(index < DiffViewer.Panels.Count)
		{
			DiffViewer.Panels.RemoveRange(index, DiffViewer.Panels.Count - index);
		}
		FlowPanelSeparator separator = null;
		for(int i = 0; i < _allDiffPanels.Count; ++i)
		{
			if((_allDiffPanels[i].DiffFile.Status & changedFilesPanel.StatusFilter) != FileStatus.Unknown)
			{
				DiffViewer.Panels.Add(_allDiffPanels[i]);
				DiffViewer.Panels.Add(separator = new FlowPanelSeparator() { SeparatorStyle = FlowPanelSeparatorStyle.Simple });
			}
		}
		if(separator != null)
		{
			separator.Height = 6;
		}
		DiffViewer.EndUpdate();
	}

	protected override void OnFetchFailed(Exception exception)
	{
		if(DiffViewer.IsDisposed)
		{
			return;
		}
		DiffViewer.BeginUpdate();
		_progressPanel.Remove();
		if(exception != null && !string.IsNullOrWhiteSpace(exception.Message))
		{
			DiffViewer.Panels.Add(new FlowProgressPanel() { Message = exception.Message });
		}
		DiffViewer.EndUpdate();
	}

	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(!DiffViewer.IsDisposed)
			{
				DiffViewer.Panels.Clear();
			}
		}
		base.Dispose(disposing);
	}

	#endregion
}
