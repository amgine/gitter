#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.TeamCity;

using System;

using Autofac;

using gitter.Framework;
using gitter.Framework.Options;
using gitter.TeamCity.Options;
using gitter.TeamCity.Properties;

public sealed class Module : Autofac.Module
{
	/// <inheritdoc/>
	protected override void Load(ContainerBuilder builder)
	{
		Assert.IsNotNull(builder);

		builder
			.RegisterType<TeamCityServiceProvider>()
			.As<IRepositoryServiceProvider>()
			.AsSelf()
			.SingleInstance();

		builder
			.RegisterTypes(
			[
				typeof(ConfigurationPage),
			])
			.AsSelf()
			.ExternallyOwned();

		builder
			.Register(static c =>
			{
				return new PropertyPageFactory<ConfigurationPage>(
					ConfigurationPage.Guid,
					Resources.StrTeamCity,
					null,
					IntegrationOptionsPage.Guid,
					c.Resolve<IFactory<ConfigurationPage>>());
			})
			.As<IPropertyPageFactory>()
			.SingleInstance();

		base.Load(builder);
	}
}
