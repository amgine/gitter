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

namespace gitter.Git.Gui.Views;

using System;

using Autofac;

using gitter.Framework;
using gitter.Framework.Controls;

abstract class GitViewFactoryBase(Guid guid, string name, IImageProvider imageProvider, bool singleton = false)
	: ViewFactoryBase(guid, name, imageProvider, singleton)
{
	public ILifetimeScope? Scope { get; set; }

	protected ILifetimeScope RequireScope()
		=> Scope
		?? throw new InvalidOperationException("Component scope is not available.");
}

class GitViewFactoryBase<T>(Guid guid, string name, IImageProvider imageProvider, bool singleton = false)
	: GitViewFactoryBase(guid, name, imageProvider, singleton) where T : ViewBase
{
	/// <summary>Attaches lifetime to a view so they will be disposed together.</summary>
	/// <param name="view">View.</param>
	/// <param name="scope">Lifetime scope.</param>
	private static void AttachScope(T view, ILifetimeScope scope)
		=> view.Disposed += (_, _) => scope.Dispose();

	/// <summary>Resolves view instance using the specified lifetime scope.</summary>
	/// <param name="scope">View lifetime scope.</param>
	/// <returns>Resolved view.</returns>
	protected virtual T ResolveView(ILifetimeScope scope)
		=> scope.Resolve<T>();

	/// <inheritdoc/>
	protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
	{
		var scope = RequireScope().BeginLifetimeScope();
		try
		{
			if(ResolveView(scope) is not { IsDisposed: false } view)
			{
				throw new InvalidOperationException("Resolved view is null or disposed.");
			}
			AttachScope(view, scope);
			return view;
		}
		catch
		{
			scope.Dispose();
			throw;
		}
	}
}
