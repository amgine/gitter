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
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	/// <summary>Control for diff viewing.</summary>
	public class DiffViewer : FlowLayoutControl
	{
		#region Events

		private static readonly object DiffFileContextMenuRequestedEvent = new object();

		public event EventHandler<DiffFileContextMenuRequestedEventArgs> DiffFileContextMenuRequested
		{
			add { Events.AddHandler(DiffFileContextMenuRequestedEvent, value); }
			remove { Events.RemoveHandler(DiffFileContextMenuRequestedEvent, value); }
		}

		private static readonly object UntrackedFileContextMenuRequestedEvent = new object();

		public event EventHandler<UntrackedFileContextMenuRequestedEventArgs> UntrackedFileContextMenuRequested
		{
			add { Events.AddHandler(UntrackedFileContextMenuRequestedEvent, value); }
			remove { Events.RemoveHandler(UntrackedFileContextMenuRequestedEvent, value); }
		}

		internal void OnFileContextMenuRequested(DiffFile file)
		{
			var handler = (EventHandler<DiffFileContextMenuRequestedEventArgs>)Events[DiffFileContextMenuRequestedEvent];
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
			var handler = (EventHandler<UntrackedFileContextMenuRequestedEventArgs>)Events[UntrackedFileContextMenuRequestedEvent];
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
		}

		#endregion

		#region Methods

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

		#endregion
	}
}
