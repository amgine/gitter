namespace gitter.Git
{
	using System;

	/// <summary>Object which points to commit.</summary>
	public interface IRevisionPointer
	{
		/// <summary>Host repository. Never null.</summary>
		Repository Repository { get; }

		/// <summary><see cref="ReferenceType"/>.</summary>
		ReferenceType Type { get; }

		/// <summary>Revision expression (reference name, sha1, relative expression, etc.).</summary>
		string Pointer { get; }

		/// <summary>Returns full non-ambiguous revision name.</summary>
		string FullName { get; }

		/// <summary>Evaluate commit which is targeted by this <see cref="IRevisionPointer"/>.</summary>
		/// <returns>Commit which is pointed by this <see cref="IRevisionPointer"/>.</returns>
		Revision Dereference();

		/// <summary>Object is deleted and not valid anymore.</summary>
		bool IsDeleted { get; }
	}
}
