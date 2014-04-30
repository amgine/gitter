#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Security;
	using System.Security.Principal;
	using System.Text;
	using System.Windows.Forms;
	using gitter.Native;

	public static class Utility
	{
		private static readonly Version _osVersion = Environment.OSVersion.Version;

		/// <summary>1 Jan 1970</summary>
		private static readonly DateTime UnixEraStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static bool IsOSVistaOrNewer
		{
			get { return _osVersion >= new Version(6, 0); }
		}

		public static bool IsOSWindows7OrNewer
		{
			get { return _osVersion >= new Version(6, 1); }
		}

		public static string FormatDate(DateTime date, DateFormat format)
		{
			switch(format)
			{
				case DateFormat.SystemDefault:
					return date.ToString(CultureInfo.CurrentCulture);
				case DateFormat.UnixTimestamp:
					return ((int)(date - UnixEraStart).TotalSeconds).ToString(CultureInfo.InvariantCulture);
				case DateFormat.Relative:
					{
						var span = DateTime.Now - date;
						if(span.TotalDays >= 365)
						{
							var years = (int)(span.TotalDays / 365);
							return (years == 1) ? "1 year ago" : years.ToString(CultureInfo.InvariantCulture) + " years ago";
						}
						if(span.TotalDays >= 30)
						{
							var months = (int)(span.TotalDays / 30);
							return (months == 1) ? "1 month ago" : months.ToString(CultureInfo.InvariantCulture) + " months ago";
						}
						if(span.TotalDays >= 7)
						{
							var weeks = (int)(span.TotalDays / 7);
							return (weeks == 1) ? "1 week ago" : weeks.ToString(CultureInfo.InvariantCulture) + " weeks ago";
						}
						if(span.TotalDays >= 1)
						{
							var days = (int)span.TotalDays;
							return (days == 1) ? "1 day ago" : days.ToString(CultureInfo.InvariantCulture) + " days ago";
						}
						if(span.TotalHours >= 1)
						{
							var hours = (int)span.TotalHours;
							return (hours == 1) ? "1 hour ago" : hours.ToString(CultureInfo.InvariantCulture) + " hours ago";
						}
						if(span.TotalMinutes >= 1)
						{
							var minutes = (int)span.TotalMinutes;
							return (minutes == 1) ? "1 minute ago" : minutes.ToString(CultureInfo.InvariantCulture) + " minutes ago";
						}
						var seconds = (int)span.TotalSeconds;
						return (seconds == 1) ? "1 second ago" : seconds.ToString(CultureInfo.InvariantCulture) + " seconds ago";
					}
				case DateFormat.ISO8601:
					return date.FormatISO8601();
				case DateFormat.RFC2822:
					return date.FormatRFC2822();
				default:
					throw new ArgumentException(
						"Unknown DateFormat value: {0}".UseAsFormat(date),
						"format");
			}
		}

		public static string ExpandNewLineCharacters(string text)
		{
			if(Environment.NewLine == "\n") return text;

			var sb = new StringBuilder(text.Length + 20);
			for(int i = 0; i < text.Length; ++i)
			{
				var c = text[i];
				if(c == '\r')
				{
					continue;
				}
				if(c == '\n')
				{
					sb.Append(Environment.NewLine);
					continue;
				}
				sb.Append(c);
			}
			return sb.ToString();
		}

		private static readonly Func<int, string> _strAlloc = GetAllocateStringMethod();

		private static Func<int, string> GetAllocateStringMethod()
		{
			var method = typeof(string).GetMethod("FastAllocateString", BindingFlags.Static | BindingFlags.NonPublic);
			if(method == null)
			{
				return new Func<int, string>(length => new string(' ', length));
			}
			else
			{
				return (Func<int, string>)Delegate.CreateDelegate(typeof(Func<int, string>), method);
			}
		}

		public static string FastAllocateString(int length)
		{
			return _strAlloc(length);
		}

		public static string GetFileType(string fileName, bool dir, bool useExtensionOnly)
		{
			const int SHGFI_USEFILEATTRIBUTES = 0x10;
			const int SHGFI_TYPENAME = 0x400;

			const int FILE_ATTRIBUTE_NORMAL = 0x80;
			const int FILE_ATTRIBUTE_DIR = 0x10;

			var attr = dir ? FILE_ATTRIBUTE_DIR | FILE_ATTRIBUTE_NORMAL : FILE_ATTRIBUTE_NORMAL;
			var req = useExtensionOnly ? SHGFI_USEFILEATTRIBUTES | SHGFI_TYPENAME : SHGFI_TYPENAME;

			var info = new SHFILEINFO();
			var ret = Shell32.SHGetFileInfo(fileName, attr, ref info, Marshal.SizeOf(info), req);
			return info.szTypeName;
		}

		public static Control GetParentControl(ToolStripItem item)
		{
			Verify.Argument.IsNotNull(item, "item");

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
			Verify.Argument.IsNotNull(menu, "menu");

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

		public static AnchorStyles InvertAnchor(AnchorStyles anchor)
		{
			var inverted = AnchorStyles.None;
			if((anchor & (AnchorStyles.Left | AnchorStyles.Right)) != (AnchorStyles.Left | AnchorStyles.Right))
			{
				if((anchor & AnchorStyles.Left) == AnchorStyles.Left) inverted |= AnchorStyles.Right;
				if((anchor & AnchorStyles.Right) == AnchorStyles.Right) inverted |= AnchorStyles.Left;
			}
			if((anchor & (AnchorStyles.Top | AnchorStyles.Bottom)) != (AnchorStyles.Top | AnchorStyles.Bottom))
			{
				if((anchor & AnchorStyles.Top) == AnchorStyles.Top) inverted |= AnchorStyles.Bottom;
				if((anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom) inverted |= AnchorStyles.Bottom;
			}
			return inverted;
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
				WM_TASKBAR_BUTTON_CREATED = User32.RegisterWindowMessage("TaskbarButtonCreated");
				_taskBarList = (ITaskbarList)Activator.CreateInstance<TaskbarList>();
			}
		}

		public static void DisableWin7TaskbarSupport()
		{
			if(_taskBarList == null) return;
			Marshal.ReleaseComObject(_taskBarList);
			_taskBarList = null;
		}

		public static Process CreateProcessFor(string url)
		{
			var psi = new ProcessStartInfo()
				{
					FileName = url,
				};
			if(Directory.Exists(url))
			{
				psi.WorkingDirectory = url;
			}
			else if(File.Exists(url))
			{
				psi.WorkingDirectory = Path.GetDirectoryName(url);
			}
			var process = new Process()
			{
				StartInfo = psi,
			};
			return process;
		}

		public static void OpenUrl(string url)
		{
			var process = CreateProcessFor(url);
			process.Start();
			process.Dispose();
		}

		[DllImport("shell32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		static extern int OpenAs_RunDLL(IntPtr hwnd, IntPtr hInst, string lpFile, int nShowCmd);

		public static void ShowOpenWithDialog(string fileName)
		{
			const int SW_NORMAL = 1;

			OpenAs_RunDLL(
				GitterApplication.MainForm.Handle,
				Marshal.GetHINSTANCE(GitterApplication.MainForm.GetType().Module),
				fileName,
				SW_NORMAL);
		}

		private static readonly Lazy<bool> _isRunningWithAdministratorRights =
			new Lazy<bool>(
				() =>
				{
					var pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
					return pricipal.IsInRole(WindowsBuiltInRole.Administrator);
				});

		/// <summary>Checks if process is running with administrator privileges.</summary>
		public static bool IsRunningWithAdministratorRights
		{
			get { return _isRunningWithAdministratorRights.Value; }
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
				if(dlg.ShowDialog(parent) == DialogResult.OK)
				{
					return dlg.SelectedPath;
				}
			}
			return null;
		}
	}
}
