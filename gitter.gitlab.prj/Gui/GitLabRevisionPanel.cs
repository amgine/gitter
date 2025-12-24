#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2020  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab.Gui;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Services;
using gitter.Git;
using gitter.GitLab.Api;

using Resources = gitter.GitLab.Properties.Resources;

class GitLabRevisionPanel : FlowPanel
{
	static readonly LoggingService Log = new(@"GitLab");

	enum HitTestArea
	{
		Empty,
		PipelineId,
		TestCases,
		ViewOnGitLab,
	}

	readonly record struct HitTetResult(int Index, HitTestArea Area)
	{
		public static HitTetResult Empty        = new(-1, HitTestArea.Empty);
		public static HitTetResult ViewOnGitLab = new(-1, HitTestArea.ViewOnGitLab);
	}

	readonly record struct RenderedArea(HitTetResult HitTestResult, Rectangle Bounds);

	readonly record struct PipelineEntry(Pipeline Pipeline, TestReportSummary? Tests);

	private readonly List<PipelineEntry> _entries = [];
	private readonly List<RenderedArea> _rendered = [];
	private bool _isUpdating;
	private bool _failed;
	private HitTetResult _hovered = HitTetResult.Empty;
	private int _height = 0;
	private Dpi _heightDpi;

	public GitLabRevisionPanel(GitLabServiceContext serviceContext, Revision revision)
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
			var pipelines = await ServiceContext
				.GetPipelinesAsync(sha: Revision.HashString, cancellationToken: cts.Token);
			if(FlowControl is null || cts.IsCancellationRequested) return;
			var linesBefore = _entries.Count;
			if(linesBefore == 0) linesBefore = 1;
			_entries.Clear();
			if(pipelines is not null)
			{
				for(int i = 0, count = pipelines.Count; i < count; ++i)
				{
					var pipeline = pipelines[i];
					if(pipeline is null) continue;
					_entries.Add(new(pipeline, null));
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
			if(pipelines is not null)
			{
				for(int i = 0, count = pipelines.Count; i < count; ++i)
				{
					var pipeline = pipelines[i];
					if(pipeline is null) continue;
					var tests = default(TestReportSummary);
					try
					{
						tests = await ServiceContext
							.GetTestReportSummaryAsync(pipeline.Id, cancellationToken: cts.Token);
					}
					catch
					{
					}
					if(FlowControl is null || cts.IsCancellationRequested) return;
					if(tests is not null)
					{
						var index = _entries.FindIndex(e => e.Pipeline == pipeline);
						if(index >= 0)
						{
							_entries[index] = new(pipeline, tests);
							Invalidate();
						}
					}
				}
			}
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

	private GitLabServiceContext ServiceContext { get; }

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
						case HitTestArea.PipelineId:
							if(hit.Index >= 0 && hit.Index < _entries.Count)
							{
								Utility.OpenUrl(_entries[hit.Index].Pipeline.WebUrl.ToString());
							}
							break;
						case HitTestArea.TestCases:
							if(hit.Index >= 0 && hit.Index < _entries.Count)
							{
								if(_entries[hit.Index].Tests is { Total.Count: > 0 })
								{
									GitterApplication
										.WorkingEnvironment
										.ViewDockService
										.ShowView(Guids.TestReportViewGuid, new TestReportViewModel(_entries[hit.Index].Pipeline.Id));
								}
							}
							break;
						case HitTestArea.ViewOnGitLab:
							Utility.OpenUrl(ServiceContext.FormatCommitUrl(Revision.HashString));
							break;
					}
				}
				break;
			case MouseButtons.Right:
				{
					var hit = HitTest(x, y);
					switch(hit.Area)
					{
						case HitTestArea.PipelineId:
							if(hit.Index >= 0 && hit.Index < _entries.Count)
							{
								var menu = new HyperlinkContextMenu(_entries[hit.Index].Pipeline.WebUrl.ToString());
								Utility.MarkDropDownForAutoDispose(menu);
								menu.Show(Cursor.Position);
							}
							break;
						case HitTestArea.ViewOnGitLab:
							if(hit.Index >= 0 && hit.Index < _entries.Count)
							{
								var menu = new HyperlinkContextMenu(ServiceContext.FormatCommitUrl(Revision.HashString));
								Utility.MarkDropDownForAutoDispose(menu);
								menu.Show(Cursor.Position);
							}
							break;
					}
				}
				break;
		}
		base.OnMouseDown(x, y, button);
	}

	static readonly string HeaderText = Resources.StrGitLab.AddColon();

	private Rectangle RenderTests(ref PaintContext context, TestReportSummary? tests)
	{
		if(tests?.Total is not { } total) return Rectangle.Empty;
		if(context.Bounds.Width <= 0) return Rectangle.Empty;

		var icon = total.Count > 0 ? CommonIcons.Test : CommonIcons.TestEmpty;
		var bounds = context.RenderCount(icon, total.Count);
		if(total.Count > 0)
		{
			context.RenderCount(Icons.TestSuccess, total.Success);
			if(total.Skipped > 0)
			{
				var b = context.RenderCount(Icons.TestSkipped, total.Skipped);
				bounds = Rectangle.Union(bounds, b);
			}
			if(total.Failed > 0)
			{
				var b = context.RenderCount(Icons.TestFailed, total.Failed);
				bounds = Rectangle.Union(bounds, b);
			}
			if(total.Error > 0)
			{
				var b = context.RenderCount(Icons.TestError, total.Error);
				bounds = Rectangle.Union(bounds, b);
			}
		}

		return bounds;
	}

	private void RenderPipeline(ref PaintContext context, int i)
	{
		var pipeline = _entries[i].Pipeline;

		var normalText = context.Style.Colors.WindowText;

		context.RenderTextAndAdvance(Resources.StrlPipeline, normalText);
		context.MeasureTextAndAdvance(" ");

		var id = "#" + pipeline.Id;

		var pipelineIdBounds = context.RenderLinkTextAndAdvance(id, isHovered: _hovered.Index == i && _hovered.Area == HitTestArea.PipelineId);
		pipelineIdBounds.X -= context.InitialBounds.X;
		pipelineIdBounds.Y -= context.InitialBounds.Y;

		_rendered.Add(new(new(i, HitTestArea.PipelineId), pipelineIdBounds));

		context.Advance(context.DpiConverter.ConvertX(4));

		if(!string.IsNullOrWhiteSpace(pipeline.Ref))
		{
			if(context.RenderIconAndAdvance(CommonIcons.Branch).Width != 0)
			{
				context.Advance(context.DpiConverter.ConvertX(2));
			}
			context.RenderTextAndAdvance(pipeline.Ref, normalText);
			context.MeasureTextAndAdvance(" ");
		}

		GuiUtils.GetPipelineStatusIconAndName(pipeline.Status, out var imageProvider, out var status);

		if(imageProvider?.GetImage(context.DpiConverter.ConvertX(16)) is { } image)
		{
			if(context.RenderIconAndAdvance(imageProvider).Width != 0)
			{
				context.Advance(context.DpiConverter.ConvertX(2));
			}
		}

		if(status is not null)
		{
			context.RenderTextAndAdvance(status, normalText);
			context.Advance(context.DpiConverter.ConvertX(4));
		}

		var testCasesBounds = RenderTests(ref context, _entries[i].Tests);
		if(testCasesBounds.Width > 0 && testCasesBounds.Height > 0)
		{
			testCasesBounds.X -= context.InitialBounds.X;
			testCasesBounds.Y -= context.InitialBounds.Y;
			_rendered.Add(new(new(i, HitTestArea.TestCases), testCasesBounds));
		}
	}

	private void RenderViewOnGitLabLink(ref PaintContext context)
	{
		var bounds = context.RenderRightAlignedLink(
			Resources.StrsViewCommitOnGitLab,
			isHovered: _hovered.Area == HitTestArea.ViewOnGitLab);
		if(bounds.Width > 0 && bounds.Height > 0)
		{
			_rendered.Add(new(HitTetResult.ViewOnGitLab, bounds));
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
				? Resources.StrlFetchingPipelineInformation
				: _failed
				? Resources.StrlFailedToFetchPipelineInformation
				: Resources.StrlNoPipelinesFound;

			context.RenderTextAndAdvance(text, context.Style.Colors.GrayText);

			RenderViewOnGitLabLink(ref context);
		}
		else
		{
			for(int i = 0; i < _entries.Count; ++i)
			{
				if(i != 0) context.NextLine();

				RenderPipeline(ref context, i);

				if(i == 0)
				{
					RenderViewOnGitLabLink(ref context);
				}
			}
		}

		context.Dispose();
	}
}
