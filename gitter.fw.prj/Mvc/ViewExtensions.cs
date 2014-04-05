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

namespace gitter.Framework.Mvc
{
	using System;

	public static class ViewExtensions
	{
		public struct CursorChangeToken : IDisposable
		{
			private readonly IView _view;
			private readonly MouseCursor _cursor;

			internal CursorChangeToken(IView view, MouseCursor cursor)
			{
				_view = view;
				_cursor = cursor;
			}

			#region IDisposable Members

			public void Dispose()
			{
				_view.MouseCursor = _cursor;
			}

			#endregion
		}

		public static CursorChangeToken ChangeCursor(this IView view, MouseCursor cursor)
		{
			Verify.Argument.IsNotNull(view, "view");

			var oldCursor = view.MouseCursor;
			view.MouseCursor = cursor;
			return new CursorChangeToken(view, oldCursor);
		}
	}
}
