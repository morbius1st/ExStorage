using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExStorSys;
using UtilityLibrary;
using static ExStorSys.ExStorConstFaux;

// user name: jeffs
// created:   5/18/2026 4:49:31 PM

namespace ProcessTests1
{

	public class Validate
	{
		private const int COL_A = -20; // TITLE
		private const int COL_B = -12; // tst res
		private const int COL_C = -28; // answer
		private const int COL_D = -16;

		private Sheet sht { get; set; }
		private WorkBook wbk { get; set; }
		private ExStorData xData { get; set; }

		public Validate(WorkBook wbk, Sheet sht, ExStorData xd)
		{
			this.wbk = wbk;
			this.sht = sht;
			xData = xd;

			init();
		}

		private void init()
		{
			Register(WBK_ISMODEXO_T, validate_WbkIsModExo, WBK_ISMODEXO_B_TITLE);
			Register(WBK_ISMODEXO_F, validate_WbkIsModExo, WBK_ISMODEXO_B_TITLE);
			
			Register(WBK_APPLYBTN_T, validate_WbkApplyBtn, WBK_APPLYBTN_B_TITLE);
			Register(WBK_APPLYBTN_F, validate_WbkApplyBtn, WBK_APPLYBTN_B_TITLE);
			
			Register(WBK_UNDOBTN_T, validate_WbkUndoBtn, WBK_UNDOBTN_B_TITLE);
			Register(WBK_UNDOBTN_F, validate_WbkUndoBtn, WBK_UNDOBTN_B_TITLE);
			
			

			Register(WBK_DESC_INIT_STR, validate_WbkDesc, WBK_DESC_STR_TITLE);
			Register(WBK_DESC_ALT1_STR, validate_WbkDesc, WBK_DESC_STR_TITLE);
			
			Register(WBK_DESC_ISDIRTY_T, validate_WbkDescIsDirty, WBK_DESC_ISDIRTY_TITLE);
			Register(WBK_DESC_ISDIRTY_F, validate_WbkDescIsDirty, WBK_DESC_ISDIRTY_TITLE);


			Register(WBK_NAMEMOD_INIT_STR, validate_WbkNameMod, WBK_NAMEMOD_STR_TITLE);
			Register(WBK_NAMEMOD_ALT_STR, validate_WbkNameMod, WBK_NAMEMOD_STR_TITLE);
			Register(WBK_NAMEMOD_ALT2_STR, validate_WbkNameMod, WBK_NAMEMOD_STR_TITLE);

			Register(WBK_NAMEMOD_ISDIRTY_T, validate_WbkNameModIsDirty, WBK_NAMEMOD_ISDIRTY_TITLE);
			Register(WBK_NAMEMOD_ISDIRTY_F, validate_WbkNameModIsDirty, WBK_NAMEMOD_ISDIRTY_TITLE);


			Register(WBK_DATEMOD_INIT_STR, validate_WbkDateMod, WBK_DATEMOD_STR_TITLE);
			Register(WBK_DATEMOD_UPD1_STR, validate_WbkDateMod, WBK_DATEMOD_STR_TITLE);

			// Register(WBK_NAMEMOD_CS_A, validate_WbkNameModChgSrc, WBK_NAMEMOD_CS_TITLE);
			Register(WBK_NAMEMOD_CS_N, validate_WbkNameModChgSrc, WBK_NAMEMOD_CS_TITLE);
			// Register(WBK_NAMEMOD_CS_E, validate_WbkNameModChgSrc, WBK_NAMEMOD_CS_TITLE);
			Register(WBK_NAMEMOD_CS_T, validate_WbkNameModChgSrc, WBK_NAMEMOD_CS_TITLE);
			Register(WBK_NAMEMOD_CS_X, validate_WbkNameModChgSrc, WBK_NAMEMOD_CS_TITLE);

			// Register(WBK_DATEMOD_CS_A, validate_WbkDateModChgSrc, WBK_DATEMOD_CS_TITLE);
			Register(WBK_DATEMOD_CS_N, validate_WbkDateModChgSrc, WBK_DATEMOD_CS_TITLE);
			// Register(WBK_DATEMOD_CS_E, validate_WbkDateModChgSrc, WBK_DATEMOD_CS_TITLE);
			Register(WBK_DATEMOD_CS_T, validate_WbkDateModChgSrc, WBK_DATEMOD_CS_TITLE);
			Register(WBK_DATEMOD_CS_X, validate_WbkDateModChgSrc, WBK_DATEMOD_CS_TITLE);

			Register(WBK_DESC_CS_A, validate_WbkDescChgSrc, WBK_DESC_CS_TITLE);
			Register(WBK_DESC_CS_N, validate_WbkDescChgSrc, WBK_DESC_CS_TITLE);


			Register(WBK_LASTID_INIT_STR, validate_WbkLastId, WBK_LASTID_STR_TITLE);
			Register(WBK_LASTID_UPD_E_STR, validate_WbkLastId, WBK_LASTID_STR_TITLE);
			Register(WBK_LASTID_UPD_F_STR, validate_WbkLastId, WBK_LASTID_STR_TITLE);
			Register(WBK_LASTID_UPD_G_STR, validate_WbkLastId, WBK_LASTID_STR_TITLE);


			Register(WBK_LASTID_ISDIRTY_T, validate_WbkLastIdIsDirty, WBK_LASTID_ISDIRTY_TITLE);
			Register(WBK_LASTID_ISDIRTY_F, validate_WbkLastIdIsDirty, WBK_LASTID_ISDIRTY_TITLE);


			Register(WBK_LASTID_CS_N, validate_WbkLastIdChgSrc, WBK_LASTID_CS_TITLE);
			Register(WBK_LASTID_CS_A, validate_WbkLastIdChgSrc, WBK_LASTID_CS_TITLE);
			Register(WBK_LASTID_CS_E, validate_WbkLastIdChgSrc, WBK_LASTID_CS_TITLE);

			// Register(WBK_SHTSLST_ISDIRTY_T, validate_WbkShtsLstIsDirty, WBK_SHTSLST_ISDIRTY_TITLE);
			Register(WBK_SHTSLST_ISDIRTY_F, validate_WbkShtsLstIsDirty, WBK_SHTSLST_ISDIRTY_TITLE);


			Register(WBK_SHTSLST_CS_N, validate_WbkShtsLstChgSrc, WBK_SHTSLST_CS_TITLE);
			Register(WBK_SHTSLST_CS_A, validate_WbkShtsLstChgSrc, WBK_SHTSLST_CS_TITLE);
			Register(WBK_SHTSLST_CS_E, validate_WbkShtsLstChgSrc, WBK_SHTSLST_CS_TITLE);

			Register(WBK_SHTSLST_NEGONE, validate_WbkShtsLstInt, WBK_SHTSLST_VAL_TITLE);
			Register(WBK_SHTSLST_ZERO, validate_WbkShtsLstInt, WBK_SHTSLST_VAL_TITLE);
			Register(WBK_SHTSLST_POSONE, validate_WbkShtsLstInt, WBK_SHTSLST_VAL_TITLE);


			Register(XD_APPLYBTNSHTSLST_T, validate_XdApplyBtnShtsLst, XD_APPLYBTNSHTSLST_B_TITLE);
			Register(XD_APPLYBTNSHTSLST_F, validate_XdApplyBtnShtsLst, XD_APPLYBTNSHTSLST_B_TITLE);

			Register(XD_UNDOBTNSHTSLST_T, validate_XdUndoBtnShtsLst, XD_UNDOBTNSHTSLST_B_TITLE);
			Register(XD_UNDOBTNSHTSLST_F, validate_XdUndoBtnShtsLst, XD_UNDOBTNSHTSLST_B_TITLE);

			assignTestSeq();
		}

