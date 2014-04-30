#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Controls
{
	using System;
	using System.Windows.Forms;

	using gitter.Framework;

	using Resources = gitter.Properties.Resources;

	sealed class StandardToolbar : ToolStrip
	{
		#region Data

		private readonly IWorkingEnvironment _environment;
		private readonly ToolStripButton _initRepositoryButton;
		private readonly ToolStripButton _cloneRepositoryButton;

		#endregion

		#region .ctor

		public StandardToolbar(IWorkingEnvironment environment)
		{
			Verify.Argument.IsNotNull(environment, "environment");

			_environment = environment;

			Text = Resources.StrStandard;

			const TextImageRelation tir = TextImageRelation.ImageAboveText;
			const ToolStripItemDisplayStyle ds = ToolStripItemDisplayStyle.ImageAndText;

			Items.AddRange(new ToolStripItem[]
				{
					_initRepositoryButton = new ToolStripButton(Resources.StrInit, CachedResources.Bitmaps["ImgRepositoryInitSmall"], OnInitRepositoryClick)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipInit },
					_cloneRepositoryButton = new ToolStripButton(Resources.StrClone, CachedResources.Bitmaps["ImgRepositoryCloneSmall"], OnCloneRepositoryClick)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipClone },
				});
		}

		#endregion

		#region Methods

		private void OnInitRepositoryClick(object sender, EventArgs e)
		{
			using(var dlg = new InitRepositoryDialog(_environment))
			{
				dlg.Run(_environment.MainForm);
			}
		}

		private void OnCloneRepositoryClick(object sender, EventArgs e)
		{
			using(var dlg = new CloneRepositoryDialog(_environment))
			{
				dlg.Run(_environment.MainForm);
			}
		}

		#endregion
	}
}
