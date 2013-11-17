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

namespace gitter.Git
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;
	
	using gitter.Git.AccessLayer;
	
	using Resources = gitter.Git.Properties.Resources;
	
	public abstract class LogSourceBase : ILogSource
	{
		protected LogSourceBase()
		{
		}

		public abstract Repository Repository { get; }

		public abstract Task<RevisionLog> GetRevisionLogAsync(LogOptions options, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return "log";
		}
	}
}
