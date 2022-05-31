namespace gitter;

using Autofac;

using gitter.Framework;

sealed class AutofacFactory<T> : IFactory<T>
{
	public AutofacFactory(IComponentContext componentContext)
	{
		Verify.Argument.IsNotNull(componentContext);

		ComponentContext = componentContext;
	}

	private IComponentContext ComponentContext { get; }

	public T Create() => ComponentContext.Resolve<T>();
}
