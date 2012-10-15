namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	public sealed class GuiCommand : INamedObject
	{
		#region Data

		private readonly string _name;
		private readonly string _displayName;
		private readonly Image _image;
		private readonly Action<IWorkingEnvironment> _execute;

		#endregion

		#region .ctor

		public GuiCommand(string name, string displayName, Image image, Action<IWorkingEnvironment> execute)
		{
			_name = name;
			_displayName = displayName;
			_execute = execute;
			_image = image;
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

		public Image Image
		{
			get { return _image; }
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
