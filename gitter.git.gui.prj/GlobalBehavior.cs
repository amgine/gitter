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

namespace gitter.Git.Gui;

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using gitter.Framework;

internal static class GlobalBehavior
{
	public static IGraphStyle GraphStyle = new GraphStyle(new ValueSource<GraphStyleOptions>(new GraphStyleOptions()));

	public static AutoCompleteMode AutoCompleteMode = AutoCompleteMode.Suggest;

	public static bool GroupReferences = true;
	public static bool GroupRemoteBranches = true;

	public static bool AskOnCommitCheckouts = true;

	#region AutoComplete

	static void AddStrings<TObject, TEventArgs>(
		AutoCompleteStringCollection              strings,
		GitObjectsCollection<TObject, TEventArgs> values)
		where TObject    : GitNamedObjectWithLifetime
		where TEventArgs : ObjectEventArgs<TObject>
	{
		Assert.IsNotNull(strings);
		Assert.IsNotNull(values);

		lock(values.SyncRoot)
		{
			foreach(var value in values)
			{
				strings.Add(value.Name);
			}
		}
	}

	private static void AddStrings(AutoCompleteStringCollection strings, RefsCollection refs, ReferenceType referenceTypes)
	{
		Assert.IsNotNull(strings);
		Assert.IsNotNull(refs);

		static bool HasFlag(ReferenceType flags, ReferenceType flag)
			=> (flags & flag) == flag;

		if(HasFlag(referenceTypes, ReferenceType.LocalBranch))
		{
			AddStrings(strings, refs.Heads);
		}
		if(HasFlag(referenceTypes, ReferenceType.RemoteBranch))
		{
			AddStrings(strings, refs.Remotes);
		}
		if(HasFlag(referenceTypes, ReferenceType.Tag))
		{
			AddStrings(strings, refs.Tags);
		}
	}

	public static void SetupAutoCompleteSource(TextBox textBox, Repository repository, ReferenceType referenceTypes)
	{
		Verify.Argument.IsNotNull(textBox);
		Verify.Argument.IsNotNull(repository);

		if(GlobalBehavior.AutoCompleteMode == AutoCompleteMode.None) return;

		AddStrings(textBox.AutoCompleteCustomSource, repository.Refs, referenceTypes);
		textBox.AutoCompleteMode   = GlobalBehavior.AutoCompleteMode;
		textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
	}

	public static void SetupAutoCompleteSource(TextBox textBox, IEnumerable<IRevisionPointer> revisions)
	{
		Verify.Argument.IsNotNull(textBox);
		Verify.Argument.IsNotNull(revisions);

		if(GlobalBehavior.AutoCompleteMode == AutoCompleteMode.None) return;

		var source = textBox.AutoCompleteCustomSource;

		foreach(var rev in revisions)
		{
			source.Add(rev.Pointer);
		}

		textBox.AutoCompleteMode   = GlobalBehavior.AutoCompleteMode;
		textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
	}

	public static void SetupAutoCompleteSource(ComboBox comboBox, Repository repository, ReferenceType referenceTypes)
	{
		Verify.Argument.IsNotNull(comboBox);
		Verify.Argument.IsNotNull(repository);

		if(GlobalBehavior.AutoCompleteMode == AutoCompleteMode.None) return;

		AddStrings(comboBox.AutoCompleteCustomSource, repository.Refs, referenceTypes);
		comboBox.AutoCompleteMode   = GlobalBehavior.AutoCompleteMode;
		comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
	}

	public static void SetupAutoCompleteSource(ComboBox comboBox, IEnumerable<IRevisionPointer> revisions)
	{
		Verify.Argument.IsNotNull(comboBox);
		Verify.Argument.IsNotNull(revisions);

		if(GlobalBehavior.AutoCompleteMode == AutoCompleteMode.None) return;

		var source = comboBox.AutoCompleteCustomSource;

		foreach(var rev in revisions)
		{
			source.Add(rev.Pointer);
		}
		comboBox.AutoCompleteMode = GlobalBehavior.AutoCompleteMode;
		comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
	}

	#endregion
}
