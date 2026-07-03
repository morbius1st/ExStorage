using System.Diagnostics;
using System.Globalization;
using UtilityLibrary;
using ExStorSys;

// using static ExStorSys.ExStorConstFaux;


// user name: jeffs
// created:   5/25/2026 11:06:41 PM

namespace ProcessTests3
{
	public abstract class AValidate
	{
		protected const string NA = "NA";

		private const int COL_0 = -8;  // test index
		private const int COL_A = -30; // TITLE
		private const int COL_B = -12; // tst res
		private const int COL_C = -28; // answer
		protected const int COL_D = -16;
		private const int COL_E = -10; // answer

		protected Sheet? Sht { get; set; }
		protected WorkBook? Wbk { get; set; }
		protected ExStorData? XData { get; set; }

		// this is the table to the "generic" tests
		public Dictionary<string, Func<bool, dynamic, string, Tuple<bool, string>>> TestTable2 = new ();

		// this is the complete list of possible test parameters
		// indexed by a test name which xrefs with the test table
		public Dictionary<string, SingleTest2> TestParameters2 = new Dictionary<string, SingleTest2>();

		protected void formatHeader1()
		{
			string colb = "[ exp ans ]";
			string colc = $"{colb, COL_B} ( answer )";

			R.WriteLine($"\n{" idx",COL_0} |{" title",COL_A - 1} {colc,COL_C}|{" who",COL_E} | message  (exp value  versus  actual)");
			R.Write($"{"|------",COL_0} {"|---------------------------",COL_A - 1} ");
			R.WriteLine($"{"|------------------------",COL_C} {"|----------",COL_E} |-------------------------------------");
		}

		protected void formatHeader2()
		{
			string colb = "[ exp ans ]";
			string colc = $"{colb, COL_B} ( answer )";

			R.WriteLine($"\n{" idx",COL_0} |{" title",COL_A - 1} {colc,COL_C}|{" who",COL_E} | message");
			R.Write($"{"|------",COL_0} {"|---------------------------",COL_A - 1} ");
			R.WriteLine($"{"|------------------------",COL_C} {"|----------",COL_E} |-------------------------------------");
		}

		// col_a 20   <-- col_c 22 ------> | no limit
		//             
		// title      [exp bool] (answer) | msg
		private string formatItem(string title, bool boolTest, bool answer, int type, int resultType, string msg = "")
		{
			// string temp = getAnswer(boolTest, resultType);
			string temp = boolTest.ToString();
			temp = $"[ {temp} ]";
			temp = $"{temp,COL_B} ( {getAnswer(answer, resultType)} )";
			temp = $" {title,COL_A} {temp,COL_C}";

			if (type == 0)
			{
				return temp;
			}

			if (type == 1)
			{
				return $"{temp}| {TesterId, COL_E}| {msg}";
			}

			return answer.ToString();
		}

		/// <summary>
		/// convert a bool into a good answer string<br/>
		/// type<br/>
		/// 0 = yes / no<br/>
		/// 1 = does match / does not match<br/>
		/// 2 = match / not match<br/>
		/// 3 = correct / wrong<br/>
		/// 4 = worked / failed<br/>
		/// </summary>
		private string getAnswer(bool result, int type)
		{
			if (type == 0)
			{
				return result ? "Yes" : "No";
			}

			if (type == 1)
			{
				return result ? "DOES Match" : "does NOT match";
			}

			if (type == 2)
			{
				return result ? "Match" : "NOT match";
			}

			if (type == 3)
			{
				return result ? "Correct" : "Wrong";
			}

			if (type == 4)
			{
				return result ? "WORKED" : "FAILED";
			}


			return result.ToString();
		}

		private Tuple<bool, string> validateAndFormatAnswer1<T>(T test,
			bool expBool, dynamic exp, string title,
			string testMeans = "", string expMeans = "", bool invert = false)
			where T : notnull
		{
			if (test == null) throw new ArgumentNullException(nameof(test));

			string t = (string.IsNullOrWhiteSpace(test?.ToString() ?? "")) ? "* empty *" : test!.ToString()!;
			string e = (string.IsNullOrWhiteSpace(exp?.ToString() ?? "")) ? "* empty *" : exp!.ToString()!;

			string? testDesc = testMeans.IsVoid() ? $"{t}" : $"{test} [ {testMeans} ]";
			string expDesc = expMeans.IsVoid() ? $"{e}" : $"{exp} [ {expMeans} ]";

			bool bAnswer = (test.Equals(exp)) == expBool;
			string temp = $"should match | {getAnswer(expBool, 0),-8} | {expDesc} vs {testDesc}" ;
			string sAnswer = formatItem(title, expBool, bAnswer, 1, 2, temp);

			return new (bAnswer, sAnswer);
		}

