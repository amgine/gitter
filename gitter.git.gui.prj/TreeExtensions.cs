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
		public static string ExtractBlobToTemporaryFile(this Tree tree, string blobPath)
		{
			if(tree == null) throw new ArgumentNullException("tree");
			if(blobPath == null) throw new ArgumentNullException("blobPath");

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
