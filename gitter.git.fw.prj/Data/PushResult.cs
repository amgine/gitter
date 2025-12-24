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

namespace gitter.Git.AccessLayer;

using System;

/// <summary>Results of pushing local reference to remote repository.</summary>
public sealed class ReferencePushResult(PushResultType type, string localRefName, string remoteRefName, string summary)
{
	public PushResultType Type { get; } = type;

	public string LocalRefName { get; } = localRefName;

	public string RemoteRefName { get; } = remoteRefName;

	public string Summary { get; } = summary;

	private static char TypeToChar(PushResultType type)
		=> type switch
		{
			PushResultType.ForceUpdated     => '+',
			PushResultType.FastForwarded    => ' ',
			PushResultType.Rejected         => '!',
			PushResultType.UpToDate         => '=',
			PushResultType.DeletedReference => '-',
			PushResultType.CreatedReference => '*',
			_ => throw new ArgumentException($"Unknown type: {type}", nameof(type)),
		};

	public override string ToString()
		=> string.Format("{0} {1} -> {2} {3}", TypeToChar(Type), LocalRefName, RemoteRefName, Summary);
}
