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

		IEnumerable<string> RecentRepositories { get; }


		IEnumerable<IRepositoryProvider> RepositoryProviders { get; }

		IEnumerable<IIssueTrackerProvider> IssueTrackerProviders { get; }

		IEnumerable<IIssueTrackerProvider> ActiveIssueTrackerProviders { get; }

		bool TryLoadIssueTracker(IIssueTrackerProvider provider);


		ViewDockService ViewDockService { get; }

		INotificationService NotificationService { get; }


		IRepositoryProvider FindProvider(string workingDirectory);

		IRepository ActiveRepository { get; }

		bool OpenRepository(string path);

		void CloseRepository();


		void ProvideMainMenuItem(ToolStripMenuItem item);

		void ProvideRepositoryExplorerItem(CustomListBoxItem item);

		void ProvideToolbar(ToolStrip toolStrip);

		void ProvideStatusBarObject(ToolStripItem item, bool leftAlign);


		void RemoveMainMenuItem(ToolStripMenuItem item);

		void RemoveRepositoryExplorerItem(CustomListBoxItem item);

		void RemoveToolbar(ToolStrip toolStrip);

		void RemoveStatusBarObject(ToolStripItem item);
	}
}
