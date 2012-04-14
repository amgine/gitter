namespace gitter.Framework.Options
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;

	public sealed class SelectableColor : IDisposable
	{
		#region data

		private readonly string _id;
		private readonly string _name;
		private readonly string _categoryId;
		private Color _color;
		private Brush _brush;
		private Pen _pen;

		#endregion

		#region Events

		public event EventHandler Changed;

		#endregion

		#region .ctor

		public SelectableColor(string id, string name, Color color, string categoryId)
		{
			_id = id;
			_name = name;
			_color = color;
			_brush = new SolidBrush(color);
			_pen = new Pen(color);
			_categoryId = categoryId;
		}

		#endregion

		#region .ctor

		public SelectableColor(string id, string name, Color color)
		{
			_id = id;
			_name = name;
			_color = color;
			_brush = new SolidBrush(color);
			_pen = new Pen(color);
		}

		~SelectableColor()
		{
			Dispose(false);
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

		public string CategoryId
		{
			get { return _categoryId; }
		}

		public Color Color
		{
			get { return _color; }
			set
			{
				if(_color != value)
				{
					_color = value;
					if(_brush != null)
						_brush.Dispose();
					if(_pen != null)
						_pen.Dispose();
					_brush = new SolidBrush(value);
					_pen = new Pen(value);
					Changed.Raise(this);
				}
			}
		}

		public Brush Brush
		{
			get { return _brush; }
		}

		public Pen Pen
		{
			get { return _pen; }
		}

		#endregion

		public static implicit operator Color(SelectableColor color)
		{
			return color._color;
		}

		public static implicit operator Brush(SelectableColor color)
		{
			return color._brush;
		}

		public static implicit operator Pen(SelectableColor color)
		{
			return color._pen;
		}

		public override string ToString()
		{
			return _name;
		}

		private void Dispose(bool disposing)
		{
			if(_brush != null)
				_brush.Dispose();
			if(_pen != null)
				_pen.Dispose();
			if(disposing)
			{
				_brush = null;
				_pen = null;
			}
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}
	}
}
