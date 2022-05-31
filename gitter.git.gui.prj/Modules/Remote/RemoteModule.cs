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

namespace gitter.Git.Gui.Modules;

using System.Collections.Generic;

using Autofac;

using gitter.Framework.Controls;
using gitter.Git.Gui.Controls;
using gitter.Git.Gui.Views;

sealed class RemoteModule : Autofac.Module
{
	const string ForRemoteReferences = @"git.remote.references";

	/// <inheritdoc/>
	protected override void Load(ContainerBuilder builder)
	{
		Assert.IsNotNull(builder);

		RegisterColumns(builder);

		builder
			.RegisterType<RemoteReferencesListBox>()
			.OnActivating(static c =>
			{
				var columns = c.Context.ResolveNamed<IEnumerable<CustomListBoxColumn>>(ForRemoteReferences);
				c.Instance.Columns.AddRange(columns);
			})
			.AsSelf()
			.ExternallyOwned();

		builder
			.RegisterType<RemoteView>()
			.AsSelf()
			.ExternallyOwned();

		builder
			.RegisterType<RemoteViewFactory>()
			.Named<IViewFactory>(@"git")
			.As<GitViewFactoryBase>()
			.SingleInstance();

		base.Load(builder);
	}

	private static void RegisterColumns(ContainerBuilder builder)
	{
		Assert.IsNotNull(builder);

		builder
			.RegisterType<NameColumn>()
			.Named<CustomListBoxColumn>(ForRemoteReferences)
			.ExternallyOwned();

		builder
			.RegisterType<HashColumn>()
			.OnActivating(static c =>
			{
				c.Instance.IsVisible  = true;
				c.Instance.Abbreviate = true;
			})
			.Named<CustomListBoxColumn>(ForRemoteReferences)
			.ExternallyOwned();
	}
}
