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

namespace gitter.GitLab.Api;

using System.Text.Json;

using NUnit.Framework;

[TestFixture]
class LabelParsingTests
{
	private const string FullLabelJson = """
		{
		  "id": 42,
		  "name": "bug",
		  "color": "#dc2626",
		  "text_color": "#ffffff",
		  "description": "Something is broken",
		  "open_issues_count": 5,
		  "closed_issues_count": 12,
		  "subscribed": true,
		  "priority": 1,
		  "is_project_label": true
		}
		""";

	[Test]
	public void Parse_FullPayload_PopulatesAllFields()
	{
		var label = JsonSerializer.Deserialize<Label>(FullLabelJson);

		Assert.That(label,                   Is.Not.Null);
		Assert.That(label!.Id,               Is.EqualTo(42));
		Assert.That(label.Name,              Is.EqualTo("bug"));
		Assert.That(label.Color,             Is.EqualTo("#dc2626"));
		Assert.That(label.TextColor,         Is.EqualTo("#ffffff"));
		Assert.That(label.Description,       Is.EqualTo("Something is broken"));
		Assert.That(label.OpenIssuesCount,   Is.EqualTo(5));
		Assert.That(label.ClosedIssuesCount, Is.EqualTo(12));
		Assert.That(label.Subscribed,        Is.True);
		Assert.That(label.Priority,          Is.EqualTo(1));
		Assert.That(label.IsProjectLabel,    Is.True);
	}

	[Test]
	public void Parse_Array_ReturnsAllElements()
	{
		var json = $"[{FullLabelJson},{FullLabelJson}]";
		var labels = JsonSerializer.Deserialize<Label[]>(json);
		Assert.That(labels, Is.Not.Null);
		Assert.That(labels!.Length, Is.EqualTo(2));
	}

	[Test]
	public void Parse_OptionalFieldsMissing_AreNull()
	{
		var json = """{ "id": 1, "name": "x", "color": "#000000" }""";
		var label = JsonSerializer.Deserialize<Label>(json);

		Assert.That(label,             Is.Not.Null);
		Assert.That(label!.TextColor,  Is.Null);
		Assert.That(label.Description, Is.Null);
		Assert.That(label.Priority,    Is.Null);
		Assert.That(label.IsProjectLabel, Is.Null);
	}

	[Test]
	public void Parse_ShorthandColor_AcceptedAsIs()
	{
		var json = """{ "id": 1, "name": "x", "color": "#abc" }""";
		var label = JsonSerializer.Deserialize<Label>(json)!;
		Assert.That(label.Color, Is.EqualTo("#abc"));
	}

	[Test]
	public void Equality_ByIdOnly()
	{
		var a = new Label { Id = 7, Name = "a", Color = "#000" };
		var b = new Label { Id = 7, Name = "b", Color = "#fff" };
		Assert.That(a, Is.EqualTo(b));
		Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
	}
}
