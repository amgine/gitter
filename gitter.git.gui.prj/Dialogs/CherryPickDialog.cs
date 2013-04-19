namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Services;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class CherryPickDialog : GitDialogBase, IExecutableDialog
	{
		#region Data

		private readonly IRevisionPointer _revisionPointer;

		#endregion

		#region .ctor

		public CherryPickDialog(IRevisionPointer revisionPointer)
		{
			Verify.Argument.IsValidRevisionPointer(revisionPointer, "revisionPointer");

			InitializeComponent();

			_revisionPointer = revisionPointer;

			Text = Resources.StrCherryPickCommit;

			_lblRevision.Text = Resources.StrRevision.AddColon();
			_grpMainlineParentCommit.Text = Resources.StrMainlineParentCommit;
			_grpOptions.Text = Resources.StrOptions;
			_chkNoCommit.Text = Resources.StrNoCommit;
			ToolTipService.Register(_chkNoCommit, Resources.TipCherryPickNoCommit);

			_txtRevision.Text = revisionPointer.Pointer;

			GitterApplication.FontManager.InputFont.Apply(_txtRevision);

			var revision = revisionPointer.Dereference();
			if(!revision.IsLoaded)
			{
				revision.Load();
			}
			if(revision.Parents.Count > 1)
			{
				_lstCommits.Style = GitterApplication.DefaultStyle;
				bool first = true;
				foreach(var parent in revision.Parents)
				{
					if(!parent.IsLoaded)
					{
						parent.Load();
					}
					if(first)
					{
						first = false;
					}
					else
					{
						_lstCommits.Panels.Add(new FlowPanelSeparator() { Height = 10 });
					}
					_lstCommits.Panels.Add(
						new RevisionHeaderPanel()
						{
							Revision = parent,
							IsSelectable = true,
							IsSelected = _lstCommits.Panels.Count == 0,
						});
				}
			}
			else
			{
				_pnlMainlineParentCommit.Visible = false;
				Height -= _pnlMainlineParentCommit.Height + _pnlOptions.Margin.Top;
				Width = 385;
			}
		}

		#endregion

		#region Properties

		public IRevisionPointer RevisionPointer
		{
			get { return _revisionPointer; }
		}

		protected override string ActionVerb
		{
			get { return Resources.StrCherryPick; }
		}

		public bool NoCommit
		{
			get { return _chkNoCommit.Checked; }
			set { _chkNoCommit.Checked = value; }
		}

		public int MainlineParentCommit
		{
			get
			{
				int index = 0;
				foreach(var p in _lstCommits.Panels)
				{
					var rhp = p as RevisionHeaderPanel;
					if(rhp != null)
					{
						++index;
						if(rhp.IsSelected)
						{
							return index;
						}
					}
				}
				return 0;
			}
			set
			{
				Verify.Argument.IsNotNegative(value, "value");

				if(value == 0)
				{
					foreach(var p in _lstCommits.Panels)
					{
						var rhp = p as RevisionHeaderPanel;
						if(rhp != null)
						{
							rhp.IsSelected = false;
						}
					}
				}
				else
				{
					int index = 0;
					foreach(var p in _lstCommits.Panels)
					{
						var rhp = p as RevisionHeaderPanel;
						if(rhp != null)
						{
							++index;
							if(index == value)
							{
								rhp.IsSelected = true;
								break;
							}
						}
					}
				}
			}
		}

		#endregion

		#region IExecutableDialog

		public bool Execute()
		{
			int mainline	= MainlineParentCommit;
			bool noCommit	= NoCommit;
			Cursor = Cursors.WaitCursor;
			try
			{
				RevisionPointer.CherryPick(mainline, noCommit);
			}
			catch(HaveConflictsException)
			{
				Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					this,
					Resources.ErrCherryPickIsNotPossibleWithConflicts,
					Resources.StrCherryPick,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
			catch(HaveLocalChangesException)
			{
				Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					this,
					Resources.ErrCherryPickIsNotPossibleWithLocalChnges,
					Resources.StrCherryPick,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
			catch(AutomaticCherryPickFailedException)
			{
				BeginInvoke(new Action(
					() =>
					{
						using(var dlg = new ConflictsDialog(RevisionPointer.Repository))
						{
							dlg.Run(GitterApplication.MainForm);
						}
					}));
			}
			catch(GitException exc)
			{
				Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					string.Format(Resources.ErrFailedToCherryPick, RevisionPointer.Pointer),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
			Cursor = Cursors.Default;
			return true;
		}

		#endregion
	}
}
