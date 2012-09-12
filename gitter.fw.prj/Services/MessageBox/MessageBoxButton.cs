namespace gitter.Framework.Services
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	public sealed class MessageBoxButton
	{
		public static readonly MessageBoxButton Yes		= new MessageBoxButton(DialogResult.Yes,	0, Resources.StrYes,	true);
		public static readonly MessageBoxButton No		= new MessageBoxButton(DialogResult.No,		0, Resources.StrNo,		false);
		public static readonly MessageBoxButton Ok		= new MessageBoxButton(DialogResult.OK,		0, Resources.StrOk,		true);
		public static readonly MessageBoxButton Close	= new MessageBoxButton(DialogResult.OK,		0, Resources.StrClose,	true);
		public static readonly MessageBoxButton Cancel	= new MessageBoxButton(DialogResult.Cancel,	0, Resources.StrCancel,	false);
		public static readonly MessageBoxButton Abort	= new MessageBoxButton(DialogResult.Abort,	0, Resources.StrAbort,	false);
		public static readonly MessageBoxButton Retry	= new MessageBoxButton(DialogResult.Retry,	0, Resources.StrRetry,	true);
		public static readonly MessageBoxButton Ignore	= new MessageBoxButton(DialogResult.Ignore,	0, Resources.StrIgnore,	false);

		public static readonly IEnumerable<MessageBoxButton> RetryCancel		= new[] { Retry, Cancel };
		public static readonly IEnumerable<MessageBoxButton> AbortRetryIgnore	= new[] { Abort, Retry, Ignore };
		public static readonly IEnumerable<MessageBoxButton> YesNo				= new[] { Yes, No };
		public static readonly IEnumerable<MessageBoxButton> YesNoCancel		= new[] { Yes, No, Cancel };
		public static readonly IEnumerable<MessageBoxButton> OkCancel			= new[] { Ok, Cancel };

		public static MessageBoxButton GetOk(string label)
		{
			return GetOk(label, 0);
		}

		public static MessageBoxButton GetOk(string label, int resultOption)
		{
			return new MessageBoxButton(DialogResult.OK, resultOption, label, true);
		}

		public static MessageBoxButton GetYes(string label)
		{
			return GetYes(label, 0);
		}

		public static MessageBoxButton GetYes(string label, int resultOption)
		{
			return new MessageBoxButton(DialogResult.OK, resultOption, label, true);
		}

		public static IEnumerable<MessageBoxButton> GetButtons(MessageBoxButtons buttons)
		{
			switch(buttons)
			{
				case MessageBoxButtons.OK:
					return new MessageBoxButton[] { Ok };
				case MessageBoxButtons.OKCancel:
					return new MessageBoxButton[] { Ok, Cancel };
				case MessageBoxButtons.YesNo:
					return new MessageBoxButton[] { Yes, No };
				case MessageBoxButtons.YesNoCancel:
					return new MessageBoxButton[] { Yes, No, Cancel };
				case MessageBoxButtons.AbortRetryIgnore:
					return new MessageBoxButton[] { Abort, Retry, Ignore };
				case MessageBoxButtons.RetryCancel:
					return new MessageBoxButton[] { Retry, Cancel };
				default:
					throw new ArgumentException(
						"Unknown MessageBoxButtons value: {0}".UseAsFormat(buttons),
						"buttons");
			}
		}

		#region Data

		private readonly DialogResult _dialogResult;
		private readonly int _resultOption;
		private readonly string _displayLabel;
		private readonly bool _isDefault;

		#endregion

		public MessageBoxButton(DialogResult dialogResult, int resultOption, string displayLabel, bool isDefault)
		{
			_dialogResult = dialogResult;
			_resultOption = resultOption;
			_displayLabel = displayLabel;
			_isDefault = isDefault;
		}

		public string DisplayLabel
		{
			get { return _displayLabel; }
		}

		public int ResultOption
		{
			get { return _resultOption; }
		}

		public DialogResult DialogResult
		{
			get { return _dialogResult; }
		}

		public bool IsDefault
		{
			get { return _isDefault; }
		}
	}
}
