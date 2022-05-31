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

namespace gitter.Git;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;

/// <summary>Represents a tag on remote repository.</summary>
public sealed class RemoteRepositoryTag : BaseRemoteReference
{
	internal RemoteRepositoryTag(RemoteReferencesCollection refs, string name, TagType type, Hash hash)
		: base(refs, name, hash)
	{
		TagType = type;
	}

	public override ReferenceType ReferenceType => ReferenceType.Tag;

	public TagType TagType { get; internal set; }

	protected override void DeleteCore() => References.RemoveTag(this);

	protected override Task DeleteCoreAsync(IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
		=> References.RemoveTagAsync(this, progress, cancellationToken);
}
