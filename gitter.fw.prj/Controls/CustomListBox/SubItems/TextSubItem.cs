namespace gitter.Framework.Controls
{
	using System;

	/// <summary>Simple plain text subitem.</summary>
	public class TextSubItem : BaseTextSubItem
	{
		private string _text;

		#region .ctor

		/// <summary>Create <see cref="TextSubItem"/>.</summary>
		/// <param name="id">Subitem id.</param>
		/// <param name="text">Subitem text.</param>
		public TextSubItem(int id, string text)
			: base(id)
		{
			_text = text;
		}

		/// <summary>Create <see cref="TextSubItem"/>.</summary>
		/// <param name="id">Subitem id.</param>
		public TextSubItem(int id)
			: this(id, null)
		{
		}

		#endregion

		/// <summary>Subitem text.</summary>
		public override string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				Invalidate();
			}
		}
	}
}
