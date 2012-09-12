namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public sealed class DragHelper
	{
		#region Data

		private bool _isTracking;
		private bool _isDragging;
		private int _x;
		private int _y;

		#endregion

		public void Start(Point point)
		{
			Start(point.X, point.Y);
		}

		public void Start(int x, int y)
		{
			Verify.State.IsFalse(IsTracking);

			_x = x;
			_y = y;
			_isTracking = true;
		}

		public void Stop()
		{
			Verify.State.IsTrue(IsTracking);

			_isTracking = false;
			_isDragging = false;
		}

		public bool Update(Point point)
		{
			return Update(point.X, point.Y);
		}

		public bool Update(int x, int y)
		{
			Verify.State.IsTrue(IsTracking);

			if(_isDragging) return true;
			var dragSize = SystemInformation.DragSize;
			IsDragging =
				(Math.Abs(x - _x) * 2 > dragSize.Width) ||
				(Math.Abs(y - _y) * 2 > dragSize.Height);
			return IsDragging;
		}

		public bool IsTracking
		{
			get { return _isTracking; }
		}

		public bool IsDragging
		{
			get { return _isDragging; }
			private set { _isDragging = value; }
		}
	}
}
