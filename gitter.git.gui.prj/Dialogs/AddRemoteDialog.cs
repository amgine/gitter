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

namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Options;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class AddRemoteDialog : GitDialogBase, IExecutableDialog
	{
		private readonly Repository _repository;

		public AddRemoteDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();
			Localize();

			SetupReferenceNameInputBox(_txtName, ReferenceType.Remote);

			if(_repository.Remotes.Count == 0)
			{
				_txtName.Text = GitConstants.DefaultRemoteName;
			}

			GitterApplication.FontManager.InputFont.Apply(_txtName, _txtUrl);
		}

		private void Localize()
		{
			Text = Resources.StrAddRemote;

			_lblName.Text			= Resources.StrName.AddColon();
			_lblUrl.Text			= Resources.StrUrl.AddColon();
			
			_grpOptions.Text		= Resources.StrOptions;
			_chkFetch.Text			= Resources.StrsFetchRemote;
			_chkMirror.Text			= Resources.StrMirror;

			_grpBranches.Text		= Resources.StrTrackingBranches;
			_trackAllBranches.Text	= Resources.StrlTrackAllBranches;
			_trackSpecified.Text	= Resources.StrlTrackSpecifiedBranches.AddColon();

			_grpTagImport.Text		= Resources.StrsTagFetchMode;
			_tagFetchDefault.Text	= Resources.StrDefault;
			_tagFetchAll.Text		= Resources.StrsFetchAll;
			_tagFetchNone.Text		= Resources.StrsFetchNone;
		}

		#region Properties

		protected override string ActionVerb
		{
			get { return Resources.StrAdd; }
		}

		public Repository Repository
		{
			get { return _repository; }
		}

		public string RemoteName
		{
			get { return _txtName.Text; }
			set { _txtName.Text = value; }
		}

		public string Url
		{
			get { return _txtUrl.Text; }
			set { _txtUrl.Text = value; }
		}

		public bool Fetch
		{
			get { return _chkFetch.Checked; }
			set { _chkFetch.Checked = value; }
		}

		public bool Mirror
		{
			get { return _chkMirror.Checked; }
			set { _chkMirror.Checked = value; }
		}

		public TagFetchMode TagFetchMode
		{
			get
			{
				if(_tagFetchAll.Checked)
				{
					return TagFetchMode.AllTags;
				}
				if(_tagFetchNone.Checked)
				{
					return TagFetchMode.NoTags;
				}
				return TagFetchMode.Default;
			}
			set
			{
				switch(value)
				{
					case TagFetchMode.AllTags:
						_tagFetchAll.Checked = true;
						break;
					case TagFetchMode.NoTags:
						_tagFetchNone.Checked = true;
						break;
					case TagFetchMode.Default:
						_tagFetchDefault.Checked = true;
						break;
					default:
						throw new ArgumentException("value");
				}
			}
		}

		#endregion

		#region IExecutableDialog

		public bool Execute()
		{
			var name = _txtName.Text.Trim();
			if(!ValidateNewRemoteName(name, _txtName, Repository))
			{
				return false;
			}
			var url = _txtUrl.Text.Trim();
			if(!ValidateUrl(url, _txtUrl))
			{
				return false;
			}
			bool fetch			= Fetch;
			bool mirror			= Mirror;
			var tagFetchMode	= TagFetchMode;
			try
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					Repository.Remotes.AddRemote(name, url, fetch, mirror, tagFetchMode);
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToAddRemote,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		#endregion
	}
}
