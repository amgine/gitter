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
		{
			get
			{
				if(_msvs2012Dark == null)
				{
					_msvs2012Dark = new MSVS2012ScrollBarRenderer(MSVS2012ScrollBarRenderer.DarkColor);
				}
				return _msvs2012Dark;
			}
		}

		public static CustomScrollBarRenderer MSVS2012Light
		{
			get
			{
				if(_msvs2012Light == null)
				{
					_msvs2012Light = new MSVS2012ScrollBarRenderer(MSVS2012ScrollBarRenderer.LightColor);
				}
				return _msvs2012Light;
			}
		}

		public static CustomScrollBarRenderer Default
		{
			get { return MSVS2012Dark; }
		}

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
