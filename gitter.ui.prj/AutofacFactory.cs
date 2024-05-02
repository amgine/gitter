namespace gitter;

using Autofac;

using gitter.Framework;

sealed class AutofacFactory<T>(IComponentContext componentContext) : IFactory<T>
{
	public T Create() => componentContext.Resolve<T>();
}
