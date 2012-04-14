namespace gitter.Updater
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public sealed class CommandLine
	{
		private readonly Dictionary<string, CommandLineParameter> _parameters;

		public CommandLine()
		{
			_parameters = new Dictionary<string, CommandLineParameter>();

			var args = Environment.GetCommandLineArgs();
			for(int i = 1; i < args.Length; ++i)
			{
				var arg = args[i].Trim('"');
				if(arg.Length > 1 && arg.StartsWith("/"))
				{
					var colonIndex = arg.IndexOf(':');
					if(colonIndex != -1)
					{
						if(colonIndex > 1)
						{
							string value;
							var name = arg.Substring(1, colonIndex - 1);
							if(colonIndex < arg.Length - 1)
							{
								value = arg.Substring(colonIndex + 1);
							}
							else
							{
								value = string.Empty;
							}
							_parameters[name] = new CommandLineParameter(name, value);
						}
					}
					else
					{
						var name = arg.Substring(1);
						_parameters[name] = new CommandLineParameter(name, null);
					}
				}
			}
		}

		public string this[string name]
		{
			get
			{
				CommandLineParameter p;
				if(_parameters.TryGetValue(name, out p))
				{
					return p.Value;
				}
				return null;
			}
		}

		public bool IsDefined(string name)
		{
			return _parameters.ContainsKey(name);
		}
	}

	public sealed class CommandLineParameter
	{
		private readonly string _value;
		private readonly string _name;

		public CommandLineParameter(string name, string value)
		{
			_name = name;
			_value = value;
		}

		public string Name
		{
			get { return _name; }
		}

		public string Value
		{
			get { return _value; }
		}
	}
}
