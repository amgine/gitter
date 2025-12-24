#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.TeamCity.Options;

using System;
using System.Drawing;
using System.Net.Http;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Options;
using gitter.Framework.Services;

using Resources = gitter.TeamCity.Properties.Resources;

partial class ConfigurationPage : PropertyPage, IExecutableDialog
{
	public static readonly new Guid Guid = new("A57F6604-4D4A-4A92-940B-B520034651EA");

	private readonly DialogControls _controls;
	private readonly NotifyCollection<ServerInfo> _servers;
	private bool _changed;
	private IDisposable _serverListBinding;

	sealed class ItemContextMenu : ContextMenuStrip
	{
		private static readonly object SetApiKeyRequestedEvent = new();

		public event EventHandler SetApiKeyRequested
		{
			add    => Events.AddHandler    (SetApiKeyRequestedEvent, value);
			remove => Events.RemoveHandler (SetApiKeyRequestedEvent, value);
		}

		private void OnSetApiKeyRequested(EventArgs e)
			=> ((EventHandler?)Events[SetApiKeyRequestedEvent])?.Invoke(this, e);

		public ItemContextMenu(Item item)
		{
			Renderer = GitterApplication.Style.ToolStripRenderer;

			Items.Add(new ToolStripMenuItem("Refresh Access Token Info", null, (_, _) => item.RefreshTokenInfo()));
			Items.Add(new ToolStripMenuItem("Set Access Token...", null, (_, e) => OnSetApiKeyRequested(e)));
			//Items.Add(new ToolStripMenuItem("Rotate Access Token", null, (_, _) =>
			//{
			//	var expiresAt = DateTime.Now.AddDays(360);
			//	item.SelfRotate(expiresAt);
			//})
			//{
			//	Enabled = item.CanSelfRotate,
			//});
		}
	}

	sealed class Item(ConfigurationPage page, HttpMessageInvoker httpMessageInvoker, ServerInfo serverInfo)
		: CustomListBoxItem<ServerInfo>(serverInfo)
	{
		private readonly Api.ApiEndpoint _ep = new(httpMessageInvoker, serverInfo);
		private TokenUpdateState _tokenState;
		private Api.AccessToken? _token;

		enum TokenUpdateState
		{
			InProcess,
			Failed,
			Received,
		}

		private static readonly StringFormat TextStringFormat = new(StringFormat.GenericTypographic)
		{
			FormatFlags   = StringFormatFlags.LineLimit | StringFormatFlags.NoClip,
			Trimming      = StringTrimming.EllipsisCharacter,
			LineAlignment = StringAlignment.Center,
		};

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			Assert.IsNotNull(paintEventArgs);

			if(paintEventArgs.Column.Id != 0) return;

			var conv     = paintEventArgs.DpiConverter;
			var iconSize = conv.Convert(new Size(32, 32));

			var graphics = paintEventArgs.Graphics;
			var bounds   = paintEventArgs.Bounds;

			var d = (bounds.Height - iconSize.Height) / 2;
			var iconBounds = new Rectangle(bounds.X + conv.ConvertX(8), bounds.Y + d, iconSize.Height, iconSize.Width);

			var icon = CachedResources.ScaledBitmaps[@"teamcity", iconSize.Width];
			if(icon is not null)
			{
				graphics.DrawImage(icon, iconBounds);
			}

			var dy = conv.ConvertY(3);
			bounds.Y      += dy;
			bounds.Height -= dy * 2;

			var x = bounds.X + iconBounds.Right + conv.ConvertX(4);
			var h = bounds.Height / 3;
			var textBounds1 = new Rectangle(x, bounds.Y, bounds.Width - iconBounds.Right - conv.ConvertX(4) * 2, h);
			var textBounds2 = new Rectangle(x, bounds.Y + textBounds1.Height, textBounds1.Width, h);
			var textBounds3 = new Rectangle(x, textBounds2.Bottom, textBounds1.Width, bounds.Height - h*2);

			var c1 = paintEventArgs.ListBox.Style.Colors.WindowText;
			var c2 = (paintEventArgs.State & ItemState.Selected) == ItemState.Selected
				? c1
				: paintEventArgs.ListBox.Style.Colors.GrayText;

			GitterApplication.TextRenderer.DrawText(graphics, DataContext.Name.ToString(),       paintEventArgs.Font, c1, textBounds1, TextStringFormat);
			GitterApplication.TextRenderer.DrawText(graphics, DataContext.ServiceUri.ToString(), paintEventArgs.Font, c2, textBounds2, TextStringFormat);
			if(_token is not null)
			{
				if(_token.ExpiresAt.HasValue)
				{
					var expiresIn = Utility.FormatDate(_token.ExpiresAt.Value, DateFormat.Relative);
					var days = (int)(_token.ExpiresAt.Value - DateTimeOffset.UtcNow).TotalDays;
					var dt = days switch
					{
						< 0 => $"token expired",
						  0 => $"token expires today",
						  1 => $"token expires in 1 day",
						> 1 => $"token expires in {days} days",
					};
					GitterApplication.TextRenderer.DrawText(graphics, dt, paintEventArgs.Font, c2, textBounds3, TextStringFormat);
				}
				else
				{
					GitterApplication.TextRenderer.DrawText(graphics, "token does not expire", paintEventArgs.Font, c2, textBounds3, TextStringFormat);
				}
			}
			else
			{
				var m = _tokenState switch
				{
					TokenUpdateState.InProcess => "updating access token info...",
					TokenUpdateState.Failed => "failed to get access token info",
					TokenUpdateState.Received => "could not select current token info",
					_ => default,
				};
				if(m is not null)
				{
					GitterApplication.TextRenderer.DrawText(graphics, m, paintEventArgs.Font, c2, textBounds3, TextStringFormat);
				}
			}
		}

