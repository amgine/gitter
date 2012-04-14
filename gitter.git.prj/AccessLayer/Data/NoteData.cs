namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	public sealed class NoteData : INamedObject, IObjectData<Note>
	{
		private readonly string _name;
		private readonly string _objectName;
		private string _message;

		public NoteData(string name, string objectName, string message)
		{
			_name = name;
			_objectName = objectName;
			_message = message;
		}

		public string Name
		{
			get { return _name; }
		}

		public string ObjectName
		{
			get { return _objectName; }
		}

		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}

		#region IObjectInformation<Note> Members

		public void Update(Note obj)
		{
			obj.Object = _objectName;
			if(_message != null)
				obj.Message = _message;
		}

		public Note Construct(IRepository repository)
		{
			return new Note((Repository)repository, _name, _objectName, _message);
		}

		#endregion
	}
}
