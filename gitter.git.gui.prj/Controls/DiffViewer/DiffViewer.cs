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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	/// <summary>Control for diff viewing.</summary>
	public class DiffViewer : FlowLayoutControl
	{
		#region Data

		private Repository _repository;
		private WeakReference _loadedDiffSource;
		private FlowProgressPanel _progressPanel;
		private IAsyncFunc<Diff> _requiredRequest;
		private readonly object _sync = new object();
		private List<FileDiffPanel> _allDiffPanels;

		#endregion

		#region Events

		public event EventHandler<DiffFileContextMenuRequestedEventArgs> DiffFileContextMenuRequested;

		public event EventHandler<UntrackedFileContextMenuRequestedEventArgs> UntrackedFileContextMenuRequested;

		internal void OnFileContextMenuRequested(DiffFile file)
		{
			var handler = DiffFileContextMenuRequested;
			if(handler != null)
			{
				var args = new DiffFileContextMenuRequestedEventArgs(file);
				handler(this, args);
				if(args.ContextMenu != null)
				{
					args.ContextMenu.Show(this, PointToClient(Cursor.Position));
				}
			}
		}

		internal void OnFileContextMenuRequested(TreeFile file)
		{
			var handler = UntrackedFileContextMenuRequested;
			if(handler != null)
			{
				var args = new UntrackedFileContextMenuRequestedEventArgs(file);
				handler(this, args);
				if(args.ContextMenu != null)
				{
					args.ContextMenu.Show(this, PointToClient(Cursor.Position));
				}
			}
		}

		#endregion

		#region .ctor

		/// <summary>Create <see cref="DiffViewer"/>.</summary>
		public DiffViewer()
		{
			_allDiffPanels = new List<FileDiffPanel>();
		}

		#endregion

		public Repository Repository
		{
			get { return _repository; }
			set { _repository = value; }
		}

		private void LoadFailedCore(Exception exc)
		{
			BeginUpdate();
			if(_progressPanel != null)
			{
				_progressPanel.Remove();
				_progressPanel = null;
			}
			if(exc != null)
			{
				Panels.Add(new FlowProgressPanel() { Message = exc.Message });
			}
			EndUpdate();
		}

		private void LoadDiffCore(Diff diff, int scrollPos = 0)
		{
			BeginUpdate();
			_allDiffPanels.Clear();
			if(_progressPanel != null)
			{
				_progressPanel.Remove();
				_progressPanel = null;
			}
			if(diff != null)
			{
				FlowPanelSeparator separator = null;
				var changedFilesPanel = new ChangedFilesPanel() { Diff = diff };
				changedFilesPanel.StatusFilterChanged += OnStatusFilterChanged;
				Panels.Add(changedFilesPanel);
				Panels.Add(new FlowPanelSeparator() { SeparatorStyle = FlowPanelSeparatorStyle.Line });
				foreach(var file in diff)
				{
					var fileDiffPanel = new FileDiffPanel(_repository, file, diff.Type);
					_allDiffPanels.Add(fileDiffPanel);
					Panels.Add(fileDiffPanel);
					Panels.Add(separator = new FlowPanelSeparator() { SeparatorStyle = FlowPanelSeparatorStyle.Simple });
				}
				if(separator != null) separator.Height = 6;
			}
			if(scrollPos > MaxVScrollPos)
			{
				scrollPos = MaxVScrollPos;
			}
			VScrollPos = scrollPos;
			EndUpdate();
		}

		private void OnStatusFilterChanged(object sender, EventArgs e)
		{
			var changedFilesPanel = (ChangedFilesPanel)sender;
			var index = Panels.IndexOf(changedFilesPanel) + 2;
			BeginUpdate();
			if(index < Panels.Count)
			{
				Panels.RemoveRange(index, Panels.Count - index);
			}
			FlowPanelSeparator separator = null;
			for(int i = 0; i < _allDiffPanels.Count; ++i)
			{
				if((_allDiffPanels[i].DiffFile.Status & changedFilesPanel.StatusFilter) != FileStatus.Unknown)
				{
					Panels.Add(_allDiffPanels[i]);
					Panels.Add(separator = new FlowPanelSeparator() { SeparatorStyle = FlowPanelSeparatorStyle.Simple });
				}
			}
			if(separator != null) separator.Height = 6;
			EndUpdate();
		}

		private void LoadAsyncDiffCore(IAsyncFunc<Diff> diffLoader, int scrollpos = 0)
		{
			lock(_sync)
			{
				_requiredRequest = diffLoader;
			}
			if(diffLoader != null)
			{
				_progressPanel = new FlowProgressPanel();
				Panels.Add(_progressPanel);

				_scrollPosAfterReload = scrollpos;
				diffLoader.BeginInvoke(this, _progressPanel.ProgressMonitor, OnDiffLoadCompleted, diffLoader);
			}
		}

		private int _scrollPosAfterReload;

		private void OnDiffLoadCompleted(IAsyncResult ar)
		{
			var diffLoader = (IAsyncFunc<Diff>)(ar.AsyncState);
			Diff diff = null;
			try
			{
				diff = diffLoader.EndInvoke(ar);
			}
			catch(GitException exc)
			{
				if(!IsHandleCreated) return;
				lock(_sync)
				{
					if(_requiredRequest == diffLoader)
					{
						_requiredRequest = null;
						try
						{
							if(InvokeRequired)
							{
								BeginInvoke(new Action<Exception>(LoadFailedCore), new object[] { exc });
							}
							else
							{
								LoadFailedCore(exc);
							}
						}
						catch(InvalidOperationException)
						{
						}
					}
				}
			}
			if(!IsHandleCreated) return;
			if(diff != null)
			{
				lock(_sync)
				{
					if(_requiredRequest == diffLoader)
					{
						_requiredRequest = null;
						try
						{
							if(InvokeRequired)
							{
								BeginInvoke(new Action<Diff, int>(LoadDiffCore), new object[] { diff, _scrollPosAfterReload });
							}
							else
							{
								LoadDiffCore(diff, _scrollPosAfterReload);
							}
						}
						catch(InvalidOperationException)
						{
						}
					}
				}
			}
		}

		protected override void OnPanelMouseDown(FlowPanel panel, int x, int y, MouseButtons button)
		{
			foreach(var p in Panels)
			{
				if(p != panel)
				{
					var filePanel = p as FileDiffPanel;
					if(filePanel != null) filePanel.DropSelection();
				}
			}
			base.OnPanelMouseDown(panel, x, y, button);
		}

		protected override void OnFreeSpaceMouseDown(int x, int y, MouseButtons button)
		{
			foreach(var p in Panels)
			{
				var filePanel = p as FileDiffPanel;
				if(filePanel != null) filePanel.DropSelection();
			}
			base.OnFreeSpaceMouseDown(x, y, button);
		}

		private bool IsFileHeaderVisible
		{
			get
			{
				FileDiffPanel fdp = null;
				foreach(var p in Panels)
				{
					fdp = p as FileDiffPanel;
					if(fdp != null) break;
				}
				if(fdp == null) return false;
				var bounds = fdp.Bounds;
				return bounds.Y - VScrollPos < 0;
			}
		}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.C:
					if(Control.ModifierKeys == Keys.Control)
					{
						foreach(var p in Panels)
						{
							var filePanel = p as FileDiffPanel;
							if(filePanel != null && filePanel.SelectionLength != 0)
							{
								var lines = filePanel.GetSelectedLines();
								var sb = new StringBuilder();
								bool first = true;
								foreach(var line in lines)
								{
									if(first)
									{
										first = false;
									}
									else
									{
										sb.Append('\n');
									}
									sb.Append(line.Text);
								}
								if(sb.Length != 0)
								{
									if(sb[sb.Length - 1] == '\r')
									{
										sb.Remove(sb.Length - 1, 1);
									}
								}
								ClipboardEx.SetTextSafe(sb.ToString());
								break;
							}
						}

						e.IsInputKey = true;
					}
					break;
			}
			base.OnPreviewKeyDown(e);
		}

		private void LoadAsyncCore(IDiffSource diffSource, DiffOptions options, int scrollPos = 0)
		{
			lock(_sync)
			{
				_requiredRequest = null;
			}
			_progressPanel = null;
			BeginUpdate();
			Panels.Clear();
			ScrollToTopLeft();
			var revisionSource = diffSource as IRevisionDiffSource;
			if(revisionSource != null)
			{
				Panels.Add(new RevisionHeaderPanel() { Revision = revisionSource.Revision.Dereference() });
				Panels.Add(new FlowPanelSeparator() { SeparatorStyle = FlowPanelSeparatorStyle.Line });
			}
			var indexSource = diffSource as IIndexDiffSource;
			if(indexSource != null && !indexSource.Cached)
			{
				var panel = new UntrackedFilesPanel(indexSource.Repository.Status);
				if(panel.Count != 0)
				{
					Panels.Add(panel);
					Panels.Add(new FlowPanelSeparator() { Height = 5 });
				}
			}
			LoadAsyncDiffCore(diffSource.GetDiffAsync(options), scrollPos);
			EndUpdate();
		}

		public void LoadAsync(IDiffSource diffSource)
		{
			Verify.Argument.IsNotNull(diffSource, "diffSource");

			int scrollpos = 0;
			if(_loadedDiffSource != null && _loadedDiffSource.Target == diffSource)
			{
				scrollpos = VScrollPos;
			}
			else
			{
				_loadedDiffSource = new WeakReference(diffSource);
			}

			LoadAsyncCore(diffSource, DiffOptions.CreateDefault(), scrollpos);
		}

		public void LoadAsync(IDiffSource diffSource, DiffOptions options)
		{
			Verify.Argument.IsNotNull(diffSource, "diffSource");
			Verify.Argument.IsNotNull(options, "options");

			int scrollpos = 0;
			if(_loadedDiffSource != null && _loadedDiffSource.Target == diffSource)
			{
				scrollpos = VScrollPos;
			}
			else
			{
				_loadedDiffSource = new WeakReference(diffSource);
			}

			LoadAsyncCore(diffSource, options, scrollpos);
		}

		/// <summary>Load specified <paramref name="diff"/>.</summary>
		/// <param name="diff"><see cref="Diff"/> to load.</param>
		public void Load(Diff diff)
		{
			lock(_sync)
			{
				_requiredRequest = null;
			}
			BeginUpdate();
			_progressPanel = null;
			Panels.Clear();
			LoadDiffCore(diff);
			ScrollToTopLeft();
			EndUpdate();
		}

		/// <summary>Load specified <paramref name="diff"/>.</summary>
		/// <param name="diff"><see cref="Diff"/> to load.</param>
		public void LoadAsync(IAsyncFunc<Diff> diff)
		{
			lock(_sync)
			{
				_requiredRequest = null;
			}
			BeginUpdate();
			_progressPanel = null;
			Panels.Clear();
			LoadAsyncDiffCore(diff);
			ScrollToTopLeft();
			EndUpdate();
		}

		/// <summary>Load specified <paramref name="diffFile"/>.</summary>
		/// <param name="diff"><see cref="DiffFile"/> to load.</param>
		public void Load(DiffFile diffFile)
		{
			lock(_sync)
			{
				_requiredRequest = null;
			}
			BeginUpdate();
			_progressPanel = null;
			Panels.Clear();
			if(diffFile != null)
			{
				Panels.Add(new FlowPanelSeparator() { SeparatorStyle = FlowPanelSeparatorStyle.Simple, Height = 5 });
				Panels.Add(new FileDiffPanel(_repository, diffFile, DiffType.Patch));
				Panels.Add(new FlowPanelSeparator() { SeparatorStyle = FlowPanelSeparatorStyle.Simple, Height = 6 });
			}
			ScrollToTopLeft();
			EndUpdate();
		}

		/// <summary>Remove any displayed panels.</summary>
		public void Clear()
		{
			lock(_sync)
			{
				_requiredRequest = null;
			}
			_progressPanel = null;
			Panels.Clear();
		}
	}
}
