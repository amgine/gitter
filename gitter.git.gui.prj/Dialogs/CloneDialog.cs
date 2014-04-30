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

namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.IO;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Mvc;
	using gitter.Framework.Mvc.WinForms;

	using gitter.Git.Gui.Controllers;
	using gitter.Git.Gui.Interfaces;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	partial class CloneDialog : GitDialogBase, IExecutableDialog, ICloneView
	{
		#region Helpers

		private sealed class RepositoryPathInput : IUserInputSource<string>, IWin32ControlInputSource
		{
			#region Data

			private readonly TextBoxBase _txtPath;
			private readonly TextBoxBase _txtUrl;
			private readonly CheckBox _chkAppendUrlToPath;

			#endregion

			#region Evrnts

			public event EventHandler ValueChanged;

			private void OnValueChanged()
			{
				var handler = ValueChanged;
				if(handler != null) handler(this, EventArgs.Empty);
			}

			#endregion

			#region .ctor

			public RepositoryPathInput(TextBoxBase txtPath, TextBoxBase txtUrl, CheckBox chkAppendUrlToPath)
			{
				Verify.Argument.IsNotNull(txtPath, "txtPath");
				Verify.Argument.IsNotNull(txtUrl, "txtUrl");
				Verify.Argument.IsNotNull(chkAppendUrlToPath, "chkAppendUrlToPath");

				_txtPath = txtPath;
				_txtUrl = txtUrl;
				_chkAppendUrlToPath = chkAppendUrlToPath;

				_chkAppendUrlToPath.CheckedChanged += OnAppendUrlToPathCheckedChanged;
				_txtPath.TextChanged += OnPathTextChanged;
				_txtUrl.TextChanged  += OnUrlTextChanged;
			}

			#endregion

			#region Methods

			private void OnAppendUrlToPathCheckedChanged(object sender, EventArgs e)
			{
				OnValueChanged();
			}

			private void OnPathTextChanged(object sender, EventArgs e)
			{
				OnValueChanged();
			}

			private void OnUrlTextChanged(object sender, EventArgs e)
			{
				if(_chkAppendUrlToPath.Checked)
				{
					OnValueChanged();
				}
			}

			private static string AppendUrlToPath(string path, string url)
			{
				if(!string.IsNullOrWhiteSpace(url))
				{
					try
					{
						path = Path.Combine(path, GitUtils.GetHumanishName(url));
					}
					catch(Exception exc)
					{
						if(exc.IsCritical())
						{
							throw;
						}
					}
				}
				return path;
			}

			#endregion

			#region IUserInputSource<string> Members

			public string Value
			{
				get
				{
					if(_chkAppendUrlToPath.Checked)
					{
						return AppendUrlToPath(_txtPath.Text.Trim(), _txtUrl.Text.Trim());
					}
					else
					{
						return _txtPath.Text;
					}
				}
				set
				{
					_chkAppendUrlToPath.Checked = false;
					_txtPath.Text = value;
				}
			}

			#endregion

			#region IUserInputSource Members

			public bool IsReadOnly
			{
				get { return _txtUrl.ReadOnly; }
				set
				{
					_txtUrl.ReadOnly = value;
					_chkAppendUrlToPath.Enabled = value;
				}
			}

			#endregion

			#region IWin32ControlInputSource Members

			public Control Control
			{
				get { return _txtPath; }
			}

			#endregion
		}

		#endregion

		#region Data

		private readonly IGitRepositoryProvider _gitRepositoryProvider;
		private readonly IUserInputSource<string> _urlInput;
		private readonly RepositoryPathInput _repositoryPathInput;
		private readonly IUserInputSource<string> _remoteNameInput;
		private readonly IUserInputSource<bool> _shallowCloneInput;
		private readonly IUserInputSource<int> _depthInput;
		private readonly IUserInputSource<bool> _useTemplateInput;
		private readonly IUserInputSource<string> _templatePathInput;
		private readonly IUserInputSource<bool> _bareInput;
		private readonly IUserInputSource<bool> _mirrorInput;
		private readonly IUserInputSource<bool> _noCheckoutInput;
		private readonly IUserInputSource<bool> _recursiveInput;
		private readonly IUserInputErrorNotifier _errorNotifier;
		private readonly ICloneController _controller;

		#endregion

		#region .ctor

		public CloneDialog(IGitRepositoryProvider gitRepositoryProvider)
		{
			Verify.Argument.IsNotNull(gitRepositoryProvider, "gitRepositoryProvider");

			_gitRepositoryProvider = gitRepositoryProvider;

			InitializeComponent();
			Localize();

			var inputs = new IUserInputSource[]
			{
				_urlInput            = new TextBoxInputSource(_txtUrl),
				_repositoryPathInput = new RepositoryPathInput(_txtPath, _txtUrl, _chkAppendRepositoryNameFromUrl),
				_remoteNameInput     = new TextBoxInputSource(_txtRemoteName),
				_shallowCloneInput   = new CheckBoxInputSource(_chkShallowClone),
				_depthInput          = new NumericUpDownInputSource<int>(_numDepth, value => (int)value, value => value),
				_useTemplateInput    = new CheckBoxInputSource(_chkUseTemplate),
				_templatePathInput   = new TextBoxInputSource(_txtTemplate),
				_bareInput           = new CheckBoxInputSource(_chkBare),
				_mirrorInput         = new CheckBoxInputSource(_chkMirror),
				_noCheckoutInput     = new CheckBoxInputSource(_chkNoCheckout),
				_recursiveInput      = new CheckBoxInputSource(_chkRecursive),
			};
			_errorNotifier = new UserInputErrorNotifier(NotificationService, inputs);
			_repositoryPathInput.ValueChanged += (s, e) => UpdateTargetPathText();

			UpdateTargetPathText();

			GitterApplication.FontManager.InputFont.Apply(_txtUrl, _txtPath, _txtRemoteName);

			_controller = new CloneController(gitRepositoryProvider) { View = this };
		}

		#endregion

		#region Properties

		protected override string ActionVerb
		{
			get { return Resources.StrClone; }
		}

		private IGitRepositoryProvider GitRepositoryProvider
		{
			get { return _gitRepositoryProvider; }
		}

		public IUserInputSource<string> Url
		{
			get { return _urlInput; }
		}

		public IUserInputSource<string> RepositoryPath
		{
			get { return _repositoryPathInput; }
		}

		public IUserInputSource<bool> Bare
		{
			get { return _bareInput; }
		}

		public IUserInputSource<bool> Mirror
		{
			get { return _mirrorInput; }
		}

		public IUserInputSource<bool> UseTemplate
		{
			get { return _useTemplateInput; }
		}

		public IUserInputSource<string> RemoteName
		{
			get { return _remoteNameInput; }
		}

		public IUserInputSource<string> TemplatePath
		{
			get { return _templatePathInput; }
		}

		public IUserInputSource<bool> ShallowClone
		{
			get { return _shallowCloneInput; }
		}

		public IUserInputSource<int> Depth
		{
			get { return _depthInput; }
		}

		public IUserInputSource<bool> Recursive
		{
			get { return _recursiveInput; }
		}

		public IUserInputSource<bool> NoCheckout
		{
			get { return _noCheckoutInput; }
		}

		public IUserInputErrorNotifier ErrorNotifier
		{
			get { return _errorNotifier; }
		}

		#endregion

		#region Methods

		private void Localize()
		{
			Text = Resources.StrCloneRepository;

			_lblPath.Text = Resources.StrPath.AddColon();
			_lblUrl.Text = Resources.StrUrl.AddColon();

			_grpOptions.Text = Resources.StrOptions;

			_chkAppendRepositoryNameFromUrl.Text = Resources.StrsAppendRepositoryNameFromURL;
			_lblWillBeClonedInto.Text = Resources.StrsWillBeClonedInto.AddColon();
			_chkUseTemplate.Text = Resources.StrTemplate.AddColon();
			_lblRemoteName.Text = Resources.StrsRemoteName.AddColon();
			_txtRemoteName.Text = GitConstants.DefaultRemoteName;
			_chkBare.Text = Resources.StrBare;
			_chkMirror.Text = Resources.StrMirror;
			_chkShallowClone.Text = Resources.StrsShallowClone;
			_chkRecursive.Text = Resources.StrRecursive;
			_lblDepth.Text = Resources.StrDepth.AddColon();
			_chkNoCheckout.Text = Resources.StrsNoCheckout;
		}

		private static string AppendUrlToPath(string path, string url)
		{
			if(url.Length != 0)
			{
				try
				{
					path = System.IO.Path.Combine(path, GitUtils.GetHumanishName(url));
				}
				catch(Exception exc)
				{
					if(exc.IsCritical())
					{
						throw;
					}
				}
			}
			return path;
		}

		private void _btnSelectDirectory_Click(object sender, EventArgs e)
		{
			var path = Utility.ShowPickFolderDialog(this);
			if(path != null)
			{
				_txtPath.Text = path;
			}
		}

		private void _btnSelectTemplate_Click(object sender, EventArgs e)
		{
			var path = Utility.ShowPickFolderDialog(this);
			if(path != null)
			{
				_txtTemplate.Text = path;
			}
		}

		private void UpdateTargetPathText()
		{
			var path = RepositoryPath.Value;
			if(string.IsNullOrWhiteSpace(path))
			{
				_lblRealPath.Text = Resources.StrlNoPathSpecified.SurroundWith("<", ">");
			}
			else
			{
				_lblRealPath.Text = path;
			}
		}

		private void _chkUseTemplate_CheckedChanged(object sender, EventArgs e)
		{
			bool enabled = _chkUseTemplate.Checked;
			_txtTemplate.Enabled = enabled;
			_btnSelectTemplate.Enabled = enabled;
		}

		private void _chkShallowClone_CheckedChanged(object sender, EventArgs e)
		{
			bool enabled = _chkShallowClone.Checked;
			_lblDepth.Enabled = enabled;
			_numDepth.Enabled = enabled;
		}

		private void _chkBare_CheckedChanged(object sender, EventArgs e)
		{
			_chkMirror.Enabled = _chkBare.Checked;
		}

		#endregion

		#region IExecutableDialog

		public bool Execute()
		{
			return _controller.TryClone();
		}

		#endregion
	}
}
