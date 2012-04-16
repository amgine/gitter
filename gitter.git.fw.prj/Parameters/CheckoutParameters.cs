namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IRepositoryAccessor.Checkout"/> opearation.</summary>
	public sealed class CheckoutParameters
	{
		/// <summary>Create <see cref="CheckoutParameters"/>.</summary>
		public CheckoutParameters()
		{
		}

		/// <summary>Create <see cref="CheckoutParameters"/>.</summary>
		/// <param name="revision">Revision to checkout.</param>
		public CheckoutParameters(string revision)
		{
			Revision = revision;
		}

		/// <summary>Create <see cref="CheckoutParameters"/>.</summary>
		/// <param name="revision">Revision to checkout.</param>
		/// <param name="force">Throw away local changes.</param>
		public CheckoutParameters(string revision, bool force)
		{
			Revision = revision;
			Force = force;
		}

		/// <summary>Revision to checkout.</summary>
		public string Revision { get; set; }

		/// <summary>Throw away local changes.</summary>
		public bool Force { get; set; }

		/// <summary>Merge local changes with target revision's tree.</summary>
		public bool Merge { get; set; }
	}
}
