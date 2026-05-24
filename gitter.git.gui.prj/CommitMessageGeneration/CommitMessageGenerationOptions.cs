#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2026  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
#endregion

namespace gitter.Git.Gui;

using gitter.Framework.Configuration;

public record class CommitMessageGenerationOptions(
	string ApiKey = "",
	string BaseAddress = "https://api.openai.com/v1",
	string Model = "gpt-4o-mini",
	string Language = "English",
	string PromptText = CommitMessageGenerationOptions.DefaultPromptText,
	int MaxTokens = 160,
	bool SingleLine = true)
{
	public const string SectionName = "CommitMessageGeneration";

	public const string DefaultPromptText = @"You are an expert in writing Git commit messages strictly following the Conventional Commits specification (https://www.conventionalcommits.org/).

You will be given a summary of code changes (e.g., a diff, a bullet list of changes, or a description). Based on that, generate a commit message that:

1. Uses one of the following types:
   - feat: a new feature for the user
   - fix: a bug fix
   - docs: documentation only changes
   - style: code style changes (formatting, missing semicolons, etc.) — no logic change
   - refactor: code change that neither fixes a bug nor adds a feature
   - perf: performance improvement
   - test: adding missing tests or correcting existing tests
   - build: changes that affect the build system or external dependencies
   - ci: changes to CI configuration files and scripts
   - chore: other changes that don't modify src or test files
   - revert: reverts a previous commit

2. Format the message as:
   <type>(<optional scope>): <short description in present tense>
   <optional blank line>
   <optional detailed body explaining what and why, not how>
   <optional blank line>
   <optional footer(s) for BREAKING CHANGE or issue references>";

	public static CommitMessageGenerationOptions Default { get; } = new();

	public static void SaveTo(CommitMessageGenerationOptions options, Section section)
	{
		Verify.Argument.IsNotNull(options);
		Verify.Argument.IsNotNull(section);

		section.SetValue("ApiKey",      options.ApiKey);
		section.SetValue("BaseAddress", options.BaseAddress);
		section.SetValue("Model",       options.Model);
		section.SetValue("Language",    options.Language);
		section.SetValue("PromptText",  options.PromptText);
		section.SetValue("MaxTokens",   options.MaxTokens);
		section.SetValue("SingleLine",  options.SingleLine);
	}

	public static CommitMessageGenerationOptions LoadFrom(Section section)
	{
		Verify.Argument.IsNotNull(section);

		return new CommitMessageGenerationOptions(
			ApiKey:      section.GetValue("ApiKey",      Default.ApiKey),
			BaseAddress: section.GetValue("BaseAddress", Default.BaseAddress),
			Model:       section.GetValue("Model",       Default.Model),
			Language:    section.GetValue("Language",    Default.Language),
			PromptText:  section.GetValue("PromptText",  Default.PromptText),
			MaxTokens:   section.GetValue("MaxTokens",   Default.MaxTokens),
			SingleLine:  section.GetValue("SingleLine",  Default.SingleLine));
	}

	public CommitMessageGenerationOptions Normalize()
		=> this with
		{
			BaseAddress = string.IsNullOrWhiteSpace(BaseAddress) ? Default.BaseAddress : BaseAddress.Trim(),
			Model       = string.IsNullOrWhiteSpace(Model)       ? Default.Model       : Model.Trim(),
			Language    = string.IsNullOrWhiteSpace(Language)    ? Default.Language    : Language.Trim(),
			PromptText  = string.IsNullOrWhiteSpace(PromptText)  ? Default.PromptText  : PromptText.Trim(),
			MaxTokens   = MaxTokens <= 0 ? Default.MaxTokens : MaxTokens,
		};
}
