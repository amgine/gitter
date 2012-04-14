namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	public sealed class RevisionLog
	{
		public static readonly RevisionLog Empty = new RevisionLog(new Revision[0]);

		private readonly IList<Revision> _revisions;

		public RevisionLog(IList<Revision> revisions)
		{
			if(revisions == null)
				throw new ArgumentNullException("revisions");

			_revisions = revisions;
		}

		public IList<Revision> Revisions
		{
			get { return _revisions; }
		}
	}
}
