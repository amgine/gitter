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

namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using gitter.Framework;

	internal sealed class GitProgress
	{
		#region Data

		private string _stage;
		private int _min;
		private int _max;
		private int _current;
		private bool _continious;

		#endregion

		#region .ctor

		public GitProgress(string stage)
		{
			_continious = true;
			_stage = stage;
		}

		public GitProgress(string stage, int min, int max, int current)
		{
			_continious = false;
			_stage = stage;
			_min = min;
			_max = max;
			_current = current;
		}

		#endregion

		#region Properties

		public string Stage
		{
			get { return _stage; }
		}

		public bool IsContinious
		{
			get { return _continious; }
		}

		public int Minimum
		{
			get { return _min; }
		}

		public int Maximum
		{
			get { return _max; }
		}

		public int Current
		{
			get { return _current; }
		}

		#endregion

		#region Methods

		public void Notify(IAsyncProgressMonitor monitor)
		{
			Verify.Argument.IsNotNull(monitor, "monitor");

			if(_continious)
			{
				monitor.SetProgressIndeterminate();
			}
			else
			{
				monitor.SetProgressRange(_min, _max);
				monitor.SetProgress(_current, _stage);
			}
		}

		#endregion
	}
}
