using System;

namespace gitter.Framework.Services
{
	/// <summary>gitter update channel.</summary>
	public interface IUpdateChannel
	{
		/// <summary>Check latest gitter version on this chanel.</summary>
		/// <returns>Latest gitter version.</returns>
		Version CheckVersion();

		/// <summary>Update gitter using this channel.</summary>
		void Update();
	}
}
