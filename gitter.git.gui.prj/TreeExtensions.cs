namespace gitter.Git.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.IO;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;

	using Resources = gitter.Git.Gui.Properties.Resources;

	static class TreeExtensions
	{
		public static string ExtractBlobToFile(this Tree tree, string blobPath)
		{
			Verify.Argument.IsNotNull(tree, "tree");
			Verify.Argument.IsNeitherNullNorWhitespace(blobPath, "blobPath");
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
			if(ExtractBlobToFile(tree, blobPath, fileName))
			{
				return fileName;
			}
			else
			{
				return null;
			}
		}

		public static bool ExtractBlobToFile(this Tree tree, string blobPath, string fileName)
		{
			Verify.Argument.IsNotNull(tree, "tree");
			Verify.Argument.IsNeitherNullNorWhitespace(blobPath, "blobPath");
			Verify.Argument.IsNeitherNullNorWhitespace(fileName, "fileName");

			byte[] bytes = null;
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
				using(var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					fs.Write(bytes, 0, bytes.Length);
				}
			}
			catch(Exception exc)
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
			Verify.Argument.IsNotNull(tree, "tree");
			Verify.Argument.IsNeitherNullNorWhitespace(blobPath, "blobPath");

			var path = Path.Combine(Path.GetTempPath(), "gitter", tree.TreeHash);
			var fileName = Path.Combine(path, blobPath);
			byte[] bytes = null;
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
			if(bytes != null)
			{
				path = Path.GetDirectoryName(fileName);
				if(!Directory.Exists(path)) Directory.CreateDirectory(path);
				using(var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					fs.Write(bytes, 0, bytes.Length);
					fs.Close();
				}
			}
			return fileName;
		}
	}
}
