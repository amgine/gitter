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
	using System.Linq;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Threading;

	using gitter.Framework.Options;
	using gitter.Framework.Services;
	using gitter.Framework.Controls;
	using gitter.Native;

	/// <summary>gitter application class.</summary>
	public static class GitterApplication
	{
		private static FormEx _mainForm;
		private static IWorkingEnvironment _environment;

		private static SelectableFontManager _fontManager;
		private static ConfigurationService _configurationService;
		private static IntegrationFeatures _integrationFeatures;

		private static readonly ITextRenderer _gdiPlusTextRenderer = new GdiPlusTextRenderer();
		private static readonly ITextRenderer _gdiTextRenderer = new GdiTextRenderer();
		private static ITextRenderer _defaultTextRenderer = _gdiPlusTextRenderer;

		private static readonly IMessageBoxService _messageBoxService = new CustomMessageBoxService();

		private static readonly IGitterStyle[] _styles =
			new IGitterStyle[]
			{
				_defaultStyle = new MSVS2010Style(),
				//new MSVS2012LightStyle(),
				new MSVS2012DarkStyle(),
			};

		private static IGitterStyle _defaultStyle;
		private static IGitterStyle _style;
		private static IGitterStyle _styleOnNextStartup;

		/// <summary>Returns the selected text renderer for application.</summary>
		public static ITextRenderer TextRenderer
		{
			get { return _defaultTextRenderer; }
			set
			{
				Verify.Argument.IsNotNull(value, nameof(value));

				_defaultTextRenderer = value;
			}
		}

		public static IEnumerable<IGitterStyle> Styles
		{
			get { return _styles; }
		}

		public static IGitterStyle DefaultStyle
		{
			get { return _defaultStyle; }
		}

		public static IGitterStyle Style
		{
			get
			{
				if(LicenseManager.UsageMode == LicenseUsageMode.Designtime)
				{
					return _defaultStyle;
				}
				return _style;
			}
			private set
			{
				Verify.Argument.IsNotNull(value, nameof(value));

				if(_style != value)
				{
					ToolStripManager.Renderer		= value.ToolStripRenderer;
					ViewManager.Renderer			= value.ViewRenderer;
					CustomListBoxManager.Renderer	= value.ListBoxRenderer;
					_style = value;
				}
			}
		}

		public static IGitterStyle StyleOnNextStartup
		{
			get { return _styleOnNextStartup; }
			set
			{
				Verify.Argument.IsNotNull(value, nameof(value));

				_styleOnNextStartup = value;
			}
		}

		public static ITextRenderer GdiTextRenderer
		{
			get { return _gdiTextRenderer; }
		}

		public static ITextRenderer GdiPlusTextRenderer
		{
			get { return _gdiPlusTextRenderer; }
		}

		public static IMessageBoxService MessageBoxService
		{
			get { return _messageBoxService; }
		}

		public static IWorkingEnvironment WorkingEnvironment
		{
			get { return _environment; }
		}

		public static FormEx MainForm
		{
			get { return _mainForm; }
		}

		public static SelectableFontManager FontManager
		{
			get { return _fontManager; }
		}

		public static ConfigurationService ConfigurationService
		{
			get { return _configurationService; }
		}

		public static IntegrationFeatures IntegrationFeatures
		{
			get { return _integrationFeatures; }
		}

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
			var exc = e.ExceptionObject as Exception;
			if(exc != null)
			{
				LoggingService.Global.Error(exc, "Application error");
				try
				{
					using(var dlg = new ExceptionDialog(exc))
					{
						dlg.Run(null);
					}
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
				using(var dlg = new ExceptionDialog(exception))
				{
					dlg.Run(null);
				}
			}
			catch
			{
			}
		}

		private static void SelectStyle()
		{
			var styleName = _configurationService.GuiSection.GetValue<string>("Style", string.Empty);
			var style = Styles.FirstOrDefault(s => s.Name == styleName);
			if(style == null)
			{
				style = Styles.First();
			}
			_styleOnNextStartup = style;
			Style = style;
		}

		public static void Run<T>()
			where T: FormEx, IWorkingEnvironment, new()
		{
			if(Utility.IsOSWindows7OrNewer)
			{
				// for win7 we can provide explicit user id for better shell integration
				Shell32.SetCurrentProcessExplicitAppUserModelID("gitter.app");
			}

			LoggingService.RegisterAppender(LogListBoxAppender.Instance);

			LoggingService.Global.Info("Application started");

			_configurationService = new ConfigurationService();
			_fontManager = new SelectableFontManager(_configurationService.GlobalSection.GetCreateSection("Fonts"));
			_integrationFeatures = new IntegrationFeatures();
			GlobalOptions.LoadFrom(_configurationService.GlobalSection);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			SelectStyle();

			if(Utility.IsOSWindows7OrNewer)
			{
				Utility.EnableWin7TaskbarSupport();
			}
			try
			{
#if !DEBUG
				SetupDefaultExceptionHandling();
#endif
				using(_mainForm = new T())
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

			GlobalOptions.SaveTo(_configurationService.GlobalSection);
			_configurationService.GuiSection.SetValue<string>("Style", StyleOnNextStartup.Name);
			_fontManager.Save();
			_configurationService.Save();

			LoggingService.Global.Info("Application exited");
		}
	}
}