		private async void UpdateTokenInfo()
		{
			_tokenState = TokenUpdateState.InProcess;
			try
			{
				var tokens = await _ep.GetAccessTokensAsync();
				if(tokens.Length == 1)
				{
					_token = tokens[0];
				}
				else
				{
					_token = Array.Find(tokens, t => string.Equals(t.Name, "gitter", StringComparison.OrdinalIgnoreCase));
				}
				_tokenState = TokenUpdateState.Received;
			}
			catch
			{
				_token = default;
				_tokenState = TokenUpdateState.Failed;
			}
			Invalidate();
		}

		public void RefreshTokenInfo()
		{
			UpdateTokenInfo();
		}

		protected override void OnListBoxAttached(CustomListBox listBox)
		{
			base.OnListBoxAttached(listBox);
			UpdateTokenInfo();
		}

		protected override void OnListBoxDetached(CustomListBox listBox)
		{
			base.OnListBoxDetached(listBox);
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new ItemContextMenu(this);
			menu.SetApiKeyRequested += (_, _) =>
			{
				page.SetApiKey(DataContext);
			};
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
	}

	sealed class Column : CustomListBoxColumn
	{
		public Column() : base(0, "", visible: true) => SizeMode = ColumnSizeMode.Fill;
	}

	readonly struct DialogControls
	{
		public readonly CustomListBox _lstServers;
		public readonly LabelControl _lblServers;
		public readonly IButtonWidget _btnAdd;
		public readonly IButtonWidget _btnRemove;

		public DialogControls(IGitterStyle? style = default)
		{
			style ??= GitterApplication.Style;

			_lstServers = new();
			_lblServers = new();
			_btnAdd     = style.ButtonFactory.Create();
			_btnRemove  = style.ButtonFactory.Create();
			_btnRemove.Enabled = false;
		}

		public void Localize()
		{
			_lblServers.Text = Resources.StrServers.AddColon();
			_btnAdd.Text     = Resources.StrAdd.AddEllipsis();
			_btnRemove.Text  = Resources.StrRemove;
		}

		public void Layout(Control parent)
		{
			var noMargin = DpiBoundValue.Constant(Padding.Empty);
			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					rows:
					[
						LayoutConstants.LabelRowHeight,
						LayoutConstants.LabelRowSpacing,
						SizeSpec.Everything(),
						SizeSpec.Absolute(4),
						SizeSpec.Absolute(23),
					],
					content:
					[
						new GridContent(new ControlContent(_lblServers, marginOverride: noMargin), row: 0),
						new GridContent(new ControlContent(_lstServers, marginOverride: noMargin), row: 2),
						new GridContent(new Grid(
							columns:
							[
								SizeSpec.Everything(),
								SizeSpec.Absolute(75),
								SizeSpec.Absolute(6),
								SizeSpec.Absolute(75),
							],
							content:
							[
								new GridContent(new WidgetContent(_btnAdd,    marginOverride: noMargin), column: 1),
								new GridContent(new WidgetContent(_btnRemove, marginOverride: noMargin), column: 3),
							]), row: 4),
					]),
			};

