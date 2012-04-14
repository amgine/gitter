namespace gitter.Git
{
	using System;

	/// <summary>Represents patch file.</summary>
	public interface IPatchFile : IDisposable
	{
		/// <summary>Returns patch file name.</summary>
		string FileName { get; }
	}
}
