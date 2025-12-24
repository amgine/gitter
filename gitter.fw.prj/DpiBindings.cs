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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework.Controls;

public class DpiBindings
{
	abstract class ImageControllerBase<T> : IImageController
		where T : class
	{
		private IImageProvider? _image;
		private Dpi _dpi;

		protected ImageControllerBase(T target, int size)
		{
			Assert.IsNotNull(target);

			Target = target;
			Size   = size;
		}

		protected int Size { get; }

		protected T Target { get; }

		public IImageProvider? Image
		{
			get => _image;
			set
			{
				if(_image == value) return;

				_image = value;
				if(_dpi != default)
				{
					SetImage(value?.GetImage(Size * _dpi.X / 96));
				}
			}
		}

		protected abstract void SetImage(Image? image);

		public void UpdateImage(Dpi dpi)
		{
			_dpi = dpi;
			SetImage(Image?.GetImage(Size * dpi.X / 96));
		}

		public void RemoveImage()
			=> SetImage(default);
	}

	class ToolStripItemController(ToolStripItem item, int size)
		: ImageControllerBase<ToolStripItem>(item, size)
	{
		protected override void SetImage(Image? image)
		{
			Target.ImageScaling = ToolStripItemImageScaling.None;
			Target.Image = image;
		}
	}

	class PictureBoxController(PictureBox control, int size)
		: ImageControllerBase<PictureBox>(control, size)
	{
		protected override void SetImage(Image? image)
			=> Target.Image = image;
	}

	class ImageWidgetController(IImageWidget widget, int size)
		: ImageControllerBase<IImageWidget>(widget, size)
	{
		protected override void SetImage(Image? image)
			=> Target.Image = image;
	}

	class LinkButtonController(LinkButton linkButton, int size)
		: ImageControllerBase<LinkButton>(linkButton, size)
	{
		protected override void SetImage(Image? image)
			=> Target.Image = image;
	}

	private readonly Dictionary<object, IImageController> _bindings = [];
	private Control? _control;

	public DpiBindings()
	{
	}

	public DpiBindings(Control control)
	{
		Verify.Argument.IsNotNull(control);

		Control = control;
		if(!Control.IsDisposed)
		{
			Control.Disposed               += OnControlDisposed;
			Control.DpiChangedBeforeParent += OnControlDpiChangedBeforeParent;
		}
	}

	private void OnControlDpiChangedBeforeParent(object? sender, EventArgs e)
	{
		if(sender != Control || Control is null) return;
		var dpi = Dpi.FromControl(Control);
		foreach(var controller in _bindings.Values)
		{
			controller.UpdateImage(dpi);
		}
	}

	private void OnControlDisposed(object? sender, EventArgs e)
	{
		if(sender is not Control control) return;

		control.Disposed               -= OnControlDisposed;
		control.DpiChangedBeforeParent -= OnControlDpiChangedBeforeParent;
	}

	public Control? Control
	{
		get => _control;
		set
		{
			if(_control == value) return;
			if(_control is not null)
			{
				_control.Disposed               -= OnControlDisposed;
				_control.DpiChangedBeforeParent -= OnControlDpiChangedBeforeParent;
			}
			_control = value;
			if(_control is { IsDisposed: false })
			{
				_control.Disposed               += OnControlDisposed;
				_control.DpiChangedBeforeParent += OnControlDpiChangedBeforeParent;

				var dpi = Dpi.FromControl(_control);
				foreach(var controller in _bindings.Values)
				{
					controller.UpdateImage(dpi);
				}
			}
		}
	}

	private static IImageController CreateController(ToolStripItem item, int size)
		=> new ToolStripItemController(item, size);

	private static IImageController CreateController(PictureBox control, int size)
		=> new PictureBoxController(control, size);

	private static IImageController CreateController(IImageWidget widget, int size)
		=> new ImageWidgetController(widget, size);

	private static IImageController CreateController(LinkButton linkButton, int size)
		=> new LinkButtonController(linkButton, size);

	private IImageController BindImageCore<T>(T target, Func<T, int, IImageController> controllerFactory, IImageProvider? image, int size)
		where T : class
	{
		if(!_bindings.TryGetValue(target, out var controller))
		{
			_bindings.Add(target, controller = controllerFactory(target, size));
			controller.Image = image;
			if(Control is not null)
			{
				controller.UpdateImage(Dpi.FromControl(Control));
			}
		}
		else
		{
			controller.Image = image;
		}
		return controller;
	}


	public IImageController BindImage(ToolStripItem item, IImageProvider? image, int size = 16)
	{
		Verify.Argument.IsNotNull(item);

		return BindImageCore(item, CreateController, image, size);
	}

	public IImageController BindImage(PictureBox pictureBox, IImageProvider? image, int size = 16)
	{
		Verify.Argument.IsNotNull(pictureBox);

		return BindImageCore(pictureBox, CreateController, image, size);
	}

	public IImageController BindImage(IImageWidget widget, IImageProvider? image, int size = 16)
	{
		Verify.Argument.IsNotNull(widget);

		return BindImageCore(widget, CreateController, image, size);
	}

	public IImageController BindImage(LinkButton linkbutton, IImageProvider? image, int size = 16)
	{
		Verify.Argument.IsNotNull(linkbutton);

		return BindImageCore(linkbutton, CreateController, image, size);
	}

	private void UnbindCore(object item)
	{
		Assert.IsNotNull(item);

#if NETCOREAPP
		if(_bindings.Remove(item, out var controller))
		{
			controller.RemoveImage();
		}
#else
		if(_bindings.TryGetValue(item, out var controller))
		{
			_bindings.Remove(item);
			controller.RemoveImage();
		}
#endif
	}

	public void UnbindImage(ToolStripItem item)
	{
		Verify.Argument.IsNotNull(item);

		UnbindCore(item);
	}

	public void UnbindAll()
	{
		foreach(var controller in _bindings.Values)
		{
			controller.RemoveImage();
		}
		_bindings.Clear();
	}
}
