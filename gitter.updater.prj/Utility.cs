namespace gitter.Updater
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.IO;
	using System.Diagnostics;

	public static class Utility
	{
		public static void EnsureDirectoryDoesNotExist(string path)
		{
			if(Directory.Exists(path))
			{
				RemoveDirectoryRecursively(path);
			}
		}

		public static void EnsureDirectoryExists(string path)
		{
			if(Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		public static void EnsureDirectoryIsEmpty(string path)
		{
			if(Directory.Exists(path))
			{
				RemoveDirectoryRecursively(path);
				Directory.CreateDirectory(path);
			}
		}

		private static void RemoveDirectoryRecursively(string dir)
		{
			var di = new DirectoryInfo(dir);
			try
			{
				DropProtectingFlags(di);
			}
			catch
			{
			}
			Directory.Delete(dir, true);
		}

		private static void DropProtectingFlags(DirectoryInfo di)
		{
			const FileAttributes drop = FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System;

			var dattrs = di.Attributes;
			if((dattrs & drop) != (FileAttributes)0)
			{
				dattrs &= ~drop;
				di.Attributes = dattrs;
			}
			foreach(var file in di.EnumerateFiles())
			{
				var attrs = file.Attributes;
				if((attrs & drop) != (FileAttributes)0)
				{
					attrs &= ~drop;
					file.Attributes = attrs;
				}
			}
			foreach(var d in di.EnumerateDirectories())
			{
				DropProtectingFlags(d);
			}
		}

		public static void CopyDirectoryContent(string source, string target)
		{
			CopyDirectoryContent(new DirectoryInfo(source), target);
		}

		public static void CopyDirectoryContent(DirectoryInfo source, string target)
		{
			EnsureDirectoryExists(target);
			foreach(var file in source.EnumerateFiles())
			{
				file.CopyTo(Path.Combine(target, file.Name), true);
			}
			foreach(var d in source.EnumerateDirectories())
			{
				CopyDirectoryContent(d, Path.Combine(target, d.Name));
			}
		}

		public static void StartApplication(string fileName)
		{
			using(var p = new Process())
			{
				p.StartInfo = new ProcessStartInfo()
				{
					FileName = fileName,
					WorkingDirectory = Path.GetDirectoryName(fileName),
				};
				p.Start();
			}
		}

		public static void KillAllGitterProcesses(string targetDirectory)
		{
			try
			{
				var di = new DirectoryInfo(targetDirectory);
				var processes = Process.GetProcesses();
				foreach(var p in processes)
				{
					bool kill = p.ProcessName == "gitter";
					if(!kill)
					{
						try
						{
							var dir = Path.GetDirectoryName(p.MainModule.FileName);
							var d = new DirectoryInfo(dir);
							while(d != null)
							{
								if(di.FullName == d.FullName)
								{
									kill = true;
									break;
								}
								d = d.Parent;
							}
						}
						catch
						{
						}
					}
					if(kill)
					{
						bool closed = false;
						try
						{
							closed = p.CloseMainWindow();
						}
						catch
						{
						}
						try
						{
							if(!closed || !p.WaitForExit(2000))
							{
								p.Kill();
							}
						}
						catch
						{
						}
					}
				}
			}
			catch
			{
			}
		}
	}
}
