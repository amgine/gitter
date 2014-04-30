#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	public partial class WebBrowserView : ViewBase
	{
		private static readonly Image _image = Resources.ImgWebBrowser;

		private readonly WebBrowserViewToolbar _toolbar;

		public WebBrowserView()
		{
			InitializeComponent();
		}

		public WebBrowserView(Guid guid, IWorkingEnvironment environment)
			: base(guid, environment)
		{
			InitializeComponent();

			Text = Resources.StrWebBrowser;

			//_webBrowser.DocumentCompleted += OnWebBrowserDocumentCompleted;

			AddTopToolStrip(_toolbar = new WebBrowserViewToolbar(this));
		}

		private void OnWebBrowserDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			Text = _webBrowser.DocumentTitle;
		}

		public override Image Image
		{
			get { return _image; }
		}

		internal protected WebBrowser WebBrowser
		{
			get { return _webBrowser; }
		}

		protected override void AttachViewModel(object viewModel)
		{
			var vm = viewModel as WebBrowserViewModel;
			if(vm != null)
			{
				_webBrowser.Navigate(vm.Url);
			}
		}
	}
}
