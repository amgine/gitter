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

namespace gitter.GitLab.Gui.ListBoxes
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.GitLab.Api;

	using Resources = gitter.GitLab.Properties.Resources;

	sealed class IssuesListBinding : AsyncDataBinding<IReadOnlyList<Issue>>
	{
		public IssuesListBinding(GitLabServiceContext serviceContext, IssuesListBox issuesListBox)
		{
			Verify.Argument.IsNotNull(serviceContext, nameof(serviceContext));
			Verify.Argument.IsNotNull(issuesListBox, nameof(issuesListBox));

			ServiceContext = serviceContext;
			IssuesListBox  = issuesListBox;

			Progress = issuesListBox.ProgressMonitor;
		}

		public GitLabServiceContext ServiceContext { get; }

		public IssuesListBox IssuesListBox { get; }

		protected override Task<IReadOnlyList<Issue>> FetchDataAsync(
			IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
		{
			Verify.State.IsFalse(IsDisposed, "IssuesListBinding is disposed.");

			IssuesListBox.Cursor = Cursors.WaitCursor;

			return ServiceContext.GetIssuesAsync(state: IssueState.Opened);
		}

		protected override void OnFetchCompleted(IReadOnlyList<Issue> issues)
		{
			Assert.IsNotNull(issues);

			if(IsDisposed || IssuesListBox.IsDisposed)
			{
				return;
			}

			IssuesListBox.BeginUpdate();
			IssuesListBox.Items.Clear();
			if(issues.Count != 0)
			{
				foreach(var issue in issues)
				{
					IssuesListBox.Items.Add(new IssueListItem(issue));
				}
				IssuesListBox.Text = string.Empty;
			}
			else
			{
				IssuesListBox.Text = Resources.StrsNoIssuesToDisplay;
			}
			IssuesListBox.EndUpdate();
			IssuesListBox.Cursor = Cursors.Default;
		}

		protected override void OnFetchFailed(Exception exception)
		{
			if(IsDisposed || IssuesListBox.IsDisposed)
			{
				return;
			}

			IssuesListBox.ProgressMonitor.Report(OperationProgress.Completed);
			IssuesListBox.Text = Resources.StrsFailedToFetchIssues;
			IssuesListBox.Items.Clear();
			IssuesListBox.Cursor = Cursors.Default;
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(!IssuesListBox.IsDisposed)
				{
					IssuesListBox.Text = Resources.StrsNoIssuesToDisplay;
					IssuesListBox.Items.Clear();
				}
			}
			base.Dispose(disposing);
		}
	}
}
