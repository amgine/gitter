#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2023  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Git.AccessLayer.CLI;

static class LogFormatPlaceholders
{
	private const string _prefix = "%";

	public const string NewLine = $@"{_prefix}n";
	public const string Percent = $@"{_prefix}%";

	static class Color
	{
		private const string _color = "C";

		public const string Red = $@"{_prefix}{_color}red";
		public const string Green = $@"{_prefix}{_color}green";
		public const string Blue = $@"{_prefix}{_color}blue";
		public const string Reset = $@"{_prefix}{_color}reset";
	}

	public const string Mark = $@"{_prefix}m";

	public const string CommitHash = $@"{_prefix}H";
	public const string AbbreviatedCommitHash = $@"{_prefix}h";
	public const string TreeHash = $@"{_prefix}T";
	public const string AbbreviatedTreeHash = $@"{_prefix}t";
	public const string ParentHashes = $@"{_prefix}P";
	public const string AbbreviatedParentHashes = $@"{_prefix}p";

	public static class Author
	{
		private const string _author = "a";

		public const string Name = $@"{_prefix}{_author}n";
		public const string NameRespectingMailmap = $@"{_prefix}{_author}N";
		public const string Email = $@"{_prefix}{_author}e";
		public const string EmailRespectingMailmap = $@"{_prefix}{_author}E";
		public const string EmailLocalPart = $@"{_prefix}{_author}l";
		public const string EmailLocalPartRespectingMailmap = $@"{_prefix}{_author}L";
		public const string Date = $@"{_prefix}{_author}d";
		public const string DateRFC2822 = $@"{_prefix}{_author}D";
		public const string DateRelative = $@"{_prefix}{_author}r";
		public const string DateUNIX = $@"{_prefix}{_author}t";
		public const string DateISO8601 = $@"{_prefix}{_author}i";
		public const string DateISO8601Strict = $@"{_prefix}{_author}I";
		public const string DateShort = $@"{_prefix}{_author}s";
		public const string DateHuman = $@"{_prefix}{_author}h";
	}

	public static class Committer
	{
		private const string _committer = "c";

		public const string Name = $@"{_prefix}{_committer}n";
		public const string NameRespectingMailmap = $@"{_prefix}{_committer}N";
		public const string Email = $@"{_prefix}e";
		public const string EmailRespectingMailmap = $@"{_prefix}{_committer}E";
		public const string EmailLocalPart = $@"{_prefix}{_committer}l";
		public const string EmailLocalPartRespectingMailmap = $@"{_prefix}{_committer}L";
		public const string Date = $@"{_prefix}{_committer}d";
		public const string DateRFC2822 = $@"{_prefix}{_committer}D";
		public const string DateRelative = $@"{_prefix}{_committer}r";
		public const string DateUNIX = $@"{_prefix}{_committer}t";
		public const string DateISO8601 = $@"{_prefix}{_committer}i";
		public const string DateISO8601Strict = $@"{_prefix}{_committer}I";
		public const string DateShort = $@"{_prefix}{_committer}s";
		public const string DateHuman = $@"{_prefix}{_committer}h";
	}

	public const string Decorate = $@"{_prefix}d";
	public const string DecorateNoWrap = $@"{_prefix}D";

	public const string RefName = $@"{_prefix}S";
	public const string Encoding = $@"{_prefix}e";
	public const string Subject = $@"{_prefix}s";
	public const string SubjectSanitized = $@"{_prefix}f";
	public const string Body = $@"{_prefix}b";
	public const string RawBody = $@"{_prefix}B";
	public const string Notes = $@"{_prefix}N";

	private const string _sign = "G";

	public const string SignatureMessage = $@"{_prefix}{_sign}G";
	public const string SignatureStatus = $@"{_prefix}{_sign}?";
	public const string SignerName = $@"{_prefix}{_sign}S";
	public const string SignKey = $@"{_prefix}{_sign}K";
	public const string SignKeyFingerprint = $@"{_prefix}{_sign}F";
	public const string SignPrimaryKeyFingerprint = $@"{_prefix}{_sign}P";
	public const string SignKeyTrustLevel = $@"{_prefix}{_sign}T";

	public static class Reflog
	{
		private const string _reflog = "g";

		public const string Selector = $@"{_prefix}{_reflog}D";
		public const string SelectorShortened = $@"{_prefix}{_reflog}d";
		public const string IdentityName = $@"{_prefix}{_reflog}n";
		public const string IdentityNameRespectingMailmap = $@"{_prefix}{_reflog}N";
		public const string IdentityEmail = $@"{_prefix}{_reflog}e";
		public const string IdentityEmailRespectingMailmap = $@"{_prefix}{_reflog}E";
		public const string Subject = $@"{_prefix}{_reflog}s";
	}

	public const string RevisionDataFormat =
		TreeHash + NewLine +
		ParentHashes + NewLine +
		Committer.DateISO8601Strict + NewLine +
		Committer.NameRespectingMailmap + NewLine +
		Committer.EmailRespectingMailmap + NewLine +
		Author.DateISO8601Strict + NewLine +
		Author.NameRespectingMailmap + NewLine +
		Author.EmailRespectingMailmap + NewLine +
		RawBody;

	public const string RevisionFormat =
		CommitHash + NewLine +
		RevisionDataFormat;

	public const string RevisionDataFormatOld =
		TreeHash + NewLine +
		ParentHashes + NewLine +
		Committer.DateISO8601Strict + NewLine +
		Committer.NameRespectingMailmap + NewLine +
		Committer.EmailRespectingMailmap + NewLine +
		Author.DateISO8601Strict + NewLine +
		Author.NameRespectingMailmap + NewLine +
		Author.EmailRespectingMailmap + NewLine +
		Subject + NewLine + NewLine + Body;

	public const string RevisionFormatOld =
		CommitHash + NewLine +
		RevisionDataFormatOld;

	public const string ReflogFormat =
		Reflog.SelectorShortened + NewLine +
		Reflog.Subject + NewLine +
		RevisionFormat;

	public const string ReflogFormatOld =
		Reflog.SelectorShortened + NewLine +
		Reflog.Subject + NewLine +
		RevisionFormatOld;
}
