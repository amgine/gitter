namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Xml;

	public abstract class RedmineObject
	{
		#region Static

		public static readonly RedmineObjectProperty<int> IdProperty =
			new RedmineObjectProperty<int>("id", "Id");

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
			Verify.Argument.IsNotNull(context, "context");

			_context = context;
			_id = id;
		}

		protected RedmineObject(RedmineServiceContext context, XmlNode node)
		{
			Verify.Argument.IsNotNull(context, "context");
			Verify.Argument.IsNotNull(node, "node");

			_context	= context;
			_id			= RedmineUtility.LoadInt(node[IdProperty.XmlNodeName]);
		}

		#endregion

		#region Methods

		internal virtual void Update(XmlNode node)
		{
			Verify.Argument.IsNotNull(node, "node");
		}

		public virtual void Update()
		{
			throw new NotSupportedException();
		}

		public object GetValue(RedmineObjectProperty property)
		{
			Verify.Argument.IsNotNull(property, "property");

			return GetType().GetProperty(property.Name).GetValue(this, null);
		}

		protected void UpdatePropertyValue<T>(ref T field, T value, RedmineObjectProperty<T> property)
		{
			if(!EqualityComparer<T>.Default.Equals(field, value))
			{
				field = value;
				OnPropertyChanged(property);
			}
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
