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

namespace gitter.Framework.Services
{
	using System;
	using System.Drawing;

	using Resources = gitter.Framework.Properties.Resources;

	/// <summary>Type of <see cref="LogEvent"/>.</summary>
	public sealed class LogEventType
	{
		#region Static

		public static readonly LogEventType Debug		= new LogEventType( 0, "Debug", "dbg", Resources.ImgLogDebug);
		public static readonly LogEventType Information = new LogEventType(10, "Information", "inf", Resources.ImgLogInfo);
		public static readonly LogEventType Warning		= new LogEventType(20, "Warning", "wrn", Resources.ImgLogWarning);
		public static readonly LogEventType Error		= new LogEventType(30, "Error", "err", Resources.ImgLogError);

		#endregion

		#region Data

		/// <summary>Event type level.</summary>
		public int Level { get; }
		/// <summary>Event type name.</summary>
		public string Name { get; }
		/// <summary>Event type short name.</summary>
		public string ShortName { get; }
		/// <summary>Event type image.</summary>
		public Bitmap Image { get; }

		#endregion

		#region .ctor

		public LogEventType(int level, string name, string shortName, Bitmap image)
		{
			Level = level;
			Name = name;
			ShortName = shortName;
			Image = image;
		}

		#endregion

		#region Overrides

		public override string ToString() => Name;

		#endregion
	}
}
