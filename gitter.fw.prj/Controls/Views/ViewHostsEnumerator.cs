#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.Controls;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

sealed class ViewHostsEnumerator : IEnumerator<ViewHost>
{
	private readonly Control _root;
	private readonly Stack<Control> _stack;

	public ViewHostsEnumerator(Control root)
	{
		Verify.Argument.IsNotNull(root);

		_root = root;
		_stack = new Stack<Control>();
		_stack.Push(root);
		Current = default!;
	}

	public ViewHost Current { get; private set; }

	object IEnumerator.Current => Current;

	void IDisposable.Dispose() { }

	public bool MoveNext()
	{
		if(_stack.Count == 0) return false;
		do
		{
			switch(_stack.Pop())
			{
				case ViewSplit viewSplit:
					for(int i = 0; i < viewSplit.Count; ++i)
					{
						_stack.Push(viewSplit[i].Content);
					}
					break;
				case ViewHost viewHost:
					Current = viewHost;
					return true;
			}
		}
		while(_stack.Count > 0);
		return false;
	}

	public void Reset()
	{
		Current = null!;
		_stack.Clear();
		_stack.Push(_root);
	}

	public ViewHostsEnumerator GetEnumerator() => this;
}
