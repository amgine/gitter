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

namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class ConflictsDialog : GitDialogBase
	{
		private Repository _repository;

		public ConflictsDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();

			Text = Resources.StrConflicts;

			_lblConflictingFiles.Text = Resources.StrConflictingFiles.AddColon();

			_lstConflicts.Style = GitterApplication.DefaultStyle;
			_lstConflicts.Columns.ShowAll(column => (ColumnId)column.Id == ColumnId.Name);
			_lstConflicts.Columns[0].SizeMode = Framework.Controls.ColumnSizeMode.Auto;
			_lstConflicts.ShowTreeLines = false;
			_lstConflicts.ItemActivated += OnItemActivated;

			AttachToReposotory();
		}

		private void AttachToReposotory()
		{
			lock(_repository.Status.SyncRoot)
			{
				foreach(var item in _repository.Status.UnstagedFiles)
				{
					if(item.Status == FileStatus.Unmerged)
					{
						_lstConflicts.Items.Add(new WorktreeConflictedFileItem(item, true));
					}
				}
				_repository.Status.NewUnstagedFile += OnNewConflictFound;
				_repository.Status.RemovedUnstagedFile += OnConflictResolved;
			}
		}

		private void DetachFromRepository()
		{
			_repository.Status.NewUnstagedFile -= OnNewConflictFound;
			_repository.Status.RemovedUnstagedFile -= OnConflictResolved;
			_lstConflicts.Items.Clear();
		}

		private void OnConflictResolved(object sender, TreeFileEventArgs e)
		{
			if(e.File.Status == FileStatus.Unmerged)
			{
				_lstConflicts.BeginInvoke(new Action<TreeFile>(RemoveTreeItem), new object[] { e.File });
			}
		}

		private void RemoveTreeItem(TreeFile file)
		{
			foreach(WorktreeConflictedFileItem item in _lstConflicts.Items)
			{
				if(item.DataContext.RelativePath == file.RelativePath)
				{
					item.RemoveSafe();
					break;
				}
			}
			if(_lstConflicts.Items.Count == 0)
			{
				ClickOk();
			}
		}

		private void OnNewConflictFound(object sender, TreeFileEventArgs e)
		{
			if(e.File.Status == FileStatus.Unmerged)
			{
				_lstConflicts.Items.AddSafe(new WorktreeConflictedFileItem(e.File, true));
			}
		}

		private void OnItemActivated(object sender, gitter.Framework.Controls.ItemEventArgs e)
		{
			var item = e.Item as TreeFileListItem;
			if(item != null)
			{
				var file = item.DataContext;
				if(file.Status == FileStatus.Unmerged)
				{
					switch(file.ConflictType)
					{
						case ConflictType.DeletedByThem:
							using(var dlg = new ConflictResolutionDialog(file.RelativePath, FileStatus.Modified, FileStatus.Removed,
								ConflictResolution.KeepModifiedFile, ConflictResolution.DeleteFile))
							{
								try
								{
									file.ResolveConflict(dlg.ConflictResolution);
								}
								catch(GitException exc)
								{
									GitterApplication.MessageBoxService.Show(
										this,
										exc.Message,
										Resources.ErrFailedToResolveConflict,
										MessageBoxButton.Close,
										MessageBoxIcon.Error);
								}
							}
							break;
						case ConflictType.DeletedByUs:
							using(var dlg = new ConflictResolutionDialog(file.RelativePath, FileStatus.Removed, FileStatus.Modified,
								ConflictResolution.KeepModifiedFile, ConflictResolution.DeleteFile))
							{
								if(dlg.Run(this) == DialogResult.OK)
								{
									try
									{
										file.ResolveConflict(dlg.ConflictResolution);
									}
									catch(GitException exc)
									{
										GitterApplication.MessageBoxService.Show(
											this,
											exc.Message,
											Resources.ErrFailedToResolveConflict,
											MessageBoxButton.Close,
											MessageBoxIcon.Error);
									}
								}
							}
							break;
						default:
							try
							{
								ProgressForm.MonitorTaskAsModalWindow(this, Resources.StrRunningMergeTool, file.RunMergeToolAsync);
							}
							catch(GitException exc)
							{
								GitterApplication.MessageBoxService.Show(
									this,
									exc.Message,
									Resources.ErrFailedToRunMergeTool,
									MessageBoxButton.Close,
									MessageBoxIcon.Error);
							}
							break;
					}
				}
			}
		}

		protected override string ActionVerb
		{
			get { return Resources.StrClose; }
		}

		public override DialogButtons OptimalButtons
		{
			get { return DialogButtons.Ok; }
		}
	}
}
