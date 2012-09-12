namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public sealed class ViewHostTab : ViewTabBase
	{
		#region Data

		private readonly ViewHostTabs _tabs;
		private readonly ViewHost _viewHost;

		private readonly ViewButtons _buttons;
		private bool _buttonsHovered;

		#endregion

		public ViewHostTab(ViewHostTabs tabs, ViewBase view)
			: base(view, tabs.Side)
		{
			Verify.Argument.IsNotNull(tabs, "tabs");

			_tabs = tabs;
			_viewHost = tabs.ViewHost;
			_buttons = new ViewButtons(tabs);
			if(_viewHost.IsDocumentWell)
			{
				_buttons.SetAvailableButtons(ViewButtonType.Close);
			}
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
			return ViewManager.Renderer.MeasureViewHostTabLength(this, graphics);
		}

		internal override void OnPaint(Graphics graphics, Rectangle bounds)
		{
			if(bounds.Width > 0 && bounds.Height > 0)
			{
				ViewManager.Renderer.RenderViewHostTabBackground(this, graphics, bounds);
				ViewManager.Renderer.RenderViewHostTabContent(this, graphics, bounds);
			}
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
