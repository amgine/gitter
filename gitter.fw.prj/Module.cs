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

namespace gitter.Framework;

using System;
using System.Collections.Generic;

using Autofac;

using gitter.Framework.Options;

using Resources = gitter.Framework.Properties.Resources;

public sealed class Module : Autofac.Module
{
	/// <inheritdoc/>
	protected override void Load(ContainerBuilder builder)
	{
		Assert.IsNotNull(builder);

		builder
			.RegisterType<PropertyPageRepository>()
			.As<IPropertyPageProvider>()
			.As<IPropertyPageRepository>()
			.OnActivating(static c =>
			{
				foreach(var factory in c.Context.Resolve<IEnumerable<IPropertyPageFactory>>())
				{
					c.Instance.RegisterPropertyPageFactory(factory);
				}
			})
			.SingleInstance();

		builder.RegisterTypes(new[]
		{
			typeof(IntegrationOptionsPage),
			typeof(SpellingPage),
			typeof(AppearancePage),
			typeof(FontsPage),
			typeof(ColorsPage),
			typeof(OptionsDialog),
		}).AsSelf().ExternallyOwned();

		builder
			.Register(static c =>
			{
				return new PropertyPageFactory<IntegrationOptionsPage>(
					IntegrationOptionsPage.Guid,
					Resources.StrIntegration,
					null,
					PropertyPageFactory.RootGroupGuid,
					c.Resolve<IFactory<IntegrationOptionsPage>>());
			})
			.As<IPropertyPageFactory>()
			.SingleInstance();

		builder
			.Register(static c =>
			{
				return new PropertyPageFactory<SpellingPage>(
					SpellingPage.Guid,
					Resources.StrSpelling,
					null,
					PropertyPageFactory.RootGroupGuid,
					c.Resolve<IFactory<SpellingPage>>());
			})
			.As<IPropertyPageFactory>()
			.SingleInstance();

		builder
			.Register(static c =>
			{
				return new PropertyPageFactory<AppearancePage>(
					PropertyPageFactory.AppearanceGroupGuid,
					Resources.StrAppearance,
					null,
					PropertyPageFactory.RootGroupGuid,
					c.Resolve<IFactory<AppearancePage>>());
			})
			.As<IPropertyPageFactory>()
			.SingleInstance();

		builder
			.Register(static c =>
			{
				return new PropertyPageFactory<FontsPage>(
					FontsPage.Guid,
					Resources.StrFonts,
					null,
					PropertyPageFactory.AppearanceGroupGuid,
					c.Resolve<IFactory<FontsPage>>());
			})
			.As<IPropertyPageFactory>()
			.SingleInstance();

		/*
		builder
			.Register(static c =>
			{
				return new PropertyPageFactory<ColorsPage>(
					ColorsPage.Guid,
					Resources.StrColors,
					null,
					PropertyPageFactory.AppearanceGroupGuid,
					c.Resolve<IFactory<ColorsPage>>());
			})
			.As<IPropertyPageFactory>()
			.SingleInstance();
		*/

		base.Load(builder);
	}
}
