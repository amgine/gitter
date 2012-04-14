namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	public abstract class BaseRemoteReference : IRemoteReference
	{
		#region Events

		public event EventHandler Deleted;

		private void InvokeDeleted()
		{
			var handler = Deleted;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		#region Data

		private readonly RemoteReferencesCollection _refs;
		private readonly string _name;
		private readonly string _hash;

		private bool _deleted;

		#endregion

		internal BaseRemoteReference(RemoteReferencesCollection refs, string name, string hash)
		{
			if(refs == null) throw new ArgumentNullException("refs");
			if(name == null) throw new ArgumentNullException("name");
			if(hash == null) throw new ArgumentNullException("hash");

			_refs = refs;
			_name = name;
			_hash = hash;
		}

		protected abstract void DeleteCore();

		public void Delete()
		{
			DeleteCore();
			MarkAsDeleted();
		}

		public void MarkAsDeleted()
		{
			_deleted = true;
			InvokeDeleted();
		}

		public bool IsDeleted
		{
			get { return _deleted; }
		}

		protected RemoteReferencesCollection References
		{
			get { return _refs; }
		}

		public Remote Remote
		{
			get { return _refs.Remote; }
		}

		public string Name
		{
			get { return _name; }
		}

		public string FullName
		{
			get
			{
				switch(ReferenceType)
				{
					case Git.ReferenceType.LocalBranch:
						return GitConstants.LocalBranchPrefix + _name;
					case Git.ReferenceType.Tag:
						return GitConstants.TagPrefix + _name;
					default:
						return _name;
				}
			}
		}

		public string Hash
		{
			get { return _hash; }
		}

		public abstract ReferenceType ReferenceType { get; }

		public override string ToString()
		{
			return _name;
		}
	}
}
