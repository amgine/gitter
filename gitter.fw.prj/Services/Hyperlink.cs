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

namespace gitter.Framework.Services
{
	/// <summary>Represents a hyperlink in text.</summary>
	public sealed class Hyperlink
	{
		/// <summary>Create <see cref="Hyperlink"/>.</summary>
		/// <param name="text">Link text.</param>
		/// <param name="url">Link URL.</param>
		public Hyperlink(Substring text, string url)
		{
			Text = text;
			Url = url;
		}

		/// <summary>Link text.</summary>
		public Substring Text { get; }

		/// <summary>Link URL.</summary>
		public string Url { get; }

		/// <summary>Navigate the hyperlink.</summary>
		public void Navigate() => Utility.OpenUrl(Url);

		/// <summary>Returns a <see cref="T:System.String"/> representation of this <see cref="Hyperlink"/>.</summary>
		/// <returns><see cref="T:System.String"/> representation of this <see cref="Hyperlink"/>.</returns>
		public override string ToString() => Url;
	}
}
