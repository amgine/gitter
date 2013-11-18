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

namespace gitter.Git.AccessLayer.CLI
{
	using System.IO;
	using System.Threading;
	using System.Threading.Tasks;
	using gitter.Framework;

	sealed class QuerySymbolicReferenceImpl : IGitFunction<QuerySymbolicReferenceParameters, SymbolicReferenceData>
	{
		private const string refPrefix = "ref: ";

		private readonly IGitRepository _repository;

		public QuerySymbolicReferenceImpl(IGitRepository repository)
		{
			_repository = repository;
		}

		private static SymbolicReferenceData Parse(string value)
		{
			if(value != null && value.Length >= 17 && value.StartsWith(refPrefix + GitConstants.LocalBranchPrefix))
			{
				return new SymbolicReferenceData(value.Substring(16), ReferenceType.LocalBranch);
			}
			else
			{
				if(GitUtils.IsValidSHA1(value))
				{
					return new SymbolicReferenceData(value, ReferenceType.Revision);
				}
			}
			return new SymbolicReferenceData(null, ReferenceType.None);
		}

		public SymbolicReferenceData Invoke(QuerySymbolicReferenceParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var fileName = _repository.GetGitFileName(parameters.Name);
			if(File.Exists(fileName))
			{
				string pointer;
				using(var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					if(fs.Length == 0)
					{
						return new SymbolicReferenceData(null, ReferenceType.None);
					}
					else
					{
						using(var sr = new StreamReader(fs))
						{
							pointer = sr.ReadLine();
							sr.Close();
						}
					}
				}
				return Parse(pointer);
			}
			return new SymbolicReferenceData(null, ReferenceType.None);
		}

		public Task<SymbolicReferenceData> InvokeAsync(QuerySymbolicReferenceParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return Task.Factory.StartNew<SymbolicReferenceData>(
				(state) => Invoke((QuerySymbolicReferenceParameters)state),
				parameters,
				cancellationToken,
				TaskCreationOptions.None,
				TaskScheduler.Default);
		}
	}
}
