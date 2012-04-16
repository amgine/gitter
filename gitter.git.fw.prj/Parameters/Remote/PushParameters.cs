namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;
	using gitter.Framework;

	/// <summary>Parameters for <see cref="IRemoteAccessor.Push"/> operation.</summary>
	public sealed class PushParameters
	{
		///	<summary>Create <see cref="PushParameters"/>.</summary>
		public PushParameters()
		{
			ThinPack = true;
		}

		///	<summary>Create <see cref="PushParameters"/>.</summary>
		///	<param name="pushMode">Push mode.</param>
		///	<param name="repository">Repository to push to.</param>
		///	<param name="refspecs">References to push.</param>
		public PushParameters(string repository, PushMode pushMode, IList<string> refspecs)
		{
			Repository = repository;
			PushMode = pushMode;
			Refspecs = refspecs;
		}

		/// <summary>Push mode.</summary>
		public PushMode PushMode { get; set; }

		/// <summary>Remote repository.</summary>
		public string Repository { get; set; }

		/// <summary>References to push.</summary>
		public IList<string> Refspecs { get; set; }

		/// <summary>Delete all listed references from remote repository.</summary>
		public bool Delete { get; set; }

		/// <summary>
		/// Path to the git-receive-pack program on the remote end. Sometimes useful when pushing to
		/// a remote repository over ssh, and you do not have the program in a directory on the default $PATH. 
		/// </summary>
		public string ReceivePack { get; set; }

		/// <summary>Force overwrite remote refs.</summary>
		public bool Force { get; set; }

		/// <summary>
		/// For every branch that is up to date or successfully pushed, add upstream (tracking) reference,
		/// used by argument-less git-pull  and other commands.
		/// </summary>
		public bool SetUpstream { get; set; }

		/// <summary>
		/// A thin transfer significantly reduces the amount of sent data when the sender and
		/// receiver share many of the same objects in common. Enabled by default. 
		/// </summary>
		public bool ThinPack { get; set; }

		/// <summary>Progress monitor.</summary>
		public IAsyncProgressMonitor Monitor { get; set; }
	}
}
