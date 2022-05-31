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

namespace gitter.Framework.Controls;

using System;

/// <summary>Simple plain text subitem.</summary>
public class TextSubItem : BaseTextSubItem
{
	private string _text;

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

	/// <summary>Subitem text.</summary>
	public override string Text
	{
		get => _text;
		set
		{
			_text = value;
			Invalidate();
		}
	}
}
