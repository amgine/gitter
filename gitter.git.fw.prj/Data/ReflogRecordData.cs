namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	public sealed class ReflogRecordData
	{
		#region Data

		private readonly int _index;
		private readonly string _message;
		private readonly RevisionData _revision;

		#endregion

		#region .ctor

		public ReflogRecordData(int index, string message, RevisionData revision)
		{
			_index = index;
			_message = message;
			_revision = revision;
		}

		#endregion

		#region Properties

		public string Name
		{
			get { return GitConstants.StashFullName + "@{" + _index + "}"; }
		}

		public string Message
		{
			get { return _message; }
		}

		public int Index
		{
			get { return _index; }
		}

		public RevisionData Revision
		{
			get { return _revision; }
		}

		#endregion
	}
}
