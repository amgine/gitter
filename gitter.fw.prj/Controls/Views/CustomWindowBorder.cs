#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.Controls;

using System;
using System.Windows.Forms;
using System.Drawing;

sealed class CustomWindowBorder : IDisposable
{
	class BorderForm : Form
	{
		public BorderForm(CustomWindowBorder border, AnchorStyles side)
		{
			Border = border;
			Side = side;

			SetStyle(
				ControlStyles.ContainerControl |
				ControlStyles.Selectable |
				ControlStyles.ResizeRedraw |
				ControlStyles.SupportsTransparentBackColor,
				false);

			StartPosition = FormStartPosition.Manual;
			FormBorderStyle = FormBorderStyle.None;
			AutoScaleMode = AutoScaleMode.None;
			ControlBox = false;
			MaximizeBox = false;
			MinimizeBox = false;
			Text = string.Empty;
			ShowIcon = false;
			ShowInTaskbar = false;
			ImeMode = ImeMode.Disable;

			MinimumSize = new Size(1, 1);

			BackColor = Color.Black;
			Opacity = ViewConstants.InactiveBorderOpacity;
		}

		private CustomWindowBorder Border { get; }

		public AnchorStyles Side { get; }

		protected override void WndProc(ref Message m)
		{
			switch((Native.WM)m.Msg)
			{
				case Native.WM.MOUSEACTIVATE:
					m.Result = (IntPtr)(-1);
					return;
			}

			base.WndProc(ref m);
		}

		private int _mX;
		private int _mY;
		private AnchorStyles _resizeSide;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if(Owner is FloatingViewForm owner)
			{
				owner.RootControl?.Focus();
			}
			else
			{
				Owner?.Activate();
			}
			_resizeSide = Side;
			if(_resizeSide is AnchorStyles.Top or AnchorStyles.Bottom)
			{
				if(e.X < Height)
				{
					_resizeSide |= AnchorStyles.Left;
				}
				else if(e.X >= Width - Height)
				{
					_resizeSide |= AnchorStyles.Right;
				}
			}
			_mX = e.X;
			_mY = e.Y;
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if(_resizeSide == AnchorStyles.None)
			{
				UpdateCursor(e.X);
			}
			if(e.Button == MouseButtons.Left)
			{
				UpdateOwnerSize(e.X, e.Y);
			}
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
			{
				UpdateOwnerSize(e.X, e.Y);
				_resizeSide = AnchorStyles.None;
				UpdateCursor(e.X);
			}
			base.OnMouseUp(e);
		}

		private void UpdateCursor(int x)
		{
			if(Side == AnchorStyles.Top)
			{
				if(x < Height)
				{
					Cursor = Cursors.SizeNWSE;
				}
				else if(x >= Width - Height)
				{
					Cursor = Cursors.SizeNESW;
				}
				else
				{
					Cursor = Cursors.SizeNS;
				}
			}
			else if(Side == AnchorStyles.Bottom)
			{
				if(x < Height)
				{
					Cursor = Cursors.SizeNESW;
				}
				else if(x >= Width - Height)
				{
					Cursor = Cursors.SizeNWSE;
				}
				else
				{
					Cursor = Cursors.SizeNS;
				}
			}
		}

