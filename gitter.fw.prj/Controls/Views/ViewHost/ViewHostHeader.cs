namespace gitter.Framework.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;
	using System.Text;

	using gitter.Framework.Services;

	using Resources = gitter.Framework.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class ViewHostHeader : Control
	{
		#region Static

		private static readonly Brush BackgroundBrush =
			new SolidBrush(Color.FromArgb(47, 57, 85));

		private static readonly Brush BackgroundNormal =
			new LinearGradientBrush(Point.Empty, new Point(0, ViewConstants.HeaderHeight), Color.FromArgb(77, 96, 130), Color.FromArgb(61, 82, 119));
		private static readonly Brush BackgroundFocused =
			new LinearGradientBrush(Point.Empty, new Point(0, ViewConstants.HeaderHeight), Color.FromArgb(255, 252, 242), Color.FromArgb(255, 232, 166));

		private static readonly StringFormat TextFormat = new StringFormat(StringFormat.GenericDefault)
		{
			Alignment = StringAlignment.Near,
			LineAlignment = StringAlignment.Center,
			FormatFlags =
				StringFormatFlags.FitBlackBox |
				StringFormatFlags.LineLimit,
			Trimming = StringTrimming.EllipsisCharacter,
		};

		private static readonly Brush TextNormal = Brushes.White;
		private static readonly Brush TextFocused = Brushes.Black;

		#endregion

		#region Events

		public event EventHandler<ViewButtonClickEventArgs> HeaderButtonClick
		{
			add { _buttons.ButtonClick += value; }
			remove { _buttons.ButtonClick -= value; }
		}

		#endregion

		#region Data

		private readonly ViewButtons _buttons;
		private ViewHost _viewHost;
		private bool _buttonsHovered;

		#endregion

		const int BetweenTextAndButtons = 2;
		const int BeforeContent = 2;
		const int AfterContent = 2;

		/// <summary>Create <see cref="ViewHostHeader"/>.</summary>
		public ViewHostHeader(ViewHost host)
		{
			if(host == null) throw new ArgumentNullException("host");
			_viewHost = host;

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

			_buttons = new ViewButtons(this);
		}

		public ViewHost ViewHost
		{
			get { return _viewHost; }
		}

		public ViewButtons Buttons
		{
			get { return _buttons; }
		}

		public void SetAvailableButtons(params ViewButtonType[] buttons)
		{
			_buttons.SetAvailableButtons(buttons);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var graphics = e.Graphics;
			var client = ClientRectangle;
			var rect = (RectangleF)client;
			rect.X -= .5f;
			rect.Width += 1;
			rect.Y -= .5f;
			rect.Height += 1;
			bool focused = _viewHost.IsActive;
			graphics.TextRenderingHint = Utility.TextRenderingHint;
			graphics.TextContrast = Utility.TextContrast;
			var textBrush = focused ? TextFocused : TextNormal;
			var backgroundBrush = focused ? BackgroundFocused : BackgroundNormal;
			if(_viewHost.Status == ViewHostStatus.Floating &&
				((Form)_viewHost.TopLevelControl).WindowState == FormWindowState.Maximized)
			{
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.FillRectangle(backgroundBrush, rect);
			}
			else
			{
				graphics.FillRectangle(BackgroundBrush, e.ClipRectangle);
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.FillRoundedRectangle(backgroundBrush, rect, 2, 2, 0, 0);
			}
			rect.X += BeforeContent;
			rect.Width -= BeforeContent;
			if(_buttons.Count != 0)
			{
				rect.Width -= _buttons.Width + BetweenTextAndButtons;
			}
			GitterApplication.TextRenderer.DrawText(
				graphics, Text, GitterApplication.FontManager.UIFont, textBrush, Rectangle.Truncate(rect), TextFormat);
			if(_buttons.Count != 0)
			{
				_buttons.OnPaint(graphics, GetButtonsRect(), focused);
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text
		{
			get { return _viewHost.Text; }
			set
			{
				throw new InvalidOperationException();
			}
		}

		protected override void OnResize(EventArgs e)
		{
			_buttons.Height = Height;
			base.OnResize(e);
		}

		private Rectangle GetButtonsRect()
		{
			var rc = ClientRectangle;
			var w = _buttons.Width;
			return new Rectangle(rc.Width - w - AfterContent, 0, w, rc.Height);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if(_buttons.Count != 0)
			{
				var rc = GetButtonsRect();
				if(rc.Contains(e.X, e.Y))
				{
					var x = e.X - rc.X;
					var y = e.Y - rc.Y;
					_buttons.OnMouseMove(x, y, e.Button);
					_buttonsHovered = true;
				}
				else
				{
					if(_buttonsHovered)
					{
						_buttons.OnMouseLeave();
						_buttonsHovered = false;
					}
					if(_buttons.PressedButton == null)
					{
						base.OnMouseMove(e);
					}
				}
			}
			else
			{
				base.OnMouseMove(e);
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			if(_buttonsHovered)
			{
				_buttons.OnMouseLeave();
				_buttonsHovered = false;
			}
			base.OnMouseLeave(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			ViewHost.Activate();
			if(_buttons.Count != 0)
			{
				var rc = GetButtonsRect();
				if(rc.Contains(e.X, e.Y))
				{
					var x = e.X - rc.X;
					var y = e.Y - rc.Y;
					_buttons.OnMouseDown(x, y, e.Button);
				}
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if(_buttons.Count != 0)
			{
				var rc = GetButtonsRect();
				if(_buttons.PressedButton != null || rc.Contains(e.X, e.Y))
				{
					var x = e.X - rc.X;
					var y = e.Y - rc.Y;
					_buttons.OnMouseUp(x, y, e.Button);
				}
			}
			base.OnMouseUp(e);
		}
	}
}
