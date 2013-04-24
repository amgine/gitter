namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	partial class HashColumnExtender : ExtenderBase
	{
		#region Data

		private ICheckBoxWidget _chkAbbreviate;
		private bool _disableEvents;

		#endregion

		public HashColumnExtender(HashColumn column)
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
			if(_chkAbbreviate != null) _chkAbbreviate.Dispose();
			_chkAbbreviate = Style.CreateCheckBox();
			_chkAbbreviate.IsChecked = Column.Abbreviate;
			_chkAbbreviate.IsCheckedChanged += OnAbbreviateCheckedChanged;
			_chkAbbreviate.Text = Resources.StrAbbreviate;
			_chkAbbreviate.Control.Bounds = new Rectangle(6, 0, 127, 27);
			_chkAbbreviate.Control.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			_chkAbbreviate.Control.Parent = this;
		}

		public new HashColumn Column
		{
			get { return (HashColumn)base.Column; }
		}

		private void SubscribeToColumnEvents()
		{
			Column.AbbreviateChanged += OnAbbreviateChanged;
		}

		private void UnsubscribeFromColumnEvents()
		{
			Column.AbbreviateChanged -= OnAbbreviateChanged;
		}

		private void OnAbbreviateChanged(object sender, EventArgs e)
		{
			Abbreviate = Column.Abbreviate;
		}

		public bool Abbreviate
		{
			get { return _chkAbbreviate != null ? _chkAbbreviate.IsChecked : Column.Abbreviate; }
			private set
			{
				if(_chkAbbreviate != null)
				{
					_disableEvents = true;
					_chkAbbreviate.IsChecked = value;
					_disableEvents = false;
				}
			}
		}

		private void OnAbbreviateCheckedChanged(object sender, EventArgs e)
		{
			if(!_disableEvents)
			{
				_disableEvents = true;
				Column.Abbreviate = _chkAbbreviate.IsChecked;
				_disableEvents = false;
			}
		}
	}
}
