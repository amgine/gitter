namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	using gitter.Framework.Options;

	abstract class ViewTabBase : IDisposable
	{
		#region Static Data

		protected static readonly Color Background = Color.FromArgb(41, 57, 85);

		protected static readonly Color NormalBorder = Color.FromArgb(54, 78, 111);
		protected static readonly Color NormalBackgroundStart = Color.FromArgb(77, 96, 130);
		protected static readonly Color NormalBackgroundEnd = Color.FromArgb(62, 83, 120);

		protected static readonly Color HoverBorder = Color.FromArgb(155, 167, 183);
		protected static readonly Color HoverBackgroundStart = Color.FromArgb(111, 119, 118);
		protected static readonly Color HoverBackgroundEnd = Color.FromArgb(77, 93, 116);

		protected static readonly Color SelectedTabBackground = Color.White;

		protected static readonly Color SelectedTabBackgroundActiveStart = Color.FromArgb(255, 252, 242);
		protected static readonly Color SelectedTabBackgroundActiveEnd = Color.FromArgb(255, 232, 166);
		protected static readonly Color SelectedTabBackgroundNormalStart = Color.FromArgb(202, 211, 226);
		protected static readonly Color SelectedTabBackgroundNormalEnd = Color.FromArgb(174, 185, 205);

		protected static readonly StringFormat NormalStringFormat = new StringFormat(StringFormat.GenericTypographic)
		{
			LineAlignment = StringAlignment.Center,
			Trimming = StringTrimming.EllipsisCharacter,
		};

		protected static readonly StringFormat VerticalStringFormat = new StringFormat(NormalStringFormat)
		{
			FormatFlags = StringFormatFlags.DirectionVertical,
		};

		#endregion

		#region Data

		private readonly Control _hostControl;
		private readonly ViewBase _view;
		private readonly AnchorStyles _anchor;
		private readonly Orientation _orientation;
		private Image _rotatedImage;
		private bool _isHovered;
		private int _length;

		#endregion

		/// <summary>Initializes a new instance of the <see cref="ViewTabBase"/> class.</summary>
		/// <param name="view">Represented <see cref="ViewBase"/>.</param>
		protected ViewTabBase(Control hostControl, ViewBase view, AnchorStyles anchor)
		{
			if(hostControl == null)
				throw new ArgumentNullException("hostControl");
			if(view == null)
				throw new ArgumentNullException("view");
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
			_hostControl = hostControl;
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

		public bool IsHovered
		{
			get { return _isHovered; }
		}

		#endregion

		#region Methods

		protected virtual int Measure(Graphics graphics)
		{
			var length = GitterApplication.TextRenderer.MeasureText(
				graphics, Text, GitterApplication.FontManager.UIFont, int.MaxValue, NormalStringFormat).Width;
			if(Image != null)
				length += 16 + ViewConstants.ImageSpacing;
			length += ViewConstants.BeforeTabContent + ViewConstants.AfterTabContent;
			return length;
		}

		public void ResetLength()
		{
			using(var graphics = _hostControl.CreateGraphics())
			{
				ResetLength(graphics);
			}
		}

		public void ResetLength(Graphics graphics)
		{
			_length = Measure(graphics);
		}

		public virtual void OnMouseLeave()
		{
			_isHovered = false;
		}

		public virtual void OnMouseEnter()
		{
			_isHovered = true;
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

		protected virtual void OnPaintBackground(Graphics graphics, Rectangle bounds)
		{
			const int corner = 1;
			int x = bounds.X;
			int y = bounds.Y;
			int w = bounds.Width;
			int h = bounds.Height;
			var linePoints = new Point[6];
			var polyPoints = new Point[6];
			LinearGradientMode gradient;
			switch(_anchor)
			{
				case AnchorStyles.Right:
					linePoints = new[]
						{
							new Point(x, y),
							new Point(x + w - corner - 1, y),
							new Point(x + w - 1, y + corner),
							new Point(x + w - 1, y + h - corner - 1),
							new Point(x + w - corner - 1, y + h - 1),
							new Point(x, y + h - 1),
						};
					polyPoints = new[]
						{
							new Point(x, y),
							new Point(x + w - corner - 1, y),
							new Point(x + w - 1, y + corner),
							new Point(x + w - 1, y + h - corner - 1),
							new Point(x + w - corner - 1, y + h - 1),
							new Point(x, y + h - 1),
						};
					break;
				case AnchorStyles.Left:
					linePoints = new[]
						{
							new Point(x + w - 1, y),
							new Point(x + corner, y),
							new Point(x, y + corner),
							new Point(x, y + h - corner - 1),
							new Point(x + corner, y + h - 1),
							new Point(x + w - 1, y + h - 1),
						};
					polyPoints = new[]
						{
							new Point(x + w - 1, y),
							new Point(x + corner, y),
							new Point(x, y + corner),
							new Point(x, y + h - corner - 1),
							new Point(x + corner, y + h - 1),
							new Point(x + w - 1, y + h - 1),
						};
					break;
				case AnchorStyles.Top:
					linePoints = new[]
						{
							new Point(x, y + h - 1),
							new Point(x, y + corner),
							new Point(x + corner, y),
							new Point(x + w - corner - 1, y),
							new Point(x + w - 1, y + corner),
							new Point(x + w - 1, y + h - 1),
						};
					polyPoints = new[]
						{
							new Point(x, y + h),
							new Point(x, y + corner),
							new Point(x + corner, y),
							new Point(x + w - corner, y),
							new Point(x + w, y + corner),
							new Point(x + w, y + h),
						};
					break;
				case AnchorStyles.Bottom:
					linePoints = new[]
						{
							new Point(x, y),
							new Point(x, y + h - corner - 1),
							new Point(x + corner, y + h - 1),
							new Point(x + w - corner - 1, y + h - 1),
							new Point(x + w - 1, y + h - corner - 1),
							new Point(x + w - 1, y),
						};
					polyPoints = new[]
						{
							new Point(x, y),
							new Point(x, y + h - corner - 1),
							new Point(x + corner + 1, y + h),
							new Point(x + w - corner - 1, y + h),
							new Point(x + w, y + h - corner - 1),
							new Point(x + w, y),
						};
					break;
				default:
					throw new ApplicationException();
			}
			switch(_orientation)
			{
				case Orientation.Horizontal:
					gradient = LinearGradientMode.Vertical;
					break;
				case Orientation.Vertical:
					gradient = LinearGradientMode.Horizontal;
					break;
				default:
					throw new ApplicationException();
			}
			var host = View.Host;
			if(IsActive)
			{
				if(host.IsDocumentWell)
				{
					Color start, end;
					if(host.IsActive)
					{
						start = SelectedTabBackgroundActiveStart;
						end = SelectedTabBackgroundActiveEnd;
					}
					else
					{
						start = SelectedTabBackgroundNormalStart;
						end = SelectedTabBackgroundNormalEnd;
					}
					using(var brush = new LinearGradientBrush(
						bounds, start, end, gradient))
					{
						graphics.FillPolygon(brush, polyPoints);
					}
				}
				else
				{
					using(var brush = new SolidBrush(SelectedTabBackground))
					{
						graphics.FillPolygon(brush, polyPoints);
					}
				}
			}
			else if(IsHovered)
			{
				using(var brush = new LinearGradientBrush(bounds,
					HoverBackgroundStart,
					HoverBackgroundEnd,
					gradient))
				{
					graphics.FillPolygon(brush, polyPoints);
				}
				using(var pen = new Pen(HoverBorder))
				{
					graphics.DrawLines(pen, linePoints);
				}
			}
			else
			{
				if(!host.IsDocumentWell)
				{
					using(var brush = new LinearGradientBrush(bounds,
						NormalBackgroundStart,
						NormalBackgroundEnd,
						gradient))
					{
						graphics.FillPolygon(brush, polyPoints);
					}
					using(var pen = new Pen(NormalBorder))
					{
						graphics.DrawLines(pen, linePoints);
					}
				}
			}
		}

		protected virtual void OnPaintContent(Graphics graphics, Rectangle bounds)
		{
			int x = bounds.X;
			int y = bounds.Y;
			int w = bounds.Width;
			int h = bounds.Height;
			Brush textBrush;
			Rectangle imageRect;
			StringFormat stringFormat;
			switch(_anchor)
			{
				case AnchorStyles.Right:
					imageRect = new Rectangle(x + (w - 16) / 2, y + 3, 16, 16);
					stringFormat = VerticalStringFormat;
					break;
				case AnchorStyles.Left:
					imageRect = new Rectangle(x + (w - 16) / 2, y + 3, 16, 16);
					stringFormat = VerticalStringFormat;
					break;
				case AnchorStyles.Top:
					imageRect = new Rectangle(x + 3, y + (h - 16) / 2, 16, 16);
					stringFormat = NormalStringFormat;
					break;
				case AnchorStyles.Bottom:
					imageRect = new Rectangle(x + 3, y + (h - 16) / 2, 16, 16);
					stringFormat = NormalStringFormat;
					break;
				default:
					throw new ApplicationException();
			}
			var host = _view.Host;
			if(IsActive)
			{
				textBrush = Brushes.Black;
			}
			else
			{
				textBrush = Brushes.White;
			}
			var image = Image;
			if(image != null)
			{
				if(_orientation == Orientation.Vertical)
				{
					if(_rotatedImage == null)
					{
						_rotatedImage = (Image)image.Clone();
						_rotatedImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
					}
					graphics.DrawImage(_rotatedImage, imageRect);
					bounds.Height -= imageRect.Height + 3;
					bounds.Y += imageRect.Height + 3;
				}
				else
				{
					graphics.DrawImage(image, imageRect);
					bounds.Width -= imageRect.Width + 3;
					bounds.X += imageRect.Width + 3;
				}
			}
			if(_orientation == Orientation.Vertical)
			{
				bounds.Y += ViewConstants.BeforeTabContent;
				bounds.Height -= ViewConstants.BeforeTabContent + ViewConstants.AfterTabContent - 1;
			}
			else
			{
				bounds.X += ViewConstants.BeforeTabContent;
				bounds.Width -= ViewConstants.BeforeTabContent + ViewConstants.AfterTabContent - 1;
			}
			if(_orientation == System.Windows.Forms.Orientation.Vertical)
			{
				bounds.Height += 10;
				GitterApplication.GdiPlusTextRenderer.DrawText(
					graphics, Text, GitterApplication.FontManager.UIFont, textBrush, bounds, stringFormat);
			}
			else
			{
				GitterApplication.TextRenderer.DrawText(
					graphics, Text, GitterApplication.FontManager.UIFont, textBrush, bounds, stringFormat);
			}
		}

		public void OnPaint(Graphics graphics, Rectangle bounds)
		{
			if(bounds.Width != 0 && bounds.Height != 0)
			{
				OnPaintBackground(graphics, bounds);
				OnPaintContent(graphics, bounds);
			}
		}

		#endregion

		/// <summary>Returns a <see cref="System.String"/> that represents this instance.</summary>
		/// <returns>A <see cref="System.String"/> that represents this instance.</returns>
		public override string ToString()
		{
			return _view.Text;
		}

		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_rotatedImage != null)
				{
					_rotatedImage.Dispose();
					_rotatedImage = null;
				}
			}
		}

		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}
	}
}
