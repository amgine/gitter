namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	public sealed class NoteData : INamedObject
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
	}
}
