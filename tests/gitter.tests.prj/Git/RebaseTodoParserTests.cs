#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git;

using System;
using System.Collections.Generic;
using System.IO;

using gitter.Git.AccessLayer.CLI;

using NUnit.Framework;

[TestFixture]
class RebaseTodoParserTests
{
	static string WriteTempFile(string content)
	{
		var path = Path.GetTempFileName();
		File.WriteAllText(path, content);
		return path;
	}

	[Test]
	public void Read_StandardEntries_ParsesCorrectly()
	{
		var path = WriteTempFile(
			"pick a1b2c3d First commit\n" +
			"squash e4f5a6b Second commit\n" +
			"drop  c7d8e9f Third commit\n");
		try
		{
			var entries = RebaseTodoParser.Read(path);
			Assert.That(entries.Count,          Is.EqualTo(3));
			Assert.That(entries[0].Action,      Is.EqualTo(RebaseAction.Pick));
			Assert.That(entries[0].Hash,        Is.EqualTo("a1b2c3d"));
			Assert.That(entries[0].Subject,     Is.EqualTo("First commit"));
			Assert.That(entries[1].Action,      Is.EqualTo(RebaseAction.Squash));
			Assert.That(entries[2].Action,      Is.EqualTo(RebaseAction.Drop));
		}
		finally { File.Delete(path); }
	}

	[Test]
	public void Read_SkipsCommentLines()
	{
		var path = WriteTempFile(
			"# This is a comment\n" +
			"pick a1b2c3d Only commit\n" +
			"# another comment\n");
		try
		{
			var entries = RebaseTodoParser.Read(path);
			Assert.That(entries.Count, Is.EqualTo(1));
			Assert.That(entries[0].Hash, Is.EqualTo("a1b2c3d"));
		}
		finally { File.Delete(path); }
	}

	[Test]
	public void Read_SkipsBlankLines()
	{
		var path = WriteTempFile(
			"\n" +
			"pick a1b2c3d Commit one\n" +
			"\n" +
			"reword b2c3d4e Commit two\n" +
			"\n");
		try
		{
			var entries = RebaseTodoParser.Read(path);
			Assert.That(entries.Count, Is.EqualTo(2));
		}
		finally { File.Delete(path); }
	}

	[Test]
	public void Write_ThenRead_RoundTrips()
	{
		var original = new List<RebaseTodoEntry>
		{
			new(RebaseAction.Pick,   "aaa1111", "First"),
			new(RebaseAction.Squash, "bbb2222", "Second"),
			new(RebaseAction.Fixup,  "ccc3333", "Third"),
			new(RebaseAction.Drop,   "ddd4444", "Fourth"),
		};

		var path = Path.GetTempFileName();
		try
		{
			RebaseTodoParser.Write(path, original);
			var roundTripped = RebaseTodoParser.Read(path);

			Assert.That(roundTripped.Count, Is.EqualTo(original.Count));
			for(var i = 0; i < original.Count; i++)
			{
				Assert.That(roundTripped[i].Action,  Is.EqualTo(original[i].Action));
				Assert.That(roundTripped[i].Hash,    Is.EqualTo(original[i].Hash));
				Assert.That(roundTripped[i].Subject, Is.EqualTo(original[i].Subject));
			}
		}
		finally { File.Delete(path); }
	}

	[Test]
	public void Read_ExecEntry_HasEmptyHash()
	{
		var path = WriteTempFile("exec make test\n");
		try
		{
			var entries = RebaseTodoParser.Read(path);
			Assert.That(entries.Count,      Is.EqualTo(1));
			Assert.That(entries[0].Action,  Is.EqualTo(RebaseAction.Exec));
			Assert.That(entries[0].Hash,    Is.EqualTo(string.Empty));
			Assert.That(entries[0].Subject, Is.EqualTo("make test"));
		}
		finally { File.Delete(path); }
	}

	[Test]
	public void Read_ExecEntry_PreservesSpacesInCommand()
	{
		var path = WriteTempFile("exec npm run test -- --watch\n");
		try
		{
			var entries = RebaseTodoParser.Read(path);
			Assert.That(entries[0].Subject, Is.EqualTo("npm run test -- --watch"));
		}
		finally { File.Delete(path); }
	}

	[Test]
	public void Write_ExecEntry_WritesWithoutHash()
	{
		var entries = new List<RebaseTodoEntry>
		{
			new(RebaseAction.Pick, "aaa1111", "A commit"),
			new(RebaseAction.Exec, string.Empty, "make test"),
		};

		var path = Path.GetTempFileName();
		try
		{
			RebaseTodoParser.Write(path, entries);
			var lines = File.ReadAllLines(path);
			Assert.That(lines[0], Is.EqualTo("pick aaa1111 A commit"));
			Assert.That(lines[1], Is.EqualTo("exec make test"));
		}
		finally { File.Delete(path); }
	}
}
