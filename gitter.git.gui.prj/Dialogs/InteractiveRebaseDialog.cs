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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class InteractiveRebaseDialog : GitDialogBase, IExecutableDialog
{
	const string ColAction  = "Action";
	const string ColHash    = "Hash";
	const string ColSubject = "Subject";

	readonly DataGridView _grid;
	readonly Button       _btnMoveUp;
	readonly Button       _btnMoveDown;

	public InteractiveRebaseDialog()
	{
		Text = "Interactive Rebase";

		_grid = new DataGridView
		{
			Dock                       = DockStyle.Fill,
			AllowUserToAddRows         = false,
			AllowUserToDeleteRows      = false,
			AllowUserToResizeRows      = false,
			RowHeadersVisible          = false,
			MultiSelect                = false,
			SelectionMode              = DataGridViewSelectionMode.FullRowSelect,
			AutoSizeColumnsMode        = DataGridViewAutoSizeColumnsMode.Fill,
			EditMode                   = DataGridViewEditMode.EditOnEnter,
		};
		_grid.Columns.Add(BuildActionColumn());
		_grid.Columns.Add(BuildHashColumn());
		_grid.Columns.Add(BuildSubjectColumn());

		_btnMoveUp = new Button
		{
			Text     = "↑ Move Up",
			AutoSize = true,
			AutoSizeMode = AutoSizeMode.GrowAndShrink,
			Padding  = new Padding(8, 0, 8, 0),
			Margin   = new Padding(0, 0, 6, 0),
			Enabled  = false,
		};
		_btnMoveUp.Click += (_, _) => MoveSelected(-1);

		_btnMoveDown = new Button
		{
			Text     = "↓ Move Down",
			AutoSize = true,
			AutoSizeMode = AutoSizeMode.GrowAndShrink,
			Padding  = new Padding(8, 0, 8, 0),
			Margin   = new Padding(0),
			Enabled  = false,
		};
		_btnMoveDown.Click += (_, _) => MoveSelected(+1);

		var toolbar = new FlowLayoutPanel
		{
			Dock          = DockStyle.Top,
			AutoSize      = true,
			AutoSizeMode  = AutoSizeMode.GrowAndShrink,
			FlowDirection = FlowDirection.LeftToRight,
			Padding       = new Padding(4),
		};
		toolbar.Controls.Add(_btnMoveUp);
		toolbar.Controls.Add(_btnMoveDown);

		Controls.Add(_grid);
		Controls.Add(toolbar);

		_grid.SelectionChanged += (_, _) => UpdateButtonState();
	}

	static DataGridViewComboBoxColumn BuildActionColumn()
	{
		var col = new DataGridViewComboBoxColumn
		{
			Name       = ColAction,
			HeaderText = "Action",
			Width      = 80,
			FillWeight = 15,
			FlatStyle  = FlatStyle.Flat,
		};
		col.Items.AddRange(System.Enum.GetNames(typeof(RebaseAction)));
		return col;
	}

	static DataGridViewTextBoxColumn BuildHashColumn() => new()
	{
		Name       = ColHash,
		HeaderText = "Hash",
		Width      = 70,
		FillWeight = 12,
		ReadOnly   = true,
		DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Consolas", 9f) },
	};

	static DataGridViewTextBoxColumn BuildSubjectColumn() => new()
	{
		Name       = ColSubject,
		HeaderText = "Subject / Command",
		FillWeight = 73,
	};

	/// <summary>Entries to display and edit.</summary>
	public IReadOnlyList<RebaseTodoEntry> Entries
	{
		get => SnapshotPlan();
		set
		{
			_grid.Rows.Clear();
			if(value is null) return;
			foreach(var entry in value)
			{
				var index = _grid.Rows.Add(entry.Action.ToString(), ShortHash(entry.Hash), entry.Subject);
				_grid.Rows[index].Tag = entry;
			}
			UpdateButtonState();
		}
	}

	IReadOnlyList<RebaseTodoEntry> _capturedPlan = System.Array.Empty<RebaseTodoEntry>();

	/// <summary>Plan captured during <see cref="Execute"/>; readable after the dialog closes.</summary>
	public IReadOnlyList<RebaseTodoEntry> ResultPlan => _capturedPlan;

	/// <inheritdoc/>
	public bool Execute()
	{
		_grid.EndEdit();
		_capturedPlan = SnapshotPlan();
		return true;
	}

	List<RebaseTodoEntry> SnapshotPlan()
	{
		var result = new List<RebaseTodoEntry>(_grid.Rows.Count);
		foreach(DataGridViewRow row in _grid.Rows)
		{
			var actionStr = row.Cells[ColAction].Value as string ?? nameof(RebaseAction.Pick);
			if(!System.Enum.TryParse<RebaseAction>(actionStr, out var action)) action = RebaseAction.Pick;

			var subject = row.Cells[ColSubject].Value as string ?? string.Empty;

			// Preserve full hash from the original entry stored on Tag
			var hash = (row.Tag is RebaseTodoEntry original) ? original.Hash : string.Empty;

			result.Add(new RebaseTodoEntry(action, hash, subject));
		}
		return result;
	}

	static string ShortHash(string hash) =>
		string.IsNullOrEmpty(hash) ? string.Empty :
		hash.Length <= 7 ? hash : hash[..7];

	void MoveSelected(int delta)
	{
		if(_grid.SelectedRows.Count == 0) return;
		var row   = _grid.SelectedRows[0];
		var index = row.Index;
		var target = index + delta;
		if(target < 0 || target >= _grid.Rows.Count) return;

		_grid.Rows.RemoveAt(index);
		_grid.Rows.Insert(target, row);
		_grid.ClearSelection();
		_grid.Rows[target].Selected = true;
		UpdateButtonState();
	}

	void UpdateButtonState()
	{
		var hasSelection = _grid.SelectedRows.Count > 0;
		var index        = hasSelection ? _grid.SelectedRows[0].Index : -1;
		_btnMoveUp.Enabled   = hasSelection && index > 0;
		_btnMoveDown.Enabled = hasSelection && index >= 0 && index < _grid.Rows.Count - 1;
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(640, 480));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrRebase;
}
