#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

/// <summary>Type of <see cref="LogEvent"/>.</summary>
/// <param name="Level">Event type level.</param>
/// <param name="Name">Event type name.</param>
/// <param name="ShortName">Event type short name.</param>
/// <param name="Image">Event type image.</param>
public sealed record class LogEventType(int Level, string Name, string ShortName, IImageProvider Image)
{
	public static readonly LogEventType Debug       = new( 0, "Debug",       @"dbg", CommonIcons.Log.Debug);
	public static readonly LogEventType Information = new(10, "Information", @"inf", CommonIcons.Log.Information);
	public static readonly LogEventType Warning     = new(20, "Warning",     @"wrn", CommonIcons.Log.Warning);
	public static readonly LogEventType Error       = new(30, "Error",       @"err", CommonIcons.Log.Error);

	/// <inheritdoc/>
	public override string ToString() => Name;
}
