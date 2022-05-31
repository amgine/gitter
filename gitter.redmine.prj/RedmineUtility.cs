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

using System;
using System.Xml;

static class RedmineUtility
{
	#region Load

	public static string LoadString(XmlNode node)
	{
		if(node == null) return null;
		return node.InnerText;
	}

	public static bool LoadBoolean(XmlNode node)
	{
		if(node == null) return false;
		return node.InnerText == "true";
	}

	public static int LoadInt(XmlNode node)
	{
		if(node == null) return 0;
		int res;
		if(!int.TryParse(node.InnerText,
			System.Globalization.NumberStyles.Number,
			System.Globalization.CultureInfo.InvariantCulture, out res))
		{
			return 0;
		}
		return res;
	}

	public static double LoadDouble(XmlNode node)
	{
		if(node == null) return 0;
		double res;
		if(!double.TryParse(node.InnerText,
			System.Globalization.NumberStyles.Number,
			System.Globalization.CultureInfo.InvariantCulture, out res))
		{
			return 0;
		}
		return res;
	}

	public static DateTime? LoadDate(XmlNode node)
	{
		if(node == null) return null;
		DateTime res;
		if(!DateTime.TryParse(
			node.InnerText,
			System.Globalization.CultureInfo.InvariantCulture,
			System.Globalization.DateTimeStyles.AssumeUniversal, out res))
			return null;
		return res;
	}

	public static DateTime LoadDateForSure(XmlNode node)
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
		int day, month, year;
		if(!int.TryParse(s[0], out day)) return null;
		if(!int.TryParse(s[1], out month)) return null;
		if(!int.TryParse(s[2], out year)) return null;
		return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
	}

	public static CustomFields LoadCustomFields(XmlNode node, Func<int, string, CustomField> initializer) 
	{
		return new CustomFields(node, initializer);
	}

	public static T LoadObject<T>(XmlNode node, Func<int, T> initializer)
		where T : RedmineObject
	{
		if(node == null) return default(T);
		var idAttr = node.Attributes[NamedRedmineObject.IdProperty.XmlNodeName];
		if(idAttr == null) return default(T);
		int id;
		if(!int.TryParse(idAttr.Value,
			System.Globalization.NumberStyles.Number,
			System.Globalization.CultureInfo.InvariantCulture, out id))
			return default(T);
		return initializer(id);
	}

	public static T LoadNamedObject<T>(XmlNode node, Func<int, string, T> initializer)
		where T : NamedRedmineObject
	{
		if(node == null) return default(T);
		var nameAttr = node.Attributes[NamedRedmineObject.NameProperty.XmlNodeName];
		if(nameAttr == null) return default(T);
		var idAttr = node.Attributes[NamedRedmineObject.IdProperty.XmlNodeName];
		if(idAttr == null) return default(T);
		int id;
		if(!int.TryParse(idAttr.Value,
			System.Globalization.NumberStyles.Number,
			System.Globalization.CultureInfo.InvariantCulture, out id))
			return default(T);
		return initializer(id, nameAttr.Value);
	}

	public static VersionStatus LoadVersionStatus(XmlNode node)
	{
		switch(node.InnerText)
		{
			case "open":
				return VersionStatus.Open;
			case "locked":
				return VersionStatus.Locked;
			case "closed":
				return VersionStatus.Closed;
			default:
				return VersionStatus.Open;
		}
	}

	public static IssueRelationType LoadIssueRelationType(XmlNode node)
	{
		switch(node.InnerText)
		{
			case "blocked":
				return IssueRelationType.Blocked;
			case "duplicated":
				return IssueRelationType.Duplicated;
			case "duplicates":
				return IssueRelationType.Blocked;
			case "blocks":
				return IssueRelationType.Blocks;
			case "follows":
				return IssueRelationType.Follows;
			case "precedes":
				return IssueRelationType.Precedes;
			case "relates":
				return IssueRelationType.Relates;
			default:
				return IssueRelationType.Relates;
		}
	}

	#endregion

	#region Emit

	public static void EmitObjectId(XmlElement element, RedmineObject value)
	{
		if(value != null)
		{
			element.InnerText = XmlConvert.ToString(value.Id);
		}
		else
		{
			element.InnerText = string.Empty;
		}
	}

	public static void EmitString(XmlElement element, string value)
	{
		element.InnerText = value;
	}

	public static void EmitBoolean(XmlElement element, bool value)
	{
		element.InnerText = XmlConvert.ToString(value);
	}

	public static void EmitInt(XmlElement element, int value)
	{
		element.InnerText = XmlConvert.ToString(value);
	}

	public static void EmitDouble(XmlElement element, double value)
	{
		element.InnerText = XmlConvert.ToString(value);
	}

	public static void EmitDate(XmlElement element, DateTime? value)
	{
		if(value.HasValue)
		{
			element.InnerText = XmlConvert.ToString(value.Value, XmlDateTimeSerializationMode.Utc);
		}
	}

	public static void EmitDate(XmlElement element, DateTime value)
	{
		element.InnerText = XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc);
	}

	#endregion
}
