﻿#region Copyright Notice
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

namespace gitter.Git.Gui.Views;

using System;

/// <summary>Git views guids.</summary>
public static partial class Guids
{
	public static readonly Guid CommitViewGuid         = new("DEB816E6-8BEA-44CA-8C3C-EC4AC990D91F");
	public static readonly Guid ConfigViewGuid         = new("DAA95671-73D9-41DC-925C-7A5E633C725C");
	public static readonly Guid GitViewGuid            = new("B3215F6C-C3E4-47C4-98D8-D4CB48A5AB70");
	public static readonly Guid HistoryViewGuid        = new("44ABB5D8-6802-46A3-BDDD-3516926B46C4");
	public static readonly Guid PathHistoryViewGuid    = new("C94719C0-9126-470A-BC7F-2110B56B07A6");
	public static readonly Guid ReflogViewGuid         = new("FEF6E622-186F-46E2-B070-B191FD051525");
	public static readonly Guid MaintenanceViewGuid    = new("262A65A7-E1EE-409B-8A24-F53D03035AD4");
	public static readonly Guid ReferencesViewGuid     = new("72B7B606-B451-44EA-A710-FFF66187B819");
	public static readonly Guid RemotesViewGuid        = new("2722F5DB-F734-4C70-8E05-1431BCB6990A");
	public static readonly Guid RemoteViewGuid         = new("F2915D43-D834-4BB7-9664-1E58FC54CB1D");
	public static readonly Guid StashViewGuid          = new("80333656-E4BB-4FF9-8E01-982AC29ABAF5");
	public static readonly Guid SubmodulesViewGuid     = new("4549BF50-2686-4F78-813E-2B9263A0F0AF");
	public static readonly Guid ContributorsViewGuid   = new("0E9CA704-EA37-4586-A5B3-8CEBE80AB9FB");
	public static readonly Guid TreeViewGuid           = new("6EB20555-BFAF-47FB-8A55-F555F55D629A");
	public static readonly Guid DiffViewGuid           = new("8082F6C4-F4DF-47FB-A63E-CAC97A287678");
	public static readonly Guid BlameViewGuid          = new("88285E06-09FB-472B-81FD-193E86B22BD2");
	public static readonly Guid ContextualDiffViewGuid = new("396C68B1-C4DA-421B-8F63-F475BF45C589");
}
