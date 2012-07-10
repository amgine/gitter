namespace gitter.Git.Integration
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework.Configuration;

	public sealed class IntegrationFeatures : IEnumerable<IntegrationFeature>
	{
		private readonly Dictionary<string, IntegrationFeature> _features;
		private readonly Gravatar _gravatar;

		/// <summary>Create <see cref="IntegrationFeatures"/>.</summary>
		internal IntegrationFeatures()
		{
			_gravatar = new Gravatar();
			_features = new Dictionary<string, IntegrationFeature>()
			{
				{ _gravatar.Name, _gravatar },
			};
		}

		/// <summary>Gravatar integration support.</summary>
		public Gravatar Gravatar
		{
			get { return _gravatar; }
		}

		public IntegrationFeature this[string name]
		{
			get { return _features[name]; }
		}

		public int Count
		{
			get { return _features.Count; }
		}

		public void SaveTo(Section section)
		{
			foreach(var feature in _features.Values)
			{
				var featureNode = section.GetCreateSection(feature.Name);
				feature.SaveTo(featureNode);
			}
		}

		public void LoadFrom(Section section)
		{
			if(section != null)
			{
				foreach(var featureNode in section.Sections)
				{
					IntegrationFeature feature;
					if(_features.TryGetValue(featureNode.Name, out feature))
					{
						feature.LoadFrom(featureNode);
					}
				}
			}
		}

		#region IEnumerable<IntegrationFeature> Members

		public IEnumerator<IntegrationFeature> GetEnumerator()
		{
			return _features.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _features.Values.GetEnumerator();
		}

		#endregion
	}
}
