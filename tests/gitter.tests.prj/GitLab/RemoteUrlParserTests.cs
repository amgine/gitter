#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

#nullable enable

namespace gitter.GitLab;

using System;

using NUnit.Framework;

[TestFixture]
class RemoteUrlParserTests
{
	private static readonly Uri GitLabCom = new(@"https://gitlab.com/");

	private static string? Parse(string remote, string service = @"https://gitlab.com/")
		=> RemoteUrlParser.TryGetProjectPath(new Uri(service), remote, out var path) ? path : null;

	#region .git suffix

	[Test]
	public void Https_StripsDotGitSuffix()
	{
		Assert.That(Parse(@"https://gitlab.com/group/project.git"), Is.EqualTo(@"group/project"));
	}

	[Test]
	public void Https_WithoutDotGitSuffix_IsUnchanged()
	{
		Assert.That(Parse(@"https://gitlab.com/group/project"), Is.EqualTo(@"group/project"));
	}

	[Test]
	public void Https_StripsDotGitSuffixWithTrailingSlash()
	{
		Assert.That(Parse(@"https://gitlab.com/group/project.git/"), Is.EqualTo(@"group/project"));
	}

	[Test]
	public void Https_StripsTrailingSlash()
	{
		Assert.That(Parse(@"https://gitlab.com/group/project/"), Is.EqualTo(@"group/project"));
	}

	[Test]
	public void Ssh_ScpLike_StripsDotGitSuffix()
	{
		Assert.That(Parse(@"git@gitlab.com:group/project.git"), Is.EqualTo(@"group/project"));
	}

	[Test]
	public void Ssh_Url_StripsDotGitSuffix()
	{
		Assert.That(Parse(@"ssh://git@gitlab.com/group/project.git"), Is.EqualTo(@"group/project"));
	}

	[Test]
	public void Ssh_UrlWithNonDefaultPort_StripsDotGitSuffix()
	{
		Assert.That(Parse(@"ssh://git@gitlab.com:2222/group/project.git"), Is.EqualTo(@"group/project"));
	}

	[Test]
	public void Git_Protocol_StripsDotGitSuffix()
	{
		Assert.That(Parse(@"git://gitlab.com/group/project.git"), Is.EqualTo(@"group/project"));
	}

	#endregion

	#region nested namespaces

	[Test]
	public void Https_KeepsNestedSubgroups()
	{
		Assert.That(Parse(@"https://gitlab.com/group/sub/deeper/project.git"),
			Is.EqualTo(@"group/sub/deeper/project"));
	}

	[Test]
	public void ScpLike_KeepsNestedSubgroups()
	{
		Assert.That(Parse(@"git@gitlab.com:group/sub/deeper/project.git"),
			Is.EqualTo(@"group/sub/deeper/project"));
	}

	[Test]
	public void ScpLike_LeadingSlashAfterColonIsIgnored()
	{
		Assert.That(Parse(@"git@gitlab.com:/group/project.git"), Is.EqualTo(@"group/project"));
	}

	#endregion

	#region real world shapes

	private const string Nested = @"promcontrol/viscont/nugets/viscont.core.framework.sparkpulse";
	private const string Mallenom = @"https://gitlab.mallenom.dev/";

	[Test]
	public void Nested_Https_WithDotGit()
	{
		Assert.That(Parse($@"{Mallenom}{Nested}.git", Mallenom), Is.EqualTo(Nested));
	}

	[Test]
	public void Nested_Https_WithoutDotGit()
	{
		Assert.That(Parse($@"{Mallenom}{Nested}", Mallenom), Is.EqualTo(Nested));
	}

	[Test]
	public void Nested_ScpLike()
	{
		Assert.That(Parse($@"git@gitlab.mallenom.dev:{Nested}.git", Mallenom), Is.EqualTo(Nested));
	}

	[Test]
	public void Nested_Ssh()
	{
		Assert.That(Parse($@"ssh://git@gitlab.mallenom.dev/{Nested}.git", Mallenom), Is.EqualTo(Nested));
	}

	[Test]
	public void DotsInProjectName_AreNotMistakenForTheDotGitSuffix()
	{
		Assert.That(Parse($@"{Mallenom}{Nested}.git", Mallenom),
			Does.EndWith(@"viscont.core.framework.sparkpulse"));
	}

	[Test]
	public void HttpsRemote_DoesNotMatchAnHttpServerEntryOnTheSameHost()
	{
		Assert.That(Parse($@"{Mallenom}{Nested}.git", @"http://gitlab.mallenom.dev/"), Is.Null);
	}

