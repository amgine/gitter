namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;

	using Resources = gitter.Framework.Properties.Resources;

	public sealed class LogToolFactory : ViewFactory
	{
		public static readonly new Guid Guid = new Guid("216F243F-0E79-4739-A88F-C2342E5975B6");

		/// <summary>Initializes a new instance of the <see cref="LogToolFactory"/> class.</summary>
		public LogToolFactory()
			: base(Guid, Resources.StrLog, Resources.ImgLog, true)
		{
			DefaultViewPosition = ViewPosition.Float;
		}

		protected override ViewBase CreateViewCore(IDictionary<string, object> parameters)
		{
			return new LogTool(parameters);
		}
	}
}
