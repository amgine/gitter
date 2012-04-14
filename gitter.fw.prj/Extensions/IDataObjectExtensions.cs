namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using System.Windows.Forms;

	public static class IDataObjectExtensions
	{
		public static T GetData<T>(this IDataObject data)
		{
			if(data == null) throw new ArgumentNullException("data");

			return (T)data.GetData(typeof(T));
		}

		public static bool GetDataPresent<T>(this IDataObject data)
		{
			if(data == null) throw new ArgumentNullException("data");

			return data.GetDataPresent(typeof(T));
		}
	}
}