	[Test]
	public void HttpRemote_DoesNotMatchAnHttpsServerEntryOnTheSameHost()
	{
		Assert.That(Parse($@"http://gitlab.mallenom.dev/{Nested}.git", Mallenom), Is.Null);
	}

	[Test]
	public void ScpLikeRemote_MatchesEitherSchemeOfTheSameHost()
	{
		Assert.That(Parse($@"git@gitlab.mallenom.dev:{Nested}.git", Mallenom),                    Is.EqualTo(Nested));
		Assert.That(Parse($@"git@gitlab.mallenom.dev:{Nested}.git", @"http://gitlab.mallenom.dev/"), Is.EqualTo(Nested));
	}

	#endregion

	#region noise the old implementation kept

	[Test]
	public void Https_DropsQueryString()
	{
		Assert.That(Parse(@"https://gitlab.com/group/project.git?ref=main"), Is.EqualTo(@"group/project"));
	}

	[Test]
	public void Https_DropsFragment()
	{
		Assert.That(Parse(@"https://gitlab.com/group/project.git#anchor"), Is.EqualTo(@"group/project"));
	}

	[Test]
	public void Https_DropsUserInfo()
	{
		Assert.That(Parse(@"https://oauth2:token@gitlab.com/group/project.git"), Is.EqualTo(@"group/project"));
	}

	[Test]
	public void Https_UnescapesPercentEncodedPath()
	{
		Assert.That(Parse(@"https://gitlab.com/my%20group/project.git"), Is.EqualTo(@"my group/project"));
	}

	[Test]
	public void SurroundingWhitespaceIsIgnored()
	{
		Assert.That(Parse("  https://gitlab.com/group/project.git\t"), Is.EqualTo(@"group/project"));
	}

	#endregion

	#region self-hosted with relative url root

	[Test]
	public void Https_StripsServiceUriBasePath()
	{
		Assert.That(Parse(@"https://host.example.com/gitlab/group/project.git", @"https://host.example.com/gitlab/"),
			Is.EqualTo(@"group/project"));
	}

	[Test]
	public void Https_ServiceUriBasePathWithoutTrailingSlash_IsStripped()
	{
		Assert.That(Parse(@"https://host.example.com/gitlab/group/project.git", @"https://host.example.com/gitlab"),
			Is.EqualTo(@"group/project"));
	}

	[Test]
	public void ScpLike_DoesNotStripServiceUriBasePath()
	{
		Assert.That(Parse(@"git@host.example.com:group/project.git", @"https://host.example.com/gitlab/"),
			Is.EqualTo(@"group/project"));
	}

	#endregion

	#region host matching

	[Test]
	public void HostComparisonIsCaseInsensitive()
	{
		Assert.That(Parse(@"https://GitLab.COM/group/project.git"), Is.EqualTo(@"group/project"));
	}

	[Test]
	public void ScpLikeHostComparisonIsCaseInsensitive()
	{
		Assert.That(Parse(@"git@GitLab.COM:group/project.git"), Is.EqualTo(@"group/project"));
	}

	[Test]
	public void ForeignHost_IsRejected()
	{
		Assert.That(Parse(@"https://github.com/group/project.git"), Is.Null);
	}

	[Test]
	public void ForeignHost_ScpLike_IsRejected()
	{
		Assert.That(Parse(@"git@github.com:group/project.git"), Is.Null);
	}

	[Test]
	public void Https_NonMatchingPort_IsRejected()
	{
		Assert.That(Parse(@"https://gitlab.com:8443/group/project.git"), Is.Null);
	}

	[Test]
	public void Https_MatchingNonDefaultPort_IsAccepted()
	{
		Assert.That(Parse(@"https://gitlab.com:8443/group/project.git", @"https://gitlab.com:8443/"),
			Is.EqualTo(@"group/project"));
	}

	[Test]
	public void Ssh_PortIsNotComparedAgainstHttpsServicePort()
	{
		Assert.That(Parse(@"ssh://git@gitlab.com:22/group/project.git"), Is.EqualTo(@"group/project"));
	}

	#endregion

	#region rejected input

	[TestCase(@"")]
	[TestCase(@"   ")]
	[TestCase(@"https://gitlab.com/")]
	[TestCase(@"https://gitlab.com/.git")]
	[TestCase(@"git@gitlab.com:")]
	[TestCase(@"D:\repos\project")]
	[TestCase(@"../relative/path")]
	public void MalformedOrEmpty_IsRejected(string remote)
	{
		Assert.That(RemoteUrlParser.TryGetProjectPath(GitLabCom, remote, out _), Is.False);
	}

	[Test]
	public void Null_IsRejected()
	{
		Assert.That(RemoteUrlParser.TryGetProjectPath(GitLabCom, null, out _), Is.False);
	}

	#endregion
}
