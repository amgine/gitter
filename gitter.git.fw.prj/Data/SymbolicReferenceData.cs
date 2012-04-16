namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	public sealed class SymbolicReferenceData
	{
		private readonly string _targetObject;
		private readonly ReferenceType _targetType;

		public SymbolicReferenceData(string targetObject, ReferenceType targetType)
		{
			_targetObject = targetObject;
			_targetType = targetType;
		}

		public string TargetObject
		{
			get { return _targetObject; }
		}

		public ReferenceType TargetType
		{
			get { return _targetType; }
		}
	}
}
