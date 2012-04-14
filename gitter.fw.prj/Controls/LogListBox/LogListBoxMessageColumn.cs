namespace gitter.Framework.Controls
{
	using Resources = gitter.Framework.Properties.Resources;

	sealed class LogListBoxMessageColumn : CustomListBoxColumn
	{
		/// <summary>Initializes a new instance of the <see cref="LogListBoxMessageColumn"/> class.</summary>
		public LogListBoxMessageColumn()
			: base((int)LogListBoxColumnId.Message, Resources.StrMessage)
		{
			SizeMode = ColumnSizeMode.Fill;
		}

		/// <summary>Gets the identification string.</summary>
		/// <value>The identification string.</value>
		public override string IdentificationString
		{
			get { return "Message"; }
		}
	}
}
