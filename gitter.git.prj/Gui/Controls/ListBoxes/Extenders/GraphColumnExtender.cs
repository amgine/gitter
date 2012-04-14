namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Extender for <see cref="GraphColumn"/>.</summary>
	[ToolboxItem(false)]
	public partial class GraphColumnExtender : BaseExtender
	{
		private readonly GraphColumn _column;
		private bool _disableEvents;

		/// <summary>Create <see cref="GraphColumnExtender"/>.</summary>
		/// <param name="column">Related column.</param>
		public GraphColumnExtender(GraphColumn column)
		{
			if(column == null) throw new ArgumentNullException("column");
			_column = column;

			InitializeComponent();

			_chkShowColors.Text = Resources.StrShowColors;

			ShowColors = column.ShowColors;
			SubscribeToColumnEvents();
		}

		private void SubscribeToColumnEvents()
		{
			_column.ShowColorsChanged += OnColumnShowColorsChanged;
		}

		private void UnsubscribeFromColumnEvents()
		{
			_column.ShowColorsChanged -= OnShowColorsCheckedChanged;
		}

		public bool ShowColors
		{
			get { return _chkShowColors.Checked; }
			set
			{
				_disableEvents = true;
				_chkShowColors.Checked = value;
				_disableEvents = false;
			}
		}

		private void OnColumnShowColorsChanged(object sender, EventArgs e)
		{
			ShowColors = _column.ShowColors;
		}

		private void OnShowColorsCheckedChanged(object sender, EventArgs e)
		{
			if(!_disableEvents)
			{
				_column.ShowColors = ((CheckBox)sender).Checked;
			}
		}
	}
}
