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

	sealed class VersionsListBinding : AsyncDataBinding<LinkedList<ProjectVersion>>
	{
		#region Data

		private readonly RedmineServiceContext _serviceContext;
		private readonly VersionsListBox _versionsListBox;

		#endregion

		#region .ctor

		public VersionsListBinding(RedmineServiceContext serviceContext, VersionsListBox versionsListBox)
		{
			Verify.Argument.IsNotNull(serviceContext, "serviceContext");
			Verify.Argument.IsNotNull(versionsListBox, "versionsListBox");

			_serviceContext  = serviceContext;
			_versionsListBox = versionsListBox;

			Progress = versionsListBox.ProgressMonitor;
		}

		#endregion

		#region Properties

		public RedmineServiceContext ServiceContext
		{
			get { return _serviceContext; }
		}

		public VersionsListBox VersionsListBox
		{
			get { return _versionsListBox; }
		}

		#endregion

		#region Methods

		protected override Task<LinkedList<ProjectVersion>> FetchDataAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.State.IsFalse(IsDisposed, "VersionsListBinding is disposed.");

			VersionsListBox.Cursor = Cursors.WaitCursor;

			return ServiceContext.ProjectVersions.FetchAsync(ServiceContext.DefaultProjectId,
				progress, cancellationToken);
		}

		protected override void OnFetchCompleted(LinkedList<ProjectVersion> versions)
		{
			Assert.IsNotNull(versions);

			if(IsDisposed || VersionsListBox.IsDisposed)
			{
				return;
			}

			VersionsListBox.BeginUpdate();
			VersionsListBox.Items.Clear();
			if(versions.Count != 0)
			{
				foreach(var version in versions)
				{
					VersionsListBox.Items.Add(new VersionListItem(version));
				}
				VersionsListBox.Text = string.Empty;
			}
			else
			{
				VersionsListBox.Text = Resources.StrsNoVersionsToDisplay;
			}
			VersionsListBox.EndUpdate();
			VersionsListBox.Cursor = Cursors.Default;
		}

		protected override void OnFetchFailed(Exception exception)
		{
			if(IsDisposed || VersionsListBox.IsDisposed)
			{
				return;
			}

			VersionsListBox.ProgressMonitor.Report(OperationProgress.Completed);
			VersionsListBox.Text = Resources.StrsFailedToFetchVersions;
			VersionsListBox.Items.Clear();
			VersionsListBox.Cursor = Cursors.Default;
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(!VersionsListBox.IsDisposed)
				{
					VersionsListBox.Text = Resources.StrsNoVersionsToDisplay;
					VersionsListBox.Items.Clear();
				}
			}
			base.Dispose(disposing);
		}

		#endregion
	}
}
