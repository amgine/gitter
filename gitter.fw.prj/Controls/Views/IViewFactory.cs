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

namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;

	/// <summary>Factory for <see cref="ViewBase"/> objects.</summary>
	public interface IViewFactory
	{
		/// <summary>View GUID.</summary>
		Guid Guid { get; }

		/// <summary>View name.</summary>
		string Name { get; }

		/// <summary>View icon.</summary>
		Image Image { get; }

		/// <summary>Only one instance of view should be maintained.</summary>
		bool IsSingleton { get; }

		/// <summary>List of views created by this factory.</summary>
		IEnumerable<ViewBase> CreatedViews { get; }

		/// <summary>Closes all views, created by this factory.</summary>
		void CloseAllViews();

		/// <summary>Create new view with default parameters.</summary>
		/// <param name="environment">Application working environment.</param>
		/// <returns>Created view.</returns>
		ViewBase CreateView(IWorkingEnvironment environment);

		/// <summary>Default view position.</summary>
		ViewPosition DefaultViewPosition { get; }
	}
}
