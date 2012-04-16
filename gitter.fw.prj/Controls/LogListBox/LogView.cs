namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class LogView : ViewBase
	{
		private static readonly Bitmap ImgLog = Resources.ImgLog;

		private LogListBox _logListBox;

		/// <summary>Initializes a new instance of the <see cref="LogView"/> class.</summary>
		public LogView(IWorkingEnvironment environment, IDictionary<string, object> parameters)
			: base(LogViewFactory.Guid, environment, parameters)
		{
			Height = 200;

			_logListBox = new LogListBox()
			{
				BorderStyle = BorderStyle.None,
				Dock = DockStyle.Fill,
				Parent = this,
			};

			Text = Resources.StrLog;
		}

		public override Image Image
		{
			get { return ImgLog; }
		}
	}
}
