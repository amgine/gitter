namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	/// <summary>Base class for subitems which display plain text.</summary>
	public abstract class BaseTextSubItem : CustomListBoxSubItem
	{
		#region Data

		private Font _font;
		private Brush _textBrush;
		private StringAlignment? _alignment;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="BaseTextSubItem"/>.</summary>
		/// <param name="id">Subitem id.</param>
		protected BaseTextSubItem(int id)
			: base(id)
		{
		}

		#endregion

		#region Properties

		/// <summary>Subitem font.</summary>
		public Font Font
		{
			get { return _font; }
			set
			{
				if(_font != value)
				{
					_font = value;
					Invalidate();
				}
			}
		}

		/// <summary>Text brush.</summary>
		public Brush TextBrush
		{
			get { return _textBrush; }
			set
			{
				if(_textBrush != value)
				{
					_textBrush = value;
					Invalidate();
				}
			}
		}

		/// <summary>Horizontal text alignment.</summary>
		public StringAlignment? TextAlignment
		{
			get { return _alignment; }
			set
			{
				if(_alignment != value)
				{
					_alignment = value;
					Invalidate();
				}
			}
		}

		/// <summary>Subitem text.</summary>
		public abstract string Text { get; set; }

		#endregion

		#region Overrides

		/// <summary>Paint event handler.</summary>
		/// <param name="paintEventArgs">Paint event args.</param>
		protected override void OnPaint(SubItemPaintEventArgs paintEventArgs)
		{
			paintEventArgs.PaintText(Text,
				_font ?? paintEventArgs.Font,
				_textBrush ?? paintEventArgs.Brush,
				_alignment ?? paintEventArgs.Alignment);
		}

		/// <summary>Measure event handler.</summary>
		/// <param name="measureEventArgs">Measure event args.</param>
		/// <returns>Subitem content size.</returns>
		protected override Size OnMeasure(SubItemMeasureEventArgs measureEventArgs)
		{
			return measureEventArgs.MeasureText(Text);
		}

		/// <summary>Returns a <see cref="T:System.String"/> representation of this <see cref="BaseTextSubItem"/>.</summary>
		/// <returns><see cref="T:System.String"/> representation of this <see cref="BaseTextSubItem"/>.</returns>
		public override string ToString()
		{
			return Text;
		}

		#endregion
	}
}
