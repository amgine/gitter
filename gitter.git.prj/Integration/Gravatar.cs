namespace gitter.Git.Integration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Services;
	using gitter.Git.Gui;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Gravatar integration feature.</summary>
	public sealed class Gravatar : IntegrationFeature
	{
		private DefaultGravatarType _defaultType;
		private const string _name = "Gravatar";

		internal Gravatar()
			: base(_name, Resources.StrsGravatarDisplayText, CachedResources.Bitmaps["ImgGravatar"], false)
		{
		}

		public DefaultGravatarType DefaultGravatarType
		{
			get { return _defaultType; }
			set { _defaultType = value; }
		}
	}
}
