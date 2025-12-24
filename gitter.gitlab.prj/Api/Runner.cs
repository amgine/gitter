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
sealed class Runner
{
	static class Names
	{
		public const string Id          = @"id";
		public const string Description = @"description";
		public const string IpAddresses = @"ip_address";
		public const string Active      = @"active";
		public const string Paused      = @"paused";
		public const string IsShared    = @"is_shared";
		public const string Type        = @"runner_type";
		public const string Name        = @"name";
		public const string Online      = @"online";
		public const string Status      = @"status";
	}

	[DataMember]
	[JsonPropertyName(Names.Id)]
	public int Id { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Description)]
	public string? Description { get; set; }

	[DataMember]
	[JsonPropertyName(Names.IpAddresses)]
	public string IpAddresses { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Active)]
	public bool Active { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Paused)]
	public bool Paused { get; set; }

	[DataMember]
	[JsonPropertyName(Names.IsShared)]
	public bool IsShared { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Type)]
	public string Type { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Name)]
	public string Name { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Online)]
	public bool Online { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Status)]
	public string Status { get; set; } = default!;
}
