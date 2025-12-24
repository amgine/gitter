#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.TeamCity.Gui;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Services;
using gitter.Git;

using Resources = gitter.TeamCity.Properties.Resources;

class TeamCityRevisionPanel : FlowPanel
{
	static readonly LoggingService Log = new(@"TeamCity");

	enum HitTestArea
	{
		Empty,
		BuildNumber,
		TestCases,
		ViewOnGitLab,
	}

	readonly record struct HitTetResult(int Index, HitTestArea Area)
	{
		public static HitTetResult Empty        = new(-1, HitTestArea.Empty);
		public static HitTetResult ViewOnGitLab = new(-1, HitTestArea.ViewOnGitLab);
	}

	readonly record struct RenderedArea(HitTetResult HitTestResult, Rectangle Bounds);

	readonly record struct BuildEntry(Build Build);

	private readonly List<BuildEntry> _entries = [];
	private readonly List<RenderedArea> _rendered = [];
	private bool _isUpdating;
	private bool _failed;
	private HitTetResult _hovered = HitTetResult.Empty;
	private int _height = 0;
	private Dpi _heightDpi;

	public TeamCityRevisionPanel(TeamCityServiceContext serviceContext, Revision revision)
	{
		Verify.Argument.IsNotNull(serviceContext);
		Verify.Argument.IsNotNull(revision);

		ServiceContext = serviceContext;
		Revision       = revision;
	}

	/// <inheritdoc/>
	protected override void OnFlowControlAttached(FlowLayoutControl flowControl)
	{
		base.OnFlowControlAttached(flowControl);
		ReloadPipelineInfo();
	}

	/// <inheritdoc/>
	protected override void OnFlowControlDetached(FlowLayoutControl flowControl)
	{
		Restart(null);
		base.OnFlowControlDetached(flowControl);
	}

	private CancellationTokenSource? _currentOperation;

	private void Restart(CancellationTokenSource? cts)
	{
		var current = Interlocked.Exchange(ref _currentOperation, cts);
		try
		{
			current?.Cancel();
		}
		catch
		{
		}
	}

	private bool TryComplete(CancellationTokenSource? cts)
		=> Interlocked.CompareExchange(ref _currentOperation, null, cts) == cts;

	private async void ReloadPipelineInfo()
	{
		_isUpdating = true;
		using var cts = new CancellationTokenSource();
		try
		{
			Restart(cts);
			var builds = await ServiceContext.Builds
				.QueryAsync(new() { Project = new() { Id = ServiceContext.DefaultProjectId }, Revision = Revision.HashString, Branch = "<any>", Running = FlagSelector.Any, }, cts.Token);
			if(FlowControl is null || cts.IsCancellationRequested) return;
			var linesBefore = _entries.Count;
			if(linesBefore == 0) linesBefore = 1;
			_entries.Clear();
			if(builds is not null)
			{
				for(int i = 0, count = builds.Length; i < count; ++i)
				{
					var build = builds[i];
					if(build is null) continue;
					_entries.Add(new(build));
				}
			}
			var linesAfter = _entries.Count;
			if(linesAfter == 0) linesAfter = 1;
			if(linesBefore != linesAfter)
			{
				_heightDpi = default;
				InvalidateSize();
			}
			Invalidate();
		}
		catch(Exception exc)
		{
			if(!cts.IsCancellationRequested)
			{
				Log.Error(exc, exc.Message);
				_failed = true;
			}
		}
		finally
		{
			_isUpdating = false;
			if(TryComplete(cts))
			{
				Invalidate();
			}
		}
	}

	private TeamCityServiceContext ServiceContext { get; }

	private Revision Revision { get; }

	protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
	{
		if(_heightDpi != measureEventArgs.Dpi)
		{
			var font   = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(measureEventArgs.Dpi);
			var lines = _entries.Count;
			if(lines < 1) lines = 1;
			_height    = TextRenderer.MeasureText(measureEventArgs.Graphics, "W", font).Height * lines;
			_heightDpi = measureEventArgs.Dpi;
		}
		return new Size(measureEventArgs.Width, _height);
	}

	private HitTetResult HitTest(int x, int y)
	{
		foreach(var item in _rendered)
		{
			if(item.Bounds.Contains(x, y))
			{
				return item.HitTestResult;
			}
		}
		return HitTetResult.Empty;
	}

	private Rectangle GetBounds(HitTetResult hit)
	{
		foreach(var item in _rendered)
		{
			if(item.HitTestResult == hit) return item.Bounds;
		}
		return Rectangle.Empty;
	}

	protected override void OnMouseMove(int x, int y)
	{
		var hovered = HitTest(x, y);
		if(_hovered != hovered)
		{
			if(_hovered.Area != HitTestArea.Empty)
			{
				Invalidate(GetBounds(_hovered));
			}
			if(hovered.Area != HitTestArea.Empty)
			{
				Invalidate(GetBounds(hovered));
			}
			_hovered = hovered;
			if(FlowControl is not null)
			{
				FlowControl.Cursor = hovered.Area != HitTestArea.Empty
					? Cursors.Hand
					: Cursors.Default;
			}
		}
		base.OnMouseMove(x, y);
	}

