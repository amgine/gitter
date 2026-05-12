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

namespace gitter.IconPacks.Options;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Options;

[ToolboxItem(false)]
[DesignerCategory("")]
public sealed class IconPackOptionPage : PropertyPage, IExecutableDialog
{
	public static readonly new Guid Guid = new("8B7A3DAD-1F5E-4F9C-B8AE-7E5DD2B4B2A0");

	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(521, 328));

	private readonly Label    _label;
	private readonly ComboBox _combo;
	private readonly Label    _hint;
	private readonly Button   _btnOpenDir;

	public IconPackOptionPage()
		: base(Guid)
	{
		Name = nameof(IconPackOptionPage);
		Text = "Icon Pack";

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);

		_label = new Label
		{
			Text     = "Active icon pack:",
			AutoSize = true,
			Location = new Point(8, 12),
			Parent   = this,
		};
		_combo = new ComboBox
		{
			DropDownStyle = ComboBoxStyle.DropDownList,
			Location      = new Point(8, 32),
			Width         = 380,
			Parent        = this,
		};
		_btnOpenDir = new Button
		{
			Text     = "Open user packs folder…",
			AutoSize = true,
			Location = new Point(8, 64),
			Parent   = this,
		};
		_hint = new Label
		{
			Text =
				"Drop VSCode-format icon-theme directories into the user packs folder " +
				"(each subdirectory must contain icon-theme.json). " +
				"Restart is not required — switching applies immediately.",
			AutoSize = false,
			Size     = new Size(480, 80),
			Location = new Point(8, 100),
			Parent   = this,
		};
		_btnOpenDir.Click += OnOpenDirClick;

		PopulateCombo();

		ResumeLayout(performLayout: false);
		PerformLayout();
	}

	private void PopulateCombo()
	{
		_combo.Items.Clear();
		_combo.Items.Add(new PackEntry(null, "(no pack — use legacy icons)"));
		foreach(var desc in IconPackBootstrap.Available)
		{
			_combo.Items.Add(new PackEntry(desc.Name, $"{desc.Name}  ({desc.Source})"));
		}

		var active = IconPackBootstrap.ActivePackName ?? IconPackSettings.LoadActivePackName();
		var idx = 0;
		for(var i = 0; i < _combo.Items.Count; i++)
		{
			var entry = (PackEntry)_combo.Items[i]!;
			if(string.Equals(entry.Name, active, StringComparison.OrdinalIgnoreCase))
			{
				idx = i;
				break;
			}
		}
		_combo.SelectedIndex = idx;
	}

	private void OnOpenDirClick(object? sender, EventArgs e)
	{
		var dir = IconPackDiscovery.EnsureUserPacksDirectory();
		try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(dir) { UseShellExecute = true }); }
		catch(Exception ex) { IconPackLog.Warn("Open user packs dir failed", ex); }
	}

	public bool Execute()
	{
		if(_combo.SelectedItem is not PackEntry entry) return true;
		IconPackBootstrap.TryActivate(entry.Name);
		IconPackRedraw.RefreshAllForms();
		return true;
	}

	private sealed class PackEntry
	{
		public PackEntry(string? name, string display) { Name = name; Display = display; }
		public string? Name    { get; }
		public string  Display { get; }
		public override string ToString() => Display;
	}
}
