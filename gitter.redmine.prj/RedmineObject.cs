namespace gitter.Redmine
{
	using System;
	using System.Xml;

	public abstract class RedmineObject
	{
		#region Static

		public static readonly RedmineObjectProperty IdProperty =
			new RedmineObjectProperty("id", "Id");

		#endregion

		#region Data

		private readonly RedmineServiceContext _context;
		private readonly int _id;

		#endregion

		#region Events

		public event EventHandler<RedmineObjectPropertyChangedEventArgs> PropertyChanged;

		protected void OnPropertyChanged(RedmineObjectProperty property)
		{
			var handler = PropertyChanged;
			if(handler != null)
			{
				handler(this, new RedmineObjectPropertyChangedEventArgs(property));
			}
		}

		#endregion

		#region .ctor

		protected RedmineObject(RedmineServiceContext context, int id)
		{
			if(context == null) throw new ArgumentNullException("context");

			_context = context;
			_id = id;
		}

		protected RedmineObject(RedmineServiceContext context, XmlNode node)
		{
			if(context == null) throw new ArgumentNullException("context");
			if(node == null) throw new ArgumentNullException("node");

			_context	= context;
			_id			= RedmineUtility.LoadInt(node[IdProperty.XmlNodeName]);
		}

		#endregion

		#region Methods

		internal virtual void Update(XmlNode node)
		{
			if(node == null) throw new ArgumentNullException("node");
		}

		public virtual void Update()
		{
			throw new NotSupportedException();
		}

		public object GetValue(RedmineObjectProperty property)
		{
			if(property == null) throw new ArgumentNullException("property");

			return GetType().GetProperty(property.Name).GetValue(this, null);
		}

		#endregion

		#region Properties

		public int Id
		{
			get { return _id; }
		}

		public RedmineServiceContext Context
		{
			get { return _context; }
		}

		#endregion
	}
}
