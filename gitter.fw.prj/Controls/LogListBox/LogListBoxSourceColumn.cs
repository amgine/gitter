namespace gitter.Framework.Controls
{
	using Resources = gitter.Framework.Properties.Resources;

	sealed class LogListBoxSourceColumn : CustomListBoxColumn
	{
		/// <summary>Initializes a new instance of the <see cref="LogListBoxSourceColumn"/> class.</summary>
		public LogListBoxSourceColumn()
			: base((int)LogListBoxColumnId.Source, Resources.StrSource)
		{
			Width = 50;
			SizeMode = ColumnSizeMode.Sizeable;
		}

		/// <summary>Gets the identification string.</summary>
		/// <value>The identification string.</value>
		public override string IdentificationString
		{
			get { return "Source"; }
		}
	}
}
