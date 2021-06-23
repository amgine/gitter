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

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	partial class HashColumnExtender : ExtenderBase
	{
		#region Data

		private ICheckBoxWidget _chkAbbreviate;
		private bool _disableEvents;

		#endregion

		public HashColumnExtender(HashColumnBase column)
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
			var conv = new DpiConverter(this);

			_chkAbbreviate?.Dispose();
			_chkAbbreviate = Style.CreateCheckBox();
			_chkAbbreviate.IsChecked = Column.Abbreviate;
			_chkAbbreviate.IsCheckedChanged += OnAbbreviateCheckedChanged;
			_chkAbbreviate.Text = Resources.StrAbbreviate;
			_chkAbbreviate.Control.Bounds = new Rectangle(conv.ConvertX(6), 0, Width - conv.ConvertX(6) * 2, conv.ConvertY(27));
			_chkAbbreviate.Control.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			_chkAbbreviate.Control.Parent = this;
		}

		public new HashColumnBase Column => (HashColumnBase)base.Column;

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
			get => _chkAbbreviate != null ? _chkAbbreviate.IsChecked : Column.Abbreviate;
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
