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

namespace gitter.Git.Gui;

using Autofac;

using gitter.Framework;
using gitter.Framework.Options;

using gitter.Git.Gui.Dialogs;

using Resources = gitter.Git.Gui.Properties.Resources;

public sealed class Module : Autofac.Module
{
	/// <inheritdoc/>
	protected override void Load(ContainerBuilder builder)
	{
		Assert.IsNotNull(builder);

		builder
			.RegisterType<DefaultGraphBuilderFactory>()
			.As<IGraphBuilderFactory>()
			.SingleInstance();

		builder
			.RegisterType<ValueSource<GraphStyleOptions>>()
			.WithParameter(TypedParameter.From(GraphStyleOptions.Default))
			.OnActivating(static c =>
			{
				var section = c.Context.Resolve<RepositoryProvider>().ConfigSection.GetCreateSection("GraphStyle");
				c.Instance.Value = GraphStyleOptions.LoadFrom(section);
			})
			.As<IValueProvider<GraphStyleOptions>>()
			.As<IValueSource<GraphStyleOptions>>()
			.SingleInstance();

		builder
			.RegisterType<GraphStyle>()
			.As<IGraphStyle>()
			.AsSelf()
			.SingleInstance();

		builder.RegisterModule<gitter.Git.AccessLayer.CLI.Module>();

		builder.RegisterModule<Modules.GitModule>();
		builder.RegisterModule<Modules.HistoryModule>();
		builder.RegisterModule<Modules.CommitModule>();
		builder.RegisterModule<Modules.ConfigModule>();
		builder.RegisterModule<Modules.ReflogModule>();
		builder.RegisterModule<Modules.ReferencesModule>();
		builder.RegisterModule<Modules.RemotesModule>();
		builder.RegisterModule<Modules.RemoteModule>();
		builder.RegisterModule<Modules.StashModule>();
		builder.RegisterModule<Modules.SubmodulesModule>();
		builder.RegisterModule<Modules.ContributorsModule>();
		builder.RegisterModule<Modules.TreeModule>();
		builder.RegisterModule<Modules.DiffModule>();
		builder.RegisterModule<Modules.BlameModule>();

		builder
			.RegisterType<AccessLayerOptionsPageFactory>()
			.As<IAccessLayerOptionsPageFactory>()
			.SingleInstance();

		builder
			.RegisterType<RepositoryProvider>()
			.AsSelf()
			.As<IRepositoryProvider>()
			.As<IGitRepositoryProvider>()
			.SingleInstance();

		builder
			.RegisterTypes(new[]
			{
				typeof(GitOptionsPage),
				typeof(ConfigurationPage),
				typeof(GraphStylePage),
				typeof(VersionCheckDialog),
				typeof(InitDialog),
				typeof(CloneDialog),
			})
			.AsSelf()
			.ExternallyOwned();

		builder
			.Register(static c =>
			{
				return new PropertyPageFactory<GitOptionsPage>(
					GitOptionsPage.Guid,
					Resources.StrGit,
					null,
					PropertyPageFactory.RootGroupGuid,
					c.Resolve<IFactory<GitOptionsPage>>());
			})
			.As<IPropertyPageFactory>()
			.SingleInstance();

		builder
			.Register(static c =>
			{
				return new PropertyPageFactory<ConfigurationPage>(
					ConfigurationPage.Guid,
					Resources.StrConfig,
					null,
					GitOptionsPage.Guid,
					c.Resolve<IFactory<ConfigurationPage>>());
			})
			.As<IPropertyPageFactory>()
			.SingleInstance();

		builder
			.Register(static c =>
			{
				return new PropertyPageFactory<GraphStylePage>(
					GraphStylePage.Guid,
					Resources.StrRevisionGraph,
					null,
					PropertyPageFactory.AppearanceGroupGuid,
					c.Resolve<IFactory<GraphStylePage>>());
			})
			.As<IPropertyPageFactory>()
			.SingleInstance();

		base.Load(builder);
	}
}
