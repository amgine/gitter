#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2026  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 */
#endregion

namespace gitter.Git.Gui;

using System.Threading;
using System.Threading.Tasks;

public interface ICommitMessageGenerator
{
	Task<string> GenerateAsync(Repository repository, CommitMessageGenerationOptions options, CancellationToken cancellationToken = default);
}
