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

namespace gitter.Git
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;

	/// <summary>Represents a reference on remote repository.</summary>
	public interface IRemoteReference
	{
		event EventHandler Deleted;

		/// <summary>Remote repository.</summary>
		Remote Remote { get; }

		/// <summary>Reference name.</summary>
		string Name { get; }

		/// <summary>Commit hash.</summary>
		Hash Hash { get; }

		ReferenceType ReferenceType { get; }

		/// <summary>Remove reference from remote repository.</summary>
		void Delete();

		Task DeleteAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		bool IsDeleted { get; }
	}
}
