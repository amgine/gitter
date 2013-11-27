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

namespace gitter.Framework.Options
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Configuration;

	using Resources = gitter.Framework.Properties.Resources;

	public sealed class SelectableFont
	{
		#region Data

		private readonly string _id;
		private readonly string _name;
		private Font _font;

		#endregion

		#region Events

		public event EventHandler Changed;

		#endregion

		#region .ctor

		public SelectableFont(string id, string name, Font font)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");
			Verify.Argument.IsNotNull(font, "font");

			_id = id;
			_name = name;
			_font = font;
		}

		public SelectableFont(string id, string name, Section section)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");
			Verify.Argument.IsNotNull(section, "section");

			var fontName	= section.GetValue<string>("Name", null);
			var size		= section.GetValue<float>("Size", 0);
			var style		= section.GetValue<FontStyle>("Style", FontStyle.Regular);

			Verify.Argument.IsTrue(fontName != null, "section", "Section does not contain a valid font name.");
			Verify.Argument.IsTrue(size > 0, "section", "Section contains invalid font size.");

			_font	= new Font(fontName, size, style, GraphicsUnit.Point);
			_id		= id;
			_name	= name;
		}

		#endregion

		#region Properties

		public string Id
		{
			get { return _id; }
		}

		public string Name
		{
			get { return _name; }
		}

		public Font Font
		{
			get { return _font; }
			set
			{
				Verify.Argument.IsNotNull(value, "value");

				if(_font != value)
				{
					_font = value;
					Changed.Raise(this);
				}
			}
		}

		#endregion

		public void Apply(params Control[] controls)
		{
			Verify.Argument.IsNotNull(controls, "controls");
			Verify.Argument.HasNoNullItems(controls, "controls");

			ApplyCore(controls);
		}

		public void Apply(IEnumerable<Control> controls)
		{
			Verify.Argument.IsNotNull(controls, "controls");
			Verify.Argument.HasNoNullItems(controls, "controls");

			ApplyCore(controls);
		}

		private void ApplyCore(IEnumerable<Control> controls)
		{
			Assert.IsNotNull(controls);

			foreach(var control in controls)
			{
				control.Font = _font;
			}
		}

		public static implicit operator Font(SelectableFont font)
		{
			return font._font;
		}

		public void SaveTo(Section section)
		{
			Verify.Argument.IsNotNull(section, "section");

			section.SetValue("Name", _font.Name);
			section.SetValue("Size", _font.SizeInPoints);
			section.SetValue("Style", _font.Style);
		}

		public void LoadFrom(Section section)
		{
			Verify.Argument.IsNotNull(section, "section");

			var name	= section.GetValue<string>("Name", null);
			var size	= section.GetValue<float>("Size", 0);
			var style	= section.GetValue<FontStyle>("Name", FontStyle.Regular);

			Assert.IsNeitherNullNorWhitespace(name);
			Assert.BoundedDoubleInc(0, size, 100);

			if(_font.Name != name || _font.Size != size || _font.Style != style)
			{
				Font font = null;
				try
				{
					font = new Font(name, size, style, GraphicsUnit.Point);
				}
				catch(Exception exc)
				{
					if(exc.IsCritical())
					{
						throw;
					}
				}
				if(font != null)
				{
					_font = font;
				}
				Changed.Raise(this);
			}
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return _name;
		}
	}
}
