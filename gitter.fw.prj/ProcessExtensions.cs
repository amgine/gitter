#if !NETCOREAPP

namespace gitter;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public static class ProcessExtensions
{
	extension(Process process)
	{
		public async Task WaitForExitAsync(CancellationToken cancellationToken = default)
		{
			if(!process.HasExited)
			{
				cancellationToken.ThrowIfCancellationRequested();
			}

			try
			{
				process.EnableRaisingEvents = true;
			}
			catch(InvalidOperationException)
			{
				if(!process.HasExited) throw;
				return;
			}

			var tcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
			EventHandler handler = (_, _) => tcs.TrySetResult(default);
			process.Exited += handler;
			try
			{
				if(!process.HasExited)
				{
					using(cancellationToken.Register(() =>
					{
						tcs.TrySetCanceled(cancellationToken);
					}))
					{
						await tcs.Task.ConfigureAwait(continueOnCapturedContext: false);
					}
				}
			}
			finally
			{
				process.Exited -= handler;
			}
		}
	}
}

#endif
