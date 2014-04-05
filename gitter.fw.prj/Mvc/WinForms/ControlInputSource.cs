#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.Mvc.WinForms
{
	using System;
	using System.Windows.Forms;

	public class ControlInputSource : IUserInputSource<string>, IWin32ControlInputSource
	{
		#region Data

		private readonly Control _control;

		#endregion

		#region .ctor

		public ControlInputSource(Control control)
		{
			Verify.Argument.IsNotNull(control, "control");

			_control = control;
		}

		#endregion

		#region IUserInputSource<string> Members

		public string Value
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		#endregion

		#region IUserInputSource Members

		public bool IsReadOnly
		{
			get { return !Control.Enabled; }
			set { Control.Enabled = !value; }
		}

		#endregion

		#region IWin32ControlInputSource Members

		public Control Control
		{
			get { return _control; }
		}

		#endregion
	}

	public abstract class ControlInputSource<TControl, TValue> : IUserInputSource<TValue>, IWin32ControlInputSource
		where TControl : Control
	{
		#region Data

		private readonly TControl _control;
		private TValue _value;
		private bool _isValueValid;

		#endregion

		#region .ctor

		public ControlInputSource(TControl control)
		{
			Verify.Argument.IsNotNull(control, "control");

			_control = control;
			if(!control.IsDisposed)
			{
				SubscribeToValueChangeEvent();
			}
		}

		#endregion

		#region Properties

		public TControl Control
		{
			get { return _control; }
		}

		#endregion

		#region Methods

		protected abstract TValue FetchValue();

		protected abstract void SetValue(TValue value);

		protected abstract void SubscribeToValueChangeEvent();

		protected abstract void UnsubscribeToValueChangeEvent();

		protected virtual void OnControlValueChanged(object sender, EventArgs e)
		{
			InvalidateValue();
		}

		protected virtual void InvalidateValue()
		{
			_isValueValid = false;
		}

		#endregion

		#region IUserInputSource<string> Members

		public TValue Value
		{
			get
			{
				if(!_isValueValid)
				{
					if(Control.IsDisposed)
					{
						_isValueValid = true;
					}
					else
					{
						_value = FetchValue();
					}
				}
				return _value;
			}
			set
			{
				_value = value;
				if(!Control.IsDisposed)
				{
					SetValue(value);
				}
				_isValueValid = true;
			}
		}

		#endregion

		#region IUserInputSource Members

		public virtual bool IsReadOnly
		{
			get { return !Control.Enabled; }
			set { Control.Enabled = !value; }
		}

		#endregion

		#region IWin32ControlInputSource Members

		Control IWin32ControlInputSource.Control
		{
			get { return Control; }
		}

		#endregion
	}
}
