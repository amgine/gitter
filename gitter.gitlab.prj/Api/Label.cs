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

namespace gitter.GitLab.Api;

using System;
using System.Runtime.Serialization;

[DataContract]
sealed class Label : IEquatable<Label>
{
	static class Names
	{
		public const string Id                = @"id";
		public const string Name              = @"name";
		public const string Color             = @"color";
		public const string TextColor         = @"text_color";
		public const string Description       = @"description";
		public const string OpenIssuesCount   = @"open_issues_count";
		public const string ClosedIssuesCount = @"closed_issues_count";
		public const string Subscribed        = @"subscribed";
		public const string Priority          = @"priority";
		public const string IsProjectLabel    = @"is_project_label";
	}

	[DataMember]
	[JsonPropertyName(Names.Id)]
	public long Id { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Name)]
	public string Name { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Color)]
	public string Color { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.TextColor)]
	public string? TextColor { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Description)]
	public string? Description { get; set; }

	[DataMember]
	[JsonPropertyName(Names.OpenIssuesCount)]
	public int OpenIssuesCount { get; set; }

	[DataMember]
	[JsonPropertyName(Names.ClosedIssuesCount)]
	public int ClosedIssuesCount { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Subscribed)]
	public bool Subscribed { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Priority)]
	public int? Priority { get; set; }

	[DataMember]
	[JsonPropertyName(Names.IsProjectLabel)]
	public bool? IsProjectLabel { get; set; }

	public bool Equals(Label? other) => other is not null && other.Id == Id;
	public override bool Equals(object? obj) => obj is Label l && Equals(l);
	public override int GetHashCode() => Id.GetHashCode();
	public override string ToString() => Name;
}
