namespace gitter.Framework.Services
{
	using System;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	/// <summary>Event Notification Service.</summary>
	public interface INotificationService : IDisposable
	{
		void Notify(Control control, string title, string message);

		void Notify(ToolStripItem control, string title, string message);

		void Notify(Control control, NotificationType type, string title, string message);

		void Notify(ToolStripItem control, NotificationType type, string title, string message);

		void NotifyInputError(Control control, string title, string message);

		void NotifyInputError(Control control, NotificationType type, string title, string message);
	}
}
