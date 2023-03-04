#region + Using Directives

#endregion

// user name: jeffs
// created:   10/11/2022 7:31:50 PM

namespace ShEval
{
	public class ShowLibrary2
	{
	#region consts

		private const int IS_HDR = 0;
		private const int IS_ROW = 1;
		private const int IS_DIV = 2;

		private const int TBL_BDR_BEG = 0;
		private const int TBL_BDR_MID = 1;
		private const int TBL_BDR_END = 2;


		private string[][] shtBorder = new []
		{
			//      begin  middle   end
			new [] { " > ", " < > ", " < " }, // header
			new [] { "|  ", "  |  ", "  |" }, // rows
			new [] { "+  ", "- + -", "  +" }  // divider
		};

		private const string STRING_NULL = "<undefined-null>";
		private const string STRING_EMPTY = "<undefined-empty>";

	#endregion

		private ShDebugMessages M { get; }

		public int ColumnWidth { get; set; } = 30;

		public ShowLibrary2(ShDebugMessages msgs)
		{
			M = msgs;
		}

	#region main methods

		public void WriteRows2<TField>(
			List<TField> fieldOrder,
			Dictionary<TField, ColData> colDefinitions,
			List<KEY> keyOrder,
			Dictionary<KEY, Dictionary<TField, string>> rowsInfo,
			int maxLines,
			JustifyVertical jv,
			bool isHeader, bool lastColAlign)
			where TField : Enum
		{
			foreach (KEY key in keyOrder)
			{
				Dictionary<TField, string> row = rowsInfo[key];

				WriteRow2<TField>(fieldOrder,
					colDefinitions,
					row,
					maxLines,
					jv,
					isHeader,
					lastColAlign);
			}
		}

		public void WriteRow2<TE>( List<TE> order,
			Dictionary<TE, ColData> hdrData,
			Dictionary<TE, string> colInfo,
			int maxLines,
			JustifyVertical jv,
			bool isHeader, bool lastColAlign)
		{
			int rowType = isHeader ? IS_HDR : IS_ROW;

			StringBuilder[] sb = initShtRow(maxLines, shtBorder[rowType][TBL_BDR_BEG]);
			bool[] hasRow = new bool[maxLines];

			// JustifyVertical jv = JustifyVertical.MIDDLE;

			TE key;     //= order[0];
			ColData cd; //= hdrData[key];

			int titleWidth; //= isHeader ? cd.TitleWidth : cd.ColWidth;

			// break up the header text into individual lines of header text
			List<string> hdrTxt; //= ColumnifyString(colInfo[key], cd.ColWidth, titleWidth, maxLines, cd.Just[0], jv, true, true);

			int i;

			for (i = 0; i < order.Count - 1; i++)
			{
				key = order[i];
				cd = hdrData[key];
				titleWidth = isHeader ? cd.TitleWidth : cd.ColWidth;
				// break up the header text into individual lines of header text
				hdrTxt = ColumnifyString(colInfo[key], cd.ColWidth, titleWidth, maxLines, cd.Just[rowType], jv, true, true);

				appendInfo2(ref sb, hdrTxt, shtBorder[rowType][TBL_BDR_MID], ref hasRow);
			}

			key = order[i];
			cd = hdrData[key];
			titleWidth = isHeader ? cd.TitleWidth : lastColAlign ? cd.ColWidth : colInfo[key]?.Length ?? cd.ColWidth;
			int colWidth = lastColAlign ? cd.ColWidth : colInfo[key]?.Length ?? cd.ColWidth;
			string colBdr = lastColAlign ? shtBorder[rowType][TBL_BDR_END] : "";

			// break up the header text into individual lines of header text
			hdrTxt = ColumnifyString(colInfo[key], colWidth, titleWidth, maxLines, cd.Just[rowType], jv, true, true);

			appendInfo2(ref sb, hdrTxt, colBdr, ref hasRow);

			for (i =  maxLines - 1; i >= 0; i--)
			{
				if (hasRow[i])
				{
					M.WriteLine(sb[i]?.ToString() ?? "null?");
					// M.NewLine();
				}
			}
		}

	#endregion

	#region support methods

