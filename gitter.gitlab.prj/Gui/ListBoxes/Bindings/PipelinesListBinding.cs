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

	sealed class PipelinesListBinding : AsyncDataBinding<IReadOnlyList<Pipeline>>
	{
		public PipelinesListBinding(GitLabServiceContext serviceContext, CustomListBox pipelinesListBox)
		{
			Verify.Argument.IsNotNull(serviceContext, nameof(serviceContext));
			Verify.Argument.IsNotNull(pipelinesListBox, nameof(pipelinesListBox));

			ServiceContext   = serviceContext;
			PipelinesListBox = pipelinesListBox;

			Progress = pipelinesListBox.ProgressMonitor;
		}

		public GitLabServiceContext ServiceContext { get; }

		public CustomListBox PipelinesListBox { get; }

		protected override Task<IReadOnlyList<Pipeline>> FetchDataAsync(
			IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
		{
			Verify.State.IsFalse(IsDisposed, "PipelinesListBinding is disposed.");

			PipelinesListBox.Cursor = Cursors.WaitCursor;

			return ServiceContext.GetPipelinesAsync();
		}

		protected override void OnFetchCompleted(IReadOnlyList<Pipeline> issues)
		{
			Assert.IsNotNull(issues);

			if(IsDisposed || PipelinesListBox.IsDisposed) return;
			 
			PipelinesListBox.BeginUpdate();
			PipelinesListBox.Items.Clear();
			if(issues.Count != 0)
			{
				foreach(var issue in issues)
				{
					PipelinesListBox.Items.Add(new PipelineListItem(issue));
				}
				PipelinesListBox.Text = string.Empty;
			}
			else
			{
				PipelinesListBox.Text = Resources.StrsNoPipelinesToDisplay;
			}
			PipelinesListBox.EndUpdate();
			PipelinesListBox.Cursor = Cursors.Default;
		}

		protected override void OnFetchFailed(Exception exception)
		{
			if(IsDisposed || PipelinesListBox.IsDisposed)
			{
				return;
			}

			PipelinesListBox.ProgressMonitor.Report(OperationProgress.Completed);
			PipelinesListBox.Text = Resources.StrsFailedToFetchPipelines;
			PipelinesListBox.Items.Clear();
			PipelinesListBox.Cursor = Cursors.Default;
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(!PipelinesListBox.IsDisposed)
				{
					PipelinesListBox.Text = Resources.StrsNoPipelinesToDisplay;
					PipelinesListBox.Items.Clear();
				}
			}
			base.Dispose(disposing);
		}
	}
}