		private Tuple<bool, string> validateAndFormatAnswer1(
			bool test, bool expBool, bool exp, string title, bool invert)
		{
			// for bool in: test is act answer, expBool is exp value, exp is answer

			string testDesc = invert ? (!test).ToString() : test.ToString();
			string expDesc = invert ? (!exp).ToString() : exp.ToString();

			bool bAnswer = (test.Equals(exp)) == expBool;
			string temp = $"should match | {getAnswer(expBool, 0),-8} | {expDesc} == {testDesc}" ;
			string sAnswer = formatItem(title, expBool, bAnswer, 1, 2, temp);

			return new (bAnswer, sAnswer);
		}

		private Tuple<bool, string> validateAndFormatUiAnswer(bool test,
			bool expBool, dynamic altSetg, bool intermeadate, bool results, string title, string xtra)
		{
			string ans1 = $"[ {expBool} ]";
			string ans2 = $"{ans1, COL_B} ( {getAnswer(results, 2)} )";

			string intermediate = altSetg == null ? "" : $"and modifier {!((bool) altSetg), -6} produces {intermeadate, -6}";

			string desc = $"value is {test, -6} {intermediate} versus {expBool, -6} | should they match? {getAnswer(results, 0)}";

			if (!xtra.IsVoid())
			{
				string margin = " ".Repeat(-(COL_A + COL_B + COL_C + COL_E));
				desc = $"{desc}\n{margin}  | {xtra}";
			}

			string temp = $" {title, COL_A} {ans2, COL_C}| {TesterId, COL_E}| {desc}";

			return new (results, temp);
		}

		private string naTestResponse(string title)
		{
			return $"{title, -20} [ this test does not apply ]";
		}

		protected void RegisterTest2(string stTitle, string tstId,
			Func<bool, dynamic, string, Tuple<bool, string>> tst,
			List<SingleTestResult2> tsts,
			ref TestChoice2 tc1)
		{
			TestTable2.Add(tstId, tst);

			TestParameters2.Add(tstId, new (stTitle, tsts));

			tc1 = new (tstId, "");
		}

		private string TesterId { get; set; }

		private string getTesterId(string testIdx)
		{
			int pos1 = testIdx.IndexOf(':');

			if (pos1 < 0) return "?unkown";

			string id = testIdx.Substring(0, pos1);

			switch (id)
			{
			case "sht":
				{
					return "sheet";
				}
			case "wbk":
				{
					return "workbook";
				}
			case "xd":
				{
					return "xdata";
				}
			}

			return "?unkown?";
		}

		public void UpdateExpValue(string testId, string testOp, dynamic value)
		{
			SingleTest2 a = TestParameters2[testId];
			SingleTestResult2 b = a.Results[testOp];
			b.ExpectedValue = value;
			a.Results[testOp] = b;
			TestParameters2[testId] = a;
		}

		/* testTable 2
		*		the actual comparison function => needs expected bool, the comparison test value, and the test title
		*		indexed by the "TestId"
		*
		* a test sequence is a list of testChoices which identify which tests to run
		* a TestChoice provides the "TestId" and a "TextIndex" which is which of the possible answers to use
		* a SingleTest is the list (SingleTestResult) of test answers the correct one is accessed using the "TestIndex"
		* a SingleTestResult (STR) is the parameters for the test answer
		*
		* to test, need to invoke against the values from an STR
		* the STR is gotten from SingleTest using the "TextIndex" from TestChoice
		*
		*/
		public bool ValidateTests2(bool stopOnError, TestSequence2 tests, int useHeader = 1)
		{
			// R.WriteLine("\n******************");
			// R.WriteLine("USING VALIDATE 2");
			// R.WriteLine("******************\n");

			if (tests.Choices == null)
			{
				R.WriteLine("\nTESTS IS NOT VALID (choices is null)\n");
				return false;
			}

			bool result = true;

			SingleTest2 testP;
			SingleTestResult2 str;
			string tstIdx;

			Tuple<bool, string> answer = new (true, "");

			Func<bool, dynamic, string, Tuple<bool, string>> f;

			if (useHeader == 1)
			{
				formatHeader1();
			}
			else
			{
				formatHeader2();
			}

			foreach (TestChoice2 choice in tests.Choices)
			{
				tstIdx = choice.TestIndex;
				TesterId = getTesterId(choice.TestId);

				// get the actual text function
				if (TestTable2.TryGetValue(choice.TestId, out f))
				{
					if (TestParameters2.TryGetValue(choice.TestId, out testP))
					{
						if (tstIdx.Equals(NA))
						{
							string temp = naTestResponse(testP.TestTitle);

							R.WriteLine($" {tstIdx,COL_0}|{temp}");
							continue;
						}

						if (!testP.Results.TryGetValue(tstIdx, out str))
						{
							R.WriteLine($"invalid test idx >{tstIdx}< for test {choice.TestId} {testP.TestTitle}");

							result = false;
							continue;
						}

						answer = f.Invoke(str.ExpectedResults, str.ExpectedValue, testP.TestTitle);

						result &= answer.Item1;
						R.WriteLine($" {tstIdx,COL_0}|{answer.Item2}");
					}
					else
					{
						result = false;

						R.WriteLineAnyway($"\n***** FAIL *****\n>> test parameters not found => {choice.TestId} <<");

						if (stopOnError)
						{
							R.WriteLine($"***** EXIT *****\n");
							return result;
						}

						R.WriteLine($"***** CONTINUE *****\n");
						R.NewLineAnyway();
						// continue;
					}
				}
				else
				{
					result = false;

					R.WriteLineAnyway($"\n***** FAIL *****\n>> test not found => {choice.TestId} <<");

					if (stopOnError)
					{
						R.WriteLine($"***** EXIT *****\n");
						return result;
					}

					R.WriteLine($"***** CONTINUE *****\n");
					R.NewLineAnyway();
					// continue;
				}
			}

			return result;
		}

