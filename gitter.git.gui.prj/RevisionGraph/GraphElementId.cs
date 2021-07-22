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

namespace gitter.Git.Gui
{
	public static class GraphElementId
	{
		/// <summary>
		/// <para>...</para>
		/// <para>.+.</para>
		/// <para>...</para>
		/// </summary>
		public const int Dot = 0;

		/// <summary>
		/// <para>...</para>
		/// <para>-..</para>
		/// <para>...</para>
		/// </summary>
		public const int HorizontalLeft = 1;
		/// <summary>
		/// <para>...</para>
		/// <para>..-</para>
		/// <para>...</para>
		/// </summary>
		public const int HorizontalRight = 2;
		/// <summary>
		/// <para>.|.</para>
		/// <para>...</para>
		/// <para>...</para>
		/// </summary>
		public const int VerticalTop = 3;
		/// <summary>
		/// <para>...</para>
		/// <para>...</para>
		/// <para>.|.</para>
		/// </summary>
		public const int VerticalBottom = 4;
		/// <summary>
		/// <para>\..</para>
		/// <para>...</para>
		/// <para>...</para>
		/// </summary>
		public const int LeftTop = 5;
		/// <summary>
		/// <para>...</para>
		/// <para>...</para>
		/// <para>..\</para>
		/// </summary>
		public const int RightBottom = 6;
		/// <summary>
		/// <para>../</para>
		/// <para>...</para>
		/// <para>...</para>
		/// </summary>
		public const int RightTop = 7;
		/// <summary>
		/// <para>...</para>
		/// <para>...</para>
		/// <para>/..</para>
		/// </summary>
		public const int LeftBottom = 8;
		/// <summary>
		/// <para>/..</para>
		/// <para>...</para>
		/// <para>...</para>
		/// </summary>
		public const int LeftTopCorner = 9;
		/// <summary>
		/// <para>..\</para>
		/// <para>...</para>
		/// <para>...</para>
		/// </summary>
		public const int RightTopCorner = 10;
		/// <summary>
		/// <para>...</para>
		/// <para>...</para>
		/// <para>\..</para>
		/// </summary>
		public const int LeftBottomCorner = 11;
		/// <summary>
		/// <para>...</para>
		/// <para>...</para>
		/// <para>../</para>
		/// </summary>
		public const int RightBottomCorner = 12;
	}
}
