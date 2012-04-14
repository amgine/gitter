namespace gitter.Framework.Controls
{
	using Resources = gitter.Framework.Properties.Resources;

	sealed class LogListBoxTypeColumn : CustomListBoxColumn
	{
		/// <summary>Initializes a new instance of the <see cref="LogListBoxTypeColumn"/> class.</summary>
		public LogListBoxTypeColumn()
			: base((int)LogListBoxColumnId.Type, Resources.StrType)
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
			get { return "Type"; }
		}
	}
}
