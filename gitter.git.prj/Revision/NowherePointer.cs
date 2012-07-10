namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	internal sealed class NowherePointer : GitObject, IRevisionPointer
	{
		private readonly string _name;

		public NowherePointer(Repository repository, string name)
			: base(repository)
		{
			_name = name;
		}

		ReferenceType IRevisionPointer.Type
		{
			get { return ReferenceType.None; }
		}

		public string Pointer
		{
			get { return _name; }
		}

		string IRevisionPointer.FullName
		{
			get { return _name; }
		}

		Revision IRevisionPointer.Dereference()
		{
			return null;
		}

		bool IRevisionPointer.IsDeleted
		{
			get { return false; }
		}

		public override string ToString()
		{
			return _name;
		}
	}
}
