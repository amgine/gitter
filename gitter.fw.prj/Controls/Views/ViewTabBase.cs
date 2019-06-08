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

	public abstract class ViewTabBase : IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="ViewTabBase"/> class.</summary>
		/// <param name="view">Represented <see cref="ViewBase"/>.</param>
		/// <param name="anchor">Tab anchor.</param>
		protected ViewTabBase(ViewBase view, AnchorStyles anchor)
		{
			Verify.Argument.IsNotNull(view, nameof(view));

			switch(anchor)
			{
				case AnchorStyles.Left:
				case AnchorStyles.Right:
					Orientation = Orientation.Vertical;
					break;
				case AnchorStyles.Top:
				case AnchorStyles.Bottom:
					Orientation = Orientation.Horizontal;
					break;
				default:
					throw new ArgumentException("Invalid anchor value.", nameof(anchor));
			}
			Anchor = anchor;
			View = view;
		}

		/// <summary>Finalizes an instance of the <see cref="ViewTabBase"/> class.</summary>
		~ViewTabBase()
		{
			Dispose(false);
		}

		protected ViewRenderer Renderer => ViewManager.Renderer;

		public AnchorStyles Anchor { get; }

		public Orientation Orientation { get; }

		public ViewBase View { get; }

		public Image Image => View.Image;

		public int Length { get; private set; }

		public string Text => View.Text;

		public virtual bool IsActive => View.Host.ActiveView == View;

		public bool IsMouseOver { get; private set; }

		public void ResetLength()
		{
			ResetLength(GraphicsUtility.MeasurementGraphics);
		}

		public void ResetLength(Graphics graphics)
		{
			Length = Measure(graphics);
		}

		protected internal virtual void OnMouseLeave()
		{
			IsMouseOver = false;
		}

		protected internal virtual void OnMouseEnter()
		{
			IsMouseOver = true;
		}

		public virtual void OnMouseDown(int x, int y, MouseButtons button)
		{
		}

		public virtual void OnMouseMove(int x, int y, MouseButtons button)
		{
		}

		public virtual void OnMouseUp(int x, int y, MouseButtons button)
		{
		}

		protected abstract int Measure(Graphics graphics);

		internal abstract void OnPaint(Graphics graphics, Rectangle bounds);

		/// <summary>Returns a <see cref="System.String"/> that represents this instance.</summary>
		/// <returns>A <see cref="System.String"/> that represents this instance.</returns>
		public override string ToString() => View.Text;

		/// <summary>Gets a value indicating whether this instance is disposed.</summary>
		/// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
		public bool IsDisposed { get; private set; }

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public void Dispose()
		{
			if(!IsDisposed)
			{
				GC.SuppressFinalize(this);
				Dispose(true);
				IsDisposed = true;
			}
		}
	}
}
