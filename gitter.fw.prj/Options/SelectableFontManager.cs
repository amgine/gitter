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
				() => new Font("Consolas", 9.0f, FontStyle.Regular, GraphicsUnit.Point)));
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