		private void appendInfo2(ref StringBuilder[] sb, List<string> s, string colDiv, ref bool[] hasRow)
		{
			for (int i = 0; i < s.Count; i++)
			{
				hasRow[i] = hasRow[i] || !string.IsNullOrWhiteSpace(s[i]);

				sb[i].Append(s[i]).Append(colDiv);
			}
		}

		private StringBuilder[] initShtRow(int maxLines, string preface)
		{
			StringBuilder[] sb = new StringBuilder[maxLines];

			for (int i = 0; i < maxLines; i++)
			{
				sb[i] = new StringBuilder(preface);
			}

			return sb;
		}

		private string fmtMsg<T1, T2> (    T1 msg1, T2 msg2, string whenNull1, string whenNull2, string divString, int colWidth = -1)
		{
			string A;
			string B;

			if (msg1 is int)
			{
				A = fmtInt(Convert.ToInt32(msg1));
			}
			else
			{
				A = msg1?.ToString();

				if (A == null)
				{
					A = whenNull1 ?? "";
				}
			}

			if (msg2 is int)
			{
				B = fmtInt(Convert.ToInt32(msg2));
			}
			else
			{
				B = msg2?.ToString();

				if (B == null)
				{
					B = whenNull2 ?? "";
				}
			}

			return A.PadRight(colWidth == -1 ? ColumnWidth : colWidth) + divString + B;
		}

		private string fmtInt(int i)
		{
			return $"{i,-4}";
		}


		// format a text string into a column of text
		// colWidth: expected width of each row of text (with exceptions)
		// maxNumber of rows of text 
		// justify: how each row of text is justified
		// doEllipsis: do or do not ellipsisify rows longer than the maximum
		// trim: remove leading and / or trailing blank spaces;
		public static List<string> ColumnifyString(
			string s,
			int colWidth,
			int titleWidth,
			int maxLines,
			JustifyHoriz justifyHoriz,
			JustifyVertical justifyVert,
			bool doEllpisis,
			bool? trim)
		{
			if (s == null)
			{
				s = STRING_NULL;
				titleWidth = s.Length;
			}

			if (string.IsNullOrWhiteSpace(s))
			{
				s = STRING_EMPTY;
				titleWidth = s.Length;
			}


			List<string> result = new List<string>();

			List<string> final = new List<string>();

			result = StringDivide(s, new [] { ' ' }, titleWidth, maxLines);

			int[] lines = calcLines(maxLines, result.Count, justifyVert);

			int i;

			for (i = 0; i < lines[0]; i++)
			{
				final.Add(JustifyString(null, JustifyHoriz.RIGHT, colWidth));
			}

			for (i = result.Count - 1; i >= 0 ; i--)
			{
				final.Add(TejString(result[i], justifyHoriz, colWidth, doEllpisis, trim));
			}

			for (i = 0; i < lines[1]; i++)
			{
				final.Add(JustifyString(null, JustifyHoriz.RIGHT, colWidth));
			}

			return final;
		}

		private static int[] calcLines(int maxLines, int resultLines, JustifyVertical jv)
		{
			int[] lines = new int[2]; // before, middle, after

			lines[0] = 0;
			lines[1] = 0;

			if (maxLines == resultLines) return lines;

			switch (jv)
			{
			case JustifyVertical.BOTTOM:
				{
					break;
				}
			case JustifyVertical.MIDDLE:
				{
					lines[0] = (int) ((maxLines - resultLines) / 2 - 0.1);
					break;
				}
			// covers unspecified and bottom
			default:
				{
					lines[0] = maxLines - resultLines;
					break;
				}
			}

			lines[1] = maxLines - resultLines - lines[0];
			return lines;
		}


