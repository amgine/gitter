namespace gitter.Framework.Options
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	public sealed class PropertyPageDescription
	{
		#region Static Data

		public static readonly Guid RootGroupGuid		= Guid.Empty;
		public static readonly Guid AppearanceGroupGuid = new Guid("F1F07910-1105-4928-9B7C-F62657601747");

		#endregion

		#region Data

		private readonly Guid _guid;
		private readonly Guid _groupGuid;
		private readonly string _name;
		private Func<PropertyPage> _getPropertyPage;
		private Bitmap _icon;

		#endregion

		#region .ctor

		public PropertyPageDescription(Guid guid, string name, Bitmap icon, Guid groupGuid, Func<PropertyPage> getPropertyPage)
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

		public PropertyPage CreatePropertyPage()
		{
			if(_getPropertyPage != null) return _getPropertyPage();
			return null;
		}

		#endregion
	}
}
