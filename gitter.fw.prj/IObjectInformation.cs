namespace gitter.Framework
{
	using System;

	public interface IObjectData<T> : INamedObject
	{
		void Update(T obj);

		T Construct(IRepository repository);
	}
}
