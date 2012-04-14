namespace gitter.Git
{
	using System;

	/// <summary>Represents patch source.</summary>
	public interface IPatchSource
	{
		/// <summary>Prepare patch before 'git apply'.</summary>
		/// <returns>Patch file.</returns>
		IPatchFile PreparePatchFile();

		/// <summary>Returns patch source display name.</summary>
		/// <value>Patch source display name.</value>
		string DisplayName { get; }
	}
}
