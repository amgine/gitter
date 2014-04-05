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
	using System.Windows.Forms;

	public class RadioButtonInputSource : IUserInputSource<bool>, IWin32ControlInputSource
	{
		#region Data

		private readonly RadioButton _radioButton;

		#endregion

		#region .ctor

		public RadioButtonInputSource(RadioButton radionButton)
		{
			Verify.Argument.IsNotNull(radionButton, "radionButton");

			_radioButton = radionButton;
		}

		#endregion

		#region IUserInputSource<bool> Members

		public bool Value
		{
			get { return _radioButton.Checked; }
			set { _radioButton.Checked = value; }
		}

		public bool IsReadOnly
		{
			get { return !_radioButton.Enabled; }
			set { _radioButton.Enabled = !value; }
		}

		#endregion

		#region IWin32ControlInputSource Members

		public Control Control
		{
			get { return _radioButton; }
		}

		#endregion
	}
}
