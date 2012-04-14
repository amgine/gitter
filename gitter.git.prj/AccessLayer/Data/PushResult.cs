namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Results of pushing local reference to remote repository.</summary>
	public sealed class ReferencePushResult
	{
		private readonly PushResultType _type;
		private readonly string _localRefName;
		private readonly string _remoteRefName;
		private readonly string _summary;

		public ReferencePushResult(PushResultType type, string localRefName, string remoteRefName, string summary)
		{
			_type = type;
			_localRefName = localRefName;
			_remoteRefName = remoteRefName;
			_summary = summary;
		}

		public PushResultType Type
		{
			get { return _type; }
		}

		public string LocalRefName
		{
			get { return _localRefName; }
		}

		public string RemoteRefName
		{
			get { return _remoteRefName; }
		}

		public string Summary
		{
			get { return _summary; }
		}

		private static char TypeToChar(PushResultType type)
		{
			switch(type)
			{
				case PushResultType.ForceUpdated:
					return '+';
				case PushResultType.FastForwarded:
					return ' ';
				case PushResultType.Rejected:
					return '!';
				case PushResultType.UpToDate:
					return '=';
				case PushResultType.DeletedReference:
					return '-';
				case PushResultType.CreatedReference:
					return '*';
				default:
					throw new ArgumentException("type");
			}
		}

		public override string ToString()
		{
			return string.Format("{0} {1} -> {2} {3}", TypeToChar(_type), _localRefName, _remoteRefName, _summary);
		}
	}
}
