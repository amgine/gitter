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

namespace gitter.GitLab.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Git;
	using gitter.GitLab.Api;

	using Resources = gitter.GitLab.Properties.Resources;

	class GitLabRevisionPanel : FlowPanel
	{
		const int PipelineId   = 1;
		const int ViewOnGitLab = 2;

		private static readonly StringFormat F1 = new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };

		private static readonly StringFormat F2 = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

		private Pipeline _pipeline;
		private bool _isUpdating;
		private bool _failed;
		//private IReadOnlyList<Job> _jobs;
		private Rectangle _pipelineIdBounds;
		private Rectangle _viewOnGitLabBounds;

		private int _hoveredItem;

		public GitLabRevisionPanel(GitLabServiceContext serviceContext, Revision revision)
		{
			Verify.Argument.IsNotNull(serviceContext, nameof(serviceContext));
			Verify.Argument.IsNotNull(revision, nameof(revision));

			ServiceContext = serviceContext;
			Revision       = revision;
			Init();
		}

		private async void Init()
		{
			_isUpdating = true;
			try
			{
				var pipelines = await ServiceContext.GetPipelinesAsync(sha: Revision.HashString);
				_pipeline = pipelines != null && pipelines.Count > 0 ? pipelines[0] : default;
				//if(_pipeline != null)
				//{
				//	_jobs = await _api.GetJobsAsync(ProjectId, _pipeline.Id);
				//}
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
			return new Size(measureEventArgs.Width, 18);
		}

		private int GetHoverItemId(int x, int y)
		{
			if(_pipelineIdBounds.Contains(x, y))   return PipelineId;
			if(_viewOnGitLabBounds.Contains(x, y)) return ViewOnGitLab;
			return 0;
		}

		private Rectangle GetBounds(int itemId)
			=> itemId switch
			{
				PipelineId   => _pipelineIdBounds,
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
							if(_pipeline != null)
							{
								Utility.OpenUrl(_pipeline.WebUrl.ToString());
							}
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
							if(_pipeline != null)
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

		protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
		{
			Rectangle textBounds;
			int w;

			var graphics = paintEventArgs.Graphics;
			var font = GitterApplication.FontManager.UIFont.Font;

			using var grayText   = new SolidBrush(Style.Colors.GrayText);
			using var normalText = new SolidBrush(Style.Colors.WindowText);
			using var linkText   = new SolidBrush(Style.Colors.HyperlinkText);

			textBounds = paintEventArgs.Bounds;
			textBounds.Width = 66;
			GitterApplication.TextRenderer.DrawText(
				graphics, HeaderText, font, grayText, textBounds, F1);

			textBounds = paintEventArgs.Bounds;
			textBounds.Width -= 70;
			textBounds.X += 70;

			if(_pipeline == null)
			{
				var text = _isUpdating
					? Resources.StrlFetchingPipelineInformation
					: _failed
					? Resources.StrlFailedToFetchPipelineInformation
					: Resources.StrlNoPipelinesFound;

				GitterApplication.TextRenderer.DrawText(
					graphics, text, font, grayText, textBounds, F2);
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
				w += 4;

				textBounds.X     += w;
				textBounds.Width -= w;

				GitterApplication.TextRenderer.DrawText(
					graphics, "status", font, normalText, textBounds, F2);

				w = GitterApplication.TextRenderer.MeasureText(
					graphics, "status ", font, textBounds.Size, F2).Width;

				textBounds.X     += w;
				textBounds.Width -= w;

				var iconBounds = textBounds;
				iconBounds.Size = SystemInformation.SmallIconSize;
				iconBounds.Y   += (textBounds.Height - iconBounds.Height) / 2;

				Bitmap image;
				string status;
				switch(_pipeline.Status)
				{
					case PipelineStatus.Success:
						image  = CachedResources.Bitmaps["ImgStatusSuccess"];
						status = "success";
						break;
					case PipelineStatus.Running:
						image  = CachedResources.Bitmaps["ImgStatusRunning"];
						status = "running...";
						break;
					case PipelineStatus.Pending:
						image  = CachedResources.Bitmaps["ImgStatusPending"];
						status = "pending...";
						break;
					case PipelineStatus.Failed:
						image  = CachedResources.Bitmaps["ImgStatusFailed"];
						status = "failed";
						break;
					case PipelineStatus.Canceled:
						image  = CachedResources.Bitmaps["ImgStatusCanceled"];
						status = "canceled";
						break;
					case PipelineStatus.Skipped:
						image  = CachedResources.Bitmaps["ImgStatusSkipped"];
						status = "skipped";
						break;
					case PipelineStatus.Preparing:
						image  = CachedResources.Bitmaps["ImgStatusPreparing"];
						status = "preparing";
						break;
					case PipelineStatus.Scheduled:
						image  = CachedResources.Bitmaps["ImgStatusScheduled"];
						status = "scheduled";
						break;
					case PipelineStatus.Created:
						image  = CachedResources.Bitmaps["ImgStatusCreated"];
						status = "created";
						break;
					case PipelineStatus.Manual:
						image  = CachedResources.Bitmaps["ImgStatusManual"];
						status = "manual";
						break;
					default:
						image = default;
						status = default;
						break;
				};

				if(image != null)
				{
					graphics.DrawImage(image, iconBounds);
				}

				textBounds.X += iconBounds.Width + 2;

				if(status != null)
				{
					GitterApplication.TextRenderer.DrawText(
						graphics, status, font, normalText, textBounds, F2);
				}
			}

			textBounds = paintEventArgs.Bounds;
			textBounds.Width -= 5;
			_viewOnGitLabBounds = textBounds;
			_viewOnGitLabBounds.X -= paintEventArgs.Bounds.X;
			_viewOnGitLabBounds.Y -= paintEventArgs.Bounds.Y;

			if(_hoveredItem == ViewOnGitLab)
			{
				using var u = new Font(font, FontStyle.Underline);
				GitterApplication.TextRenderer.DrawText(
					graphics, Resources.StrsViewCommitOnGitLab, u, linkText, textBounds, F1);
			}
			else
			{
				GitterApplication.TextRenderer.DrawText(
					graphics, Resources.StrsViewCommitOnGitLab, font, linkText, textBounds, F1);
			}

			w = GitterApplication.TextRenderer.MeasureText(
				graphics, Resources.StrsViewCommitOnGitLab, font, textBounds.Size, F1).Width;

			_viewOnGitLabBounds.X += _viewOnGitLabBounds.Width - w;
			_viewOnGitLabBounds.Width = w;
		}
	}
}
