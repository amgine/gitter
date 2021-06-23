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

namespace gitter.TeamCity
{
	using System;
	using System.Xml;

	static class TeamCityUtility
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
			var text			= node.InnerText;
			var dateString		= text.Substring(0, 15);
			var date			= DateTime.ParseExact(dateString, @"yyyyMMdd\THHmmss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);
			var offsetHours		= int.Parse(text.Substring(16, 2), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
			var offsetMinutes	= int.Parse(text.Substring(18, 2), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
			var offset = new TimeSpan(offsetHours, offsetMinutes, 0);
			if(text[15] == '+')
			{
				date += offset;
			}
			else if(text[15] == '-')
			{
				date -= offset;
			}
			return date.ToLocalTime();
		}

		public static BuildStatus LoadBuildStatus(XmlNode node)
		{
			if(node is null) return BuildStatus.Unknown;
			return node.InnerText switch
			{
				"ERROR"   => BuildStatus.Error,
				"FAILURE" => BuildStatus.Failure,
				"SUCCESS" => BuildStatus.Success,
				_ => BuildStatus.Unknown,
			};
		}

		#endregion
	}
}