		/* test component base test */

		// the "generic" test routines that does the actual comparisons,
		// returns the results and formats the results
		private Tuple<bool, string> validateString(string actual, bool expBool, dynamic exp, string title)
		{
			if (((string) exp).IsVoid() && actual.IsVoid())
			{
				return validateAndFormatAnswer1(string.Empty, expBool, string.Empty, title);
			}

			return validateAndFormatAnswer1(actual, expBool, exp, title);
		}

		private Tuple<bool, string> validateChgSrc(ChgSrcId test, bool expBool, dynamic exp, string title)
		{
			return validateAndFormatAnswer1(test, expBool, exp, title);
		}

		private Tuple<bool, string> validateIsDirty(bool actual, bool expBool, dynamic exp, string title)
		{
			// bool bAnswer = ((bool) exp) == (actual) == expBool;
			// string temp = $"{(bool) exp} vs {actual} should match | {getAnswer(expBool, 0)}" ;
			// string sAnswer = formatItem(title, expBool, bAnswer, 1, 2, temp);

			// return new (bAnswer, sAnswer);

			return validateAndFormatAnswer1(actual, expBool, exp, title);
		}

		private Tuple<bool, string> validateBool(bool test, bool expBool, string title, bool invert = false)
		{
			bool bAnswer = expBool == test;

			// in: test is the exp value, expBool is the expected result
			// that is, test should == expBool (as per above test)
			// final result exp answer is T (they match), act value is (test (F)) and exp value is (expbool (F))

			return validateAndFormatAnswer1(test, bAnswer, expBool, title, invert);
		}

		private Tuple<bool, string> validateInt(int test, bool expBool, dynamic exp, string title,
			Dictionary<int, string>? meaning)
		{
			string meansExp = meaning != null ? meaning[exp] : "";
			string meansTst = meaning != null ? meaning[test] : "";

			return validateAndFormatAnswer1(test, expBool, exp, title, meansTst, meansExp);
		}

		private Tuple<bool, string> validateEnum<T>(T test, bool expBool, dynamic exp, string title)
			where T : Enum
		{
			return validateAndFormatAnswer1(test, expBool, exp, title);
		}


		//                            that is v- does this == v- this  and does that equal this -v
		//                                    test field     expected   secondary
		//                                    value          answer     test
		private Tuple<bool, string> verifyUi(bool test, bool expBool, dynamic? altSetg, string title, string desc = "")
		{
			// bool bAnswer = (expBool == test) == (bool) altSetg;

			bool intermeadate = altSetg == null ? test : test &= ((bool) altSetg);

			bool bAnswer = ( intermeadate ) == expBool;

			return validateAndFormatUiAnswer(test, expBool, altSetg, intermeadate, bAnswer, title, desc);
		}


	#region test components - bools and buttons - wbk

		/* wbk test component */

		protected const string WBK_TST_IS_MOD_EXO_B = "wbk:ismodexo_bool";
		protected Tuple<bool, string> validate_WbkIsModExoB(bool expBool, dynamic exp, string title)
		{
			return validateBool(Wbk.IsModifiedExo, expBool, title);
		}

		protected const string WBK_TST_UNDO_BTN_B = "wbk:undobtn_bool";
		protected Tuple<bool, string> validate_WbkUndoBtnB(bool expBool, dynamic exp, string title)
		{
			return validateBool(Wbk.UndoBtnStatus, expBool, title);
		}

		protected const string WBK_TST_APPLY_BTN_B = "wbk:applybtn_bool";
		protected Tuple<bool, string> validate_WbkApplyBtnB(bool expBool, dynamic exp, string title)
		{
			return validateBool(Wbk.ApplyBtnStatus, expBool, title);
		}

