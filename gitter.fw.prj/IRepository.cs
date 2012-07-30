namespace gitter.Framework
{
	using System;

	using gitter.Framework.Configuration;

	/// <summary>Abstract repository.</summary>
	public interface IRepository
	{
		/// <summary>Repository deleted.</summary>
		event EventHandler Deleted;

		/// <summary>Working path of repository.</summary>
		string WorkingDirectory { get; }

		/// <summary>Returns repository configuration section.</summary>
		/// <value>Repository configuration section.</value>
		Section ConfigSection { get; }
	}
}
