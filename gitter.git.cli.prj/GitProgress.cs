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
