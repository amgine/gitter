namespace gitter.Framework
{
	public interface IProgress<in T>
	{
		void Report(T progress);
	}
}
