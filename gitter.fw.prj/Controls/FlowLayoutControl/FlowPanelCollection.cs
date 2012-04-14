namespace gitter.Framework.Controls
{
	using System;
	using System.ComponentModel;

	/// <summary>Collection of <see cref="FlowPanel"/>'s hosted in <see cref="FlowLayoutControl"/>.</summary>
	public sealed class FlowPanelCollection : SafeNotifySortedCollection<FlowPanel>
	{
		#region Data

		private readonly FlowLayoutControl _control;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="FlowPanelCollection"/>.</summary>
		/// <param name="control">Host <see cref="FlowLayoutControl"/>.</param>
		internal FlowPanelCollection(FlowLayoutControl control)
		{
			if(control == null) throw new ArgumentNullException("control");

			_control = control;
		}

		#endregion

		#region Properties

		/// <summary>Host <see cref="FlowLayoutControl"/>.</summary>
		public FlowLayoutControl FlowLayoutControl
		{
			get { return _control; }
		}

		#endregion

		#region Overrides

		protected override ISynchronizeInvoke SynchronizationObject
		{
			get { return _control; }
		}

		protected override void FreeItem(FlowPanel item)
		{
			item.FlowControl = null;
		}

		protected override void AcquireItem(FlowPanel item)
		{
			item.FlowControl = _control;
		}

		protected override bool VerifyItem(FlowPanel item)
		{
			return item != null && item.FlowControl == null;
		}

		#endregion
	}
}
