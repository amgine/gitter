namespace gitter.Framework.CLI
{
	using System;
	using System.Diagnostics;
	using System.IO;

	/// <summary>Receives output from stderr/stdout.</summary>
	public interface IOutputReceiver
	{
		/// <summary>Gets a value indicating whether this instance is initialized.</summary>
		/// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
		bool IsInitialized { get; }

		/// <summary>Initializes output reader.</summary>
		/// <param name="process">Process to read from.</param>
		/// <param name="reader">StreamReader to read from.</param>
		void Initialize(Process process, StreamReader reader);

		/// <summary>Closes the receiver.</summary>
		void Close();
	}
}
