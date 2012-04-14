namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Text;
	using System.Collections.Generic;

	/// <summary>Represents git command line command.</summary>
	public class Command
	{
		protected static readonly IEnumerable<CommandArgument> NoArguments =
			new CommandArgument[0];

		private readonly IEnumerable<CommandArgument> _arguments;
		private readonly string _name;

		/// <summary>
		/// Initializes a new instance of the <see cref="Command"/> class.
		/// </summary>
		/// <param name="name">Command name.</param>
		/// <param name="arguments">Command arguments.</param>
		public Command(string name, IEnumerable<CommandArgument> arguments)
		{
			if(string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Invalid command name.", "name");
			}

			_name = name;
			if(arguments == null)
			{
				_arguments = NoArguments;
			}
			else
			{
				_arguments = arguments;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Command"/> class.
		/// </summary>
		/// <param name="name">Command name.</param>
		public Command(string name)
			: this(name, null)
		{
		}

		/// <summary>Returns command name.</summary>
		/// <value>Command name.</value>
		public string Name
		{
			get { return _name; }
		}
		
		/// <summary>Returns list of command arguments.</summary>
		/// <value>List of command arguments.</value>
		public IEnumerable<CommandArgument> Arguments
		{
			get { return _arguments; }
		}

		/// <summary>Returns string representation of command with arguments.</summary>
		/// <returns>String representation of command with arguments.</returns>
		public string GetCommand()
		{
			var sb = new StringBuilder();
			GetCommand(sb);
			return sb.ToString();
		}

		/// <summary>Appends command with all arguments to a specified <paramref name="stringBuilder"/>.</summary>
		/// <param name="stringBuilder"><see cref="StringBuilder"/> which will receive command string representation.</param>
		/// <exception cref="ArgumentNullException"><paramref name="stringBuilder"/> == <c>null</c>.</exception>
		public void GetCommand(StringBuilder stringBuilder)
		{
			if(stringBuilder == null) throw new ArgumentNullException("stringBuilder");

			const char ArgumentSeparator = ' ';

			stringBuilder.Append(Name);
			foreach(var arg in Arguments)
			{
				stringBuilder.Append(ArgumentSeparator);
				arg.GetArgument(stringBuilder);
			}
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this <see cref="Command"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this <see cref="Command"/>.
		/// </returns>
		public override string ToString()
		{
			return GetCommand();
		}
	}
}
