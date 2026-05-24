#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2026  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 */
#endregion

namespace gitter.Git.Gui;

using System;
using System.ClientModel;
using System.Threading;
using System.Threading.Tasks;

using OpenAI;
using OpenAI.Chat;

public sealed class OpenAICommitMessageGenerator : ICommitMessageGenerator
{
	private readonly CommitMessagePromptBuilder _promptBuilder = new();

	public async Task<string> GenerateAsync(Repository repository, CommitMessageGenerationOptions options, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNotNull(options);

		options = options.Normalize();
		if(string.IsNullOrWhiteSpace(options.ApiKey))
		{
			throw new InvalidOperationException("OpenAI API key is not configured.");
		}

		using var source = repository.Status.GetDiffSource(cached: true);
		var diffOptions = new DiffOptions { Context = 1, Binary = false };
		var diff = await source.GetDiffAsync(diffOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
		if(diff.IsEmpty)
		{
			throw new InvalidOperationException("There are no staged changes to summarize.");
		}

		var prompt = _promptBuilder.Build(options, diff);
		var content = await RequestAsync(options, prompt, cancellationToken).ConfigureAwait(false);
		content = Cleanup(content, options.SingleLine);
		if(string.IsNullOrWhiteSpace(content))
		{
			throw new InvalidOperationException("The model returned an empty commit message.");
		}
		return content;
	}

	private static async Task<string> RequestAsync(CommitMessageGenerationOptions options, string prompt, CancellationToken cancellationToken)
	{
		var client = new ChatClient(
			model: options.Model,
			credential: new ApiKeyCredential(options.ApiKey),
			options: new OpenAIClientOptions
			{
				Endpoint = new Uri(options.BaseAddress.TrimEnd('/') + "/"),
			});
		var completionOptions = new ChatCompletionOptions
		{
			MaxOutputTokenCount = options.MaxTokens,
			Temperature = 0.2f,
		};
		var completion = await client.CompleteChatAsync(
			new ChatMessage[]
			{
				new SystemChatMessage("You write accurate Git commit messages from staged diff summaries."),
				new UserChatMessage(prompt),
			},
			completionOptions,
			cancellationToken).ConfigureAwait(false);
		if(completion.Value.Content.Count == 0)
		{
			return string.Empty;
		}
		return completion.Value.Content[0].Text ?? string.Empty;
	}

	private static string Cleanup(string value, bool singleLine)
	{
		value = value.Trim().Trim('`').Trim();
		if(singleLine)
		{
			var lines = value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
			return lines.Length == 0 ? string.Empty : lines[0].Trim();
		}
		return value.Replace("\r\n", Environment.NewLine).Replace("\n", Environment.NewLine);
	}
}
