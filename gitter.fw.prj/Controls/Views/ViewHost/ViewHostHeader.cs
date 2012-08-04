namespace gitter.Framework.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	[ToolboxItem(false)]
	public sealed class ViewHostHeader : Control
	{
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

		/// <summary>Create <see cref="ViewHostHeader"/>.</summary>
		internal ViewHostHeader(ViewHost viewHost)
		{
			if(viewHost == null) throw new ArgumentNullException("viewHost");
			_viewHost = viewHost;

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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text
		{
			get { return ViewHost.Text; }
			set
			{
				throw new InvalidOperationException();
			}
		}

		public void SetAvailableButtons(params ViewButtonType[] buttons)
		{
			_buttons.SetAvailableButtons(buttons);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			ViewManager.Renderer.RenderViewHostHeader(this, e);
			if(Buttons.Count != 0)
			{
				Buttons.OnPaint(e.Graphics, GetButtonsRect(), ViewHost.IsActive);
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
			return new Rectangle(rc.Width - w - 2, 0, w, rc.Height);
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
			ViewHost.Activate();
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
	}
}
