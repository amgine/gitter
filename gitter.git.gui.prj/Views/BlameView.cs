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

namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;

	using gitter.Framework;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	internal partial class BlameView : GitViewBase
	{
		#region Data

		private BlameFileBinding _blameFileBinding;

		#endregion

		#region .ctor

		public BlameView(GuiProvider gui)
			: base(Guids.BlameViewGuid, gui)
		{
			InitializeComponent();

			Text = Resources.StrlBlame;
		}

		#endregion

		#region Properties

		public override IImageProvider ImageProvider { get; } = new ScaledImageProvider(CachedResources.ScaledBitmaps, @"blame");

		public override bool IsDocument => true;

		private BlameFileBinding BlameFileBinding
		{
			get => _blameFileBinding;
			set
			{
				if(_blameFileBinding != value)
				{
					_blameFileBinding?.Dispose();
					_blameFileBinding = value;
					_blameFileBinding?.ReloadData();
				}
			}
		}

		#endregion

		#region Methods

		protected override void AttachViewModel(object viewModel)
		{
			base.AttachViewModel(viewModel);

			if(viewModel is not BlameViewModel vm) return;

			var blameSource = vm.BlameSource;
			if(blameSource != null)
			{
				Text = Resources.StrBlame + ": " + blameSource.ToString();
				BlameFileBinding = new BlameFileBinding(blameSource, _blamePanel, BlameOptions.Default);
			}
			else
			{
				Text = Resources.StrBlame;
				BlameFileBinding = null;
			}
		}

		#endregion
	}
}
