namespace gitter.Framework.Controls
{
	using Resources = gitter.Framework.Properties.Resources;

	sealed class LogListBoxExceptionColumn : CustomListBoxColumn
	{
		/// <summary>Initializes a new instance of the <see cref="LogListBoxExceptionColumn"/> class.</summary>
		public LogListBoxExceptionColumn()
			: base((int)LogListBoxColumnId.Exception, Resources.StrException)
		{
			Width = 16 + ListBoxConstants.SpaceBeforeImage + ListBoxConstants.SpaceAfterImage;
			SizeMode = ColumnSizeMode.Fixed;
		}

		protected override void OnPaintContent(ItemPaintEventArgs paintEventArgs)
		{
		}

		/// <summary>Gets the identification string.</summary>
		/// <value>The identification string.</value>
		public override string IdentificationString
		{
			get { return "Exception"; }
		}
	}
}
