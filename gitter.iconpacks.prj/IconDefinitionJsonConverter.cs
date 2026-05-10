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

namespace gitter.IconPacks;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Accepts an icon definition that may be either:
///   "_html": "./icons/html.svg"
/// or:
///   "_html": { "iconPath": "./icons/html.svg", "fontColor": "#abc", ... }
/// </summary>
internal sealed class IconDefinitionJsonConverter : JsonConverter<IconDefinition>
{
	public override IconDefinition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if(reader.TokenType == JsonTokenType.String)
		{
			return new IconDefinition { IconPath = reader.GetString() };
		}
		if(reader.TokenType == JsonTokenType.StartObject)
		{
			var def = new IconDefinition();
			while(reader.Read())
			{
				if(reader.TokenType == JsonTokenType.EndObject) return def;
				if(reader.TokenType != JsonTokenType.PropertyName) continue;
				var name = reader.GetString();
				reader.Read();
				switch(name)
				{
					case "iconPath":      def.IconPath      = ReadStringOrSkip(ref reader); break;
					case "fontCharacter": def.FontCharacter = ReadStringOrSkip(ref reader); break;
					case "fontColor":     def.FontColor     = ReadStringOrSkip(ref reader); break;
					case "fontSize":      def.FontSize      = ReadStringOrSkip(ref reader); break;
					case "fontId":        def.FontId        = ReadStringOrSkip(ref reader); break;
					default: reader.Skip(); break;
				}
			}
			return def;
		}
		throw new JsonException($"Unexpected token {reader.TokenType} for IconDefinition.");
	}

	public override void Write(Utf8JsonWriter writer, IconDefinition value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		if(value.IconPath      is not null) writer.WriteString("iconPath",      value.IconPath);
		if(value.FontCharacter is not null) writer.WriteString("fontCharacter", value.FontCharacter);
		if(value.FontColor     is not null) writer.WriteString("fontColor",     value.FontColor);
		if(value.FontSize      is not null) writer.WriteString("fontSize",      value.FontSize);
		if(value.FontId        is not null) writer.WriteString("fontId",        value.FontId);
		writer.WriteEndObject();
	}

	private static string? ReadStringOrSkip(ref Utf8JsonReader reader)
	{
		if(reader.TokenType == JsonTokenType.String) return reader.GetString();
		if(reader.TokenType == JsonTokenType.Number) return reader.GetDouble().ToString(System.Globalization.CultureInfo.InvariantCulture);
		reader.Skip();
		return null;
	}
}
