namespace gitter.Git
{
	using System;

	/// <summary>Feature which is available in specified version of git.</summary>
	internal sealed class VersionFeature : GitFeature
	{
		private readonly Version _version;

		public VersionFeature(string name, Version version)
			: base(name)
		{
			if(version == null) throw new ArgumentNullException("version");
			_version = version;
		}

		public Version RequiredVersion
		{
			get { return _version; }
		}

		public override bool IsAvailable
		{
			get { return RepositoryProvider.GitVersion >= _version; }
		}
	}
}
