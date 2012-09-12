namespace gitter.Framework.Options
{
	using System;
	using System.Drawing;

	public sealed class PropertyPageFactory
	{
		#region Static Data

		public static readonly Guid RootGroupGuid		= Guid.Empty;
		public static readonly Guid AppearanceGroupGuid = new Guid("F1F07910-1105-4928-9B7C-F62657601747");

		#endregion

		#region Data

		private readonly Guid _guid;
		private readonly Guid _groupGuid;
		private readonly string _name;
		private Func<IWorkingEnvironment, PropertyPage> _getPropertyPage;
		private Bitmap _icon;

		#endregion

		#region .ctor

		public PropertyPageFactory(Guid guid, string name, Bitmap icon, Guid groupGuid, Func<IWorkingEnvironment, PropertyPage> getPropertyPage)
		{
			_guid = guid;
			_name = name;
			_groupGuid = groupGuid;
			_icon = icon;
			_getPropertyPage = getPropertyPage;
		}

		#endregion

		#region Properties

		public Guid Guid
		{
			get { return _guid; }
		}

		public Guid GroupGuid
		{
			get { return _groupGuid; }
		}

		public string Name
		{
			get { return _name; }
		}

		public Bitmap Icon
		{
			get { return _icon; }
		}

		#endregion

		#region Methods

		public PropertyPage CreatePropertyPage(IWorkingEnvironment environment)
		{
			Verify.Argument.IsNotNull(environment, "environment");

			if(_getPropertyPage != null)
			{
				return _getPropertyPage(environment);
			}
			else
			{
				return null;
			}
		}

		#endregion
	}
}
