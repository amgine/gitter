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

	public sealed class RepositoryLogSource : LogSourceBase
	{
		private readonly Repository _repository;

		public RepositoryLogSource(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;
		}

		public override Repository Repository
		{
			get { return _repository; }
		}

		protected override RevisionLog GetLogCore(LogOptions options)
		{
			Assert.IsNotNull(options);

			if(Repository.IsEmpty)
			{
				return new RevisionLog(Repository, new Revision[0]);
			}
			else
			{
				var log = Repository.Accessor.QueryRevisions(options.GetLogParameters());
				var res = Repository.Revisions.Resolve(log);
				return new RevisionLog(Repository, res);
			}
		}
	}
}
