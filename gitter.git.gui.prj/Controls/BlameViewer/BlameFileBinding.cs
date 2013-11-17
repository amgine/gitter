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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;
	using gitter.Framework.Controls;

	sealed class BlameFileBinding : AsyncDataBinding<BlameFile>
	{
		#region Data

		private readonly IBlameSource _blameSource;
		private readonly BlameViewer _blameViewer;
		private readonly FlowProgressPanel _progressPanel;
		private BlameOptions _blameOptions;

		#endregion

		#region .ctor

		public BlameFileBinding(IBlameSource blameSource, BlameViewer blameViewer, BlameOptions blameOptions)
		{
			Verify.Argument.IsNotNull(blameSource, "blameSource");
			Verify.Argument.IsNotNull(blameViewer, "blameViewer");
			Verify.Argument.IsNotNull(blameOptions, "blameOptions");

			_blameSource = blameSource;
			_blameViewer = blameViewer;
			_blameOptions = blameOptions;
			_progressPanel = new FlowProgressPanel();

			Progress = _progressPanel.ProgressMonitor;
		}

		#endregion

		#region Properties

		public IBlameSource BlameSource
		{
			get { return _blameSource; }
		}

		public BlameViewer BlameViewer
		{
			get { return _blameViewer; }
		}

		public BlameOptions BlameOptions
		{
			get { return _blameOptions; }
			set
			{
				Verify.Argument.IsNotNull(value, "value");

				_blameOptions = value;
			}
		}

		#endregion

		#region Methods

		protected override Task<BlameFile> FetchDataAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.State.IsFalse(IsDisposed, "BlameFileBinding is disposed.");

			BlameViewer.Panels.Clear();
			BlameViewer.Panels.Add(_progressPanel);
			return BlameSource.GetBlameAsync(BlameOptions, progress, cancellationToken);
		}

		protected override void OnFetchCompleted(BlameFile blameFile)
		{
			Assert.IsNotNull(blameFile);

			if(IsDisposed || BlameViewer.IsDisposed)
			{
				return;
			}

			BlameViewer.Panels.Clear();
			BlameViewer.Panels.Add(new BlameFilePanel(BlameSource.Repository, blameFile));
		}

		protected override void OnFetchFailed(Exception exception)
		{
			if(IsDisposed || BlameViewer.IsDisposed)
			{
				return;
			}

			BlameViewer.Panels.Clear();
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(!BlameViewer.IsDisposed)
				{
					BlameViewer.Panels.Clear();
				}
			}
			base.Dispose(disposing);
		}

		#endregion
	}
}
