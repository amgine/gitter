namespace gitter.TeamCity
{
	using System;
	using System.Globalization;
	using System.Text;

	public sealed class BuildLocator : ObjectLocator
	{
		public BuildLocator()
		{
		}

		public string Id
		{
			get;
			set;
		}

		public BuildTypeLocator BuildType
		{
			get;
			set;
		}

		public string Number
		{
			get;
			set;
		}

		public UserLocator User
		{
			get;
			set;
		}

		public string AgentName
		{
			get;
			set;
		}

		public BuildStatus BuildStatus
		{
			get;
			set;
		}

		public FlagSelector Personal
		{
			get;
			set;
		}

		public FlagSelector Pinned
		{
			get;
			set;
		}

		public FlagSelector Running
		{
			get;
			set;
		}

		public FlagSelector Canceled
		{
			get;
			set;
		}

		public int Start
		{
			get;
			set;
		}

		public int Count
		{
			get;
			set;
		}

		public BuildLocator SinceBuild
		{
			get;
			set;
		}

		public override string ToString()
		{
			if(!string.IsNullOrWhiteSpace(Id))
			{
				return "id:" + Id;
			}
			var sb = new StringBuilder();
			AppendArgument(sb, "buildType", BuildType);
			AppendArgument(sb, "number", Number);
			AppendArgument(sb, "status", BuildStatus);
			AppendArgument(sb, "user", User);
			AppendArgument(sb, "personal", Personal);
			AppendArgument(sb, "canceled", Canceled);
			AppendArgument(sb, "running", Running);
			AppendArgument(sb, "pinned", Pinned);
			AppendArgument(sb, "agentName", AgentName);
			AppendArgument(sb, "count", Count);
			AppendArgument(sb, "start", Start);
			AppendArgument(sb, "sinceBuild", SinceBuild);
			return sb.ToString();
		}
	}
}
