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

namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;

	static class GitConfigHelper
	{
		private static void InsertConfigFileSpecifier(IList<CommandArgument> args, BaseConfigParameters parameters)
		{
			switch(parameters.ConfigFile)
			{
				case ConfigFile.Repository:
				case ConfigFile.Other:
					if(parameters.FileName != null)
					{
						args.Add(ConfigCommand.File(parameters.FileName));
					}
					break;
				case ConfigFile.System:
					args.Add(ConfigCommand.System());
					break;
				case ConfigFile.User:
					args.Add(ConfigCommand.Global());
					break;
			}
		}

		[DebuggerHidden]
		private static void HandleConfigResults(GitOutput output)
		{
			switch(output.ExitCode)
			{
				case 0:
					return;
				case 1:
					throw new InvalidConfigFileException();
				case 2:
					throw new CannotWriteConfigFileException();
				case 3:
					throw new NoSectionProvidedException();
				case 4:
					throw new InvalidSectionOrKeyException();
				case 5:
					throw new ConfigParameterDoesNotExistException();
				default:
					output.Throw();
					break;
			}
		}

		[DebuggerHidden]
		private static void HandleConfigResults(Task<GitOutput> task)
		{
			var output = TaskUtility.UnwrapResult(task);
			HandleConfigResults(output);
		}

		#region QueryConfigParameter

		private static Command GetQueryConfigParameterCommand(QueryConfigParameterParameters parameters)
		{
			Assert.IsNull(parameters);

			var args = new List<CommandArgument>(2);
			InsertConfigFileSpecifier(args, parameters);
			args.Add(new CommandArgument(parameters.ParameterName));
			return new ConfigCommand(args);
		}

		private static ConfigParameterData ParseQueryConfigParameterResult(QueryConfigParameterParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

			if(output.ExitCode == 0)
			{
				var value = output.Output.TrimEnd('\n');
				return new ConfigParameterData(parameters.ParameterName, value, parameters.ConfigFile, parameters.FileName);
			}
			else
			{
				return null;
			}
		}

		/// <summary>Query config parameter.</summary>
		/// <param name="parameters"><see cref="QueryConfigParameterParameters"/>.</param>
		/// <returns><see cref="ConfigParameterData"/> for requested parameter or null if parameter does not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static ConfigParameterData QueryConfigParameter(ICommandExecutor executor, QueryConfigParameterParameters parameters)
		{
			Verify.Argument.IsNotNull(executor, "executor");
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = GetQueryConfigParameterCommand(parameters);
			var output = executor.ExecuteCommand(command);
			return ParseQueryConfigParameterResult(parameters, output);
		}

		/// <summary>Query config parameter.</summary>
		/// <param name="parameters"><see cref="QueryConfigParameterParameters"/>.</param>
		/// <returns><see cref="ConfigParameterData"/> for requested parameter or null if parameter does not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static Task<ConfigParameterData> QueryConfigParameterAsync(ICommandExecutor executor, QueryConfigParameterParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(executor, "executor");
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = GetQueryConfigParameterCommand(parameters);
			return executor
				.ExecuteCommandAsync(command, cancellationToken)
				.ContinueWith(
				t =>
				{
					var output = TaskUtility.UnwrapResult(t);
					return ParseQueryConfigParameterResult(parameters, output);
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		#endregion

		#region QueryConfig

		private static Command GetQueryConfig(QueryConfigParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<CommandArgument>(3);
			args.Add(ConfigCommand.NullTerminate());
			args.Add(ConfigCommand.List());
			InsertConfigFileSpecifier(args, parameters);
			return new ConfigCommand(args);
		}

		private static IList<ConfigParameterData> ParseQueryConfigResults(QueryConfigParameters parameters, GitOutput output)
		{
			Assert.IsNotNull(parameters);
			Assert.IsNotNull(output);

			if(output.ExitCode != 0 && parameters.ConfigFile != ConfigFile.Other)
			{
				return new ConfigParameterData[0];
			}
			HandleConfigResults(output);
			var res = new List<ConfigParameterData>();
			var parser = new GitParser(output.Output);
			while(!parser.IsAtEndOfString)
			{
				var name = parser.ReadStringUpTo(parser.FindNewLineOrEndOfString(), 1);
				var value = parser.ReadStringUpTo(parser.FindNullOrEndOfString(), 1);
				if(parameters.ConfigFile != ConfigFile.Other)
				{
					res.Add(new ConfigParameterData(name, value, parameters.ConfigFile));
				}
				else
				{
					res.Add(new ConfigParameterData(name, value, parameters.FileName));
				}
			}
			return res;
		}

		/// <summary>Query configuration parameter list.</summary>
		/// <param name="parameters"><see cref="QueryConfigParameters"/>.</param>
		/// <returns>List of requested parameters.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static IList<ConfigParameterData> QueryConfig(ICommandExecutor executor, QueryConfigParameters parameters)
		{
			Verify.Argument.IsNotNull(executor, "executor");
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = GetQueryConfig(parameters);
			var output = executor.ExecuteCommand(command);
			return ParseQueryConfigResults(parameters, output);
		}

		/// <summary>Query configuration parameter list.</summary>
		/// <param name="parameters"><see cref="QueryConfigParameters"/>.</param>
		/// <returns>List of requested parameters.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static Task<IList<ConfigParameterData>> QueryConfigAsync(ICommandExecutor executor, QueryConfigParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(executor, "executor");
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = GetQueryConfig(parameters);
			return executor
				.ExecuteCommandAsync(command, cancellationToken)
				.ContinueWith(
				t =>
				{
					var output = TaskUtility.UnwrapResult(t);
					return ParseQueryConfigResults(parameters, output);
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		#endregion

		#region AddConfigValue

		private static Command GetAddConfigValueCommand(AddConfigValueParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<CommandArgument>(4);
			GitConfigHelper.InsertConfigFileSpecifier(args, parameters);
			args.Add(ConfigCommand.Add());
			args.Add(new CommandArgument(parameters.ParameterName));
			args.Add(new CommandArgument(parameters.ParameterValue.SurroundWith("\"", "\"")));
			return new ConfigCommand(args);
		}

		/// <summary>Add config value.</summary>
		/// <param name="parameters"><see cref="AddConfigValueParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static void AddConfigValue(ICommandExecutor executor, AddConfigValueParameters parameters)
		{
			Verify.Argument.IsNotNull(executor, "executor");
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = GetAddConfigValueCommand(parameters);
			var output = executor.ExecuteCommand(command);
			HandleConfigResults(output);
		}

		/// <summary>Add config value.</summary>
		/// <param name="parameters"><see cref="AddConfigValueParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static Task AddConfigValueAsync(ICommandExecutor executor, AddConfigValueParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(executor, "executor");
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = GetAddConfigValueCommand(parameters);
			return executor
				.ExecuteCommandAsync(command, cancellationToken)
				.ContinueWith(
					HandleConfigResults,
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
		}

		#endregion

		#region SetConfigValue

		private static Command GetSetConfigValueCommand(SetConfigValueParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<CommandArgument>(3);
			GitConfigHelper.InsertConfigFileSpecifier(args, parameters);
			args.Add(new CommandArgument(parameters.ParameterName));
			args.Add(new CommandArgument(parameters.ParameterValue.SurroundWith("\"", "\"")));
			return new ConfigCommand(args);
		}

		/// <summary>Set config value.</summary>
		/// <param name="parameters"><see cref="SetConfigValueParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static void SetConfigValue(ICommandExecutor executor, SetConfigValueParameters parameters)
		{
			Verify.Argument.IsNotNull(executor, "executor");
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = GetSetConfigValueCommand(parameters);
			var output = executor.ExecuteCommand(command);
			HandleConfigResults(output);
		}

		/// <summary>Set config value.</summary>
		/// <param name="parameters"><see cref="SetConfigValueParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static Task SetConfigValueAsync(ICommandExecutor executor, SetConfigValueParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(executor, "executor");
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = GetSetConfigValueCommand(parameters);
			return executor
				.ExecuteCommandAsync(command, cancellationToken)
				.ContinueWith(
					HandleConfigResults,
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
		}

		#endregion

		#region UnsetConfigValue

		private static Command GetUnsetConfigValueCommand(UnsetConfigValueParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<CommandArgument>(3);
			GitConfigHelper.InsertConfigFileSpecifier(args, parameters);
			args.Add(ConfigCommand.Unset());
			args.Add(new CommandArgument(parameters.ParameterName));
			return new ConfigCommand(args);
		}

		/// <summary>Unset config parameter value.</summary>
		/// <param name="parameters"><see cref="UnsetConfigValueParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static void UnsetConfigValue(ICommandExecutor executor, UnsetConfigValueParameters parameters)
		{
			Verify.Argument.IsNotNull(executor, "executor");
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = GetUnsetConfigValueCommand(parameters);
			var output = executor.ExecuteCommand(command);
			GitConfigHelper.HandleConfigResults(output);
		}

		/// <summary>Unset config parameter value.</summary>
		/// <param name="parameters"><see cref="UnsetConfigValueParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static Task UnsetConfigValueAsync(ICommandExecutor executor, UnsetConfigValueParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(executor, "executor");
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = GetUnsetConfigValueCommand(parameters);
			return executor
				.ExecuteCommandAsync(command, cancellationToken)
				.ContinueWith(
					HandleConfigResults,
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
		}

		#endregion

		#region RenameConfigSection

		private static Command GetRenameConfigSectionCommand(RenameConfigSectionParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<CommandArgument>(2);
			GitConfigHelper.InsertConfigFileSpecifier(args, parameters);
			args.Add(ConfigCommand.RenameSection(parameters.OldName, parameters.NewName));
			return new ConfigCommand(args);
		}

		/// <summary>Rename configuration section.</summary>
		/// <param name="parameters"><see cref="RenameConfigSectionParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static void RenameConfigSection(ICommandExecutor executor, RenameConfigSectionParameters parameters)
		{
			Verify.Argument.IsNotNull(executor, "executor");
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = GetRenameConfigSectionCommand(parameters);
			var output = executor.ExecuteCommand(command);
			GitConfigHelper.HandleConfigResults(output);
		}

		/// <summary>Rename configuration section.</summary>
		/// <param name="parameters"><see cref="RenameConfigSectionParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static Task RenameConfigSectionAsync(ICommandExecutor executor, RenameConfigSectionParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(executor, "executor");
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = GetRenameConfigSectionCommand(parameters);
			return executor
				.ExecuteCommandAsync(command, cancellationToken)
				.ContinueWith(
					HandleConfigResults,
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
		}

		#endregion

		#region DeleteConfigSection

		private static Command GetDeleteConfigSectionCommand(DeleteConfigSectionParameters parameters)
		{
			Assert.IsNotNull(parameters);

			var args = new List<CommandArgument>(2);
			GitConfigHelper.InsertConfigFileSpecifier(args, parameters);
			args.Add(ConfigCommand.RemoveSection(parameters.SectionName));
			return new ConfigCommand(args);
		}

		/// <summary>Delete configuration section.</summary>
		/// <param name="parameters"><see cref="DeleteConfigSectionParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static void DeleteConfigSection(ICommandExecutor executor, DeleteConfigSectionParameters parameters)
		{
			Verify.Argument.IsNotNull(executor, "executor");
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = GetDeleteConfigSectionCommand(parameters);
			var output = executor.ExecuteCommand(command);
			GitConfigHelper.HandleConfigResults(output);
		}

		/// <summary>Delete configuration section.</summary>
		/// <param name="parameters"><see cref="DeleteConfigSectionParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static Task DeleteConfigSectionAsync(ICommandExecutor executor, DeleteConfigSectionParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(executor, "executor");
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = GetDeleteConfigSectionCommand(parameters);
			return executor
				.ExecuteCommandAsync(command, cancellationToken)
				.ContinueWith(
					HandleConfigResults,
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
		}

		#endregion
	}
}
