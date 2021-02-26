﻿#region Copyright Notice
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
	using System.Collections.Generic;
	using System.Windows.Forms;
	using System.Drawing;

	/// <summary>Splits controls by movable splitters.</summary>
	internal sealed class ViewSplit : Control, IEnumerable<Control>
	{
		private readonly List<Control> _items;
		private readonly ViewSplitPositions _positions;
		private Size _size;

		/// <summary>
		/// Replace <paramref name="origin"/> with a <see cref="ViewSplit"/>, containing
		/// <paramref name="origin"/> and <paramref name="sibling"/> controls.
		/// </summary>
		/// <param name="origin">Original control.</param>
		/// <param name="sibling">Inserted control.</param>
		/// <param name="side"><paramref name="sibling"/> position.</param>
		/// <returns>Created <see cref="ViewSplit"/>.</returns>
		public static ViewSplit Replace(Control origin, Control sibling, AnchorStyles side)
		{
			Control item1;
			Control item2;
			Orientation orientation;
			var bounds = origin.Bounds;
			var anchor = origin.Anchor;
			var parent = origin.Parent;
			if(parent == null) return null;
			switch(side)
			{
				case AnchorStyles.Left:
					item1 = sibling;
					item2 = origin;
					orientation = Orientation.Horizontal;
					break;
				case AnchorStyles.Top:
					item1 = sibling;
					item2 = origin;
					orientation = Orientation.Vertical;
					break;
				case AnchorStyles.Right:
					item1 = origin;
					item2 = sibling;
					orientation = Orientation.Horizontal;
					break;
				case AnchorStyles.Bottom:
					item1 = origin;
					item2 = sibling;
					orientation = Orientation.Vertical;
					break;
				default:
					throw new ArgumentException($"Invalid value: {side}", nameof(side));
			}
			parent.DisableRedraw();
			parent.SuspendLayout();
			try
			{
				var split = new ViewSplit(orientation, bounds, item1, item2);
				split.SuspendLayout();
				split.Bounds = bounds;
				split.Parent = parent;

				switch(parent)
				{
					case ViewSplit psplit:
						split.Anchor = anchor;
						var index = psplit.IndexOf(origin);
						psplit[index] = split;
						break;
					case ViewDockGrid pgrid:
						split.Anchor = anchor;
						pgrid.RootControl = split;
						break;
					case FloatingViewForm pfloat:
						pfloat.RootControl = split;
						split.Dock = DockStyle.Fill;
						if(origin is ViewHost vh)
						{
							pfloat.EnterMulticontrolMode();
							vh.Status = ViewHostStatus.DockedOnFloat;
						}
						break;
					default:
						throw new ApplicationException("Unsupported parent control type.");
				}

				item1.Parent = split;
				item2.Parent = split;
				split.ResumeLayout(false);
				return split;
			}
			finally
			{
				parent.ResumeLayout(false);
				parent.EnableRedraw();
				parent.RedrawWindow();
			}
		}

		/// <summary>Gets optimal sizes for splitter controls.</summary>
		/// <param name="available">Available space.</param>
		/// <param name="minimum">Minimum control size.</param>
		/// <param name="s1">Size of the first control.</param>
		/// <param name="s2">Size of the second control.</param>
		public static void OptimalSizes(int available, int minimum, ref int s1, ref int s2)
		{
			if(s1 >= available * 0.9 && s2 >= available * 0.9)
			{
				available -= ViewConstants.Spacing;
				s1 = available / 2;
				s2 = available - s1;
			}
			else
			{
				if(s1 == available)
				{
					s1 -= s2 + ViewConstants.Spacing;
					if(s1 < minimum)
					{
						int d = minimum - s1;
						s1 = minimum;
						s2 -= d;
					}
				}
				else if(s2 == available)
				{
					s2 -= s1 + ViewConstants.Spacing;
					if(s2 < minimum)
					{
						int d = minimum - s2;
						s2 = minimum;
						s1 -= d;
					}
				}
				else
				{
					int freew = available - (s1 + ViewConstants.Spacing + s2);
					if(freew != 0)
					{
						int f1 = freew / 2;
						int f2 = freew - f1;
						s1 += f1;
						s2 += f2;
					}
				}
			}
		}

		/// <summary>Compares control's priority.</summary>
		/// <param name="c1">First control.</param>
		/// <param name="c2">Second control.</param>
		/// <returns>Result of comparing <paramref name="c1"/> and <paramref name="c2"/>.</returns>
		private static int ComparePriority(Control c1, Control c2)
		{
			var dw1 = (c1 is ViewHost host1 && host1.IsDocumentWell)
			       || (c1 is ViewSplit ts1 && ts1.ContainsDocumentWell);
			var dw2 = (c2 is ViewHost host2 && host2.IsDocumentWell)
			       || (c2 is ViewSplit ts2 && ts2.ContainsDocumentWell);
			if(dw1 == dw2) return 0;
			if(dw1) return -1;
			return 1;
		}

		/// <summary>Initializes a new instance of the <see cref="ViewSplit"/> class.</summary>
		/// <param name="orientation">Control flow orientation.</param>
		/// <param name="bounds">Bounds.</param>
		/// <param name="item1">First item.</param>
		/// <param name="item2">Second item.</param>
		private ViewSplit(Orientation orientation, Rectangle bounds, Control item1, Control item2)
		{
			Verify.Argument.IsNotNull(item1, nameof(item1));
			Verify.Argument.IsNotNull(item2, nameof(item2));

			SetStyle(ControlStyles.ContainerControl, true);
			SetStyle(ControlStyles.Selectable, false);

			BackColor = ViewManager.Renderer.BackgroundColor;

			Orientation = orientation;
			_items = new List<Control>() { item1, item2 };
			_size = bounds.Size;
			Rectangle bounds1, bounds2;
			AnchorStyles anchor1, anchor2;
			int prioroty = ComparePriority(item1, item2);
			switch(orientation)
			{
				case Orientation.Horizontal:
					if(prioroty == 0)
					{
						anchor1 = ViewConstants.AnchorDockLeft;
						anchor2 = ViewConstants.AnchorDockLeft;
					}
					else
					{
						if(prioroty == 1)
						{
							anchor1 = ViewConstants.AnchorDockLeft;
							anchor2 = ViewConstants.AnchorAll;
						}
						else
						{
							anchor1 = ViewConstants.AnchorAll;
							anchor2 = ViewConstants.AnchorDockRight;
						}
					}
					int w1 = item1.Width;
					int w2 = item2.Width;
					OptimalSizes(bounds.Width, ViewConstants.MinimumHostWidth, ref w1, ref w2);
					bounds1 = new Rectangle(0, 0, w1, bounds.Height);
					bounds2 = new Rectangle(w1 + ViewConstants.Spacing, 0, w2, bounds.Height);
					_positions = new ViewSplitPositions(this, bounds.Width,
						(double)(w1 + ViewConstants.Spacing / 2) / (double)bounds.Width);
					break;
				case Orientation.Vertical:
					if(prioroty == 0)
					{
						anchor1 = ViewConstants.AnchorDockTop;
						anchor2 = ViewConstants.AnchorDockTop;
					}
					else
					{
						if(prioroty == 1)
						{
							anchor1 = ViewConstants.AnchorDockTop;
							anchor2 = ViewConstants.AnchorAll;
						}
						else
						{
							anchor1 = ViewConstants.AnchorAll;
							anchor2 = ViewConstants.AnchorDockBottom;
						}
					}
					int h1 = item1.Height;
					int h2 = item2.Height;
					OptimalSizes(bounds.Height, ViewConstants.MinimumHostHeight, ref h1, ref h2);
					bounds1 = new Rectangle(0, 0, bounds.Width, h1);
					bounds2 = new Rectangle(0, h1 + ViewConstants.Spacing, bounds.Width, h2);
					_positions = new ViewSplitPositions(this, bounds.Height,
						(double)(h1 + ViewConstants.Spacing / 2) / (double)bounds.Height);
					break;
				default:
					throw new ArgumentException(
						string.Format("Unknown Orientation value: {0}", orientation),
						"orientation");
			}
			item1.Dock = DockStyle.None;
			item1.Anchor = anchor1;
			item1.Bounds = bounds1;
			item2.Dock = DockStyle.None;
			item2.Anchor = anchor2;
			item2.Bounds = bounds2;
		}

		/// <summary>Layout orientation.</summary>
		public Orientation Orientation { get; }

		/// <summary>Gets or sets hosted <see cref="System.Windows.Forms.Control"/> at the specified index.</summary>
		public Control this[int index]
		{
			get { return _items[index]; }
			set { _items[index] = value; }
		}

		/// <summary>Gets the hosted control count.</summary>
		/// <value>Hosted control count.</value>
		public int Count => _items.Count;

		/// <summary>Returns index of hosted <paramref name="control"/>.</summary>
		/// <param name="control">Control to look for.</param>
		/// <returns>Index of specified <paramref name="control"/> or -1, if it was not found.</returns>
		public int IndexOf(Control control)
		{
			return _items.IndexOf(control);
		}

		public IEnumerable<double> Positions => _positions;

		/// <summary>
		/// Gets a value indicating whether this <see cref="ViewSplit"/> contains <see cref="ViewHost"/> with
		/// <see cref="M:ViewHost.IsDocumentWell"/> == true.
		/// </summary>
		public bool ContainsDocumentWell
		{
			get
			{
				foreach(var ctl in _items)
				{
					switch(ctl)
					{
						case ViewHost host:
							if(host.IsDocumentWell) return true;
							break;
						case ViewSplit split:
							if(split.ContainsDocumentWell) return true;
							break;
					}
				}
				return false;
			}
		}

		/// <summary>Removes the specified control.</summary>
		/// <param name="item">Control to remove.</param>
		public void Remove(Control item)
		{
			Verify.Argument.IsNotNull(item, nameof(item));

			RemoveAt(_items.IndexOf(item));
		}

		/// <summary>Removes control at specified position.</summary>
		/// <param name="index">Index of removing control.</param>
		/// <returns>Removed control.</returns>
		public Control RemoveAt(int index)
		{
			Verify.Argument.IsValidIndex(index, _items.Count, nameof(index));

			if(_items.Count == 2)
			{
				int singleIndex = index == 0 ? 1 : 0;
				var parent = Parent;
				SuspendLayout();
				var removedItem = _items[index];
				var singleItem = _items[singleIndex];
				_items.Clear();
				removedItem.Parent = null;
				singleItem.Bounds = Bounds;
				singleItem.Anchor = Anchor;
				singleItem.Parent = parent;
				Parent = null;
				// valid parents for ViewSplit: ViewDockGrid, ViewSplit, ViewFloatingForm
				switch(parent)
				{
					case ViewSplit split:
						// embed remaining item into this split's old position.
						split[split.IndexOf(this)] = singleItem;
						break;
					case ViewDockGrid pgrid:
						pgrid.RootControl = singleItem;
						break;
					case FloatingViewForm pfloat:
						pfloat.RootControl = singleItem;
						if(singleItem is ViewHost th)
						{
							pfloat.LeaveMulticontrolMode();
							th.Status = ViewHostStatus.Floating;
						}
						singleItem.Anchor = AnchorStyles.Left | AnchorStyles.Top;
						singleItem.Dock = DockStyle.Fill;
						break;
					default:
						throw new ApplicationException("Unexpected ViewSplit.Parent: " + parent);
				}
				ResumeLayout(false);
				Dispose();
				return removedItem;
			}
			else
			{
				var removedItem = _items[index];
				removedItem.Parent = null;
				_items.RemoveAt(index);
				return removedItem;
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseEnter"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnMouseEnter(EventArgs e)
		{
			switch(Orientation)
			{
				case Orientation.Horizontal:
					Cursor = Cursors.SizeWE;
					break;
				case Orientation.Vertical:
					Cursor = Cursors.SizeNS;
					break;
				default:
					throw new ApplicationException();
			}
			base.OnMouseEnter(e);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseLeave"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnMouseLeave(EventArgs e)
		{
			Cursor = Cursors.Default;
			base.OnMouseLeave(e);
		}

		/// <summary>
		/// Raises the <see cref="E:Resize"/> event.
		/// </summary>
		/// <param name="eventargs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);
			var size = Size;
			bool enforceChildSizes = true;
			switch(Orientation)
			{
				case Orientation.Horizontal:
					if(_size.Width == size.Width)
						enforceChildSizes = false;
					break;
				case Orientation.Vertical:
					if(_size.Height == size.Height)
						enforceChildSizes = false;
					break;
				default:
					throw new ApplicationException();
			}
			if(enforceChildSizes)
			{
				for(int i = 0; i < _items.Count; ++i)
				{
					if(_items[i].Anchor == ViewConstants.AnchorAll)
					{
						enforceChildSizes = false;
						_positions.Actualize();
						break;
					}
				}
			}
			if(enforceChildSizes)
			{
				_positions.Apply();
			}
			_size = size;
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
			{
				_positions.StartMoving(e.Location);
			}
			base.OnMouseDown(e);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseMove"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if(_positions.IsMoving)
			{
				_positions.UpdateMoving(e.Location);
			}
			base.OnMouseMove(e);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if(_positions.IsMoving)
			{
				_positions.CommitMoving(e.Location);
			}
			base.OnMouseUp(e);
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"/> and its child controls and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				_positions.Dispose();
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator&lt;Control&gt;"/> object that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<Control> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}
	}
}
