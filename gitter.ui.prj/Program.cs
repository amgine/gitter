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

namespace gitter;

using System;

using Autofac;

using gitter.Framework;

internal static class Program
{
	/// <summary>The main entry point for the application.</summary>
	[STAThread]
	public static void Main() => GitterApplication.Run(
		static builder =>
		{
			builder.RegisterAssemblyModules(typeof(Program).Assembly);

			builder.RegisterModule<gitter.Framework.Module>();

			builder.RegisterModule<gitter.Git.Gui.Module>();

			builder.RegisterModule<gitter.GitLab.Module>();
			builder.RegisterModule<gitter.TeamCity.Module>();
			builder.RegisterModule<gitter.Redmine.Module>();

			builder
				.RegisterGeneric(typeof(AutofacFactory<>))
				.As(typeof(IFactory<>))
				.SingleInstance();
		});
}
