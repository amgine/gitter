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
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Git;
using gitter.GitLab.Api;

using Resources = gitter.GitLab.Properties.Resources;

class GitLabRevisionPanel : FlowPanel
{
	const int PipelineId   = 1;
	const int TestCases    = 2;
	const int ViewOnGitLab = 3;

	private static readonly StringFormat F1 = new() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };

	private static readonly StringFormat F2 = new() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

	private Pipeline _pipeline;
	private TestReportSummary _testReportSummary;
	private bool _isUpdating;
	private bool _failed;
	//private IReadOnlyList<Job> _jobs;
	private Rectangle _pipelineIdBounds;
	private Rectangle _testCasesBounds;
	private Rectangle _viewOnGitLabBounds;

	private int _hoveredItem;
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
	protected override void OnFlowControlAttached()
	{
		base.OnFlowControlAttached();
		Init();
	}

	/// <inheritdoc/>
	protected override void OnFlowControlDetached()
	{
		base.OnFlowControlDetached();
	}

	private async void Init()
	{
		_isUpdating = true;
		try
		{
			var pipelines = await ServiceContext.GetPipelinesAsync(sha: Revision.HashString);
			if(FlowControl is null) return;
			_pipeline = pipelines is { Count: > 0 } ? pipelines[0] : default;
			Invalidate();
			//if(_pipeline != null)
			//{
			//	_jobs = await _api.GetJobsAsync(ProjectId, _pipeline.Id);
			//}
			if(_pipeline is not null)
			{
				try
				{
					_testReportSummary = await ServiceContext.GetTestReportSummaryAsync(_pipeline.Id);
				}
				catch
				{
				}
				if(FlowControl is null) return;
				Invalidate();
			}
		}
		catch
		{
			_failed = true;
		}
		finally
		{
			_isUpdating = false;
			Invalidate();
		}
	}

	private GitLabServiceContext ServiceContext { get; }

	private Revision Revision { get; }

	protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
	{
		if(_heightDpi != measureEventArgs.Dpi)
		{
			var font   = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(measureEventArgs.Dpi);
			_height    = TextRenderer.MeasureText(measureEventArgs.Graphics, "W", font).Height;
			_heightDpi = measureEventArgs.Dpi;
		}
		return new Size(measureEventArgs.Width, _height);
	}

	private int GetHoverItemId(int x, int y)
	{
		if(_pipelineIdBounds.Contains(x, y))   return PipelineId;
		if(_testCasesBounds.Contains(x, y))    return TestCases;
		if(_viewOnGitLabBounds.Contains(x, y)) return ViewOnGitLab;
		return 0;
	}

	private Rectangle GetBounds(int itemId)
		=> itemId switch
		{
			PipelineId   => _pipelineIdBounds,
			TestCases    => _testCasesBounds,
			ViewOnGitLab => _viewOnGitLabBounds,
			_ => Rectangle.Empty,
		};

	protected override void OnMouseMove(int x, int y)
	{
		var hovered = GetHoverItemId(x, y);
		if(_hoveredItem != hovered)
		{
			if(_hoveredItem != 0)
			{
				Invalidate(GetBounds(_hoveredItem));
			}
			if(hovered != 0)
			{
				Invalidate(GetBounds(hovered));
			}
			_hoveredItem = hovered;
			FlowControl.Cursor = hovered != 0
				? Cursors.Hand
				: Cursors.Default;
		}
		base.OnMouseMove(x, y);
	}

	protected override void OnMouseLeave()
	{
		if(_hoveredItem != 0)
		{
			if(_hoveredItem != 0)
			{
				Invalidate(GetBounds(_hoveredItem));
			}
			_hoveredItem = 0;
			FlowControl.Cursor = Cursors.Default;
		}
		base.OnMouseLeave();
	}

	protected override void OnMouseDown(int x, int y, MouseButtons button)
	{
		switch(button)
		{
			case MouseButtons.Left:
				switch(GetHoverItemId(x, y))
				{
					case PipelineId:
						if(_pipeline is not null)
						{
							Utility.OpenUrl(_pipeline.WebUrl.ToString());
						}
						break;
					case TestCases when _pipeline is not null && _testReportSummary is { Total.Count: > 0 }:
						GitterApplication
							.WorkingEnvironment
							.ViewDockService
							.ShowView(Guids.TestReportViewGuid, new TestReportViewModel(_pipeline.Id));
						break;
					case ViewOnGitLab:
						Utility.OpenUrl(ServiceContext.FormatCommitUrl(Revision.HashString));
						break;
				}
				break;
			case MouseButtons.Right:
				switch(GetHoverItemId(x, y))
				{
					case PipelineId:
						if(_pipeline is not null)
						{
							var menu = new HyperlinkContextMenu(_pipeline.WebUrl.ToString());
							Utility.MarkDropDownForAutoDispose(menu);
							menu.Show(Cursor.Position);
						}
						break;
					case ViewOnGitLab:
						{
							var menu = new HyperlinkContextMenu(ServiceContext.FormatCommitUrl(Revision.HashString));
							Utility.MarkDropDownForAutoDispose(menu);
							menu.Show(Cursor.Position);
						}
						break;
				}
				break;
		}
		base.OnMouseDown(x, y, button);
	}

	static readonly string HeaderText = Resources.StrGitLab.AddColon();

	private int RenderTests(Graphics graphics, Dpi dpi, Color normalText, Color grayText, Rectangle bounds)
	{
		if(_testReportSummary is null) return bounds.X;

		var conv = DpiConverter.FromDefaultTo(dpi);
		var iconSize = conv.Convert(new Size(16, 16));

		void RenderCount(IImageProvider icon, int count, ref Rectangle bounds)
		{
			var iconBounds = bounds;
			iconBounds.Size = iconSize;
			iconBounds.Y   += (bounds.Height - iconBounds.Height) / 2;

			var w = conv.ConvertX(2) + iconBounds.Width;
			if(icon?.GetImage(iconBounds.Width) is { } image)
			{
				graphics.DrawImage(image, iconBounds);
				bounds.X     += w;
				bounds.Width -= w;
			}

			var font = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);

#if NET5_0_OR_GREATER
			Span<char> chars = stackalloc char[11];
			if(count.TryFormat(chars, out int charsWritten, provider: CultureInfo.InvariantCulture))
			{
				chars = chars.Slice(0, charsWritten);
				w = GitterApplication.TextRenderer.MeasureText(
					graphics, chars, font, bounds.Size, F2).Width;
				GitterApplication.TextRenderer.DrawText(
					graphics, chars, font, count != 0 ? normalText : grayText, bounds, F2);
			}
			else
			{
#endif
				var text = count.ToString(CultureInfo.InvariantCulture);
				w = GitterApplication.TextRenderer.MeasureText(
					graphics, text, font, bounds.Size, F2).Width;
				GitterApplication.TextRenderer.DrawText(
					graphics, text, font, count != 0 ? normalText : grayText, bounds, F2);
#if NET5_0_OR_GREATER
			}
#endif

			w += conv.ConvertX(4);
			bounds.X     += w;
			bounds.Width -= w;
		}

		RenderCount(_testReportSummary.Total.Count != 0 ? CommonIcons.Test : CommonIcons.TestEmpty, _testReportSummary.Total.Count, ref bounds);
		if(_testReportSummary.Total.Count > 0)
		{
			RenderCount(Icons.TestSuccess, _testReportSummary.Total.Success, ref bounds);
			if(_testReportSummary.Total.Skipped > 0)
			{
				RenderCount(Icons.TestSkipped, _testReportSummary.Total.Skipped, ref bounds);
			}
			if(_testReportSummary.Total.Failed > 0)
			{
				RenderCount(Icons.TestFailed, _testReportSummary.Total.Failed, ref bounds);
			}
			if(_testReportSummary.Total.Error > 0)
			{
				RenderCount(Icons.TestError, _testReportSummary.Total.Error, ref bounds);
			}
		}

		return bounds.X;
	}

	protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
	{
		Rectangle textBounds;
		int w;

		var graphics = paintEventArgs.Graphics;
		var conv = new DpiConverter(FlowControl);
		var font = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(paintEventArgs.Dpi);

		var grayText   = Style.Colors.GrayText;
		var normalText = Style.Colors.WindowText;
		var linkText   = Style.Colors.HyperlinkText;

		textBounds = paintEventArgs.Bounds;
		textBounds.Width = conv.ConvertX(66);
		GitterApplication.TextRenderer.DrawText(
			graphics, HeaderText, font, grayText, textBounds, F1);

		textBounds = paintEventArgs.Bounds;
		var dx = conv.ConvertX(70);
		textBounds.Width -= dx;
		textBounds.X     += dx;

		int x;

		if(_pipeline is null)
		{
			_pipelineIdBounds = default;
			_testCasesBounds  = default;

			var text = _isUpdating
				? Resources.StrlFetchingPipelineInformation
				: _failed
				? Resources.StrlFailedToFetchPipelineInformation
				: Resources.StrlNoPipelinesFound;

			GitterApplication.TextRenderer.DrawText(
				graphics, text, font, grayText, textBounds, F2);

			x = textBounds.X + GitterApplication.TextRenderer.MeasureText(graphics, text, font, textBounds.Size, F2).Width;
		}
		else
		{
			var text = Resources.StrlPipeline;
			GitterApplication.TextRenderer.DrawText(
				graphics, text, font, normalText, textBounds, F2);

			w = GitterApplication.TextRenderer.MeasureText(
				graphics, text + " ", font, textBounds.Size, F2).Width;

			textBounds.X     += w;
			textBounds.Width -= w;
			var id = "#" + _pipeline.Id;

			_pipelineIdBounds = textBounds;
			_pipelineIdBounds.X -= paintEventArgs.Bounds.X;
			_pipelineIdBounds.Y -= paintEventArgs.Bounds.Y;

			if(_hoveredItem == PipelineId)
			{
				using var u = new Font(font, FontStyle.Underline);
				GitterApplication.TextRenderer.DrawText(
					graphics, id, u, linkText, textBounds, F2);
			}
			else
			{
				GitterApplication.TextRenderer.DrawText(
					graphics, id, font, linkText, textBounds, F2);
			}

			w = GitterApplication.TextRenderer.MeasureText(
				graphics, id, font, textBounds.Size, F2).Width;

			_pipelineIdBounds.Width = w;
			w += conv.ConvertX(4);

			textBounds.X     += w;
			textBounds.Width -= w;

			GitterApplication.TextRenderer.DrawText(
				graphics, "status", font, normalText, textBounds, F2);

			w = GitterApplication.TextRenderer.MeasureText(
				graphics, "status ", font, textBounds.Size, F2).Width;

			textBounds.X += w;
			textBounds.Width -= w;

			var iconBounds = textBounds;
			iconBounds.Size = conv.Convert(new Size(16, 16));
			iconBounds.Y   += (textBounds.Height - iconBounds.Height) / 2;

			GuiUtils.GetPipelineStatusIconAndName(_pipeline.Status, out var imageProvider, out var status);

			if(imageProvider is not null)
			{
				var image = imageProvider.GetImage(conv.ConvertX(16));
				graphics.DrawImage(image, iconBounds);
			}

			textBounds.X += iconBounds.Width + conv.ConvertX(2);

			if(status is not null)
			{
				GitterApplication.TextRenderer.DrawText(
					graphics, status, font, normalText, textBounds, F2);

				w = GitterApplication.TextRenderer.MeasureText(
					graphics, status, font, textBounds.Size, F2).Width;

				w += conv.ConvertX(4);

				textBounds.X     += w;
				textBounds.Width -= w;
			}

			x = RenderTests(graphics, paintEventArgs.Dpi, normalText, grayText, textBounds);
			_testCasesBounds = textBounds;
			_testCasesBounds.Width = x - textBounds.X;
			_testCasesBounds.X -= paintEventArgs.Bounds.X;
			_testCasesBounds.Y -= paintEventArgs.Bounds.Y;
		}

		textBounds = paintEventArgs.Bounds;
		textBounds.Width -= conv.ConvertX(5);

		var viewOnGitlabText = Resources.StrsViewCommitOnGitLab;
		w = GitterApplication.TextRenderer.MeasureText(
			graphics, viewOnGitlabText, font, textBounds.Size, F1).Width;

		if(w > paintEventArgs.Bounds.Width - x)
		{
			_viewOnGitLabBounds = Rectangle.Empty;
			return;
		}

		_viewOnGitLabBounds = textBounds;
		_viewOnGitLabBounds.X -= paintEventArgs.Bounds.X;
		_viewOnGitLabBounds.Y -= paintEventArgs.Bounds.Y;

		if(_hoveredItem == ViewOnGitLab)
		{
			using var u = new Font(font, FontStyle.Underline);
			GitterApplication.TextRenderer.DrawText(
				graphics, viewOnGitlabText, u, linkText, textBounds, F1);
		}
		else
		{
			GitterApplication.TextRenderer.DrawText(
				graphics, viewOnGitlabText, font, linkText, textBounds, F1);
		}

		_viewOnGitLabBounds.X += _viewOnGitLabBounds.Width - w;
		_viewOnGitLabBounds.Width = w;
	}
}
