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

namespace gitter.Redmine.Gui.ListBoxes;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using gitter.Framework;

using Resources = gitter.Redmine.Properties.Resources;

sealed class NewsListBinding : AsyncDataBinding<List<News>>
{
	#region .ctor

	public NewsListBinding(RedmineServiceContext serviceContext, NewsListBox newsListBox)
	{
		Verify.Argument.IsNotNull(serviceContext);
		Verify.Argument.IsNotNull(newsListBox);

		ServiceContext = serviceContext;
		NewsListBox    = newsListBox;

		Progress = newsListBox.ProgressMonitor;
	}

	#endregion

	#region Properties

	public RedmineServiceContext ServiceContext { get; }

	public NewsListBox NewsListBox { get; }

	#endregion

	#region Methods

	protected override Task<List<News>> FetchDataAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
	{
		Verify.State.IsNotDisposed(IsDisposed, this);

		NewsListBox.Cursor = Cursors.WaitCursor;

		return ServiceContext.News.FetchAsync(ServiceContext.DefaultProjectId,
			progress, cancellationToken);
	}

	protected override void OnFetchCompleted(List<News> newsList)
	{
		Assert.IsNotNull(newsList);

		if(IsDisposed || NewsListBox.IsDisposed)
		{
			return;
		}

		NewsListBox.BeginUpdate();
		NewsListBox.Items.Clear();
		if(newsList.Count != 0)
		{
			foreach(var news in newsList)
			{
				NewsListBox.Items.Add(new NewsListItem(news));
			}
			NewsListBox.Text = string.Empty;
		}
		else
		{
			NewsListBox.Text = Resources.StrsNoNewsToDisplay;
		}
		NewsListBox.EndUpdate();
		NewsListBox.Cursor = Cursors.Default;
	}

	protected override void OnFetchFailed(Exception exception)
	{
		if(IsDisposed || NewsListBox.IsDisposed)
		{
			return;
		}

		NewsListBox.ProgressMonitor.Report(OperationProgress.Completed);
		NewsListBox.Text = Resources.StrsFailedToFetchNews;
		NewsListBox.Items.Clear();
		NewsListBox.Cursor = Cursors.Default;
	}

	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(!NewsListBox.IsDisposed)
			{
				NewsListBox.Text = Resources.StrsNoNewsToDisplay;
				NewsListBox.Items.Clear();
			}
		}
		base.Dispose(disposing);
	}

	#endregion
}
