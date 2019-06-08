#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework.Configuration;

	public sealed class IntegrationFeatures : IEnumerable<IIntegrationFeature>
	{
		private readonly Dictionary<string, IIntegrationFeature> _features;

		/// <summary>Create <see cref="IntegrationFeatures"/>.</summary>
		internal IntegrationFeatures()
		{
			Gravatar = new GravatarFeature();
			var explorerContextMenu = new ExplorerContextMenuFeature();
			_features = new Dictionary<string, IIntegrationFeature>()
			{
				{ explorerContextMenu.Name, explorerContextMenu },
				{ Gravatar.Name, Gravatar },
			};
		}

		/// <summary>Gravatar integration support.</summary>
		public GravatarFeature Gravatar { get; }

		public IIntegrationFeature this[string name] => _features[name];

		public int Count => _features.Count;

		public void SaveTo(Section section)
		{
			Verify.Argument.IsNotNull(section, nameof(section));

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
			Verify.Argument.IsNotNull(section, nameof(section));

			if(section != null)
			{
				foreach(var featureNode in section.Sections)
				{
					if(_features.TryGetValue(featureNode.Name, out var feature) && feature.HasConfiguration)
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
