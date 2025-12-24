namespace gitter.Framework;

public struct OperationProgress
{
	public static readonly OperationProgress Completed = new() { ActionName = "Completed.", IsCompleted = true };

	public static OperationProgress Indeterminate(string actionName)
		=> new() { ActionName = actionName, IsIndeterminate = true };

	public OperationProgress(string actionName)
	{
		ActionName      = actionName;
		IsIndeterminate = true;
		MaxProgress     = 0;
		CurrentProgress = 0;
		IsCompleted     = false;
	}

	public OperationProgress(string actionName, int maxProgress)
	{
		Verify.Argument.IsNotNegative(maxProgress);

		ActionName      = actionName;
		IsIndeterminate = false;
		MaxProgress     = maxProgress;
		CurrentProgress = 0;
		IsCompleted     = false;
	}

	public string ActionName { get; set; }

	public bool IsIndeterminate { get; set; }

	public int MaxProgress { get; set; }

	public int CurrentProgress { get; set; }

	public bool IsCompleted { get; set; }
}
