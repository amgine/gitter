#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2026  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 */
#endregion

namespace gitter.Git.Gui;

using System;
using System.Text;

public sealed class CommitMessagePromptBuilder
{
	private const int MaxFiles = 40;
	private const int MaxLinesPerFile = 120;
	private const int MaxPromptChars = 24000;
	private const int HugeAddedFileLineCount = 600;
	private const int MaxLineChars = 500;

	public string Build(CommitMessageGenerationOptions options, Diff diff)
	{
		Verify.Argument.IsNotNull(options);
		Verify.Argument.IsNotNull(diff);

		var sb = new StringBuilder();
		AppendLine(sb, options.PromptText);
		AppendLine(sb, string.Empty);
		AppendLine(sb, $"Language: {options.Language}");
		AppendLine(sb, options.SingleLine
			? "Format: single-line commit subject only."
			: "Format: commit subject line followed by an optional body with bullet points when useful.");
		AppendLine(sb, string.Empty);
		AppendLine(sb, "Staged diff summary:");

		if(diff.IsEmpty)
		{
			AppendLine(sb, "No staged changes were found.");
			return sb.ToString();
		}

		var fileCount = 0;
		foreach(var file in diff)
		{
			if(fileCount >= MaxFiles)
			{
				AppendLine(sb, $"... {diff.FilesCount - MaxFiles} more file(s) omitted.");
				break;
			}
			++fileCount;

			var name = file.Status == FileStatus.Removed ? file.SourceFile : file.TargetFile;
			AppendLine(sb, string.Empty);
			AppendLine(sb, $"File: {name}");
			AppendLine(sb, $"Status: {file.Status}; +{file.Stats.AddedLinesCount}/-{file.Stats.RemovedLinesCount}; binary: {file.IsBinary}; lines: {file.LineCount}");
			if(file.Status == FileStatus.Renamed || file.Status == FileStatus.Copied)
			{
				AppendLine(sb, $"From: {file.SourceFile}");
				AppendLine(sb, $"To: {file.TargetFile}");
			}

			if(file.IsBinary)
			{
				AppendLine(sb, "Content omitted: binary file.");
				continue;
			}

			if(file.Status == FileStatus.Added && file.LineCount > HugeAddedFileLineCount)
			{
				AppendLine(sb, "Content omitted: large newly added file summarized by metadata only.");
				continue;
			}

			AppendDiffLines(sb, file);
			if(sb.Length >= MaxPromptChars)
			{
				AppendLine(sb, "Prompt truncated because staged changes are large.");
				break;
			}
		}

		return sb.ToString();
	}

	private static void AppendDiffLines(StringBuilder sb, DiffFile file)
	{
		var lines = 0;
		foreach(var hunk in file)
		{
			foreach(var line in hunk)
			{
				if(lines >= MaxLinesPerFile)
				{
					AppendLine(sb, $"... {file.LineCount - MaxLinesPerFile} more diff line(s) omitted for this file.");
					return;
				}
				AppendLine(sb, Truncate(line.ToString(), MaxLineChars));
				++lines;
			}
		}
	}

	private static string Truncate(string value, int maxLength)
		=> value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";

	private static void AppendLine(StringBuilder sb, string value)
	{
		if(sb.Length >= MaxPromptChars) return;
		var remaining = MaxPromptChars - sb.Length;
		if(value.Length + 2 > remaining)
		{
			value = value.Substring(0, Math.Max(0, remaining - 2));
		}
		sb.AppendLine(value);
	}
}
