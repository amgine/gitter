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

namespace gitter.Git
{
	public sealed class DiffOptions
	{
		#region Static

		public static readonly DiffOptions Default = CreateDefault();

		private static DiffOptions CreateDefault()
		{
			var options = new DiffOptions();
			options.Freeze();
			return options;
		}

		#endregion

		#region Data

		private int _context;
		private bool _usePatienceAlgorithm;
		private bool _ignoreWhitespace;
		private bool _binary;
		private bool _isFrozen;

		#endregion

		#region .ctor

		public DiffOptions()
		{
			_context = 3;
		}

		#endregion

		#region Properties

		public int Context
		{
			get { return _context; }
			set
			{
				Verify.State.IsFalse(IsFrozen, "This DiffOptions instance is frozen.");

				_context = value;
			}
		}

		public bool UsePatienceAlgorithm
		{
			get { return _usePatienceAlgorithm; }
			set
			{
				Verify.State.IsFalse(IsFrozen, "This DiffOptions instance is frozen.");

				_usePatienceAlgorithm = value;
			}
		}

		public bool IgnoreWhitespace
		{
			get { return _ignoreWhitespace; }
			set
			{
				Verify.State.IsFalse(IsFrozen, "This DiffOptions instance is frozen.");

				_ignoreWhitespace = value;
			}
		}

		public bool Binary
		{
			get { return _binary; }
			set
			{
				Verify.State.IsFalse(IsFrozen, "This DiffOptions instance is frozen.");

				_binary = value;
			}
		}

		public bool IsFrozen
		{
			get { return _isFrozen; }
			private set { _isFrozen = value; }
		}

		#endregion

		#region Methods

		public void Freeze()
		{
			IsFrozen = true;
		}

		#endregion
	}
}
