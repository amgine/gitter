﻿#region Copyright Notice
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

namespace gitter.Framework;

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#pragma warning disable 0108

static class VistaPickFolderDialog
{
	static class IIDGuid
	{
		internal const string IModalWindow = "b4db1657-70d7-485e-8e3e-6fcb5a5c1802";
		internal const string IFileDialog = "42f85136-db7e-439c-85f1-e4075d135fc8";
		internal const string IFileOpenDialog = "d57c7288-d4ad-4768-be02-9d969532d960";
	}

	static class CLSIDGuid
	{
		internal const string FileOpenDialog = "DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7";
	}

	[Flags]
	internal enum FOS : uint
	{
		FOS_ALLNONSTORAGEITEMS = 0x80,
		FOS_ALLOWMULTISELECT = 0x200,
		FOS_CREATEPROMPT = 0x2000,
		FOS_DEFAULTNOMINIMODE = 0x20000000,
		FOS_DONTADDTORECENT = 0x2000000,
		FOS_FILEMUSTEXIST = 0x1000,
		FOS_FORCEFILESYSTEM = 0x40,
		FOS_FORCESHOWHIDDEN = 0x10000000,
		FOS_HIDEMRUPLACES = 0x20000,
		FOS_HIDEPINNEDPLACES = 0x40000,
		FOS_NOCHANGEDIR = 8,
		FOS_NODEREFERENCELINKS = 0x100000,
		FOS_NOREADONLYRETURN = 0x8000,
		FOS_NOTESTFILECREATE = 0x10000,
		FOS_NOVALIDATE = 0x100,
		FOS_OVERWRITEPROMPT = 2,
		FOS_PATHMUSTEXIST = 0x800,
		FOS_PICKFOLDERS = 0x20,
		FOS_SHAREAWARE = 0x4000,
		FOS_STRICTFILETYPES = 4
	}