	protected override void OnMouseLeave()
	{
		if(_hovered.Area != HitTestArea.Empty)
		{
			Invalidate(GetBounds(_hovered));
			_hovered = HitTetResult.Empty;
			if(FlowControl is not null)
			{
				FlowControl.Cursor = Cursors.Default;
			}
		}
		base.OnMouseLeave();
	}

	protected override void OnMouseDown(int x, int y, MouseButtons button)
	{
		switch(button)
		{
			case MouseButtons.Left:
				{
					var hit = HitTest(x, y);
					switch(hit.Area)
					{
						case HitTestArea.BuildNumber:
							if(hit.Index >= 0 && hit.Index < _entries.Count)
							{
								var url = _entries[hit.Index].Build.WebUrl;
								if(url is not null) Utility.OpenUrl(url);
							}
							break;
					}
				}
				break;
			case MouseButtons.Right:
				{
					var hit = HitTest(x, y);
					switch(hit.Area)
					{
						case HitTestArea.BuildNumber:
							if(hit.Index >= 0 && hit.Index < _entries.Count)
							{
								var build = _entries[hit.Index].Build;
								var url   = build.WebUrl;
								if(url is not null)
								{
									var menu = new BuildContextMenu(build);
									Utility.MarkDropDownForAutoDispose(menu);
									menu.Show(Cursor.Position);
								}
							}
							break;
					}
				}
				break;
		}
		base.OnMouseDown(x, y, button);
	}

	static readonly string HeaderText = Resources.StrTeamCity.AddColon();

	private void RenderBuild(ref PaintContext context, int i)
	{
		var build = _entries[i].Build;

		var normalText = context.Style.Colors.WindowText;

		var name = build.BuildType?.Name;
		if(string.IsNullOrWhiteSpace(name))
		{
			context.RenderTextAndAdvance("build", normalText);
		}
		else
		{
			if(context.RenderIconAndAdvance(Icons.Builds).Width != 0)
			{
				context.Advance(context.DpiConverter.ConvertX(2));
			}
			context.RenderTextAndAdvance(name, normalText);
		}
		context.MeasureTextAndAdvance(" ");

		var buildNumber = context.RenderLinkTextAndAdvance("#" + build.Number, isHovered: _hovered.Index == i && _hovered.Area == HitTestArea.BuildNumber);
		buildNumber.X -= context.InitialBounds.X;
		buildNumber.Y -= context.InitialBounds.Y;

		_rendered.Add(new(new(i, HitTestArea.BuildNumber), buildNumber));

		context.Advance(context.DpiConverter.ConvertX(4));

		if(!string.IsNullOrWhiteSpace(build.BranchName))
		{
			if(context.RenderIconAndAdvance(CommonIcons.Branch).Width != 0)
			{
				context.Advance(context.DpiConverter.ConvertX(2));
			}
			context.RenderTextAndAdvance(build.BranchName, normalText);
			context.MeasureTextAndAdvance(" ");
		}

		var statusIcon = default(IImageProvider);
		var statusText = default(string);
		switch(build.State)
		{
			case BuildState.Running:
				statusText = "running...";
				break;
			case BuildState.Queued:
				statusText = "queued";
				break;
			case BuildState.Unknown:
				statusText = "unknown";
				break;
			case BuildState.Finished:
				(statusIcon, statusText) = build.Status switch
				{
					BuildStatus.Success => (Icons.StatusSuccess, "success"),
					BuildStatus.Failure => (Icons.StatusFailure, "failure"),
					BuildStatus.Error   => (Icons.StatusError,   "error"),
					_ => (default, default),
				};
				break;
		}

		if(statusIcon is not null || statusText is not null)
		{
			if(context.RenderIconAndAdvance(statusIcon).Width != 0)
			{
				context.Advance(context.DpiConverter.ConvertX(2));
			}
			context.RenderTextAndAdvance(statusText, normalText);
		}
	}

	protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
	{
		if(FlowControl is null) return;

		var context = new PaintContext(FlowControl, paintEventArgs);

		context.RenderHeader(HeaderText);

		_rendered.Clear();
		if(_entries.Count == 0)
		{
			var text = _isUpdating
				? "fetching builds..."
				: _failed
				? "failed to fetch builds"
				: "no associated builds found";

			context.RenderTextAndAdvance(text, context.Style.Colors.GrayText);
		}
		else
		{
			for(int i = 0; i < _entries.Count; ++i)
			{
				if(i != 0) context.NextLine();

				RenderBuild(ref context, i);
			}
		}

		context.Dispose();
	}
}
