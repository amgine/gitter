namespace gitter.Framework.Configuration
{
	public interface IDataAdapter
	{
		void Store(Section section);

		Section Load();
	}
}
