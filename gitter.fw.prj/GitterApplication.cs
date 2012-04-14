namespace gitter.Framework
{
	using System;
	using System.Windows.Forms;
	using System.Reflection;
	using System.Text;
	using System.Threading;

	using gitter.Framework.Options;
	using gitter.Framework.Services;
	using gitter.Framework.Controls;

	/// <summary>gitter application class.</summary>
	public static class GitterApplication
	{
		private static FormEx _mainForm;
		private static IWorkingEnvironment _environment;

		private static SelectableFontManager _fontManager;
		private static ConfigurationService _configurationService;

		private static readonly ITextRenderer _gdiPlusTextRenderer = new GdiPlusTextRenderer();
		private static readonly ITextRenderer _gdiTextRenderer = new GdiTextRenderer();
		private static ITextRenderer _defaultTextRenderer = _gdiPlusTextRenderer;

		private static readonly IMessageBoxService _messageBoxService = new CustomMessageBoxService();

		/// <summary>Returns the selected text renderer for application.</summary>
		public static ITextRenderer TextRenderer
		{
			get { return _defaultTextRenderer; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				_defaultTextRenderer = value;
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

		private static void SetupDefaultExceptionHandling()
		{
			Application.ThreadException += OnThreadException;
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
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

		public static void Run<T>()
			where T: FormEx, IWorkingEnvironment, new()
		{
			if(Utility.IsOSWindows7OrNewer)
			{
				// for win7 we can provide explicit user id for better shell integration
				NativeMethods.SetCurrentProcessExplicitAppUserModelID("gitter.app");
			}

			LoggingService.RegisterAppender(LogListBoxAppender.Instance);

			LoggingService.Global.Info("Application started");

			_configurationService = new ConfigurationService();
			_fontManager = new SelectableFontManager(_configurationService.GlobalSection.GetCreateSection("Fonts"));
			GlobalOptions.LoadFrom(_configurationService.GlobalSection);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			ToolStripManager.Renderer = new MSVS2010StyleRenderer();

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
			_fontManager.Save();
			_configurationService.Save();

			LoggingService.Global.Info("Application exited");
		}
	}
}
