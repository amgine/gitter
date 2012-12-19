namespace gitter.Framework.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	[ToolboxItem(false)]
	public sealed class PopupNotificationHeader : Control
	{
		#region Data

		private readonly ViewButtons _buttons;
		private bool _buttonsHovered;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="PopupNotificationHeader"/>.</summary>
		internal PopupNotificationHeader()
		{
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
			_buttons.SetAvailableButtons(ViewButtonType.Close);
			_buttons.ButtonClick += OnButtonClick;
		}

		#endregion

		private void OnButtonClick(object sender, ViewButtonClickEventArgs e)
		{
			switch(e.Button)
			{
				case ViewButtonType.Close:
					FindForm().Close();
					break;
			}
		}

		private Rectangle GetButtonsRect()
		{
			return new Rectangle(
				Width - Buttons.Width - 2,
				0,
				ViewConstants.ViewButtonSize,
				ViewConstants.ViewButtonSize);
		}

		public ViewButtons Buttons
		{
			get { return _buttons; }
		}

		protected override void OnResize(EventArgs e)
		{
			_buttons.Height = Height;
			base.OnResize(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if(Buttons.Count != 0)
			{
				var rc = GetButtonsRect();
				if(rc.Contains(e.X, e.Y))
				{
					var x = e.X - rc.X;
					var y = e.Y - rc.Y;
					Buttons.OnMouseMove(x, y, e.Button);
					_buttonsHovered = true;
				}
				else
				{
					if(_buttonsHovered)
					{
						Buttons.OnMouseLeave();
						_buttonsHovered = false;
					}
					if(Buttons.PressedButton == null)
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
				Buttons.OnMouseLeave();
				_buttonsHovered = false;
			}
			base.OnMouseLeave(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if(Buttons.Count != 0)
			{
				var rc = GetButtonsRect();
				if(rc.Contains(e.X, e.Y))
				{
					var x = e.X - rc.X;
					var y = e.Y - rc.Y;
					Buttons.OnMouseDown(x, y, e.Button);
				}
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if(Buttons.Count != 0)
			{
				var rc = GetButtonsRect();
				if(Buttons.PressedButton != null || rc.Contains(e.X, e.Y))
				{
					var x = e.X - rc.X;
					var y = e.Y - rc.Y;
					Buttons.OnMouseUp(x, y, e.Button);
				}
			}
			base.OnMouseUp(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			ViewManager.Renderer.RenderPopupNotificationHeader(this, e);
			_buttons.OnPaint(e.Graphics, GetButtonsRect(), true);
		}
	}
}
