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

namespace gitter.Framework.Controls;

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

public abstract class FlowPanel
{
	private FlowLayoutControl? _flowControl;

	protected ref struct PaintContext
	{
		private Rectangle _bounds;

		public const TextFormatFlags MeasureTextFormatFlags
			= TextFormatFlags.NoPadding;

		public const TextFormatFlags RenderTextFormatFlags =
			TextFormatFlags.NoPadding |
			TextFormatFlags.PreserveGraphicsClipping |
			TextFormatFlags.VerticalCenter;

		public PaintContext(FlowLayoutControl owner, FlowPanelPaintEventArgs paintEventArgs)
		{
			DpiConverter  = DpiConverter.FromDefaultTo(owner);
			Style         = owner.Style;
			Font          = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(paintEventArgs.Dpi);
			Graphics      = paintEventArgs.Graphics;
			_bounds       = paintEventArgs.Bounds;
			InitialBounds = _bounds;
			Clip          = paintEventArgs.ClipRectangle;

			var h = MeasureText("W").Height;
			_bounds.Height = h;
		}

		public readonly Graphics Graphics { get; }

		public IGitterStyle Style { get; }

		public Font Font { get; }

		public DpiConverter DpiConverter { get; }

		public Rectangle InitialBounds { get; }

		public Rectangle Bounds
		{
			readonly get => _bounds;
			set => _bounds = value;
		}

		public readonly Rectangle Clip { get; }

		public void NextLine()
		{
			_bounds.X     = InitialBounds.X;
			_bounds.Y    += _bounds.Height;
			_bounds.Width = InitialBounds.Width;
			Advance(DpiConverter.ConvertX(70));
		}

		public void Advance(int width)
		{
			_bounds.X     += width;
			_bounds.Width -= width;
		}

		public readonly Rectangle MeasureText(string? text)
			=> MeasureText(text, Font);

		public readonly Rectangle MeasureText(string? text, Font font)
		{
			if(_bounds.Width <= 0) return Rectangle.Empty;

			var size = TextRenderer.MeasureText(
				Graphics, text, font, new(short.MaxValue, short.MaxValue), MeasureTextFormatFlags);
			return new(Bounds.X, Bounds.Y + (Bounds.Height - size.Height) / 2, size.Width, size.Height);
		}

#if NETCOREAPP

		public readonly Rectangle MeasureText(ReadOnlySpan<char> text)
			=> MeasureText(text, Font);

		public readonly Rectangle MeasureText(ReadOnlySpan<char> text, Font font)
		{
			if(_bounds.Width <= 0) return Rectangle.Empty;

			var size = TextRenderer.MeasureText(
				Graphics, text, font, new(short.MaxValue, short.MaxValue), MeasureTextFormatFlags);
			return new(Bounds.X, Bounds.Y + (Bounds.Height - size.Height) / 2, size.Width, size.Height);
		}

#endif

		public Rectangle MeasureTextAndAdvance(string? text)
			=> MeasureTextAndAdvance(text, Font);

		public Rectangle MeasureTextAndAdvance(string? text, Font font)
		{
			if(_bounds.Width <= 0) return Rectangle.Empty;

			var bounds = MeasureText(text, font);
			Advance(bounds.Width);
			return bounds;
		}

#if NETCOREAPP

		public Rectangle MeasureTextAndAdvance(ReadOnlySpan<char> text)
			=> MeasureTextAndAdvance(text, Font);

		public Rectangle MeasureTextAndAdvance(ReadOnlySpan<char> text, Font font)
		{
			if(_bounds.Width <= 0) return Rectangle.Empty;

			var bounds = MeasureText(text, font);
			Advance(bounds.Width);
			return bounds;
		}

#endif

		public Rectangle RenderTextAndAdvance(string? text, Color color)
			=> RenderTextAndAdvance(text, Font, color);

		public Rectangle RenderTextAndAdvance(string? text, Font font, Color color)
		{
			if(_bounds.Width <= 0) return Rectangle.Empty;

			var bounds = MeasureText(text, font);
			if(Clip.IntersectsWith(bounds))
			{
				TextRenderer.DrawText(
					Graphics, text, font, _bounds, color, RenderTextFormatFlags);
			}
			Advance(bounds.Width);
			return bounds;
		}

		public Rectangle RenderLinkTextAndAdvance(string? text, bool isHovered)
		{
			if(!isHovered) return RenderTextAndAdvance(text, Style.Colors.HyperlinkText);
			using var font = new Font(Font, FontStyle.Underline);
			return RenderTextAndAdvance(text, font, Style.Colors.HyperlinkTextHotTrack);
		}

#if NETCOREAPP

		public Rectangle RenderTextAndAdvance(scoped ReadOnlySpan<char> text, Color color)
			=> RenderTextAndAdvance(text, Font, color);

		public Rectangle RenderTextAndAdvance(scoped ReadOnlySpan<char> text, Font font, Color color)
		{
			if(_bounds.Width <= 0) return Rectangle.Empty;

			var bounds = MeasureText(text, font);
			if(Clip.IntersectsWith(bounds))
			{
				TextRenderer.DrawText(
					Graphics, text, font, _bounds, color, RenderTextFormatFlags);
			}
			Advance(bounds.Width);
			return bounds;
		}

		public Rectangle RenderLinkTextAndAdvance(scoped ReadOnlySpan<char> text, bool isHovered)
		{
			if(!isHovered) return RenderTextAndAdvance(text, Style.Colors.HyperlinkText);
			using var font = new Font(Font, FontStyle.Underline);
			return RenderTextAndAdvance(text, font, Style.Colors.HyperlinkTextHotTrack);
		}

#endif

		public Rectangle RenderIconAndAdvance(IImageProvider? image)
			=> image?.GetImage(DpiConverter.ConvertX(16)) is { } icon
				? RenderIconAndAdvance(icon)
				: Rectangle.Empty;

		public Rectangle RenderIconAndAdvance(Image image)
		{
			var iconBounds  = Bounds;
			iconBounds.Size = DpiConverter.Convert(new Size(16, 16));
			iconBounds.Y   += (Bounds.Height - iconBounds.Height) / 2;
			if(Clip.IntersectsWith(iconBounds))
			{
				Graphics.DrawImage(image, iconBounds);
			}
			Advance(iconBounds.Width);
			return iconBounds;
		}

		public Rectangle RenderCount(IImageProvider icon, int count)
		{
			var x0 = Bounds.X;
			var h = 0;
			if(icon?.GetImage(DpiConverter.ConvertX(16)) is { } image)
			{
				var i0 = RenderIconAndAdvance(image).Height;
				h = Math.Max(h, i0);
				Advance(DpiConverter.ConvertX(2));
			}

#if NET5_0_OR_GREATER
			Span<char> chars = stackalloc char[11];
			if(count.TryFormat(chars, out int charsWritten, provider: CultureInfo.InvariantCulture))
			{
				chars = chars.Slice(0, charsWritten);
				var i1 = RenderTextAndAdvance(chars, count != 0 ? Style.Colors.WindowText : Style.Colors.GrayText).Height;
				h = Math.Max(h, i1);
			}
			else
			{
#endif
				var text = count.ToString(CultureInfo.InvariantCulture);
				var i2 = RenderTextAndAdvance(text, count != 0 ? Style.Colors.WindowText : Style.Colors.GrayText).Height;
				h = Math.Max(h, i2);
#if NET5_0_OR_GREATER
			}
#endif
			var x1 = Bounds.X;
			var bounds = new Rectangle(x0, Bounds.Y + (Bounds.Height - h) / 2, x1 - x0, h);
			Advance(DpiConverter.ConvertX(4));
			return bounds;
		}

		public void RenderHeader(string text)
		{
			var grayText   = Style.Colors.GrayText;
			var textBounds = _bounds;
			textBounds.Width = DpiConverter.ConvertX(66);
			var size = TextRenderer.MeasureText(
				Graphics, text, Font, new(short.MaxValue, short.MaxValue), MeasureTextFormatFlags);
			textBounds.Height = size.Height;
			if(Clip.IntersectsWith(textBounds))
			{
				TextRenderer.DrawText(
					Graphics, text, Font, textBounds, grayText, RenderTextFormatFlags | TextFormatFlags.Right);
			}

			Advance(DpiConverter.ConvertX(70));
		}

		public Rectangle RenderRightAlignedLink(string viewOnGitlabText, bool isHovered)
		{
			var textBounds = InitialBounds;
			textBounds.Width -= DpiConverter.ConvertX(5);
			textBounds.Height = _bounds.Height;

			var s = MeasureText(viewOnGitlabText);

			if(s.Width > Bounds.Width - DpiConverter.ConvertX(4))
			{
				return Rectangle.Empty;
			}

			var bounds = textBounds;
			bounds.Height = s.Height;
			bounds.X -= InitialBounds.X;
			bounds.Y -= InitialBounds.Y;
			bounds.X += bounds.Width - s.Width;
			bounds.Width = s.Width;

			if(isHovered)
			{
				using var u = new Font(Font, FontStyle.Underline);
				TextRenderer.DrawText(
					Graphics, viewOnGitlabText, u, textBounds, Style.Colors.HyperlinkTextHotTrack, RenderTextFormatFlags | TextFormatFlags.Right);
			}
			else
			{
				TextRenderer.DrawText(
					Graphics, viewOnGitlabText, Font, textBounds, Style.Colors.HyperlinkText, RenderTextFormatFlags | TextFormatFlags.Right);
			}
			return bounds;
		}

		public void Dispose()
		{
		}
	}

