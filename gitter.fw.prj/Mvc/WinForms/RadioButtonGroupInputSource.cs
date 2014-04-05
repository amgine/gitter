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

	public class RadioButtonGroupInputSource<T> : IUserInputSource<T>
	{
		#region Data

		private readonly Tuple<RadioButton, T>[] _valueMappings;
		private bool _isReadonly;

		#endregion

		#region .ctor

		public RadioButtonGroupInputSource(Tuple<RadioButton, T>[] valueMappings)
		{
			Verify.Argument.IsNotNull(valueMappings, "valueMappings");

			_valueMappings = valueMappings;
		}

		#endregion

		#region IUserInputSource<T> Members

		public T Value
		{
			get
			{
				for(int i = 0; i < _valueMappings.Length; ++i)
				{
					if(_valueMappings[i].Item1.Checked)
					{
						return _valueMappings[i].Item2;
					}
				}
				return default(T);
			}
			set
			{
				var val = (object)value;
				for(int i = 0; i < _valueMappings.Length; ++i)
				{
					if(object.Equals(val, _valueMappings[i].Item2))
					{
						_valueMappings[i].Item1.Checked = true;
						return;
					}
				}
				for(int i = 0; i < _valueMappings.Length; ++i)
				{
					_valueMappings[i].Item1.Checked = false;
				}
			}
		}

		public bool IsReadOnly
		{
			get { return _isReadonly; }
			set
			{
				if(_isReadonly != value)
				{
					_isReadonly = value;
					for(int i = 0; i < _valueMappings.Length; ++i)
					{
						_valueMappings[i].Item1.Enabled = value;
					}
				}
			}
		}

		#endregion
	}
}
