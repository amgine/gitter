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

namespace gitter.Framework
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Runtime.Remoting;
	using System.Runtime.Remoting.Channels;
	using System.Runtime.Remoting.Channels.Ipc;
	using System.Threading;

	public static class HelperExecutables
	{
		/// <summary>Runs <see cref="action"/> with administrator privileges, requesting them by UAC if necessary.</summary>
		/// <param name="action">Action to execute.</param>
		public static void ExecuteWithAdministartorRights(Action action)
		{
			Verify.Argument.IsNotNull(action, "action");

			if(Utility.IsRunningWithAdministratorRights)
			{
				action();
			}
			else
			{
				const int waitTime = 50;
				const string RemotingChannelName = "gitter.UAC";
				const string RemotingObjectName = "RemoteExecutor.rem";

				var ipc = new IpcChannel("RemoteExecutorClient");
				ChannelServices.RegisterChannel(ipc, false);
				try
				{
					var path = Path.Combine(Path.GetDirectoryName(typeof(Utility).Assembly.Location), "gitter.uac.exe");
					Process administratorProcess;
					bool allowWaitForExit = false;
					using(administratorProcess = new Process()
					{
						StartInfo = new ProcessStartInfo(path, "--remoting")
						{
							CreateNoWindow = true,
						},
					})
					{
						administratorProcess.Start();
						try
						{
							IRemoteProcedureExecutor executor = null;
							while(!administratorProcess.HasExited)
							{
								System.Threading.Thread.Sleep(waitTime);
								try
								{
									executor = (IRemoteProcedureExecutor)Activator.GetObject(typeof(IRemoteProcedureExecutor),
										string.Format(@"ipc://{0}/{1}", RemotingChannelName, RemotingObjectName));
								}
								catch(Exception exc)
								{
									if(exc.IsCritical())
									{
										throw;
									}
								}
								if(executor != null)
								{
									break;
								}
							}
							if(executor != null)
							{
								while(!administratorProcess.HasExited)
								{
									try
									{
										executor.Execute(action);
										break;
									}
									catch(RemotingException)
									{
										System.Threading.Thread.Sleep(waitTime);
									}
								}
								if(!administratorProcess.HasExited)
								{
									try
									{
										executor.Close();
										allowWaitForExit = true;
									}
									catch(Exception exc)
									{
										if(exc.IsCritical())
										{
											throw;
										}
									}
								}
							}
							else
							{
								throw new Exception();
							}
						}
						finally
						{
							try
							{
								if(allowWaitForExit)
								{
									if(!administratorProcess.WaitForExit(750))
									{
										administratorProcess.Kill();
									}
								}
								else
								{
									administratorProcess.Kill();
								}
							}
							catch(Exception exc)
							{
								if(exc.IsCritical())
								{
									throw;
								}
							}
						}
					}
				}
				finally
				{
					ChannelServices.UnregisterChannel(ipc);
				}
			}
		}

		public static bool CheckIfUpdaterIsRunning()
		{
			bool singleInstance = false;
			using(var s = new Semaphore(0, 1, "gitter-updater", out singleInstance))
			{
			}
			return !singleInstance;
		}

		public static bool CheckIfCanLaunchUpdater()
		{
			const string updaterExeName = @"gitter.updater.exe";

			var sourcePath = Path.GetDirectoryName(typeof(Utility).Assembly.Location);
			return File.Exists(Path.Combine(sourcePath, updaterExeName)) && !CheckIfUpdaterIsRunning();
		}

		public static void LaunchUpdater(string cmdline)
		{
			const string tempPathName = @"gitter-updater";
			const string updaterExeName = @"gitter.updater.exe";
			const string updaterConfigName = @"gitter.updater.exe.config";

			var sourcePath = Path.GetDirectoryName(typeof(HelperExecutables).Assembly.Location);
			var targetPath = Path.Combine(Path.GetTempPath(), tempPathName);
			if(!Directory.Exists(targetPath))
			{
				Directory.CreateDirectory(targetPath);
			}
			File.Copy(Path.Combine(sourcePath, updaterExeName), Path.Combine(targetPath, updaterExeName), true);
			File.Copy(Path.Combine(sourcePath, updaterConfigName), Path.Combine(targetPath, updaterConfigName), true);
			using(var process = new Process()
				{
					StartInfo = new ProcessStartInfo(Path.Combine(targetPath, updaterExeName), cmdline)
					{
						WindowStyle = ProcessWindowStyle.Normal,
						CreateNoWindow = false,
						LoadUserProfile = true,
					},
				})
			{
				process.Start();
			}
		}

		public static void CleanupUpdaterTemporaryFiles()
		{
			const string tempPathName = @"gitter-updater";
			var path = Path.Combine(Path.GetTempPath(), tempPathName);
			Directory.Delete(path, true);
		}
	}
}
