namespace gitter.Framework.Services
{
	/// <summary>Represents a hyperlink in text.</summary>
	public sealed class Hyperlink
	{
		#region Data

		private Substring _text;
		private string _url;

		#endregion

		/// <summary>Create <see cref="Hyperlink"/>.</summary>
		/// <param name="text">Link text.</param>
		/// <param name="url">Link URL.</param>
		public Hyperlink(Substring text, string url)
		{
			_text = text;
			_url = url;
		}

		/// <summary>Link text.</summary>
		public Substring Text
		{
			get { return _text; }
		}

		/// <summary>Link URL.</summary>
		public string Url
		{
			get { return _url; }
		}

		/// <summary>Navigate the hyperlink.</summary>
		public void Navigate()
		{
			Utility.OpenUrl(_url);
		}

		/// <summary>Returns a <see cref="T:System.String"/> representation of this <see cref="Hyperlink"/>.</summary>
		/// <returns><see cref="T:System.String"/> representation of this <see cref="Hyperlink"/>.</returns>
		public override string ToString()
		{
			return _url;
		}
	}
}
