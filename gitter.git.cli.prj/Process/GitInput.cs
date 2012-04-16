namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>Input data for git.exe.</summary>
	internal sealed class GitInput
	{
		#region Data

		private readonly string _workingDirectory;
		private readonly Command _command;
		private readonly Encoding _encoding;
		private IDictionary<string, string> _environment;
		private IList<CommandArgument> _options;

		#endregion

		#region .ctor

		public GitInput(IList<CommandArgument> options)
			: this(null, null, GitProcess.DefaultEncoding, options)
		{
		}

		public GitInput(Command command)
			: this(null, command, GitProcess.DefaultEncoding, null)
		{
		}

		public GitInput(Command command, Encoding encoding)
			: this(null, command, encoding, null)
		{
		}

		public GitInput(string workingDirectory, Command command)
			: this(workingDirectory, command, GitProcess.DefaultEncoding, null)
		{
		}

		public GitInput(string workingDirectory, Command command, Encoding encoding)
			: this(workingDirectory, command, encoding, null)
		{
		}

		public GitInput(string workingDirectory, IList<CommandArgument> options)
			: this(workingDirectory, null, GitProcess.DefaultEncoding, options)
		{
		}

		public GitInput(string workingDirectory, Command command, IList<CommandArgument> options)
			: this(workingDirectory, command, GitProcess.DefaultEncoding, options)
		{
		}

		public GitInput(string workingDirectory, Command command, Encoding encoding, IList<CommandArgument> options)
		{
			if(encoding == null) throw new ArgumentNullException("encoding");

			_workingDirectory = workingDirectory;
			_command = command;
			_options = options;
			_encoding = encoding;
		}

		#endregion

		#region Properties

		public string WorkingDirectory
		{
			get { return _workingDirectory; }
		}

		public Encoding Encoding
		{
			get { return _encoding; }
		}

		public IList<CommandArgument> Options
		{
			get { return _options; }
			set { _options = value; }
		}

		public Command Command
		{
			get { return _command; }
		}

		public IDictionary<string, string> Environment
		{
			get { return _environment; }
			set { _environment = value; }
		}

		#endregion

		#region Methods

		public string GetArguments()
		{
			var sb = new StringBuilder();
			if(_options != null && _options.Count != 0)
			{
				foreach(var opt in _options)
				{
					opt.GetArgument(sb);
				}
			}
			if(_command != null)
			{
				_command.GetCommand(sb);
			}
			return sb.ToString();
		}

		#endregion
	}
}
