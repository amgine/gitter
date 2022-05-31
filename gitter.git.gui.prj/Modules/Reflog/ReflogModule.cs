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

sealed class ReflogModule : Autofac.Module
{
	private const string ForReflog = @"git.reflog";

	/// <inheritdoc/>
	protected override void Load(ContainerBuilder builder)
	{
		Assert.IsNotNull(builder);

		RegisterColumns(builder);

		builder
			.RegisterType<ReflogListBox>()
			.OnActivating(static c =>
			{
				var columns = c.Context.ResolveNamed<IEnumerable<CustomListBoxColumn>>(ForReflog);
				c.Instance.Columns.AddRange(columns);
			})
			.AsSelf()
			.ExternallyOwned();

		builder
			.RegisterType<ReflogView>()
			.AsSelf()
			.ExternallyOwned();

		builder
			.RegisterType<ReflogViewFactory>()
			.Named<IViewFactory>(@"git")
			.As<GitViewFactoryBase>()
			.SingleInstance();

		base.Load(builder);
	}

	private static void RegisterColumns(ContainerBuilder builder)
	{
		Assert.IsNotNull(builder);

		builder
			.RegisterType<CommitDateColumn>()
			.OnActivating(static c => c.Instance.IsVisible = false)
			.Named<CustomListBoxColumn>(ForReflog)
			.ExternallyOwned();

		builder
			.RegisterType<HashColumn>()
			.OnActivating(static c => c.Instance.IsVisible = true)
			.Named<CustomListBoxColumn>(ForReflog)
			.ExternallyOwned();

		builder
			.RegisterType<TreeHashColumn>()
			.OnActivating(static c => c.Instance.IsVisible = false)
			.Named<CustomListBoxColumn>(ForReflog)
			.ExternallyOwned();

		builder
			.RegisterType<MessageColumn>()
			.OnActivating(static c => c.Instance.IsVisible = true)
			.Named<CustomListBoxColumn>(ForReflog)
			.ExternallyOwned();

		builder
			.RegisterType<ReflogRecordSubjectPainter>()
			.Named<ISubItemPainter>(@"git.reflog.subject")
			.SingleInstance();

		builder
			.RegisterType<SubjectColumn>()
			.WithParameter(ResolvedParameter.ForNamed<ISubItemPainter>(@"git.reflog.subject"))
			.OnActivating(static c => c.Instance.IsVisible = false)
			.Named<CustomListBoxColumn>(ForReflog)
			.ExternallyOwned();

		builder
			.RegisterType<ReflogRecordCommitterPainter>()
			.Named<ISubItemPainter>(@"git.reflog.committer")
			.SingleInstance();

		builder
			.RegisterType<CommitterColumn>()
			.WithParameter(ResolvedParameter.ForNamed<ISubItemPainter>(@"git.reflog.committer"))
			.OnActivating(static c => c.Instance.IsVisible = false)
			.Named<CustomListBoxColumn>(ForReflog)
			.ExternallyOwned();

		builder
			.RegisterType<ReflogRecordCommitterEmailPainter>()
			.Named<ISubItemPainter>(@"git.reflog.committer.email")
			.SingleInstance();

		builder
			.RegisterType<CommitterEmailColumn>()
			.WithParameter(ResolvedParameter.ForNamed<ISubItemPainter>(@"git.reflog.committer.email"))
			.OnActivating(static c => c.Instance.IsVisible = false)
			.Named<CustomListBoxColumn>(ForReflog)
			.ExternallyOwned();

		builder
			.RegisterType<AuthorDateColumn>()
			.OnActivating(static c => c.Instance.IsVisible = false)
			.Named<CustomListBoxColumn>(ForReflog)
			.ExternallyOwned();

		builder
			.RegisterType<ReflogRecordAuthorPainter>()
			.Named<ISubItemPainter>(@"git.reflog.author")
			.SingleInstance();

		builder
			.RegisterType<AuthorColumn>()
			.WithParameter(ResolvedParameter.ForNamed<ISubItemPainter>(@"git.reflog.author"))
			.OnActivating(static c => c.Instance.IsVisible = false)
			.Named<CustomListBoxColumn>(ForReflog)
			.ExternallyOwned();

		builder
			.RegisterType<ReflogRecordAuthorEmailPainter>()
			.Named<ISubItemPainter>(@"git.reflog.author.email")
			.SingleInstance();

		builder
			.RegisterType<AuthorEmailColumn>()
			.WithParameter(ResolvedParameter.ForNamed<ISubItemPainter>(@"git.reflog.author.email"))
			.OnActivating(static c => c.Instance.IsVisible = false)
			.Named<CustomListBoxColumn>(ForReflog)
			.ExternallyOwned();
	}
}
