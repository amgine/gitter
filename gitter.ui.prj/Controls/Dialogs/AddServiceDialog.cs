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

namespace gitter.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using gitter.Framework;

	using Resources = gitter.Properties.Resources;

	public partial class AddServiceDialog : PickerDialog<ServiceProviderPicker, IRepositoryServiceProvider>, IExecutableDialog
	{
		#region Data

		private readonly IWorkingEnvironment _environment;

		#endregion

		#region .ctor

		public AddServiceDialog(IWorkingEnvironment environment)
			: base(Resources.StrProvider.AddColon())
		{
			Verify.Argument.IsNotNull(environment, "environment");

			_environment = environment;

			Text = Resources.StrAddService;
		}

		#endregion

		#region Properties

		protected override string ActionVerb
		{
			get { return Resources.StrAdd; }
		}

		#endregion

		#region Methods

		protected override void LoadItems(ServiceProviderPicker picker)
		{
			var hs = new HashSet<IRepositoryServiceProvider>(_environment.ActiveIssueTrackerProviders);
			foreach(var prov in _environment.IssueTrackerProviders)
			{
				if(!hs.Contains(prov))
				{
					var item = new ServiceProviderListItem(prov);
					picker.DropDownItems.Add(item);
				}
			}
		}

		protected override Control CreateControl(IRepositoryServiceProvider item)
		{
			if(item == null)
			{
				return null;
			}
			return item.CreateSetupDialog(_environment.ActiveRepository);
		}

		public override bool Execute()
		{
			var provider = SelectedValue;
			if(provider == null)
			{
				return false;
			}
			if(!base.Execute())
			{
				return false;
			}
			if(!_environment.TryLoadIssueTracker(provider))
			{
				return false;
			}
			return true;
		}

		#endregion
	}
}
