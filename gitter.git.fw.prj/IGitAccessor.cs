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

namespace gitter.Git.AccessLayer
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;
	using gitter.Framework.Configuration;

	/// <summary>Defines repository-independent git operations.</summary>
	public interface IGitAccessor : IConfigAccessor
	{
		#region Properties

		/// <summary>Returns provider of this accessor.</summary>
		/// <value>Provider of this accessor</value>
		IGitAccessorProvider Provider { get; }

		/// <summary>Returns git version.</summary>
		/// <value>git version.</value>
		Version GitVersion { get; }

		/// <summary>Create an empty git repository or reinitialize an existing one.</summary>
		IGitAction<InitRepositoryParameters> InitRepository { get; }

		/// <summary>Clone existing repository.</summary>
		IGitAction<CloneRepositoryParameters> CloneRepository { get; }

		#endregion

		#region Methods

		/// <summary>Forces re-check of git version.</summary>
		void InvalidateGitVersion();

		/// <summary>Create <see cref="IRepositoryAccessor"/> for specified <paramref name="repository"/>.</summary>
		/// <param name="repository">git repository to get accessor for.</param>
		/// <returns>git repository accessor.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		IRepositoryAccessor CreateRepositoryAccessor(IGitRepository repository);

		/// <summary>Checks if specified path is a valid repository.</summary>
		/// <param name="path">Path to check.</param>
		/// <returns><c>true</c> if specified path is a valid repository, <c>false</c> otherwise.</returns>
		bool IsValidRepository(string path);

		/// <summary>Save parameters to the specified <paramref name="section"/>.</summary>
		/// <param name="section">Section to store parameters.</param>
		void SaveTo(Section section);

		/// <summary>Load parameters from the specified <paramref name="section"/>.</summary>
		/// <param name="section">Section to look for parameters.</param>
		void LoadFrom(Section section);

		#endregion
	}
}
