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
using Autofac.Core;

using gitter.Framework.Controls;
using gitter.Git.Gui.Controls;
using gitter.Git.Gui.Views;

sealed class StashModule : Autofac.Module
{
	const string ForStash = @"git.stash";

	/// <inheritdoc/>
	protected override void Load(ContainerBuilder builder)
	{
		Assert.IsNotNull(builder);

		RegisterColumns(builder);

		builder
			.RegisterType<StashListBox>()
			.OnActivating(static c =>
			{
				var columns = c.Context.ResolveNamed<IEnumerable<CustomListBoxColumn>>(ForStash);
				c.Instance.Columns.AddRange(columns);
			})
			.AsSelf()
			.ExternallyOwned();

		builder
			.RegisterType<StashView>()
			.AsSelf()
			.ExternallyOwned();

		builder
			.RegisterType<StashViewFactory>()
			.Named<IViewFactory>(@"git")
			.As<GitViewFactoryBase>()
			.SingleInstance();

		base.Load(builder);
	}

	private static void RegisterColumns(ContainerBuilder builder)
	{
		Assert.IsNotNull(builder);

		builder
			.RegisterType<HashColumn>()
			.Named<CustomListBoxColumn>(ForStash)
			.ExternallyOwned();

		builder
			.RegisterType<CommitDateColumn>()
			.Named<CustomListBoxColumn>(ForStash)
			.ExternallyOwned();

		builder
			.RegisterType<StashedStateSubjectPainter>()
			.Named<ISubItemPainter>(@"git.stash.subject")
			.SingleInstance();

		builder
			.RegisterType<SubjectColumn>()
			.WithParameter(ResolvedParameter.ForNamed<ISubItemPainter>(@"git.stash.subject"))
			.WithParameter(new NamedParameter(@"enableExtender", false))
			.OnActivating(c =>
			{
				var column = c.Instance;
				column.AlignToGraph       = false;
				column.ShowLocalBranches  = false;
				column.ShowRemoteBranches = false;
				column.ShowTags           = false;
				column.ShowStash          = false;
			})
			.Named<CustomListBoxColumn>(ForStash)
			.ExternallyOwned();

		builder
			.RegisterType<StashedStateCommitterPainter>()
			.Named<ISubItemPainter>(@"git.stash.committer")
			.SingleInstance();

		builder
			.RegisterType<CommitterColumn>()
			.WithParameter(ResolvedParameter.ForNamed<ISubItemPainter>(@"git.stash.committer"))
			.Named<CustomListBoxColumn>(ForStash)
			.ExternallyOwned();

		builder
			.RegisterType<StashedStateCommitterEmailPainter>()
			.Named<ISubItemPainter>(@"git.stash.committer.email")
			.SingleInstance();

		builder
			.RegisterType<CommitterEmailColumn>()
			.WithParameter(ResolvedParameter.ForNamed<ISubItemPainter>(@"git.stash.committer.email"))
			.Named<CustomListBoxColumn>(ForStash)
			.ExternallyOwned();

		builder
			.RegisterType<AuthorDateColumn>()
			.Named<CustomListBoxColumn>(ForStash)
			.ExternallyOwned();

		builder
			.RegisterType<StashedStateAuthorPainter>()
			.Named<ISubItemPainter>(@"git.stash.author")
			.SingleInstance();

		builder
			.RegisterType<AuthorColumn>()
			.WithParameter(ResolvedParameter.ForNamed<ISubItemPainter>(@"git.stash.author"))
			.Named<CustomListBoxColumn>(ForStash)
			.ExternallyOwned();

		builder
			.RegisterType<StashedStateAuthorPainter>()
			.Named<ISubItemPainter>(@"git.stash.author.email")
			.SingleInstance();

		builder
			.RegisterType<AuthorEmailColumn>()
			.WithParameter(ResolvedParameter.ForNamed<ISubItemPainter>(@"git.stash.author.email"))
			.Named<CustomListBoxColumn>(ForStash)
			.ExternallyOwned();
	}
}
