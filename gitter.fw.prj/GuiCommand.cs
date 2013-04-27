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

namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	public sealed class GuiCommand : INamedObject
	{
		#region Data

		private readonly string _name;
		private readonly string _displayName;
		private readonly Image _image;
		private readonly Action<IWorkingEnvironment> _execute;

		#endregion

		#region .ctor

		public GuiCommand(string name, string displayName, Image image, Action<IWorkingEnvironment> execute)
		{
			_name = name;
			_displayName = displayName;
			_execute = execute;
			_image = image;
		}

		#endregion

		#region Propertes

		public string Name
		{
			get { return _name; }
		}

		public string DisplayName
		{
			get { return _displayName; }
		}

		public Image Image
		{
			get { return _image; }
		}

		#endregion

		#region Methods

		public void Execute(IWorkingEnvironment env)
		{
			_execute(env);
		}

		#endregion
	}
}
