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

namespace gitter.GitLab.Gui;

using System;
using System.Globalization;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Configuration;
using gitter.Framework.Controls;
using gitter.GitLab.Api;
using gitter.GitLab.Gui.ListBoxes;

using Resources = gitter.GitLab.Properties.Resources;

partial class TestReportView : GitLabViewBase, ISearchableView<TestReportSearchOptions>
{
	private readonly TestCasesListBox _lstTestCases;
	private readonly TestReportToolBar _toolbar;
	private ISearchToolBarController _searchToolbar;
	private TestReportListBinding _dataSource;

	public TestReportView(IWorkingEnvironment environment)
		: base(Guids.TestReportViewGuid, environment)
	{
		SuspendLayout();
		_lstTestCases = new();
		_lstTestCases.Dock = DockStyle.Fill;
		_lstTestCases.BorderStyle = BorderStyle.None;
		_lstTestCases.Name = nameof(_lstTestCases);
		_lstTestCases.TabIndex = 0;
		_lstTestCases.Text = Resources.StrsNoTestsToDisplay;
		_lstTestCases.Parent = this;
		Name = nameof(TestReportView);
		ResumeLayout(false);

		Search = new TestReportSearch(_lstTestCases);

		_searchToolbar = CreateSearchToolbarController<TestReportView, TestReportSearchToolBar, TestReportSearchOptions>(this);

		AddTopToolStrip(_toolbar = new(this));

		_toolbar.Success.CheckedChanged += (_, _) => DataSource?.NotifyFilterUpdated();
		_toolbar.Skipped.CheckedChanged += (_, _) => DataSource?.NotifyFilterUpdated();
		_toolbar.Failed.CheckedChanged  += (_, _) => DataSource?.NotifyFilterUpdated();
		_toolbar.Errors.CheckedChanged  += (_, _) => DataSource?.NotifyFilterUpdated();

		_toolbar.ExpandAll.Click   += (_, _) => _lstTestCases.ExpandAll();
		_toolbar.CollapseAll.Click += (_, _) => _lstTestCases.CollapseAll();

		//_lstTestCases.ItemActivated  += OnItemActivated;
		_lstTestCases.PreviewKeyDown += OnKeyDown;
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			DataSource = null;
		}
		base.Dispose(disposing);
	}

	public override IImageProvider ImageProvider { get; } = CommonIcons.Test;

	private TestReportListBinding DataSource
	{
		get => _dataSource;
		set
		{
			if(_dataSource != value)
			{
				if(_dataSource is not null)
				{
					_dataSource.Dispose();
				}
				_dataSource = value;
				if(_dataSource is not null)
				{
					_dataSource.DataChanged += OnDataSourceDataChanged;
					_dataSource.ReloadData();
				}
			}
		}
	}

	private void OnDataSourceDataChanged(object sender, EventArgs e)
	{
		if(sender is not TestReportListBinding binding) return;
		if(binding != DataSource) return;
		if(IsDisposed || Disposing) return;

		var data = binding.Data;
		if(data is { TotalCount: > 0 })
		{
			_toolbar.Success.Text = data.SuccessCount.ToString(CultureInfo.InvariantCulture);
			_toolbar.Failed.Text  = data.FailedCount.ToString(CultureInfo.InvariantCulture);
			_toolbar.Skipped.Text = data.SkippedCount.ToString(CultureInfo.InvariantCulture);
			_toolbar.Errors.Text  = data.ErrorCount.ToString(CultureInfo.InvariantCulture);
		}
		else
		{
			_toolbar.Success.Text = "0";
			_toolbar.Failed.Text  = "0";
			_toolbar.Skipped.Text = "0";
			_toolbar.Errors.Text  = "0";
		}
	}

	private long? PipelineId { get; set; }

	protected override void OnContextDetached(GitLabServiceContext serviceContext)
	{
		Assert.IsNotNull(serviceContext);

		DataSource = default;
	}

	private bool FilterTestCase(TestCase testCase)
		=> testCase.Status switch
		{
			TestCaseStatus.Success => _toolbar.Success.Checked,
			TestCaseStatus.Skipped => _toolbar.Skipped.Checked,
			TestCaseStatus.Failed  => _toolbar.Failed.Checked,
			TestCaseStatus.Error   => _toolbar.Errors.Checked,
			_ => false,
		};

	protected override void OnContextAttached(GitLabServiceContext serviceContext)
	{
		Assert.IsNotNull(serviceContext);

		DataSource = PipelineId.HasValue
			? new TestReportListBinding(serviceContext, _lstTestCases, PipelineId.Value, FilterTestCase)
			: default;
	}

	protected override void AttachViewModel(object viewModel)
	{
		if(viewModel is not TestReportViewModel vm) return;

		if(ServiceContext is not null && vm.PipelineId != PipelineId)
		{
			PipelineId = vm.PipelineId;
			DataSource = new TestReportListBinding(ServiceContext, _lstTestCases, vm.PipelineId, FilterTestCase);
		}
		Text = "Tests @ pipeline #" + vm.PipelineId;

		base.AttachViewModel(viewModel);
	}

	protected override void SaveMoreViewTo(Section section)
	{
		var listNode = section.GetCreateSection("TestCasesList");
		_lstTestCases.SaveViewTo(listNode);
	}

	protected override void LoadMoreViewFrom(Section section)
	{
		var listNode = section.TryGetSection("TestCasesList");
		if(listNode is not null)
		{
			_lstTestCases.LoadViewFrom(listNode);
		}
	}

	protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
	{
		OnKeyDown(this, e);
		base.OnPreviewKeyDown(e);
	}

	private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		Assert.IsNotNull(e);

		switch(e.KeyCode)
		{
			case Keys.F when e.Modifiers == Keys.Control:
				_searchToolbar.Show();
				e.IsInputKey = true;
				break;
			case Keys.F5:
				RefreshContent();
				break;
		}
	}

	public void RefreshContent() => DataSource?.ReloadData();

	public ISearch<TestReportSearchOptions> Search { get; }

	public bool SearchToolBarVisible
	{
		get => _searchToolbar.IsVisible;
		set => _searchToolbar.IsVisible = value;
	}
}
