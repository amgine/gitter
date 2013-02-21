namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public abstract class ViewTabBase : IDisposable
	{
		#region Data

		private readonly ViewBase _view;
		private readonly AnchorStyles _anchor;
		private readonly Orientation _orientation;
		private bool _isMouseOver;
		private int _length;

		#endregion

		/// <summary>Initializes a new instance of the <see cref="ViewTabBase"/> class.</summary>
		/// <param name="view">Represented <see cref="ViewBase"/>.</param>
		protected ViewTabBase(ViewBase view, AnchorStyles anchor)
		{
			Verify.Argument.IsNotNull(view, "view");

			switch(anchor)
			{
				case AnchorStyles.Left:
				case AnchorStyles.Right:
					_orientation = Orientation.Vertical;
					break;
				case AnchorStyles.Top:
				case AnchorStyles.Bottom:
					_orientation = Orientation.Horizontal;
					break;
			}
			_anchor = anchor;
			_view = view;
		}

		~ViewTabBase()
		{
			Dispose(false);
		}

		#region Properties

		public AnchorStyles Anchor
		{
			get { return _anchor; }
		}

		public Orientation Orientation
		{
			get { return _orientation; }
		}

		public ViewBase View
		{
			get { return _view; }
		}

		public Image Image
		{
			get { return _view.Image; }
		}

		public int Length
		{
			get { return _length; }
		}

		public string Text
		{
			get { return _view.Text; }
		}

		public virtual bool IsActive
		{
			get { return _view.Host.ActiveView == View; }
		}

		public bool IsMouseOver
		{
			get { return _isMouseOver; }
		}

		#endregion

		#region Methods

		public void ResetLength()
		{
			ResetLength(Utility.MeasurementGraphics);
		}

		public void ResetLength(Graphics graphics)
		{
			_length = Measure(graphics);
		}

		public virtual void OnMouseLeave()
		{
			_isMouseOver = false;
		}

		public virtual void OnMouseEnter()
		{
			_isMouseOver = true;
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

		#endregion

		/// <summary>Returns a <see cref="System.String"/> that represents this instance.</summary>
		/// <returns>A <see cref="System.String"/> that represents this instance.</returns>
		public override string ToString()
		{
			return _view.Text;
		}

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
			GC.SuppressFinalize(this);
			Dispose(true);
		}
	}
}
