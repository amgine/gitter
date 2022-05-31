#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

using gitter.Framework;
using gitter.GitLab.Api;

static class GuiUtils
{
	public static void GetPipelineStatusIconAndName(PipelineStatus status, out IImageProvider image, out string name)
	{
		switch(status)
		{
			case PipelineStatus.Success:
				image = Icons.PipelineSuccess;
				name  = "success";
				break;
			case PipelineStatus.Running:
				image = Icons.PipelineRunning;
				name  = "running...";
				break;
			case PipelineStatus.Pending:
				image = Icons.PipelinePending;
				name  = "pending...";
				break;
			case PipelineStatus.Failed:
				image = Icons.PipelineFailed;
				name  = "failed";
				break;
			case PipelineStatus.Canceled:
				image = Icons.PipelineCanceled;
				name  = "canceled";
				break;
			case PipelineStatus.Skipped:
				image = Icons.PipelineSkipped;
				name  = "skipped";
				break;
			case PipelineStatus.Preparing:
				image = Icons.PipelinePreparing;
				name  = "preparing";
				break;
			case PipelineStatus.Scheduled:
				image = Icons.PipelineScheduled;
				name  = "scheduled";
				break;
			case PipelineStatus.Created:
				image = Icons.PipelineCreated;
				name  = "created";
				break;
			case PipelineStatus.Manual:
				image = Icons.PipelineManual;
				name  = "manual";
				break;
			default:
				image = default;
				name  = default;
				break;
		}
	}
}