		protected const string XD_TST_UNDO_BTN_SHTS_LST_B = "xd:undobtnshtslst_bool";
		protected Tuple<bool, string> validate_XdUndoBtnShtsLstB(bool expBool, dynamic exp, string title)
		{
			return validateBool(XData.UndoBtnShtsLstStatus, expBool, title);
		}

		protected const string XD_TST_APPLY_BTN_SHTS_LST_B = "xd:applybtnshtslst_bool";
		protected Tuple<bool, string> validate_XdApplyBtnShtsLstB(bool expBool, dynamic exp, string title)
		{
			return validateBool(XData.ApplyBtnShtsLstStatus, expBool, title);
		}

	#endregion

	#region test components - wbk

		/* desc */
		protected const string WBK_TST_DESC_STR = "wbk:desc_str";
		protected Tuple<bool, string> validate_WbkDescStr(bool expBool, dynamic exp, string title)
		{
			//                  actual value exp result  exp value
			//  that is	 does ( actual value == exp value ) == exp result
			return validateString(Wbk.Desc, expBool, exp, title);
		}

		protected const string WBK_TST_DESC_IS_DIRTY = "wbk:desc_isdirty";
		protected Tuple<bool, string> validate_WbkDescIsDirty(bool expBool, dynamic exp, string title)
		{
			return validateIsDirty(Wbk.DescField.IsDirty(), expBool, exp, title);
		}

		protected const string WBK_TST_DESC_CS = "wbk:desc_cs";
		protected Tuple<bool, string> validate_WbkDescChgSrc(bool expBool, dynamic exp, string title)
		{
			return validateChgSrc(Wbk.DescField.ChgSrc, expBool, exp, title);
		}


		/* name mod */
		protected const string WBK_TST_NAME_MOD_STR = "wbk:namemod_str";
		protected Tuple<bool, string> validate_WbkNameModStr(bool expBool, dynamic exp, string title)
		{
			return validateString(Wbk.NameModified, expBool, exp, title);
		}

		protected const string WBK_TST_NAME_MOD_IS_DIRTY = "wbk:namemod_isdirty";
		protected Tuple<bool, string> validate_WbkNameModIsDirty(bool expBool, dynamic exp, string title)
		{
			return validateIsDirty(Wbk.NameModifiedField.IsDirty(), expBool, exp, title);
		}

		protected const string WBK_TST_NAME_MOD_CS = "wbk:namemod_cs";
		protected Tuple<bool, string> validate_WbkNameModChgSrc(bool expBool, dynamic exp, string title)
		{
			return validateChgSrc(Wbk.NameModifiedField.ChgSrc, expBool, exp, title);
		}


		/* date mod */
		protected const string WBK_TST_DATE_MOD_STR = "wbk:datemod_str";
		protected Tuple<bool, string> validate_WbkDateModStr(bool expBool, dynamic exp, string title)
		{
			return validateString(Wbk.DateModified, expBool, exp, title);
		}

		protected const string WBK_TST_DATE_MOD_IS_DIRTY = "wbk:datemod_isdirty";
		protected Tuple<bool, string> validate_WbkDateModIsDirty(bool expBool, dynamic exp, string title)
		{
			return validateIsDirty(Wbk.DateModifiedField.IsDirty(), expBool, exp, title);
		}

		protected const string WBK_TST_DATE_MOD_CS = "wbk:datemod_cs";
		protected Tuple<bool, string> validate_WbkDateModChgSrc(bool expBool, dynamic exp, string title)
		{
			return validateChgSrc(Wbk.DateModifiedField.ChgSrc, expBool, exp, title);
		}


		/* last id */
		protected const string WBK_TST_LAST_ID_STR = "wbk:lastid_str";
		protected Tuple<bool, string> validate_WbkLastIdStr(bool expBool, dynamic exp, string title)
		{
			return validateString(Wbk.LastId, expBool, exp, title);
		}

		protected const string WBK_TST_LAST_ID_PRIOR_STR = "wbk:lastid_priorstr";
		protected Tuple<bool, string> validate_WbkLastIdPriorStr(bool expBool, dynamic exp, string title)
		{
			return validateString(Wbk.LastIdField.DyValue.PriorValue, expBool, exp, title);
		}


		protected const string WBK_TST_LAST_ID_IS_DIRTY = "wbk:lastid_isdirty";
		protected Tuple<bool, string> validate_WbkLastIdIsDirty(bool expBool, dynamic exp, string title)
		{
			return validateIsDirty(Wbk.LastIdField.IsDirty(), expBool, exp, title);
		}

		protected const string WBK_TST_LAST_ID_CS = "wbk:lastid_cs";
		protected Tuple<bool, string> validate_WbkLastIdChgSrc(bool expBool, dynamic exp, string title)
		{
			return validateChgSrc(Wbk.LastIdField.ChgSrc, expBool, exp, title);
		}

	#endregion

	#region test components - sheets list

