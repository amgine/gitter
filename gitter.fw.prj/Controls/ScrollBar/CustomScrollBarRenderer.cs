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

namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public abstract class CustomScrollBarRenderer
	{
		private static CustomScrollBarRenderer _msvs2012Dark;
		private static CustomScrollBarRenderer _msvs2012Light;

		public static CustomScrollBarRenderer MSVS2012Dark
			=> _msvs2012Dark ??= new MSVS2012ScrollBarRenderer(MSVS2012ScrollBarRenderer.DarkColor);

		public static CustomScrollBarRenderer MSVS2012Light
			=> _msvs2012Light ??= new MSVS2012ScrollBarRenderer(MSVS2012ScrollBarRenderer.LightColor);

		public static CustomScrollBarRenderer Default => MSVS2012Dark;

		public virtual void Render(
			Orientation scrollBarOrientation, bool isEnabled, Graphics graphics, Rectangle clipRectangle,
			Rectangle decreaseButtonBounds, Rectangle decreaseTrackBarBounds, Rectangle thumbBounds, Rectangle increaseTrackBarBounds, Rectangle increaseButtonBounds,
			CustomScrollBarPart hoveredPart, CustomScrollBarPart pressedPart)
		{
			if(pressedPart != CustomScrollBarPart.None)
			{
				hoveredPart = pressedPart;
			}
			if(decreaseButtonBounds.IntersectsWith(clipRectangle))
			{
				RenderPart(
					CustomScrollBarPart.DecreaseButton,
					scrollBarOrientation,
					graphics,
					decreaseButtonBounds,
					isEnabled,
					hoveredPart == CustomScrollBarPart.DecreaseButton,
					pressedPart == CustomScrollBarPart.DecreaseButton);
			}
			if(decreaseTrackBarBounds.IntersectsWith(clipRectangle))
			{
				RenderPart(
					CustomScrollBarPart.DecreaseTrackBar,
					scrollBarOrientation,
					graphics,
					decreaseTrackBarBounds,
					isEnabled,
					hoveredPart == CustomScrollBarPart.DecreaseTrackBar,
					pressedPart == CustomScrollBarPart.DecreaseTrackBar);
			}
			if(thumbBounds.IntersectsWith(clipRectangle))
			{
				RenderPart(
					CustomScrollBarPart.Thumb,
					scrollBarOrientation,
					graphics,
					thumbBounds,
					isEnabled,
					hoveredPart == CustomScrollBarPart.Thumb,
					pressedPart == CustomScrollBarPart.Thumb);
			}
			if(increaseTrackBarBounds.IntersectsWith(clipRectangle))
			{
				RenderPart(
					CustomScrollBarPart.IncreaseTrackBar,
					scrollBarOrientation,
					graphics,
					increaseTrackBarBounds,
					isEnabled,
					hoveredPart == CustomScrollBarPart.IncreaseTrackBar,
					pressedPart == CustomScrollBarPart.IncreaseTrackBar);
			}
			if(increaseButtonBounds.IntersectsWith(clipRectangle))
			{
				RenderPart(
					CustomScrollBarPart.IncreaseButton,
					scrollBarOrientation,
					graphics,
					increaseButtonBounds,
					isEnabled,
					hoveredPart == CustomScrollBarPart.IncreaseButton,
					pressedPart == CustomScrollBarPart.IncreaseButton);
			}
		}

		protected abstract void RenderPart(CustomScrollBarPart part, Orientation scrollBarOrientation, Graphics graphics, Rectangle bounds, bool isEnabled, bool isHovered, bool isPressed);
	}
}
