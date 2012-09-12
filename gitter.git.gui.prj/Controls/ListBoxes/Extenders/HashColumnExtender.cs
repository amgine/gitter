namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class HashColumnExtender : BaseExtender
	{
		private readonly HashColumn _column;
		private bool _disableEvents;

		public HashColumnExtender(HashColumn column)
		{
			Verify.Argument.IsNotNull(column, "column");

			_column = column;

			InitializeComponent();

			_chkAbbreviate.Text = Resources.StrAbbreviate;

			Abbreviate = column.Abbreviate;
			SubscribeToColumnEvents();
		}

		private void SubscribeToColumnEvents()
		{
			_column.AbbreviateChanged += OnAbbreviateChanged;
		}

		private void UnsubscribeFromColumnEvents()
		{
			_column.AbbreviateChanged -= OnAbbreviateChanged;
		}

		private void OnAbbreviateChanged(object sender, EventArgs e)
		{
			Abbreviate = _column.Abbreviate;
		}

		public bool Abbreviate
		{
			get { return _chkAbbreviate.Checked; }
			set
			{
				_disableEvents = true;
				_chkAbbreviate.Checked = value;
				_disableEvents = false;
			}
		}

		private void OnAbbreviateCheckedChanged(object sender, EventArgs e)
		{
			if(!_disableEvents)
			{
				_column.Abbreviate = ((CheckBox)sender).Checked;
			}
		}
	}
}
