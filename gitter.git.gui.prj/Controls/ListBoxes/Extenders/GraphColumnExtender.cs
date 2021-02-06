#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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

		public new GraphColumn Column => (GraphColumn)base.Column;

		public bool ShowColors
		{
			get => _chkShowColors != null ? _chkShowColors.IsChecked : Column.ShowColors;
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
