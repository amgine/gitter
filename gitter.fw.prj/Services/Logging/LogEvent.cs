namespace gitter.Framework.Services
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	public struct LogEvent
	{
		#region Data

		public readonly string Source;
		public readonly LogEventType Type;
		public readonly string Message;
		public readonly DateTime Timestamp;
		public readonly Exception Exception;

		#endregion

		public LogEvent(string source, LogEventType type, string message, DateTime timestamp)
		{
			Source = source;
			Type = type;
			Message = message;
			Timestamp = timestamp;
			Exception = null;
		}

		public LogEvent(string source, LogEventType type, string message, DateTime timestamp, Exception exception)
		{
			Source = source;
			Type = type;
			Message = message;
			Timestamp = timestamp;
			Exception = exception;
		}

		public override string ToString()
		{
			return string.Format("({0}->) [{1}] {2}: {3}", Source, Type.ShortName, Message, Timestamp);
		}
	}
}
