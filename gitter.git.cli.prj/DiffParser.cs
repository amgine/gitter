	using System.Globalization;
			int eol = FindLfLineEnding();
			if(String[eol - 1] == '\r')
			{
				--eol;
				ending = LineEnding.CrLf;
			}
			else
			{
				ending = LineEnding.Lf;
			}
						NumberStyles.Integer,
						CultureInfo.InvariantCulture);
						NumberStyles.Integer,
						CultureInfo.InvariantCulture);
					var lineNumberStr = ReadStringUpTo(space, 1);
						lineNumberStr,
						NumberStyles.Integer,
						CultureInfo.InvariantCulture);