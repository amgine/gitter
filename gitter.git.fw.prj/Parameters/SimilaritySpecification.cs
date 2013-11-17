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

namespace gitter.Git.AccessLayer
{
	public struct SimilaritySpecification
	{
		#region Data

		private readonly bool _isSpecified;
		private readonly double _similarity;

		#endregion

		#region .ctor

		public SimilaritySpecification(bool isSpecified)
		{
			_isSpecified = isSpecified;
			_similarity = 0.0;
		}

		public SimilaritySpecification(double similarity)
		{
			Verify.Argument.IsInRange(0.0, similarity, 1.0, "similarity");

			_isSpecified = true;
			_similarity = similarity;
		}

		#endregion

		#region Properties

		public bool IsSpecified
		{
			get { return _isSpecified; }
		}

		public double Similarity
		{
			get { return _similarity; }
		}

		#endregion
	}
}
