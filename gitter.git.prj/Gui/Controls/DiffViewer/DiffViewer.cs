namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
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

		#endregion

		#region Events

		public event EventHandler<DiffFileContextMenuRequestedEventArgs> DiffFileContextMenuRequested;

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

		#endregion

		#region .ctor

		/// <summary>Create <see cref="DiffViewer"/>.</summary>
		public DiffViewer()
		{
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
			if(_progressPanel != null)
			{
				_progressPanel.Remove();
				_progressPanel = null;
			}
			if(diff != null)
			{
				FlowPanelSeparator separator = null;
				Panels.Add(new ChangedFilesPanel() { Diff = diff });
				Panels.Add(new FlowPanelSeparator() { Style = FlowPanelSeparatorStyle.Line });
				foreach(var file in diff)
				{
					Panels.Add(new FileDiffPanel(_repository, file, diff.Type));
					Panels.Add(separator = new FlowPanelSeparator() { Style = FlowPanelSeparatorStyle.Simple });
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
										first = false;
									else
										sb.Append('\n');
									sb.Append(line.Text);
								}
								if(sb.Length != 0)
								{
									if(sb[sb.Length - 1] == '\r')
										sb.Remove(sb.Length - 1, 1);
								}
								Clipboard.SetText(sb.ToString());
								break;
							}
						}

						e.IsInputKey = true;
					}
					break;
			}
			base.OnPreviewKeyDown(e);
		}

		/// <summary>Load diff for specified <paramref name="revision"/>.</summary>
		/// <param name="revision">Revision to load diff for.</param>
		public void Load(IRevisionPointer revision)
		{
			lock(_sync)
			{
				_requiredRequest = null;
			}
			BeginUpdate();
			_progressPanel = null;
			Panels.Clear();
			if(revision != null)
			{
				Panels.Add(new RevisionHeaderPanel() { Revision = revision.Dereference() });
				Panels.Add(new FlowPanelSeparator() { Style = FlowPanelSeparatorStyle.Line });
				try
				{
					LoadDiffCore(revision.GetDiff());
				}
				catch(GitException exc)
				{
					LoadFailedCore(exc);
				}
			}
			ScrollToTopLeft();
			EndUpdate();
		}

		/// <summary>Load diff for specified <paramref name="wtreeItem"/>.</summary>
		/// <param name="wtreeItem">Working tree item to load diff for.</param>
		public void Load(TreeItem wtreeItem)
		{
			lock(_sync)
			{
				_requiredRequest = null;
			}
			BeginUpdate();
			_progressPanel = null;
			Panels.Clear();
			if(wtreeItem != null)
			{
				try
				{
					LoadDiffCore(wtreeItem.GetDiff());
				}
				catch(GitException exc)
				{
					LoadFailedCore(exc);
				}
			}
			ScrollToTopLeft();
			EndUpdate();
		}

		/// <summary>Load diff for specified <paramref name="revision"/> asynchronously.</summary>
		/// <param name="revision">Revision to load diff for.</param>
		public void LoadAsync(IRevisionPointer revision)
		{
			lock(_sync)
			{
				_requiredRequest = null;
			}
			_progressPanel = null;
			BeginUpdate();
			Panels.Clear();
			if(revision != null)
			{
				Panels.Add(new RevisionHeaderPanel() { Revision = revision.Dereference() });
				Panels.Add(new FlowPanelSeparator() { Style = FlowPanelSeparatorStyle.Line });

				LoadAsyncDiffCore(revision.GetDiffAsync());
			}
			ScrollToTopLeft();
			EndUpdate();
		}

		/// <summary>Load diff for specified <paramref name="wtreeItem"/> asynchronously.</summary>
		/// <param name="wtreeItem">Working tree item to load diff for.</param>
		public void LoadAsync(TreeItem wtreeItem)
		{
			lock(_sync)
			{
				_requiredRequest = null;
			}
			_progressPanel = null;
			BeginUpdate();
			Panels.Clear();
			if(wtreeItem != null)
			{
				LoadAsyncDiffCore(wtreeItem.GetDiffAsync());
			}
			ScrollToTopLeft();
			EndUpdate();
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
			var panels = diffSource.GetInformationPanels();
			if(panels != null)
			{
				Panels.AddRange(panels);
			}
			LoadAsyncDiffCore(diffSource.GetDiffAsync(options), scrollPos);
			EndUpdate();
		}

		public void LoadAsync(IDiffSource diffSource)
		{
			if(diffSource == null)
				throw new ArgumentNullException("diffSource");

			int scrollpos = 0;
			if(_loadedDiffSource != null && _loadedDiffSource.Target == diffSource)
			{
				scrollpos = VScrollPos;
			}
			else
			{
				_loadedDiffSource = new WeakReference(diffSource);
			}

			LoadAsyncCore(diffSource, DiffOptions.Default, scrollpos);
		}

		public void LoadAsync(IDiffSource diffSource, DiffOptions options)
		{
			if(diffSource == null)
				throw new ArgumentNullException("diffSource");
			if(options == null)
				throw new ArgumentNullException("options");

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
				Panels.Add(new FlowPanelSeparator() { Style = FlowPanelSeparatorStyle.Simple, Height = 5 });
				Panels.Add(new FileDiffPanel(_repository, diffFile, DiffType.Patch));
				Panels.Add(new FlowPanelSeparator() { Style = FlowPanelSeparatorStyle.Simple, Height = 6 });
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
