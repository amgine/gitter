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
	using System.Windows.Forms;

	using gitter.Framework;

	using Resources = gitter.Properties.Resources;

	public partial class CloneRepositoryDialog : PickerDialog<RepositoryProviderPicker, IRepositoryProvider>, IExecutableDialog
	{
		public CloneRepositoryDialog(IWorkingEnvironment workingEnvironment)
			: base(Resources.StrVCS.AddColon())
		{
			Verify.Argument.IsNotNull(workingEnvironment, nameof(workingEnvironment));

			WorkingEnvironment = workingEnvironment;

			Text = Resources.StrCloneRepository;
		}

		protected override string ActionVerb => Resources.StrClone;

		protected override int MinimumSelectableItems => 2;

		private IWorkingEnvironment WorkingEnvironment { get; }

		protected override void LoadItems(RepositoryProviderPicker picker)
		{
			foreach(var provider in WorkingEnvironment.RepositoryProviders)
			{
				var item = new RepositoryProviderListItem(provider);
				picker.DropDownItems.Add(item);
			}
		}

		protected override Control CreateControl(IRepositoryProvider item)
		{
			if(item == null)
			{
				return null;
			}
			return item.CreateCloneDialog();
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
			if(SelectedControl is IRepositoryCloneDialog cloneDialog)
			{
				var repositoryPath = cloneDialog.RepositoryPath.Value;
				WorkingEnvironment.BeginInvoke(
					new Func<string, bool>(WorkingEnvironment.OpenRepository),
					new object[] { repositoryPath });
			}
			return true;
		}
	}
}
