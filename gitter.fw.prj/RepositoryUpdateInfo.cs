namespace gitter.Framework
{
	using System;
	using System.Drawing;

	public sealed class RepositoryUpdateInfo
	{
		private readonly Image _icon;
		private readonly string _name;

		public RepositoryUpdateInfo(string name, Image icon)
		{
			_name = name;
			_icon = icon;
		}

		public Image Icon
		{
			get { return _icon;}
		}

		public string Name
		{
			get { return _name; }
		}
	}

	public sealed class UpdatedItem
	{
		private readonly Image _icon;
		private readonly string _name;

		public UpdatedItem(string name, Image icon)
		{
			_name = name;
			_icon = icon;
		}

		public Image Icon
		{
			get { return _icon; }
		}

		public string Name
		{
			get { return _name; }
		}

		public override string ToString()
		{
			return _name;
		}
	}
}
