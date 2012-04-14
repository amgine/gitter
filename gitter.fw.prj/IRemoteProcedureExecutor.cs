namespace gitter.Framework
{
	using System;

	public interface IRemoteProcedureExecutor
	{
		void Execute(Action action);

		T Execute<T>(Func<T> func);

		void Close();
	}
}
