namespace gitter.Framework.Options
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	[ToolboxItem(false)]
	public partial class BehaviorPage : PropertyPage, IExecutableDialog, IElevatedExecutableDialog
	{
		public static readonly new Guid Guid = new Guid("74FF3B33-DD69-4752-83A1-4AB4912D93A5");

		private bool _isIntegartedInFolderMenu;

		/// <summary>Create <see cref="BehaviorPage"/>.</summary>
		public BehaviorPage()
			: base(Guid)
		{
			InitializeComponent();

			_isIntegartedInFolderMenu = GlobalOptions.IsIntegratedInExplorerContextMenu;
			_chkIntergateInFolderMenu.Checked = _isIntegartedInFolderMenu;
		}

		protected override void OnShown()
		{
			base.OnShown();
			_chkIntergateInFolderMenu.CheckedChanged += OnIntergateInFolderMenuCheckedChanged;
		}

		private void OnIntergateInFolderMenuCheckedChanged(object sender, EventArgs e)
		{
			RequireElevationChanged.Raise(this);
		}

		public bool Execute()
		{
			if(_isIntegartedInFolderMenu != _chkIntergateInFolderMenu.Checked)
			{
				_isIntegartedInFolderMenu = GlobalOptions.IsIntegratedInExplorerContextMenu;
				_chkIntergateInFolderMenu.CheckedChanged -= OnIntergateInFolderMenuCheckedChanged;
				_chkIntergateInFolderMenu.Checked = _isIntegartedInFolderMenu;
				_chkIntergateInFolderMenu.CheckedChanged += OnIntergateInFolderMenuCheckedChanged;
				RequireElevationChanged.Raise(this);
			}
			return true;
		}

		#region IElevatedExecutableDialog Members

		public event EventHandler RequireElevationChanged;

		public bool RequireElevation
		{
			get { return _isIntegartedInFolderMenu != _chkIntergateInFolderMenu.Checked; }
		}

		public Action ElevatedExecutionAction
		{
			get
			{
				if(RequireElevation)
				{
					return _chkIntergateInFolderMenu.Checked ?
						(Action)GlobalOptions.IntegrateInExplorerContextMenu:
						(Action)GlobalOptions.RemoveFromExplorerContextMenu;
				}
				else
				{
					return null;
				}
			}
		}

		#endregion
	}
}
