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
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	[DesignerCategory("")]
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

		private static readonly object IsCheckedChangedEvent  = new();
		private static readonly object CheckStateChangedEvent = new();

		public event EventHandler IsCheckedChanged
		{
			add    => Events.AddHandler    (IsCheckedChangedEvent, value);
			remove => Events.RemoveHandler (IsCheckedChangedEvent, value);
		}

		public event EventHandler CheckStateChanged
		{
			add    => Events.AddHandler    (CheckStateChangedEvent, value);
			remove => Events.RemoveHandler (CheckStateChangedEvent, value);
		}

		protected virtual void OnIsCheckedChanged()
			=> ((EventHandler)Events[IsCheckedChangedEvent])?.Invoke(this, EventArgs.Empty);

		protected virtual void OnCheckStateChanged()
			=> ((EventHandler)Events[CheckStateChangedEvent])?.Invoke(this, EventArgs.Empty);

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
			get => _renderer;
			set
			{
				Verify.Argument.IsNotNull(value, nameof(value));

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
			get => _checkState;
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
			get => _image;
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
			get => CheckState == CheckState.Checked;
			set => CheckState = value ? CheckState.Checked : CheckState.Unchecked;
		}

		[DefaultValue(false)]
		public bool ThreeState
		{
			get => _threeState;
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

		public bool IsMouseOver => _isMouseOver;

		public bool IsPressed => _isPressed;

		#endregion

		#region Methods

		private void CycleState()
		{
			if(ThreeState)
			{
				CheckState = CheckState switch
				{
					CheckState.Checked       => CheckState.Indeterminate,
					CheckState.Indeterminate => CheckState.Unchecked,
					CheckState.Unchecked     => CheckState.Checked,
					_ => CheckState.Checked,
				};
			}
			else
			{
				IsChecked = !IsChecked;
			}
		}

		#endregion

		#region Overrides

		/// <inheritdoc/>
		protected override void OnMouseClick(MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
			{
				CycleState();
			}
			base.OnMouseClick(e);
		}

		/// <inheritdoc/>
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

		/// <inheritdoc/>
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

		/// <inheritdoc/>
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

		/// <inheritdoc/>
		protected override void OnPaint(PaintEventArgs e)
		{
			Assert.IsNotNull(e);

			Renderer.Render(e.Graphics, new Dpi(DeviceDpi), e.ClipRectangle, this);
		}

		/// <inheritdoc/>
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
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
			_isPressed = false;
			Invalidate();
		}

		/// <inheritdoc/>
		protected override void OnMouseEnter(EventArgs e)
		{
			_isMouseOver = true;
			base.OnMouseEnter(e);
			Invalidate();
		}

		/// <inheritdoc/>
		protected override void OnMouseLeave(EventArgs e)
		{
			_isMouseOver = false;
			base.OnMouseLeave(e);
			Invalidate();
		}

		/// <inheritdoc/>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			_isPressed = true;
			base.OnMouseDown(e);
			Focus();
			Invalidate();
		}

		/// <inheritdoc/>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			_isPressed = false;
			base.OnMouseUp(e);
			Invalidate();
		}

		#endregion
	}
}
