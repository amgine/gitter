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

namespace gitter.Redmine.Gui.ListBoxes
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	sealed class IssuesListBinding : AsyncDataBinding<LinkedList<Issue>>
	{
		#region Data

		private readonly RedmineServiceContext _serviceContext;
		private readonly IssuesListBox _issuesListBox;

		#endregion

		#region .ctor

		public IssuesListBinding(RedmineServiceContext serviceContext, IssuesListBox issuesListBox)
		{
			Verify.Argument.IsNotNull(serviceContext, "serviceContext");
			Verify.Argument.IsNotNull(issuesListBox, "issuesListBox");

			_serviceContext = serviceContext;
			_issuesListBox  = issuesListBox;

			Progress = issuesListBox.ProgressMonitor;
		}

		#endregion

		#region Properties

		public RedmineServiceContext ServiceContext
		{
			get { return _serviceContext; }
		}

		public IssuesListBox IssuesListBox
		{
			get { return _issuesListBox; }
		}

		#endregion

		#region Methods

		protected override Task<LinkedList<Issue>> FetchDataAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.State.IsFalse(IsDisposed, "IssuesListBinding is disposed.");

			IssuesListBox.Cursor = Cursors.WaitCursor;

			return ServiceContext.Issues.FetchOpenAsync(ServiceContext.DefaultProjectId,
				progress, cancellationToken);
		}

		protected override void OnFetchCompleted(LinkedList<Issue> issues)
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
				var cf = new Dictionary<int, CustomListBoxColumn>();
				foreach(var column in IssuesListBox.Columns)
				{
					if(column.Id >= (int)ColumnId.CustomFieldOffset)
					{
						var id = column.Id - (int)ColumnId.CustomFieldOffset;
						cf.Add(id, column);
					}
				}
				foreach(var issue in issues)
				{
					foreach(var cfv in issue.CustomFields)
					{
						if(!cf.ContainsKey(cfv.Field.Id))
						{
							var column = new IssueCustomFieldColumn(cfv.Field);
							cf.Add(cfv.Field.Id, column);
							IssuesListBox.Columns.Add(column);
						}
					}
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

		#endregion
	}
}
