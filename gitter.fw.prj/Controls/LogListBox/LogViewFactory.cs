namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;

	using Resources = gitter.Framework.Properties.Resources;

	public sealed class LogViewFactory : ViewFactoryBase
	{
		public static readonly new Guid Guid = new Guid("216F243F-0E79-4739-A88F-C2342E5975B6");

		/// <summary>Initializes a new instance of the <see cref="LogViewFactory"/> class.</summary>
		public LogViewFactory()
			: base(Guid, Resources.StrLog, Resources.ImgLog, true)
		{
			DefaultViewPosition = ViewPosition.Float;
		}

		/// <summary>Create new view with specified parameters.</summary>
		/// <param name="environment">Application working environment.</param>
		/// <param name="parameters">Creation parameters.</param>
		/// <returns>Created view.</returns>
		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new LogView(environment, parameters);
		}
	}
}
