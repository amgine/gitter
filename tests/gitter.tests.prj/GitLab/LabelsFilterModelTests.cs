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

#nullable enable

namespace gitter.GitLab.Gui;

using System.Linq;

using NUnit.Framework;

using Label = gitter.GitLab.Api.Label;

[TestFixture]
class LabelsFilterModelTests
{
	private static Label L(string name, string? description = null)
		=> new() { Name = name, Color = "#000000", Description = description };

	private static LabelsFilterModel WithLabels(params Label[] labels)
		=> new() { Labels = labels };

	private static string[] Names(LabelsFilterModel model, string? filter)
		=> model.GetVisibleLabels(filter).Select(static l => l.Name).ToArray();

	#region filtering

	[Test]
	public void EmptyFilter_ShowsEverything()
	{
		var model = WithLabels(L("bug"), L("ui"), L("backend"));
		Assert.That(Names(model, null),   Is.EqualTo(new[] { "bug", "ui", "backend" }));
		Assert.That(Names(model, ""),     Is.EqualTo(new[] { "bug", "ui", "backend" }));
		Assert.That(Names(model, "   "),  Is.EqualTo(new[] { "bug", "ui", "backend" }));
	}

	[Test]
	public void Filter_MatchesSubstringCaseInsensitively()
	{
		var model = WithLabels(L("Bug"), L("debug"), L("ui"));
		Assert.That(Names(model, "bug"), Is.EqualTo(new[] { "Bug", "debug" }));
		Assert.That(Names(model, "BUG"), Is.EqualTo(new[] { "Bug", "debug" }));
	}

	[Test]
	public void Filter_MatchesDescriptionToo()
	{
		var model = WithLabels(L("p1", "highest priority"), L("p2", "normal"));
		Assert.That(Names(model, "priority"), Is.EqualTo(new[] { "p1" }));
	}

	[Test]
	public void Filter_IsTrimmed()
	{
		var model = WithLabels(L("bug"), L("ui"));
		Assert.That(Names(model, "  bug  "), Is.EqualTo(new[] { "bug" }));
	}

	[Test]
	public void Filter_NoMatch_ReturnsEmpty()
	{
		var model = WithLabels(L("bug"), L("ui"));
		Assert.That(Names(model, "zzz"), Is.Empty);
	}

	[Test]
	public void Filter_PreservesRegistryOrder()
	{
		var model = WithLabels(L("zeta"), L("alpha"), L("zulu"));
		Assert.That(Names(model, "z"), Is.EqualTo(new[] { "zeta", "zulu" }));
	}

	[Test]
	public void Labels_NullResetsToEmpty()
	{
		var model = WithLabels(L("bug"));
		model.Labels = null!;
		Assert.That(model.GetVisibleLabels(null), Is.Empty);
	}

	[Test]
	public void Matches_NullLabel_IsFalse()
	{
		Assert.That(LabelsFilterModel.Matches(null, null), Is.False);
	}

	#endregion

	#region selection

	[Test]
	public void Select_TogglesMembership()
	{
		var model = WithLabels(L("bug"), L("ui"));

		Assert.That(model.Select("bug", selected: true), Is.True);
		Assert.That(model.IsSelected("bug"),             Is.True);
		Assert.That(model.SelectedCount,                 Is.EqualTo(1));

		Assert.That(model.Select("bug", selected: false), Is.True);
		Assert.That(model.IsSelected("bug"),              Is.False);
		Assert.That(model.SelectedCount,                  Is.EqualTo(0));
	}

	[Test]
	public void Select_RedundantChange_ReportsNoChange()
	{
		var model = WithLabels(L("bug"));

		model.Select("bug", selected: true);
		Assert.That(model.Select("bug", selected: true),   Is.False);
		Assert.That(model.Select("ui",  selected: false),  Is.False);
	}

	[Test]
	public void Select_IgnoresEmptyName()
	{
		var model = WithLabels(L("bug"));
		Assert.That(model.Select("", selected: true), Is.False);
		Assert.That(model.SelectedCount,              Is.EqualTo(0));
	}

	[Test]
	public void Selection_SurvivesFilteringTheLabelOutOfView()
	{
		var model = WithLabels(L("bug"), L("ui"));
		model.Select("bug", selected: true);

		Assert.That(Names(model, "ui"),      Is.EqualTo(new[] { "ui" }));
		Assert.That(model.IsSelected("bug"), Is.True);
		Assert.That(model.Selected,          Is.EquivalentTo(new[] { "bug" }));
	}

	[Test]
	public void Selection_SurvivesLabelListReplacement()
	{
		var model = WithLabels(L("bug"), L("ui"));
		model.Select("bug", selected: true);

		model.Labels = new[] { L("bug"), L("ui"), L("backend") };

		Assert.That(model.IsSelected("bug"), Is.True);
	}

	[Test]
	public void Selection_KeepsLabelsMissingFromTheRegistry()
	{
		var model = WithLabels(L("bug"));
		model.SetSelected(new[] { "bug", "removed-upstream" });

		Assert.That(model.Selected, Is.EquivalentTo(new[] { "bug", "removed-upstream" }));
	}

	[Test]
	public void SetSelected_ReplacesPreviousSelection()
	{
		var model = WithLabels(L("bug"), L("ui"));
		model.Select("bug", selected: true);

		model.SetSelected(new[] { "ui" });

		Assert.That(model.Selected, Is.EquivalentTo(new[] { "ui" }));
	}

	[Test]
	public void SetSelected_Null_ClearsSelection()
	{
		var model = WithLabels(L("bug"));
		model.Select("bug", selected: true);

		model.SetSelected(null);

		Assert.That(model.SelectedCount, Is.EqualTo(0));
	}

	[Test]
	public void SetSelected_SkipsEmptyNames()
	{
		var model = WithLabels(L("bug"));
		model.SetSelected(new[] { "bug", "", null! });

		Assert.That(model.Selected, Is.EquivalentTo(new[] { "bug" }));
	}

	[Test]
	public void SelectionIsCaseSensitive()
	{
		var model = WithLabels(L("Bug"), L("bug"));
		model.Select("Bug", selected: true);

		Assert.That(model.IsSelected("Bug"), Is.True);
		Assert.That(model.IsSelected("bug"), Is.False);
	}

	[Test]
	public void Clear_DropsEverythingAndReportsChange()
	{
		var model = WithLabels(L("bug"), L("ui"));
		model.SetSelected(new[] { "bug", "ui" });

		Assert.That(model.Clear(),       Is.True);
		Assert.That(model.SelectedCount, Is.EqualTo(0));
		Assert.That(model.Clear(),       Is.False);
	}

	#endregion
}
