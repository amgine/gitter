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

namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	using gitter.Git.AccessLayer.CLI;

	internal sealed partial class RepositoryCLI : IBranchAccessor
	{
		/// <summary>Check if branch exists and get its position.</summary>
		/// <param name="parameters"><see cref="QueryBranchParameters"/>.</param>
		/// <returns><see cref="BranchData"/> or null, if requested branch doesn't exist.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public BranchData QueryBranch(QueryBranchParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			string fullName = (parameters.IsRemote ?
				GitConstants.RemoteBranchPrefix :
				GitConstants.LocalBranchPrefix) + parameters.BranchName;
			var cmd = new ShowRefCommand(
					ShowRefCommand.Verify(),
					ShowRefCommand.Heads(),
					ShowRefCommand.Hash(),
					ShowRefCommand.NoMoreOptions(),
					new CommandArgument(fullName));

			var output = _executor.ExecCommand(cmd);
			if(output.ExitCode == 0)
			{
				var hash = output.Output.Substring(0, 40);
				return new BranchData(parameters.BranchName, hash, false, parameters.IsRemote, false);
			}
			else
			{
				return null;
			}
		}

		/// <summary>Query branch list.</summary>
		/// <param name="parameters"><see cref="QueryBranchesParameters"/>.</param>
		/// <returns>List of requested branches.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public BranchesData QueryBranches(QueryBranchesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>(6);
			args.Add(BranchCommand.NoColor());
			args.Add(BranchCommand.Verbose());
			args.Add(BranchCommand.NoAbbrev());
			switch(parameters.Restriction)
			{
				case QueryBranchRestriction.All:
					args.Add(BranchCommand.All());
					break;
				case QueryBranchRestriction.Remote:
					args.Add(BranchCommand.Remote());
					break;
			}
			switch(parameters.Mode)
			{
				case BranchQueryMode.Contains:
					args.Add(BranchCommand.Contains());
					break;
				case BranchQueryMode.Merged:
					args.Add(BranchCommand.Merged());
					break;
				case BranchQueryMode.NoMerged:
					args.Add(BranchCommand.NoMerged());
					break;
			}
			if(parameters.Revision != null)
			{
				args.Add(new CommandArgument(parameters.Revision));
			}

			var cmd = new BranchCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();

			var parser = new GitParser(output.Output);
			return parser.ParseBranches(parameters.Restriction, parameters.AllowFakeBranch);
		}

		/// <summary>Create local branch.</summary>
		/// <param name="parameters"><see cref="CreateBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void CreateBranch(CreateBranchParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>(6);
			switch(parameters.TrackingMode)
			{
				case BranchTrackingMode.NotTracking:
					args.Add(BranchCommand.NoTrack());
					break;
				case BranchTrackingMode.Tracking:
					args.Add(BranchCommand.Track());
					break;
			}
			if(parameters.CreateReflog)
			{
				args.Add(BranchCommand.RefLog());
			}
			if(parameters.Checkout)
			{
				if(parameters.Orphan)
				{
					args.Add(CheckoutCommand.Orphan());
				}
				else
				{
					args.Add(CheckoutCommand.Branch());
				}
			}
			args.Add(new CommandArgument(parameters.BranchName));
			if(!string.IsNullOrEmpty(parameters.StartingRevision))
			{
				args.Add(new CommandArgument(parameters.StartingRevision));
			}

			Command cmd;
			if(parameters.Checkout)
			{
				cmd = new CheckoutCommand(args);
			}
			else
			{
				cmd = new BranchCommand(args);
			}

			var output = _executor.ExecCommand(cmd);
			if(output.ExitCode != 0)
			{
				if(IsUnknownRevisionError(output.Error, parameters.StartingRevision))
				{
					throw new UnknownRevisionException(parameters.StartingRevision);
				}
				if(IsBranchAlreadyExistsError(output.Error, parameters.BranchName))
				{
					throw new BranchAlreadyExistsException(parameters.BranchName);
				}
				if(IsInvalidBranchNameError(output.Error, parameters.BranchName))
				{
					throw new InvalidBranchNameException(parameters.BranchName);
				}
				output.Throw();
			}
		}

		/// <summary>Reset local branch.</summary>
		/// <param name="parameters"><see cref="ResetBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void ResetBranch(ResetBranchParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var cmd = new BranchCommand(
				BranchCommand.Reset(),
				new CommandArgument(parameters.BranchName),
				new CommandArgument(parameters.Revision));

			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>Remove branch <paramref name="branchName"/>.</summary>
		/// <param name="parameters"><see cref="DeleteBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void DeleteBranch(DeleteBranchParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			string branchName = parameters.BranchName;
			bool force = parameters.Force;
			bool remote = parameters.Remote;

			Command cmd;
			if(remote)
			{
				cmd = new BranchCommand(
					force ? BranchCommand.DeleteForce() : BranchCommand.Delete(),
					BranchCommand.Remote(),
					new CommandArgument(branchName));
			}
			else
			{
				cmd = new BranchCommand(
					force ? BranchCommand.DeleteForce() : BranchCommand.Delete(),
					new CommandArgument(branchName));
			}

			var output = _executor.ExecCommand(cmd);
			if(output.ExitCode != 0)
			{
				if(IsBranchNotFoundError(output.Error, remote, branchName))
				{
					throw new BranchNotFoundException(branchName);
				}
				if(!force)
				{
					if(IsBranchNotFullyMergedError(output.Error, branchName))
					{
						throw new BranchIsNotFullyMergedException(branchName);
					}
				}
				output.Throw();
			}
		}

		/// <summary>Rename branch.</summary>
		/// <param name="parameters"><see cref="RenameBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void RenameBranch(RenameBranchParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var cmd = new BranchCommand(
				parameters.Force?BranchCommand.MoveForce():BranchCommand.Move(),
				new CommandArgument(parameters.OldName),
				new CommandArgument(parameters.NewName));
			var output = _executor.ExecCommand(cmd);
			if(output.ExitCode != 0)
			{
				if(IsBranchAlreadyExistsError(output.Error, parameters.NewName))
				{
					throw new BranchAlreadyExistsException(parameters.NewName);
				}
				if(IsInvalidBranchNameError(output.Error, parameters.NewName))
				{
					throw new InvalidBranchNameException(parameters.NewName);
				}
				output.Throw();
			}
		}
	}
}