		/* shts lst */
		protected const string WBK_TST_SHTS_LST_INT = "wbk:shtslst_int";
		protected Tuple<bool, string> validate_WbkShtsLstInt(bool expBool, dynamic exp, string title)
		{
			return validateInt(Wbk.SheetsList, expBool, exp, title, shtsListMeaning);
		}

		private Dictionary<int, string> shtsListMeaning = new ()
			{ { 1, "got changes (1+ new, etc)" }, { -1, "not got changes (1+ new_del, etc)" }, { 0, "all existing" }, };

		protected const string WBK_TST_SHTS_LST_IS_DIRTY = "wbk:shtslst_isdirty";
		protected Tuple<bool, string> validate_WbkShtsLstIsDirty(bool expBool, dynamic exp, string title)
		{
			return validateIsDirty(Wbk.ShtsListField.IsDirty(), expBool, exp, title);
		}

		protected const string WBK_TST_SHTS_LST_CS = "wbk:shtslst_cs";
		protected Tuple<bool, string> validate_WbkShtsLstChgSrc(bool expBool, dynamic exp, string title)
		{
			return validateChgSrc(Wbk.ShtsListField.ChgSrc, expBool, exp, title);
		}

	#endregion

		/* sht test component */

	#region test components - bools, buttons, other - sht

		protected const string SHT_TST_IS_MOD_EXO_B = "sht:ismodexo_bool";
		protected Tuple<bool, string> validate_ShtIsModExoB(bool expBool, dynamic exp, string title)
		{
			return validateBool(Sht.IsModifiedExo, expBool, title);
		}

		protected const string SHT_TST_UNDO_BTN_B = "sht:undobtn_bool";
		protected Tuple<bool, string> validate_ShtUndoBtnB(bool expBool, dynamic exp, string title)
		{
			return validateBool(Sht.UndoBtnStatus, expBool, title);
		}

		protected const string SHT_TST_APPLY_BTN_B = "sht:applybtn_bool";
		protected Tuple<bool, string> validate_ShtApplyBtnB(bool expBool, dynamic exp, string title)
		{
			return validateBool(Sht.ApplyBtnStatus, expBool, title);
		}

		protected const string SHT_TST_SHT_STAT_ENUM = "sht:shtstat_enum";
		protected Tuple<bool, string> validate_ShtStatEnum(bool expBool, dynamic exp, string title)
		{
			return validateEnum(Sht.SheetStatus, expBool, exp, title);
		}


		/* sht desc */
		protected const string SHT_TST_DESC_STR = "sht:desc_str";
		protected Tuple<bool, string> validate_ShtDescStr(bool expBool, dynamic exp, string title)
		{
			return validateString(Sht.Desc, expBool, exp, title);
		}

		protected const string SHT_TST_DESC_IS_DIRTY = "sht:desc_isdirty";
		protected Tuple<bool, string> validate_ShtDescIsDirty(bool expBool, dynamic exp, string title)
		{
			return validateIsDirty(Sht.DescField.IsDirty(), expBool, exp, title);
		}

		protected const string SHT_TST_DESC_CS = "sht:desc_cs";
		protected Tuple<bool, string> validate_ShtDescChgSrc(bool expBool, dynamic exp, string title)
		{
			return validateChgSrc(Sht.DescField.ChgSrc, expBool, exp, title);
		}


		/* sht opseq */
		protected const string SHT_TST_OP_SEQ_STR = "sht:opseq_str";
		protected Tuple<bool, string> validate_ShtOpSeqStr(bool expBool, dynamic exp, string title)
		{
			return validateString(Sht.OpSequence, expBool, exp, title);
		}

		protected const string SHT_TST_OP_SEQ_IS_DIRTY = "sht:opseq_isdirty";
		protected Tuple<bool, string> validate_ShtOpSeqIsDirty(bool expBool, dynamic exp, string title)
		{
			return validateIsDirty(Sht.OpSequenceField.IsDirty(), expBool, exp, title);
		}

		protected const string SHT_TST_OP_SEQ_CS = "sht:opseq_cs";
		protected Tuple<bool, string> validate_ShtOpSeqChgSrc(bool expBool, dynamic exp, string title)
		{
			return validateChgSrc(Sht.OpSequenceField.ChgSrc, expBool, exp, title);
		}


		/* sht ur */
		protected const string SHT_TST_UPD_RULE_ENUM = "sht:ur_str";
		protected Tuple<bool, string> validate_ShtUpdRuleEnum(bool expBool, dynamic exp, string title)
		{
			return validateEnum(Sht.UpdateRule, expBool, exp, title);
		}

		protected const string SHT_TST_UPD_RULE_IS_DIRTY = "sht:ur_isdirty";
		protected Tuple<bool, string> validate_ShtUpdRuleIsDirty(bool expBool, dynamic exp, string title)
		{
			return validateIsDirty(Sht.UpdateRuleField.IsDirty(), expBool, exp, title);
		}