		// divide a string into sub-strings of maxLength size and a maximum
		// of maxLines.  Last line has the overflow if any.
		// maxLength > 0 means split on Word boundaries
		// < 0 means split on character boundaries (exact maxLength)
		// when maxLength > 0 the returned line can exceed maxLength
		public static List<string> StringDivide(string text,
			char[] splitanyOf,
			int maxLength,
			int maxLines)
		{
			text = text ?? "";

			bool splitMidWord = false;

			if ( maxLength < 0)
			{
				splitMidWord = true;
				maxLength *= -1;
			}

			List<string> result = new List<string>();

			string final;

			// result.Add("");

			int index = 0;
			int loop = 0;

			while (text.Length > 0)
			{
				int splitIdx;

				if (maxLength + 1 <= text.Length)
				{
					splitIdx = text.Substring(0, maxLength - 1).LastIndexOfAny(splitanyOf) + 1;

					if (!splitMidWord)
					{
						if ((splitIdx == 0 || splitIdx == -1))
						{
							splitIdx = text.IndexOfAny(splitanyOf, maxLength);
						}
					}
				}
				else
				{
					splitIdx = text.Length - index;
				}

				if (splitIdx == -1 || splitIdx == 0)
				{
					splitIdx = maxLength;
				}

				if (loop + 1 == maxLines)
				{
					final = text;
					splitIdx = text.Length;
				}
				else
				{
					final = text.Substring(0, splitIdx);
				}

				result.Add(final);

				if (text.Length > splitIdx)
				{
					text = text.Substring(splitIdx);
				}
				else
				{
					text = string.Empty;
				}

				loop++;
			}

			return result;
		}


		// combines TrimString + Ellipsisify + JustifyString
		// and has them in the correct order
		public static string TejString(string s, JustifyHoriz justifyHoriz, int maxLength,
			bool doEllipsis, bool? trim)
		{
			string result = TrimString(s, justifyHoriz, trim);
			result = doEllipsis ? EllipsisifyString(result, justifyHoriz, maxLength) : result;
			result = JustifyString(result, justifyHoriz, maxLength);

			return result;
		}

		// trim & pad but no ellipsisify
		// trim == null : full trim
		// trim == true : trim per justify
		//		justify == right : trim right side
		//		justify == left : trim left side
		//		justify == center or undefined : trim bith sides
		// trim == false : do nothing
		public static string TrimString(string s, JustifyHoriz justifyHoriz, bool? trim)
		{
			string result = s.IsVoid() ? "" : s;

			if (trim.HasValue)
			{
				if (trim.Value)
				{
					switch (justifyHoriz)
					{
					case JustifyHoriz.RIGHT:
						{
							result = result.TrimEnd();
							break;
						}
					case JustifyHoriz.LEFT:
						{
							result = result.TrimStart();
							break;
						}
					default:
						{
							result = result.Trim();
							break;
						}
					}
				}
			}
			else
			{
				result = result.Trim();
			}

			return result;
		}

		// ellipsisify - do not trim
		public static string EllipsisifyString(string s, JustifyHoriz j, int maxLength)
		{
			string msg = s ?? "";
			int beg = 0;
			int end = 0;
			int len = msg.Length;

			if (maxLength >= len || maxLength < 2) return s;

			//                     L    C    R
			string[] e = new [] { "…", "…", "…" };

			switch (j)
			{
			case JustifyHoriz.RIGHT:
				{
					// beg = 0;
					// ellipsis in left
					end = len - (maxLength - e[2].Length);
					msg = e[2] + s.Substring(end);
					break;
				}
			case JustifyHoriz.LEFT:
				{
					// ellipsis on right
					beg = maxLength - e[0].Length;
					msg = s.Substring(0, beg) + e[0];
					break;
				}
			default:
				{
					// ellipsis in center
					beg = (int) (((maxLength - e[1].Length) / 2) + 0.5);
					end = len - (maxLength - (beg + e[1].Length));

					msg = s.Substring(0, beg) + e[1] + s.Substring(end);
					break;
				}
			}


			return msg;
		}

		// justify the string within the provided colWidth
		public static string JustifyString(string s, JustifyHoriz j, int maxLength)
		{
			if (maxLength == 0) return s;

			char pad = ' ';

			string msg = s.IsVoid() ? "" : s;

			switch (j)
			{
			case JustifyHoriz.RIGHT:
				{
					msg = msg.PadLeft(maxLength, pad);
					break;
				}
			case JustifyHoriz.CENTER:
				{
					msg = msg.PadCenter(maxLength, pad);
					break;
				}
			default:
				{
					msg = msg.PadRight(maxLength, pad);
					break;
				}
			}


			return msg;
		}

	#endregion
	}
}