			var tabIndex = 0;
			_lblServers.TabIndex = tabIndex++;
			_lstServers.TabIndex = tabIndex++;
			_btnAdd.TabIndex     = tabIndex++;
			_btnRemove.TabIndex  = tabIndex++;

			_lblServers.Parent = parent;
			_lstServers.Parent = parent;
			_btnAdd.Parent     = parent;
			_btnRemove.Parent  = parent;
		}
	}

	public ConfigurationPage(IWorkingEnvironment workingEnvironment, TeamCityServiceProvider serviceProvider, HttpMessageInvoker httpMessageInvoker)
		: base(Guid)
	{
		Verify.Argument.IsNotNull(workingEnvironment);
		Verify.Argument.IsNotNull(serviceProvider);
		Verify.Argument.IsNotNull(httpMessageInvoker);

		Name = nameof(ConfigurationPage);

		WorkingEnvironment = workingEnvironment;
		ServiceProvider    = serviceProvider;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode = AutoScaleMode.Dpi;
		Size = new(617, 542);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		_controls._btnAdd.Click += OnAddServerClick;
		_controls._btnRemove.Click += OnRemoveServerClick;

		_controls._lstServers.BeginUpdate();
		_controls._lstServers.HeaderStyle = HeaderStyle.Hidden;
		_controls._lstServers.Columns.Add(new Column());
		_controls._lstServers.BaseItemHeight = 16*3 + 4;
		_controls._lstServers.EndUpdate();

		_servers = [.. serviceProvider.Servers];
		_serverListBinding = new NotifyCollectionBinding<ServerInfo>(
			_controls._lstServers.Items, _servers, serverInfo => new Item(this, httpMessageInvoker, serverInfo));

		_controls._lstServers.SelectionChanged += (_, _) => _controls._btnRemove.Enabled = _controls._lstServers.SelectedItems.Count > 0;

		_controls._lstServers.KeyDown += (_, e) =>
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

	private TeamCityServiceProvider ServiceProvider { get; }

	private void RemoveSelectedServers()
	{
		if(_controls._lstServers.SelectedItems.Count == 1)
		{
			_servers.Remove(((CustomListBoxItem<ServerInfo>)_controls._lstServers.SelectedItems[0]).DataContext);
			_changed = true;
		}
	}

	private void OnAddServerClick(object? sender, EventArgs e)
	{
		using var dialog = new AddServerDialog(ServiceProvider.HttpMessageInvoker, _servers);
		if(dialog.Run(this) == DialogResult.OK)
		{
			_changed = true;
		}
	}

	private void SetApiKey(ServerInfo server)
	{
		using var dialog = new SetApiKeyDialog(ServiceProvider.HttpMessageInvoker, _servers, server);
		if(dialog.Run(this) == DialogResult.OK)
		{
			_changed = true;
		}
	}

	private void OnRemoveServerClick(object? sender, EventArgs e)
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
		}
		base.Dispose(disposing);
	}
}
