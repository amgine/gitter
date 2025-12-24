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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

/// <summary>Command button with WinVista/Win7 Command Link style.</summary>
[ToolboxBitmap(typeof(CommandLink), "gitter.Framework.Properties.ui-button.png")]
[DesignerCategory("")]
#if NET6_0_OR_GREATER
[global::System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
public class CommandLink : Control
{
	#region Static

	private static readonly IDpiBoundValue<Font> _titleFont       = DpiBoundValue.Font(new((SystemFonts.IconTitleFont ?? SystemFonts.DefaultFont).FontFamily, 12f, FontStyle.Regular));
	private static readonly IDpiBoundValue<Font> _descriptionFont = DpiBoundValue.Font(new((SystemFonts.IconTitleFont ?? SystemFonts.DefaultFont).FontFamily, 8.25f, FontStyle.Regular));
	private static readonly Color _textColor = Color.FromArgb(0, 51, 153);

	private static readonly StringFormat _descriptionStringFormat = new(StringFormat.GenericTypographic)
	{
		Alignment   = StringAlignment.Near,
		Trimming    = StringTrimming.Character,
		FormatFlags = StringFormatFlags.NoClip,
	};

	#endregion

	#region Data

	private bool _hovered;
	private bool _pressed;

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
	public IImageProvider? Image
	{
		get;
		set
		{
			field = value;
			Invalidate();
		}
	}

	/// <summary>Description text.</summary>
	[DefaultValue(null)]
	[Description("Description text")]
	public string? Description
	{
		get;
		set
		{
			field = value;
			Invalidate();
		}
	}

	#endregion

	#region Overrides

	/// <inheritdoc/>
	protected override void OnMouseEnter(EventArgs e)
	{
		base.OnMouseEnter(e);
		if(!_hovered)
		{
			_hovered = true;
			Invalidate();
		}
	}

	/// <inheritdoc/>
	protected override void OnMouseLeave(EventArgs e)
	{
		base.OnMouseLeave(e);
		if(_hovered)
		{
			_hovered = false;
			Invalidate();
		}
	}

	/// <inheritdoc/>
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

	/// <inheritdoc/>
	protected override void OnGotFocus(EventArgs e)
	{
		base.OnGotFocus(e);
		Invalidate();
	}

	/// <inheritdoc/>
	protected override void OnLostFocus(EventArgs e)
	{
		base.OnLostFocus(e);
		Invalidate();
	}

	/// <inheritdoc/>
	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		Invalidate();
	}

	/// <inheritdoc/>
	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
		_pressed = true;
		Focus();
		Invalidate();
	}

	/// <inheritdoc/>
	protected override void OnMouseUp(MouseEventArgs e)
	{
		base.OnMouseUp(e);
		_pressed = false;
		Invalidate();
	}

	/// <inheritdoc/>
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

	/// <inheritdoc/>
	protected sealed override void OnPaintBackground(PaintEventArgs pevent)
	{
	}

	/// <inheritdoc/>
	protected override void OnTextChanged(EventArgs e)
	{
		Invalidate();
		base.OnTextChanged(e);
	}

	private IBackgroundStyle? GetBackgroundStyle()
	{
		var styles = GitterApplication.Style.ItemBackgroundStyles;
		if(Focused)
		{
			if(_pressed) return styles.SelectedFocused;
			if(_hovered) return styles.SelectedFocused;
			return styles.SelectedNoFocus;
		}
		else
		{
			if(_hovered) return styles.Hovered;
		}
		return default;
	}

	private bool PaintIcon(Graphics graphics, Rectangle iconBounds)
	{
		var imageProvider = Image ?? (_hovered ? CommonIcons.ActionHover : CommonIcons.Action);
		var image = imageProvider.GetImage(iconBounds.Width);
		if(image is null) return false;
		graphics.DrawImage(image, iconBounds);
		return true;
	}

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		var rc = e.ClipRectangle;
		if(rc.Width <= 0 || rc.Height <= 0) return;

		var dpi     = Dpi.FromControl(this);
		var dpiConv = DpiConverter.FromDefaultTo(dpi);

		var graphics = e.Graphics;
		graphics.GdiFill(BackColor, e.ClipRectangle);
		using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
		{
			graphics.TextRenderingHint = GraphicsUtility.TextRenderingHint;

			GetBackgroundStyle()?.Draw(graphics, new(dpi, ClientRectangle, e.ClipRectangle));

			var textColor = GitterApplication.Style.Type == GitterStyleType.DarkBackground
				? GitterApplication.Style.Colors.WindowText
				: _textColor;

			if(!string.IsNullOrEmpty(Description))
			{
				var iconBounds = dpiConv.Convert(new Rectangle(5, 15, 16, 16));
				if(_hovered && _pressed)
				{
					iconBounds.Offset(dpiConv.ConvertX(1), dpiConv.ConvertY(1));
				}
				PaintIcon(graphics, iconBounds);

				var textBounds = new Rectangle(dpiConv.ConvertX(25), dpiConv.ConvertY(8), Width - dpiConv.ConvertX(30), dpiConv.ConvertY(21));
				if(_hovered && _pressed)
				{
					textBounds.Offset(dpiConv.ConvertX(1), dpiConv.ConvertY(1));
				}
				if(!string.IsNullOrEmpty(Text))
				{
					GitterApplication.TextRenderer.DrawText(
						graphics, Text, _titleFont.GetValue(dpi), textColor, textBounds);
				}
				textBounds = new Rectangle(
					textBounds.X,
					textBounds.Y + textBounds.Height,
					textBounds.Width,
					Height - dpiConv.ConvertY(4) - textBounds.Bottom);
				GitterApplication.TextRenderer.DrawText(
					graphics, Description!, _descriptionFont.GetValue(dpi), textColor, textBounds, _descriptionStringFormat);
			}
			else
			{
				var iconSize   = dpiConv.Convert(new Size(16, 16));
				var iconBounds = new Rectangle(dpiConv.ConvertX(5), (Height - iconSize.Height) / 2, iconSize.Width, iconSize.Height);
				if(_hovered && _pressed)
				{
					iconBounds.Offset(dpiConv.ConvertX(1), dpiConv.ConvertY(1));
				}
				PaintIcon(graphics, iconBounds);

				var textBounds = new Rectangle(dpiConv.ConvertX(25), 0, Width - dpiConv.ConvertX(30), Height);
				if(_hovered && _pressed)
				{
					textBounds.Offset(dpiConv.ConvertX(1), dpiConv.ConvertY(1));
				}
				if(!string.IsNullOrEmpty(Text))
				{
					GitterApplication.TextRenderer.DrawText(
						graphics, Text, _titleFont.GetValue(dpi), textColor, textBounds);
				}
			}
		}
	}

	#endregion
}
