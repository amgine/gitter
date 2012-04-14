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
			if(textBox == null) throw new ArgumentNullException("textBox");
			if(repository == null) throw new ArgumentNullException("repository");

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
			if(textBox == null) throw new ArgumentNullException("textBox");
			if(revisions == null) throw new ArgumentNullException("revisions");

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
			if(comboBox == null) throw new ArgumentNullException("comboBox");
			if(repository == null) throw new ArgumentNullException("repository");

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
			if(comboBox == null) throw new ArgumentNullException("comboBox");
			if(revisions == null) throw new ArgumentNullException("revisions");

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
