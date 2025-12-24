#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git;

/// <summary>Message specification type.</summary>
public enum MessageSpecificationType
{
	/// <summary>No message provided.</summary>
	None,
	/// <summary>String value provided.</summary>
	Text,
	/// <summary>Message is saved to file, file name is provided.</summary>
	File,
}

/// <summary>Specifies message source for commits &amp; notes.</summary>
public readonly struct MessageSpecification
{
	private readonly string? _value;

	/// <summary>Specifies message from the text file.</summary>
	/// <param name="file">File name.</param>
	/// <returns>Message specification.</returns>
	public static MessageSpecification FromFile(string file)
		=> new(MessageSpecificationType.File, file);

	/// <summary>Specifies message directly.</summary>
	/// <param name="text">Message text.</param>
	/// <returns>Message specification.</returns>
	public static MessageSpecification FromText(string text)
		=> new(MessageSpecificationType.Text, text);

	private MessageSpecification(MessageSpecificationType type, string value)
	{
		Type   = type;
		_value = value;
	}

	/// <summary>Returns message specification type.</summary>
	public MessageSpecificationType Type { get; }

	/// <summary>
	/// Returns message text if <see cref="Type"/> is
	/// <see cref="MessageSpecificationType.Text"/>.
	/// </summary>
	public string? Text => Type is MessageSpecificationType.Text ? _value : null;

	/// <summary>
	/// Returns message file name if <see cref="Type"/> is
	/// <see cref="MessageSpecificationType.File"/>.
	/// </summary>
	public string? File => Type is MessageSpecificationType.File ? _value : null;

	/// <inheritdoc/>
	public override string? ToString() => _value;
}
