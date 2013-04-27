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
	using System.Drawing;
	using System.Windows.Forms;

	public sealed class SystemCheckBoxAdapter : ICheckBoxWidget
	{
		#region Data

		private readonly CheckBox _checkBox;

		#endregion

		#region Events

		public event EventHandler IsCheckedChanged;

		public event EventHandler CheckStateChanged;

		private void OnIsCheckedChanged()
		{
			var handler = IsCheckedChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		private void OnCheckStateChanged()
		{
			var handler = CheckStateChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		#region .ctor

		public SystemCheckBoxAdapter()
		{
			_checkBox = new CheckBox()
			{
				FlatStyle = FlatStyle.System,
			};
			_checkBox.CheckedChanged += OnCheckBoxCheckedChanged;
			_checkBox.CheckStateChanged += OnCheckBoxCheckStateChanged;
		}

		#endregion

		#region Event Handlers

		private void OnCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			OnIsCheckedChanged();
		}

		private void OnCheckBoxCheckStateChanged(object sender, EventArgs e)
		{
			OnCheckStateChanged();
		}

		#endregion

		#region Properties

		public Control Control
		{
			get { return _checkBox; }
		}

		public Image Image
		{
			get { return _checkBox.Image; }
			set
			{
				if(value != _checkBox.Image)
				{
					_checkBox.Image = value;
					if(value == null)
					{
						_checkBox.FlatStyle = FlatStyle.System;
					}
					else
					{
						_checkBox.FlatStyle = FlatStyle.Standard;
						_checkBox.TextImageRelation = TextImageRelation.ImageBeforeText;
						_checkBox.ImageAlign = ContentAlignment.MiddleLeft;
					}
				}
			}
		}

		public string Text
		{
			get { return _checkBox.Text; }
			set { _checkBox.Text = value; }
		}

		public bool IsChecked
		{
			get { return _checkBox.Checked; }
			set { _checkBox.Checked = value; }
		}

		public CheckState CheckState
		{
			get { return _checkBox.CheckState; }
			set { _checkBox.CheckState = value; }
		}

		public bool ThreeState
		{
			get { return _checkBox.ThreeState; }
			set { _checkBox.ThreeState = value; }
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			_checkBox.CheckedChanged -= OnCheckBoxCheckedChanged;
			_checkBox.CheckStateChanged -= OnCheckBoxCheckStateChanged;
			_checkBox.Dispose();
		}

		#endregion
	}
}
