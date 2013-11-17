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

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Dialog for stashing working directory changes.</summary>
	[ToolboxItem(false)]
	public partial class StashSaveDialog : GitDialogBase, IExecutableDialog
	{
		#region Data

		private Repository _repository;
		private TextBoxSpellChecker _speller;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="StashSaveDialog"/>.</summary>
		/// <param name="repository">Repository for performing stash save.</param>
		/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		public StashSaveDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();

			Text = Resources.StrStashSave;

			_lblMessage.Text = Resources.StrOptionalMessage.AddColon();
			_chkKeepIndex.Text = Resources.StrKeepIndex;
			_chkIncludeUntrackedFiles.Text = Resources.StrsIncludeUntrackedFiles;
			if(!GitFeatures.StashIncludeUntrackedOption.IsAvailableFor(repository))
			{
				_chkIncludeUntrackedFiles.Enabled = false;
				_chkIncludeUntrackedFiles.Text += " " +
					Resources.StrfVersionRequired
							 .UseAsFormat(GitFeatures.StashIncludeUntrackedOption.RequiredVersion)
							 .SurroundWithBraces();
			}

			ToolTipService.Register(_chkKeepIndex, Resources.TipStashKeepIndex);

			GitterApplication.FontManager.InputFont.Apply(_txtMessage);

			if(SpellingService.Enabled)
			{
				_speller = new TextBoxSpellChecker(_txtMessage, true);
			}
		}

		#endregion

		#region Properties

		public Repository Repository
		{
			get { return _repository; }
		}

		/// <summary>Do not stash staged changes.</summary>
		public bool KeepIndex
		{
			get { return _chkKeepIndex.Checked; }
			set { _chkKeepIndex.Checked = value; }
		}

		/// <summary>Include untracked files in stash.</summary>
		public bool IncludeUntrackedFiles
		{
			get { return _chkIncludeUntrackedFiles.Checked; }
			set { _chkIncludeUntrackedFiles.Checked = value; }
		}

		/// <summary>Custom commit message (optional).</summary>
		public string Message
		{
			get { return _txtMessage.Text; }
			set { _txtMessage.Text = value; }
		}

		protected override string ActionVerb
		{
			get { return Resources.StrSave; }
		}

		#endregion

		#region IExecutableDialog Members

		/// <summary>Perform stash save.</summary>
		/// <returns>true if stash save succeeded.</returns>
		public bool Execute()
		{
			bool keepIndex = KeepIndex;
			bool includeUntracked =
				GitFeatures.StashIncludeUntrackedOption.IsAvailableFor(_repository) &&
				IncludeUntrackedFiles;
			var message = Message;
			message = message == null ? string.Empty : message.Trim();

			if(GuiCommands.SaveStash(this, Repository.Stash, keepIndex, includeUntracked, message) == GuiCommandStatus.Faulted)
			{
				return false;
			}
			return true;
		}

		#endregion
	}
}
