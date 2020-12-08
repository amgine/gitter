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

namespace gitter.Framework
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Configuration;

	/// <summary>Provides additional services for repository.</summary>
	public interface IRepositoryServiceProvider : INamedObject
	{
		/// <summary>Returns service display name.</summary>
		/// <value>Service display name.</value>
		string DisplayName { get; }

		/// <summary>Determines if provider can be manually added for repository.</summary>
		/// <value><c>true</c>, if can be added, <c>false</c> otherwise.</value>
		bool CanBeAddedManually { get; }

		/// <summary>Returns service icon.</summary>
		/// <value>Service icon.</value>
		Image Icon { get; }

		/// <summary>Prepare for working inside specified <paramref name="environment"/>.</summary>
		/// <param name="environment"><see cref="IWorkingEnvironment"/> to work in.</param>
		/// <param name="section">Provider configuration section.</param>
		bool LoadFor(IWorkingEnvironment environment, Section section);

		/// <summary>Save configuration to <paramref name="section"/>.</summary>
		/// <param name="section"><see cref="Section"/> for storing configuration.</param>
		void SaveTo(Section section);

		/// <summary>Creates service setup control for the specified repository.</summary>
		/// <param name="repository">Repository to configure service for.</param>
		/// <returns>Setup control for the service.</returns>
		Control CreateSetupDialog(IRepository repository);

		/// <summary>Creates GUI provider to modify application UI.</summary>
		/// <param name="repository">Specifies repository.</param>
		/// <param name="guiProvider">GUI provider for the service.</param>
		/// <returns><c>true</c> on success, <c>false</c> if incompatible with <paramref name="repository"/>.</returns>
		bool TryCreateGuiProvider(IRepository repository, out IGuiProvider guiProvider);
	}
}
