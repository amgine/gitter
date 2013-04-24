namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Extender for <see cref="GraphColumn"/>.</summary>
	[ToolboxItem(false)]
	partial class GraphColumnExtender : ExtenderBase
	{
		#region Data

		private ICheckBoxWidget _chkShowColors;
		private bool _disableEvents;

		#endregion

		/// <summary>Create <see cref="GraphColumnExtender"/>.</summary>
		/// <param name="column">Related column.</param>
		public GraphColumnExtender(GraphColumn column)
			: base(column)
		{
			InitializeComponent();
			CreateControls();
			SubscribeToColumnEvents();
		}

		protected override void OnStyleChanged()
		{
			base.OnStyleChanged();
			CreateControls();
		}

		private void CreateControls()
		{
			if(_chkShowColors != null) _chkShowColors.Dispose();
			_chkShowColors = Style.CreateCheckBox();
			_chkShowColors.IsChecked = Column.ShowColors;
			_chkShowColors.IsCheckedChanged += OnShowColorsCheckedChanged;
			_chkShowColors.Text = Resources.StrShowColors;
			_chkShowColors.Control.Bounds = new Rectangle(6, 0, 127, 27);
			_chkShowColors.Control.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			_chkShowColors.Control.Parent = this;
		}

		private void SubscribeToColumnEvents()
		{
			Column.ShowColorsChanged += OnColumnShowColorsChanged;
		}

		private void UnsubscribeFromColumnEvents()
		{
			Column.ShowColorsChanged -= OnShowColorsCheckedChanged;
		}

		public new GraphColumn Column
		{
			get { return (GraphColumn)base.Column; }
		}

		public bool ShowColors
		{
			get { return _chkShowColors != null ? _chkShowColors.IsChecked : Column.ShowColors; }
			private set
			{
				if(_chkShowColors != null)
				{
					_disableEvents = true;
					_chkShowColors.IsChecked = value;
					_disableEvents = false;
				}
			}
		}

		private void OnColumnShowColorsChanged(object sender, EventArgs e)
		{
			ShowColors = Column.ShowColors;
		}

		private void OnShowColorsCheckedChanged(object sender, EventArgs e)
		{
			if(!_disableEvents)
			{
				_disableEvents = true;
				Column.ShowColors = _chkShowColors.IsChecked;
				_disableEvents = false;
			}
		}
	}
}
