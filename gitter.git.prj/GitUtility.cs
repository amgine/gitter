namespace gitter.Git
{
	using System;
	using System.Text;
	using System.Linq;
	using System.Collections.Generic;

	using gitter.Framework;

	internal static class GitUtility
	{
		public static string FormatDate(DateTime date, DateFormat format)
		{
			switch(format)
			{
				case DateFormat.SystemDefault:
					return date.ToString("dd.MM.yyyy HH:mm:ss");
				case DateFormat.UnixTimestamp:
					return ((int)(date - GitConstants.UnixEraStart).TotalSeconds).ToString();
				case DateFormat.Relative:
					{
						var span = DateTime.Now - date;
						if(span.TotalDays >= 365)
						{
							var years = (int)(span.TotalDays / 365);
							if(years == 1)
								return "1 year ago";
							else
								return years.ToString() + " years ago";
						}
						if(span.TotalDays >= 30)
						{
							var months = (int)(span.TotalDays / 30);
							if(months == 1)
								return "1 month ago";
							else
								return months.ToString() + " months ago";
						}
						if(span.TotalDays >= 7)
						{
							var weeks = (int)(span.TotalDays / 7);
							if(weeks == 1)
								return "1 week ago";
							else
								return weeks.ToString() + " weeks ago";
						}
						if(span.TotalDays >= 1)
						{
							var days = (int)span.TotalDays;
							if(days == 1)
								return "1 day ago";
							else
								return days.ToString() + " days ago";
						}
						if(span.TotalHours >= 1)
						{
							var hours = (int)span.TotalHours;
							if(hours == 1)
								return "1 hour ago";
							else
								return hours.ToString() + " hours ago";
						}
						if(span.TotalMinutes >= 1)
						{
							var minutes = (int)span.TotalMinutes;
							if(minutes == 1)
								return "1 minute ago";
							else
								return minutes.ToString() + " minutes ago";
						}
						var seconds = (int)span.TotalSeconds;
						if(seconds == 1)
							return "1 second ago";
						else
							return seconds.ToString() + " seconds ago";
					}
				case DateFormat.ISO8601:
					return date.FormatISO8601();
				case DateFormat.RFC2822:
					return date.FormatRFC2822();
				default:
					throw new ArgumentException("format");
			}
		}

		public static DateTime UnixTimestampToDateTime(int timestamp)
		{
			return GitConstants.UnixEraStart.AddSeconds(timestamp).ToLocalTime();
		}

		private static string[] RemoveTrailingEmptyStringElements(string[] elements)
		{
			var list = new List<string>(elements.Length);
			for(int i = elements.Length - 1; i > -1; i--)
			{
				if(!(elements[i] == string.Empty))
				{
					list.AddRange(elements.Take<string>(i + 1));
					break;
				}
			}
			return list.ToArray();
		}

		public static string GetHumanishName(string url)
		{
			if(string.IsNullOrEmpty(url))
			{
				throw new InvalidOperationException("Path is either null or empty.");
			}
			string[] elements = url.Split(new char[] { '/' });
			if(elements.Length == 0)
			{
				throw new InvalidOperationException();
			}
			string[] strArray2 = RemoveTrailingEmptyStringElements(elements);
			if(strArray2.Length == 0)
			{
				throw new InvalidOperationException();
			}
			string str = strArray2[strArray2.Length - 1];
			if(".git".Equals(str))
			{
				return strArray2[strArray2.Length - 2];
			}
			if(str.EndsWith(".git"))
			{
				str = str.Substring(0, str.Length - ".git".Length);
			}
			int p = str.LastIndexOf(':');
			if(p != -1) str = str.Substring(p + 1);
			return str;
		}
	}
}
