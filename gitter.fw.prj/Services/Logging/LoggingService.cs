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
	using System.Collections.Generic;
	using System.Diagnostics;

	public class LoggingService
	{
		private static readonly LoggingService _global;
		public static LoggingService Global { get { return _global; } }

		private readonly string _source;

		private static readonly LogEvent[] _log;
		private static readonly List<ILogAppender> _appenders;
		private static int _start;
		private static int _count;

		static LoggingService()
		{
			_global = new LoggingService("global");
			_appenders = new List<ILogAppender>();
			_log = new LogEvent[1000];
		}

		public LoggingService(string source)
		{
			_source = source;
		}

		public LoggingService(Type source)
		{
			_source = source.Name;
		}

		public string Source
		{
			get { return _source; }
		}

		private void LogCore(LogEventType type, Exception exc, string @event)
		{
			Assert.IsNotNull(type);

			var evt = new LogEvent(_source, type, @event, DateTime.Now, exc);
			lock(_log)
			{
				int index;
				if(_count == _log.Length)
				{
					index = _start;
					_start = (_start + 1) % _log.Length;
				}
				else
				{
					index = (_start + _count) % _log.Length;
					++_count;
				}
				_log[index] = evt;
				for(int i = 0; i < _appenders.Count; ++i)
				{
					_appenders[i].Append(evt);
				}
			}
		}

		public static void RegisterAppender(ILogAppender logAppender)
		{
			RegisterAppender(logAppender, true);
		}

		public static void RegisterAppender(ILogAppender logAppender, bool fill)
		{
			Verify.Argument.IsNotNull(logAppender, nameof(logAppender));

			lock(_log)
			{
				_appenders.Add(logAppender);
				if(fill && _count != 0)
				{
					int end = (_start + _count) % _log.Length;
					for(int i = _start; i != end; i = (i + 1) % _log.Length)
					{
						logAppender.Append(_log[i]);
					}
				}
			}
		}

		public static void UnregisterAppender(ILogAppender logAppender)
		{
			Verify.Argument.IsNotNull(logAppender, nameof(logAppender));

			lock(_log)
			{
				_appenders.Remove(logAppender);
			}
		}

		#region Log

		public void Log(LogEventType type, string @event)
		{
			Verify.Argument.IsNotNull(type, nameof(type));

			LogCore(type, null, @event);
		}

		public void Log(LogEventType type, string @event, object arg0)
		{
			Verify.Argument.IsNotNull(type, nameof(type));

			LogCore(type, null, string.Format(@event, arg0));
		}

		public void Log(LogEventType type, string @event, params object[] args)
		{
			Verify.Argument.IsNotNull(type, nameof(type));

			LogCore(type, null, string.Format(@event, args));
		}

		public void Log(LogEventType type, Exception exc, string @event)
		{
			Verify.Argument.IsNotNull(type, nameof(type));

			LogCore(type, exc, @event);
		}

		public void Log(LogEventType type, Exception exc, string @event, object arg0)
		{
			Verify.Argument.IsNotNull(type, nameof(type));

			LogCore(type, exc, string.Format(@event, arg0));
		}

		public void Log(LogEventType type, Exception exc, string @event, params object[] args)
		{
			Verify.Argument.IsNotNull(type, nameof(type));

			LogCore(type, exc, string.Format(@event, args));
		}

		#endregion

		#region Debug

		[Conditional("DEBUG")]
		public void Debug(string @event)
		{
			LogCore(LogEventType.Debug, null, @event);
		}

		[Conditional("DEBUG")]
		public void Debug(string @event, params object[] args)
		{
			LogCore(LogEventType.Debug, null, string.Format(@event, args));
		}

		[Conditional("DEBUG")]
		public void Debug(string @event, object arg0)
		{
			LogCore(LogEventType.Debug, null, string.Format(@event, arg0));
		}

		[Conditional("DEBUG")]
		public void Debug(Exception exc)
		{
			LogCore(LogEventType.Debug, exc, exc?.Message);
		}

		[Conditional("DEBUG")]
		public void Debug(Exception exc, string @event)
		{
			LogCore(LogEventType.Debug, exc, @event);
		}

		[Conditional("DEBUG")]
		public void Debug(Exception exc, string @event, object arg0)
		{
			LogCore(LogEventType.Debug, exc, string.Format(@event, arg0));
		}

		[Conditional("DEBUG")]
		public void Debug(Exception exc, string @event, params object[] args)
		{
			LogCore(LogEventType.Debug, exc, string.Format(@event, args));
		}

		#endregion

		#region Information

		public void Info(string @event)
		{
			LogCore(LogEventType.Information, null, @event);
		}

		public void Info(string @event, params object[] args)
		{
			LogCore(LogEventType.Information, null, string.Format(@event, args));
		}

		public void Info(string @event, object arg0)
		{
			LogCore(LogEventType.Information, null, string.Format(@event, arg0));
		}

		public void Info(Exception exc)
		{
			LogCore(LogEventType.Information, exc, exc?.Message);
		}

		public void Info(Exception exc, string @event)
		{
			LogCore(LogEventType.Information, exc, @event);
		}

		public void Info(Exception exc, string @event, object arg0)
		{
			LogCore(LogEventType.Information, exc, string.Format(@event, arg0));
		}

		public void Info(Exception exc, string @event, params object[] args)
		{
			LogCore(LogEventType.Information, exc, string.Format(@event, args));
		}

		#endregion

		#region Warning

		public void Warning(string @event)
		{
			LogCore(LogEventType.Warning, null, @event);
		}

		public void Warning(string @event, params object[] args)
		{
			LogCore(LogEventType.Warning, null, string.Format(@event, args));
		}

		public void Warning(string @event, object arg0)
		{
			LogCore(LogEventType.Warning, null, string.Format(@event, arg0));
		}

		public void Warning(Exception exc)
		{
			LogCore(LogEventType.Warning, exc, exc?.Message);
		}

		public void Warning(Exception exc, string @event)
		{
			LogCore(LogEventType.Warning, exc, @event);
		}

		public void Warning(Exception exc, string @event, object arg0)
		{
			LogCore(LogEventType.Warning, exc, string.Format(@event, arg0));
		}

		public void Warning(Exception exc, string @event, params object[] args)
		{
			LogCore(LogEventType.Warning, exc, string.Format(@event, args));
		}

		#endregion

		#region Error

		public void Error(string @event)
		{
			LogCore(LogEventType.Error, null, @event);
		}

		public void Error(string @event, params object[] args)
		{
			LogCore(LogEventType.Error, null, string.Format(@event, args));
		}

		public void Error(string @event, object arg0)
		{
			LogCore(LogEventType.Error, null, string.Format(@event, arg0));
		}

		public void Error(Exception exc)
		{
			LogCore(LogEventType.Error, exc, exc?.Message);
		}

		public void Error(Exception exc, string @event)
		{
			LogCore(LogEventType.Error, exc, @event);
		}

		public void Error(Exception exc, string @event, object arg0)
		{
			LogCore(LogEventType.Error, exc, string.Format(@event, arg0));
		}

		public void Error(Exception exc, string @event, params object[] args)
		{
			LogCore(LogEventType.Error, exc, string.Format(@event, args));
		}

		#endregion
	}
}