		private void UpdateOwnerSize(int x, int y)
		{
			static bool ResizeLeft(int dx, Size minSize, Size maxSize, ref Rectangle bounds)
			{
				if(dx == 0) return false;
				if(maxSize.Width > 0 && bounds.Width - dx > maxSize.Width)
				{
					if(bounds.Width == maxSize.Width) return false;
					bounds.X = bounds.Right - maxSize.Width;
					bounds.Width = maxSize.Width;
				}
				else if(minSize.Width > 0 && bounds.Width - dx < minSize.Width)
				{
					if(bounds.Width == minSize.Width) return false;
					bounds.X = bounds.Right - minSize.Width;
					bounds.Width = minSize.Width;
				}
				else
				{
					bounds.X += dx;
					bounds.Width -= dx;
				}
				return true;
			}

			static bool ResizeRight(int dx, Size minSize, Size maxSize, ref Rectangle bounds)
			{
				if(dx == 0) return false;
				if(maxSize.Width > 0 && bounds.Width - dx > maxSize.Width)
				{
					if(bounds.Width == maxSize.Width) return false;
					bounds.Width = maxSize.Width;
				}
				else if(minSize.Width > 0 && bounds.Width - dx < minSize.Width)
				{
					if(bounds.Width == minSize.Width) return false;
					bounds.Width = minSize.Width;
				}
				else
				{
					bounds.Width -= dx;
				}
				return true;
			}

			static bool ResizeTop(int dy, Size minSize, Size maxSize, ref Rectangle bounds)
			{
				if(dy == 0) return false;
				if(maxSize.Height > 0 && bounds.Height - dy > maxSize.Height)
				{
					if(bounds.Height == maxSize.Height) return false;
					bounds.Y = bounds.Bottom - maxSize.Height;
					bounds.Height = maxSize.Height;
				}
				else if(minSize.Height > 0 && bounds.Height - dy < minSize.Height)
				{
					if(bounds.Height == minSize.Height) return false;
					bounds.Y = bounds.Bottom - minSize.Height;
					bounds.Height = minSize.Height;
				}
				else
				{
					bounds.Y += dy;
					bounds.Height -= dy;
				}
				return true;
			}

			static bool ResizeBottom(int dy, Size minSize, Size maxSize, ref Rectangle bounds)
			{
				if(dy == 0) return false;
				if(maxSize.Height > 0 && bounds.Height - dy > maxSize.Height)
				{
					if(bounds.Height == maxSize.Height) return false;
					bounds.Height = maxSize.Height;
				}
				else if(minSize.Height > 0 && bounds.Height - dy < minSize.Height)
				{
					if(bounds.Height == minSize.Height) return false;
					bounds.Height = minSize.Height;
				}
				else
				{
					bounds.Height -= dy;
				}
				return true;
			}

			if(Owner is not { IsDisposed: false } owner) return;

			var original = owner.Bounds;
			var bounds   = original;
			var minSize  = Owner.MinimumSize;
			var maxSize  = Owner.MaximumSize;
			var resized  = false;
			var side     = _resizeSide;

			if((side & AnchorStyles.Left)   != 0) resized |= ResizeLeft  (x - _mX, minSize, maxSize, ref bounds);
			if((side & AnchorStyles.Top)    != 0) resized |= ResizeTop   (y - _mY, minSize, maxSize, ref bounds);
			if((side & AnchorStyles.Right)  != 0) resized |= ResizeRight (_mX - x, minSize, maxSize, ref bounds);
			if((side & AnchorStyles.Bottom) != 0) resized |= ResizeBottom(_mY - y, minSize, maxSize, ref bounds);

			if(!resized) return;

			var boundsSpecified = BoundsSpecified.None;
			if(bounds.X      != original.X)      boundsSpecified |= BoundsSpecified.X;
			if(bounds.Y      != original.Y)      boundsSpecified |= BoundsSpecified.Y;
			if(bounds.Width  != original.Width)  boundsSpecified |= BoundsSpecified.Width;
			if(bounds.Height != original.Height) boundsSpecified |= BoundsSpecified.Height;
			if(boundsSpecified != BoundsSpecified.None)
			{
				if(_resizeSide == (AnchorStyles.Right | AnchorStyles.Top))
				{
					_mX = x;
				}
				else if(_resizeSide == (AnchorStyles.Right | AnchorStyles.Bottom))
				{
					_mX = x;
				}

				Owner.SetBounds(
					bounds.X, bounds.Y, bounds.Width, bounds.Height,
					boundsSpecified);
				Border.Update();
				Owner.Update();
			}
		}

		protected override bool ShowWithoutActivation => true;

		protected override bool ScaleChildren => false;

		protected override void ScaleControl(SizeF factor, BoundsSpecified specified) { }

		protected override void ScaleCore(float x, float y) { }
	}

	private readonly BorderForm _left;
	private readonly BorderForm _top;
	private readonly BorderForm _right;
	private readonly BorderForm _bottom;
	private double _opacity = ViewConstants.InactiveBorderOpacity;

	public CustomWindowBorder(Form owner)
	{
		Owner = owner;

		_left   = new(this, AnchorStyles.Left)   { Cursor = Cursors.SizeWE, };
		_top    = new(this, AnchorStyles.Top)    { Cursor = Cursors.SizeNS, };
		_right  = new(this, AnchorStyles.Right)  { Cursor = Cursors.SizeWE, };
		_bottom = new(this, AnchorStyles.Bottom) { Cursor = Cursors.SizeNS, };
		Update();
		_left.Show(Owner);
		_top.Show(Owner);
		_right.Show(Owner);
		_bottom.Show(Owner);
		Update();
	}

	public double Opacity
	{
		get => _opacity;
		set
		{
			if(_opacity == value) return;

			_opacity = value;
			if(IsDisposed) return;
			_left.Opacity = value;
			_top.Opacity = value;
			_right.Opacity = value;
			_bottom.Opacity = value;
		}
	}

	private Form Owner { get; }

	public void Update()
	{
		if(IsDisposed || Owner.IsDisposed) return;
		var conv = DpiConverter.FromDefaultTo(Owner);
		var size = conv.ConvertX(6);
		var b = Owner.Bounds;
		_left.Bounds   = new(b.Left - size, b.Top, size, b.Height);
		_top.Bounds    = new(b.Left - size, b.Top - size, b.Width + size * 2, size);
		_right.Bounds  = new(b.Right, b.Top, size, b.Height);
		_bottom.Bounds = new(b.Left - size, b.Bottom, b.Width + size * 2, size);
	}

	public bool IsDisposed { get; private set; }

	public void Dispose()
	{
		if(IsDisposed) return;
		_left.Dispose();
		_top.Dispose();
		_right.Dispose();
		_bottom.Dispose();
		IsDisposed = true;
	}
}