	/// <summary>Create <see cref="FlowPanel"/>.</summary>
	protected FlowPanel()
	{
	}

	/// <summary>Host <see cref="FlowLayoutControl"/>.</summary>
	public FlowLayoutControl? FlowControl
	{
		get => _flowControl;
		internal set
		{
			if(_flowControl == value) return;

			if(_flowControl is not null)
			{
				OnFlowControlDetached(_flowControl);
			}
			_flowControl = value;
			if(_flowControl is not null)
			{
				OnFlowControlAttached(_flowControl);
			}
		}
	}

	protected IGitterStyle Style => _flowControl?.Style ?? GitterApplication.DefaultStyle;

	public virtual FlowPanelHeader? Header => null;

	public Rectangle Bounds
	{
		get
		{
			if(FlowControl is null) return Rectangle.Empty;
			return FlowControl.GetPanelBounds(this);
		}
	}

	public virtual void InvalidateSize()
		=> FlowControl?.InvalidatePanelSize(this);

	public void Invalidate()
		=> FlowControl?.InvalidatePanel(this);

	public void Invalidate(Rectangle rect)
		=> FlowControl?.InvalidatePanel(this, rect);

	public void InvalidateSafe()
	{
		var control = FlowControl;
		if(control is { Created: true, IsDisposed: false })
		{
			if(control.InvokeRequired)
			{
				try
				{
					control.BeginInvoke(new MethodInvoker(Invalidate), null);
				}
				catch(ObjectDisposedException)
				{
				}
			}
			else
			{
				Invalidate();
			}
		}
	}

