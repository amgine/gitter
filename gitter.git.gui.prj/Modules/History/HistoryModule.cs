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

using System;
using System.Collections.Generic;
using System.Reflection;

using Autofac;
using Autofac.Core;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Git.Gui.Controls;
using gitter.Git.Gui.Views;

sealed class HistoryModule : Autofac.Module
{
	sealed class NamedFactoryParameter<T> : Parameter
	{
		sealed class Factory : IFactory<T>
		{
			public Factory(IComponentContext componentContext, string name)
			{
				Assert.IsNotNull(componentContext);
				Assert.IsNotNull(name);

				ComponentContext = componentContext;
				Name             = name;
			}

			private IComponentContext ComponentContext { get; }

			private string Name { get; }

			public T Create() => ComponentContext.ResolveNamed<T>(Name);
		}

		public NamedFactoryParameter(string name)
		{
			Verify.Argument.IsNotNull(name);

			Name = name;
		}

		private string Name { get; }

		public override bool CanSupplyValue(ParameterInfo pi, IComponentContext context, out Func<object> valueProvider)
		{
			Assert.IsNotNull(pi);
			Assert.IsNotNull(context);

			if(typeof(IFactory<T>).IsAssignableFrom(pi.ParameterType))
			{
				valueProvider = () => new Factory(context, Name);
				return true;
			}
			valueProvider = default;
			return false;
		}
	}

	const string ForRevisionHistory     = @"git.revisions";
	const string ForPathRevisionHistory = @"git.path.revisions";

	/// <inheritdoc/>
	protected override void Load(ContainerBuilder builder)
	{
		Assert.IsNotNull(builder);

		RegisterColumns(builder);
		RegisterListBox(builder, ForRevisionHistory);
		RegisterListBox(builder, ForPathRevisionHistory);

		RegisterView<HistoryView>    (builder, ForRevisionHistory);
		RegisterView<PathHistoryView>(builder, ForPathRevisionHistory);

		builder
			.RegisterTypes(new[]
			{
				typeof(HistoryViewFactory),
				typeof(PathHistoryViewFactory),
			})
			.Named<IViewFactory>(@"git")
			.As<GitViewFactoryBase>()
			.SingleInstance();

		base.Load(builder);
	}

	private static void RegisterView<T>(ContainerBuilder builder, string name)
		where T : HistoryViewBase
	{
		Assert.IsNotNull(builder);
		Assert.IsNeitherNullNorWhitespace(name);

		builder
			.RegisterType<T>()
			.WithParameter(new NamedFactoryParameter<RevisionListBox>(name))
			.AsSelf()
			.ExternallyOwned();
	}

	private static void RegisterHashColumns(ContainerBuilder builder)
	{
		Assert.IsNotNull(builder);

		builder
			.RegisterType<HashColumn>()
			.Named<CustomListBoxColumn>(ForRevisionHistory)
			.Named<CustomListBoxColumn>(ForPathRevisionHistory)
			.ExternallyOwned();

		builder
			.RegisterType<TreeHashColumn>()
			.Named<CustomListBoxColumn>(ForRevisionHistory)
			.Named<CustomListBoxColumn>(ForPathRevisionHistory)
			.ExternallyOwned();
	}

	private static void RegisterSubjectColumn(ContainerBuilder builder)
	{
		Assert.IsNotNull(builder);

		const string ForFakeItem = @"git.revision.fake.subject";
		const string ForRealItem = @"git.revision.real.subject";
		const string Composite   = @"git.revision.subject";

		builder
			.RegisterType<FakeRevisionListItemSubjectPainter>()
			.AsSelf()
			.Named<ISubItemPainter>(ForFakeItem)
			.SingleInstance();

		builder
			.RegisterType<RevisionSubjectPainter>()
			.AsSelf()
			.Named<ISubItemPainter>(ForRealItem)
			.SingleInstance();

		builder
			.Register(static c => new CompositeSubItemPainter(
				c.ResolveNamed<ISubItemPainter>(ForRealItem),
				c.ResolveNamed<ISubItemPainter>(ForFakeItem)))
			.Named<ISubItemPainter>(Composite)
			.SingleInstance();

		builder
			.RegisterType<SubjectColumn>()
			.Named<CustomListBoxColumn>(ForRevisionHistory)
			.Named<CustomListBoxColumn>(ForPathRevisionHistory)
			.WithParameter(ResolvedParameter.ForNamed<ISubItemPainter>(Composite))
			.ExternallyOwned();
	}

