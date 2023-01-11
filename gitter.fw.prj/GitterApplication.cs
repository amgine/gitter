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

namespace gitter.Framework;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

using Autofac;

using gitter.Framework.Options;
using gitter.Framework.Services;
using gitter.Framework.Controls;
using gitter.Native;
using System.Runtime.InteropServices;

/// <summary>gitter application class.</summary>
public static class GitterApplication
{
	private static FormEx _mainForm;
	private static IWorkingEnvironment _environment;

	private static ITextRenderer _textRenderer;

	private static IGitterStyle _defaultStyle;
	private static IGitterStyle _style;
	private static IGitterStyle _styleOnNextStartup;

	/// <summary>Returns the selected text renderer for application.</summary>
	public static ITextRenderer TextRenderer
	{
		get => _textRenderer;
		set
		{
			Verify.Argument.IsNotNull(value);

			_textRenderer = value;
		}
	}

	public static IEnumerable<IGitterStyle> Styles { get; } =
		new IGitterStyle[]
		{
			_defaultStyle = new MSVS2010Style(),
			//new MSVS2012LightStyle(),
			new MSVS2012DarkStyle(),
		};

	public static IGitterStyle DefaultStyle => _defaultStyle;

	public static IGitterStyle Style
	{
		get
		{
			if(System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
			{
				return _defaultStyle;
			}
			return _style;
		}
		private set
		{
			Verify.Argument.IsNotNull(value);

			if(_style != value)
			{
				ToolStripManager.Renderer     = value.ToolStripRenderer;
				ViewManager.Renderer          = value.ViewRenderer;
				CustomListBoxManager.Renderer = value.ListBoxRenderer;
				_style = value;
			}
		}
	}

	public static IGitterStyle StyleOnNextStartup
	{
		get => _styleOnNextStartup;
		set
		{
			Verify.Argument.IsNotNull(value);

			_styleOnNextStartup = value;
		}
	}

	public static ITextRenderer GdiTextRenderer { get; } = new GdiTextRenderer();

	public static ITextRenderer GdiPlusTextRenderer { get; } = new GdiPlusTextRenderer();

	public static IMessageBoxService MessageBoxService { get; } = new CustomMessageBoxService();

	public static IWorkingEnvironment WorkingEnvironment => _environment;

	public static FormEx MainForm => _mainForm;

	public static SelectableFontManager FontManager { get; private set; }

	public static IntegrationFeatures IntegrationFeatures { get; private set; }

	private static void SetupDefaultExceptionHandling()
	{
		if(!System.Diagnostics.Debugger.IsAttached)
		{
			Application.ThreadException += OnThreadException;
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
		}
	}

	private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		if(e.ExceptionObject is Exception exc)
		{
			LoggingService.Global.Error(exc, "Application error");
			try
			{
				using var dlg = new ExceptionDialog(exc);
				dlg.Run(null);
			}
			catch
			{
			}
		}
		else
		{
			LoggingService.Global.Error("Unknown application error");
		}
	}

	private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
	{
		var exception = e.Exception;
		LoggingService.Global.Error(exception, "Application error");
		try
		{
			using var dlg = new ExceptionDialog(exception);
			dlg.Run(null);
		}
		catch
		{
		}
	}

	private static void SelectStyle(ConfigurationService configurationService)
	{
		var styleName = configurationService.GuiSection.GetValue<string>("Style", string.Empty);
		var style = Styles.FirstOrDefault(s => s.Name == styleName);
		style ??= Styles.First();
		_styleOnNextStartup = style;
		Style = style;
	}

	private static IContainer CreateContainer(Action<ContainerBuilder> configuration)
	{
		Assert.IsNotNull(configuration);

		var builder = new ContainerBuilder();

		builder
			.RegisterType<ConfigurationService>()
			.AsSelf()
			.SingleInstance();

		builder
			.RegisterInstance(MessageBoxService)
			.SingleInstance()
			.AsSelf()
			.ExternallyOwned();

		configuration?.Invoke(builder);

		return builder.Build();
	}

	public static void Run(Action<ContainerBuilder> configuration)
	{
		_textRenderer = GdiPlusTextRenderer;

		using var container = CreateContainer(configuration);

		if(Utility.IsOSWindows7OrNewer)
		{
			// for win7 we can provide explicit user id for better shell integration
			_ = Shell32.SetCurrentProcessExplicitAppUserModelID("gitter.app");
		}

		LoggingService.RegisterAppender(LogListBoxAppender.Instance);

		var log = LoggingService.Global;
		log.Info($"Application started");
#if NETCOREAPP
		log.Info($"Framework: {RuntimeInformation.FrameworkDescription} ({RuntimeInformation.RuntimeIdentifier})");
#else
		log.Info($"Framework: {RuntimeInformation.FrameworkDescription}");
#endif

		var configurationService = container.Resolve<ConfigurationService>();
		FontManager = new SelectableFontManager(configurationService.GlobalSection.GetCreateSection("Fonts"));
		IntegrationFeatures = new IntegrationFeatures();
		GlobalOptions.LoadFrom(configurationService.GlobalSection);

#if NETCOREAPP
		Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
#endif
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

		SelectStyle(configurationService);

		if(Utility.IsOSWindows7OrNewer)
		{
			Utility.EnableWin7TaskbarSupport();
		}
		try
		{
#if !DEBUG
			SetupDefaultExceptionHandling();
#endif
			using(_mainForm = container.ResolveNamed<FormEx>("main"))
			{
				_environment = (IWorkingEnvironment)_mainForm;

				Application.Run(_mainForm);

				_environment = null;
			}
		}
		finally
		{
			if(Utility.IsOSWindows7OrNewer)
			{
				Utility.DisableWin7TaskbarSupport();
			}
			_mainForm = null;
		}

		GlobalOptions.SaveTo(configurationService.GlobalSection);
		configurationService.GuiSection.SetValue<string>("Style", StyleOnNextStartup.Name);
		FontManager.Save();
		configurationService.Save();

		LoggingService.Global.Info("Application exited");
	}
}
