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

namespace gitter.Framework.Services;

using System;

#nullable enable

public readonly struct LogEvent
{
	public readonly string Source;
	public readonly LogEventType Type;
	public readonly string Message;
	public readonly DateTime Timestamp;
	public readonly Exception? Exception;

	public LogEvent(string source, LogEventType type, string message, DateTime timestamp)
	{
		Source = source;
		Type = type;
		Message = message;
		Timestamp = timestamp;
		Exception = null;
	}

	public LogEvent(string source, LogEventType type, string message, DateTime timestamp, Exception? exception)
	{
		Source = source;
		Type = type;
		Message = message;
		Timestamp = timestamp;
		Exception = exception;
	}

	/// <inheritdoc/>
	public override string ToString()
		=> string.Format("({0}->) [{1}] {2}: {3}", Source, Type.ShortName, Message, Timestamp);
}
