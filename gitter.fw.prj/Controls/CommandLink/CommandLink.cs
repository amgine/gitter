namespace gitter.Framework.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	/// <summary>Command button with WinVista/Win7 Command Link style.</summary>
	[ToolboxBitmap(typeof(CommandLink), "gitter.Framework.Properties.ui-button.png")]
	public class CommandLink : Control
	{
		#region Static

		private static readonly Font _titleFont = new Font(SystemFonts.IconTitleFont.FontFamily, 12f);
		private static readonly Font _descriptionFont = new Font(SystemFonts.IconTitleFont.FontFamily, 8.25f);
		private static readonly Brush _textBrush = new SolidBrush(Color.FromArgb(0,51,153));

		private static readonly StringFormat DescriptionStringFormat = new StringFormat(StringFormat.GenericTypographic)
		{
			Alignment = StringAlignment.Near,
			Trimming = StringTrimming.Character,
			FormatFlags = StringFormatFlags.NoClip,
		};

		#endregion

		#region Data

		private bool _hovered;
		private bool _pressed;
		private string _description;
		private Image _image;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="CommandLink"/>.</summary>
		public CommandLink()
		{
			SetStyle(
				ControlStyles.ContainerControl |
				ControlStyles.ResizeRedraw |
				ControlStyles.SupportsTransparentBackColor,
				false);
			SetStyle(
				ControlStyles.UserPaint |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.Selectable |
				ControlStyles.OptimizedDoubleBuffer,
				true);
		}

		#endregion

		#region Properties

		/// <summary>Button image.</summary>
		[DefaultValue(null)]
		[Description("Button image")]
		public Image Image
		{
			get { return _image; }
			set
			{
				_image = value;
				Invalidate();
			}
		}

		/// <summary>Descriptin text.</summary>
		[DefaultValue("")]
		[Description("Description text")]
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				Invalidate();
			}
		}

		#endregion

		#region Overrides

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			if(!_hovered)
			{
				_hovered = true;
				Invalidate();
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			if(_hovered)
			{
				_hovered = false;
				Invalidate();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if(_pressed)
			{
				if(ClientRectangle.Contains(e.Location))
				{
					if(!_hovered)
					{
						_hovered = true;
						Invalidate();
					}
				}
				else
				{
					if(_hovered)
					{
						_hovered = false;
						Invalidate();
					}
				}
			}
			base.OnMouseMove(e);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			Invalidate();
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			Invalidate();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			_pressed = true;
			Focus();
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			_pressed = false;
			Invalidate();
		}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Enter:
					e.IsInputKey = true;
					OnClick(EventArgs.Empty);
					break;
			}
			base.OnPreviewKeyDown(e);
		}

		protected sealed override void OnPaintBackground(PaintEventArgs pevent)
		{
		}

		protected override void OnTextChanged(EventArgs e)
		{
			Invalidate();
			base.OnTextChanged(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var rc = e.ClipRectangle;
			if(rc.Width <= 0 || rc.Height <= 0) return;
			var g = e.Graphics;
			g.Clear(BackColor);
			g.SetClip(e.ClipRectangle);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.TextRenderingHint = Utility.TextRenderingHint;
			if(Focused)
			{
				if(_pressed)
				{
					BackgroundStyle.SelectedFocused.Draw(g, ClientRectangle);
				}
				else
				{
					if(_hovered)
					{
						BackgroundStyle.SelectedFocused.Draw(g, ClientRectangle);
					}
					else
					{
						BackgroundStyle.SelectedNoFocus.Draw(g, ClientRectangle);
					}
				}
			}
			else
			{
				if(_hovered)
				{
					BackgroundStyle.Hovered.Draw(g, ClientRectangle);
				}
			}
			if(!string.IsNullOrEmpty(_description))
			{
				var loc = new Point(5, 15);
				if(_hovered && _pressed) loc.Offset(1, 1);
				if(_image == null)
					g.DrawImage(_hovered ? Resources.ImgActionHover : Resources.ImgAction, loc);
				else
					g.DrawImage(_image, loc);
				var r = new Rectangle(25, 8, Width - 30, 21);
				if(_hovered && _pressed) r.Offset(1, 1);
				if(!string.IsNullOrEmpty(Text))
				{
					GitterApplication.TextRenderer.DrawText(
						g, Text, _titleFont, _textBrush, r);
				}
				r = new Rectangle(r.X, r.Y + r.Height, r.Width, Height - 8 - r.Bottom);
				GitterApplication.TextRenderer.DrawText(
					g, _description, _descriptionFont, _textBrush, r, DescriptionStringFormat);
			}
			else
			{
				var loc = new Point(5, (Height - 16) / 2);
				if(_hovered && _pressed) loc.Offset(1, 1);
				if(_image == null)
					g.DrawImage(_hovered ? Resources.ImgActionHover : Resources.ImgAction, loc);
				else
					g.DrawImage(_image, loc);
				var r = new Rectangle(25, 0, Width - 30, Height);
				if(_hovered && _pressed) r.Offset(1, 1);
				if(!string.IsNullOrEmpty(Text))
				{
					GitterApplication.TextRenderer.DrawText(
						g, Text, _titleFont, _textBrush, r);
				}
			}
		}

		#endregion
	}
}
