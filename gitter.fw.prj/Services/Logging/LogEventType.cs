namespace gitter.Framework.Services
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	using Resources = gitter.Framework.Properties.Resources;

	/// <summary>Type of <see cref="LogEvent"/>.</summary>
	public sealed class LogEventType
	{
		#region Static

		public static readonly LogEventType Debug		= new LogEventType(0, "Debug", "dbg", Resources.ImgLogDebug);
		public static readonly LogEventType Information = new LogEventType(10, "Information", "inf", Resources.ImgLogInfo);
		public static readonly LogEventType Warning		= new LogEventType(20, "Warning", "wrn", Resources.ImgLogWarning);
		public static readonly LogEventType Error		= new LogEventType(30, "Error", "err", Resources.ImgLogError);

		#endregion

		#region Data

		/// <summary>Event type level.</summary>
		public int Level { get; private set; }
		/// <summary>Event type name.</summary>
		public string Name { get; private set; }
		/// <summary>Event type short name.</summary>
		public string ShortName { get; private set; }
		/// <summary>Event type image.</summary>
		public Bitmap Image { get; private set; }

		#endregion

		#region .ctor

		public LogEventType(int level, string name, string shortName, Bitmap image)
		{
			Level = level;
			Name = name;
			Image = image;
		}

		#endregion

		#region Overrides

		public override string ToString()
		{
			return Name;
		}

		#endregion
	}
}
