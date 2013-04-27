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
	using System.Windows.Forms;
	using System.Xml;

	using gitter.Framework.Configuration;

	/// <summary>Interface for gui provider/builder.</summary>
	public interface IGuiProvider
	{
		/// <summary>Build gui inside supplied <paramref name="IWorkingEnvironment"/>.</summary>
		/// <param name="environment">Environment for gui elements hosting.</param>
		void AttachToEnvironment(IWorkingEnvironment environment);

		/// <summary>Remove gui, created by this provider from <paramref name="IWorkingEnvironment"/>.</summary>
		/// <param name="environment">Environment for gui elements hosting.</param>
		void DetachFromEnvironment(IWorkingEnvironment environment);

		void SaveTo(Section section);

		void LoadFrom(Section section);
	}
}
