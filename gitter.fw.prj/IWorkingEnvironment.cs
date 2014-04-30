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
	using System.ComponentModel;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using gitter.Framework.Services;
	using gitter.Framework.Controls;

	/// <summary>GUI Working Environment.</summary>
	public interface IWorkingEnvironment : ISynchronizeInvoke
	{
		Form MainForm { get; }

		string RecentRepositoryPath { get; }

		RepositoryManagerService RepositoryManagerService { get; }


		IEnumerable<IRepositoryProvider> RepositoryProviders { get; }

		T GetRepositoryProvider<T>() where T : class, IRepositoryProvider;

		IEnumerable<IRepositoryServiceProvider> IssueTrackerProviders { get; }

		IEnumerable<IRepositoryServiceProvider> ActiveIssueTrackerProviders { get; }

		bool TryLoadIssueTracker(IRepositoryServiceProvider provider);


		ViewDockService ViewDockService { get; }

		INotificationService NotificationService { get; }


		IRepositoryProvider FindProviderForDirectory(string workingDirectory);

		IRepositoryProvider ActiveRepositoryProvider { get; }

		IRepository ActiveRepository { get; }

		bool OpenRepository(string path);

		void CloseRepository();


		void ProvideMainMenuItem(ToolStripMenuItem item);

		void ProvideViewMenuItem(ToolStripMenuItem item);

		void ProvideRepositoryExplorerItem(CustomListBoxItem item);

		void ProvideToolbar(ToolStrip toolStrip);

		void ProvideStatusBarObject(ToolStripItem item, bool leftAlign);


		void RemoveMainMenuItem(ToolStripMenuItem item);

		void RemoveViewMenuItem(ToolStripMenuItem item);

		void RemoveRepositoryExplorerItem(CustomListBoxItem item);

		void RemoveToolbar(ToolStrip toolStrip);

		void RemoveStatusBarObject(ToolStripItem item);
	}
}
