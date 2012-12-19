namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	static class RefsDiff
	{
		public static ReferenceChange[] Calculate(RefsState state1, RefsState state2)
		{
			var changeset = new List<ReferenceChange>();
			foreach(var refState2 in state2.States)
			{
				var refState1 = state1.GetState(refState2.FullName, refState2.ReferenceType);
				if(refState1 == null)
				{
					changeset.Add(
						new ReferenceChange(
							refState2.ReferenceType,
							refState2.FullName,
							refState2.Name,
							null,
							refState2.Hash,
							ReferenceChangeType.Added));
				}
				else
				{
					if(refState2.Hash != refState1.Hash)
					{
						changeset.Add(
							new ReferenceChange(
								refState1.ReferenceType,
								refState1.FullName,
								refState1.Name,
								refState1.Hash,
								refState2.Hash,
								ReferenceChangeType.Moved));
					}
				}
			}
			foreach(var refState1 in state1.States)
			{
				var refState2 = state2.GetState(refState1.FullName, refState1.ReferenceType);
				if(refState2 == null)
				{
					changeset.Add(
						new ReferenceChange(
							refState1.ReferenceType,
							refState1.FullName,
							refState1.Name,
							refState1.Hash,
							null,
							ReferenceChangeType.Removed));
				}
			}
			return changeset.ToArray();
		}
	}
}
