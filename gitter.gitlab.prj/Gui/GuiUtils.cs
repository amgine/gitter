namespace gitter.GitLab.Gui
{
	using System;
	using System.Drawing;

	using gitter.GitLab.Api;

	static class GuiUtils
	{
		public static void GetPipelineStatusIconAndName(PipelineStatus status, out Bitmap image, out string name)
		{
			switch(status)
			{
				case PipelineStatus.Success:
					image = CachedResources.Bitmaps["ImgStatusSuccess"];
					name  = "success";
					break;
				case PipelineStatus.Running:
					image = CachedResources.Bitmaps["ImgStatusRunning"];
					name  = "running...";
					break;
				case PipelineStatus.Pending:
					image = CachedResources.Bitmaps["ImgStatusPending"];
					name  = "pending...";
					break;
				case PipelineStatus.Failed:
					image = CachedResources.Bitmaps["ImgStatusFailed"];
					name  = "failed";
					break;
				case PipelineStatus.Canceled:
					image = CachedResources.Bitmaps["ImgStatusCanceled"];
					name  = "canceled";
					break;
				case PipelineStatus.Skipped:
					image = CachedResources.Bitmaps["ImgStatusSkipped"];
					name  = "skipped";
					break;
				case PipelineStatus.Preparing:
					image = CachedResources.Bitmaps["ImgStatusPreparing"];
					name  = "preparing";
					break;
				case PipelineStatus.Scheduled:
					image = CachedResources.Bitmaps["ImgStatusScheduled"];
					name  = "scheduled";
					break;
				case PipelineStatus.Created:
					image = CachedResources.Bitmaps["ImgStatusCreated"];
					name  = "created";
					break;
				case PipelineStatus.Manual:
					image = CachedResources.Bitmaps["ImgStatusManual"];
					name  = "manual";
					break;
				default:
					image = default;
					name  = default;
					break;
			}
		}
	}
}