		protected const string SHT_TST_UPD_RULE_CS = "sht:ur_cs";
		protected Tuple<bool, string> validate_ShtUpdRuleChgSrc(bool expBool, dynamic exp, string title)
		{
			return validateChgSrc(Sht.UpdateRuleField.ChgSrc, expBool, exp, title);
		}


		/* sht name mod */
		protected const string SHT_TST_NAME_MOD_STR = "sht:namemod_str";
		protected Tuple<bool, string> validate_ShtNameModStr(bool expBool, dynamic exp, string title)
		{
			return validateString(Sht.NameModified, expBool, exp, title);
		}

		protected const string SHT_TST_NAME_MOD_IS_DIRTY = "sht:namemod_isdirty";
		protected Tuple<bool, string> validate_ShtNameModIsDirty(bool expBool, dynamic exp, string title)
		{
			return validateIsDirty(Sht.NameModifiedField.IsDirty(), expBool, exp, title);
		}

		protected const string SHT_TST_NAME_MOD_CS = "sht:namemod_cs";
		protected Tuple<bool, string> validate_ShtNameModChgSrc(bool expBool, dynamic exp, string title)
		{
			return validateChgSrc(Sht.NameModifiedField.ChgSrc, expBool, exp, title);
		}


		/* sht date mod */
		protected const string SHT_TST_DATE_MOD_STR = "sht:datemod_str";
		protected Tuple<bool, string> validate_ShtDateModStr(bool expBool, dynamic exp, string title)
		{
			return validateString(Sht.DateModified, expBool, exp, title);
		}

		protected const string SHT_TST_DATE_MOD_IS_DIRTY = "sht:datemod_isdirty";
		protected Tuple<bool, string> validate_ShtDateModIsDirty(bool expBool, dynamic exp, string title)
		{
			return validateIsDirty(Sht.DateModifiedField.IsDirty(), expBool, exp, title);
		}

		protected const string SHT_TST_DATE_MOD_CS = "sht:datemod_cs";
		protected Tuple<bool, string> validate_ShtDateModChgSrc(bool expBool, dynamic exp, string title)
		{
			return validateChgSrc(Sht.DateModifiedField.ChgSrc, expBool, exp, title);
		}

	#endregion

	#region fam and type tests - sht

		protected const string SHT_TST_FAT_COUNT_INT = "sht:fat_count_int";
		protected Tuple<bool, string> validate_ShtFatCountInt(bool expBool, dynamic exp, string title)
		{
			//                    actual value  exp result  exp value
			//  that is    does ( actual value == exp value ) == exp result
			return validateInt(Sht.FamilyListCnt, expBool, exp, title, null);
		}

		protected const string SHT_TST_FAT_COUNT_WKG_INT = "sht:fat_count_wkg_int";
		protected Tuple<bool, string> validate_ShtFatCountWkgInt(bool expBool, dynamic exp, string title)
		{
			//                    actual value  exp result  exp value
			//  that is    does ( actual value == exp value ) == exp result
			return validateInt(Sht.FamilyListWkgCnt, expBool, exp, title, null);
		}

		protected const string SHT_TST_FAT_IS_DIRTY = "sht:fat_isdirty";
		protected Tuple<bool, string> validate_ShtFatIsDirty(bool expBool, dynamic exp, string title)
		{
			return validateIsDirty(Sht.FamilyListField.IsDirty(), expBool, exp, title);
		}

		protected const string SHT_TST_FAT_CS = "sht:fat_Cs";
		protected Tuple<bool, string> validate_ShtFatChgSrc(bool expBool, dynamic exp, string title)
		{
			return validateChgSrc(Sht.FamilyListField.ChgSrc, expBool, exp, title);
		}

		protected const string SHT_TST_FAT_WKG_HAS_NEW_B = "sht:fat_wkg_has_new_b";
		protected Tuple<bool, string> validate_ShtFatWkgHasNewBool(bool expBool, dynamic exp, string title)
		{
			return validateBool(Sht.IsModifiedFamListWkg, expBool, title);
		}

		protected const string SHT_TST_FAT_HAS_KEY_STR = "sht:fat_has_key_str";
		protected Tuple<bool, string> validate_ShtFatHasKeyStr(bool expBool, dynamic exp, string title)
		{
			bool b = Sht.FamLstHasKey((string) exp);

			// exp is the expected key (exp value)
			// above test converts this to a bool (exp value) b
			// passed is b, exp value, expBool the expected result
			// that is, b should == expBool

			return validateBool(b, expBool, title);
		}

		protected const string SHT_TST_FAT_HAS_KEY_WKG_STR = "sht:fat_has_key_wkg_str";
		protected Tuple<bool, string> validate_ShtFatHasKeyWkgStr(bool expBool, dynamic exp, string title)
		{
			bool b = Sht.FamLstWkgHasKey((string) exp);

			return validateBool(b, expBool, title);
		}

