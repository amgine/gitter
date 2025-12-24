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

using System;

using gitter.Framework;

static class Icons
{
	private static IImageProvider CreateProvider(string name)
		=> new ScaledImageProvider(CachedResources.ScaledBitmaps, name);

	public static readonly IImageProvider GitLab      = CreateProvider(@"gitlab");
	public static readonly IImageProvider TestFailed  = CreateProvider(@"test.failed");
	public static readonly IImageProvider TestSuccess = CreateProvider(@"test.success");
	public static readonly IImageProvider TestSkipped = CreateProvider(@"test.skipped");
	public static readonly IImageProvider TestError   = CreateProvider(@"test.error");

	public static readonly IImageProvider PipelineCanceled  = CreateProvider(@"pipeline.status.canceled");
	public static readonly IImageProvider PipelineCreated   = CreateProvider(@"pipeline.status.created");
	public static readonly IImageProvider PipelineFailed    = CreateProvider(@"pipeline.status.failed");
	public static readonly IImageProvider PipelineManual    = CreateProvider(@"pipeline.status.manual");
	public static readonly IImageProvider PipelinePending   = CreateProvider(@"pipeline.status.pending");
	public static readonly IImageProvider PipelinePreparing = CreateProvider(@"pipeline.status.preparing");
	public static readonly IImageProvider PipelineRunning   = CreateProvider(@"pipeline.status.running");
	public static readonly IImageProvider PipelineScheduled = CreateProvider(@"pipeline.status.scheduled");
	public static readonly IImageProvider PipelineSkipped   = CreateProvider(@"pipeline.status.skipped");
	public static readonly IImageProvider PipelineSuccess   = CreateProvider(@"pipeline.status.success");
}
