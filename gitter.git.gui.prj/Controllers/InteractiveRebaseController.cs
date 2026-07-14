#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2026  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.Gui.Controllers;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Services;

using gitter.Git.Gui.Dialogs;

/// <summary>Drives the interactive rebase flow.</summary>
public sealed class InteractiveRebaseController
{
	readonly IInteractiveRebaseExecutor _executor;

	public InteractiveRebaseController(IInteractiveRebaseExecutor executor)
	{
		Verify.Argument.IsNotNull(executor);
		_executor = executor;
	}

	public async Task RunAsync(string upstream, string? onto = null, IWin32Window? owner = null)
	{
		Verify.Argument.IsNeitherNullNorWhitespace(upstream);

		IInteractiveRebaseSession session;
		try
		{
			session = await _executor.StartAsync(upstream, onto).ConfigureAwait(true);
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
			GitterApplication.MessageBoxService.Show(
				owner,
				exc.Message,
				"Interactive Rebase",
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			return;
		}

		using var dialog = new InteractiveRebaseDialog { Entries = session.Entries };
		var dialogResult = dialog.Run(owner);

		try
		{
			if(dialogResult != DialogResult.OK)
			{
				await session.AbortAsync().ConfigureAwait(true);
				return;
			}

			var plan = dialog.ResultPlan;

			// git complains if all entries are drop
			if(plan.Count > 0 && plan.All(e => e.Action == RebaseAction.Drop))
			{
				var confirm = GitterApplication.MessageBoxService.Show(
					owner,
					"All commits are marked Drop. The branch will be reset to the rebase base. Continue?",
					"Interactive Rebase",
					MessageBoxButton.YesNo,
					MessageBoxIcon.Warning);
				if(confirm != DialogResult.Yes)
				{
					await session.AbortAsync().ConfigureAwait(true);
					return;
				}
			}

			RebaseResult result;
			var prevCursor = Cursor.Current;
			try
			{
				Cursor.Current = Cursors.WaitCursor;
				result = await session.CommitPlanAsync(plan).ConfigureAwait(true);
			}
			finally { Cursor.Current = prevCursor; }

			// non-zero exit covers both errors and conflict pauses (rebase-in-progress UI shows up after refresh)
			if(!result.Success)
			{
				var message = string.IsNullOrWhiteSpace(result.Stderr)
					? $"git rebase exited with code {result.ExitCode}."
					: result.Stderr;
				GitterApplication.MessageBoxService.Show(
					owner, message, "Interactive Rebase",
					MessageBoxButton.Close, MessageBoxIcon.Warning);
			}
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
			GitterApplication.MessageBoxService.Show(
				owner,
				exc.Message,
				"Interactive Rebase",
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
		}
	}
}