	#endregion

	#region ui tests - wbk

		/* ui tests */

		/* wbk */

		protected const string UI_WBK_LASTID_CAN_UNDO = "wbk:ui_lastid_can_undo";
		protected Tuple<bool, string> verify_ui_wbk_lastid_can_undo(bool expBool, dynamic? notUsed, string title)
		{
			bool altSetg = !XData.ApplyBtnShtsLstStatus;

			return verifyUi(Wbk.LastIdField.IsDirty(), expBool, altSetg, title);
		}

		// can always edit except when altSetg is true
		protected const string UI_WBK_CAN_EDIT = "wbk:ui_can_edit";
		protected Tuple<bool, string> verify_ui_wbk_can_edit(bool expBool, dynamic? notUsed, string title)
		{
			bool altSetg = !XData.ApplyBtnShtsLstStatus;

			return verifyUi(true, expBool, altSetg, title);
		}

		// altSetg does not apply to this
		protected const string UI_WBK_CHANGED = "wbk:ui_changed";
		protected Tuple<bool, string> verify_ui_wbk_changed(bool expBool, dynamic altSetg, string title)
		{
			return verifyUi(Wbk.IsModifiedExo, expBool, altSetg, title);
		}

		// altsetg does not apply to this (it has already been accounted for)
		protected const string UI_WBK_APPLY_BTN_ENABLED = "wbk:ui_apply_btn_enabled";
		protected Tuple<bool, string> verify_ui_wbk_apply_btn_enabled(bool expBool, dynamic altSetg, string title)
		{
			return verifyUi(Wbk.ApplyBtnStatus, expBool, altSetg, title);
		}

		// altsetg does not apply to this (it has already been accounted for)
		protected const string UI_WBK_UNDO_BTN_ENABLED = "wbk:ui_undo_btn_enabled";
		protected Tuple<bool, string> verify_ui_wbk_undo_btn_enabled(bool expBool, dynamic altSetg, string title)
		{
			return verifyUi(Wbk.UndoBtnStatus, expBool, altSetg, title);
		}

		protected const string UI_WBK_DESC_CAN_UNDO = "wbk:ui_desc_can_undo";
		protected Tuple<bool, string> verify_ui_wbk_desc_can_undo(bool expBool, dynamic? notUsed, string title)
		{
			bool altSetg = !XData.ApplyBtnShtsLstStatus;

			return verifyUi(Wbk.DescField.IsDirty(), expBool, altSetg, title);
		}

		protected const string UI_WBK_NAMEMOD_CAN_UNDO = "wbk:ui_namemod_can_undo";
		protected Tuple<bool, string> verify_ui_wbk_namemod_can_undo(bool expBool, dynamic? notUsed, string title)
		{
			// can undo when - is dirty
			// btn == false => true
			// chgsrc == X => true
			// or when
			// is dirty == T & !modifier is T  (ie. modifier is F)
			// can undo | modifier () => T => !T = F => !F => T usr change {final answer)
			// {btn (F) == F => T && chtsrc (X) == X => T} => T => !T => F => !F => T (correct) [user changed]
			// {btn (T) == F => F && chtsrc (T) == X => F) => F => !F => T => !T => F (correct) [sheet added]

			bool altSetg = !XData.ApplyBtnShtsLstStatus;
			altSetg &= Wbk.NameModifiedField.ChgSrc == ChgSrcId.CI_SRC_X;
			// altSetg = !altSetg;

			string desc = "";
			// desc = $"not btn is {!XData.ApplyBtnShtsLstStatus} | ";
			// desc = $"{desc} chgsrc == X is {Wbk.NameModifiedField.ChgSrc == ChgSrcId.CI_SRC_X} ( {Wbk.NameModifiedField.ChgSrc} ) | ";
			// desc = $"{desc} result altsetg is {altSetg} | so modifier is {altSetg}";

			return verifyUi(Wbk.NameModifiedField.IsDirty(), expBool, altSetg, title, desc);
		}

	#endregion

	#region ui tests - xd

		/* xd */

		// altsetg does not apply to this (it has already been accounted for)
		protected const string UI_XD_APPLY_BTN_SHTSLST_ENABLED = "xd:ui_apply_btn_shtslst_enabled";
		protected Tuple<bool, string> verify_ui_xd_apply_btn_shtslst_enabled(bool expBool, dynamic altSetg, string title)
		{
			return verifyUi(XData.ApplyBtnShtsLstStatus, expBool, altSetg, title);
		}

		// altsetg does not apply to this (it has already been accounted for)
		protected const string UI_XD_UNDO_BTN_SHTSLST_ENABLED = "xd:ui_undo_btn_shtslst_enabled";
		protected Tuple<bool, string> verify_ui_xd_undo_btn_shtslst_enabled(bool expBool, dynamic altSetg, string title)
		{
			return verifyUi(XData.UndoBtnShtsLstStatus, expBool, altSetg, title);
		}

