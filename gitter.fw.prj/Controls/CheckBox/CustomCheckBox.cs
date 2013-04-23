namespace gitter.Framework.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	public class CustomCheckBox : Control
	{
		#region Data

		private CustomCheckBoxRenderer _renderer;
		private CheckState _checkState;
		private Image _image;
		private bool _threeState;
		private bool _isMouseOver;
		private bool _isPressed;

		#endregion

		#region Events

		private static readonly object IsCheckedChangedEvent = new object();
		private static readonly object CheckStateChangedEvent = new object();

		public event EventHandler IsCheckedChanged
		{
			add { Events.AddHandler(IsCheckedChangedEvent, value); }
			remove { Events.RemoveHandler(IsCheckedChangedEvent, value); }
		}

		public event EventHandler CheckStateChanged
		{
			add { Events.AddHandler(CheckStateChangedEvent, value); }
			remove { Events.RemoveHandler(CheckStateChangedEvent, value); }
		}

		protected virtual void OnIsCheckedChanged()
		{
			var handler = (EventHandler)Events[IsCheckedChangedEvent];
			if(handler != null) handler(this, EventArgs.Empty);
		}

		protected virtual void OnCheckStateChanged()
		{
			var handler = (EventHandler)Events[CheckStateChangedEvent];
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		#region .ctor

		public CustomCheckBox()
		{
			SetStyle(
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.OptimizedDoubleBuffer |
				ControlStyles.Opaque |
				ControlStyles.Selectable |
				ControlStyles.ResizeRedraw |
				ControlStyles.UserPaint, true);
			SetStyle(
				ControlStyles.ContainerControl |
				ControlStyles.SupportsTransparentBackColor, false);
			_renderer = CustomCheckBoxRenderer.Default;
			Size = new Size(86, 18);
			TabStop = true;
		}

		#endregion

		#region Properties

		public CustomCheckBoxRenderer Renderer
		{
			get { return _renderer; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");

				if(_renderer != value)
				{
					_renderer = value;
					Invalidate();
				}
			}
		}

		[DefaultValue(CheckState.Unchecked)]
		public CheckState CheckState
		{
			get { return _checkState; }
			set
			{
				if(_checkState != value)
				{
					bool isCheckedChanged = (_checkState == CheckState.Checked) != (value == CheckState.Checked);
					_checkState = value;
					OnCheckStateChanged();
					if(isCheckedChanged)
					{
						OnIsCheckedChanged();
					}
					Invalidate();
				}
			}
		}

		[DefaultValue(null)]
		public Image Image
		{
			get { return _image; }
			set
			{
				if(_image != value)
				{
					_image = value;
					Invalidate();
				}
			}
		}

		[DefaultValue(false)]
		public bool IsChecked
		{
			get { return CheckState == CheckState.Checked; }
			set { CheckState = value ? CheckState.Checked : CheckState.Unchecked; }
		}

		[DefaultValue(false)]
		public bool ThreeState
		{
			get { return _threeState; }
			set
			{
				if(_threeState != value)
				{
					_threeState = value;
					if(!value && CheckState == CheckState.Indeterminate)
					{
						CheckState = CheckState.Checked;
					}
				}
			}
		}

		public bool IsMouseOver
		{
			get { return _isMouseOver; }
		}

		public bool IsPressed
		{
			get { return _isPressed; }
		}

		#endregion

		#region Methods

		private void CycleState()
		{
			if(ThreeState)
			{
				switch(CheckState)
				{
					case CheckState.Checked:
						CheckState = CheckState.Indeterminate;
						break;
					case CheckState.Indeterminate:
						CheckState = CheckState.Unchecked;
						break;
					case CheckState.Unchecked:
						CheckState = CheckState.Checked;
						break;
					default:
						CheckState = CheckState.Checked;
						break;
				}
			}
			else
			{
				IsChecked = !IsChecked;
			}
		}

		#endregion

		#region Overrides

		protected override void OnMouseClick(MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
			{
				CycleState();
			}
			base.OnMouseClick(e);
		}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Space:
					e.IsInputKey = true;
					break;
				default:
					base.OnPreviewKeyDown(e);
					break;
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			switch(e.KeyCode)
			{
				case Keys.Space:
					_isPressed = true;
					Invalidate();
					break;
			}
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			switch(e.KeyCode)
			{
				case Keys.Space:
					_isPressed = false;
					CycleState();
					break;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Renderer.Render(e.Graphics, e.ClipRectangle, this);
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			Invalidate();
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			_isPressed = false;
			Invalidate();
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			_isMouseOver = true;
			base.OnMouseEnter(e);
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			_isMouseOver = false;
			base.OnMouseLeave(e);
			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			_isPressed = true;
			base.OnMouseDown(e);
			Focus();
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			_isPressed = false;
			base.OnMouseUp(e);
			Invalidate();
		}

		#endregion
	}
}
