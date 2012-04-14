namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml;

	public abstract class RedmineObjectDefinition<T>
		where T : RedmineObject
	{
		private bool _isCommitted;
		
		protected RedmineObjectDefinition()
		{
		}

		protected static void EmitIfChanged<TValue>(TValue original, TValue current, XmlDocument doc, XmlElement root, string name, Action<XmlElement, TValue> emit)
		{
			if(!EqualityComparer<TValue>.Default.Equals(original, current))
			{
				var e = doc.CreateElement(name);
				emit(e, current);
			}
		}

		public bool IsCommitted
		{
			get { return _isCommitted; }
		}

		protected abstract void ResetCore();

		protected abstract void CommitCore();

		public void Reset()
		{
			if(IsCommitted) throw new InvalidOperationException();

			ResetCore();
		}

		public void Commit()
		{
			if(IsCommitted) throw new InvalidOperationException();

			CommitCore();
			_isCommitted = true;
		}
	}
}
