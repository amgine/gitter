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

namespace gitter;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Services;

using Resources = gitter.Properties.Resources;

class GuiItemFactory
{
	private readonly DpiBindings _dpiBindings;

	public GuiItemFactory(DpiBindings dpiBindings)
	{
		Verify.Argument.IsNotNull(dpiBindings);

		_dpiBindings = dpiBindings;
	}

	private static readonly IPathCommandsProvider[] _pathCommandsProviders =
		new IPathCommandsProvider[]
		{
			new OpenWithVisualStudioCodeProvider(),
			new OpenVisualStudioSolutionsProvider(),
		};

	interface IPathCommandsProvider
	{
		bool Nested { get; }

		string DisplayName { get; }

		IEnumerable<GuiCommand> GetPathCommands(string path);
	}

	class OpenWithVisualStudioCodeProvider : IPathCommandsProvider
	{
		private string _vsCodePath;
		private bool _vsCodeSearched;

		private static string FindVSCode()
		{
			var path0 = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				@"Programs\Microsoft VS Code\Code.exe");

			if(File.Exists(path0)) return path0;

			var path1 = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
				@"Microsoft VS Code\Code.exe");

			if(File.Exists(path1)) return path1;

			if(Environment.Is64BitOperatingSystem)
			{
				var path2 = Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
					@"Microsoft VS Code\Code.exe");

				if(File.Exists(path2)) return path2;
			}

			return default;
		}

		public bool Nested => false;

		public string DisplayName => Resources.StrOpenInVSCode;

		public IEnumerable<GuiCommand> GetPathCommands(string path)
		{
			if(!_vsCodeSearched)
			{
				_vsCodePath = FindVSCode();
				_vsCodeSearched = true;
			}

			if(_vsCodePath is null) yield break;

			yield return new GuiCommand("OpenVSCode", DisplayName, CommonIcons.VSCode, e =>
			{
				Process.Start(new ProcessStartInfo
				{
					FileName  = _vsCodePath,
					Arguments = path.AssureDoubleQuotes(),
				})?.Dispose();
			});
		}
	}

	class OpenVisualStudioSolutionsProvider : IPathCommandsProvider
	{
		public bool Nested => true;

		public string DisplayName => Resources.StrOpenVisualStudioSolutions;

		static string GetFileName(string path, string fullFileName)
		{
			var len = path.Length;
			if(!path.EndsWith(Path.DirectorySeparatorChar) && !path.EndsWith(Path.AltDirectorySeparatorChar))
			{
				++len;
			}
			if(fullFileName.Length > len)
			{
				fullFileName = fullFileName.Substring(len);
			}
			return fullFileName;
		}

		public IEnumerable<GuiCommand> GetPathCommands(string path)
		{
			path = Path.GetFullPath(path);
			foreach(var sln in Directory.EnumerateFiles(path, "*.sln", SearchOption.TopDirectoryOnly))
			{
				yield return new GuiCommand("OpenVSSolution", GetFileName(path, sln), CommonIcons.Solution, e => Utility.OpenUrl(sln));
			}
			foreach(var dir in Directory.EnumerateDirectories(path))
			{
				foreach(var sln in Directory.EnumerateFiles(dir, "*.sln", SearchOption.TopDirectoryOnly))
				{
					yield return new GuiCommand("OpenVSSolution", GetFileName(path, sln), CommonIcons.Solution, e => Utility.OpenUrl(sln));
				}
			}
		}
	}

	public T GetRemoveRecentRepositoryItem<T>(RepositoryLink repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrRemoveRepository,
			Tag  = repository,
		};
		_dpiBindings.BindImage(item, Icons.RepositoryRemove);
		item.Click += OnRemoveRecentRepositoryClick;
		return item;
	}

	public T GetRemoveRepositoryItem<T>(RepositoryListItem repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrRemoveRepository,
			Tag  = repository,
		};
		_dpiBindings.BindImage(item, Icons.RepositoryRemove);
		item.Click += OnRemoveRepositoryClick;
		return item;
	}

	public static T GetOpenUrlItem<T>(string name, Image image, string url)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNeitherNullNorWhitespace(url);

		var item = new T()
		{
			Image = image,
			Text  = name ?? url,
			Tag   = url,
		};
		item.Click += OnOpenUrlItemClick;
		return item;
	}

	public T GetOpenCmdAtItem<T>(string name, string path)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNeitherNullNorWhitespace(path);

		var item = new T()
		{
			Text  = name ?? path,
			Tag   = path,
		};
		_dpiBindings.BindImage(item, CommonIcons.Terminal);
		item.Click += OnOpenCmdAtItemClick;
		return item;
	}

	private static void OnRemoveRecentRepositoryClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var repo = (RepositoryLink)item.Tag;

		GitterApplication.WorkingEnvironment.RepositoryManagerService.RecentRepositories.Remove(repo);
	}

	private static void OnRemoveRepositoryClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var repository = (RepositoryListItem)item.Tag;

		if(repository.ListBox is LocalRepositoriesListBox list && list.FullList != null)
		{
			list.FullList.Remove(repository);
		}
		repository.Remove();
	}

	private static void OnOpenUrlItemClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var url  = (string)item.Tag;

		Utility.OpenUrl(url);
	}

	private static void OnOpenCmdAtItemClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var path = (string)item.Tag;

		var psi = new ProcessStartInfo(@"cmd")
			{
				WorkingDirectory = path,
			};
		using var p = new Process();
		p.StartInfo = psi;
		p.Start();
	}

	private T WrapCommand<T>(GuiCommand command)
		where T : ToolStripItem, new()
	{
		var item = new T()
		{
			Tag     = command,
			Name    = command.Name,
			Text    = command.DisplayName,
			Enabled = command.IsEnabled,
		};
		if(command.Image is not null)
		{
			_dpiBindings.BindImage(item, command.Image);
		}
		item.Click += OnGuiCommandItemClick;
		return item;
	}

	public IReadOnlyList<ToolStripItem> GetRepositoryActions(string workingDirectory)
	{
		if(!Directory.Exists(workingDirectory)) return Preallocated<ToolStripItem>.EmptyArray;

		var res = new List<ToolStripItem>();

		foreach(var provider in _pathCommandsProviders)
		{
			var parent = default(ToolStripMenuItem);
			foreach(var command in provider.GetPathCommands(workingDirectory))
			{
				if(provider.Nested)
				{
					if(parent is null)
					{
						parent = new ToolStripMenuItem(provider.DisplayName);
						res.Add(parent);
					}
					parent.DropDownItems.Add(WrapCommand<ToolStripMenuItem>(command));
				}
				else
				{
					res.Add(WrapCommand<ToolStripMenuItem>(command));
				}
			}
		}

		var p = GitterApplication.WorkingEnvironment.FindProviderForDirectory(workingDirectory);
		if(p is not null)
		{
			bool separatorAdded = res.Count == 0;
			foreach(var command in p.GetRepositoryCommands(workingDirectory))
			{
				if(!separatorAdded)
				{
					res.Add(new ToolStripSeparator());
					separatorAdded = true;
				}
				res.Add(WrapCommand<ToolStripMenuItem>(command));
			}
		}

		return res;
	}

	static void OnGuiCommandItemClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var cmd  = (GuiCommand)item.Tag;

		cmd.Execute(GitterApplication.WorkingEnvironment);
	}
}
