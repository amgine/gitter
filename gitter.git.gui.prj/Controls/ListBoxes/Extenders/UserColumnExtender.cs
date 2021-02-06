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

	/// <summary>Extender for <see cref="UserColumn"/>.</summary>
	[ToolboxItem(false)]
	partial class UserColumnExtender : ExtenderBase
	{
		#region Data

		private ICheckBoxWidget _chkShowEmail;
		private bool _disableEvents;

		#endregion

		/// <summary>Create <see cref="UserColumnExtender"/>.</summary>
		/// <param name="column">Related column.</param>
		public UserColumnExtender(UserColumn column)
			: base(column)
		{
			InitializeComponent();
			CreateControls();
			SubscribeToColumnEvents();
		}

		private void CreateControls()
		{
			_chkShowEmail?.Dispose();
			_chkShowEmail = Style.CreateCheckBox();
			_chkShowEmail.IsChecked = Column.ShowEmail;
			_chkShowEmail.IsCheckedChanged += OnShowEmailCheckedChanged;
			_chkShowEmail.Image = CachedResources.Bitmaps["ImgMail"];
			_chkShowEmail.Text = Resources.StrShowEmail;
			_chkShowEmail.Control.Bounds = new Rectangle(6, 0, 127, 27);
			_chkShowEmail.Control.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			_chkShowEmail.Control.Parent = this;
		}

		protected override void OnStyleChanged()
		{
			base.OnStyleChanged();
			CreateControls();
		}

		public new UserColumn Column => (UserColumn)base.Column;

		private void SubscribeToColumnEvents()
		{
			Column.ShowEmailChanged += OnColumnShowEmailChanged;
		}

		private void UnsubscribeFromColumnEvents()
		{
			Column.ShowEmailChanged -= OnColumnShowEmailChanged;
		}

		private void OnColumnShowEmailChanged(object sender, EventArgs e)
		{
			ShowEmail = Column.ShowEmail;
		}

		public bool ShowEmail
		{
			get => _chkShowEmail != null ? _chkShowEmail.IsChecked : Column.ShowEmail;
			private set
			{
				if(_chkShowEmail != null)
				{
					_disableEvents = true;
					_chkShowEmail.IsChecked = value;
					_disableEvents = false;
				}
			}
		}

		private void OnShowEmailCheckedChanged(object sender, EventArgs e)
		{
			if(!_disableEvents)
			{
				_disableEvents = true;
				Column.ShowEmail = _chkShowEmail.IsChecked;
				_disableEvents = false;
			}
		}
	}
}
