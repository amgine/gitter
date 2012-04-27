namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Options;
	using gitter.Framework.Services;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Dialog for diaplaying and/or editing <see cref="Remote"/> object properties.</summary>
	public partial class RemotePropertiesDialog : GitDialogBase, IExecutableDialog
	{
		private Remote _remote;

		private sealed class RefspecItem : CustomListBoxItem<string>
		{
			private readonly bool _forced;
			private readonly string _from;
			private readonly string _to;
			private readonly bool _fetch;

			private static readonly Bitmap ImgPush = CachedResources.Bitmaps["ImgPush"];
			private static readonly Bitmap ImgPull = CachedResources.Bitmaps["ImgPull"];
			private static readonly Bitmap ImgPlus = CachedResources.Bitmaps["ImgPlus"];

			public RefspecItem(string refspec, bool fetch)
				: base(refspec)
			{
				_fetch = fetch;
				int pos = refspec.IndexOf(':');
				int start;
				if(refspec[0] == '+')
				{
					start = 1;
					_forced = true;
				}
				else
				{
					start = 0;
				}
				if(pos == -1)
				{
					_from = refspec.Substring(start, refspec.Length - start);
					_to = "";
				}
				else
				{
					_from = refspec.Substring(start, pos - start);
					_to = refspec.Substring(pos + 1);
				}
			}

			public bool Fetch
			{
				get { return _fetch; }
			}

			public bool Forced
			{
				get { return _forced; }
			}

			protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
			{
				switch(measureEventArgs.SubItemId)
				{
					case 0:
						return measureEventArgs.MeasureImage(_fetch ? ImgPull : ImgPush);
					case 1:
						return measureEventArgs.MeasureImage(ImgPlus);
					case 2:
						return measureEventArgs.MeasureText(_from);
					case 3:
						return measureEventArgs.MeasureText(_to);
					default:
						return Size.Empty;
				}
			}

			protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
			{
				switch(paintEventArgs.SubItemId)
				{
					case 0:
						paintEventArgs.PaintImage(_fetch ? ImgPull : ImgPush);
						break;
					case 1:
						if(_forced)
							paintEventArgs.PaintImage(ImgPlus);
						break;
					case 2:
						paintEventArgs.PaintText(_from);
						break;
					case 3:
						paintEventArgs.PaintText(_to);
						break;
				}
			}
		}

		/// <summary>Create <see cref="RemotePropertiesDialog"/>.</summary>
		/// <param name="remote">Related remote.</param>
		public RemotePropertiesDialog(Remote remote)
		{
			if(remote == null) throw new ArgumentNullException("remote");
			_remote = remote;

			InitializeComponent();

			Text = string.Format("{0}: {1}", Resources.StrProperties, remote.Name);

			_lblFetchURL.Text = Resources.StrFetchUrl.AddColon();
			_lblPushURL.Text = Resources.StrPushUrl.AddColon();
			_lblProxy.Text = Resources.StrProxy.AddColon();
			_lblVCS.Text = Resources.StrVCS.AddColon();

			_grpUpdatedReferences.Text = Resources.StrUpdatedReferences;
			_lblRefspec.Text = Resources.StrRefspec.AddColon();
			_btnAddRefspec.Text = Resources.StrAdd;

			_grpOptions.Text = Resources.StrDefaultBehavior;

			_lblReceivePack.Text = Resources.StrReceivePack.AddColon();
			_lblUploadPack.Text = Resources.StrUploadPack.AddColon();

			_chkMirror.Text = Resources.StrMirror;
			_chkSkipFetchAll.Text = Resources.StrSkipFetchAll;
			_lblFetchTags.Text = Resources.StrFetchTags.AddColon();

			_radNormal.Text = Resources.StrDefault;
			_radFetchAll.Text = Resources.StrAll;
			_radFetchNone.Text = Resources.StrNone;

			GitterApplication.FontManager.InputFont.Apply(_txtFetchURL, _txtPushURL, _txtProxy, _txtVCS, _txtRefspec, _txtReceivePack, _txtUploadPack);

			_txtFetchURL.Text = remote.FetchUrl;
			_txtPushURL.Text = remote.PushUrl;
			_txtProxy.Text = remote.Proxy;
			_txtVCS.Text = remote.VCS;
			_txtReceivePack.Text = remote.ReceivePack;
			_txtUploadPack.Text = remote.ReceivePack;
			_chkMirror.Checked = remote.Mirror;
			_chkSkipFetchAll.Checked = remote.SkipFetchAll;
			TagFetchMode = remote.TagFetchMode;

			_radFetch.Text = Resources.StrFetch;
			_radPush.Text = Resources.StrPush;

			_lstUpdatedReferences.Columns.AddRange(new[]
				{
					new CustomListBoxColumn(0, "") { SizeMode = ColumnSizeMode.Fixed, Width = 20 },
					new CustomListBoxColumn(1, "") { SizeMode = ColumnSizeMode.Fixed, Width = 20 },
					new CustomListBoxColumn(2, Resources.StrFrom) { SizeMode = ColumnSizeMode.Fill },
					new CustomListBoxColumn(3, Resources.StrTo) { SizeMode = ColumnSizeMode.Fill },
				});
			var fetchRefspecs = remote.FetchRefspec.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			for(int i = 0; i < fetchRefspecs.Length; ++i)
			{
				_lstUpdatedReferences.Items.Add(new RefspecItem(fetchRefspecs[i], true));
			}
			var pushRefspecs = remote.PushRefspec.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			for(int i = 0; i < pushRefspecs.Length; ++i)
			{
				_lstUpdatedReferences.Items.Add(new RefspecItem(pushRefspecs[i], false));
			}

			ToolTipService.Register(_txtFetchURL, Resources.TipRemoteFetchURL);
			ToolTipService.Register(_txtPushURL, Resources.TipRemotePushURL);
			ToolTipService.Register(_txtProxy, Resources.TipRemoteProxy);
			ToolTipService.Register(_txtVCS, Resources.TipRemoteVCS);
			ToolTipService.Register(_txtUploadPack, Resources.TipRemoteUploadPack);
			ToolTipService.Register(_txtReceivePack, Resources.TipRemoteReceivePack);
			ToolTipService.Register(_chkMirror, Resources.TipRemoteMirror);
			ToolTipService.Register(_chkSkipFetchAll, Resources.TipRemoteSkipFetchAll);

			_lstUpdatedReferences.KeyDown += OnUpdatedReferencesKeyDown;
		}

		private void OnUpdatedReferencesKeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Delete)
			{
				while(_lstUpdatedReferences.SelectedItems.Count != 0)
				{
					_lstUpdatedReferences.SelectedItems[0].Remove();
				}
			}
		}

		private void GetRefspecs(out string fetch, out string push)
		{
			var sbfetch = new StringBuilder();
			var sbpush = new StringBuilder();
			foreach(RefspecItem refspec in _lstUpdatedReferences.Items)
			{
				StringBuilder sb = (refspec.Fetch) ? sbfetch : sbpush;
				if(sb.Length != 0)
					sb.Append(' ');
				sb.Append(refspec.DataContext);
			}
			fetch = sbfetch.ToString();
			push = sbpush.ToString();
		}

		public Remote Remote
		{
			get { return _remote; }
		}

		public string FetchURL
		{
			get { return _txtFetchURL.Text; }
			set { _txtFetchURL.Text = value; }
		}

		public string PushURL
		{
			get { return _txtPushURL.Text; }
			set { _txtPushURL.Text = value; }
		}

		public string Proxy
		{
			get { return _txtProxy.Text; }
			set { _txtProxy.Text = value; }
		}

		public string VCS
		{
			get { return _txtVCS.Text; }
			set { _txtVCS.Text = value; }
		}

		public bool Mirror
		{
			get { return _chkMirror.Checked; }
			set { _chkMirror.Checked = value; }
		}

		public bool SkipFetchAll
		{
			get { return _chkSkipFetchAll.Checked; }
			set { _chkSkipFetchAll.Checked = value; }
		}

		public TagFetchMode TagFetchMode
		{
			get
			{
				if(_radFetchAll.Checked)
					return TagFetchMode.AllTags;
				if(_radFetchNone.Checked)
					return TagFetchMode.NoTags;
				return TagFetchMode.Default;
			}
			set
			{
				switch(value)
				{
					case TagFetchMode.AllTags:
						_radFetchAll.Checked = true;
						break;
					case TagFetchMode.NoTags:
						_radFetchNone.Checked = true;
						break;
					default:
						_radNormal.Checked = true;
						break;
				}
			}
		}

		private void _btnAddRefspec_Click(object sender, EventArgs e)
		{
			var refspec = _txtRefspec.Text.Trim();
			if(refspec == "")
			{
				NotificationService.NotifyInputError(
					_txtRefspec,
					Resources.ErrInvalidRefspec,
					Resources.ErrRefspecCannotBeEmpty);
				return;
			}
			if(refspec.IndexOf(' ') != -1)
			{
				NotificationService.NotifyInputError(
					_txtRefspec,
					Resources.ErrInvalidRefspec,
					Resources.ErrRefspecCannotContainSpaces);
				return;
			}
			_lstUpdatedReferences.Items.Add(new RefspecItem(refspec, _radFetch.Checked));
			_txtRefspec.Clear();
		}

		#region IExecutableDialog Members

		public bool Execute()
		{
			Cursor = Cursors.WaitCursor;
			string fetchUrl = _txtFetchURL.Text.Trim();
			string pushUrl = _txtPushURL.Text.Trim();
			string proxy = _txtProxy.Text.Trim();
			string vcs = _txtVCS.Text.Trim();
			string fetch;
			string push;
			GetRefspecs(out fetch, out push);
			string receivePack = _txtReceivePack.Text.Trim();
			string uploadPack = _txtUploadPack.Text.Trim();
			bool mirror = _chkMirror.Checked;
			bool skipFetchAll = _chkSkipFetchAll.Checked;
			var tagFetchMode = TagFetchMode;
			try
			{
				lock(Remote.Repository.Configuration.SyncRoot)
				{
					Remote.FetchUrl = fetchUrl;
					Remote.PushUrl = pushUrl;
					Remote.Proxy = proxy;
					Remote.VCS = vcs;
					Remote.FetchRefspec = fetch;
					Remote.PushRefspec = push;
					Remote.ReceivePack = receivePack;
					Remote.UploadPack = uploadPack;
					Remote.Mirror = mirror;
					Remote.SkipFetchAll = skipFetchAll;
					Remote.TagFetchMode = tagFetchMode;
				}
				Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToSetRemoteProperties,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
			return true;
		}

		#endregion
	}
}
