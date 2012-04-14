namespace gitter.Framework.Services
{
	using System.Windows.Forms;

	public sealed class MessageBoxResult
	{
		private readonly DialogResult _dialogResult;
		private readonly int _resultOption;

		public MessageBoxResult(DialogResult dialogResult, int resultOption)
		{
			_dialogResult = dialogResult;
			_resultOption = resultOption;
		}

		public int ResultOption
		{
			get { return _resultOption; }
		}

		public DialogResult DialogResult
		{
			get { return _dialogResult; }
		}
	}
}
