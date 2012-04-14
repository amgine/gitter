namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	sealed class ViewHostTab : ViewTabBase
	{
		#region Data

		private readonly ViewHostTabs _tabs;
		private readonly ViewHost _toolHost;

		private readonly ViewButtons _buttons;
		private bool _buttonsHovered;

		#endregion

		public ViewHostTab(ViewHostTabs tabs, ViewBase tool)
			: base(tabs, tool, tabs.Side)
		{
			_tabs = tabs;
			_toolHost = tabs.ViewHost;
			_buttons = new ViewButtons(tabs);
			if(_toolHost.IsDocumentWell)
				_buttons.SetAvailableButtons(ViewButtonType.Close);
			_buttons.Height = ViewConstants.TabHeight + ViewConstants.TabFooterHeight;
			_buttons.ButtonClick += OnButtonClick;
		}

		private void OnButtonClick(object sender, ViewButtonClickEventArgs e)
		{
			switch(e.Button)
			{
				case ViewButtonType.Close:
					View.Close();
					break;
			}
		}

		public void EnsureVisible()
		{
			_tabs.EnsureVisible(this);
		}

		public ViewHostTabs Tabs
		{
			get { return _tabs; }
		}

		public ViewButtons Buttons
		{
			get { return _buttons; }
		}

		protected override int Measure(Graphics graphics)
		{
			var length = base.Measure(graphics);
			length += _buttons.Width;
			return length;
		}

		protected override void OnPaintContent(Graphics graphics, Rectangle bounds)
		{
			base.OnPaintContent(graphics, bounds);
			var buttonsBounds = new Rectangle(bounds.Right - _buttons.Width - 2, 0, _buttons.Width, bounds.Height);
			_buttons.OnPaint(graphics, buttonsBounds, IsActive);
		}

		public override void OnMouseLeave()
		{
			base.OnMouseLeave();
			_buttons.OnMouseLeave();
			_tabs.Invalidate();
		}

		public override void OnMouseEnter()
		{
			base.OnMouseEnter();
			_tabs.Invalidate();
		}

		public override void OnMouseDown(int x, int y, MouseButtons button)
		{
			base.OnMouseDown(x, y, button);
			switch(button)
			{
				case MouseButtons.Middle:
					View.Close();
					return;
				case MouseButtons.Right:
					return;
			}
			var buttonsBounds = new Rectangle(Length - _buttons.Width - 2, 0, _buttons.Width, ViewConstants.TabHeight);
			if(buttonsBounds.Contains(x, y))
			{
				x -= buttonsBounds.X;
				y -= buttonsBounds.Y;
				_buttons.OnMouseDown(x, y, button);
			}
			if(_buttons.PressedButton == null)
			{
				View.Activate();
			}
		}

		public override void OnMouseMove(int x, int y, MouseButtons button)
		{
			base.OnMouseMove(x, y, button);
			var buttonsBounds = new Rectangle(Length - _buttons.Width - 2, 0, _buttons.Width, ViewConstants.TabHeight);
			if(buttonsBounds.Contains(x, y))
			{
				_buttonsHovered = true;
				x -= buttonsBounds.X;
				y -= buttonsBounds.Y;
				_buttons.OnMouseMove(x, y, button);
			}
			else
			{
				if(_buttonsHovered)
				{
					_buttons.OnMouseLeave();
					_buttonsHovered = false;
				}
			}
		}

		public override void OnMouseUp(int x, int y, MouseButtons button)
		{
			base.OnMouseUp(x, y, button);
			var buttonsBounds = new Rectangle(Length - _buttons.Width - 2, 0, _buttons.Width, ViewConstants.TabHeight);
			if(_buttons.PressedButton != null || buttonsBounds.Contains(x, y))
			{
				x -= buttonsBounds.X;
				y -= buttonsBounds.Y;
				_buttons.OnMouseUp(x, y, button);
			}
		}
	}
}
