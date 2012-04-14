namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	public sealed class StaticRepositoryAction : INamedObject
	{
		#region Data

		private readonly string _name;
		private readonly string _displayName;
		private readonly Image _icon;
		private readonly Action<IWorkingEnvironment> _execute;

		#endregion

		#region .ctor

		public StaticRepositoryAction(string name, string displayName, Image icon, Action<IWorkingEnvironment> execute)
		{
			_name = name;
			_displayName = displayName;
			_execute = execute;
			_icon = icon;
		}

		#endregion

		#region Propertes

		public string Name
		{
			get { return _name; }
		}

		public string DisplayName
		{
			get { return _displayName; }
		}

		public Image Icon
		{
			get { return _icon; }
		}

		#endregion

		#region Methods

		public void Execute(IWorkingEnvironment env)
		{
			_execute(env);
		}

		#endregion
	}
}
