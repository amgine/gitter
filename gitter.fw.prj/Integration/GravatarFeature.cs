namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Services;

	using Resources = gitter.Framework.Properties.Resources;

	/// <summary>Gravatar integration feature.</summary>
	public sealed class GravatarFeature : IntegrationFeature
	{
		private DefaultGravatarType _defaultType;
		private const string _name = "Gravatar";

		internal GravatarFeature()
			: base(_name, Resources.StrsGravatarDisplayText, CachedResources.Bitmaps["ImgGravatar"], false)
		{
			IsEnabled = true;
		}

		public DefaultGravatarType DefaultGravatarType
		{
			get { return _defaultType; }
			set { _defaultType = value; }
		}
	}
}
