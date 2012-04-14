namespace gitter.Framework.Services
{
	interface ILogFilter
	{
		bool Filter(LogEvent @event);
	}
}
