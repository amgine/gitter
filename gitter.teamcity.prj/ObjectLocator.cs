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
	using System.Text;

	public abstract class ObjectLocator
	{
		protected static void BeginArgument(StringBuilder sb, string argname)
		{
			if(sb.Length != 0) sb.Append(',');
			sb.Append(argname);
			sb.Append(':');
		}

		protected static void AppendArgument(StringBuilder sb, string argname, BuildStatus value)
		{
			if(value != BuildStatus.Unknown)
			{
				BeginArgument(sb, argname);
				switch(value)
				{
					case BuildStatus.Error:
						sb.Append("ERROR");
						break;
					case BuildStatus.Failure:
						sb.Append("FAILURE");
						break;
					case BuildStatus.Success:
						sb.Append("SUCCESS");
						break;
					default:
						throw new ApplicationException();
				}
			}
		}

		protected static void AppendArgument(StringBuilder sb, string argname, string value)
		{
			if(!string.IsNullOrWhiteSpace(value))
			{
				BeginArgument(sb, argname);
				if(value.Contains(","))
				{
					sb.Append('(');
					sb.Append(value);
					sb.Append(')');
				}
				else
				{
					sb.Append(value);
				}
			}
		}

		protected static void AppendArgument(StringBuilder sb, string argname, int value)
		{
			if(value != 0)
			{
				BeginArgument(sb, argname);
				sb.Append(value);
			}
		}

		protected static void AppendArgument(StringBuilder sb, string argname, FlagSelector value)
		{
			if(value != FlagSelector.Unspecified)
			{
				BeginArgument(sb, argname);
				switch(value)
				{
					case FlagSelector.True:
						sb.Append("true");
						break;
					case FlagSelector.False:
						sb.Append("false");
						break;
					case FlagSelector.Any:
						sb.Append("any");
						break;
					default:
						throw new ApplicationException();
				}
			}
		}

		protected static void AppendArgument(StringBuilder sb, string argname, ObjectLocator locator)
		{
			if(locator == null) return;
			var value = locator.ToString();
			if(!string.IsNullOrWhiteSpace(value))
			{
				BeginArgument(sb, argname);
				sb.Append('(');
				sb.Append(value);
				sb.Append(')');
			}
		}
	}
}
