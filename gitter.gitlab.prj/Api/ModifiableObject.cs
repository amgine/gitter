#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2020  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
class ModifiableObject
{
	static class Names
	{
		public const string Iid       = @"iid";
		public const string Id        = @"id";
		public const string CreatedAt = @"created_at";
		public const string UpdatedAt = @"updated_at";
	}

	[DataMember]
	[JsonPropertyName(Names.Iid)]
	public int Iid { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Id)]
	public int Id { get; set; }

	[DataMember]
	[JsonPropertyName(Names.CreatedAt)]
	public DateTimeOffset CreatedAt { get; set; }

	[DataMember]
	[JsonPropertyName(Names.UpdatedAt)]
	public DateTimeOffset UpdatedAt { get; set; }
}
