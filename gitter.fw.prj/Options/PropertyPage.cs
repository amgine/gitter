namespace gitter.Framework.Options
{
	using System;
	using System.ComponentModel;

	using gitter.Framework.Options;

	[ToolboxItem(false)]
	public partial class PropertyPage : DialogBase
	{
		private readonly Guid _guid;

		public PropertyPage()
		{
		}

		public PropertyPage(Guid guid)
		{
			_guid = guid;
		}

		public Guid Guid
		{
			get { return _guid; }
		}
	}
}
