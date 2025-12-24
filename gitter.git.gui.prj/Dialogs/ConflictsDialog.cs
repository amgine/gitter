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

namespace gitter.Git.Gui.Dialogs;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Services;
using gitter.Git.Gui.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class ConflictsDialog : GitDialogBase
{
	readonly struct DialogControls
	{
		public  readonly TreeListBox  _lstConflicts;
		private readonly LabelControl _lblConflictingFiles;

		public DialogControls(IGitterStyle? style)
		{
			style ??= GitterApplication.Style;

			_lstConflicts = new()
			{
				Style       = style,
				HeaderStyle = HeaderStyle.Hidden,
			};
			_lstConflicts.Columns.ShowAll(static column => (ColumnId)column.Id == ColumnId.Name);
			_lstConflicts.Columns[0].SizeMode = ColumnSizeMode.Auto;
			_lstConflicts.ShowTreeLines = false;
			_lblConflictingFiles = new();
		}

		public void Localize()
		{
			_lblConflictingFiles.Text = Resources.StrConflictingFiles.AddColon();
		}

		public void Layout(Control parent)
		{
			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					rows:
					[
						LayoutConstants.LabelRowHeight,
						LayoutConstants.LabelRowSpacing,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblConflictingFiles, marginOverride: LayoutConstants.NoMargin), row: 0),
						new GridContent(new ControlContent(_lstConflicts,        marginOverride: LayoutConstants.NoMargin), row: 2),
					]),
			};

			_lblConflictingFiles.Parent = parent;
			_lstConflicts.Parent = parent;
		}
	}

	private readonly DialogControls _controls;
	private Repository _repository;

	public ConflictsDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		SuspendLayout();
		AutoScaleMode = AutoScaleMode.Dpi;
		AutoScaleDimensions = Dpi.Default;
		Size = ScalableSize.GetValue(Dpi.Default);
		Name = nameof(ConflictsDialog);
		_repository = repository;
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		Text = Resources.StrConflicts;

		_controls._lstConflicts.ItemActivated += OnItemActivated;

		AttachToReposotory();
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(_repository is not null)
		{
			DetachFromRepository();
			_repository = null!;
		}
		base.Dispose(disposing);
	}

	private void AttachToReposotory()
	{
		lock(_repository.Status.SyncRoot)
		{
			foreach(var item in _repository.Status.UnstagedFiles)
			{
				if(item.Status == FileStatus.Unmerged)
				{
					_controls._lstConflicts.Items.Add(new WorktreeConflictedFileItem(item, true));
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
		_controls._lstConflicts.Items.Clear();
	}

	private void OnConflictResolved(object? sender, TreeFileEventArgs e)
	{
		if(e.File.Status == FileStatus.Unmerged)
		{
			_controls._lstConflicts.BeginInvoke(new Action<TreeFile>(RemoveTreeItem), [e.File]);
		}
	}

	private void RemoveTreeItem(TreeFile file)
	{
		foreach(WorktreeConflictedFileItem item in _controls._lstConflicts.Items)
		{
			if(item.DataContext.RelativePath == file.RelativePath)
			{
				item.RemoveSafe();
				break;
			}
		}
		if(_controls._lstConflicts.Items.Count == 0)
		{
			ClickOk();
		}
	}

	private void OnNewConflictFound(object? sender, TreeFileEventArgs e)
	{
		if(e.File.Status == FileStatus.Unmerged)
		{
			_controls._lstConflicts.Items.AddSafe(new WorktreeConflictedFileItem(e.File, true));
		}
	}

	private void OnItemActivated(object? sender, ItemEventArgs e)
	{
		if(e.Item is TreeFileListItem item)
		{
			var file = item.DataContext;
			if(file.Status == FileStatus.Unmerged)
			{
				switch(file.ConflictType)
				{
					case ConflictType.DeletedByThem:
						using(var dialog = new ConflictResolutionDialog(file.RelativePath, FileStatus.Modified, FileStatus.Removed,
							ConflictResolution.KeepModifiedFile, ConflictResolution.DeleteFile))
						{
							try
							{
								file.ResolveConflict(dialog.ConflictResolution);
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
						using(var dialog = new ConflictResolutionDialog(file.RelativePath, FileStatus.Removed, FileStatus.Modified,
							ConflictResolution.KeepModifiedFile, ConflictResolution.DeleteFile))
						{
							if(dialog.Run(this) == DialogResult.OK)
							{
								try
								{
									file.ResolveConflict(dialog.ConflictResolution);
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
							ProgressForm.MonitorTaskAsModalWindow(this, Resources.StrRunningMergeTool,
								(p, c) => file.RunMergeToolAsync(progress: p, cancellationToken: c));
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

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(400, 325));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrClose;

	/// <inheritdoc/>
	public override DialogButtons OptimalButtons => DialogButtons.Ok;
}
