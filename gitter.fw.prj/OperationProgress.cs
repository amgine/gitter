namespace gitter.Framework
{
	public struct OperationProgress
	{
		#region Static

		public static readonly OperationProgress Completed = new OperationProgress() { ActionName = "Completed.", IsCompleted = true };

		#endregion

		#region Data

		private string _actionName;
		private bool _isIndeterminate;
		private int _maxProgress;
		private int _currentProgress;
		private bool _isCompleted;

		#endregion

		#region .ctor

		public OperationProgress(string actionName)
		{
			_actionName = actionName;
			_isIndeterminate = true;
			_maxProgress = 0;
			_currentProgress = 0;
			_isCompleted = false;
		}

		public OperationProgress(string actionName, int maxProgress)
		{
			Verify.Argument.IsNotNegative(maxProgress, "maxProgress");

			_actionName = actionName;
			_isIndeterminate = false;
			_maxProgress = maxProgress;
			_currentProgress = 0;
			_isCompleted = false;
		}

		#endregion

		#region Properties

		public string ActionName
		{
			get { return _actionName; }
			set { _actionName = value; }
		}

		public bool IsIndeterminate
		{
			get { return _isIndeterminate; }
			set { _isIndeterminate = value; }
		}

		public int MaxProgress
		{
			get { return _maxProgress; }
			set { _maxProgress = value; }
		}

		public int CurrentProgress
		{
			get { return _currentProgress; }
			set { _currentProgress = value; }
		}

		public bool IsCompleted
		{
			get { return _isCompleted; }
			set { _isCompleted = value; }
		}

		#endregion
	}
}
