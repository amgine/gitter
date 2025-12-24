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

namespace gitter.Framework.Controls;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Layout;

using Resources = gitter.Framework.Properties.Resources;

/// <summary>Extender for <see cref="DateColumn"/>.</summary>
[ToolboxItem(false)]
[DesignerCategory("")]
public partial class DateColumnExtender : ExtenderBase
{
	private readonly DateColumn _column;
	private bool _disableEvents;

	readonly struct DialogControls
	{
		public  readonly IRadioButtonWidget _radUnixTimestamp;
		public  readonly IRadioButtonWidget _radRelative;
		public  readonly IRadioButtonWidget _radSystemDefault;
		public  readonly IRadioButtonWidget _radISO8601;
		public  readonly IRadioButtonWidget _radRFC2822;
		private readonly LabelControl _lblUnixTimestamp;
		private readonly LabelControl _lblRelative;
		private readonly LabelControl _lblSystemDefault;
		private readonly LabelControl _lblISO8601;
		private readonly LabelControl _lblRFC2822;
		private readonly LabelControl _lblDateFormat;
		private readonly LabelControl _lblExample;
		public  readonly ICheckBoxWidget _chkConvertToLocal;
		public  readonly ICheckBoxWidget _chkShowUTCOffset;

		public DialogControls(IGitterStyle style)
		{
			style ??= GitterApplication.Style;

			var rbf = style.RadioButtonFactory;
			var cbf = style.CheckBoxFactory;

			_radUnixTimestamp  = rbf.Create();
			_radRelative       = rbf.Create();
			_radSystemDefault  = rbf.Create();
			_radISO8601        = rbf.Create();
			_radRFC2822        = rbf.Create();
			_lblUnixTimestamp  = new();
			_lblRelative       = new();
			_lblSystemDefault  = new();
			_lblISO8601        = new();
			_lblRFC2822        = new();
			_lblDateFormat     = new();
			_lblExample        = new();
			_chkConvertToLocal = cbf.Create();
			_chkShowUTCOffset  = cbf.Create();
		}

		public void Localize()
		{
			_lblDateFormat.Text		= Resources.StrDateFormat.AddColon();
			_lblExample.Text		= Resources.StrExample.AddColon();
			
			_radUnixTimestamp.Text	= Resources.StrUNIXTimestamp;
			_radRelative.Text		= Resources.StrRelative;
			_radSystemDefault.Text	= Resources.StrDefaultFormat;
			_radRFC2822.Text		= Resources.StrRFC2822;
			_radISO8601.Text		= Resources.StrISO8601;

			var date = DateTimeOffset.Now;
			_lblUnixTimestamp.Text	= Utility.FormatDate(date, DateFormat.UnixTimestamp);
			_lblRelative.Text		= Utility.FormatDate(date, DateFormat.Relative);
			_lblSystemDefault.Text	= Utility.FormatDate(date, DateFormat.SystemDefault);
			_lblRFC2822.Text		= Utility.FormatDate(date, DateFormat.RFC2822);
			_lblISO8601.Text		= Utility.FormatDate(date, DateFormat.ISO8601);

			_chkConvertToLocal.Text = Resources.StrConvertDateTimeToLocal;
			_chkShowUTCOffset.Text  = Resources.StrShowUTCOffset;

		}

		public void Layout(Control parent)
		{
			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					padding: DpiBoundValue.Padding(new(5)),
					columns:
					[
						SizeSpec.Absolute(140),
						SizeSpec.Everything(),
					],
					rows:
					[
						/* 0 */ LayoutConstants.LabelRowHeight,
						/* 1 */ LayoutConstants.LabelRowSpacing,
						/* 2 */ LayoutConstants.RadioButtonRowHeight,
						/* 3 */ LayoutConstants.RadioButtonRowHeight,
						/* 4 */ LayoutConstants.RadioButtonRowHeight,
						/* 5 */ LayoutConstants.RadioButtonRowHeight,
						/* 6 */ LayoutConstants.RadioButtonRowHeight,
						/* 7 */ LayoutConstants.RowSpacing,
						/* 8 */ LayoutConstants.CheckBoxRowHeight,
						/* 9 */ LayoutConstants.CheckBoxRowHeight,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblDateFormat,     marginOverride: LayoutConstants.NoMargin), row: 0, column: 0),
						new GridContent(new ControlContent(_lblExample,        marginOverride: LayoutConstants.NoMargin), row: 0, column: 1),
						new GridContent(new WidgetContent (_radUnixTimestamp,  marginOverride: LayoutConstants.NoMargin), row: 2, column: 0),
						new GridContent(new ControlContent(_lblUnixTimestamp,  marginOverride: LayoutConstants.NoMargin), row: 2, column: 1),
						new GridContent(new WidgetContent (_radRelative,       marginOverride: LayoutConstants.NoMargin), row: 3, column: 0),
						new GridContent(new ControlContent(_lblRelative,       marginOverride: LayoutConstants.NoMargin), row: 3, column: 1),
						new GridContent(new WidgetContent (_radSystemDefault,  marginOverride: LayoutConstants.NoMargin), row: 4, column: 0),
						new GridContent(new ControlContent(_lblSystemDefault,  marginOverride: LayoutConstants.NoMargin), row: 4, column: 1),
						new GridContent(new WidgetContent (_radISO8601,        marginOverride: LayoutConstants.NoMargin), row: 5, column: 0),
						new GridContent(new ControlContent(_lblISO8601,        marginOverride: LayoutConstants.NoMargin), row: 5, column: 1),
						new GridContent(new WidgetContent (_radRFC2822,        marginOverride: LayoutConstants.NoMargin), row: 6, column: 0),
						new GridContent(new ControlContent(_lblRFC2822,        marginOverride: LayoutConstants.NoMargin), row: 6, column: 1),
						new GridContent(new WidgetContent (_chkConvertToLocal, marginOverride: LayoutConstants.NoMargin), row: 8, columnSpan: 2),
						new GridContent(new WidgetContent (_chkShowUTCOffset,  marginOverride: LayoutConstants.NoMargin), row: 9, columnSpan: 2),
					]),
			};

			var tabIndex = 0;
			_lblDateFormat.TabIndex = tabIndex++;
			_lblExample.TabIndex = tabIndex++;
			_radUnixTimestamp.TabIndex = tabIndex++;
			_lblUnixTimestamp.TabIndex = tabIndex++;
			_radRelative.TabIndex = tabIndex++;
			_lblRelative.TabIndex = tabIndex++;
			_radSystemDefault.TabIndex = tabIndex++;
			_lblSystemDefault.TabIndex = tabIndex++;
			_radISO8601.TabIndex = tabIndex++;
			_lblISO8601.TabIndex = tabIndex++;
			_radRFC2822.TabIndex = tabIndex++;
			_lblRFC2822.TabIndex = tabIndex++;
			_chkConvertToLocal.TabIndex = tabIndex++;
			_chkShowUTCOffset.TabIndex = tabIndex++;

			_lblDateFormat.Parent = parent;
			_lblExample.Parent = parent;
			_radUnixTimestamp.Parent = parent;
			_lblUnixTimestamp.Parent = parent;
			_radRelative.Parent = parent;
			_lblRelative.Parent = parent;
			_radSystemDefault.Parent = parent;
			_lblSystemDefault.Parent = parent;
			_radISO8601.Parent = parent;
			_lblISO8601.Parent = parent;
			_radRFC2822.Parent = parent;
			_lblRFC2822.Parent = parent;
			_chkConvertToLocal.Parent = parent;
			_chkShowUTCOffset.Parent = parent;
		}
	}

	private readonly DialogControls _controls;

	/// <summary>Create <see cref="DateColumnExtender"/>.</summary>
	/// <param name="column">Host <see cref="DateColumn"/>.</param>
	public DateColumnExtender(DateColumn column) : base(column)
	{
		Verify.Argument.IsNotNull(column);

		_column = column;

		Name = nameof(DateColumnExtender);

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		DateFormat     = column.DateFormat;
		ConvertToLocal = column.ConvertToLocal;
		ShowUTCOffset  = column.ShowUTCOffset;

		_controls._radUnixTimestamp.IsCheckedChanged  += OnCheckedChanged;
		_controls._radRelative.IsCheckedChanged       += OnCheckedChanged;
		_controls._radSystemDefault.IsCheckedChanged  += OnCheckedChanged;
		_controls._radISO8601.IsCheckedChanged        += OnCheckedChanged;
		_controls._radRFC2822.IsCheckedChanged        += OnCheckedChanged;
		_controls._chkConvertToLocal.IsCheckedChanged += _chkConvertToLocal_CheckedChanged;
		_controls._chkShowUTCOffset.IsCheckedChanged  += _chkShowUTCOffset_CheckedChanged;

		SubscribeToColumnEvents();
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			UnsubscribeFromColumnEvents();
		}
		base.Dispose(disposing);
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(336, 172));

	private void SubscribeToColumnEvents()
	{
		_column.DateFormatChanged     += OnColumnDateFormatChanged;
		_column.ConvertToLocalChanged += OnConvertToLocalChanged;
		_column.ShowUTCOffsetChanged  += OnShowUTCOffsetChanged;
	}

	private void UnsubscribeFromColumnEvents()
	{
		_column.DateFormatChanged     -= OnColumnDateFormatChanged;
		_column.ConvertToLocalChanged -= OnConvertToLocalChanged;
		_column.ShowUTCOffsetChanged  -= OnShowUTCOffsetChanged;
	}

	private void OnColumnDateFormatChanged(object? sender, EventArgs e)
		=> DateFormat = _column.DateFormat;

	private void OnConvertToLocalChanged(object? sender, EventArgs e)
		=> ConvertToLocal = _column.ConvertToLocal;

	private void OnShowUTCOffsetChanged(object? sender, EventArgs e)
		=> ShowUTCOffset = _column.ShowUTCOffset;

	public DateFormat DateFormat
	{
		get
		{
			if(_controls._radUnixTimestamp.IsChecked) return DateFormat.UnixTimestamp;
			if(_controls._radRelative.IsChecked)      return DateFormat.Relative;
			if(_controls._radSystemDefault.IsChecked) return DateFormat.SystemDefault;
			if(_controls._radRFC2822.IsChecked)       return DateFormat.RFC2822;
			if(_controls._radISO8601.IsChecked)       return DateFormat.ISO8601;
			return DateFormat.SystemDefault;
		}
		set
		{
			var radio = value switch
			{
				DateFormat.UnixTimestamp => _controls._radUnixTimestamp,
				DateFormat.Relative      => _controls._radRelative,
				DateFormat.SystemDefault => _controls._radSystemDefault,
				DateFormat.RFC2822       => _controls._radRFC2822,
				DateFormat.ISO8601       => _controls._radISO8601,
				_ => default,
			};
			if(radio is not null)
			{
				_disableEvents = true;
				radio.IsChecked  = true;
				_disableEvents = false;
			}
		}
	}

	public bool ConvertToLocal
	{
		get => _controls._chkConvertToLocal.IsChecked;
		set
		{
			_disableEvents = true;
			try
			{
				_controls._chkConvertToLocal.IsChecked = value;
			}
			finally
			{
				_disableEvents = false;
			}
		}
	}

	public bool ShowUTCOffset
	{
		get => _controls._chkShowUTCOffset.IsChecked;
		set
		{
			_disableEvents = true;
			try
			{
				_controls._chkShowUTCOffset.IsChecked = value;
			}
			finally
			{
				_disableEvents = false;
			}
		}
	}

	private void OnCheckedChanged(object? sender, EventArgs e)
	{
		if(_disableEvents) return;
		if(sender is not IRadioButtonWidget { IsChecked: true }) return;
		_column.DateFormat = DateFormat;
	}

	private void _chkConvertToLocal_CheckedChanged(object? sender, EventArgs e)
	{
		if(!_disableEvents)
		{
			_column.ConvertToLocal = ConvertToLocal;
		}
	}

	private void _chkShowUTCOffset_CheckedChanged(object? sender, EventArgs e)
	{
		if(!_disableEvents)
		{
			_column.ShowUTCOffset = ShowUTCOffset;
		}
	}
}
