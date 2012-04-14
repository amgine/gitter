namespace gitter
{
	using System;
	using System.ComponentModel;

	/// <summary>Extension methods for raising events.</summary>
	public static class EventHandlerExtensions
	{
		/// <summary>Raise event.</summary>
		/// <param name="handler">Event handler.</param>
		/// <param name="sender">Event sender.</param>
		public static void Raise(this EventHandler handler, object sender)
		{
			if(handler != null) handler(sender, EventArgs.Empty);
		}

		/// <summary>Raise event.</summary>
		/// <param name="handler">Event handler.</param>
		/// <param name="args">Event args.</param>
		/// <param name="sender">Event sender.</param>
		public static void Raise(this EventHandler handler, object sender, EventArgs args)
		{
			if(handler != null) handler(sender, args);
		}

		/// <summary>Raise event.</summary>
		/// <param name="handler">Event handler.</param>
		/// <param name="args">Function, returning event args (not called if <paramref name="handler"/> == null).</param>
		/// <param name="sender">Event sender.</param>
		public static void Raise(this EventHandler handler, object sender, Func<EventArgs> args)
		{
			if(handler != null) handler(sender, args());
		}

		/// <summary>Raise event.</summary>
		/// <typeparam name="T">Type of event args.</typeparam>
		/// <param name="handler">Event handler.</param>
		/// <param name="args">Event args.</param>
		/// <param name="sender">Event sender.</param>
		public static void Raise<T>(this EventHandler<T> handler, object sender, T args)
			where T : EventArgs
		{
			if(handler != null) handler(sender, args);
		}

		/// <summary>Raise event.</summary>
		/// <typeparam name="T">Type of event args.</typeparam>
		/// <param name="handler">Event handler.</param>
		/// <param name="args">Function, returning event args (not called if <paramref name="handler"/> == null).</param>
		/// <param name="sender">Event sender.</param>
		public static void Raise<T>(this EventHandler<T> handler, object sender, Func<T> args)
			where T : EventArgs
		{
			if(handler != null) handler(sender, args());
		}

		/// <summary>Raise event from <see cref="EventHandlerList"/>.</summary>
		///	<param name="evhList">Event list.</param>
		///	<param name="event">Event handler key.</param>
		/// <param name="sender">Event sender.</param>
		public static void Raise(this EventHandlerList evhList, object @event, object sender)
		{
			var handler = (EventHandler)evhList[@event];
			if(handler != null) handler(sender, EventArgs.Empty);
		}

		/// <summary>Raise event from <see cref="EventHandlerList"/>.</summary>
		///	<param name="evhList">Event list.</param>
		///	<param name="event">Event handler key.</param>
		///	<param name="args">Event args.</param>
		/// <param name="sender">Event sender.</param>
		public static void Raise(this EventHandlerList evhList, object @event, object sender, EventArgs args)
		{
			var handler = (EventHandler)evhList[@event];
			if(handler != null) handler(sender, args);
		}

		/// <summary>Raise event from <see cref="EventHandlerList"/>.</summary>
		///	<param name="evhList">Event list.</param>
		///	<param name="event">Event handler key.</param>
		/// <param name="args">Function, returning event args (not called if event handler is not set).</param>
		/// <param name="sender">Event sender.</param>
		public static void Raise(this EventHandlerList evhList, object @event, object sender, Func<EventArgs> args)
		{
			var handler = (EventHandler)evhList[@event];
			if(handler != null) handler(sender, args());
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
			var handler = (EventHandler<T>)evhList[@event];
			if(handler != null) handler(sender, args);
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
			var handler = (EventHandler<T>)evhList[@event];
			if(handler != null) handler(sender, args());
		}
	}
}
