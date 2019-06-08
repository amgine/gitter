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

	public class NumericUpDownInputSource<T> : ControlInputSource<NumericUpDown, T>
	{
		private readonly Converter<decimal, T> _convert;
		private readonly Converter<T, decimal> _convertBack;

		public NumericUpDownInputSource(NumericUpDown numericUpDown, Converter<decimal, T> convert, Converter<T, decimal> convertBack)
			: base(numericUpDown)
		{
			Verify.Argument.IsNotNull(convert, nameof(convert));
			Verify.Argument.IsNotNull(convertBack, nameof(convertBack));

			_convert = convert;
			_convertBack = convertBack;
		}

		public override bool IsReadOnly
		{
			get { return Control.ReadOnly; }
			set { Control.ReadOnly = value; }
		}

		protected override T FetchValue() => _convert(Control.Value);

		protected override void SetValue(T value) => Control.Value = _convertBack(value);

		protected override void SubscribeToValueChangeEvent()
		{
			Control.ValueChanged +=	OnControlValueChanged;
		}

		protected override void UnsubscribeToValueChangeEvent()
		{
			Control.ValueChanged -= OnControlValueChanged;
		}
	}
}
