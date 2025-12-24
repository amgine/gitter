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

namespace gitter.Git.AccessLayer.CLI;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using gitter.Framework;

sealed class QuerySymbolicReferenceFunction(IGitRepository repository)
	: IGitFunction<QuerySymbolicReferenceRequest, SymbolicReferenceData>
{
	private const string refPrefix = "ref: ";

	private static SymbolicReferenceData Parse(string? value)
	{
		if(value is { Length: >= 17 } && value.StartsWith(refPrefix + GitConstants.LocalBranchPrefix))
		{
			return new SymbolicReferenceData(value.Substring(16), ReferenceType.LocalBranch);
		}
		if(Sha1Hash.IsValidString(value))
		{
			return new SymbolicReferenceData(value, ReferenceType.Revision);
		}
		return SymbolicReferenceData.None;
	}

	public SymbolicReferenceData Invoke(QuerySymbolicReferenceRequest parameters)
	{
		Verify.Argument.IsNotNull(parameters);

		var fileName = repository.GetGitFileName(parameters.Name);
		if(!File.Exists(fileName)) return SymbolicReferenceData.None;

		string? pointer;
		using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
		if(fs.Length == 0)
		{
			return SymbolicReferenceData.None;
		}
		else
		{
			using var sr = new StreamReader(fs);
			pointer = sr.ReadLine();
		}
		return Parse(pointer);
	}

	public Task<SymbolicReferenceData> InvokeAsync(QuerySymbolicReferenceRequest parameters,
		IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(parameters);

		return Task.Factory.StartNew(
			state => Invoke((QuerySymbolicReferenceRequest)state!),
			parameters,
			cancellationToken,
			TaskCreationOptions.None,
			TaskScheduler.Default);
	}
}
