namespace gitter.UAC
{
	using System;
	using System.Threading;

	using gitter.Framework;

	sealed class RemoteExecutor : MarshalByRefObject, IRemoteProcedureExecutor
	{
		private ManualResetEvent _evExit;

		public RemoteExecutor()
		{
			_evExit = new ManualResetEvent(false);
		}

		public void Execute(Action action)
		{
			action();
		}

		public T Execute<T>(Func<T> func)
		{
			return func();
		}

		public WaitHandle ExitEvent
		{
			get { return _evExit; }
		}

		public void Close()
		{
			_evExit.Set();
		}
	}
}
