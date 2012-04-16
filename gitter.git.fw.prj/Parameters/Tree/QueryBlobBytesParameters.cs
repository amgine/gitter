namespace gitter.Git.AccessLayer
{
	public sealed class QueryBlobBytesParameters
	{
		public string Treeish { get; set; }

		public string ObjectName { get; set; }
	}
}
