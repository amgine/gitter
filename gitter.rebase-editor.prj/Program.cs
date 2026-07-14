#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2026  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.RebaseEditor;

using System.IO;
using System.IO.Pipes;
using System.Text;

// args: <pipename> <todo-filepath>
static class Program
{
	static int Main(string[] args)
	{
		if(args.Length < 2) return 1;

		var pipeName     = args[0];
		var todoFilePath = args[1];

		using var pipe = new NamedPipeClientStream(
			serverName: ".",
			pipeName:   pipeName,
			direction:  PipeDirection.InOut,
			options:    PipeOptions.None);

		pipe.Connect(30_000);

		using var writer = new StreamWriter(pipe, Encoding.UTF8, bufferSize: 1024, leaveOpen: true);
		writer.WriteLine(todoFilePath);
		writer.Flush();

		pipe.ReadByte();
		return 0;
	}
}
