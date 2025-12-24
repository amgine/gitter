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

namespace gitter.TeamCity.Gui;

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;

using Resources = gitter.TeamCity.Properties.Resources;

public partial class ProviderSetupDialog : DialogBase, IExecutableDialog
{
	readonly struct DialogControls
	{
		private readonly LabelControl _lblServer;
		public  readonly ServerPicker _serverPicker;
		private readonly LabelControl _lblProject;
		public  readonly TextBox _cmbProject;

		public DialogControls(IGitterStyle style)
		{
			style ??= GitterApplication.Style;

			_lblServer    = new();
			_serverPicker = new();
			_lblProject   = new();
			_cmbProject   = new();

			GitterApplication.FontManager.InputFont.Apply(_cmbProject);
		}

		public void Localize()
		{
			_lblServer.Text  = Resources.StrServer.AddColon();
			_lblProject.Text = Resources.StrProject.AddColon();
		}

		public void Layout(Control parent)
		{
			var projectDec = new TextBoxDecorator(_cmbProject);

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(94),
						SizeSpec.Everything(),
					],
					rows:
					[
						LayoutConstants.TextInputRowHeight,
						LayoutConstants.TextInputRowHeight,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblServer,     marginOverride: LayoutConstants.NoMargin),      row: 0, column: 0),
						new GridContent(new ControlContent(_serverPicker,  marginOverride: LayoutConstants.TextBoxMargin), row: 0, column: 1),
						new GridContent(new ControlContent(_lblProject,    marginOverride: LayoutConstants.NoMargin),      row: 1, column: 0),
						new GridContent(new ControlContent(projectDec,     marginOverride: LayoutConstants.TextBoxMargin), row: 1, column: 1),
					]),
			};

			var tabIndex = 0;
			_lblServer.TabIndex = tabIndex++;
			_serverPicker.TabIndex = tabIndex++;
			_lblProject.TabIndex = tabIndex++;
			projectDec.TabIndex = tabIndex++;

			_lblServer.Parent = parent;
			_serverPicker.Parent = parent;
			_lblProject.Parent = parent;
			projectDec.Parent = parent;
		}
	}

	private readonly DialogControls _controls;
	private IRepository _repository;

	public ProviderSetupDialog(IRepository repository, NotifyCollection<ServerInfo> servers)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNotNull(servers);

		_repository = repository;

		Name = nameof(ProviderSetupDialog);
		Text = Resources.StrTeamCity;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		_controls._serverPicker.LoadData(servers);

		var section = _repository.ConfigSection.TryGetSection("IssueTrackers");
		if(section is not null)
		{
			section = section.TryGetSection("TeamCity");
		}
		var serverName = "";
		if(section is not null)
		{
			serverName = section.GetValue<string>("ServerName", string.Empty);
			_controls._cmbProject.Text = section.GetValue<string>("ProjectId", string.Empty);
		}
		var server =
			string.IsNullOrWhiteSpace(serverName)
				? servers.FirstOrDefault()
				: servers.FirstOrDefault(s => s.Name == serverName);

		_controls._serverPicker.SelectedValue = server;
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(400, 59));

	public ServerInfo? SelectedServer
	{
		get => _controls._serverPicker.SelectedValue;
		set => _controls._serverPicker.SelectedValue = value;
	}

	public string ProjectId
	{
		get => _controls._cmbProject.Text.Trim();
		set => _controls._cmbProject.Text = value;
	}

	private bool ValidateProjectId(string projectId)
	{
		if(string.IsNullOrWhiteSpace(projectId))
		{
			NotificationService.NotifyInputError(
				_controls._cmbProject,
				Resources.ErrNoProjectNameSpecified,
				Resources.ErrProjectNameCannotBeEmpty);
			return false;
		}
		return true;
	}

	public bool Execute()
	{
		var server = SelectedServer;
		var pid    = ProjectId;

		if(server is null) return false;
		if(!ValidateProjectId(pid))  return false;

		var section = _repository
			.ConfigSection
			.GetCreateSection("IssueTrackers")
			.GetCreateSection("TeamCity");
		section.SetValue<string>("ServerName", server.Name);
		section.SetValue<string>("ProjectId", pid);

		return true;
	}
}