		// input, bool is the expected bool, string is the expected string
		// one, or the other, or both may be used
		private Dictionary<string, Tuple<Func<bool, dynamic, string, Tuple<bool, string>>, string>> testTable = new ();

		public void Register(string name, Func<bool, dynamic, string, Tuple<bool, string>> f, string title)
		{
			testTable.Add(name, new (f, title));
		}

		private void formatHeader()
		{
			string colb = "[ exp ans ]";
			string colc = $"{colb, COL_B} ( answer )";

			R.WriteLine($"\n{" title",COL_A - 1} {colc,COL_C}| message  (expected  versus  actual");
			R.WriteLine($"{"|----------------",COL_A} {"|------------------------",COL_C} |-------------------------------------");
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
				return $"{temp}| {msg}";
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

		//                            test key, exp b, exp s, title
		public bool ValidateTests(bool stopOnError, Tuple<string, List<Tuple<string, bool, dynamic>>> tests)
		{
			bool result = true;
			Tuple<bool, string> answer = new (true, "");

			Tuple<Func<bool, dynamic, string, Tuple<bool, string>>, string> f;

			R.WriteLine($"validate using test sequence | ** {tests.Item1} **");

			formatHeader();

			foreach (Tuple<string, bool, dynamic> test in tests.Item2)
			{
				if (testTable.TryGetValue(test.Item1, out f))
				{
					answer = f.Item1.Invoke(test.Item2, test.Item3, f.Item2);
				}
				else
				{
					result = false;

					R.WriteLineAnyway($"\n***** FAIL *****\n>> test not found => {test} <<");
					if (stopOnError)
					{
						R.WriteLine($"***** EXIT *****\n");
						return false;
					}

					R.WriteLine($"***** CONTINUE *****\n");
					R.NewLineAnyway();
					continue;
				}

				result &= answer.Item1;

				R.WriteLine(answer.Item2);
			}

			return result;
		}


		/* never mind

		public bool ValidateTest3(bool stopOnError, TestSequence tests)
		{
			bool result = false;

			Tuple<bool, string> answer = new (true, "");

			Func<bool, dynamic, string, Tuple<bool, string>> f;

			foreach (TestChoice choice in tests.Choices)
			{
				int idx = choice.TestIndex;

				if (testTable2.TryGetValue(choice.TestId, out f))
				{
					SingleTest testP;

					if (TestParameters.TryGetValue(choice.TestId, out testP))
					{
						SingleTestResult str = testP.Results[idx];

						if (choice.TestIndex > testP.Count)
						{
							R.WriteLine("invalid test idx");
							continue;
						}

						answer = f.Invoke(str.ExpectedResults, str.ExpectedValue, testP.TestTitle);

						result &= answer.Item1;

						R.WriteLine(answer.Item2);
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
					continue;
				}
			}

			return result;
		}
		*/

		private Tuple<bool, string> validateString(string actual, bool expBool, dynamic exp, string title)
		{
			bool bAnswer = ((string) exp).Equals(actual) == expBool;
			string temp = $"{(string) exp} vs {actual} should match | {getAnswer(expBool, 0)}" ;
			string sAnswer = formatItem(title, expBool, bAnswer, 1, 2, temp);

			return new (bAnswer, sAnswer);
		}

		private Tuple<bool, string> validateChgSrc(ChgSrcId test, bool expBool, dynamic exp, string title)
		{
			bool bAnswer = (((ChgSrcId) exp) == test) == expBool;
			string temp = $"{exp} vs {test} should match | {getAnswer(expBool, 0)}" ;
			string sAnswer = formatItem(title, expBool, bAnswer, 1, 2, temp);

			return new (bAnswer, sAnswer);
		}

		//                                          act value      exp result      exp value
		private Tuple<bool, string> validateIsDirty(bool actual, bool expBool, dynamic exp, string title)
		{
			bool bAnswer = ((bool) exp) == (actual) == expBool;
			string temp = $"{(bool) exp} vs {actual} should match | {getAnswer(expBool, 0)}" ;
			string sAnswer = formatItem(title, expBool, bAnswer, 1, 2, temp);

			return new (bAnswer, sAnswer);
		}


		private Tuple<bool, string> validateBool(bool test, bool expBool, string title)
		{
			bool bAnswer = expBool == test;
			string sAnswer = formatItem(title, expBool, bAnswer, 0, 2);

			return new (bAnswer, sAnswer);
		}

		private Tuple<bool, string> validateInt(int test, bool expBool, dynamic exp, string title, Dictionary<int, string> meaning)
		{
			bool bAnswer = (((int) exp) == test) == expBool;

			string meansExp = meaning[exp];
			string meansTst = meaning[test];

			string temp = $"{exp} [ {meansExp} ] vs {test} [ {meansTst} ] should match | {getAnswer(expBool, 0)}" ;
			string sAnswer = $"{formatItem(title, expBool, bAnswer, 1, 2, temp)}";

			return new (bAnswer, sAnswer);
		}

		/* tests sequences */

		private void assignTestSeq()
		{
			/* never mind

			TestSequence1 = new
				("Test Seq1", [
					WbkDescStrIsInit, new ("wbk:desc_chgsrc", 0),
					new ("wbk:namemod_str", 0), new ("wbk:datemod_str", 0),
					new ("wbk:lastid_str", 0)
				]);


			testTable2.Add("wbk:desc_str", validate_WbkDesc);
			testTable2.Add("wbk:desc_chgsrc", validate_WbkDescChgSrc);

			testTable2.Add("wbk:namemod_str", validate_WbkDescChgSrc);
			testTable2.Add("wbk:datemod_str", validate_WbkDescChgSrc);
			testTable2.Add("wbk:lastid_str", validate_WbkDescChgSrc);
			*/

			WbkNameModChanged_alt2 = new ("Change name modified => alt2",
			[
				WbkIsMod_true, WbkApplyBtn_true, WbkUndoBtn_true, 

				WbkNameModAlt2,
				WbkNameModsDirty_true,
				WbkNameMod_ChgSrc_X,

				WbkDateModUpd1, WbkdateMod_ChgSrc_T,
			]);



			WbkLastIdChanged_g = new ("Change Last Id",
			[
				WbkIsMod_true, WbkApplyBtn_true, WbkUndoBtn_true, 

				WbkLastIdUpd_g,
				WbkLastIdIsDirty_true,
				WbkLastId_ChgSrc_A,

				WbkNameModAlt, WbkNameMod_ChgSrc_T, 
				WbkDateModUpd1, WbkdateMod_ChgSrc_T,
			]);

			WbkLastIdUndo = new ("Undo Change last id (single or all)",
			[
				WbkIsMod_false, WbkApplyBtn_false, WbkUndoBtn_false,
				WbkLastIdInit,
				WbkLastIdIsDirty_false,
				WbkLastId_ChgSrc_N,
				WbkNameModInit, WbkNameMod_ChgSrc_N,
				WbkDateModInit, WbkdateMod_ChgSrc_N,
			]);

			WbkLastIdApply = new ("Apply Change last id (single or all)",
			[
				WbkIsMod_false, WbkApplyBtn_false, WbkUndoBtn_false,
				WbkLastIdUpd_g,
				WbkLastIdIsDirty_false,
				WbkLastId_ChgSrc_N,
				WbkNameModAlt, WbkNameMod_ChgSrc_N,
				WbkDateModUpd1, WbkdateMod_ChgSrc_N,
			]);

			WbkDescChanged_alt1 = new ("Change Description",
			[
				WbkIsMod_true, WbkApplyBtn_true, WbkUndoBtn_true, 
				WbkDescAlt1, WbkDesc_ChgSrc_A,
				WbkDescIsDirty_true,
				WbkNameModAlt, WbkNameMod_ChgSrc_T, 
				WbkDateModUpd1, WbkdateMod_ChgSrc_T,
			]);

			WbkDescChgUndo = new ("Undo Change Description (single or all)",
			[
				WbkIsMod_false, WbkApplyBtn_false, WbkUndoBtn_false, 
				WbkDescInit, WbkDesc_ChgSrc_N,
				WbkDescIsDirty_false,
				WbkNameModInit, WbkNameMod_ChgSrc_N, 
				WbkDateModInit, WbkdateMod_ChgSrc_N,
			]);
			
			WbkDescChgApply = new ("Apply Change Description (single or all)",
			[
				WbkIsMod_false, WbkApplyBtn_false, WbkUndoBtn_false, 
				WbkDescAlt1, WbkDesc_ChgSrc_N,
				WbkDescIsDirty_false,
				WbkNameModAlt, WbkNameMod_ChgSrc_N, 
				WbkDateModUpd1, WbkdateMod_ChgSrc_N,
			]);


			WbkDescChgUndoAfterAddSheet = new ("Undo Change Description after add sheet(s)", new (WbkDescChgUndo.Item2));
			WbkDescChgUndoAfterAddSheet.Item2.AddRange(
			[
				XdApplyBtnShtsLst_true, XdUndoBtnShtsLst_true, WbkShtsLst_posone, WbkShtsLstIsDirty_false,
				WbkShtsLst_ChgSrc_E, WbkLastIdUpd_e, WbkLastId_ChgSrc_E
			]);

			WbkDescChgApplyAfterAddSheet = new ("Apply Change Description after add sheet(s)",
			[
				WbkIsMod_true, WbkApplyBtn_false, WbkUndoBtn_false, 
				XdApplyBtnShtsLst_true, XdUndoBtnShtsLst_true,

				WbkDescAlt1, WbkDesc_ChgSrc_N, WbkDescIsDirty_false,

				WbkNameModAlt, WbkNameMod_ChgSrc_T, 
				WbkDateModUpd1, WbkdateMod_ChgSrc_T,
				WbkShtsLst_posone, WbkShtsLstIsDirty_false, WbkShtsLst_ChgSrc_E, 
				WbkLastIdUpd_e, WbkLastIdIsDirty_true,WbkLastId_ChgSrc_E
			]);

			List<Tuple<string, bool, dynamic>> baseSheetTests1_Undo =
			[
				WbkIsMod_false, WbkApplyBtn_false, WbkUndoBtn_false, 
				XdApplyBtnShtsLst_false, XdUndoBtnShtsLst_true, 
				WbkNameModInit, WbkNameMod_ChgSrc_N, 
				WbkDateModInit, WbkdateMod_ChgSrc_N,
				WbkShtsLst_negone, WbkShtsLstIsDirty_false, WbkShtsLst_ChgSrc_E,
			];

			XdShtsLstAdd1UndoAll = new ("Add one sheet - Undo All", new (baseSheetTests1_Undo));
			XdShtsLstAdd1UndoAll.Item2.AddRange([WbkLastIdUpd_e, WbkLastIdIsDirty_false, WbkLastId_ChgSrc_N]);

			XdShtsLstAdd3UndoAll = new ("Add three sheets - Undo All", new (baseSheetTests1_Undo));
			XdShtsLstAdd3UndoAll.Item2.AddRange([WbkLastIdUpd_g, WbkLastIdIsDirty_false, WbkLastId_ChgSrc_N]);

			List<Tuple<string, bool, dynamic>> baseSheetTests1_Apply =
			[
				WbkIsMod_false, WbkApplyBtn_false, WbkUndoBtn_false, XdApplyBtnShtsLst_false,
				XdUndoBtnShtsLst_false, WbkNameModAlt, WbkNameMod_ChgSrc_N, WbkDateModUpd1, WbkdateMod_ChgSrc_N,
				WbkShtsLst_zero, WbkShtsLstIsDirty_false, WbkShtsLst_ChgSrc_N,
			];

			XdShtsLstAdd1ApplyAll = new ("Add one sheet - apply All", new (baseSheetTests1_Apply));
			XdShtsLstAdd1ApplyAll.Item2.AddRange([WbkLastIdUpd_e, WbkLastIdIsDirty_false, WbkLastId_ChgSrc_N]);

			XdShtsLstAdd3ApplyAll = new ("Add three sheets - apply All", new (baseSheetTests1_Apply));
			XdShtsLstAdd3ApplyAll.Item2.AddRange([WbkLastIdUpd_g, WbkLastIdIsDirty_false, WbkLastId_ChgSrc_N]);


			List<Tuple<string, bool, dynamic>> baseSheetTests1 =
			[
				WbkIsMod_true, WbkApplyBtn_false, WbkUndoBtn_false, 
				WbkNameModAlt, WbkNameMod_ChgSrc_T, 
				WbkDateModUpd1, WbkdateMod_ChgSrc_T, 
				XdApplyBtnShtsLst_true, XdUndoBtnShtsLst_true, 
				WbkShtsLst_posone, WbkShtsLstIsDirty_false, WbkShtsLst_ChgSrc_E,
			];

			XdAddFirstSheet = new ("Add First Sheet", new (baseSheetTests1));
			XdAddFirstSheet.Item2.AddRange([WbkLastIdUpd_e, WbkLastIdIsDirty_true, WbkLastId_ChgSrc_E]);

			XdAddFirstSheetPlusDesc = new ("Add Sheet Plus Change Desc", new (XdAddFirstSheet.Item2));
			XdAddFirstSheetPlusDesc.Item2.AddRange([WbkDescAlt1, WbkDescIsDirty_true, WbkDesc_ChgSrc_A]);

			XdAddSecondSheet = new ("Add Second Sheet", new (baseSheetTests1));
			XdAddSecondSheet.Item2.AddRange([WbkLastIdUpd_f, WbkLastIdIsDirty_true, WbkLastId_ChgSrc_E]);

			XdAddThirdSheet = new ("Add Third Sheet", new (baseSheetTests1));
			XdAddThirdSheet.Item2.AddRange([WbkLastIdUpd_g, WbkLastIdIsDirty_true, WbkLastId_ChgSrc_E]);

			XdAdd3ShtsDelOneAfterLast = new ("Delete one sheet", new (baseSheetTests1));
			XdAdd3ShtsDelOneAfterLast.Item2.AddRange([WbkLastIdUpd_g, WbkLastIdIsDirty_true, WbkLastId_ChgSrc_E]);

			XdAdd3ShtsDelOneAfterFirst = new ("Delete one sheet", new (baseSheetTests1));
			XdAdd3ShtsDelOneAfterFirst.Item2.AddRange([WbkLastIdUpd_e, WbkLastIdIsDirty_true, WbkLastId_ChgSrc_E]);

			XdUnDelete1SheetAfterAdd3 = new ("Add sheet, delete one, add 2 sheets, un-delete sheet",
			[
				WbkIsMod_false, WbkApplyBtn_false, WbkUndoBtn_false,
				XdApplyBtnShtsLst_true, XdUndoBtnShtsLst_true,
				WbkNameModAlt, WbkNameMod_ChgSrc_T,
				WbkDateModUpd1, WbkdateMod_ChgSrc_T,
				WbkShtsLst_posone, WbkShtsLstIsDirty_false, WbkShtsLst_ChgSrc_E,
				WbkLastIdUpd_g, WbkLastIdIsDirty_true, WbkLastId_ChgSrc_E
			]);

			XdUndoAllAfterAddAndDelete = new ("Add sheet, delete one, add 2 sheets, undo all",
			[
				WbkIsMod_false, WbkApplyBtn_false, WbkUndoBtn_false,
				XdApplyBtnShtsLst_false, XdUndoBtnShtsLst_true,
				WbkNameModInit, WbkNameMod_ChgSrc_N,
				WbkDateModInit, WbkdateMod_ChgSrc_N,
				WbkShtsLst_negone, WbkShtsLstIsDirty_false, WbkShtsLst_ChgSrc_E,
				WbkLastIdUpd_g, WbkLastIdIsDirty_false, WbkLastId_ChgSrc_N
			]);

			XdApplyAllAfterAddAndDelete = new ("Add sheet, delete one, add 2 sheets, apply all",
			[
				WbkIsMod_false, WbkApplyBtn_false, WbkUndoBtn_false,
				XdApplyBtnShtsLst_false, XdUndoBtnShtsLst_false,
				WbkNameModAlt, WbkNameMod_ChgSrc_N,
				WbkDateModUpd1, WbkdateMod_ChgSrc_N,
				WbkShtsLst_zero, WbkShtsLstIsDirty_false, WbkShtsLst_ChgSrc_N,
				WbkLastIdUpd_g, WbkLastIdIsDirty_false, WbkLastId_ChgSrc_N
			]);
		}

		public  Tuple<string, List<Tuple<string, bool, dynamic>>> WbkNameModChanged_alt2;


		public  Tuple<string, List<Tuple<string, bool, dynamic>>> WbkLastIdChanged_g;
		public  Tuple<string, List<Tuple<string, bool, dynamic>>> WbkLastIdUndo;
		public  Tuple<string, List<Tuple<string, bool, dynamic>>> WbkLastIdApply;

		public  Tuple<string, List<Tuple<string, bool, dynamic>>> WbkDescChanged_alt1;
		public  Tuple<string, List<Tuple<string, bool, dynamic>>> WbkDescChgUndo;
		public  Tuple<string, List<Tuple<string, bool, dynamic>>> WbkDescChgApply;

		public  Tuple<string, List<Tuple<string, bool, dynamic>>> WbkDescChgUndoAfterAddSheet;
		public  Tuple<string, List<Tuple<string, bool, dynamic>>> WbkDescChgApplyAfterAddSheet;


		public  Tuple<string, List<Tuple<string, bool, dynamic>>> XdAddFirstSheet;
		public  Tuple<string, List<Tuple<string, bool, dynamic>>> XdAddFirstSheetPlusDesc;


		public  Tuple<string, List<Tuple<string, bool, dynamic>>> XdAddSecondSheet;
		public  Tuple<string, List<Tuple<string, bool, dynamic>>> XdAddThirdSheet;
		public  Tuple<string, List<Tuple<string, bool, dynamic>>> XdAdd3ShtsDelOneAfterLast;
		public  Tuple<string, List<Tuple<string, bool, dynamic>>> XdAdd3ShtsDelOneAfterFirst;
		public  Tuple<string, List<Tuple<string, bool, dynamic>>> XdShtsLstAdd1UndoAll;
		public  Tuple<string, List<Tuple<string, bool, dynamic>>> XdShtsLstAdd3UndoAll;
		public  Tuple<string, List<Tuple<string, bool, dynamic>>> XdShtsLstAdd1ApplyAll;
		public  Tuple<string, List<Tuple<string, bool, dynamic>>> XdShtsLstAdd3ApplyAll;

		public  Tuple<string, List<Tuple<string, bool, dynamic>>> XdUnDelete1SheetAfterAdd3;
		public  Tuple<string, List<Tuple<string, bool, dynamic>>> XdUndoAllAfterAddAndDelete;
		public  Tuple<string, List<Tuple<string, bool, dynamic>>> XdApplyAllAfterAddAndDelete;


		/*

		public TestSequence TestSequence1;

		public TestChoice WbkDescStrIsInit = new ("wbk:desc_str", 0);
		public TestChoice WbkDescStrIsAlt1 = new ("wbk:desc_str", 1);

		private Dictionary<string, Func<bool, dynamic, string, Tuple<bool, string>>> testTable2 = new ();

		private Dictionary<string, SingleTest> TestParameters = new ()
		{
			{ "wbk:desc_str", WbkDesc_Str },
			{ "wbk:namemod_str", WbkNameMod_Str },
			{ "wbk:datemod_str", WbkDateMod_Str },
			{ "wbk:lastid_str", WbkLastId_Str },

			{ "wbk:desc_chgsrc", WbkDesc_Cs }
		};

		private static SingleTest WbkDesc_Str = new ("desc str ==",
			[ new (true, FAUX_DESCRIPTION_INIT), new (true, FAUX_DESCRIPTION_ALT1)]);

		private static SingleTest WbkNameMod_Str = new ("name mod str ==",
			[ new (true, FAUX_USER_NAME_INIT), new (true, FAUX_USER_NAME_ALT)]);

		private static SingleTest WbkDateMod_Str = new ("date mod str ==",
			[ new (true, FAUX_MOD_DATE_INIT), new (true, FAUX_MOD_DATE_UPD1)]);

		private static SingleTest WbkLastId_Str = new ("last id str ==",
		[
			new (true, FAUX_LAST_ID_INIT), new (true, FAUX_LAST_ID_UPD_E),
			new (true, FAUX_LAST_ID_UPD_F), new (true, FAUX_LAST_ID_UPD_G),
		] );

		private static SingleTest WbkDesc_Cs = new ("desc chg src ==",
			[ new (true, ChgSrcId.CI_SRC_A), new (true, ChgSrcId.CI_NONE)]);
			*/


		/* wbk tests */

		/* string tests */

// wbk desc fld text
		public const string WBK_DESC_STR_TITLE = "desc ==";
		public const string WBK_DESC_INIT_STR = "wbk:desc_init";
		public readonly Tuple<string, bool, dynamic> WbkDescInit = new (WBK_DESC_INIT_STR, true, FAUX_DESCRIPTION_INIT);

		public const string WBK_DESC_ALT1_STR = "wbk:desc_alt1";
		public readonly Tuple<string, bool, dynamic> WbkDescAlt1 = new (WBK_DESC_ALT1_STR, true, FAUX_DESCRIPTION_ALT1);

// wbk change name mod text
		public const string WBK_NAMEMOD_STR_TITLE = "name mod ==";
		public const string WBK_NAMEMOD_INIT_STR = "wbk:namemod_init";
		public readonly Tuple<string, bool, dynamic> WbkNameModInit = new (WBK_NAMEMOD_INIT_STR, true, FAUX_USER_NAME_INIT);

		public const string WBK_NAMEMOD_ALT_STR = "wbk:namemod_alt";
		public readonly Tuple<string, bool, dynamic> WbkNameModAlt = new (WBK_NAMEMOD_ALT_STR, true, FAUX_USER_NAME_ALT);

		public const string WBK_NAMEMOD_ALT2_STR = "wbk:namemod2_alt";
		public readonly Tuple<string, bool, dynamic> WbkNameModAlt2 = new (WBK_NAMEMOD_ALT2_STR, true, FAUX_USER_NAME_ALT2);

		public const string WBK_DATEMOD_STR_TITLE = "date mod ==";
		public const string WBK_DATEMOD_INIT_STR = "wbk:datemod_init";
		public readonly Tuple<string, bool, dynamic> WbkDateModInit = new (WBK_DATEMOD_INIT_STR, true, FAUX_MOD_DATE_INIT);

// wbk change date mod text
		public const string WBK_DATEMOD_UPD1_STR = "wbk:datemod_upd1";
		public readonly Tuple<string, bool, dynamic> WbkDateModUpd1 = new (WBK_DATEMOD_UPD1_STR, true, FAUX_MOD_DATE_UPD1);

		public const string WBK_LASTID_STR_TITLE = "last id ==";
		public const string WBK_LASTID_INIT_STR = "wbk:lastid_init";
		public readonly Tuple<string, bool, dynamic> WbkLastIdInit = new (WBK_LASTID_INIT_STR, true, FAUX_LAST_ID_INIT);

// wbk change last id text
		public const string WBK_LASTID_UPD_E_STR = "wbk:lastid_upd_e";
		public readonly Tuple<string, bool, dynamic> WbkLastIdUpd_e = new (WBK_LASTID_UPD_E_STR, true, FAUX_LAST_ID_UPD_E);

		public const string WBK_LASTID_UPD_F_STR = "wbk:lastid_upd_f";
		public readonly Tuple<string, bool, dynamic> WbkLastIdUpd_f = new (WBK_LASTID_UPD_F_STR, true, FAUX_LAST_ID_UPD_F);

		public const string WBK_LASTID_UPD_G_STR = "wbk:lastid_upd_g";
		public readonly Tuple<string, bool, dynamic> WbkLastIdUpd_g = new (WBK_LASTID_UPD_G_STR, true, FAUX_LAST_ID_UPD_G);


		private Tuple<bool, string> validate_WbkDesc(bool expBool, dynamic exp, string title)
		{
			return validateString(wbk.Desc, expBool, exp, title);
		}

		private Tuple<bool, string> validate_WbkNameMod(bool expBool, dynamic exp, string title)
		{
			return validateString(wbk.NameModified, expBool, exp, title);
		}

		private Tuple<bool, string> validate_WbkDateMod(bool expBool, dynamic exp, string title)
		{
			return validateString(wbk.DateModified, expBool, exp, title);
		}

		private Tuple<bool, string> validate_WbkLastId(bool expBool, dynamic exp, string title)
		{
			return validateString(wbk.LastId, expBool, exp, title);
		}


		/* is dirty tests */

// wbk desc fld is dirty
		public const string WBK_DESC_ISDIRTY_TITLE = "desc is dirty?";
		public const string WBK_DESC_ISDIRTY_T = "wbk:descisdirty_t";
		public readonly Tuple<string, bool, dynamic> WbkDescIsDirty_true = new (WBK_DESC_ISDIRTY_T, true, true);

		public const string WBK_DESC_ISDIRTY_F = "wbk:descisdirty_f";
		public readonly Tuple<string, bool, dynamic> WbkDescIsDirty_false = new (WBK_DESC_ISDIRTY_F, true, false);

// wbk lastid fld is dirty
		public const string WBK_LASTID_ISDIRTY_TITLE = "lastid is dirty?";
		public const string WBK_LASTID_ISDIRTY_T = "wbk:lastidisdirty_t";
		public readonly Tuple<string, bool, dynamic> WbkLastIdIsDirty_true = new (WBK_LASTID_ISDIRTY_T, true, true);

		public const string WBK_LASTID_ISDIRTY_F = "wbk:lastidisdirty_f";
		public readonly Tuple<string, bool, dynamic> WbkLastIdIsDirty_false = new (WBK_LASTID_ISDIRTY_F, true, false);

// wbk name mod fld is dirty
		public const string WBK_NAMEMOD_ISDIRTY_TITLE = "namemod is dirty?";
		public const string WBK_NAMEMOD_ISDIRTY_T = "wbk:namemodisdirty_t";
		public readonly Tuple<string, bool, dynamic> WbkNameModsDirty_true = new (WBK_NAMEMOD_ISDIRTY_T, true, true);

		public const string WBK_NAMEMOD_ISDIRTY_F = "wbk:namemodisdirty_f";
		public readonly Tuple<string, bool, dynamic> WbkNameModIsDirty_false = new (WBK_NAMEMOD_ISDIRTY_F, true, false);

// wbk shts lst fld is dirty
		public const string WBK_SHTSLST_ISDIRTY_TITLE = "shtslst is dirty?";
		// public const string WBK_SHTSLST_ISDIRTY_T = "wbk:shtslstisdirty_t";
		// public readonly Tuple<string, bool, dynamic> WbkShtsLstIsDirty_true = new (WBK_SHTSLST_ISDIRTY_T, true, true);

		public const string WBK_SHTSLST_ISDIRTY_F = "wbk:shtslstisdirty_f";
		public readonly Tuple<string, bool, dynamic> WbkShtsLstIsDirty_false = new (WBK_SHTSLST_ISDIRTY_F, true, false);



		private Tuple<bool, string> validate_WbkDescIsDirty(bool expBool, dynamic exp, string title)
		{
			return validateIsDirty(wbk.DescField.IsDirty(), expBool, exp, title);
		}

		private Tuple<bool, string> validate_WbkNameModIsDirty(bool expBool, dynamic exp, string title)
		{
			return validateIsDirty(wbk.NameModifiedField.IsDirty(), expBool, exp, title);
		}

		private Tuple<bool, string> validate_WbkLastIdIsDirty(bool expBool, dynamic exp, string title)
		{
			return validateIsDirty(wbk.LastIdField.IsDirty(), expBool, exp, title);
		}

		private Tuple<bool, string> validate_WbkShtsLstIsDirty(bool expBool, dynamic exp, string title)
		{
			return validateIsDirty(wbk.ShtsListField.IsDirty(), expBool, exp, title);
		}


		/* bool tests */

		// wbk is mod exo bool
		public const string WBK_ISMODEXO_B_TITLE = "is mod";
		public const string WBK_ISMODEXO_T = "wbk:ismodexo_true";
		public readonly static Tuple<string, bool, dynamic> WbkIsMod_true = new (WBK_ISMODEXO_T, true, null);

		public const string WBK_ISMODEXO_F = "wbk:ismodexo_false";
		public readonly static Tuple<string, bool, dynamic> WbkIsMod_false = new (WBK_ISMODEXO_F, false, null);

// wbk apply btn bool
		public const string WBK_APPLYBTN_B_TITLE = "apply btn";
		public const string WBK_APPLYBTN_T = "wbk:applybtn_true";
		public readonly Tuple<string, bool, dynamic> WbkApplyBtn_true = new (WBK_APPLYBTN_T, true, null);

		public const string WBK_APPLYBTN_F = "wbk:applybtn_false";
		public readonly Tuple<string, bool, dynamic> WbkApplyBtn_false = new (WBK_APPLYBTN_F, false, null);

// wbk undo bth bool
		public const string WBK_UNDOBTN_B_TITLE = "undo btn";
		public const string WBK_UNDOBTN_T = "wbk:undobtn_true";
		public readonly Tuple<string, bool, dynamic> WbkUndoBtn_true = new (WBK_UNDOBTN_T, true, null);

		public const string WBK_UNDOBTN_F = "wbk:undobtn_false";
		public readonly Tuple<string, bool, dynamic> WbkUndoBtn_false = new (WBK_UNDOBTN_F, false, null);


		private Tuple<bool, string> validate_WbkIsModExo(bool expBool, dynamic exp, string title)
		{
			return validateBool(wbk.IsModifiedExo, expBool, title);
		}

		private Tuple<bool, string> validate_WbkApplyBtn(bool expBool, dynamic exp, string title)
		{
			return validateBool(wbk.ApplyBtnStatus, expBool, title);
		}

		private Tuple<bool, string> validate_WbkUndoBtn(bool expBool, dynamic exp, string title)
		{
			return validateBool(wbk.UndoBtnStatus, expBool, title);
		}

		/* int tests */

		private Dictionary<int, string> shtsListMeaning = new () { { 1, "got changes (1+ new, etc)" }, { -1, "not got changes (1+ new_del, etc)" }, { 0, "all existing" }, };

// wbk shts lst value int
		public const string WBK_SHTSLST_VAL_TITLE = "shts lst value";
		public const string WBK_SHTSLST_NEGONE = "wbk:shtslst_negone";
		public readonly static Tuple<string, bool, dynamic> WbkShtsLst_negone = new (WBK_SHTSLST_NEGONE, true, -1);

		public const string WBK_SHTSLST_ZERO = "wbk:shtslst_zero";
		public readonly static Tuple<string, bool, dynamic> WbkShtsLst_zero = new (WBK_SHTSLST_ZERO, true, 0);

		public const string WBK_SHTSLST_POSONE = "wbk:shtslst_posone";
		public readonly static Tuple<string, bool, dynamic> WbkShtsLst_posone = new (WBK_SHTSLST_POSONE, true, +1);

		private Tuple<bool, string> validate_WbkShtsLstInt(bool expBool, dynamic exp, string title)
		{
			return validateInt(wbk.SheetsList, expBool, exp, title, shtsListMeaning);
		}


		/* chgsrc tests */

// wbk desc fld chg src
		public const string WBK_DESC_CS_TITLE = "desc chg src";
		public const string WBK_DESC_CS_A = "wbk:desc_chgsrc_a";
		public readonly Tuple<string, bool, dynamic> WbkDesc_ChgSrc_A = new (WBK_DESC_CS_A, true, ChgSrcId.CI_SRC_A);

		public const string WBK_DESC_CS_N = "wbk:desc_chgsrc_n";
		public readonly Tuple<string, bool, dynamic> WbkDesc_ChgSrc_N = new (WBK_DESC_CS_N, true, ChgSrcId.CI_NONE);

// wbk name mod chg src
		public const string WBK_NAMEMOD_CS_TITLE = "name mod chg src";
		public const string WBK_NAMEMOD_CS_N = "wbk:namemod_chgsrc_n";
		public readonly Tuple<string, bool, dynamic> WbkNameMod_ChgSrc_N = new (WBK_NAMEMOD_CS_N, true, ChgSrcId.CI_NONE);

		// public const string WBK_NAMEMOD_CS_A = "wbk:namemod_chgsrc_a";
		// public readonly Tuple<string, bool, dynamic> WbkNameMod_ChgSrc_A = new (WBK_NAMEMOD_CS_A, true, ChgSrcId.CI_SRC_A);
		//
		// public const string WBK_NAMEMOD_CS_E = "wbk:namemod_chgsrc_e";
		// public readonly Tuple<string, bool, dynamic> WbkNameMod_ChgSrc_E = new (WBK_NAMEMOD_CS_E, true, ChgSrcId.CI_SRC_E);

		public const string WBK_NAMEMOD_CS_T = "wbk:namemod_chgsrc_t";
		public readonly Tuple<string, bool, dynamic> WbkNameMod_ChgSrc_T = new (WBK_NAMEMOD_CS_T, true, ChgSrcId.CI_SRC_T);

		public const string WBK_NAMEMOD_CS_X = "wbk:namemod_chgsrc_x";
		public readonly Tuple<string, bool, dynamic> WbkNameMod_ChgSrc_X = new (WBK_NAMEMOD_CS_X, true, ChgSrcId.CI_SRC_X);

// wbk date mod chg src
		public const string WBK_DATEMOD_CS_TITLE = "date mod chg src";
		public const string WBK_DATEMOD_CS_N = "wbk:datemod_chgsrc_n";
		public readonly Tuple<string, bool, dynamic> WbkdateMod_ChgSrc_N = new (WBK_DATEMOD_CS_N, true, ChgSrcId.CI_NONE);

		// public const string WBK_DATEMOD_CS_A = "wbk:datemod_chgsrc_a";
		// public readonly Tuple<string, bool, dynamic> WbkdateMod_ChgSrc_A = new (WBK_DATEMOD_CS_A, true, ChgSrcId.CI_SRC_A);
		//
		// public const string WBK_DATEMOD_CS_E = "wbk:datemod_chgsrc_e";
		// public readonly Tuple<string, bool, dynamic> WbkdateMod_ChgSrc_E = new (WBK_DATEMOD_CS_E, true, ChgSrcId.CI_SRC_E);

		public const string WBK_DATEMOD_CS_T = "wbk:datemod_chgsrc_t";
		public readonly Tuple<string, bool, dynamic> WbkdateMod_ChgSrc_T = new (WBK_DATEMOD_CS_T, true, ChgSrcId.CI_SRC_T);

		public const string WBK_DATEMOD_CS_X = "wbk:datemod_chgsrc_x";
		public readonly Tuple<string, bool, dynamic> WbkdateMod_ChgSrc_X = new (WBK_DATEMOD_CS_X, true, ChgSrcId.CI_SRC_X);

// wbk last id fld chg src
		public const string WBK_LASTID_CS_TITLE = "last id chg src";
		public const string WBK_LASTID_CS_N = "wbk:lastid_chgsrc_n";
		public readonly Tuple<string, bool, dynamic> WbkLastId_ChgSrc_N = new (WBK_LASTID_CS_N, true, ChgSrcId.CI_NONE);

		public const string WBK_LASTID_CS_A = "wbk:lastid_chgsrc_a";
		public readonly Tuple<string, bool, dynamic> WbkLastId_ChgSrc_A = new (WBK_LASTID_CS_A, true, ChgSrcId.CI_SRC_A);

		public const string WBK_LASTID_CS_E = "wbk:lastid_chgsrc_e";
		public readonly Tuple<string, bool, dynamic> WbkLastId_ChgSrc_E = new (WBK_LASTID_CS_E, true, ChgSrcId.CI_SRC_E);

// wbk shts lst fld chg src
		public const string WBK_SHTSLST_CS_TITLE = "shts lst chg src";
		public const string WBK_SHTSLST_CS_N = "wbk:shtslst_chgsrc_n";
		public readonly Tuple<string, bool, dynamic> WbkShtsLst_ChgSrc_N = new (WBK_SHTSLST_CS_N, true, ChgSrcId.CI_NONE);

		public const string WBK_SHTSLST_CS_A = "wbk:shtslst_chgsrc_a";
		public readonly Tuple<string, bool, dynamic> WbkShtsLst_ChgSrc_A = new (WBK_SHTSLST_CS_A, true, ChgSrcId.CI_SRC_A);

		public const string WBK_SHTSLST_CS_E = "wbk:shtslst_chgsrc_e";
		public readonly Tuple<string, bool, dynamic> WbkShtsLst_ChgSrc_E = new (WBK_SHTSLST_CS_E, true, ChgSrcId.CI_SRC_E);


		private Tuple<bool, string> validate_WbkNameModChgSrc(bool expBool, dynamic exp, string title)
		{
			return validateChgSrc(wbk.NameModifiedField.ChgSrc, expBool, exp, title);
		}

		private Tuple<bool, string> validate_WbkDateModChgSrc(bool expBool, dynamic exp, string title)
		{
			return validateChgSrc(wbk.DateModifiedField.ChgSrc, expBool, exp, title);
		}

		private Tuple<bool, string> validate_WbkDescChgSrc(bool expBool, dynamic exp, string title)
		{
			return validateChgSrc(wbk.DescField.ChgSrc, expBool, exp, title);
		}

		private Tuple<bool, string> validate_WbkLastIdChgSrc(bool expBool, dynamic exp, string title)
		{
			return validateChgSrc(wbk.LastIdField.ChgSrc, expBool, exp, title);
		}

		private Tuple<bool, string> validate_WbkShtsLstChgSrc(bool expBool, dynamic exp, string title)
		{
			return validateChgSrc(wbk.ShtsListField.ChgSrc, expBool, exp, title);
		}


		/* xData tests */

		/* string tests */

		/* bool tests */

// xdata apply shts lst bool
		public const string XD_APPLYBTNSHTSLST_B_TITLE = "apply btn shts lst";
		public const string XD_APPLYBTNSHTSLST_T = "xd:applybthshtslst_true";
		public readonly Tuple<string, bool, dynamic> XdApplyBtnShtsLst_true = new (XD_APPLYBTNSHTSLST_T, true, null);

		public const string XD_APPLYBTNSHTSLST_F = "xd:applybthshtslst_false";
		public readonly Tuple<string, bool, dynamic> XdApplyBtnShtsLst_false = new (XD_APPLYBTNSHTSLST_F, false, null);

// xdata undo shts lst bool
		public const string XD_UNDOBTNSHTSLST_B_TITLE = "undo btn shts lst";
		public const string XD_UNDOBTNSHTSLST_T = "xd:undobthshtslst_true";
		public readonly Tuple<string, bool, dynamic> XdUndoBtnShtsLst_true = new (XD_UNDOBTNSHTSLST_T, true, null);

		public const string XD_UNDOBTNSHTSLST_F = "xd:undobthshtslst_false";
		public readonly Tuple<string, bool, dynamic> XdUndoBtnShtsLst_false = new (XD_UNDOBTNSHTSLST_F, false, null);


		private Tuple<bool, string> validate_XdApplyBtnShtsLst(bool expBool, dynamic exp, string title)
		{
			return validateBool(xData.ApplyBtnShtsLstStatus, expBool, title);
		}

		private Tuple<bool, string> validate_XdUndoBtnShtsLst(bool expBool, dynamic exp, string title)
		{
			return validateBool(xData.UndoBtnShtsLstStatus, expBool, title);
		}


		/* chgsrc tests */
	}
}