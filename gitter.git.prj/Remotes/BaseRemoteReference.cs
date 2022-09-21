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

#nullable enable

namespace gitter.Git;

using System;
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;

public abstract class BaseRemoteReference : IRemoteReference
{
	public event EventHandler? Deleted;

	private void InvokeDeleted()
		=> Deleted?.Invoke(this, EventArgs.Empty);

	internal BaseRemoteReference(RemoteReferencesCollection refs, string name, Hash hash)
	{
		Verify.Argument.IsNotNull(refs);
		Verify.Argument.IsNeitherNullNorWhitespace(name);

		References = refs;
		Name = name;
		Hash = hash;
	}

	protected abstract void DeleteCore();

	protected abstract Task DeleteCoreAsync(IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default);

	public void Delete()
	{
		DeleteCore();
		MarkAsDeleted();
	}

	public async Task DeleteAsync(IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		await DeleteCoreAsync(progress, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		MarkAsDeleted();
	}

	public void MarkAsDeleted()
	{
		if(IsDeleted) return;

		IsDeleted = true;
		InvokeDeleted();
	}

	public bool IsDeleted { get; private set; }

	protected RemoteReferencesCollection References { get; }

	public Remote Remote => References.Remote;

	public string Name { get; }

	public string FullName => ReferenceType switch
		{
			ReferenceType.LocalBranch => GitConstants.LocalBranchPrefix + Name,
			ReferenceType.Tag         => GitConstants.TagPrefix + Name,
			_ => Name,
		};

	public Hash Hash { get; }

	public abstract ReferenceType ReferenceType { get; }

	/// <inheritdoc/>
	public override string ToString() => Name;
}
