namespace gitter.Git
{
	using System;

	/// <summary>Represents a reference on remote repository.</summary>
	public interface IRemoteReference
	{
		event EventHandler Deleted;

		/// <summary>Remote repository.</summary>
		Remote Remote { get; }

		/// <summary>Reference name.</summary>
		string Name { get; }

		/// <summary>Commit hash.</summary>
		string Hash { get; }

		ReferenceType ReferenceType { get; }

		/// <summary>Remove reference from remote repository.</summary>
		void Delete();

		bool IsDeleted { get; }
	}
}
