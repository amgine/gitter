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

namespace gitter;

using System;
using System.ComponentModel;

/// <summary>Extension methods for raising events.</summary>
public static class EventHandlerExtensions
{
	/// <summary>Raise event from <see cref="EventHandlerList"/>.</summary>
	///	<param name="evhList">Event list.</param>
	///	<param name="event">Event handler key.</param>
	/// <param name="sender">Event sender.</param>
	public static void Raise(this EventHandlerList evhList, object @event, object sender)
	{
		((EventHandler?)evhList[@event])?.Invoke(sender, EventArgs.Empty);
	}

	/// <summary>Raise event from <see cref="EventHandlerList"/>.</summary>
	///	<param name="evhList">Event list.</param>
	///	<param name="event">Event handler key.</param>
	///	<param name="args">Event args.</param>
	/// <param name="sender">Event sender.</param>
	public static void Raise(this EventHandlerList evhList, object @event, object sender, EventArgs args)
	{
		((EventHandler?)evhList[@event])?.Invoke(sender, args);
	}

	/// <summary>Raise event from <see cref="EventHandlerList"/>.</summary>
	///	<param name="evhList">Event list.</param>
	///	<param name="event">Event handler key.</param>
	/// <param name="args">Function, returning event args (not called if event handler is not set).</param>
	/// <param name="sender">Event sender.</param>
	public static void Raise(this EventHandlerList evhList, object @event, object sender, Func<EventArgs> args)
	{
		((EventHandler?)evhList[@event])?.Invoke(sender, args());
	}

	/// <summary>Raise event from <see cref="EventHandlerList"/>.</summary>
	/// <typeparam name="T">Type of event args.</typeparam>
	///	<param name="evhList">Event list.</param>
	///	<param name="event">Event handler key.</param>
	/// <param name="args">Event args.</param>
	/// <param name="sender">Event sender.</param>
	public static void Raise<T>(this EventHandlerList evhList, object @event, object sender, T args)
		where T : EventArgs
	{
		((EventHandler<T>?)evhList[@event])?.Invoke(sender, args);
	}

	/// <summary>Raise event from <see cref="EventHandlerList"/>.</summary>
	/// <typeparam name="T">Type of event args.</typeparam>
	///	<param name="evhList">Event list.</param>
	///	<param name="event">Event handler key.</param>
	/// <param name="args">Function, returning event args (not called if event handler is not set).</param>
	/// <param name="sender">Event sender.</param>
	public static void Raise<T>(this EventHandlerList evhList, object @event, object sender, Func<T> args)
		where T : EventArgs
	{
		((EventHandler<T>?)evhList[@event])?.Invoke(sender, args());
	}
}
