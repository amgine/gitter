namespace gitter
{
	using System;

	using gitter.Framework;

	using Resources = gitter.Properties.Resources;

	internal static class Program
	{
		/// <summary>The main entry point for the application.</summary>
		[STAThread]
		public static void Main()
		{
			GitterApplication.Run<MainForm>();
		}
	}
}
