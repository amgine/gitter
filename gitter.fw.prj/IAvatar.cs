namespace gitter.Framework
{
	using System;
	using System.Drawing;

	public interface IAvatar
	{
		event EventHandler Updated;

		Image Image { get; }

		bool IsLoaded { get; }

		IAsyncResult BeginUpdate();

		void EndUpdate(IAsyncResult ar);

		void Update();
	}
}
