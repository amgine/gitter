namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Text;

	/// <summary>Command argument.</summary>
	public class CommandArgument
	{
		#region Data

		private readonly string _name;
		private readonly char _separator;
		private string _value;

		#endregion

		#region Static

		public static CommandArgument DryRun()
		{
			return new CommandArgument("--dry-run");
		}

		public static CommandArgument Verbose()
		{
			return new CommandArgument("--verbose");
		}

		public static CommandArgument Quiet()
		{
			return new CommandArgument("--quiet");
		}

		public static CommandArgument SignOff()
		{
			return new CommandArgument("--signoff");
		}

		public static CommandArgument Interactive()
		{
			return new CommandArgument("--interactive");
		}

		/// <summary>Do not interpret any more arguments as options.</summary>
		public static CommandArgument NoMoreOptions()
		{
			return new CommandArgument("--");
		}

		#endregion

		#region .ctor

		public CommandArgument(string name)
		{
			_name = name;
			_separator = '=';
		}

		public CommandArgument(string name, string value)
			: this(name)
		{
			_value = value;
		}

		public CommandArgument(string name, string value, char separator)
			: this(name, value)
		{
			_separator = separator;
		}

		#endregion

		#region Properties

		public string Name
		{
			get { return _name; }
		}

		public char Separator
		{
			get { return _separator; }
		}

		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		#endregion

		#region Methods

		public string GetArgument()
		{
			if(string.IsNullOrEmpty(_value)) return _name;
			return _name + _separator + _value;
		}

		public void GetArgument(StringBuilder stringBuilder)
		{
			stringBuilder.Append(_name);
			if(!string.IsNullOrEmpty(_value))
			{
				stringBuilder.Append(_separator);
				stringBuilder.Append(_value);
			}
		}

		#endregion

		#region Overrides

		public override string ToString()
		{
			return GetArgument();
		}

		#endregion
	}

	public class PathCommandArgument : CommandArgument
	{
		public PathCommandArgument(string path)
			: base(path.AssureDoubleQuotes())
		{
		}
	}
}