	public void InvalidateSafe(Rectangle rect)
	{
		var control = FlowControl;
		if(control is { Created: true, IsDisposed: false })
		{
			if(control.InvokeRequired)
			{
				try
				{
					control.BeginInvoke(new Action<Rectangle>(Invalidate), rect);
				}
				catch(ObjectDisposedException)
				{
				}
			}
			else
			{
				Invalidate(rect);
			}
		}
	}

	public void ScrollIntoView()
		=> FlowControl?.ScrollIntoView(this);

	public void Remove()
	{
		Verify.State.IsTrue(FlowControl != null);

		FlowControl!.Panels.Remove(this);
	}

	public void RemoveSafe()
	{
		var control = FlowControl;
		Verify.State.IsTrue(control != null);

		control!.BeginInvoke(new Func<FlowPanel, bool>(control.Panels.Remove), [this]);
	}

	protected virtual void OnFlowControlAttached(FlowLayoutControl flowControl) { }

	protected virtual void OnFlowControlDetached(FlowLayoutControl flowControl) { }

	protected abstract Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs);

	protected abstract void OnPaint(FlowPanelPaintEventArgs paintEventArgs);

	protected virtual void OnMouseEnter()
	{
	}

	protected virtual void OnMouseLeave()
	{
	}

	protected virtual void OnMouseMove(int x, int y)
	{
	}

	protected virtual void OnMouseDown(int x, int y, MouseButtons button)
	{
	}

	protected virtual void OnMouseUp(int x, int y, MouseButtons button)
	{
	}

	protected virtual void OnMouseDoubleClick(int x, int y, MouseButtons button)
	{
	}

	public void ShowContextMenu(ContextMenuStrip menu, int x, int y)
	{
		Verify.Argument.IsNotNull(menu);
		Verify.State.IsTrue(FlowControl != null);

		var bounds = FlowControl!.GetPanelBounds(this);
		menu.Show(FlowControl,
			bounds.X + x - FlowControl.HScrollPos,
			bounds.Y + y - FlowControl.VScrollPos);
	}

	internal void MouseEnter()
	{
		OnMouseEnter();
	}

	internal void MouseLeave()
	{
		OnMouseLeave();
	}

	internal void MouseMove(int x, int y)
	{
		OnMouseMove(x, y);
	}

	internal void MouseDown(int x, int y, MouseButtons button)
	{
		OnMouseDown(x, y, button);
	}

	internal void DoubleClick(int x, int y, MouseButtons button)
	{
		OnMouseDoubleClick(x, y, button);
	}

	internal void MouseUp(int x, int y, MouseButtons button)
	{
		OnMouseUp(x, y, button);
	}

	public Size Measure(FlowPanelMeasureEventArgs measureEventArgs)
	{
		return OnMeasure(measureEventArgs);
	}

	public void Paint(FlowPanelPaintEventArgs paintEventArgs)
	{
		OnPaint(paintEventArgs);
	}
}
