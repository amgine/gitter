namespace gitter.Framework
{
	using System;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	sealed class MSVS2010StyleRenderer : ToolStripRenderer
	{
		private static class ColorTable
		{
			public static readonly Color SizingGrip00 = Color.FromArgb(250, 251, 251);
			public static readonly Color SizingGrip10 = Color.FromArgb(199, 207, 222);
			public static readonly Color SizingGrip01 = Color.FromArgb(228, 232, 239);
			public static readonly Color SizingGrip11 = Color.FromArgb(170, 183, 205);

			public static readonly Color Grip = Color.FromArgb(94, 116, 140);
			public static readonly Color PanelBackground = Color.FromArgb(156, 170, 193);

			public static readonly Color MenuStripBackgroundStart = Color.FromArgb(202, 211, 226);
			public static readonly Color MenuStripBackgroundEnd = Color.FromArgb(174, 185, 205);

			public static readonly Color StatusStripBackground = Color.FromArgb(41, 57, 85);

			public static readonly Color DropDownBorder = Color.FromArgb(155, 167, 183);
			public static readonly Color DropDownBackgroundStart = Color.FromArgb(233, 236, 238);
			public static readonly Color DropDownBackgroundEnd = Color.FromArgb(208, 215, 226);

			public static readonly Color ToolStripBorder = Color.FromArgb(201, 210, 225);
			public static readonly Color ToolStripBackground = Color.FromArgb(188, 199, 216);

			public static readonly Color RootMenuItemPressedBorder = Color.FromArgb(155, 167, 183);
			public static readonly Color RootMenuItemPressedBackground = DropDownBackgroundStart;
			public static readonly Color MenuItemSelectedBorder = Color.FromArgb(229, 195, 101);
			public static readonly Color MenuItemSelectedBackgroundStart = Color.FromArgb(255, 251, 240);
			public static readonly Color MenuItemSelectedBackgroundEnd = Color.FromArgb(255, 236, 181);

			public static readonly Color ItemSelectedBorder = Color.FromArgb(229, 195, 101);
			public static readonly Color ItemSelectedBackgroundStart = Color.FromArgb(255, 251, 238);
			public static readonly Color ItemSelectedBackgroundEnd = Color.FromArgb(229, 195, 101);

			public static readonly Color ItemPressedBorder = Color.FromArgb(229, 195, 101);
			public static readonly Color ItemPressedBackground = Color.FromArgb(255, 232, 166);

			public static readonly Color VerticalSeparatorColor = Color.FromArgb(133, 145, 162);
			public static readonly Color HorizontalSeparatorColor = Color.FromArgb(190, 192, 203);

			public static readonly Color CheckboxBorder = Color.FromArgb(229, 195, 101);
			public static readonly Color CheckboxNormal = Color.FromArgb(255, 239, 187);
			public static readonly Color CheckboxSelected = Color.FromArgb(255, 252, 244);

			public static readonly Color StatusLabelBackground = StatusStripBackground;

			public static readonly Color ContentPanelBackground = Color.FromArgb(41, 57, 85);
			public static readonly Color ContentPanelForeground = Color.FromArgb(50, 68, 100);
		}

		private static readonly Bitmap ImgMenuCheck = Resources.ImgMenuCheck;

		#region Stage 1 - Container Backgrounds

		protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
		{
			var strip = e.ToolStrip;
			if(strip is ToolStripDropDown)
			{
				RenderDropDownBackground(e);
			}
			else if(strip is MenuStrip)
			{
				RenderMenuStripBackground(e);
			}
			else if(strip is StatusStrip)
			{
				RenderStatusStripBackground(e);
			}
			else
			{
				RenderToolStripBackgroundInternal(e);
			}
		}

		protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
		{
			var rect = e.AffectedBounds;
			rect.Y += 2;
			rect.Height -= 4;
			using(var brush = new SolidBrush(ColorTable.DropDownBackgroundStart))
			{
				e.Graphics.FillRectangle(brush, rect);
			}
		}

		protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
		{
			//using(var brush = new HatchBrush(HatchStyle.Percent20,
			//    ColorTable.ContentPanelForeground,
			//    ColorTable.ContentPanelBackground))
			using(var brush = new SolidBrush(ColorTable.ContentPanelBackground))
			{
				e.Graphics.FillRectangle(brush, new Rectangle(Point.Empty, e.ToolStripContentPanel.Size));
			}
			e.Handled = true;
		}

		protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
		{
			using(var b = new SolidBrush(ColorTable.PanelBackground))
			{
				e.Graphics.FillRectangle(b, e.ToolStripPanel.Bounds);
			}
			e.Handled = true;
		}

		protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
		{
			int x = e.ToolStrip.Bounds.Width - 13;
			int y = e.ToolStrip.Bounds.Height - 13;
			using(var brush00 = new SolidBrush(ColorTable.SizingGrip00))
			using(var brush10 = new SolidBrush(ColorTable.SizingGrip10))
			using(var brush01 = new SolidBrush(ColorTable.SizingGrip01))
			using(var brush11 = new SolidBrush(ColorTable.SizingGrip11))
			{
				for(int i = 0; i < 4; ++i)
				{
					for(int j = 0; j < 4; ++j)
					{
						if(i + j >= 3)
						{
							e.Graphics.FillRectangle(brush00, new Rectangle(x + i * 3 + 0, y + j * 3 + 0, 1, 1));
							e.Graphics.FillRectangle(brush10, new Rectangle(x + i * 3 + 1, y + j * 3 + 0, 1, 1));
							e.Graphics.FillRectangle(brush01, new Rectangle(x + i * 3 + 0, y + j * 3 + 1, 1, 1));
							e.Graphics.FillRectangle(brush11, new Rectangle(x + i * 3 + 1, y + j * 3 + 1, 1, 1));
						}
					}
				}
			}
		}

		#endregion

		#region Stage 2 - Item Backgrounds

		protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
		{
			var size = e.Item.Size;
			if(e.Vertical)
			{
				var x = size.Width / 2;
				var y = 4;
				using(var pen = new Pen(ColorTable.VerticalSeparatorColor))
				{
					e.Graphics.DrawLine(pen, x, y, x, y + size.Height - 8);
				}
			}
			else
			{
				var x = 0;
				var y = size.Height / 2;
				using(var pen = new Pen(ColorTable.HorizontalSeparatorColor))
				{
					e.Graphics.DrawLine(pen, x + 26, y, x + size.Width - 3, y);
				}
			}
		}

		protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
		{
			using(var brush = new SolidBrush(ColorTable.Grip))
			{
				var x = e.GripBounds.X + 1;
				var y = e.GripBounds.Y + 2;
				switch(e.GripDisplayStyle)
				{
					case ToolStripGripDisplayStyle.Horizontal:
						x += 4;
						while(x < e.GripBounds.Right - 4)
						{
							e.Graphics.FillRectangle(brush, new Rectangle(x, y, 2, 2));
							x += 4;
						}
						break;
					case ToolStripGripDisplayStyle.Vertical:
						y += 4;
						while(y < e.GripBounds.Bottom - 4)
						{
							e.Graphics.FillRectangle(brush, new Rectangle(x, y, 2, 2));
							y += 4;
						}
						break;
				}
			}
		}

		protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
		{
			var item = e.Item as ToolStripButton;
			RenderItemBackgroundInternal(e.Graphics, item.Width, item.Height, item.Pressed, item.Selected || item.Checked);
		}

		protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
		{
			var item = e.Item as ToolStripDropDownButton;
			RenderItemBackgroundInternal(e.Graphics, item.Width, item.Height, item.Pressed, item.Selected && item.Enabled);
			Rectangle arrowBounds = new Rectangle(item.Width - 16, 0, 16, item.Height);

			DrawArrow(new ToolStripArrowRenderEventArgs(
				e.Graphics, e.Item,
				arrowBounds,
				Color.Black, ArrowDirection.Down));
		}

		protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
		{
			RenderItemBackgroundInternal(e.Graphics, e.Item.Width, e.Item.Height, e.Item.Pressed, e.Item.Selected);
		}

		protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
		{
			base.OnRenderLabelBackground(e);
		}

		protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
		{
			base.OnRenderOverflowButtonBackground(e);
		}

		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			var item = e.Item as ToolStripMenuItem;
			RenderMenuItemBackgroundInternal(e.Graphics, 0, 0, item.Width, item.Height, item.Pressed, item.Selected && item.Enabled, e.ToolStrip is MenuStrip);
		}

		protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
		{
			var splitButton = e.Item as ToolStripSplitButton;
			if(splitButton.DropDownButtonPressed)
			{
				RenderItemBackgroundInternal(e.Graphics, e.Item.Width, e.Item.Height, true, e.Item.Selected);
			}
			else
			{
				if(splitButton.ButtonPressed)
				{
					RenderItemBackgroundInternal(e.Graphics,
						splitButton.ButtonBounds.Width + 1,
						splitButton.ButtonBounds.Height,
						true, false);

					RenderItemBackgroundInternal(e.Graphics,
						splitButton.DropDownButtonBounds.X - 1, 0,
						splitButton.DropDownButtonBounds.Width + 1,
						splitButton.DropDownButtonBounds.Height,
						false, true);
				}
				else if(splitButton.Selected)
				{
					RenderItemBackgroundInternal(e.Graphics, e.Item.Width, e.Item.Height, false, true);
					var x = splitButton.ButtonBounds.Right;
					using(var pen = new Pen(Color.FromArgb(229, 195, 101)))
					{
						e.Graphics.DrawLine(pen, x, 0, x, splitButton.Height - 1);
					}
				}
			}

			DrawArrow(new ToolStripArrowRenderEventArgs(
				e.Graphics, e.Item,
				splitButton.DropDownButtonBounds,
				Color.Black, ArrowDirection.Down));
		}

		protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
		{
			using(var b = new SolidBrush(ColorTable.StatusLabelBackground))
			{
				e.Graphics.FillRectangle(b, e.Item.Bounds);
			}
		}

		#endregion

		#region Stage 3 - Item Foreground Effects

		protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
		{
			if(e.Image != null)
			{
				var item = e.Item as ToolStripMenuItem;
				if(item != null && item.Checked)
				{
					RenderItemBackgroundInternal(e.Graphics,
						e.ImageRectangle.X - 2,
						e.ImageRectangle.Y - 2,
						e.ImageRectangle.Width + 4,
						e.ImageRectangle.Height + 4,
						true, false);
				}
				if(!e.Item.Enabled)
					base.OnRenderItemImage(e);
				else
					e.Graphics.DrawImage(e.Image, e.ImageRectangle);
			}
		}

		protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
		{
			Color backColor;
			if(e.Item.Selected)
				backColor = ColorTable.CheckboxSelected;
			else
				backColor = ColorTable.CheckboxNormal;
			using(var brush = new SolidBrush(backColor))
			{
				e.Graphics.FillRectangle(brush, e.ImageRectangle);
			}
			using(var pen = new Pen(ColorTable.CheckboxBorder))
			{
				e.Graphics.DrawRectangle(pen, e.ImageRectangle);
			}
			var checkRect = e.ImageRectangle;
			checkRect.X += (checkRect.Width + 1 - ImgMenuCheck.Width) / 2;
			checkRect.Y += (checkRect.Height + 1 - ImgMenuCheck.Height) / 2;
			checkRect.Width = ImgMenuCheck.Width;
			checkRect.Height = ImgMenuCheck.Height;
			e.Graphics.DrawImage(ImgMenuCheck, checkRect);
		}

		protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
		{
			if(e.Item.Enabled)
				e.ArrowColor = Color.Black;
			else
				e.ArrowColor = SystemColors.GrayText;
			base.OnRenderArrow(e);
		}

		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			if(e.ToolStrip is StatusStrip)
			{
				if(!e.Item.Selected)
					e.TextColor = Color.White;
				else
					e.TextColor = Color.Black;
			}
			else
			{
				e.TextColor = Color.Black;
			}
			base.OnRenderItemText(e);
		}
		
		#endregion

		#region Stage 4 - Paint the borders on the toolstrip if necessary

		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
		{
			base.OnRenderToolStripBorder(e);

			if(e.ToolStrip is ToolStripDropDown)
			{
				RenderDropDownBorder(e);
			}
		}

		#endregion

		private static void RenderMenuStripBackground(ToolStripRenderEventArgs e)
		{
			var strip = e.ToolStrip;
			var rc = new Rectangle(0, 0, strip.Width, strip.Height);
			using(var brush = new LinearGradientBrush(rc,
				ColorTable.MenuStripBackgroundStart,
				ColorTable.MenuStripBackgroundEnd,
				LinearGradientMode.Vertical))
			{
				e.Graphics.FillRectangle(brush, e.AffectedBounds);
			}
		}

		private static void RenderStatusStripBackground(ToolStripRenderEventArgs e)
		{
			using(var brush = new SolidBrush(ColorTable.StatusStripBackground))
			{
				e.Graphics.FillRectangle(brush, e.AffectedBounds);
			}
		}

		private static void RenderDropDownBackground(ToolStripRenderEventArgs e)
		{
			var strip = e.ToolStrip;
			var rc = new Rectangle(1, 1, strip.Width - 2, strip.Height - 2);
			using(var brush = new LinearGradientBrush(rc,
				ColorTable.DropDownBackgroundStart,
				ColorTable.DropDownBackgroundEnd,
				LinearGradientMode.Vertical))
			{
				e.Graphics.FillRectangle(brush, rc);
			}
		}

		private static void RenderToolStripBackgroundInternal(ToolStripRenderEventArgs e)
		{
			if(e.ToolStrip.Stretch)
			{
				using(var brush = new SolidBrush(ColorTable.ToolStripBackground))
				{
					e.Graphics.FillRectangle(brush, e.AffectedBounds);
				}
			}
			else
			{
				using(var brush = new SolidBrush(ColorTable.PanelBackground))
				{
					e.Graphics.FillRectangle(brush, e.AffectedBounds);
				}
				var rc = new Rectangle(Point.Empty, e.ToolStrip.Size);
				rc.Offset(1, 1);
				rc.Width -= 2;
				rc.Height -= 2;
				e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
				using(Pen p = new Pen(ColorTable.ToolStripBorder))
				using(var b = new SolidBrush(ColorTable.ToolStripBackground))
				{
					e.Graphics.FillRoundedRectangle(b, p, rc, 3);
				}
			}
		}

		private static void RenderDropDownBorder(ToolStripRenderEventArgs e)
		{
			var strip = e.ToolStrip as ToolStripDropDown;
			var rc = new Rectangle(0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
			using(var pen = new Pen(ColorTable.DropDownBorder))
			{
				e.Graphics.DrawRectangle(pen, rc);
			}
			if(strip.OwnerItem != null && strip.OwnerItem.Owner is MenuStrip)
			{
				using(var brush = new SolidBrush(ColorTable.DropDownBackgroundStart))
				{
					rc = e.ConnectedArea;
					rc.Width -= 1;
					e.Graphics.FillRectangle(brush, rc);
				}
			}
		}

		private static void RenderMenuItemBackgroundInternal(Graphics graphics, int x, int y, int width, int height, bool pressed, bool selected, bool root)
		{
			var rc = new Rectangle(x, y, width - 1, height - 1);
			if(pressed)
			{
				if(root)
				{
					rc.Height += 4;
					using(var pen = new Pen(ColorTable.RootMenuItemPressedBorder))
					using(var brush = new SolidBrush(ColorTable.RootMenuItemPressedBackground))
					{
						graphics.FillRoundedRectangle(brush, pen, rc, 2);
					}
				}
				else
				{
					rc.Offset(2, 1);
					rc.Width -= 2;
					rc.Height -= 1;
					using(var pen = new Pen(ColorTable.MenuItemSelectedBorder))
					using(var brush = new LinearGradientBrush(rc,
						ColorTable.MenuItemSelectedBackgroundStart,
						ColorTable.MenuItemSelectedBackgroundEnd,
						LinearGradientMode.Vertical))
					{
						graphics.FillRoundedRectangle(brush, pen, rc, 2);
					}
				}
			}
			else if(selected)
			{
				if(root)
				{
					rc.Offset(1, 1);
					rc.Width -= 1;
				}
				else
				{
					rc.Offset(2, 1);
					rc.Width -= 2;
				}
				rc.Height -= 1;
				using(var pen = new Pen(ColorTable.MenuItemSelectedBorder))
				using(var brush = new LinearGradientBrush(rc,
					ColorTable.MenuItemSelectedBackgroundStart,
					ColorTable.MenuItemSelectedBackgroundEnd,
					LinearGradientMode.Vertical))
				{
					graphics.FillRoundedRectangle(brush, pen, rc, 2);
				}
			}
		}

		private static void RenderItemBackgroundInternal(Graphics graphics, int x, int y, int width, int height, bool pressed, bool selected)
		{
			if(pressed)
			{
				var rc = new Rectangle(x, y, width - 1, height - 1);
				using(var pen = new Pen(ColorTable.ItemPressedBorder))
				{
					graphics.DrawRectangle(pen, rc);
				}
				rc.Offset(1, 1);
				rc.Width -= 1;
				rc.Height -= 1;
				using(var brush = new SolidBrush(ColorTable.ItemPressedBackground))
				{
					graphics.FillRectangle(brush, rc);
				}
			}
			else if(selected)
			{
				var rc = new Rectangle(x, y, width - 1, height - 1);
				using(var pen = new Pen(ColorTable.ItemSelectedBorder))
				{
					graphics.DrawRectangle(pen, rc);
				}
				rc.Offset(1, 1);
				rc.Width -= 1;
				rc.Height -= 1;
				using(var brush = new LinearGradientBrush(rc,
					ColorTable.ItemSelectedBackgroundStart,
					ColorTable.ItemSelectedBackgroundEnd,
					LinearGradientMode.Vertical))
				{
					graphics.FillRectangle(brush, rc);
				}
			}
		}

		private static void RenderItemBackgroundInternal(Graphics graphics, int width, int height, bool pressed, bool selected)
		{
			RenderItemBackgroundInternal(graphics, 0, 0, width, height, pressed, selected);
		}
	}
}
