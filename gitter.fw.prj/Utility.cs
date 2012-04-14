namespace gitter.Framework
{
	using System;
	using System.Runtime.InteropServices;
	using System.Collections.Generic;
	using System.IO;
	using System.Threading;
	using System.Diagnostics;
	using System.Reflection;
	using System.Drawing;
	using System.Drawing.Text;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;
	using System.Runtime.Remoting;
	using System.Runtime.Remoting.Channels;
	using System.Runtime.Remoting.Channels.Ipc;
	using System.Security.Principal;

	using Microsoft.Win32;

	public static class Utility
	{
		private static readonly Version _osVersion = Environment.OSVersion.Version;

		private static readonly Image _dummyImage = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		private static readonly Graphics _measurementGraphics = Graphics.FromImage(_dummyImage);

		public static Graphics MeasurementGraphics
		{
			get { return _measurementGraphics; }
		}

		public static bool IsOSVistaOrNewer
		{
			get { return _osVersion >= new Version(6, 0); }
		}

		public static bool IsOSWindows7OrNewer
		{
			get { return _osVersion >= new Version(6, 1); }
		}

		private static readonly string[] SizePostfix = new[]
		{
			"b ", "Kb", "Mb", "Gb", "Tb", "Pb"
		};

		public static string FormatSize(long size)
		{
			double s = size;
			var sizeId = 0;
			while(s > 1024)
			{
				s /= 1024;
				++sizeId;
			}
			if(sizeId >= SizePostfix.Length) return size.ToString();
			return ((int)(s + .5)).ToString() + " " + SizePostfix[sizeId];
		}

		public static Bitmap QueryIcon(string fileName)
		{
			return IconCache.GetIcon(fileName);
		}

		public static string GetFileType(string fileName, bool dir, bool useExtensionOnly)
		{
			const int SHGFI_USEFILEATTRIBUTES = 0x10;
			const int SHGFI_TYPENAME = 0x400;

			const int FILE_ATTRIBUTE_NORMAL = 0x80;
			const int FILE_ATTRIBUTE_DIR = 0x10;

			var attr = dir?FILE_ATTRIBUTE_DIR|FILE_ATTRIBUTE_NORMAL:FILE_ATTRIBUTE_NORMAL;
			var req = useExtensionOnly?SHGFI_USEFILEATTRIBUTES|SHGFI_TYPENAME:SHGFI_TYPENAME;

			var info = new NativeMethods.SHFILEINFO();
			var ret = NativeMethods.SHGetFileInfo(fileName, attr, ref info, System.Runtime.InteropServices.Marshal.SizeOf(info), req);
			return info.szTypeName;
		}

		public static Icon ExtractAssociatedFileIcon16ByExt(string fileName)
		{
			const int SHGFI_ICON = 0x100;
			const int SHGFI_SMALLICON = 0x1;
			const int SHGFI_USEFILEATTRIBUTES = 0x10;

			const int FILE_ATTRIBUTE_NORMAL = 0x80;

			var info = new NativeMethods.SHFILEINFO();
			NativeMethods.SHGetFileInfo(fileName, FILE_ATTRIBUTE_NORMAL, ref info, System.Runtime.InteropServices.Marshal.SizeOf(info), SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES);
			try
			{
				return Icon.FromHandle(info.hIcon);
			}
			catch
			{
				return null;
			}
		}

		public static Icon ExtractAssociatedFileIcon16(string fileName)
		{
			const int SHGFI_ICON = 0x100;
			const int SHGFI_SMALLICON = 0x1;
			const int SHGFI_USEFILEATTRIBUTES = 0x10;

			const int FILE_ATTRIBUTE_NORMAL = 0x80;

			var info = new NativeMethods.SHFILEINFO();
			NativeMethods.SHGetFileInfo(fileName, FILE_ATTRIBUTE_NORMAL, ref info, System.Runtime.InteropServices.Marshal.SizeOf(info), SHGFI_ICON | SHGFI_SMALLICON);
			try
			{
				return Icon.FromHandle(info.hIcon);
			}
			catch
			{
				NativeMethods.SHGetFileInfo(fileName, FILE_ATTRIBUTE_NORMAL, ref info, System.Runtime.InteropServices.Marshal.SizeOf(info), SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES);
				try
				{
					return Icon.FromHandle(info.hIcon);
				}
				catch
				{
					return null;
				}
			}
		}

		public static Icon ExtractAssociatedFolderIcon16(string fileName)
		{
			const int SHGFI_ICON = 0x100;
			const int SHGFI_SMALLICON = 0x1;
			const int SHGFI_USEFILEATTRIBUTES = 0x10;

			const int FILE_ATTRIBUTE_NORMAL = 0x80;
			const int FILE_ATTRIBUTE_DIR = 0x10;

			var info = new NativeMethods.SHFILEINFO();
			NativeMethods.SHGetFileInfo(fileName, FILE_ATTRIBUTE_NORMAL | FILE_ATTRIBUTE_DIR, ref info, System.Runtime.InteropServices.Marshal.SizeOf(info), SHGFI_ICON | SHGFI_SMALLICON);
			try
			{
				return Icon.FromHandle(info.hIcon);
			}
			catch
			{
				NativeMethods.SHGetFileInfo(fileName, FILE_ATTRIBUTE_NORMAL | FILE_ATTRIBUTE_DIR, ref info, System.Runtime.InteropServices.Marshal.SizeOf(info), SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES);
				try
				{
					return Icon.FromHandle(info.hIcon);
				}
				catch
				{
					return null;
				}
			}
		}

		public static Icon ExtractAssociatedFolderIcon16ByType(string fileName)
		{
			const int SHGFI_ICON = 0x100;
			const int SHGFI_SMALLICON = 0x1;
			const int SHGFI_USEFILEATTRIBUTES = 0x10;

			const int FILE_ATTRIBUTE_NORMAL = 0x80;
			const int FILE_ATTRIBUTE_DIR = 0x10;

			var info = new NativeMethods.SHFILEINFO();
			NativeMethods.SHGetFileInfo(fileName, FILE_ATTRIBUTE_NORMAL | FILE_ATTRIBUTE_DIR, ref info, System.Runtime.InteropServices.Marshal.SizeOf(info), SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES);
			try
			{
				return Icon.FromHandle(info.hIcon);
			}
			catch
			{
				return null;
			}
		}

		public static Icon ExtractAssociatedIcon16_(string fileName)
		{
			var pos1 = fileName.LastIndexOf(Path.DirectorySeparatorChar);
			var pos2 = fileName.LastIndexOf('.');
			string ext;
			if(pos2 > pos1)
			{
				ext = fileName.Substring(pos2);
			}
			else
			{
				ext = fileName.Substring(pos1 + 1);
			}
			if(ext.Equals(".ico", StringComparison.InvariantCultureIgnoreCase))
				return new Icon(fileName, 16, 16);
			try
			{
				using(var key = Registry.ClassesRoot.OpenSubKey(ext))
				{
					var alias = (string)key.GetValue(null);
					key.Close();
					using(var aliasKey = Registry.ClassesRoot.OpenSubKey(alias + @"\DefaultIcon"))
					{
						var desc = (string)aliasKey.GetValue(null);
						var file = desc;
						var id = 0;
						var pos = desc.LastIndexOf(',');
						if(pos != -1)
						{
							if(int.TryParse(desc.Substring(pos + 1), out id))
							{
								file = desc.Substring(0, pos);
							}
						}
						aliasKey.Close();
						if(file == "%1") file = fileName;
						IntPtr[] icons = new IntPtr[1];
						var c = NativeMethods.ExtractIconEx(file, id, null, icons, 1);
						if(c == 1)
						{
							return Icon.FromHandle(icons[0]);
						}
						else
						{
							return null;
						}
					}
				}
			}
			catch
			{
				return null;
			}
		}

		public static Control GetParentControl(ToolStripItem item)
		{
			if(item == null) throw new ArgumentNullException("item");
			var cms = item.Owner as ContextMenuStrip;
			if(cms != null)
			{
				return cms.SourceControl;
			}
			var tsdd = item.Owner as ToolStripDropDown;
			if(tsdd != null)
			{
				return tsdd.Parent;
			}
			return item.Owner;
		}

		private static readonly LinkedList<IDisposable> LazyDisposables = new LinkedList<IDisposable>();

		public static void MarkDropDownForAutoDispose(ToolStripDropDown menu)
		{
			if(menu == null) throw new ArgumentNullException("menu");
			menu.Closed += (sender, e) => DisposeOnIdle((IDisposable)sender);
		}

		private static void DisposeOnIdle(IDisposable obj)
		{
			LazyDisposables.AddLast(obj);
			if(LazyDisposables.Count == 1)
			{
				Application.Idle += DisposeRegisteredObjects;
			}
		}

		private static void DisposeRegisteredObjects(object sender, EventArgs e)
		{
			foreach(var obj in LazyDisposables)
			{
				obj.Dispose();
			}
			LazyDisposables.Clear();
			Application.Idle -= DisposeRegisteredObjects;
		}

		public static readonly StringFormat DefaultStringFormat =
			new StringFormat(StringFormat.GenericTypographic)
			{
				FormatFlags =
					StringFormatFlags.LineLimit |
					StringFormatFlags.DisplayFormatControl |
					StringFormatFlags.MeasureTrailingSpaces |
					StringFormatFlags.FitBlackBox |
					StringFormatFlags.NoWrap,
				HotkeyPrefix = HotkeyPrefix.None,
				LineAlignment = StringAlignment.Near,
				Trimming = StringTrimming.None,
			};

		public static readonly StringFormat DefaultStringFormatLeftAlign =
			new StringFormat(DefaultStringFormat)
			{
				Alignment = StringAlignment.Near,
			};

		public static readonly StringFormat DefaultStringFormatRightAlign =
			new StringFormat(DefaultStringFormat)
			{
				Alignment = StringAlignment.Far,
			};

		public static readonly StringFormat DefaultStringFormatCenterAlign =
			new StringFormat(DefaultStringFormat)
			{
				Alignment = StringAlignment.Center,
			};

		public const TextRenderingHint TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

		public const int TextContrast = 0;

		public static GraphicsPath GetRoundedRectangle(RectangleF rect, float arcRadius)
		{
			var x = rect.X;
			var y = rect.Y;
			var w = rect.Width;
			var h = rect.Height;
			var d = 2 * arcRadius;

			var gp = new GraphicsPath();
			if(arcRadius == 0)
			{
				gp.AddRectangle(rect);
			}
			else
			{
				gp.AddArc(x, y, d, d, 180, 90);
				gp.AddLine(x + arcRadius, y, x + w - arcRadius - 1, y);
				gp.AddArc(x + w - d - 1, y, d, d, 270, 90);
				gp.AddLine(x + w - 1, y + arcRadius, x + w - 1, y + h - arcRadius - 1);
				gp.AddArc(x + w - d - 1, y + h - d - 1, d, d, 0, 90);
				gp.AddLine(x + w - arcRadius - 1, y + h - 1, x + arcRadius, y + h - 1);
				gp.AddArc(x, y + h - d - 1, d, d, 90, 90);
				gp.AddLine(x, y + h - arcRadius - 1, x, y + arcRadius);
			}
			gp.CloseFigure();
			return gp;
		}

		public static Region GetRoundedRegion(RectangleF rect, float arcRadius)
		{
			var x = rect.X;
			var y = rect.Y;
			var w = rect.Width;
			var h = rect.Height;
			var d = 2 * arcRadius;

			using(var gp = new GraphicsPath())
			{
				if(arcRadius == 0)
				{
					gp.AddRectangle(rect);
				}
				else
				{
					gp.AddArc(x, y, d, d, 180, 90);
					gp.AddLine(x + arcRadius, y, x + w - arcRadius + 1, y);
					gp.AddArc(x + w - d, y, d - 1, d, 270, 90);
					gp.AddLine(x + w, y + arcRadius, x + w, y + h - arcRadius);
					gp.AddArc(x + w - d, y + h - d - 1, d - 1, d, 0, 90);
					gp.AddLine(x + w - arcRadius - 1, y + h, x + arcRadius, y + h);
					gp.AddArc(x, y + h - d - 1, d, d, 90, 90);
					gp.AddLine(x, y + h - arcRadius, x, y + arcRadius);
				}
				gp.CloseFigure();
				return new Region(gp);
			}
		}

		public static GraphicsPath GetRoundedRectangle(RectangleF rect, float topLeftCorner, float topRightCorner, float bottomLeftCorner, float bottomRightCorner)
		{
			var x = rect.X;
			var y = rect.Y;
			var w = rect.Width;
			var h = rect.Height;

			var gp = new GraphicsPath();
			if(topLeftCorner != 0)
			{
				gp.AddArc(x, y,
					2 * topLeftCorner, 2 * topLeftCorner, 180, 90);
			}
			gp.AddLine(x, y, x + w - topRightCorner - 1, y);
			if(topRightCorner != 0)
			{
				gp.AddArc(
					x + w - 2 * topRightCorner - 1,
					y,
					2 * topRightCorner, 2 * topRightCorner, 270, 90);
			}
			gp.AddLine(x + w - 1, y, x + w - 1, y + h - bottomRightCorner - 1);
			if(bottomRightCorner != 0)
			{
				gp.AddArc(
					x + w - 2 * bottomRightCorner - 1,
					y + h - 2 * bottomRightCorner - 1,
					2 * bottomRightCorner, 2 * bottomRightCorner, 0, 90);
			}
			gp.AddLine(x + w, y + h - 1, x + bottomLeftCorner, y + h - 1);
			if(bottomLeftCorner != 0)
			{
				gp.AddArc(
					x,
					y + h - 2 * bottomLeftCorner - 1,
					2 * bottomLeftCorner, 2 * bottomLeftCorner, 90, 90);
			}
			gp.AddLine(x, y + h - bottomLeftCorner - 1, x, y + topLeftCorner);
			gp.CloseFigure();
			return gp;
		}

		private static ITaskbarList _taskBarList;
		internal static int WM_TASKBAR_BUTTON_CREATED;

		internal static ITaskbarList TaskBarList
		{
			get { return _taskBarList; }
		}

		public static void EnableWin7TaskbarSupport()
		{
			if(_taskBarList != null) return;
			if(IsOSWindows7OrNewer)
			{
				WM_TASKBAR_BUTTON_CREATED = NativeMethods.RegisterWindowMessage("TaskbarButtonCreated");
				_taskBarList = (ITaskbarList)Activator.CreateInstance<TaskbarList>();
			}
		}

		public static void DisableWin7TaskbarSupport()
		{
			if(_taskBarList == null) return;
			Marshal.ReleaseComObject(_taskBarList);
			_taskBarList = null;
		}

		public static void OpenUrl(string url)
		{
			var psi = new System.Diagnostics.ProcessStartInfo()
				{
					FileName = url,
				};
			if(Directory.Exists(url))
				psi.WorkingDirectory = url;
			else if(File.Exists(url))
				psi.WorkingDirectory = Path.GetDirectoryName(url);
			var p = new System.Diagnostics.Process()
			{
				StartInfo = psi,
			};
			p.Start();
		}

		public static void ShowOpenWithDialog(string fileName)
		{
			var psi = new System.Diagnostics.ProcessStartInfo()
				{
					FileName = "rundll32.exe",
					Arguments = "shell32.dll,OpenAs_RunDLL \"" + fileName + "\"",
				};
			if(Directory.Exists(fileName))
				psi.WorkingDirectory = fileName;
			else if(File.Exists(fileName))
				psi.WorkingDirectory = Path.GetDirectoryName(fileName);
			var p = new System.Diagnostics.Process()
			{
				StartInfo = psi,
			};
			p.Start();
		}

		/// <summary>Checks if process is running with administrator privileges.</summary>
		public static bool IsRunningWithAdministratorRights
		{
			get
			{
				var pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
				return pricipal.IsInRole(WindowsBuiltInRole.Administrator);
			}
		}


		[StructLayout(LayoutKind.Sequential)]
		private struct PROCESS_BASIC_INFORMATION
		{
			public int ExitStatus;
			public int PebBaseAddress;
			public int AffinityMask;
			public int BasePriority;
			public int UniqueProcessId;
			public int InheritedFromUniqueProcessId;
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern bool TerminateProcess(IntPtr hProcess, int exitCode);

		[DllImport("ntdll.dll")]

		static extern int NtQueryInformationProcess(
		   IntPtr hProcess,
		   int processInformationClass /* 0 */,
		   ref PROCESS_BASIC_INFORMATION processBasicInformation,
		   uint processInformationLength,
		   out uint returnLength
		);

		/// <summary>
		/// Terminate a process tree
		/// </summary>
		/// <param name="hProcess">The handle of the process</param>
		/// <param name="processID">The ID of the process</param>
		/// <param name="exitCode">The exit code of the process</param>
		public static void TerminateProcessTree(IntPtr hProcess, int processID, int exitCode)
		{
			foreach(var process in Process.GetProcesses())
			{
				var pbi = new PROCESS_BASIC_INFORMATION();
				try
				{
					uint bytesWritten;
					if(NtQueryInformationProcess(
						process.Handle,
						0, ref pbi, (uint)Marshal.SizeOf(pbi),
						out bytesWritten) == 0)
					{
						if(pbi.InheritedFromUniqueProcessId == processID)
						{
							TerminateProcessTree(process.Handle, pbi.UniqueProcessId, exitCode);
						}
					}
				}
				catch
				{
				}
			}
			TerminateProcess(hProcess, exitCode);
		}

		/// <summary>Runs <see cref="action"/> with administrator privileges, requesting them by UAC if necessary.</summary>
		/// <param name="action">Action to execute.</param>
		public static void ExecuteWithAdministartorRights(Action action)
		{
			if(action == null) throw new ArgumentNullException("action");

			if(IsRunningWithAdministratorRights)
			{
				action();
			}
			else
			{
				const int waitTime = 50;
				const string RemotingChannelName = "gitter.UAC";
				const string RemotingObjectName = "RemoteExecutor.rem";

				var ipc = new IpcChannel("RemoteExecutorClient");
				ChannelServices.RegisterChannel(ipc, false);
				try
				{
					var path = Path.Combine(Path.GetDirectoryName(typeof(Utility).Assembly.Location), "gitter.uac.exe");
					Process administratorProcess;
					bool allowWaitForExit = false;
					using(administratorProcess = new Process()
					{
						StartInfo = new ProcessStartInfo(path, "--remoting")
						{
							CreateNoWindow = true,
						},
					})
					{
						try
						{
							administratorProcess.Start();
						}
						catch
						{
							throw new Exception();
						}
						try
						{
							IRemoteProcedureExecutor executor = null;
							while(!administratorProcess.HasExited)
							{
								System.Threading.Thread.Sleep(waitTime);
								try
								{
									executor = (IRemoteProcedureExecutor)Activator.GetObject(typeof(IRemoteProcedureExecutor),
										string.Format(@"ipc://{0}/{1}", RemotingChannelName, RemotingObjectName));
								}
								catch { }
								if(executor != null) break;
							}
							if(executor != null)
							{
								while(!administratorProcess.HasExited)
								{
									try
									{
										executor.Execute(action);
										break;
									}
									catch(RemotingException)
									{
										System.Threading.Thread.Sleep(waitTime);
									}
								}
								if(!administratorProcess.HasExited)
								{
									try
									{
										executor.Close();
										allowWaitForExit = true;
									}
									catch
									{
									}
								}
							}
							else
							{
								throw new Exception();
							}
						}
						finally
						{
							try
							{
								if(allowWaitForExit)
								{
									if(!administratorProcess.WaitForExit(750))
									{
										administratorProcess.Kill();
									}
								}
								else
								{
									administratorProcess.Kill();
								}
							}
							catch { }
						}
					}
				}
				finally
				{
					ChannelServices.UnregisterChannel(ipc);
				}
			}
		}

		public static bool CheckIfUpdaterIsRunning()
		{
			bool singleInstance = false;
			using(var s = new Semaphore(0, 1, "gitter-updater", out singleInstance))
			{
			}
			return !singleInstance;
		}

		public static bool CheckIfCanLaunchUpdater()
		{
			const string updaterExeName = @"gitter.updater.exe";

			var sourcePath = Path.GetDirectoryName(typeof(Utility).Assembly.Location);
			return File.Exists(Path.Combine(sourcePath, updaterExeName)) && !CheckIfUpdaterIsRunning();
		}

		public static string ShowPickFolderDialog(IWin32Window parent)
		{
			if(IsOSVistaOrNewer)
			{
				try
				{
					return VistaPickFolderDialog.Show(parent);
				}
				catch { }
			}
			using(var dlg = new FolderBrowserDialog())
			{
				if(dlg.ShowDialog() == DialogResult.OK)
				{
					return dlg.SelectedPath;
				}
			}
			return null;
		}

		public static void LaunchUpdater(string cmdline)
		{
			const string updaterExeName = @"gitter.updater.exe";
			const string updaterConfigName = @"gitter.updater.exe.config";

			var sourcePath = Path.GetDirectoryName(typeof(Utility).Assembly.Location);
			var targetPath = Path.Combine(Path.GetTempPath(), "gitter-updater");
			if(!Directory.Exists(targetPath))
			{
				Directory.CreateDirectory(targetPath);
			}
			File.Copy(Path.Combine(sourcePath, updaterExeName), Path.Combine(targetPath, updaterExeName), true);
			File.Copy(Path.Combine(sourcePath, updaterConfigName), Path.Combine(targetPath, updaterConfigName), true);
			using(var process = new Process())
			{
				process.StartInfo = new ProcessStartInfo(Path.Combine(targetPath, updaterExeName), cmdline)
				{
					WindowStyle = ProcessWindowStyle.Normal,
					CreateNoWindow = false,
					LoadUserProfile = true,
				};
				process.Start();
			}
		}
	}
}
