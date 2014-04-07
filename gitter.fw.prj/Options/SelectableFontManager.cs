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

	using gitter.Framework.Configuration;

	using Resources = gitter.Framework.Properties.Resources;

	public sealed class SelectableFontManager : IEnumerable<SelectableFont>
	{
		public const string IdFontUI = "FontUI";
		public const string IdFontInput = "FontInput";
		public const string IdFontViewer = "FontViewer";

		private readonly Section _section;
		private readonly Dictionary<string, SelectableFont> _fonts;

		private SelectableFont _uiFont;
		private SelectableFont _inputFont;
		private SelectableFont _viewerFont;

		private static SelectableFont TryLoadFont(Section section, string id, string name, Func<Font> defaultFont)
		{
			section = section.TryGetSection(id);
			if(section != null)
			{
				SelectableFont font;
				try
				{
					font = new SelectableFont(id, name, section);
				}
				catch
				{
					font = new SelectableFont(id, name, defaultFont());
				}
				return font;
			}
			else
			{
				return new SelectableFont(id, name, defaultFont());
			}
		}

		internal SelectableFontManager(Section section)
		{
			_fonts = new Dictionary<string, SelectableFont>();
			_section = section;
			LoadStandardFonts();
		}

		private void LoadStandardFonts()
		{
			_fonts.Add(IdFontUI, _uiFont = TryLoadFont(_section, IdFontUI, Resources.StrUIFont,
				() => new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point)));
			_fonts.Add(IdFontInput, _inputFont = TryLoadFont(_section, IdFontInput, Resources.StrInputFont,
				() => new Font("Consolas", 10.0f, FontStyle.Regular, GraphicsUnit.Point)));
			_fonts.Add(IdFontViewer, _viewerFont = TryLoadFont(_section, IdFontViewer, Resources.StrViewerFont,
				() => new Font("Consolas", 9.0f, FontStyle.Regular, GraphicsUnit.Point)));
		}

		public SelectableFont RegisterFont(string id, string name, Func<Font> defaultFont)
		{
			SelectableFont font;
			if(!_fonts.TryGetValue(id, out font))
			{
				font = TryLoadFont(_section, id, name, defaultFont);
			}
			return font;
		}

		public SelectableFont UIFont
		{
			get { return _uiFont; }
		}

		public SelectableFont InputFont
		{
			get { return _inputFont; }
		}

		public SelectableFont ViewerFont
		{
			get { return _viewerFont; }
		}

		public SelectableFont this[string name]
		{
			get { return _fonts[name]; }
		}

		internal void Save()
		{
			foreach(var font in _fonts.Values)
			{
				var section = _section.GetCreateSection(font.Id);
				font.SaveTo(section);
			}
		}

		public IEnumerator<SelectableFont> GetEnumerator()
		{
			return _fonts.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _fonts.Values.GetEnumerator();
		}
	}
}
