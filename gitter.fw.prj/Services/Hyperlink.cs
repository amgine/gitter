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
using System.Collections.Generic;

/// <summary>Represents a hyperlink in text.</summary>
/// <param name="text">Link text.</param>
/// <param name="url">Link URL.</param>
public sealed class Hyperlink(Substring text, string url)
{
	private static readonly List<NavigationHandler> _handlers = [];
	private static readonly LockType _handlersSync = new();

	private sealed class NavigationHandler(Func<string, bool> handler) : IDisposable
	{
		public bool Navigate(string url) => handler(url);

		public bool IsDisposed { get; private set; }

		public void Dispose()
		{
			if(IsDisposed) return;
			lock(_handlersSync)
			{
				if(IsDisposed) return;
				_handlers.Remove(this);
				IsDisposed = true;
			}
		}
	}

	public static IDisposable RegisterInternalHandler(Func<string, bool> onNavigate)
	{
		Verify.Argument.IsNotNull(onNavigate);

		var handler = new NavigationHandler(onNavigate);
		lock(_handlersSync)
		{
			_handlers.Add(handler);
		}
		return handler;
	}

	private static void NavigateInternal(string url)
	{
		lock(_handlersSync)
		{
			foreach(var handler in _handlers)
			{
				if(handler.Navigate(url)) break;
			}
		}
	}

	/// <summary>Link text.</summary>
	public Substring Text { get; } = text;

	/// <summary>Link URL.</summary>
	public string Url { get; } = url;

	/// <summary>Navigate the hyperlink.</summary>
	public void Navigate()
	{
		if(Url.StartsWith("gitter://"))
		{
			NavigateInternal(Url);
		}
		else
		{
			Utility.OpenUrl(Url);
		}
	}

	/// <inheritdoc/>
	public override string ToString() => Url;
}
