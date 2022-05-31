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

namespace gitter.Git.Gui;

using System;
using System.IO;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Services;

using Resources = gitter.Git.Gui.Properties.Resources;

static class TreeExtensions
{
	public static string ExtractBlobToFile(this Tree tree, string blobPath)
	{
		Verify.Argument.IsNotNull(tree);
		Verify.Argument.IsNeitherNullNorWhitespace(blobPath);

		string fileName = null;
		using(var dlg = new SaveFileDialog()
		{
			FileName = Path.GetFileName(blobPath),
		})
		{
			if(dlg.ShowDialog() != DialogResult.OK)
			{
				return null;
			}
			fileName = dlg.FileName;
		}
		return ExtractBlobToFile(tree, blobPath, fileName)
			? fileName
			: default;
	}

	public static bool ExtractBlobToFile(this Tree tree, string blobPath, string fileName)
	{
		Verify.Argument.IsNotNull(tree);
		Verify.Argument.IsNeitherNullNorWhitespace(blobPath);
		Verify.Argument.IsNeitherNullNorWhitespace(fileName);

		byte[] bytes;
		try
		{
			bytes = tree.GetBlobContent(blobPath);
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				null,
				exc.Message,
				Resources.ErrfFailedToQueryBlob.UseAsFormat(blobPath),
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			return false;
		}
		try
		{
			using var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
			fs.Write(bytes, 0, bytes.Length);
		}
		catch(Exception exc) when(!exc.IsCritical())
		{
			GitterApplication.MessageBoxService.Show(
				null,
				exc.Message,
				Resources.ErrfFailedToSaveFile.UseAsFormat(fileName),
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			return false;
		}
		return true;
	}

	public static string ExtractBlobToTemporaryFile(this Tree tree, string blobPath)
	{
		Verify.Argument.IsNotNull(tree);
		Verify.Argument.IsNeitherNullNorWhitespace(blobPath);

		var path = Path.Combine(Path.GetTempPath(), "gitter", tree.TreeHash);
		var fileName = Path.Combine(path, blobPath);
		byte[] bytes;
		try
		{
			bytes = tree.GetBlobContent(blobPath);
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				null,
				exc.Message,
				Resources.ErrfFailedToQueryBlob.UseAsFormat(blobPath),
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			return null;
		}
		if(bytes is not null)
		{
			path = Path.GetDirectoryName(fileName);
			if(!Directory.Exists(path)) Directory.CreateDirectory(path);
			using var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
			fs.Write(bytes, 0, bytes.Length);
		}
		return fileName;
	}
}
