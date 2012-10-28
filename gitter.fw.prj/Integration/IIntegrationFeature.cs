namespace gitter.Framework
{
	using System;
	using System.Drawing;
	
	using gitter.Framework.Configuration;

	public interface IIntegrationFeature : INamedObject
	{
		event EventHandler IsEnabledChanged;

		string DisplayText { get; }

		Bitmap Icon { get; }

		bool IsEnabled { get; set; }

		bool AdministratorRightsRequired { get; }

		Action GetEnableAction(bool enable);

		bool HasConfiguration { get; }

		void SaveTo(Section section);

		void LoadFrom(Section section);
	}
}
