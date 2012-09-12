namespace gitter.Framework.Services
{
	using System;

	using gitter.Framework.Configuration;

	public sealed class RepositoryLink
	{
		public event EventHandler Deleted;

		private readonly string _path;
		private readonly string _type;
		private string _description;

		public RepositoryLink(Section section)
		{
			Verify.Argument.IsNotNull(section, "section");

			_path = section.GetValue("Path", string.Empty);
			_type = section.GetValue("Type", string.Empty);
			_description = section.GetValue("Description", string.Empty);
		}

		public RepositoryLink(string path, string type)
		{
			_path = path;
			_type = type;
		}

		internal void InvokeDeleted()
		{
			var handler = Deleted;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		public string Path
		{
			get { return _path; }
		}

		public string Type
		{
			get { return _type; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public void SaveTo(Section section)
		{
			Verify.Argument.IsNotNull(section, "section");

			section.SetValue("Path", _path);
			section.SetValue("Type", _type);
			section.SetValue("Description", _description);
		}
	}
}
