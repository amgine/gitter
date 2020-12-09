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

namespace gitter.Framework
{
	using System;
	using System.Drawing;
	using gitter.Framework.Configuration;

	using Resources = gitter.Framework.Properties.Resources;

	sealed class ExplorerContextMenuFeature : IIntegrationFeature
	{
		public event EventHandler IsEnabledChanged;

		private void OnIsEnabledChanged() => IsEnabledChanged?.Invoke(this, EventArgs.Empty);

		public string Name => "ExplorerContextMenu";

		public string DisplayText => Resources.StrsExplorerContextMenuFeature;

		public Bitmap Icon => CachedResources.Bitmaps["ImgFolder"];

		public bool IsEnabled
		{
			get => GlobalOptions.IsIntegratedInExplorerContextMenu;
			set
			{
				if(value)
				{
					GlobalOptions.IntegrateInExplorerContextMenu();
				}
				else
				{
					GlobalOptions.RemoveFromExplorerContextMenu();
				}
			}
		}

		public bool AdministratorRightsRequired => true;

		public Action GetEnableAction(bool enable)
			=> enable ?
				(Action)GlobalOptions.IntegrateInExplorerContextMenu :
				(Action)GlobalOptions.RemoveFromExplorerContextMenu;

		bool IIntegrationFeature.HasConfiguration => false;

		void IIntegrationFeature.SaveTo(Section section)
		{
		}

		void IIntegrationFeature.LoadFrom(Section section)
		{
		}
	}
}
