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

namespace gitter.Git.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	internal static class GlobalBehavior
	{
		public static IGraphBuilderFactory GraphBuilderFactory = new DefaultGraphBuilderFactory();

		public static IGraphStyle GraphStyle = new DefaultGraphStyle();

		public static AutoCompleteMode AutoCompleteMode = AutoCompleteMode.Suggest;

		public static bool GroupReferences = true;
		public static bool GroupRemoteBranches = true;

		public static bool AskOnCommitCheckouts = true;

		#region AutoComplete

		public static void SetupAutoCompleteSource(TextBox textBox, Repository repository, ReferenceType referenceTypes)
		{
			Verify.Argument.IsNotNull(textBox, nameof(textBox));
			Verify.Argument.IsNotNull(repository, nameof(repository));

			if(GlobalBehavior.AutoCompleteMode == AutoCompleteMode.None) return;

			var source = textBox.AutoCompleteCustomSource;

			if((referenceTypes & ReferenceType.LocalBranch) == ReferenceType.LocalBranch)
			{
				foreach(var branch in repository.Refs.Heads)
				{
					source.Add(branch.Name);
				}
			}
			if((referenceTypes & ReferenceType.RemoteBranch) == ReferenceType.RemoteBranch)
			{
				foreach(var branch in repository.Refs.Remotes)
				{
					source.Add(branch.Name);
				}
			}
			if((referenceTypes & ReferenceType.Tag) == ReferenceType.Tag)
			{
				foreach(var tag in repository.Refs.Tags)
				{
					source.Add(tag.Name);
				}
			}

			textBox.AutoCompleteMode = GlobalBehavior.AutoCompleteMode;
			textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
		}

		public static void SetupAutoCompleteSource(TextBox textBox, IEnumerable<IRevisionPointer> revisions)
		{
			Verify.Argument.IsNotNull(textBox, nameof(textBox));
			Verify.Argument.IsNotNull(revisions, nameof(revisions));

			if(GlobalBehavior.AutoCompleteMode == AutoCompleteMode.None) return;

			var source = textBox.AutoCompleteCustomSource;

			foreach(var rev in revisions)
			{
				source.Add(rev.Pointer);
			}

			textBox.AutoCompleteMode = GlobalBehavior.AutoCompleteMode;
			textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
		}

		public static void SetupAutoCompleteSource(ComboBox comboBox, Repository repository, ReferenceType referenceTypes)
		{
			Verify.Argument.IsNotNull(comboBox, nameof(comboBox));
			Verify.Argument.IsNotNull(repository, nameof(repository));

			if(GlobalBehavior.AutoCompleteMode == AutoCompleteMode.None) return;

			var source = comboBox.AutoCompleteCustomSource;

			if((referenceTypes & ReferenceType.LocalBranch) == ReferenceType.LocalBranch)
			{
				foreach(var branch in repository.Refs.Heads)
				{
					source.Add(branch.Name);
				}
			}
			if((referenceTypes & ReferenceType.RemoteBranch) == ReferenceType.RemoteBranch)
			{
				foreach(var branch in repository.Refs.Remotes)
				{
					source.Add(branch.Name);
				}
			}
			if((referenceTypes & ReferenceType.Tag) == ReferenceType.Tag)
			{
				foreach(var tag in repository.Refs.Tags)
				{
					source.Add(tag.Name);
				}
			}

			comboBox.AutoCompleteMode = GlobalBehavior.AutoCompleteMode;
			comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
		}

		public static void SetupAutoCompleteSource(ComboBox comboBox, IEnumerable<IRevisionPointer> revisions)
		{
			Verify.Argument.IsNotNull(comboBox, nameof(comboBox));
			Verify.Argument.IsNotNull(revisions, nameof(revisions));

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
}
