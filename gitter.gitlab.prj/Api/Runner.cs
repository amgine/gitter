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
#if SYSTEM_TEXT_JSON
using System.Text.Json.Serialization;
#elif NEWTONSOFT_JSON
using Newtonsoft.Json;
#endif

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

#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Id)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Id)]
#endif
	public int Id { get; set; }

#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Description)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Description)]
#endif
	public string Description { get; set; }

#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.IpAddresses)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.IpAddresses)]
#endif
	public string IpAddresses { get; set; }

#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Active)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Active)]
#endif
	public bool Active { get; set; }

#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Paused)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Paused)]
#endif
	public bool Paused { get; set; }

#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.IsShared)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.IsShared)]
#endif
	public bool IsShared { get; set; }

#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Type)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Type)]
#endif
	public string Type { get; set; }

#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Name)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Name)]
#endif
	public string Name { get; set; }

#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Online)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Online)]
#endif
	public bool Online { get; set; }

#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Status)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Status)]
#endif
	public string Status { get; set; }
}
