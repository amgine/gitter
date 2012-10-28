namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework.Configuration;

	public sealed class IntegrationFeatures : IEnumerable<IIntegrationFeature>
	{
		private readonly Dictionary<string, IIntegrationFeature> _features;
		private readonly GravatarFeature _gravatar;

		/// <summary>Create <see cref="IntegrationFeatures"/>.</summary>
		internal IntegrationFeatures()
		{
			_gravatar = new GravatarFeature();
			var explorerContextMenu = new ExplorerContextMenuFeature();
			_features = new Dictionary<string, IIntegrationFeature>()
			{
				{ explorerContextMenu.Name, explorerContextMenu },
				{ _gravatar.Name, _gravatar },
			};
		}

		/// <summary>Gravatar integration support.</summary>
		public GravatarFeature Gravatar
		{
			get { return _gravatar; }
		}

		public IIntegrationFeature this[string name]
		{
			get { return _features[name]; }
		}

		public int Count
		{
			get { return _features.Count; }
		}

		public void SaveTo(Section section)
		{
			Verify.Argument.IsNotNull(section, "section");

			foreach(var feature in _features.Values)
			{
				if(feature.HasConfiguration)
				{
					var featureNode = section.GetCreateSection(feature.Name);
					feature.SaveTo(featureNode);
				}
			}
		}

		public void LoadFrom(Section section)
		{
			Verify.Argument.IsNotNull(section, "section");

			if(section != null)
			{
				foreach(var featureNode in section.Sections)
				{
					IIntegrationFeature feature;
					if(_features.TryGetValue(featureNode.Name, out feature) && feature.HasConfiguration)
					{
						feature.LoadFrom(featureNode);
					}
				}
			}
		}

		#region IEnumerable<IIntegrationFeature> Members

		public IEnumerator<IIntegrationFeature> GetEnumerator()
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
