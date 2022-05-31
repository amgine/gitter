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

namespace gitter.GitLab.Gui;

using System;
using System.Globalization;

using gitter.Framework.Controls;

sealed class TestReportSearch : ListBoxTreeSearch<TestReportSearchOptions>
{
	public TestReportSearch(CustomListBox listBox)
		: base(listBox)
	{
	}

	private static bool TestTestClass(string className, TestReportSearchOptions search)
	{
		Assert.IsNotNull(className);
		Assert.IsNotNull(search);

		if(TestString(className, search)) return true;
		return false;
	}

	private static bool TestTestSuite(Api.TestSuite testSuite, TestReportSearchOptions search)
	{
		Assert.IsNotNull(testSuite);
		Assert.IsNotNull(search);

		if(TestString(testSuite.Name, search)) return true;
		return false;
	}

	private static bool TestTestCase(Api.TestCase testCase, TestReportSearchOptions search)
	{
		Assert.IsNotNull(testCase);
		Assert.IsNotNull(search);

		if(TestString(testCase.Name,      search)) return true;
		if(TestString(testCase.ClassName, search)) return true;
		return false;
	}

	protected override bool TestItem(CustomListBoxItem item, TestReportSearchOptions search)
	{
		Assert.IsNotNull(item);
		Assert.IsNotNull(search);

		return item switch
		{
			TestCaseListBoxItem      testCase  => TestTestCase (testCase.DataContext,  search),
			TestSuiteListBoxItem     testSuite => TestTestSuite(testSuite.DataContext, search),
			TestCaseClassListBoxItem @class    => TestTestClass(@class.DataContext,    search),
			_ => false,
		};
	}
}
