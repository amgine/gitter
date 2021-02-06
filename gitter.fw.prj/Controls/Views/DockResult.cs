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
	/// <summary>Represents dock position.</summary>
	public enum DockResult
	{
		/// <summary>None.</summary>
		None = 0,

		/// <summary>Inside target dock host.</summary>
		Fill,

		/// <summary>On floating form.</summary>
		Float,

		/// <summary>To the left of dock host.</summary>
		Left,
		/// <summary>To the top of dock host.</summary>
		Top,
		/// <summary>To the bottom of dock host.</summary>
		Bottom,
		/// <summary>To the right of dock host.</summary>
		Right,

		/// <summary>Inside new document host to the left of dock host.</summary>
		DocumentLeft,
		/// <summary>Inside new document host to the top of dock host.</summary>
		DocumentTop,
		/// <summary>Inside new document host to the bottom of dock host.</summary>
		DocumentBottom,
		/// <summary>Inside new document host to the right of dock host.</summary>
		DocumentRight,

		/// <summary>Inside auto-hiding container at the left side of dock host.</summary>
		AutoHideLeft,
		/// <summary>Inside auto-hiding container at the top side of dock host.</summary>
		AutoHideTop,
		/// <summary>Inside auto-hiding container at the bottom side of dock host.</summary>
		AutoHideBottom,
		/// <summary>Inside auto-hiding container at the right side of dock host.</summary>
		AutoHideRight,
	}
}
