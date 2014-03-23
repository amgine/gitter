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

namespace gitter
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;

	using Resources = gitter.Properties.Resources;

	static class GuiItemFactory
	{
		public static T GetRemoveRecentRepositoryItem<T>(RepositoryLink repository)
			where T : ToolStripItem, new()
		{
			Verify.Argument.IsNotNull(repository, "repository");

			var item = new T()
			{
				Text  = Resources.StrRemoveRepository,
				Image = CachedResources.Bitmaps["ImgRepositoryRemove"],
				Tag   = repository,
			};
			item.Click += OnRemoveRecentRepositoryClick;
			return item;
		}

		public static T GetRemoveRepositoryItem<T>(RepositoryListItem repository)
			where T : ToolStripItem, new()
		{
			Verify.Argument.IsNotNull(repository, "repository");

			var item = new T()
			{
				Text = Resources.StrRemoveRepository,
				Image = CachedResources.Bitmaps["ImgRepositoryRemove"],
				Tag = repository,
			};
			item.Click += OnRemoveRepositoryClick;
			return item;
		}

		public static T GetOpenUrlItem<T>(string name, Image image, string url)
			where T : ToolStripItem, new()
		{
			Verify.Argument.IsNeitherNullNorWhitespace(url, "url");

			var item = new T()
			{
				Image = image,
				Text = name != null ? name : url,
				Tag = url,
			};
			item.Click += OnOpenUrlItemClick;
			return item;
		}

		public static T GetOpenCmdAtItem<T>(string name, Image image, string path)
			where T : ToolStripItem, new()
		{
			Verify.Argument.IsNeitherNullNorWhitespace(path, "path");

			var item = new T()
			{
				Image = image,
				Text = name != null ? name : path,
				Tag = path,
			};
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
			repository.Remove();
		}

		private static void OnOpenUrlItemClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var url = (string)item.Tag;

			Utility.OpenUrl(url);
		}

		private static void OnOpenCmdAtItemClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var path = (string)item.Tag;

			var psi = new ProcessStartInfo("cmd")
				{
					WorkingDirectory = path,
				};
			using(var p = new Process())
			{
				p.StartInfo = psi;
				p.Start();
			}
		}

		public static IList<T> GetRepositoryActions<T>(string workingDirectory)
			where T : ToolStripItem, new()
		{
			var p = GitterApplication.WorkingEnvironment.FindProviderForDirectory(workingDirectory);
			if(p != null)
			{
				var commands = new List<GuiCommand>(p.GetRepositoryCommands(workingDirectory));
				if(commands.Count == 0)
				{
					return new T[0];
				}
				var res = new List<T>(commands.Count);
				foreach(var cmd in commands)
				{
					var item = new T()
					{
						Tag		= cmd,
						Name	= cmd.Name,
						Text	= cmd.DisplayName,
						Image	= cmd.Image,
					};
					item.Click += OnGuiCommandItemClick;
					res.Add(item);
				}
				return res;
			}
			return new T[0];
		}

		static void OnGuiCommandItemClick(object sender, EventArgs e)
		{
			var item	= (ToolStripItem)sender;
			var cmd		= (GuiCommand)item.Tag;

			cmd.Execute(GitterApplication.WorkingEnvironment);
		}
	}
}
