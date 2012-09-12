namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using gitter.Git.AccessLayer.CLI;

	partial class RepositoryCLI : ISubmoduleAccessor
	{
		/// <summary>Updates submodule.</summary>
		/// <param name="parameters"><see cref="SubmoduleUpdateParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void UpdateSubmodule(SubmoduleUpdateParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>();

			args.Add(SubmoduleCommand.Update());
			if(parameters.Init)
			{
				args.Add(SubmoduleCommand.InitFlag());
			}
			if(parameters.NoFetch)
			{
				args.Add(SubmoduleCommand.NoFetch());
			}
			switch(parameters.Mode)
			{
				case SubmoduleUpdateMode.Merge:
					args.Add(SubmoduleCommand.Merge());
					break;
				case SubmoduleUpdateMode.Rebase:
					args.Add(SubmoduleCommand.Rebase());
					break;
			}
			if(parameters.Recursive)
			{
				args.Add(SubmoduleCommand.Recursive());
			}
			if(!string.IsNullOrEmpty(parameters.Path))
			{
				args.Add(SubmoduleCommand.NoMoreOptions());
				args.Add(new PathCommandArgument(parameters.Path));
			}

			var cmd = new SubmoduleCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>Adds new submodule.</summary>
		/// <param name="parameters"><see cref="AddSubmoduleParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void AddSubmodule(AddSubmoduleParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>();

			args.Add(SubmoduleCommand.Add());
			if(parameters.Branch != null)
			{
				args.Add(SubmoduleCommand.Branch(parameters.Branch));
			}
			if(parameters.Force)
			{
				args.Add(SubmoduleCommand.Force());
			}
			if(parameters.ReferenceRepository != null)
			{
				args.Add(SubmoduleCommand.Reference(parameters.ReferenceRepository));
			}
			args.Add(CommandArgument.NoMoreOptions());
			args.Add(new CommandArgument(parameters.Repository));
			if(parameters.Path != null)
			{
				args.Add(new PathCommandArgument(parameters.Path));
			}

			var cmd = new SubmoduleCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}
	}
}
