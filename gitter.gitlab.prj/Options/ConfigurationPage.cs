﻿#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2020  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab.Options;

using System;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Options;

using Resources = gitter.GitLab.Properties.Resources;

partial class ConfigurationPage : PropertyPage, IExecutableDialog
{
	public static readonly new Guid Guid = new("FD59B6BD-AFC4-4ABB-AE0C-F170B57B25BE");

	private readonly NotifyCollection<ServerInfo> _servers;
	private bool _changed;
	private IDisposable _serverListBinding;

	sealed class Item : CustomListBoxItem<ServerInfo>
	{
		private static readonly StringFormat TextStringFormat = new(StringFormat.GenericTypographic)
		{
			FormatFlags   = StringFormatFlags.LineLimit | StringFormatFlags.NoClip,
			Trimming      = StringTrimming.EllipsisCharacter,
			LineAlignment = StringAlignment.Center,
		};

		public Item(ServerInfo serverInfo)
			: base(serverInfo)
		{
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			Assert.IsNotNull(paintEventArgs);

			if(paintEventArgs.Column.Id != 0) return;

			var conv     = paintEventArgs.DpiConverter;
			var iconSize = conv.Convert(new Size(32, 32));

			var graphics = paintEventArgs.Graphics;
			var bounds   = paintEventArgs.Bounds;

			var d = (bounds.Height - iconSize.Height) / 2;
			var iconBounds = new Rectangle(bounds.X + d, bounds.Y + d, iconSize.Height, iconSize.Width);

			graphics.DrawImage(CachedResources.ScaledBitmaps[@"gitlab", iconSize.Width], iconBounds);

			var dy = conv.ConvertY(3);
			bounds.Y      += dy;
			bounds.Height -= dy * 2;

			var x = bounds.X + iconBounds.Right + conv.ConvertX(4);
			var textBounds1 = new Rectangle(x, bounds.Y, bounds.Width - iconBounds.Right - conv.ConvertX(4) * 2, bounds.Height / 2);
			var textBounds2 = new Rectangle(x, bounds.Y + textBounds1.Height, textBounds1.Width, bounds.Height - textBounds1.Height);

			GitterApplication.TextRenderer.DrawText(graphics, DataContext.Name.ToString(),       paintEventArgs.Font, SystemBrushes.WindowText, textBounds1, TextStringFormat);
			GitterApplication.TextRenderer.DrawText(graphics, DataContext.ServiceUrl.ToString(), paintEventArgs.Font, SystemBrushes.GrayText,   textBounds2, TextStringFormat);
		}
	}

	sealed class Column : CustomListBoxColumn
	{
		public Column() : base(0, "", visible: true) => SizeMode = ColumnSizeMode.Fill;
	}

	public ConfigurationPage(IWorkingEnvironment workingEnvironment, GitLabServiceProvider serviceProvider)
		: base(Guid)
	{
		Verify.Argument.IsNotNull(workingEnvironment);
		Verify.Argument.IsNotNull(serviceProvider);

		WorkingEnvironment = workingEnvironment;
		ServiceProvider    = serviceProvider;

		InitializeComponent();
		_lstServers.BeginUpdate();
		_lstServers.HeaderStyle = HeaderStyle.Hidden;
		_lstServers.Columns.Add(new Column());
		_lstServers.Style = GitterApplication.DefaultStyle;
		_lstServers.BaseItemHeight = 32 + 4;
		_lstServers.EndUpdate();
		_lblServers.Text = Resources.StrServers.AddColon();
		_btnAdd.Text = Resources.StrAdd.AddEllipsis();
		_btnRemove.Text = Resources.StrRemove;

		_servers = new NotifyCollection<ServerInfo>();
		_servers.AddRange(serviceProvider.Servers);
		_serverListBinding = new NotifyCollectionBinding<ServerInfo>(
			_lstServers.Items, _servers, serverInfo => new Item(serverInfo));

		_lstServers.SelectionChanged += (_, _) => _btnRemove.Enabled = _lstServers.SelectedItems.Count > 0;

		_lstServers.KeyDown += (_, e) =>
		{
			switch(e.KeyCode)
			{
				case Keys.Delete:
					RemoveSelectedServers();
					break;
			}
		};
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(617, 542));

	private IWorkingEnvironment WorkingEnvironment { get; }

	private GitLabServiceProvider ServiceProvider { get; }

	private void RemoveSelectedServers()
	{
		if(_lstServers.SelectedItems.Count == 1)
		{
			_servers.Remove(((CustomListBoxItem<ServerInfo>)_lstServers.SelectedItems[0]).DataContext);
			_changed = true;
		}
	}

	private void OnAddServerClick(object sender, EventArgs e)
	{
		using var dialog = new AddServerDialog(ServiceProvider.HttpMessageInvoker, _servers);
		if(dialog.Run(this) == DialogResult.OK)
		{
			_changed = true;
		}
	}

	private void OnRemoveServerClick(object sender, EventArgs e)
	{
		RemoveSelectedServers();
	}

	public bool Execute()
	{
		if(_changed)
		{
			ServiceProvider.Servers.Clear();
			ServiceProvider.Servers.AddRange(_servers);
			_changed = false;
		}
		return true;
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_serverListBinding.Dispose();
			components?.Dispose();
		}
		base.Dispose(disposing);
	}
}
