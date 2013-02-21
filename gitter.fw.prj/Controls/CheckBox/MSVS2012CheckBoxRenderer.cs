namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	sealed class MSVS2012CheckBoxRenderer : CustomCheckBoxRenderer
	{
		public static readonly IColorTable DarkColor = new DarkColorTable();

		public interface IColorTable
		{
			Color Foreground { get; }

			Color ForegroundHighlight { get; }

			Color ForegroundDisabled { get; }

			Color CheckBackground { get; }

			Color CheckBackgroundHighlight { get; }

			Color CheckHighlight { get; }
		}

		private sealed class DarkColorTable : IColorTable
		{
			private static readonly Color FOREGROUND = Color.FromArgb(153, 153, 153);
			private static readonly Color FOREGROUND_HIGHLIGHT = Color.FromArgb(241, 241, 241);
			private static readonly Color FOREGROUND_DISABLED = Color.FromArgb(101, 101, 101);
			private static readonly Color CHECK_BACKGROUND = Color.FromArgb(63, 63, 63);
			private static readonly Color CHECK_BACKGROUND_HIGHLIGHT = Color.FromArgb(70, 70, 70);
			private static readonly Color CHECK_HIGHLIGHT = Color.FromArgb(85, 170, 255);

			public Color Foreground
			{
				get { return FOREGROUND; }
			}

			public Color ForegroundHighlight
			{
				get { return FOREGROUND_HIGHLIGHT; }
			}

			public Color ForegroundDisabled
			{
				get { return FOREGROUND_DISABLED; }
			}

			public Color CheckBackground
			{
				get { return CHECK_BACKGROUND; }
			}

			public Color CheckBackgroundHighlight
			{
				get { return CHECK_BACKGROUND_HIGHLIGHT; }
			}

			public Color CheckHighlight
			{
				get { return CHECK_HIGHLIGHT; }
			}
		}

		private readonly IColorTable _colorTable;

		public MSVS2012CheckBoxRenderer(IColorTable colorTable)
		{
			if(colorTable == null) throw new ArgumentNullException("colorTable");

			_colorTable = colorTable;
		}

		private IColorTable ColorTable
		{
			get { return _colorTable; }
		}

		public override void Render(Graphics graphics, Rectangle clipRectangle, CustomCheckBox checkBox)
		{
			const int CheckBoxSize = 16;
			using(var brush = new SolidBrush(checkBox.BackColor))
			{
				graphics.FillRectangle(brush, clipRectangle);
			}
			bool highlight;
			Color foregroundColor;
			if(checkBox.Enabled)
			{
				highlight = checkBox.IsMouseOver || checkBox.Focused;
				foregroundColor = highlight ? ColorTable.ForegroundHighlight : ColorTable.Foreground;
			}
			else
			{
				highlight = false;
				foregroundColor = ColorTable.ForegroundDisabled;
			}
			var rcCheckBox = new Rectangle(1, 1 + (checkBox.Height - CheckBoxSize) / 2, CheckBoxSize - 2, CheckBoxSize - 2);
			using(var brush = new SolidBrush(highlight ? ColorTable.CheckBackgroundHighlight : ColorTable.CheckBackground))
			{
				graphics.FillRectangle(brush, rcCheckBox);
			}
			rcCheckBox.X -= 1;
			rcCheckBox.Y -= 1;
			rcCheckBox.Width += 1;
			rcCheckBox.Height += 1;
			if(highlight)
			{
				using(var pen = new Pen(ColorTable.CheckHighlight))
				{
					graphics.DrawRectangle(pen, rcCheckBox);
				}
			}
			switch(checkBox.CheckState)
			{
				case CheckState.Checked:
					{
						var path = new Point[]
						{
							new Point(4,   7 + rcCheckBox.Y),
							new Point(6,  10 + rcCheckBox.Y),
							new Point(11,  3 + rcCheckBox.Y),
						};
						var mode = graphics.SmoothingMode;
						graphics.SmoothingMode = SmoothingMode.HighQuality;
						using(var pen = new Pen(foregroundColor, 1.7f))
						{
							graphics.DrawLines(pen, path);
						}
						graphics.SmoothingMode = mode;
					}
					break;
				case CheckState.Indeterminate:
					{
						var rect = new Rectangle(rcCheckBox.X + 5, rcCheckBox.Y + 5, rcCheckBox.Width - 9, rcCheckBox.Height - 9);
						using(var brush = new SolidBrush(foregroundColor))
						{
							graphics.FillRectangle(brush, rect);
						}
					}
					break;
			}
			var text = checkBox.Text;
			if(!string.IsNullOrWhiteSpace(text))
			{
				TextRenderer.DrawText(
					graphics,
					text,
					checkBox.Font,
					new Rectangle(16, 0, checkBox.Width - 16, checkBox.Height),
					foregroundColor,
					TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
			}
		}
	}
}
