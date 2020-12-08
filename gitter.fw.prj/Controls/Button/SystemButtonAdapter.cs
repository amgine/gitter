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
	using System.Windows.Forms;

	public sealed class SystemButtonAdapter : IButtonWidget
	{
		#region Data

		private readonly Button _button;

		#endregion

		#region Events

		public event EventHandler Click;

		private void OnClick(EventArgs e)
			=> Click?.Invoke(this, e);

		#endregion

		#region .ctor

		public SystemButtonAdapter()
		{
			_button = new Button()
			{
				FlatStyle = FlatStyle.System,
			};
			_button.Click += OnButtonClick;
		}

		#endregion

		#region Event Handlers

		private void OnButtonClick(object sender, EventArgs e)
			=> OnClick(e);

		#endregion

		#region Properties

		public Control Control => _button;

		public string Text
		{
			get => _button.Text;
			set => _button.Text = value;
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			_button.Click -= OnButtonClick;
			_button.Dispose();
		}

		#endregion
	}
}
