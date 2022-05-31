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

namespace gitter.GitLab.Gui.ListBoxes;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.GitLab.Api;

using Resources = gitter.GitLab.Properties.Resources;

internal class TestReportListBinding : AsyncDataBinding<TestReport>
{
	public TestReportListBinding(GitLabServiceContext serviceContext, CustomListBox listBox, long pipelineId, Predicate<TestCase> filter)
	{
		Verify.Argument.IsNotNull(serviceContext);
		Verify.Argument.IsNotNull(listBox);

		ServiceContext = serviceContext;
		ListBox        = listBox;
		PipelineId     = pipelineId;
		Filter         = filter;
	}

	private GitLabServiceContext ServiceContext { get; }

	private CustomListBox ListBox { get; }

	public long PipelineId { get; }

	private Predicate<TestCase> Filter { get; }

	public void NotifyFilterUpdated()
	{
		if(IsDisposed || ListBox.IsDisposed)
		{
			return;
		}

		UpdateListBox(Data);
	}

	protected override Task<TestReport> FetchDataAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
	{
		Verify.State.IsFalse(IsDisposed, "TestReportListBinding is disposed.");

		progress?.Report(new($"Fetching test report..."));
		ListBox.Cursor = Cursors.WaitCursor;

		return ServiceContext.GetTestReportAsync(PipelineId, cancellationToken);
	}

	private void UpdateListBox(TestReport data)
	{
		if(data is { TotalCount: > 0 })
		{
			ListBox.BeginUpdate();
			ListBox.Items.Clear();
			if(data.TestSuites.Length == 1)
			{
				AddSuite(ListBox.Items, data.TestSuites[0]);
			}
			else
			{
				foreach(var testSuite in data.TestSuites)
				{
					if(testSuite is null) continue;

					var testSuiteItem = new TestSuiteListBoxItem(testSuite) { IsExpanded = true };
					AddSuite(testSuiteItem.Items, testSuite);
					if(testSuiteItem.Items.Count > 0)
					{
						ListBox.Items.Add(testSuiteItem);
					}
				}
			}
			ListBox.Text = string.Empty;
			ListBox.EndUpdate();
		}
		else
		{
			ListBox.Items.Clear();
			ListBox.Text = Resources.StrsNoTestsToDisplay;
		}
	}

	private void AddSuite(CustomListBoxItemsCollection rootItems, TestSuite testSuite)
	{
		var classes = new Dictionary<string, TestCaseClassListBoxItem>();
		foreach(var testCase in testSuite.TsstCases)
		{
			if(testCase is null) continue;
			if(Filter is not null && !Filter(testCase)) continue;

			var item      = new TestCaseListBoxItem(testCase);
			var items     = rootItems;
			var className = testCase.ClassName;
			if(!string.IsNullOrWhiteSpace(className))
			{
				if(!classes.TryGetValue(className, out var parent))
				{
					classes.Add(className, parent = new(className) { IsExpanded = true });
					items.Add(parent);
				}
				items = parent.Items;
			}
			items.Add(item);
		}
	}

	protected override void OnFetchCompleted(TestReport data)
	{
		if(IsDisposed || ListBox.IsDisposed)
		{
			return;
		}

		UpdateListBox(data);
		ListBox.Cursor = Cursors.Default;
	}

	protected override void OnFetchFailed(Exception exception)
	{
		if(IsDisposed || ListBox.IsDisposed)
		{
			return;
		}

		ListBox.ProgressMonitor.Report(OperationProgress.Completed);
		ListBox.Text = Resources.StrsFailedToFetchTestReport;
		ListBox.Items.Clear();
		ListBox.Cursor = Cursors.Default;
	}
}