	private static void RegisterColumns(ContainerBuilder builder)
	{
		Assert.IsNotNull(builder);

		RegisterHashColumns(builder);

		builder
			.RegisterType<CommitDateColumn>()
			.Named<CustomListBoxColumn>(ForRevisionHistory)
			.Named<CustomListBoxColumn>(ForPathRevisionHistory)
			.ExternallyOwned();

		builder
			.RegisterType<GraphColumn>()
			.Named<CustomListBoxColumn>(ForRevisionHistory)
			.ExternallyOwned();

		RegisterSubjectColumn(builder);

		builder
			.RegisterType<FakeRevisionUserPainter>()
			.Named<ISubItemPainter>(@"git.revision.fake.user")
			.SingleInstance();

		builder
			.RegisterType<RevisionAuthorPainter>()
			.Named<ISubItemPainter>(@"git.revision.real.author")
			.SingleInstance();

		builder
			.RegisterType<RevisionCommitterPainter>()
			.Named<ISubItemPainter>(@"git.revision.real.committer")
			.SingleInstance();

		builder
			.Register(static c => new CompositeSubItemPainter(
				c.ResolveNamed<ISubItemPainter>(@"git.revision.real.author"),
				c.ResolveNamed<ISubItemPainter>(@"git.revision.fake.user")))
			.Named<ISubItemPainter>(@"git.revision.author")
			.SingleInstance();

		builder
			.Register(static c => new CompositeSubItemPainter(
				c.ResolveNamed<ISubItemPainter>(@"git.revision.real.committer"),
				c.ResolveNamed<ISubItemPainter>(@"git.revision.fake.user")))
			.Named<ISubItemPainter>(@"git.revision.committer")
			.SingleInstance();

		builder
			.RegisterType<CommitterColumn>()
			.WithParameter(ResolvedParameter.ForNamed<ISubItemPainter>(@"git.revision.committer"))
			.Named<CustomListBoxColumn>(ForRevisionHistory)
			.Named<CustomListBoxColumn>(ForPathRevisionHistory)
			.ExternallyOwned();

		builder
			.RegisterType<FakeRevisionEmailPainter>()
			.Named<ISubItemPainter>(@"git.revision.fake.user.email")
			.SingleInstance();

		builder
			.RegisterType<RevisionCommitterEmailPainter>()
			.Named<ISubItemPainter>(@"git.revision.real.user.committer.email")
			.SingleInstance();

		builder
			.RegisterType<RevisionAuthorEmailPainter>()
			.Named<ISubItemPainter>(@"git.revision.real.user.author.email")
			.SingleInstance();

		builder
			.Register(static c => new CompositeSubItemPainter(
				c.ResolveNamed<ISubItemPainter>(@"git.revision.real.user.committer.email"),
				c.ResolveNamed<ISubItemPainter>(@"git.revision.fake.user.email")))
			.Named<ISubItemPainter>(@"git.revision.committer.email")
			.SingleInstance();

		builder
			.Register(static c => new CompositeSubItemPainter(
				c.ResolveNamed<ISubItemPainter>(@"git.revision.real.user.author.email"),
				c.ResolveNamed<ISubItemPainter>(@"git.revision.fake.user.email")))
			.Named<ISubItemPainter>(@"git.revision.author.email")
			.SingleInstance();

		builder
			.RegisterType<CommitterEmailColumn>()
			.WithParameter(ResolvedParameter.ForNamed<ISubItemPainter>(@"git.revision.committer.email"))
			.Named<CustomListBoxColumn>(ForRevisionHistory)
			.Named<CustomListBoxColumn>(ForPathRevisionHistory)
			.ExternallyOwned();

		builder
			.RegisterType<AuthorDateColumn>()
			.Named<CustomListBoxColumn>(ForRevisionHistory)
			.Named<CustomListBoxColumn>(ForPathRevisionHistory)
			.ExternallyOwned();

		builder
			.RegisterType<AuthorColumn>()
			.WithParameter(ResolvedParameter.ForNamed<ISubItemPainter>(@"git.revision.author"))
			.Named<CustomListBoxColumn>(ForRevisionHistory)
			.Named<CustomListBoxColumn>(ForPathRevisionHistory)
			.ExternallyOwned();

		builder
			.RegisterType<AuthorEmailColumn>()
			.WithParameter(ResolvedParameter.ForNamed<ISubItemPainter>(@"git.revision.author.email"))
			.Named<CustomListBoxColumn>(ForRevisionHistory)
			.Named<CustomListBoxColumn>(ForPathRevisionHistory)
			.ExternallyOwned();
	}

	private static void RegisterListBox(ContainerBuilder builder, string consumer)
	{
		Assert.IsNotNull(builder);
		Assert.IsNeitherNullNorWhitespace(consumer);

		builder
			.RegisterType<RevisionListBox>()
			.OnActivating(c =>
			{
				var columns = c.Context.ResolveNamed<IEnumerable<CustomListBoxColumn>>(consumer);
				c.Instance.Columns.AddRange(columns);
			})
			.Named<RevisionListBox>(consumer)
			.ExternallyOwned();
	}
}
