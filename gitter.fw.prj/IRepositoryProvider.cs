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
	using System.Collections.Generic;
	using System.Drawing;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Forms;

	using gitter.Framework.Configuration;

	/// <summary>Repository provider.</summary>
	public interface IRepositoryProvider : INamedObject
	{
		#region Properties

		/// <summary>Returns provider display name.</summary>
		/// <value>Provider display name.</value>
		string DisplayName { get; }

		/// <summary>Returns provider icon.</summary>
		/// <value>Provider icon.</value>
		Image Icon { get; }

		/// <summary>Returns GUI provider.</summary>
		/// <value>GUI provider.</value>
		IRepositoryGuiProvider GuiProvider { get; }

		/// <summary>Gets a value indicating whether this provider is loaded.</summary>
		/// <value><c>true</c> if this provider is loaded; otherwise, <c>false</c>.</value>
		bool IsLoaded { get; }

		#endregion

		#region Methods

		/// <summary>Prepare to work in context of specified <paramref name="environment"/>.</summary>
		/// <param name="environment"><see cref="IWorkingEnvironment"/> to work in.</param>
		/// <param name="section">Provider configuration section.</param>
		bool LoadFor(IWorkingEnvironment environment, Section section);

		/// <summary>Save configuration to <paramref name="section"/>.</summary>
		/// <param name="section"><see cref="Section"/> for storing configuration.</param>
		void SaveTo(Section section);

		/// <summary>Checks if provider can create repository for <paramref name="workingDirectory"/>.</summary>
		/// <param name="workingDirectory">Repository working directory.</param>
		/// <returns>true, if <see cref="OpenRepository()"/> can succeed for <paramref name="workingDirectory"/>.</returns>
		bool IsValidFor(string workingDirectory);

		/// <summary>Opens repository specified by <paramref name="workingDirectory"/>.</summary>
		/// <param name="workingDirectory">Working directory of repository.</param>
		/// <returns>Opened repository.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="workingDirectory"/> == <c>null</c>.</exception>
		IRepository OpenRepository(string workingDirectory);

		/// <summary>Opens repository specified by <paramref name="workingDirectory"/>.</summary>
		/// <param name="workingDirectory">Working directory of repository.</param>
		/// <returns>Opened repository.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="workingDirectory"/> == <c>null</c>.</exception>
		Task<IRepository> OpenRepositoryAsync(string workingDirectory, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Called after repository is successfully loaded by environment.</summary>
		/// <param name="repository">Loaded repository.</param>
		void OnRepositoryLoaded(IRepository repository);

		/// <summary>Releases all resources allocated by repository if applicable.</summary>
		/// <param name="repository">Repository to close.</param>
		/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		void CloseRepository(IRepository repository);

		/// <summary>Creates control to initialize new repository.</summary>
		/// <returns>Control to initialize new repository.</returns>
		Control CreateInitDialog();

		/// <summary>Creates control to clone existing repository.</summary>
		/// <returns>Control to clone existing repository.</returns>
		Control CreateCloneDialog();

		/// <summary>Get list of repository operations.</summary>
		/// <param name="workingDirectory">Repository working directory.</param>
		IEnumerable<GuiCommand> GetRepositoryCommands(string workingDirectory);

		#endregion
	}
}
