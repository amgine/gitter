#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.Controls;

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

[DesignerCategory("")]
public class FolderPickerTextBoxDecorator(TextBox textBox)
	: TextBoxDecoratorWithButton(textBox)
{
	private FolderBrowserDialog? _dialog;

	/// <inheritdoc/>
	protected override Bitmap? GetIcon(Dpi dpi)
		=> CachedResources.ScaledBitmaps["open.folder.mask",
			DpiConverter.FromDefaultTo(dpi).ConvertX(16)];

	/// <inheritdoc/>
	protected override void OnButtonClick()
	{
		_dialog ??= new();
		_dialog.SelectedPath = Decorated.Text;
		if(_dialog.ShowDialog(this) == DialogResult.OK)
		{
			Decorated.Text = _dialog.SelectedPath;
		}
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			DisposableUtility.Dispose(ref _dialog);
		}
		base.Dispose(disposing);
	}
}
