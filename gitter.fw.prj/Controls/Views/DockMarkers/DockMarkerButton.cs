namespace gitter.Framework.Controls
{
	using System.Drawing;
	using System.Drawing.Drawing2D;

	/// <summary>Button which indicates specific docking position on dock marker.</summary>
	public sealed class DockMarkerButton
	{
		#region Static Data

		private static readonly Color ButtonBorder = Color.FromArgb(138, 145, 156);
		private static readonly Color ButtonBackgroundStart = Color.FromArgb(245, 248, 251);
		private static readonly Color ButtonBackgroundEnd = Color.FromArgb(222, 226, 233);

		private static readonly Color ButtonContentBorder = Color.FromArgb(68, 88, 121);
		private static readonly Color ButtonContentStart = Color.FromArgb(253, 231, 165);
		private static readonly Color ButtonContentEnd = Color.FromArgb(247, 198, 113);

		#endregion

		#region Data

		private readonly Rectangle _bounds;
		private readonly DockResult _type;

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="DockMarkerButton"/> class.</summary>
		/// <param name="bounds">Button bounds.</param>
		/// <param name="type">Button type.</param>
		public DockMarkerButton(Rectangle bounds, DockResult type)
		{
			_bounds = bounds;
			_type = type;
		}

		#endregion

		#region Properties

		/// <summary>Gets the bounds of this <see cref="DockMarkerButton"/>.</summary>
		/// <value>Bounds of this <see cref="DockMarkerButton"/>.</value>
		public Rectangle Bounds
		{
			get { return _bounds; }
		}

		/// <summary>Gets the docking position associated with this button.</summary>
		/// <value>Docking position associated with this button.</value>
		public DockResult Type
		{
			get { return _type; }
		}

		#endregion

		private void PaintButtonBackground(Graphics graphics, bool hover)
		{
			Color start, end, border;
			if(hover)
			{
				start = ButtonBackgroundStart;
				end = ButtonBackgroundEnd;
				border = ButtonBorder;
			}
			else
			{
				start = Color.FromArgb((byte)(ViewConstants.OpacityNormal * 255),
					ButtonBackgroundStart.R,
					ButtonBackgroundStart.G,
					ButtonBackgroundStart.B);
				end = Color.FromArgb((byte)(ViewConstants.OpacityNormal * 255),
					ButtonBackgroundEnd.R,
					ButtonBackgroundEnd.G,
					ButtonBackgroundEnd.B);
				border = Color.FromArgb((byte)(ViewConstants.OpacityNormal * 255),
					ButtonBorder.R,
					ButtonBorder.G,
					ButtonBorder.B);
			}
			using(var pen = new Pen(border))
			using(var brush = new LinearGradientBrush(Bounds,
				start, end, LinearGradientMode.Vertical))
			{
				graphics.FillRoundedRectangle(brush, pen, Bounds, 2);
			}
		}

		private static void InitButtonContentColors(bool hover, out Color start, out Color end, out Color border)
		{
			if(hover)
			{
				start = ButtonContentStart;
				end = ButtonContentEnd;
				border = ButtonContentBorder;
			}
			else
			{
				start = Color.FromArgb((byte)(ViewConstants.OpacityNormal * 255),
					ButtonContentStart.R,
					ButtonContentStart.G,
					ButtonContentStart.B);
				end = Color.FromArgb((byte)(ViewConstants.OpacityNormal * 255),
					ButtonContentEnd.R,
					ButtonContentEnd.G,
					ButtonContentEnd.B);
				border = Color.FromArgb((byte)(ViewConstants.OpacityNormal * 255),
					ButtonContentBorder.R,
					ButtonContentBorder.G,
					ButtonContentBorder.B);
			}
		}

		private static void PaintDockTopButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color start, end, border;
			InitButtonContentColors(hover, out start, out end, out border);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 24, 12);
			var arrow = new PointF[]
				{
					new PointF(rect.X + 11.5f, rect.Y + 25.5f),
					new PointF(rect.X + 15.5f, rect.Y + 21.5f),
					new PointF(rect.X + 16.5f, rect.Y + 21.5f),
					new PointF(rect.X + 19.5f, rect.Y + 25.5f),
				};
			using(var brush = new SolidBrush(border))
			{
				graphics.FillRectangle(brush, rc);
				graphics.FillPolygon(brush, arrow);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new LinearGradientBrush(rc,
				start, end, LinearGradientMode.Vertical))
			{
				if(!hover)
					graphics.FillRectangle(Brushes.White, rc);
				graphics.FillRectangle(brush, rc);
			}
		}

		private static void PaintDockDocumentTopButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color start, end, border;
			InitButtonContentColors(hover, out start, out end, out border);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 24, 24);
			using(var brush = new SolidBrush(border))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new LinearGradientBrush(rc,
				start, end, LinearGradientMode.Vertical))
			{
				graphics.FillRectangle(Brushes.White, rc);
				rc.Height = 11;
				graphics.FillRectangle(brush, rc);
			}
			using(var pen = new Pen(border)
			{
				DashStyle = DashStyle.Dot,
			})
			{
				graphics.DrawLine(pen, rc.X, rc.Bottom - 1, rc.Right, rc.Bottom - 1);
			}
		}

		private static void PaintDockLeftButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color start, end, border;
			InitButtonContentColors(hover, out start, out end, out border);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 12, 24);
			var arrow = new PointF[]
				{
					new PointF(rect.X + 25.5f, rect.Y + 11.5f),
					new PointF(rect.X + 25.5f, rect.Y + 19.5f),
					new PointF(rect.X + 21.5f, rect.Y + 15.5f),
					new PointF(rect.X + 21.5f, rect.Y + 14.5f),
				};
			using(var brush = new SolidBrush(border))
			{
				graphics.FillRectangle(brush, rc);
				graphics.FillPolygon(brush, arrow);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new LinearGradientBrush(rc,
				start, end, LinearGradientMode.Vertical))
			{
				if(!hover)
					graphics.FillRectangle(Brushes.White, rc);
				graphics.FillRectangle(brush, rc);
			}
		}

		private static void PaintDockDocumentLeftButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color start, end, border;
			InitButtonContentColors(hover, out start, out end, out border);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 24, 24);
			using(var brush = new SolidBrush(border))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new LinearGradientBrush(rc,
				start, end, LinearGradientMode.Vertical))
			{
				graphics.FillRectangle(Brushes.White, rc);
				rc.Width = 11;
				graphics.FillRectangle(brush, rc);
			}
			using(var pen = new Pen(border)
			{
				DashStyle = DashStyle.Dot,
			})
			{
				graphics.DrawLine(pen, rc.Right - 1, rc.Y, rc.Right - 1, rc.Bottom);
			}
		}

		private static void PaintDockFillButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color start, end, border;
			InitButtonContentColors(hover, out start, out end, out border);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 24, 24);
			using(var brush = new SolidBrush(border))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new LinearGradientBrush(rc,
				start, end, LinearGradientMode.Vertical))
			{
				if(!hover)
					graphics.FillRectangle(Brushes.White, rc);
				graphics.FillRectangle(brush, rc);
			}
		}

		private static void PaintDockRightButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color start, end, border;
			InitButtonContentColors(hover, out start, out end, out border);
			var rc = new Rectangle(rect.X + 16, rect.Y + 4, 12, 24);
			var arrow = new PointF[]
				{
					new PointF(rect.X + 5.5f, rect.Y + 20.5f),
					new PointF(rect.X + 5.5f, rect.Y + 11.5f),
					new PointF(rect.X + 9.5f, rect.Y + 14.5f),
					new PointF(rect.X + 9.5f, rect.Y + 16.5f),
				};
			using(var brush = new SolidBrush(border))
			{
				graphics.FillRectangle(brush, rc);
				graphics.FillPolygon(brush, arrow);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new LinearGradientBrush(rc,
				start, end, LinearGradientMode.Vertical))
			{
				if(!hover)
					graphics.FillRectangle(Brushes.White, rc);
				graphics.FillRectangle(brush, rc);
			}
		}

		private static void PaintDockDocumentRightButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color start, end, border;
			InitButtonContentColors(hover, out start, out end, out border);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 24, 24);
			using(var brush = new SolidBrush(border))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new LinearGradientBrush(rc,
				start, end, LinearGradientMode.Vertical))
			{
				graphics.FillRectangle(Brushes.White, rc);
				rc.X += 11;
				rc.Width = 11;
				graphics.FillRectangle(brush, rc);
			}
			using(var pen = new Pen(border)
			{
				DashStyle = DashStyle.Dot,
			})
			{
				graphics.DrawLine(pen, rc.X, rc.Y, rc.X, rc.Bottom);
			}
		}

		private static void PaintDockBottomButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color start, end, border;
			InitButtonContentColors(hover, out start, out end, out border);
			var rc = new Rectangle(rect.X + 4, rect.Y + 16, 24, 12);
			var arrow = new PointF[]
				{
					new PointF(rect.X + 10.5f, rect.Y + 5.5f),
					new PointF(rect.X + 15.5f, rect.Y + 9.5f),
					new PointF(rect.X + 16.5f, rect.Y + 9.5f),
					new PointF(rect.X + 20.5f, rect.Y + 5.5f),
				};
			using(var brush = new SolidBrush(border))
			{
				graphics.FillRectangle(brush, rc);
				graphics.FillPolygon(brush, arrow);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new LinearGradientBrush(rc,
				start, end, LinearGradientMode.Vertical))
			{
				if(!hover)
					graphics.FillRectangle(Brushes.White, rc);
				graphics.FillRectangle(brush, rc);
			}
		}

		private static void PaintDockDocumentBottomButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color start, end, border;
			InitButtonContentColors(hover, out start, out end, out border);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 24, 24);
			using(var brush = new SolidBrush(border))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new LinearGradientBrush(rc,
				start, end, LinearGradientMode.Vertical))
			{
				graphics.FillRectangle(Brushes.White, rc);
				rc.Y += 9;
				rc.Height = 11;
				graphics.FillRectangle(brush, rc);
			}
			using(var pen = new Pen(border)
			{
				DashStyle = DashStyle.Dot,
			})
			{
				graphics.DrawLine(pen, rc.X, rc.Y, rc.Right, rc.Y);
			}
		}

		private void PaintButtonContent(Graphics graphics, bool hover)
		{
			switch(Type)
			{
				case DockResult.Top:
					PaintDockTopButton(graphics, Bounds, hover);
					break;
				case DockResult.DocumentTop:
					PaintDockDocumentTopButton(graphics, Bounds, hover);
					break;
				case DockResult.Left:
					PaintDockLeftButton(graphics, Bounds, hover);
					break;
				case DockResult.DocumentLeft:
					PaintDockDocumentLeftButton(graphics, Bounds, hover);
					break;
				case DockResult.Fill:
					PaintDockFillButton(graphics, Bounds, hover);
					break;
				case DockResult.Right:
					PaintDockRightButton(graphics, Bounds, hover);
					break;
				case DockResult.DocumentRight:
					PaintDockDocumentRightButton(graphics, Bounds, hover);
					break;
				case DockResult.Bottom:
					PaintDockBottomButton(graphics, Bounds, hover);
					break;
				case DockResult.DocumentBottom:
					PaintDockDocumentBottomButton(graphics, Bounds, hover);
					break;
			}
		}

		/// <summary>Paints this <see cref="DockMarkerButton"/>.</summary>
		/// <param name="graphics">The graphics surface to draw on.</param>
		/// <param name="hover">Indicates whether this button is hovered.</param>
		public void Paint(Graphics graphics, bool hover)
		{
			PaintButtonBackground(graphics, hover);
			PaintButtonContent(graphics, hover);
		}
	}
}
