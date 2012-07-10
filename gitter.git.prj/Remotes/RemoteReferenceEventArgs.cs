namespace gitter.Git
{
	using System;

	public sealed class RemoteReferenceEventArgs : EventArgs
	{
		private IRemoteReference _reference;

		public RemoteReferenceEventArgs(IRemoteReference reference)
		{
			_reference = reference;
		}

		public IRemoteReference Reference
		{
			get { return _reference; }
		}
	}
}
