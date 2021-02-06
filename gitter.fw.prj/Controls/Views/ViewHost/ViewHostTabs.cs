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

namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;
	using System.Collections.Generic;

	using gitter.Framework.Services;

	/// <summary>Tab control for view host.</summary>
	public sealed class ViewHostTabs : Control, IEnumerable<ViewHostTab>
	{
		#region Data

		private readonly List<ViewHostTab> _tabs;
		private readonly TrackingService<ViewHostTab> _tabHover;
		private readonly TrackingService<object> _areaHover;
		private int _areaMouseDown;
		private bool _readyToFloat;
		private int _mdX;
		private int _mdY;
		private int _floatId;
		private int _length;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="ViewHostTabs"/>.</summary>
		internal ViewHostTabs(ViewHost viewHost, AnchorStyles side)
		{
			Verify.Argument.IsNotNull(viewHost, nameof(viewHost));

			ViewHost = viewHost;
			ViewHost.ActiveViewChanged += OnActiveViewChanged;

			SetStyle(
				ControlStyles.ContainerControl |
				ControlStyles.Selectable |
				ControlStyles.SupportsTransparentBackColor,
				false);
			SetStyle(
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.OptimizedDoubleBuffer,
				true);

			Side = side;

			_tabs = new List<ViewHostTab>();
			foreach(var view in viewHost.Views)
			{
				_tabs.Add(new ViewHostTab(this, view));
			}
			if(_tabs.Count != 0)
			{
				int length = 0;
				foreach(var tab in _tabs)
				{
					tab.ResetLength(GraphicsUtility.MeasurementGraphics);
					length += tab.Length;
				}
				_length = length;
			}

			_tabHover = new TrackingService<ViewHostTab>(OnTabHoverChanged);
			_areaHover = new TrackingService<object>(OnAreaHoverChanged);
			_areaMouseDown = -1;
			if(viewHost.IsDocumentWell)
			{
				var tabHeight = Renderer.TabHeight;
				var tabFooterHeight = Renderer.TabFooterHeight;
				LeftButtons = new ViewButtons(this)
				{
					Height = tabHeight + tabFooterHeight,
				};
				LeftButtons.ButtonClick += OnViewButtonClick;
				RightButtons = new ViewButtons(this)
				{
					Height = tabHeight + tabFooterHeight,
				};
				RightButtons.SetAvailableButtons(ViewButtonType.TabsMenu);
				RightButtons.ButtonClick += OnViewButtonClick;
			}
			_floatId = -1;
		}

		private void OnActiveViewChanged(object sender, EventArgs e)
		{
			Invalidate();
		}

		private void OnAreaHoverChanged(object sender, TrackingEventArgs<object> e)
		{
			switch(e.Index)
			{
				case 0:
					if(!e.IsTracked)
					{
						LeftButtons.OnMouseLeave();
					}
					break;
				case 1:
					if(!e.IsTracked)
					{
						_tabHover.Drop();
					}
					break;
				case 2:
					if(!e.IsTracked)
					{
						RightButtons.OnMouseLeave();
					}
					break;
			}
		}

		private void OnTabHoverChanged(object sender, TrackingEventArgs<ViewHostTab> e)
		{
			if(e.IsTracked)
			{
				e.Item.OnMouseEnter();
			}
			else
			{
				e.Item.OnMouseLeave();
			}
		}

		#endregion

		#region Properties

		public ViewHostTab this[int index] => _tabs[index];

		private ViewRenderer Renderer => ViewManager.Renderer;

		public int Count => _tabs.Count;

		public AnchorStyles Side { get; }

		public ViewHost ViewHost { get; }

		public ViewButtons LeftButtons { get; }

		public ViewButtons RightButtons { get; }

		public int FirstTabIndex { get; private set; }

		#endregion

		#region Methods

		internal void InvalidateTab(ViewBase view)
		{
			Verify.Argument.IsNotNull(view, nameof(view));

			var index	= IndexOf(view);
			var tab		= _tabs[index];
			var length	= tab.Length;
			tab.ResetLength(GraphicsUtility.MeasurementGraphics);
			var dl = tab.Length - length;
			if(dl != 0)
			{
				_length += dl;
				ResetButtons();
			}
			Invalidate();
		}

		internal void InvalidateTab(ViewHostTab tab)
		{
			Verify.Argument.IsNotNull(tab, nameof(tab));
			Verify.Argument.AreEqual(this, tab.Tabs, "tab", "Tab is hosted in another control.");

			var index	= IndexOf(tab);
			var length	= tab.Length;
			tab.ResetLength(GraphicsUtility.MeasurementGraphics);
			var dl = tab.Length - length;
			if(dl != 0)
			{
				_length += dl;
				ResetButtons();
			}
			Invalidate();
		}

		public void EnsureVisible(ViewHostTab tab)
		{
			Verify.Argument.IsNotNull(tab, nameof(tab));
			Verify.Argument.AreEqual(this, tab.Tabs, "tab", "Tab is hosted in another control.");

			if(ViewHost.IsDocumentWell) EnsureVisible(IndexOf(tab));
		}

		public void EnsureVisible(ViewBase view)
		{
			Verify.Argument.IsNotNull(view, nameof(view));

			if(ViewHost.IsDocumentWell) EnsureVisible(IndexOf(view));
		}

		private void EnsureVisible(int index)
		{
			Verify.Argument.IsValidIndex(index, _tabs.Count, "index");

			if(index < FirstTabIndex)
			{
				FirstTabIndex = index;
				ResetButtons();
				Invalidate();
			}
			else if(index == FirstTabIndex)
			{
				return;
			}
			else
			{
				var w = Width;
				if(LeftButtons != null && LeftButtons.Count != 0)
				{
					w -= LeftButtons.Width + ViewConstants.TabHeaderButtonSpacing;
				}
				if(RightButtons != null && RightButtons.Count != 0)
				{
					w -= RightButtons.Width + ViewConstants.TabHeaderButtonSpacing;
				}

				if(w <= 0)
				{
					FirstTabIndex = index;
					ResetButtons();
					Invalidate();
				}
				else
				{
					int space = 0;
					for(int i = FirstTabIndex; i <= index; ++i)
					{
						space += _tabs[i].Length;
					}

					if(space > w)
					{
						space = 0;
						FirstTabIndex = index;
						for(int i = index; i >= FirstTabIndex; --i)
						{
							if(space > w) break;
							FirstTabIndex = i;
							space += _tabs[i].Length;
						}
						ResetButtons();
						Invalidate();
					}
				}
			}
		}

		private void ResetButtons()
		{
			if(ViewHost.IsDocumentWell)
			{
				var viewButtonSize = ViewManager.Renderer.ViewButtonSize;
				var space = Width;
				space -= LeftButtons.Width + viewButtonSize +
					2 * ViewConstants.TabHeaderButtonSpacing;
				var length = 0;
				if(FirstTabIndex == 0)
				{
					length = _length;
				}
				else
				{
					if(_length < space)
					{
						FirstTabIndex = 0;
					}
					else
					{
						if(FirstTabIndex >= _tabs.Count)
						{
							FirstTabIndex = _tabs.Count - 1;
						}
						for(int i = FirstTabIndex; i < _tabs.Count; ++i)
						{
							length += _tabs[i].Length;
						}
						int free = 0;
						for(int i = FirstTabIndex - 1; i >= 0; --i)
						{
							free += _tabs[i].Length;
							if(free + length > space)
							{
								FirstTabIndex = i + 1;
								length += free - _tabs[i].Length;
								break;
							}
						}
					}
				}
				if(FirstTabIndex != 0)
				{
					if(LeftButtons.Count == 0)
					{
						LeftButtons.SetAvailableButtons(ViewButtonType.ScrollTabsLeft);
					}
				}
				else
				{
					if(LeftButtons.Count == 1)
					{
						LeftButtons.SetAvailableButtons(null);
					}
				}
				if(length > space)
				{
					bool needRightScroll = (FirstTabIndex != _tabs.Count - 1);
					if(needRightScroll)
					{
						if(RightButtons.Count == 1)
						{
							RightButtons.SetAvailableButtons(
								ViewButtonType.ScrollTabsRight,
								ViewButtonType.TabsScrollMenu);
						}
					}
					else
					{
						if(RightButtons.Count == 2)
						{
							RightButtons.SetAvailableButtons(
								ViewButtonType.TabsScrollMenu);
						}
					}
				}
				else
				{
					if(RightButtons.Count == 2)
					{
						if(FirstTabIndex == 0)
						{
							RightButtons.SetAvailableButtons(ViewButtonType.TabsMenu);
						}
						else
						{
							RightButtons.SetAvailableButtons(ViewButtonType.TabsScrollMenu);
						}
					}
					else if(RightButtons.Count == 1)
					{
						if(FirstTabIndex == 0)
						{
							if(RightButtons[0].Type == ViewButtonType.TabsScrollMenu)
							{
								RightButtons.SetAvailableButtons(ViewButtonType.TabsMenu);
							}
						}
						else
						{
							if(RightButtons[0].Type == ViewButtonType.TabsMenu)
							{
								RightButtons.SetAvailableButtons(ViewButtonType.TabsScrollMenu);
							}
						}
					}
				}
			}
		}

		public void AddView(ViewBase view)
		{
			Verify.Argument.IsNotNull(view, nameof(view));

			var tab = new ViewHostTab(this, view);
			tab.ResetLength();
			_tabs.Add(tab);
			_length += tab.Length;
			ResetButtons();
			Invalidate();
		}

		public void InsertView(ViewBase view, int index)
		{
			Verify.Argument.IsNotNull(view, nameof(view));

			var tab = new ViewHostTab(this, view);
			tab.ResetLength();
			_length += tab.Length;
			_tabs.Insert(index, tab);
			ResetButtons();
			Invalidate();
		}

		private void SwapTabsCore(int index1, int index2)
		{
			if(index1 == index2) return;
			var tab1 = _tabs[index1];
			var tab2 = _tabs[index2];
			_tabs[index1] = tab2;
			_tabs[index2] = tab1;
			Invalidate();
		}

		public void SwapViews(ViewBase view1, ViewBase view2)
		{
			Verify.Argument.IsNotNull(view1, nameof(view1));
			Verify.Argument.IsNotNull(view2, nameof(view2));

			if(view1 == view2) return;
			int index1 = -1;
			int index2 = -1;

			for(int i = 0; i < _tabs.Count; ++i)
			{
				var view = _tabs[i].View;
				if(index1 == -1 && view == view1)
				{
					index1 = i;
					if(index2 == -1) break;
				}
				else if(index2 == -1 && view == view2)
				{
					index2 = i;
					if(index1 == -1) break;
				}
			}
			Verify.Argument.IsTrue(index1 != -1, nameof(view1), "View #1 was not found.");
			Verify.Argument.IsTrue(index2 != -1, nameof(view2), "View #2 was not found.");
			SwapTabsCore(index1, index2);
		}

		public void SwapTabs(int index1, int index2)
		{
			Verify.Argument.IsValidIndex(index1, _tabs.Count, nameof(index1));
			Verify.Argument.IsValidIndex(index2, _tabs.Count, nameof(index2));

			SwapTabsCore(index1, index2);
		}

		public bool Remove(ViewHostTab tab)
		{
			Verify.Argument.IsNotNull(tab, nameof(tab));

			if(_tabHover.Item == tab)
			{
				_tabHover.Reset(-1, null);
			}
			_length -= tab.Length;
			return _tabs.Remove(tab);
		}

		public void RemoveAt(int index)
		{
			var tab = _tabs[index];
			if(_tabHover.Item == tab)
			{
				_tabHover.Reset(-1, null);
			}
			_length -= tab.Length;
			_tabs.RemoveAt(index);
			tab.Dispose();
		}

		public bool RemoveView(ViewBase view)
		{
			Verify.Argument.IsNotNull(view, nameof(view));

			int id = IndexOf(view);
			if(id != -1)
			{
				RemoveAt(id);
				ResetButtons();
				Invalidate();
				return true;
			}
			else
			{
				return false;
			}
		}

		public void Clear()
		{
			if(_tabs.Count != 0)
			{
				_tabs.Clear();
				ResetButtons();
				_length = 0;
				Invalidate();
			}
		}

		public int IndexOf(ViewBase view)
		{
			for(int i = 0; i < _tabs.Count; ++i)
			{
				if(_tabs[i].View == view) return i;
			}
			return -1;
		}

		public int IndexOf(ViewHostTab tab)
		{
			return _tabs.IndexOf(tab);
		}

		public bool Contains(ViewBase view)
		{
			foreach(var tab in _tabs)
			{
				if(tab.View == view) return true;
			}
			return false;
		}

		public bool Contains(ViewHostTab tab)
		{
			return _tabs.Contains(tab);
		}

		#endregion

		private int GetTabOffset(ViewHostTab tab)
		{
			int x = 0;
			if(LeftButtons != null)
			{
				x += LeftButtons.Width;
			}
			for(int i = FirstTabIndex; i < _tabs.Count; ++i)
			{
				if(_tabs[i] == tab) return x;
				x += _tabs[i].Length;
			}
			throw new ArgumentException("Tab was not found.", "tab");
		}

		private int GetTabOffset(int index)
		{
			int x = 0;
			if(LeftButtons != null)
			{
				x += LeftButtons.Width;
			}
			for(int i = FirstTabIndex; i < index; ++i)
			{
				var length = _tabs[i].Length;
				x += length;
			}
			return x;
		}

		private int HitTestArea(int x, int y)
		{
			var width = Width;
			if(x < 0 || x > width || y < 0 || y >= ViewManager.Renderer.TabHeight)
			{
				return -1;
			}
			if(LeftButtons != null)
			{
				var lbw = LeftButtons.Width;
				if(lbw != 0)
				{
					if(x <= lbw)
					{
						return 0;
					}
				}
			}
			if(RightButtons != null)
			{
				var rbw = RightButtons.Width;
				if(rbw != 0)
				{
					if(x >= width - rbw)
					{
						return 2;
					}
				}
			}
			return 1;
		}

		private int HitTestTab(int x, int y, out Rectangle bounds)
		{
			if(y >= 0 && y < Renderer.TabHeight)
			{
				var maxTabWidth = GetMaxTabWidth();
				int space = Width;
				int cx = 0;
				if(LeftButtons != null)
				{
					if(LeftButtons.Count != 0)
					{
						var lbw = LeftButtons.Width;
						space -= lbw + ViewConstants.TabHeaderButtonSpacing;
						cx += lbw + ViewConstants.TabHeaderButtonSpacing;
					}
				}
				if(RightButtons != null)
				{
					if(RightButtons.Count != 0)
					{
						var rbw = RightButtons.Width;
						space -= rbw + ViewConstants.TabHeaderButtonSpacing;
					}
				}
				for(int i = FirstTabIndex; i < _tabs.Count; ++i)
				{
					var length = _tabs[i].Length;
					if(length > maxTabWidth)
						length = maxTabWidth;
					space -= length;
					if(!ViewHost.IsDocumentWell)
					{
						if(maxTabWidth != int.MaxValue && space < 3)
							length += space;
					}
					var cx2 = cx + length;
					if(x >= cx && x < cx2)
					{
						bounds = new Rectangle(cx, 0, length, Renderer.TabHeight);
						return i;
					}
					cx = cx2;
					if(space <= 0) break;
				}
			}
			bounds = Rectangle.Empty;
			return -1;
		}

		private void OnViewButtonClick(object sender, ViewButtonClickEventArgs e)
		{
			switch(e.Button)
			{
				case ViewButtonType.ScrollTabsLeft:
					if(FirstTabIndex != 0)
					{
						--FirstTabIndex;
						ResetButtons();
						Invalidate();
					}
					break;
				case ViewButtonType.ScrollTabsRight:
					if(FirstTabIndex < _tabs.Count - 1)
					{
						++FirstTabIndex;
						ResetButtons();
						Invalidate();
					}
					break;
				case ViewButtonType.TabsMenu:
				case ViewButtonType.TabsScrollMenu:
					if(_tabs.Count != 0)
					{
						var menu = new ContextMenuStrip();
						foreach(var tab in _tabs)
						{
							menu.Items.Add(
								new ToolStripMenuItem(tab.Text, tab.Image,
									(item, args) =>
									{
										var view = (ViewBase)((ToolStripMenuItem)item).Tag;
										ViewHost.Activate(view);
									})
								{
									Tag = tab.View,
								});
						}
						Utility.MarkDropDownForAutoDispose(menu);
						var viewButtonSize = Renderer.ViewButtonSize;
						menu.Show(this,
							Width - viewButtonSize - 1,
							viewButtonSize + 4);
					}
					break;
			}
		}

		/// <summary>Raises the <see cref="E:System.Windows.Forms.Control.MouseLeave"/> event.</summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnMouseLeave(EventArgs e)
		{
			_areaHover.Drop();
			base.OnMouseLeave(e);
		}

		/// <summary>Raises the <see cref="E:System.Windows.Forms.Control.MouseDown"/> event.</summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			_mdX = e.X;
			_mdY = e.Y;
			Rectangle bounds;
			_areaMouseDown = HitTestArea(e.X, e.Y);
			switch(_areaMouseDown)
			{
				case 0:
					LeftButtons.OnMouseDown(e.X, e.Y, e.Button);
					break;
				case 1:
					int tabId = HitTestTab(_mdX, _mdY, out bounds);
					if(tabId != -1)
					{
						_floatId = tabId;
						int x = e.X - bounds.X;
						int y = e.Y - bounds.Y;
						var tab = _tabs[tabId];
						_tabs[tabId].OnMouseDown(x, y, e.Button);
						if(_tabs.Count <= tabId || _tabs[tabId] != tab)
						{
							_floatId = -1;
						}
						else
						{
							_readyToFloat = e.Button == MouseButtons.Left &&
								_tabs[tabId].Buttons.PressedButton == null;
						}
					}
					else
					{
						ViewHost.Focus();
					}
					break;
				case 2:
					RightButtons.OnMouseDown(e.X - (Width - RightButtons.Width), e.Y, e.Button);
					break;
				default:
					ViewHost.Focus();
					break;
			}
			base.OnMouseDown(e);
		}

		/// <summary>Raises the <see cref="E:System.Windows.Forms.Control.MouseMove"/> event.</summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if(_readyToFloat)
			{
				if(Math.Abs(_mdX - e.X) >= ViewConstants.ViewFloatDragMargin ||
					Math.Abs(_mdY - e.Y) >= ViewConstants.ViewFloatDragMargin)
				{
					var form = ViewHost.GetRootOwnerForm();
					_readyToFloat = false;
					var tab = _tabs[_floatId];
					ViewHost.RemoveView(tab.View);
					var host = new ViewHost(ViewHost.Grid, false, false, new[] { tab.View });
					var floatingForm = host.PrepareFloatingMode();
					Capture = false;
					ViewHost.Focus();
					floatingForm.Show(form);
					floatingForm.Shown += (sender, args) =>
						((ViewHost)((Form)sender).Controls[0]).StartMoving(e.X, e.Y);
					_floatId = -1;
				}
			}
			int areaId = _areaMouseDown;
			int tabId;
			int offset;
			if(areaId == -1)
			{
				areaId = HitTestArea(e.X, e.Y);
			}
			if(areaId == -1)
			{
				_areaHover.Drop();
			}
			else
			{
				_areaHover.Track(areaId, null);
			}
			if(areaId == -1)
			{
				_areaHover.Drop();
				tabId = -1;
				offset = 0;
			}
			else
			{
				if(_floatId == -1)
				{
					if(areaId == 1)
					{
						tabId = HitTestTab(e.X, e.Y, out var bounds);
						if(tabId == -1) _tabHover.Drop();
						offset = bounds.X;
					}
					else
					{
						tabId = -1;
						offset = 0;
					}
				}
				else
				{
					areaId = 1;
					tabId = _floatId;
					offset = GetTabOffset(_floatId);
				}
			}
			switch(areaId)
			{
				case 0:
					LeftButtons.OnMouseMove(e.X, e.Y, e.Button);
					break;
				case 1:
					if(tabId != -1)
					{
						_tabHover.Track(tabId, _tabs[tabId]);
						int x = e.X;
						int y = e.Y;
						switch(Side)
						{
							case AnchorStyles.Top:
							case AnchorStyles.Bottom:
								x -= offset;
								break;
							case AnchorStyles.Left:
							case AnchorStyles.Right:
								y -= offset;
								break;
						}
						_tabs[tabId].OnMouseMove(x, y, e.Button);
					}
					break;
				case 2:
					RightButtons.OnMouseMove(e.X - (Width - RightButtons.Width), e.Y, e.Button);
					break;
			}
			base.OnMouseMove(e);
		}

		/// <summary>Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"/> event.</summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			switch(_areaMouseDown)
			{
				case 0:
					LeftButtons.OnMouseUp(e.X, e.Y, e.Button);
					break;
				case 1:
					if(_floatId != -1)
					{
						var offset = GetTabOffset(_floatId);
						int x = e.X;
						int y = e.Y;
						switch(Side)
						{
							case AnchorStyles.Top:
							case AnchorStyles.Bottom:
								x -= offset;
								break;
							case AnchorStyles.Left:
							case AnchorStyles.Right:
								y -= offset;
								break;
						}
						_tabs[_floatId].OnMouseUp(x, y, e.Button);
					}
					_readyToFloat = false;
					_floatId = -1;
					break;
				case 2:
					RightButtons.OnMouseUp(e.X - (Width - RightButtons.Width), e.Y, e.Button);
					break;
			}
			_areaMouseDown = -1;
			base.OnMouseUp(e);
		}

		/// <summary>Paints the background of the control.</summary>
		/// <param name="pevent">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains information about the control to paint.</param>
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
		}

		private int GetMaxTabWidth()
		{
			var width = Width;
			int maxTabWidth;
			if(!ViewHost.IsDocumentWell)
			{
				if(_length > width && _tabs.Count != 0)
				{
					maxTabWidth = width / _tabs.Count;
					var free = width;
					var nfree = 0;
					for(int i = 0; i < _tabs.Count; ++i)
					{
						var tl = _tabs[i].Length;
						if(tl > maxTabWidth)
						{
							++nfree;
						}
						else
						{
							free -= tl;
						}
					}
					if(nfree > 0 && nfree < _tabs.Count)
					{
						maxTabWidth = free / nfree;
					}
				}
				else
				{
					maxTabWidth = int.MaxValue;
				}
			}
			else
			{
				maxTabWidth = int.MaxValue;
			}
			return maxTabWidth;
		}

		/// <summary>Raises the <see cref="E:System.Windows.Forms.Control.Resize"/> event.</summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			ResetButtons();
		}

		/// <summary>Raises the <see cref="E:System.Windows.Forms.Control.Paint"/> event.</summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			var graphics = e.Graphics;
			var width = Width;
			int space = width;
			var maxTabWidth = GetMaxTabWidth();

			graphics.TextRenderingHint = GraphicsUtility.TextRenderingHint;
			graphics.TextContrast      = GraphicsUtility.TextContrast;

			Renderer.RenderViewHostTabsBackground(this, e);

			var tabHeight = Renderer.TabHeight;
			var rect = new Rectangle(0, 0, width, tabHeight);
			if(ViewHost.IsDocumentWell)
			{
				if(LeftButtons != null)
				{
					var lbw = LeftButtons.Width;
					if(lbw != 0)
					{
						rect.X += lbw + ViewConstants.TabHeaderButtonSpacing;
						space -= lbw + ViewConstants.TabHeaderButtonSpacing;
						var buttonsRect = new Rectangle(0, 0, lbw, tabHeight);
						LeftButtons.OnPaint(graphics, buttonsRect, ViewHost.IsActive);
					}
				}
				if(RightButtons != null)
				{
					var rbw = RightButtons.Width;
					if(rbw != 0)
					{
						space -= rbw + ViewConstants.TabHeaderButtonSpacing;
						var buttonsRect = new Rectangle(width - rbw, 0, rbw, tabHeight);
						RightButtons.OnPaint(graphics, buttonsRect, ViewHost.IsActive);
					}
				}
			}
			int dx = 0, dy = 0;
			graphics.SetClip(new Rectangle(rect.X, rect.Y, space, rect.Height));
			for(int i = FirstTabIndex; i < _tabs.Count; ++i)
			{
				var length = _tabs[i].Length;
				if(length > maxTabWidth)
				{
					length = maxTabWidth;
				}
				space -= length;
				if(!ViewHost.IsDocumentWell)
				{
					if(maxTabWidth != int.MaxValue && space < 3)
					{
						length += space;
					}
				}
				switch(Side)
				{
					case AnchorStyles.Top:
					case AnchorStyles.Bottom:
						rect.Width = length;
						dx = length;
						break;
					case AnchorStyles.Left:
					case AnchorStyles.Right:
						rect.Height = length;
						dy = length;
						break;
					default:
						throw new ApplicationException(
							string.Format("Unknown AnchorStyle value: {0}", Side));
				}
				_tabs[i].OnPaint(graphics, rect);
				if(space <= 0) break;
				rect.Offset(dx, dy);
			}
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"/>
		/// and its child controls and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">
		/// true to release both managed and unmanaged resources; false to release only unmanaged resources.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				foreach(var tab in _tabs)
				{
					tab.Dispose();
				}
				_tabs.Clear();
				ViewHost.ActiveViewChanged -= OnActiveViewChanged;
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator&lt;ViewHostTab&gt;"/> object
		/// that can be used to iterate through the collection.
		/// </returns>
		public List<ViewHostTab>.Enumerator GetEnumerator()
			=> _tabs.GetEnumerator();

		/// <inheritdoc/>
		IEnumerator<ViewHostTab> IEnumerable<ViewHostTab>.GetEnumerator()
			=> _tabs.GetEnumerator();

		/// <inheritdoc/>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			=> _tabs.GetEnumerator();
	}
}
