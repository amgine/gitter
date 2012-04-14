namespace gitter.Framework.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public sealed class ConfigurationManager : INamedObject
	{
		private Section _rootSection;

		public ConfigurationManager(IDataAdapter dataAdapter)
		{
			Load(dataAdapter);
		}

		public ConfigurationManager(string name)
		{
			_rootSection = new Section(name);
		}

		public string Name
		{
			get { return _rootSection.Name; }
		}

		public Section RootSection
		{
			get { return _rootSection; }
		}

		public void Save(IDataAdapter dataAdapter)
		{
			if(dataAdapter == null) throw new ArgumentNullException("dataAdapter");
			dataAdapter.Store(_rootSection);
		}

		public void Load(IDataAdapter dataAdapter)
		{
			if(dataAdapter == null) throw new ArgumentNullException("dataAdapter");
			_rootSection = dataAdapter.Load();
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
