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

using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.GitLab.Api;

sealed class TestCaseListBoxItem : CustomListBoxItem<TestCase>
{
	public TestCaseListBoxItem(TestCase testCase) : base(testCase)
	{
	}

	private static IImageProvider GetIcon(TestCaseStatus status)
		=> status switch
		{
			TestCaseStatus.Success => Icons.TestSuccess,
			TestCaseStatus.Skipped => Icons.TestSkipped,
			TestCaseStatus.Failed  => Icons.TestFailed,
			TestCaseStatus.Error   => Icons.TestError,
			_ => default,
		};

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		switch(paintEventArgs.Column.Id)
		{
			case 0:
				var icon  = GetIcon(DataContext.Status);
				var image = icon.GetImage(16 * paintEventArgs.Dpi.X / 96);
				paintEventArgs.PaintImageAndText(image, DataContext.Name);
				break;
			default:
				base.OnPaintSubItem(paintEventArgs);
				break;
		}
	}

	/// <inheritdoc/>
	public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		Assert.IsNotNull(requestEventArgs);

		var menu = new ContextMenuStrip();
		var dpiBindings = new DpiBindings(menu);

		var copyName = new ToolStripMenuItem("Name", null, (_, _) => ClipboardEx.TrySetTextSafe(DataContext.Name))
		{
			ToolTipText = DataContext.Name,
		};
		dpiBindings.BindImage(copyName, CommonIcons.ClipboardCopy);
		menu.Items.Add(copyName);
		var copyClassName = new ToolStripMenuItem("Class Name", null, (_, _) => ClipboardEx.TrySetTextSafe(DataContext.ClassName))
		{
			ToolTipText = DataContext.ClassName,
		};
		dpiBindings.BindImage(copyClassName, CommonIcons.ClipboardCopy);
		menu.Items.Add(copyClassName);
		if(!string.IsNullOrWhiteSpace(DataContext.SystemOutput))
		{
			var copySystemOutput = new ToolStripMenuItem("SystemOutput", null, (_, _) => ClipboardEx.TrySetTextSafe(DataContext.SystemOutput));
			dpiBindings.BindImage(copySystemOutput, CommonIcons.ClipboardCopy);
			menu.Items.Add(copySystemOutput);
		}

		Utility.MarkDropDownForAutoDispose(menu);
		return menu;
	}
}