	[ComImport]
	[Guid(IIDGuid.IModalWindow)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IModalWindow
	{
		[PreserveSig]
		int Show([In] IntPtr parent);
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
	internal struct COMDLG_FILTERSPEC
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		internal string pszName;
		[MarshalAs(UnmanagedType.LPWStr)]
		internal string pszSpec;
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid(IIDGuid.IFileDialog)]
	internal interface IFileDialog
	{
		[PreserveSig]
		int Show([In] IntPtr parent);
		void SetFileTypes([In] uint cFileTypes, [In, MarshalAs(UnmanagedType.LPArray)] COMDLG_FILTERSPEC[] rgFilterSpec);
		void SetFileTypeIndex([In] uint iFileType);
		void GetFileTypeIndex(out uint piFileType);
		void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);
		void Unadvise([In] uint dwCookie);
		void SetOptions([In] FOS fos);
		void GetOptions(out FOS pfos);
		void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);
		void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);
		void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
		void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
		void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);
		void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);
		void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
		void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);
		void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
		void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
		void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, int alignment);
		void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);
		void Close([MarshalAs(UnmanagedType.Error)] int hr);
		void SetClientGuid([In] ref Guid guid);
		void ClearClientData();
		void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid(IIDGuid.IFileOpenDialog)]
	internal interface IFileOpenDialog : IFileDialog
	{
		[PreserveSig]
		int Show([In] IntPtr parent);
		void SetFileTypes([In] uint cFileTypes, [In] ref COMDLG_FILTERSPEC rgFilterSpec);
		void SetFileTypeIndex([In] uint iFileType);
		void GetFileTypeIndex(out uint piFileType);
		void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);
		void Unadvise([In] uint dwCookie);
		void SetOptions([In] FOS fos);
		void GetOptions(out FOS pfos);
		void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);
		void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);
		void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
		void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
		void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);
		void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);
		void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
		void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);
		void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
		void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
		void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, FileDialogCustomPlace fdcp);
		void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);
		void Close([MarshalAs(UnmanagedType.Error)] int hr);
		void SetClientGuid([In] ref Guid guid);
		void ClearClientData();
		void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);
		void GetResults([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppenum);
		void GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppsai);
	}

	internal enum FDE_OVERWRITE_RESPONSE
	{
		FDEOR_DEFAULT,
		FDEOR_ACCEPT,
		FDEOR_REFUSE
	}

	internal enum FDE_SHAREVIOLATION_RESPONSE
	{
		FDESVR_DEFAULT,
		FDESVR_ACCEPT,
		FDESVR_REFUSE
	}

	[ComImport]
	[Guid("973510DB-7D7F-452B-8975-74A85828D354")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IFileDialogEvents
	{
		[PreserveSig]
		int OnFileOk([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);
		[PreserveSig]
		int OnFolderChanging([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psiFolder);
		void OnFolderChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);
		void OnSelectionChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);
		void OnShareViolation([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, out FDE_SHAREVIOLATION_RESPONSE pResponse);
		void OnTypeChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);
		void OnOverwrite([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, out FDE_OVERWRITE_RESPONSE pResponse);
	}

	internal enum SIGDN : uint
	{
		SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,
		SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,
		SIGDN_FILESYSPATH = 0x80058000,
		SIGDN_NORMALDISPLAY = 0,
		SIGDN_PARENTRELATIVE = 0x80080001,
		SIGDN_PARENTRELATIVEEDITING = 0x80031001,
		SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,
		SIGDN_PARENTRELATIVEPARSING = 0x80018001,
		SIGDN_URL = 0x80068000
	}

	[ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IShellItem
	{
		void BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid bhid, [In] ref Guid riid, out IntPtr ppv);
		void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
		void GetDisplayName([In] SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);
		void GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);
		void Compare([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, out int piOrder);
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct PROPERTYKEY
	{
		internal Guid fmtid;
		internal uint pid;
	}

	internal enum SIATTRIBFLAGS
	{
		SIATTRIBFLAGS_AND = 1,
		SIATTRIBFLAGS_APPCOMPAT = 3,
		SIATTRIBFLAGS_OR = 2
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("B63EA76D-1F85-456F-A19C-48159EFA858B")]
	internal interface IShellItemArray
	{
		void BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid rbhid, [In] ref Guid riid, out IntPtr ppvOut);
		void GetPropertyStore([In] int Flags, [In] ref Guid riid, out IntPtr ppv);
		void GetPropertyDescriptionList([In] ref PROPERTYKEY keyType, [In] ref Guid riid, out IntPtr ppv);
		void GetAttributes([In] SIATTRIBFLAGS dwAttribFlags, [In] uint sfgaoMask, out uint psfgaoAttribs);
		void GetCount(out uint pdwNumItems);
		void GetItemAt([In] uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
		void EnumItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenumShellItems);
	}

	[ComImport]
	[ClassInterface(ClassInterfaceType.None)]
	[TypeLibType(TypeLibTypeFlags.FCanCreate)]
	[Guid(CLSIDGuid.FileOpenDialog)]
	internal class FileOpenDialogRCW
	{
	}

	[ComImport]
	[Guid(IIDGuid.IFileOpenDialog)]
	[CoClass(typeof(FileOpenDialogRCW))]
	internal interface NativeFileOpenDialog : IFileOpenDialog
	{
	}

	internal static string GetFilePathFromShellItem(IShellItem item)
	{
		string str;
		unchecked
		{
			item.GetDisplayName((SIGDN)(-2147319808), out str);
		}
		return str;
	}

	public static string Show(IWin32Window parent)
	{
		var dialog = default(IFileDialog);
		try
		{
			dialog = new NativeFileOpenDialog();
			dialog.SetOptions(FOS.FOS_PICKFOLDERS);
			if(dialog.Show(parent is not null ? parent.Handle : IntPtr.Zero) == 0)
			{
				string result = string.Empty;
				IShellItem item;
				dialog.GetResult(out item);
				return GetFilePathFromShellItem(item);
			}
			else
			{
				return null;
			}
		}
		finally
		{
			if(dialog is not null)
			{
				Marshal.FinalReleaseComObject(dialog);
			}
		}
	}
}
