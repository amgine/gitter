namespace gitter.Framework.Options
{
	using System;
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
			if(font == null) throw new ArgumentNullException("font");

			_id = id;
			_name = name;
			_font = font;
		}

		public SelectableFont(string id, string name, Section section)
		{
			if(section == null)
				throw new ArgumentNullException("section");

			var fontName = section.GetValue<string>("Name", null);
			if(fontName == null) throw new ArgumentException("section");
			var size = section.GetValue<float>("Size", 0);
			if(size <= 0) throw new ArgumentException("section");
			var style = section.GetValue<FontStyle>("Style", FontStyle.Regular);
			_font = new Font(fontName, size, style, GraphicsUnit.Point);
			_id = id;
			_name = name;
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
				if(value == null) throw new ArgumentNullException("value");
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
			if(controls == null) throw new ArgumentNullException("controls");
			for(int i = 0; i < controls.Length; ++i)
				controls[i].Font = _font;
		}

		public static implicit operator Font(SelectableFont font)
		{
			return font._font;
		}

		public void SaveTo(Section section)
		{
			if(section == null)
				throw new ArgumentNullException("section");

			section.SetValue("Name", _font.Name);
			section.SetValue("Size", _font.SizeInPoints);
			section.SetValue("Style", _font.Style);
		}

		public void LoadFrom(Section section)
		{
			if(section == null)
				throw new ArgumentNullException("section");

			var name = section.GetValue<string>("Name", null);
			var size = section.GetValue<float>("Size", 0);
			var style = section.GetValue<FontStyle>("Name", FontStyle.Regular);

			if(_font.Name != name || _font.Size != size || _font.Style != style)
			{
				Font font = null;
				try
				{
					font = new Font(name, size, style, GraphicsUnit.Point);
				}
				catch { }
				if(font != null) _font = font;
				Changed.Raise(this);
			}
		}

		public override string ToString()
		{
			return _name;
		}
	}
}
