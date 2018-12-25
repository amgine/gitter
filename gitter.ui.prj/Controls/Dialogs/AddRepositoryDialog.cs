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

namespace gitter
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Options;
	using gitter.Framework.Services;

	using Resources = gitter.Properties.Resources;

	internal partial class AddRepositoryDialog : DialogBase, IExecutableDialog
	{
		#region Data

		private readonly IWorkingEnvironment _environment;
		private readonly LocalRepositoriesListBox _repositoryList;

		#endregion

		public AddRepositoryDialog(IWorkingEnvironment environment, LocalRepositoriesListBox repositoriyList)
		{
			Verify.Argument.IsNotNull(environment, nameof(environment));
			Verify.Argument.IsNotNull(repositoriyList, nameof(repositoriyList));

			_environment = environment;
			_repositoryList = repositoriyList;

			InitializeComponent();

			Text = Resources.StrAddRepository;

			_lblPath.Text = Resources.StrPath.AddColon();
			_lblDescription.Text = Resources.StrDescription.AddColon();

			GitterApplication.FontManager.InputFont.Apply(_txtPath, _txtDescription);
		}

		protected override string ActionVerb
		{
			get { return Resources.StrAdd; }
		}

		public string Path
		{
			get { return _txtPath.Text; }
			set { _txtPath.Text = value; }
		}

		public bool AllowChangePath
		{
			get { return !_txtPath.ReadOnly; }
			set { _txtPath.ReadOnly = !value; }
		}

		public string Description
		{
			get { return _txtDescription.Text; }
			set { _txtDescription.Text = value; }
		}

		private void _btnSelectDirectory_Click(object sender, EventArgs e)
		{
			var path = Utility.ShowPickFolderDialog(this);
			if(path != null)
			{
				_txtPath.Text = path;
			}
		}

		public bool Execute()
		{
			var path = _txtPath.Text.Trim();
			if(path.Length == 0)
			{
				NotificationService.NotifyInputError(_txtPath,
					Resources.ErrInvalidPath,
					Resources.ErrPathCannotBeEmpty);
				return false;
			}
			var prov = _environment.FindProviderForDirectory(path);
			if(prov == null)
			{
				NotificationService.NotifyInputError(_txtPath,
					Resources.ErrInvalidPath,
					Resources.ErrPathIsNotValidRepository.UseAsFormat(path));
				return false;
			}
			var item = new RepositoryListItem(new RepositoryLink(path, prov.Name) { Description = _txtDescription.Text });
			_repositoryList.Items.Add(item);
			return true;
		}
	}
}
