namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	internal struct GripBounds
	{
		private const int GripSize = 6;
		private const int CornerGripSize = GripSize << 1;

		private readonly Rectangle _clientRectangle;

		public GripBounds(Rectangle clientRectangle)
		{
			_clientRectangle = clientRectangle;
		}

		public Rectangle ClientRectangle
		{
			get { return _clientRectangle; }
		}

		public Rectangle Bottom
		{
			get
			{
				var rect = _clientRectangle;
				rect.Y = rect.Bottom - GripSize + 1;
				rect.Height = GripSize;
				return rect;
			}
		}

		public Rectangle BottomRight
		{
			get
			{
				var rect = _clientRectangle;
				rect.Y = rect.Bottom - CornerGripSize + 1;
				rect.Height = CornerGripSize;
				rect.X = rect.Width - CornerGripSize + 1;
				rect.Width = CornerGripSize;
				return rect;
			}
		}

		public Rectangle Top
		{
			get
			{
				var rect = _clientRectangle;
				rect.Height = GripSize;
				return rect;
			}
		}

		public Rectangle TopRight
		{
			get
			{
				var rect = _clientRectangle;
				rect.Height = CornerGripSize;
				rect.X = rect.Width - CornerGripSize + 1;
				rect.Width = CornerGripSize;
				return rect;
			}
		}

		public Rectangle Left
		{
			get
			{
				var rect = _clientRectangle;
				rect.Width = GripSize;
				return rect;
			}
		}

		public Rectangle BottomLeft
		{
			get
			{
				var rect = _clientRectangle;
				rect.Width = CornerGripSize;
				rect.Y = rect.Height - CornerGripSize + 1;
				rect.Height = CornerGripSize;
				return rect;
			}
		}

		public Rectangle Right
		{
			get
			{
				var rect = _clientRectangle;
				rect.X = rect.Right - GripSize + 1;
				rect.Width = GripSize;
				return rect;
			}
		}

		public Rectangle TopLeft
		{
			get
			{
				var rect = _clientRectangle;
				rect.Width = CornerGripSize;
				rect.Height = CornerGripSize;
				return rect;
			}
		}
	}
}
