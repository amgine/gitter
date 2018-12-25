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

	public static class CustomListBoxManager
	{
		#region Data

		private static CustomListBoxRenderer _renderer;
		private static CustomListBoxRenderer _win7Renderer;
		private static CustomListBoxRenderer _msvs2012LightRenderer;
		private static CustomListBoxRenderer _msvs2012DarkRenderer;

		#endregion

		#region .ctor

		static CustomListBoxManager()
		{
		}

		#endregion

		#region Properties

		public static CustomListBoxRenderer Renderer
		{
			get
			{
				if(_renderer == null)
				{
					return Win7Renderer;
				}
				return _renderer;
			}
			set
			{
				Verify.Argument.IsNotNull(value, nameof(value));

				_renderer = value;
			}
		}

		public static CustomListBoxRenderer Win7Renderer
		{
			get
			{
				if(_win7Renderer == null)
				{
					_win7Renderer = new Win7CustomListBoxRenderer();
				}
				return _win7Renderer;
			}
		}

		public static CustomListBoxRenderer MSVS2012LightRenderer
		{
			get
			{
				if(_msvs2012LightRenderer == null)
				{
					_msvs2012LightRenderer = new MSVS2012CustomListBoxRenderer(MSVS2012CustomListBoxRenderer.LightColors);
				}
				return _msvs2012LightRenderer;
			}
		}

		public static CustomListBoxRenderer MSVS2012DarkRenderer
		{
			get
			{
				if(_msvs2012DarkRenderer == null)
				{
					_msvs2012DarkRenderer = new MSVS2012CustomListBoxRenderer(MSVS2012CustomListBoxRenderer.DarkColors);
				}
				return _msvs2012DarkRenderer;
			}
		}

		#endregion
	}
}
