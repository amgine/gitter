namespace gitter.Framework
{
	using System;
	using System.Runtime.InteropServices;
	using System.Threading;
	using System.Windows.Forms;

	public static class ClipboardEx
	{
		public static void SetTextSafe(string text, int attemptsCount = 3, int retryInterval = 50)
		{
			const uint CLIPBRD_E_CANT_OPEN = 0x800401D0;
			while(attemptsCount > 0)
			{
				try
				{
					Clipboard.SetText(text);
					return;
				}
				catch(COMException exc)
				{
					if((uint)exc.ErrorCode != CLIPBRD_E_CANT_OPEN)
					{
						throw;
					}
				}
				--attemptsCount;
				Thread.Sleep(retryInterval);
			}
		}
	}
}
