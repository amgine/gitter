namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.Drawing;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class SelectResetModeDialog : GitDialogBase
	{
		private readonly ResetMode _availableModes;
		private ResetMode _resetMode;
		private List<CommandLink> _buttons;

		public SelectResetModeDialog(ResetMode availableModes)
		{
			InitializeComponent();

			Text = Resources.StrReset;

			_availableModes = availableModes;
			_resetMode = ResetMode.Mixed;

			_buttons = new List<CommandLink>(5);

			if((_availableModes & ResetMode.Soft) == ResetMode.Soft)
			{
				_buttons.Add(CreateResetButton(ResetMode.Soft));
			}
			if((_availableModes & ResetMode.Mixed) == ResetMode.Mixed)
			{
				_buttons.Add(CreateResetButton(ResetMode.Mixed));
			}
			if((_availableModes & ResetMode.Hard) == ResetMode.Hard)
			{
				_buttons.Add(CreateResetButton(ResetMode.Hard));
			}
			if((_availableModes & ResetMode.Merge) == ResetMode.Merge)
			{
				_buttons.Add(CreateResetButton(ResetMode.Merge));
			}
			if((_availableModes & ResetMode.Keep) == ResetMode.Keep)
			{
				_buttons.Add(CreateResetButton(ResetMode.Keep));
			}

			const int margin = 16;

			var location = new Point(margin, margin);
			var h = margin;
			foreach(var btn in _buttons)
			{
				btn.Location = location;
				btn.Parent = this;
				h += btn.Height + margin;
				location.Y += btn.Height + margin;
			}

			Height = h;
		}

		public SelectResetModeDialog()
			: this(ResetMode.Soft | ResetMode.Mixed | ResetMode.Hard)
		{
		}

		public ResetMode AvailableModes
		{
			get { return _availableModes; }
		}

		public ResetMode ResetMode
		{
			get { return _resetMode; }
			set { _resetMode = value; }
		}

		protected override string ActionVerb
		{
			get { return Resources.StrReset; }
		}

		public override DialogButtons OptimalButtons
		{
			get { return DialogButtons.Cancel; }
		}

		private CommandLink CreateResetButton(ResetMode mode)
		{
			string text = string.Empty;
			string desc = string.Empty;

			switch(mode)
			{
				case ResetMode.Soft:
					text = Resources.StrSoft;
					desc = Resources.TipSoftReset;
					break;
				case ResetMode.Mixed:
					text = Resources.StrMixed;
					desc = Resources.TipMixedReset;
					break;
				case ResetMode.Hard:
					text = Resources.StrHard;
					desc = Resources.TipHardReset;
					break;
				case ResetMode.Merge:
					text = Resources.StrMerge;
					desc = Resources.TipMergeReset;
					break;
				case ResetMode.Keep:
					text = Resources.StrKeep;
					desc = Resources.TipKeepReset;
					break;
				default:
					throw new NotSupportedException();
			}

			var btn = new CommandLink()
			{
				Size		= new Size(319, 66),
				Text		= text,
				Description	= desc,
				Tag			= mode,
			};

			btn.Click += (s, e) =>
				{
					ResetMode = (ResetMode)((Control)s).Tag;
					ClickOk();
				};

			return btn;
		}

		protected override void OnShown()
		{
			base.OnShown();

			if(_buttons != null && _buttons.Count != 0)
			{
				foreach(var btn in _buttons)
				{
					if(((ResetMode)btn.Tag) == _resetMode)
					{
						btn.Focus();
						return;
					}
				}
				_buttons[0].Focus();
			}
		}
	}
}
