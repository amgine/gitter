namespace gitter.Framework.Controls
{
	using Resources = gitter.Framework.Properties.Resources;

	sealed class LogListBoxTimestampColumn : CustomListBoxColumn
	{
		/// <summary>Initializes a new instance of the <see cref="LogListBoxTimestampColumn"/> class.</summary>
		public LogListBoxTimestampColumn()
			: base((int)LogListBoxColumnId.Timestamp, Resources.StrTimestamp)
		{
			Width = 155;
		}

		/// <summary>Gets the identification string.</summary>
		/// <value>The identification string.</value>
		public override string IdentificationString
		{
			get { return "Timestamp"; }
		}
	}
}
