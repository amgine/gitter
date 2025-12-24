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

namespace gitter.Redmine;

#nullable enable

using System;
using System.Xml;

static class RedmineUtility
{
	#region Load

	public static string? LoadString(XmlNode? node)
		=> node?.InnerText;

	public static bool LoadBoolean(XmlNode? node)
		=> node?.InnerText == "true";

	public static int LoadInt(XmlNode? node)
	{
		if(node is null) return 0;
		if(!int.TryParse(node.InnerText,
			System.Globalization.NumberStyles.Number,
			System.Globalization.CultureInfo.InvariantCulture, out var res))
		{
			return 0;
		}
		return res;
	}

	public static double LoadDouble(XmlNode? node)
	{
		if(node is null) return 0;
		if(!double.TryParse(node.InnerText,
			System.Globalization.NumberStyles.Number,
			System.Globalization.CultureInfo.InvariantCulture, out var res))
		{
			return 0;
		}
		return res;
	}

	public static DateTime? LoadDate(XmlNode? node)
	{
		if(node is null) return null;
		if(!DateTime.TryParse(
			node.InnerText,
			System.Globalization.CultureInfo.InvariantCulture,
			System.Globalization.DateTimeStyles.AssumeUniversal, out var res))
		{
			return null;
		}
		return res;
	}

	public static DateTime LoadDateRequired(XmlNode node)
	{
		return DateTime.Parse(node.InnerText,
			System.Globalization.CultureInfo.InvariantCulture,
			System.Globalization.DateTimeStyles.AssumeUniversal);
	}

	public static DateTime? LoadDate2(XmlNode node)
	{
		if(node == null) return null;
		var s = node.InnerText.Split('/');
		if(s.Length != 3) return null;
		if(!int.TryParse(s[0], out var day)) return null;
		if(!int.TryParse(s[1], out var month)) return null;
		if(!int.TryParse(s[2], out var year)) return null;
		return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
	}

	public static CustomFields LoadCustomFields(XmlNode node, Func<int, string, CustomField> initializer) 
	{
		return new CustomFields(node, initializer);
	}

	public static T? LoadObject<T>(XmlNode? node, Func<int, T> initializer)
		where T : RedmineObject
	{
		var idAttr = node?.Attributes?[NamedRedmineObject.IdProperty.XmlNodeName];
		if(idAttr is null) return default;
		if(!int.TryParse(idAttr.Value,
			System.Globalization.NumberStyles.Number,
			System.Globalization.CultureInfo.InvariantCulture, out var id))
			return default;
		return initializer(id);
	}

	public static T? LoadNamedObject<T>(XmlNode? node, Func<int, string, T> initializer)
		where T : NamedRedmineObject
	{
		if(node is null) return default;
		var nameAttr = node.Attributes?[NamedRedmineObject.NameProperty.XmlNodeName];
		if(nameAttr is null) return default;
		var idAttr = node.Attributes?[NamedRedmineObject.IdProperty.XmlNodeName];
		if(idAttr is null) return default;
		if(!int.TryParse(idAttr.Value,
			System.Globalization.NumberStyles.Number,
			System.Globalization.CultureInfo.InvariantCulture, out var id))
			return default;
		return initializer(id, nameAttr.Value);
	}

	public static VersionStatus LoadVersionStatus(XmlNode node)
		=> node.InnerText switch
		{
			"open"   => VersionStatus.Open,
			"locked" => VersionStatus.Locked,
			"closed" => VersionStatus.Closed,
			_        => VersionStatus.Open,
		};

	public static IssueRelationType LoadIssueRelationType(XmlNode node)
		=> node.InnerText switch
		{
			"blocked"    => IssueRelationType.Blocked,
			"duplicated" => IssueRelationType.Duplicated,
			"duplicates" => IssueRelationType.Blocked,
			"blocks"     => IssueRelationType.Blocks,
			"follows"    => IssueRelationType.Follows,
			"precedes"   => IssueRelationType.Precedes,
			"relates"    => IssueRelationType.Relates,
			_            => IssueRelationType.Relates,
		};

	#endregion

	#region Emit

	public static void EmitObjectId(XmlElement element, RedmineObject value)
		=> element.InnerText = value is not null
			? XmlConvert.ToString(value.Id)
			: string.Empty;

	public static void EmitString(XmlElement element, string value)
		=> element.InnerText = value;

	public static void EmitBoolean(XmlElement element, bool value)
		=> element.InnerText = XmlConvert.ToString(value);

	public static void EmitInt(XmlElement element, int value)
		=> element.InnerText = XmlConvert.ToString(value);

	public static void EmitDouble(XmlElement element, double value)
		=> element.InnerText = XmlConvert.ToString(value);

	public static void EmitDate(XmlElement element, DateTime? value)
	{
		if(value.HasValue)
		{
			element.InnerText = XmlConvert.ToString(value.Value, XmlDateTimeSerializationMode.Utc);
		}
	}

	public static void EmitDate(XmlElement element, DateTime value)
		=> element.InnerText = XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc);

	#endregion
}
