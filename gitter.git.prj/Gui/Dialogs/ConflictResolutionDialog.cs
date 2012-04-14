namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;

	using Resources = gitter.Git.Properties.Resources;

	internal partial class ConflictResolutionDialog : GitDialogBase
	{
		private readonly ConflictResolution _resolution1;
		private readonly ConflictResolution _resolution2;
		private ConflictResolution _resolution;

		private static string StatusToString(FileStatus status)
		{
			switch(status)
			{
				case FileStatus.Added:
					return Resources.StrlAdded;
				case FileStatus.Removed:
					return Resources.StrlDeleted;
				case FileStatus.Modified:
					return Resources.StrlModified;
				default:
					throw new ArgumentException("status");
			}
		}

		private static Color StatusToColor(FileStatus status)
		{
			switch(status)
			{
				case FileStatus.Added:
					return Color.Green;
				case FileStatus.Removed:
					return Color.Red;
				case FileStatus.Modified:
					return Color.Yellow;
				default:
					throw new ArgumentException("status");
			}
		}

		private static string ConflictResolutionToString(ConflictResolution conflictResolution)
		{
			switch(conflictResolution)
			{
				case ConflictResolution.KeepModifiedFile:
					return Resources.StrsKeepModifiedFile;
				case ConflictResolution.DeleteFile:
					return Resources.StrsDeleteFile;
				case ConflictResolution.UseOurs:
					return Resources.StrsUseOursVersion;
				case ConflictResolution.UseTheirs:
					return Resources.StrsUseTheirsVersion;
				default:
					throw new ArgumentException("conflictResolution");
			}
		}

		public ConflictResolutionDialog(string fileName, FileStatus oursStatus, FileStatus theirsStatus,
			ConflictResolution resolution1, ConflictResolution resolution2)
		{
			InitializeComponent();

			Text = Resources.StrConflictResolution;

			_lblFileName.Text = fileName;

			_lblOursStatus.Text = StatusToString(oursStatus);
			_lblTheirsStatus.Text = StatusToString(theirsStatus);

			_lblOursStatus.BackColor = StatusToColor(oursStatus);
			_lblTheirsStatus.BackColor = StatusToColor(theirsStatus);

			_resolution1 = resolution1;
			_resolution2 = resolution2;
		}

		public override DialogButtons OptimalButtons
		{
			get { return DialogButtons.Cancel; }
		}

		public ConflictResolution ConflictResolution
		{
			get { return _resolution; }
		}

		private void _btnResolution1_Click(object sender, EventArgs e)
		{
			_resolution = _resolution1;
			ClickOk();
		}

		private void _btnResolution2_Click(object sender, EventArgs e)
		{
			_resolution = _resolution2;
			ClickOk();
		}
	}
}
