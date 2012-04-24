namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework.Configuration;
	using gitter.Framework.Services;

	/// <summary>Represents control with advanced docking capabilities.</summary>
	[ToolboxItem(false)]
	public partial class ViewBase : UserControl
	{
		#region Data

		private readonly Guid _guid;
		private readonly IWorkingEnvironment _environment;
		private IDictionary<string, object> _parameters;
		private ViewHost _host;
		private INotificationService _notificationService;
		private IToolTipService _toolTipService;

		#endregion

		#region Events

		private static readonly object ClosingEvent = new object();
		public event EventHandler Closing
		{
			add { Events.AddHandler(ClosingEvent, value); }
			remove { Events.RemoveHandler(ClosingEvent, value); }
		}

		#endregion

		#region .ctor

		/// <summary>To keep designer happy.</summary>
		public ViewBase()
		{
			Size = new Size(555, 362);
			AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			BackColor = System.Drawing.SystemColors.Window;
			if(LicenseManager.UsageMode == LicenseUsageMode.Runtime)
			{
				Font = GitterApplication.FontManager.UIFont;
			}
			else
			{
				Font = SystemFonts.MessageBoxFont;
			}
		}

		/// <summary>Create <see cref="ViewBase"/>.</summary>
		public ViewBase(Guid guid, IWorkingEnvironment environment, IDictionary<string, object> parameters)
			: this()
		{
			if(environment == null) throw new ArgumentNullException("environment");

			_guid = guid;
			_environment = environment;
			_parameters = parameters;
		}

		#endregion

		public IWorkingEnvironment WorkingEnvironment
		{
			get { return _environment; }
		}

		protected INotificationService NotificationService
		{
			get
			{
				if(_notificationService == null)
				{
					_notificationService = new BalloonNotificationService();
				}
				return _notificationService;
			}
		}

		protected IToolTipService ToolTipService
		{
			get
			{
				if(_toolTipService == null)
				{
					_toolTipService = new DefaultToolTipService();
				}
				return _toolTipService;
			}
		}

		//protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		//{
		//    switch(e.KeyCode)
		//    {
		//        case Keys.F4:
		//            if(e.Control)
		//            {
		//                e.IsInputKey = true;
		//            }
		//            break;
		//    }
		//    base.OnPreviewKeyDown(e);
		//}

		//protected override void OnKeyDown(KeyEventArgs e)
		//{
		//    switch(e.KeyCode)
		//    {
		//        case Keys.F4:
		//            if(e.Control)
		//            {
		//                Close();
		//            }
		//            break;
		//    }
		//    base.OnKeyDown(e);
		//}

		public Guid Guid
		{
			get { return _guid; }
		}

		public IDictionary<string, object> Parameters
		{
			get { return _parameters; }
		}

		public virtual bool IsDocument
		{
			get { return false; }
		}

		public void Activate()
		{
			if(_host != null)
			{
				if(_host.Status == ViewHostStatus.AutoHide)
				{
					_host.DockSide.ActivateView(this);
				}
				else
				{
					_host.Activate(this);
				}
			}
			OnActivated();
		}

		protected virtual void OnClosing()
		{
			Events.Raise(ClosingEvent, this);
		}

		public void Close()
		{
			if(_host != null)
			{
				_host.RemoveView(this);
				Dispose();
			}
		}

		public virtual bool ParametersIdentical(IDictionary<string, object> parameters)
		{
			if(_parameters == parameters) return true;
			if(_parameters == null || parameters == null)
			{
				if(_parameters == null)
				{
					if(parameters == null) return true;
					if(parameters.Count == 0) return true;
				}
				else
				{
					if(_parameters == null) return true;
					if(_parameters.Count == 0) return true;
				}
				return false;
			}
			else
			{
				if(_parameters.Count == parameters.Count)
				{
					foreach(var kvp in _parameters)
					{
						object value;
						if(!parameters.TryGetValue(kvp.Key, out value) ||
							!object.Equals(value, kvp.Value))
						{
							return false;
						}
					}
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public virtual void ApplyParameters(IDictionary<string, object> parameters)
		{
		}

		internal ViewHost Host
		{
			get { return _host; }
			set { _host = value; }
		}

		internal bool IsHosted
		{
			get { return _host != null; }
		}

		public virtual void OnActivated()
		{
		}

		/// <summary>View's identification string.</summary>
		public virtual string IdentificationString
		{
			get { return GetType().Name; }
		}

		/// <summary>View's text.</summary>
		public override string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		/// <summary>View's image.</summary>
		public virtual Image Image
		{
			get { return null; }
		}

		protected void AddTopToolStrip(ToolStrip toolStrip)
		{
			if(toolStrip == null) throw new ArgumentNullException("toolStrip");
			toolStrip.GripStyle = ToolStripGripStyle.Hidden;
			toolStrip.Stretch = true;
			toolStrip.Padding = new Padding(2);
			toolStrip.Dock = DockStyle.Top;
			Controls.Add(toolStrip);
			toolStrip.MouseDown += OnToolStripClick;
			foreach(Control c in Controls)
			{
				if(c.Dock == DockStyle.None)
				{
					if((c.Anchor & AnchorStyles.Top) == AnchorStyles.Top)
					{
						c.Top += toolStrip.Height;
						if((c.Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
						{
							c.Height -= toolStrip.Height;
						}
					}
				}
			}
		}

		protected void AddBottomToolStrip(ToolStrip toolStrip)
		{
			if(toolStrip == null) throw new ArgumentNullException("toolStrip");
			toolStrip.GripStyle = ToolStripGripStyle.Hidden;
			toolStrip.Stretch = true;
			toolStrip.Padding = new Padding(2);
			toolStrip.Dock = DockStyle.Bottom;
			Controls.Add(toolStrip);
			toolStrip.MouseDown += OnToolStripClick;
			foreach(Control c in Controls)
			{
				if(c.Dock == DockStyle.None)
				{
					if((c.Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
					{
						if((c.Anchor & AnchorStyles.Top) == AnchorStyles.Top)
						{
							c.Height -= toolStrip.Height;
						}
						else
						{
							c.Top -= toolStrip.Height;
						}
					}
				}
			}
		}

		protected void RemoveToolStrip(ToolStrip toolStrip)
		{
			if(toolStrip == null) throw new ArgumentNullException("toolStrip");
			if(toolStrip.Parent != this) throw new ArgumentException("toolStrip");
			var dock = toolStrip.Dock;
			toolStrip.Parent = null;
			toolStrip.Click -= OnToolStripClick;
			switch(dock)
			{
				case DockStyle.Top:
					foreach(Control c in Controls)
					{
						if(c.Dock == DockStyle.None)
						{
							if((c.Anchor & AnchorStyles.Top) == AnchorStyles.Top)
							{
								if((c.Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
								{
									c.SuspendLayout();
									c.Top -= toolStrip.Height;
									c.Height += toolStrip.Height;
									c.ResumeLayout();
								}
								else
								{
									c.Top -= toolStrip.Height;
								}
							}
						}
					}
					break;
				case DockStyle.Bottom:
					foreach(Control c in Controls)
					{
						if(c.Dock == DockStyle.None)
						{
							if((c.Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
							{
								if((c.Anchor & AnchorStyles.Top) == AnchorStyles.Top)
								{
									c.Height += toolStrip.Height;
								}
								else
								{
									c.Top += toolStrip.Height;
								}
							}
						}
					}
					break;
				default:
					throw new ArgumentException("toolStrip");
			}
		}

		private void OnToolStripClick(object sender, EventArgs e)
		{
			Focus();
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				OnClosing();
				if(_notificationService != null)
				{
					_notificationService.Dispose();
					_notificationService = null;
				}
				if(_toolTipService != null)
				{
					_toolTipService.Dispose();
					_toolTipService = null;
				}
				_parameters = null;
			}
			base.Dispose(disposing);
		}

		protected virtual void SaveMoreViewTo(Section section)
		{
		}

		public void SaveViewTo(Section section)
		{
			if(section == null) throw new ArgumentNullException("section");
			SaveMoreViewTo(section);
		}

		protected virtual void LoadMoreViewFrom(Section section)
		{
		}

		public void LoadViewFrom(Section section)
		{
			if(section == null) throw new ArgumentNullException("section");
			LoadMoreViewFrom(section);
		}
	}
}
