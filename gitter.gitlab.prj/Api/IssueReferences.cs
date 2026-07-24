#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2026  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab.Api;

using System.Runtime.Serialization;

[DataContract]
sealed class IssueReferences
{
	static class Names
	{
		public const string Short    = @"short";
		public const string Relative = @"relative";
		public const string Full     = @"full";
	}

	[DataMember]
	[JsonPropertyName(Names.Short)]
	public string? Short { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Relative)]
	public string? Relative { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Full)]
	public string? Full { get; set; }

	public override string ToString() => Full ?? Relative ?? Short ?? string.Empty;
}
