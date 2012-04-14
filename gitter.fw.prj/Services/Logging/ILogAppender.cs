namespace gitter.Framework.Services
{
	public interface ILogAppender
	{
		void Append(LogEvent @event);
	}
}