	#endregion

	#region ui tests - sht

		/* sht */

		// can always edit except when altSetg is true
		protected const string UI_SHT_CAN_EDIT = "sht:ui_can_edit";
		protected Tuple<bool, string> verify_ui_sht_can_edit(bool expBool, dynamic? notUsed, string title)
		{
			bool altSetg = !Wbk.IsModifiedExo;

			string desc = $"wbk is mod exo {Wbk.IsModifiedExo} passed is {!Wbk.IsModifiedExo}";

			return verifyUi(true, expBool, altSetg, title, desc);
		}

		// altSetg does not apply to this
		protected const string UI_SHT_CHANGED = "sht:ui_changed";
		protected Tuple<bool, string> verify_ui_sht_changed(bool expBool, dynamic altSetg, string title)
		{
			return verifyUi(Sht.IsModifiedExo, expBool, altSetg, title);
		}

		protected const string UI_SHT_ISMODFAMLSTWKG = "sht:ui_ismodfamlstwkg";
		protected Tuple<bool, string> validate_ui_sht_ismodfamlstwkg(bool expBool, dynamic exp, string title)
		{
			return validateBool(Sht.IsModifiedFamListWkg, expBool, title);
		}

		// altsetg does not apply to this (it has already been accounted for)
		protected const string UI_SHT_APPLY_BTN_ENABLED = "sht:ui_apply_btn_enabled";
		protected Tuple<bool, string> verify_ui_sht_apply_btn_enabled(bool expBool, dynamic altSetg, string title)
		{
			return verifyUi(Sht.ApplyBtnStatus, expBool, altSetg, title);
		}

		// altsetg does not apply to this (it has already been accounted for)
		protected const string UI_SHT_UNDO_BTN_ENABLED = "sht:ui_undo_btn_enabled";
		protected Tuple<bool, string> verify_ui_sht_undo_btn_enabled(bool expBool, dynamic altSetg, string title)
		{
			return verifyUi(Sht.UndoBtnStatus, expBool, altSetg, title);
		}

		protected const string UI_SHT_DESC_CAN_UNDO = "sht:ui_desc_can_undo";
		protected Tuple<bool, string> verify_ui_sht_desc_can_undo(bool expBool, dynamic? notUsed, string title)
		{
			bool altSetg = !Wbk.IsModifiedExo;

			return verifyUi(Sht.DescField.IsDirty(), expBool, altSetg, title);
		}

		protected const string UI_SHT_OPSEQ_CAN_UNDO = "sht:ui_opseq_can_undo";
		protected Tuple<bool, string> verify_ui_sht_opseq_can_undo(bool expBool, dynamic? notUsed, string title)
		{
			bool altSetg = !Wbk.IsModifiedExo;

			return verifyUi(Sht.OpSequenceField.IsDirty(), expBool, altSetg, title);
		}

		protected const string UI_SHT_UPDRULE_CAN_UNDO = "sht:ui_updrule_can_undo";
		protected Tuple<bool, string> verify_ui_sht_updrule_can_undo(bool expBool, dynamic? notUsed, string title)
		{
			bool altSetg = !Wbk.IsModifiedExo;

			return verifyUi(Sht.UpdateRuleField.IsDirty(), expBool, altSetg, title);
		}

		protected const string UI_SHT_NAMEMOD_CAN_UNDO = "sht:ui_namemod_can_undo";
		protected Tuple<bool, string> verify_ui_sht_namemod_can_undo(bool expBool, dynamic? notUsed, string title)
		{
			// can undo when - is dirty
			// btn == false => true
			// chgsrc == X => true
			// or when
			// is dirty == T & !modifier is T  (ie. modifier is F)
			// can undo | modifier () => T => !T = F => !F => T usr change {final answer)
			// {btn (F) == F => T && chtsrc (X) == X => T} => T => !T => F => !F => T (correct) [user changed]
			// {btn (T) == F => F && chtsrc (T) == X => F) => F => !F => T => !T => F (correct) [sheet added]

			bool altSetg = !Wbk.IsModifiedExo;
			altSetg &= Sht.NameModifiedField.ChgSrc == ChgSrcId.CI_SRC_X;
			// altSetg = !altSetg;

			string desc = "";
			// desc = $"not btn is {!XData.ApplyBtnShtsLstStatus} | ";
			// desc = $"{desc} chgsrc == X is {Sht.NameModifiedField.ChgSrc == ChgSrcId.CI_SRC_X} ( {Sht.NameModifiedField.ChgSrc} ) | ";
			// desc = $"{desc} result altsetg is {altSetg} | so modifier is {altSetg}";

			return verifyUi(Sht.NameModifiedField.IsDirty(), expBool, altSetg, title, desc);
		}

	#endregion
	}
}