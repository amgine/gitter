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
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;

	using Resources = gitter.Git.Gui.Properties.Resources;

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
