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

namespace gitter.Framework.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>Configuration section.</summary>
	public sealed class Section : INamedObject
	{
		#region Data

		/// <summary>Subsections dictionary.</summary>
		private readonly Dictionary<string, Section> _sections = new();
		/// <summary>Parameters dictionary.</summary>
		private readonly Dictionary<string, Parameter> _parameters = new();

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="Section"/> class.</summary>
		/// <param name="name">Section name.</param>
		public Section(string name) => Name = name;

		#endregion

		/// <summary>Gets section name.</summary>
		/// <value>Section name.</value>
		public string Name { get; }

		/// <summary>Gets a value indicating whether this section is modified.</summary>
		/// <value><c>true</c> if this section is modified; otherwise, <c>false</c>.</value>
		public bool IsModified { get; private set; }

		#region Sub-section Management

		/// <summary>Gets list of all subsections.</summary>
		/// <value>List of all subsections.</value>
		public IEnumerable<Section> Sections => _sections.Values;

		/// <summary>Gets the section count.</summary>
		/// <value>The section count.</value>
		public int SectionCount => _sections.Count;

		/// <summary>Determines whether this section contains subsection with the specified name.</summary>
		/// <param name="name">Subsection name.</param>
		/// <returns><c>true</c> if this section contains subsection with the specified name; otherwise, <c>false</c>.</returns>
		public bool ContainsSection(string name)
			=> _sections.ContainsKey(name);

		/// <summary>Removes all subsections.</summary>
		public void ClearSections()
		{
			if(_sections.Count != 0)
			{
				_sections.Clear();
				IsModified = true;
			}
		}

		/// <summary>
		/// Gets empty subsection with specified name. If subsection does not exist, creates it.
		/// </summary>
		/// <param name="name">Subsection name.</param>
		/// <returns>Subsection with specified name.</returns>
		public Section GetCreateEmptySection(string name)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, nameof(name));

			if(!_sections.TryGetValue(name, out var section))
			{
				section = new Section(name);
				_sections.Add(name, section);
			}
			else
			{
				section.Clear();
			}
			return section;
		}

		/// <summary>
		/// Gets subsection with specified name. If subsection does not exist, creates it.
		/// </summary>
		/// <param name="name">Subsection name.</param>
		/// <returns>Subsection with specified name.</returns>
		public Section GetCreateSection(string name)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, nameof(name));

			if(!_sections.TryGetValue(name, out var section))
			{
				section = new Section(name);
				_sections.Add(name, section);
			}
			return section;
		}

		/// <summary>Gets subsection with the specified name.</summary>
		/// <param name="name">Subsection name.</param>
		/// <returns>Subsection with the specified name</returns>
		public Section GetSection(string name)
		{
			Verify.Argument.IsTrue(
				_sections.TryGetValue(name, out var section),
				nameof(name), "Section not found.");
			return section;
		}

		public Section TryGetSection(string name)
			=> _sections.TryGetValue(name, out var section)
				? section
				: default;

		public bool TryGetSection(string name, out Section section)
			=> _sections.TryGetValue(name, out section);

		public void AddSection(Section section)
		{
			Verify.Argument.IsNotNull(section, nameof(section));

			_sections.Add(section.Name, section);
		}

		public Section CreateSection(string name)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, nameof(name));

			var section = new Section(name);
			_sections.Add(name, section);
			IsModified = true;
			return section;
		}

		#endregion

		#region Parameter Management

		/// <summary>Gets list of all parameters of this section.</summary>
		/// <value>List of all parameters of this section.</value>
		public IEnumerable<Parameter> Parameters => _parameters.Values;

		/// <summary>Gets the parameter count.</summary>
		/// <value>Parameter count.</value>
		public int ParameterCount => _parameters.Count;

		public Parameter GetParameter(string name) => _parameters[name];

		public void AddParameter(Parameter parameter)
		{
			Verify.Argument.IsNotNull(parameter, nameof(parameter));

			_parameters.Add(parameter.Name, parameter);
		}

		public bool ContainsParameter(string name)
			=> _parameters.ContainsKey(name);

		/// <summary>Removes all parameters in this section.</summary>
		public void ClearParameters()
		{
			if(_parameters.Count != 0)
			{
				_parameters.Clear();
				IsModified = true;
			}
		}

		public Parameter TryGetParameter(string name)
			=> _parameters.TryGetValue(name, out var parameter)
				? parameter
				: default;

		public bool TryGetParameter(string name, out Parameter parameter)
			=> _parameters.TryGetValue(name, out parameter);

		public void SetValue<T>(string name, T value)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, nameof(name));

			if(!_parameters.TryGetValue(name, out var parameter))
			{
				parameter = new Parameter(name,
					TypeHelpers.GetType<T>(value),
					TypeHelpers.PackValue<T>(value));
				_parameters.Add(name, parameter);
				IsModified = true;
			}
			else
			{
				parameter.Value = TypeHelpers.PackValue<T>(value);
				IsModified = true;
			}
		}

		public T GetValue<T>(string name, T defaultValue)
			=> _parameters.TryGetValue(name, out var parameter)
				? TypeHelpers.UnpackValue<T>(parameter.Value)
				: defaultValue;

		public T GetValue<T>(string name)
			=> GetValue<T>(name, default);

		#endregion

		/// <summary>Removes all sections and parameters.</summary>
		public void Clear()
		{
			ClearSections();
			ClearParameters();
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString() => Name;
	}
}
