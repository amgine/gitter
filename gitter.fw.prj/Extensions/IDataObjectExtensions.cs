namespace gitter.Framework
{
	using System.Windows.Forms;

	public static class IDataObjectExtensions
	{
		public static T GetData<T>(this IDataObject data)
		{
			Verify.Argument.IsNotNull(data, "data");

			return (T)data.GetData(typeof(T));
		}

		public static bool GetDataPresent<T>(this IDataObject data)
		{
			Verify.Argument.IsNotNull(data, "data");

			return data.GetDataPresent(typeof(T));
		}
	}
}
