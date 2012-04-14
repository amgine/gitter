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
	internal sealed class LogTool : ViewBase
	{
		private static readonly Bitmap ImgLog = Resources.ImgLog;

		private LogListBox _logListBox;

		/// <summary>Initializes a new instance of the <see cref="LogTool"/> class.</summary>
		public LogTool(IDictionary<string, object> parameters)
			: base(LogToolFactory.Guid, parameters)
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
