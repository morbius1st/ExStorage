using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityLibrary;
using ExStorSys;
using static ExStorSys.ExStorConstFaux;


// user name: jeffs
// created:   5/25/2026 10:51:55 PM

namespace ProcessTests2
{
	public struct SingleTestResult2
	{
		public string Index { get; set; }
		public bool ExpectedResults { get; set; }
		public dynamic? ExpectedValue { get; set; }

		public SingleTestResult2(string index,
			bool expectedResults,
			dynamic? expectedValue)
		{
			Index = index;
			ExpectedResults = expectedResults;
			ExpectedValue = expectedValue;
		}

		public override string ToString()
		{
			return $"idx {Index} | exp result {ExpectedResults} | exp value {ExpectedValue}";
		}
	}

	public struct SingleTest2
	{
		public string TestTitle { get; set; }
		public Dictionary<string, SingleTestResult2> Results { get; set; }
		public int Count { get; set; }

		public SingleTest2(string testTitle, List<SingleTestResult2> results)
		{
			TestTitle = testTitle;
			Count = results.Count;
			Results = new ();

			foreach (SingleTestResult2 str in results)
			{
				Results.Add(str.Index, str);
			}
		}

		public override string ToString()
		{
			return $"title {TestTitle} | count {Count}";
		}
	}

	public struct TestChoice2
	{
		/// <summary>
		/// the test identifier - used as the key to select the test in the list of tests
		/// </summary>
		public string TestId { get; set; }

		/// <summary>
		/// the text index - used at the key to select a test result from the list
		/// of possible results for the identified test
		/// </summary>
		public string TestIndex { get; set; }

		public TestChoice2(string testId, string testIndex)
		{
			TestId = testId;
			TestIndex = testIndex;
		}

		public override string ToString()
		{
			return $"id {TestId} | idx {TestIndex}";
		}
	}

	public struct TestSequence2
	{
		private List<string> testOptions;
		public string TestTitle { get; set; }
		public List<TestChoice2> Choices { get; set; }


		public TestSequence2(string testTitle, List<TestChoice2> choices)
		{
			TestTitle = testTitle;
			Choices = choices;
		}

		public TestSequence2 SetTests(List<string>? testOpts)
		{
			TestSequence2 ts2 = new TestSequence2(TestTitle, new List<TestChoice2>());

			ts2.testOptions = testOpts;

			if (testOpts == null) return ts2;
			if (testOpts.Count != Choices.Count) return new ();

			for (var i = 0; i < Choices.Count; i++)
			{
				TestChoice2 c = Choices[i];
				c.TestIndex = testOpts[i];
				ts2.Choices.Add(c);
			}

			return ts2;
		}

		public override string ToString()
		{
			return $"title {TestTitle} | count {Choices.Count}";
		}

	}

	/*
	public struct TestOptGroup3
	{
		public string TogIndex { get; set; }
		public List<string> TestOpts { get; set; }

		public TestOptGroup3(string togIdx, List<string> testOpts)
		{
			TogIndex = togIdx;
			TestOpts = testOpts;
		}
	}

	public struct TestChoiceGroup3
	{
		public string TgIndex { get; set; }
		public string Title { get; set; }
		public List<TestChoice2> Choices3 { get; set; }

		public TestChoiceGroup3(string tgIdx, string title, List<TestChoice2> choices3)
		{
			TgIndex = tgIdx;
			Title = title;
			Choices3 = choices3;
		}

		public void SetTestChoices(string [] tstOpts)
		{
			if (Choices3 == null || Choices3.Count == 0 ||
				Choices3.Count != tstOpts.Length) return;

			for (int i = 0; i < tstOpts.Length; i++)
			{
				TestChoice2 tc2 = Choices3[i];
				tc2.TestIndex = tstOpts[i];
				Choices3[i] = tc2;
			}
		}
	}

	public struct TestSequence3
	{
		public string TestTitle { get; set; }
		public Dictionary<string, TestChoiceGroup3> Choices { get; set; }
	}
	*/


	// to configure a test
	// in the abstract class (AValidate)
	//		in the base test section, if needed, create a base test
	//			this is the actual test, results, and format result routines
	//			this is by test subject, bool, int, string, enum, etc.
	//		create a test component
	//			this does the prep for each base test
	//			this must match the delegate
	//			also provide a const string as the test id / index / keyu
	//		in this class are various routines: formatting, header, etc, and test collections
	//	in the concrete class (validate2)
	//		create a field for each test sequence (when needed)
	//		create a field for each single test choice 
	//		within configTests2()
	//			configure each single test (via RegisterTest2())
	//				needs the test title, test id, the test component (from abstract class)
	//				contains the list of possible test results
	//			configure each test sequence
	//				a test title and a list which tests to use (test choices)




	public class Validate2 : AValidate
	{
		// private Sheet sht { get; set; }
		// private WorkBook wbk { get; set; }
		// private ExStorData xData { get; set; }

		public Validate2(WorkBook wbk, Sheet sht, ExStorData xd)
		{
			Wbk = wbk;
			Sht = sht;
			XData = xd;

			configTests2();
		}

	#region test sequences

		/* test sequences fields*/

		/// <summary>
		/// Tests<br/>
		/// Desc field - matches: str, isDirty, chgSrc<br/>
		/// name mod - matches: str, isDirty, chgSrc<br/>
		/// date mod - matches: str, isDirty, chgSrc<br/>
		/// buttons: ismodexo, undo, apply
		/// </summary>
		public TestSequence2 Ts2_WbkStdTestsA;

		/// <summary>
		/// Tests<br/>
		/// Desc field - matches: str, isDirty, chgSrc<br/>
		/// LastId field - matches: str, isDirty, chgSrc<br/>
		/// name mod (usr mod) - matches: str, isDirty, chgSrc<br/>
		/// date mod - matches: str, isDirty, chgSrc<br/>
		/// buttons: ismodexo, undo, apply
		/// </summary>
		public TestSequence2 Ts2_WbkStdTestsC;

		/// <summary>
		/// Tests<br/>
		/// shts lst value, shts list isDirty, shts lst chg src<br/>
		/// LastId field - matches: str, isDirty, chgSrc<br/>
		/// name mod (usr mod) - matches: str, isDirty, chgSrc<br/>
		/// date mod - matches: str, isDirty, chgSrc<br/>
		/// buttons: ismodexo, undo, apply
		/// buttons: shts lst apply, shts lst undo
		/// </summary>
		public TestSequence2 Ts2_WbkShtLstTestsA;

		/// <summary>
		/// Tests<br/>
		/// Desc field - matches: str, isDirty, chgSrc<br/>
		/// LastId field - matches: str, isDirty, chgSrc<br/>
		/// name mod (usr mod) - matches: str, isDirty, chgSrc<br/>
		/// date mod - matches: str, isDirty, chgSrc<br/>
		/// shts lst value, shts list isDirty, shts lst chg src<br/>
		/// buttons: ismodexo, undo, apply
		/// buttons: shts lst apply, shts lst undo
		/// </summary>
		public TestSequence2 Ts2_WbkShtLstTestsB;

		/// <summary>
		/// UI Tests<br/>
		/// A) Wbk Desc can undo | B) Wbk LastId can undo | C) Wbk NameMod can undo<br/>
		/// D) Wbk can be edited | E) Wbk is changed<br/>
		/// F) Wbk Apply btn enabled | G) Wbk undo btn enabled<br/>
		/// H) XD Apply btn shts lst enabled | I) Wbk undo btn shts lst enabled
		/// </summary>
		public TestSequence2 Ts2_WbkUiEndSequenceA;


		/* sheet */

		/// <summary>
		/// Tests<br/>
		/// Desc field - matches: str, isDirty, chgSrc<br/>
		/// name mod - matches: str, isDirty, chgSrc<br/>
		/// date mod - matches: str, isDirty, chgSrc<br/>
		/// buttons: ismodexo, undo, apply
		/// </summary>
		public TestSequence2 Ts2_ShtStdTestsA;

		/// <summary>
		/// Tests<br/>
		/// Desc field - matches: str, isDirty, chgSrc<br/>
		/// UpdRule field - matches: enum, isDirty, chgSrc<br/>
		/// OpSeq field - matches: str, isDirty, chgSrc<br/>
		/// name mod - matches: str, isDirty, chgSrc<br/>
		/// date mod - matches: str, isDirty, chgSrc<br/>
		/// buttons: ismodexo, undo, apply
		/// </summary>
		public TestSequence2 Ts2_ShtStdTestsB;

		/// <summary>
		/// Tests - related to fam and type list<br/>
		/// A) Sht fam lst is dirty | B) sht fam lst chg src<br/>
		/// C) Sht fam lst wkg has new | D) sht fam lst has key<br/>
		/// E) sht fam lst wkg has key<br/>
		/// F) sht fam lst count | G) sht fam lst wkg count<br/>
		/// name mod - matches: ) H) str, I) isDirty, J) chgSrc<br/>
		/// date mod - matches: K) str, L) isDirty, M) chgSrc<br/>
		/// buttons: L) ismodexo, M) undo, N) apply
		/// </summary>
		public TestSequence2 Ts2_ShtStdTestsC;


		/// <summary>
		/// UI Tests<br/>
		/// A) Sht Desc can undo | B) Sht NameMod can undo<br/>
		/// C) Sht can be edited | D) Sht is changed<br/>
		/// E) Sht Apply btn enabled | F) Sht undo btn enabled<br/>
		/// </summary>
		public TestSequence2 Ts2_ShtUiEndSequenceA;

		/// <summary>
		/// UI Tests<br/>
		/// A) Sht Desc can undo | B) UpdRule can undo
		/// C) OpSeq can undo | D) Sht NameMod can undo<br/>
		/// E) Sht can be edited | F) Sht is changed<br/>
		/// G) Sht Apply btn enabled | H) Sht undo btn enabled<br/>
		/// </summary>
		public TestSequence2 Ts2_ShtUiEndSequenceB;

		/// <summary>
		/// UI Tests - related to fam and type list<br/>
		/// A) Sht is mod fam lst wkg<br/>
		/// B) Sht can edit | C) sht changed<br/>
		/// D) Sht Apply btn enabled | E) Sht undo btn enabled<br/>
		/// </summary>
		public TestSequence2 Ts2_ShtUiEndSequenceC;

	#endregion

	#region test choices

		/* test choice fields */

		/* ui */

		/* wbk */

		private TestChoice2 uiWbkDescCanUndo;
		private TestChoice2 uiWbkLastIdCanUndo;
		private TestChoice2 uiWbkNameModCanUndo;
		private TestChoice2 uiWbkCanBeEdited;

		private TestChoice2 uiWbkChanged;
		private TestChoice2 uiWbkApplyBtnEnabled;
		private TestChoice2 uiWbkUndoBtnEnabled;

		/* xd */

		private TestChoice2 uiXdApplyBtnShtsLstEnabled;
		private TestChoice2 uiXdUndoBtnShtsLstEnabled;

		/* sht */

		private TestChoice2 uiShtDescCanUndo;
		private TestChoice2 uiShtOpSeqCanUndo;
		private TestChoice2 uiShtUpdRuleCanUndo;
		private TestChoice2 uiShtNameModCanUndo;
		private TestChoice2 uiShtCanBeEdited;

		private TestChoice2 uiShtChanged;
		private TestChoice2 uiShtApplyBtnEnabled;
		private TestChoice2 uiShtUndoBtnEnabled;

		private TestChoice2 uiShtIsModFamLstWkg;


		/* wbk bool */

		// ReSharper disable InconsistentNaming
		private TestChoice2 wbkIsModExo;

		private TestChoice2 wbkUndoBtn;
		private TestChoice2 wbkApplyBtn;
		private TestChoice2 xdUndoBtnShtsLst;
		private TestChoice2 xdApplyBtnShtsLst;


		/* desc */
		private TestChoice2 wbkDescStr;
		private TestChoice2 wbkDescDirty;
		private TestChoice2 wbkDescCs;

		/* name mod */
		private TestChoice2 wbkNameModStr;
		private TestChoice2 wbkNameModDirty;
		private TestChoice2 wbkNameModCs;

		/* date mod */
		private TestChoice2 wbkDateModStr;
		private TestChoice2 wbkDateModDirty;
		private TestChoice2 wbkDateModCs;

		/* lastid */
		private TestChoice2 wbkLastIdStr;
		private TestChoice2 wbkLastIdPriorStr;
		private TestChoice2 wbkLastIdDirty;
		private TestChoice2 wbkLastIdCs;

		/* shts list */
		private TestChoice2 wbkShtsLstInt;
		private TestChoice2 wbkShtsLstDirty;
		private TestChoice2 wbkShtsLstCs;


		/* sht bool */

		private TestChoice2 shtIsModExo;

		private TestChoice2 shtUndoBtn;
		private TestChoice2 shtApplyBtn;

		/* sht items */

		/* desc */
		private TestChoice2 shtDescStr;
		private TestChoice2 shtDescDirty;
		private TestChoice2 shtDescCs;


		/* update rule */
		private TestChoice2 shtUpdRuleEnum;
		private TestChoice2 shtUpdRuleDirty;
		private TestChoice2 shtUpdRuleCs;

		/* op sequence */
		private TestChoice2 shtOpSeqStr;
		private TestChoice2 shtOpSeqDirty;
		private TestChoice2 shtOpSeqCs;


		/* name mod */
		private TestChoice2 shtNameModStr;
		private TestChoice2 shtNameModDirty;
		private TestChoice2 shtNameModCs;

		/* date mod */
		private TestChoice2 shtDateModStr;
		private TestChoice2 shtDateModDirty;
		private TestChoice2 shtDateModCs;

		/* fam and type list */
		private TestChoice2 shtFatCountInt;
		private TestChoice2 shtFatCountIntWkg;
		private TestChoice2 shtFatIsDirty;
		private TestChoice2 shtFatCs;
		private TestChoice2 shtFatWkgHasNew;
		private TestChoice2 shtFatHasKey;
		private TestChoice2 shtFatHasKeyWkg;


		#endregion


		// ReSharper restore InconsistentNaming


		private void configTests2()
		{
			/* bools */

			// TestChoice2 w = wbkIsModExo;

			// wbk is mod exo bool
			RegisterTest2( "IsModExo?", WBK_TST_IS_MOD_EXO_B, validate_WbkIsModExoB,
				[
					new ("F", false, true),
					new ("T", true, true)
				],
				ref wbkIsModExo);

			// wbk undo button
			RegisterTest2("undo button?", WBK_TST_UNDO_BTN_B, validate_WbkUndoBtnB,
				[
					new ("F", false, true), new ("T", true, true)
				],
				ref wbkUndoBtn);

			// wbk apply button
			RegisterTest2("apply button?", WBK_TST_APPLY_BTN_B, validate_WbkApplyBtnB,
				[new ("F", false, true), new ("T", true, true)],
				ref wbkApplyBtn);

			// is undo btn shts list
			RegisterTest2("UndoBtnShtsLst?", XD_TST_UNDO_BTN_SHTS_LST_B, validate_XdUndoBtnShtsLstB,
				[new ("F", false, true), new ("T", true, true)],
				ref xdUndoBtnShtsLst);

			// is apply btn shts list
			RegisterTest2("ApplyBtnShtsLst?", XD_TST_APPLY_BTN_SHTS_LST_B, validate_XdApplyBtnShtsLstB,
				[new ("F", false, true), new ("T", true, true)],
				ref xdApplyBtnShtsLst);

			/* desc */

			// wbk desc str
			RegisterTest2("desc str ==", WBK_TST_DESC_STR, validate_WbkDescStr,
				[new ("Init", true, FAUX_WBK_DESC_INIT), new ("Alt1", true, FAUX_WBK_DESC_ALT1)],
				ref wbkDescStr);

			// wbk desc is dirty
			RegisterTest2("desc is dirty?", WBK_TST_DESC_IS_DIRTY, validate_WbkDescIsDirty,
				[new ("F", false, true), new ("T", true, true)],
				ref wbkDescDirty);

			// wbk desc chg src
			RegisterTest2("desc chg src ==", WBK_TST_DESC_CS, validate_WbkDescChgSrc,
				[new ("N", true, ChgSrcId.CI_NONE), new ("A", true, ChgSrcId.CI_SRC_A)],
				ref wbkDescCs);

			/* name mod */

			// wbk name mod str
			RegisterTest2("name mod str ==", WBK_TST_NAME_MOD_STR, validate_WbkNameModStr,
				[new ("Init", true, FAUX_USER_NAME_INIT), new ("Alt1", true, FAUX_USER_NAME_ALT1), new ("Alt2", true, FAUX_USER_NAME_ALT2)],
				ref wbkNameModStr);

			// wbk desc is dirty
			RegisterTest2("name mod is dirty?", WBK_TST_NAME_MOD_IS_DIRTY, validate_WbkNameModIsDirty,
				[new ("F", false, true), new ("T", true, true)],
				ref wbkNameModDirty);

			// wbk name mod chg src
			RegisterTest2("name mod chg src ==", WBK_TST_NAME_MOD_CS, validate_WbkNameModChgSrc,
				[new ("N", true, ChgSrcId.CI_NONE), new ("T", true, ChgSrcId.CI_SRC_T), new ("X", true, ChgSrcId.CI_SRC_X)],
				ref wbkNameModCs);

			/* date mod */

			// wbk date mod str
			RegisterTest2("date mod str ==", WBK_TST_DATE_MOD_STR, validate_WbkDateModStr,
				[
					new ("Init", true, FAUX_MOD_DATE_INIT), new ("Upd1", true, FAUX_MOD_DATE_UPD1),
					new ("Upd2", true, FAUX_MOD_DATE_UPD2)
				],
				ref wbkDateModStr);

			// wbk date mod is dirty
			RegisterTest2("date mod is dirty?", WBK_TST_DATE_MOD_IS_DIRTY, validate_WbkDateModIsDirty,
				[new ("F", false, true), new ("T", true, true)],
				ref wbkDateModDirty);

			// wbk date mod chg src
			RegisterTest2("date mod chg src ==", WBK_TST_DATE_MOD_CS, validate_WbkDateModChgSrc,
				[new ("N", true, ChgSrcId.CI_NONE), new ("T", true, ChgSrcId.CI_SRC_T), new ("X", true, ChgSrcId.CI_SRC_X)],
				ref wbkDateModCs);


			/* last id */

			// wbk last id str
			RegisterTest2("last id str ==", WBK_TST_LAST_ID_STR, validate_WbkLastIdStr,
				[
					new ("Init", true, FAUX_LAST_ID_INIT), new ("E", true, FAUX_LAST_ID_UPD_E),
					new ("F", true, FAUX_LAST_ID_UPD_F), new ("G", true, FAUX_LAST_ID_UPD_G),
				],
				ref wbkLastIdStr);

			RegisterTest2("last id str ==", WBK_TST_LAST_ID_PRIOR_STR, validate_WbkLastIdPriorStr,
				[
					new ("MT", true, ""),
					new ("Init", true, FAUX_LAST_ID_INIT), new ("E", true, FAUX_LAST_ID_UPD_E),
					new ("F", true, FAUX_LAST_ID_UPD_F), new ("G", true, FAUX_LAST_ID_UPD_G),
				],
				ref wbkLastIdPriorStr);


			// wbk date mod is dirty
			RegisterTest2("last id is dirty?", WBK_TST_LAST_ID_IS_DIRTY, validate_WbkLastIdIsDirty,
				[new ("F", false, true), new ("T", true, true)],
				ref wbkLastIdDirty);

			// wbk date mod chg src
			RegisterTest2("last id chg src ==", WBK_TST_LAST_ID_CS, validate_WbkLastIdChgSrc,
				[new ("N", true, ChgSrcId.CI_NONE), new ("A", true, ChgSrcId.CI_SRC_A), new ("E", true, ChgSrcId.CI_SRC_E)],
				ref wbkLastIdCs);


			/* shts list */

			// wbk shts list int
			RegisterTest2("shts lst value ==", WBK_TST_SHTS_LST_INT, validate_WbkShtsLstInt,
				[
					new ("neg_one", true, -1), new ("zero", true, 0),
					new ("pos_one", true, 1)
				],
				ref wbkShtsLstInt);

			// wbk shts list is dirty
			RegisterTest2("shts is dirty?", WBK_TST_SHTS_LST_IS_DIRTY, validate_WbkShtsLstIsDirty,
				[new ("F", false, true), new ("T", true, true)],
				ref wbkShtsLstDirty);

			// wbk shts list chg src
			RegisterTest2("shts lst is cs ==?", WBK_TST_SHTS_LST_CS, validate_WbkShtsLstChgSrc,
			[
				new ("N", true, ChgSrcId.CI_NONE),
				new ("E", true, ChgSrcId.CI_SRC_E)
			], ref wbkShtsLstCs);


			/* sht */

			// sht is mod exo bool
			RegisterTest2("IsModExo?", SHT_TST_IS_MOD_EXO_B, validate_ShtIsModExoB,
			[
				new ("F", false, true),
				new ("T", true, true)
			], ref shtIsModExo);

			// sht undo button
			RegisterTest2("undo button?", SHT_TST_UNDO_BTN_B, validate_ShtUndoBtnB,
				[new ("F", false, true), new ("T", true, true)],
				ref shtUndoBtn);

			// sht apply button
			RegisterTest2("apply button?", SHT_TST_APPLY_BTN_B, validate_ShtApplyBtnB,
				[new ("F", false, true), new ("T", true, true)],
				ref shtApplyBtn);


			/* desc */

			// sht desc str
			RegisterTest2("desc str ==", SHT_TST_DESC_STR, validate_ShtDescStr,
			[
				new ("Init", true, FAUX_SHT_DESC_INIT),
				new ("Alt1", true, FAUX_SHT_DESC_ALT1)
			], ref shtDescStr);

			// sht desc is dirty
			RegisterTest2("desc is dirty?", SHT_TST_DESC_IS_DIRTY, validate_ShtDescIsDirty,
			[
				new ("F", false, true),
				new ("T", true, true)
			], ref shtDescDirty);

			// sht desc chg src
			RegisterTest2("desc chg src ==", SHT_TST_DESC_CS, validate_ShtDescChgSrc,
			[
				new ("N", true, ChgSrcId.CI_NONE),
				new ("A", true, ChgSrcId.CI_SRC_A)
			], ref shtDescCs);


			/* update rule */

			// sht update rule str
			RegisterTest2("update rule ==", SHT_TST_UPD_RULE_ENUM, validate_ShtUpdRuleEnum,
			[
				new ("Init", true, FAUX_SHT_UPDATE_RULE_INIT),
				new ("Alt1", true, FAUX_SHT_UPDATE_RULE_ALT1),
				new ("Alt2", true, FAUX_SHT_UPDATE_RULE_ALT2),
			], ref shtUpdRuleEnum);

			// sht update rule is dirty
			RegisterTest2("update rule is dirty?", SHT_TST_UPD_RULE_IS_DIRTY, validate_ShtUpdRuleIsDirty,
			[
				new ("F", false, true),
				new ("T", true, true)
			], ref shtUpdRuleDirty);

			// sht update rule chg src
			RegisterTest2("update rule chg src ==", SHT_TST_UPD_RULE_CS, validate_ShtUpdRuleChgSrc,
			[
				new ("N", true, ChgSrcId.CI_NONE),
				new ("A", true, ChgSrcId.CI_SRC_A)
			], ref shtUpdRuleCs);


			/* op seq */

			// sht op seq str
			RegisterTest2("op seq str ==", SHT_TST_OP_SEQ_STR, validate_ShtOpSeqStr,
			[
				new ("Init", true, FAUX_SHT_OP_SEQ_INIT),
				new ("Alt1", true, FAUX_SHT_OP_SEQ_ALT1),
				new ("Alt2", true, FAUX_SHT_OP_SEQ_ALT2),
			], ref shtOpSeqStr);

			// sht op seq is dirty
			RegisterTest2("op seq is dirty?", SHT_TST_OP_SEQ_IS_DIRTY, validate_ShtOpSeqIsDirty,
			[
				new ("F", false, true),
				new ("T", true, true)
			], ref shtOpSeqDirty);

			// sht op seq chg src
			RegisterTest2("op seq chg src ==", SHT_TST_OP_SEQ_CS, validate_ShtOpSeqChgSrc,
			[
				new ("N", true, ChgSrcId.CI_NONE),
				new ("A", true, ChgSrcId.CI_SRC_A)
			], ref shtOpSeqCs);


			/* name mod */

			// sht name mod str
			RegisterTest2("name mod str ==", SHT_TST_NAME_MOD_STR, validate_ShtNameModStr,
				[new ("Init", true, FAUX_USER_NAME_INIT), new ("Alt1", true, FAUX_USER_NAME_ALT1), new ("Alt2", true, FAUX_USER_NAME_ALT2)],
				ref shtNameModStr);

			// sht desc is dirty
			RegisterTest2("name mod is dirty?", SHT_TST_NAME_MOD_IS_DIRTY, validate_ShtNameModIsDirty,
				[new ("F", false, true), new ("T", true, true)],
				ref shtNameModDirty);

			// sht name mod chg src
			RegisterTest2("name mod chg src ==", SHT_TST_NAME_MOD_CS, validate_ShtNameModChgSrc,
				[new ("N", true, ChgSrcId.CI_NONE), new ("T", true, ChgSrcId.CI_SRC_T), new ("X", true, ChgSrcId.CI_SRC_X)],
				ref shtNameModCs);

			/* date mod */

			// sht date mod str
			RegisterTest2("date mod str ==", SHT_TST_DATE_MOD_STR, validate_ShtDateModStr,
				[
					new ("Init", true, FAUX_MOD_DATE_INIT), new ("Upd1", true, FAUX_MOD_DATE_UPD1),
					new ("Upd2", true, FAUX_MOD_DATE_UPD2)
				],
				ref shtDateModStr);

			// sht date mod is dirty
			RegisterTest2("date mod is dirty?", SHT_TST_DATE_MOD_IS_DIRTY, validate_ShtDateModIsDirty,
				[new ("F", false, true), new ("T", true, true)],
				ref shtDateModDirty);

			// sht date mod chg src
			RegisterTest2("date mod chg src ==", SHT_TST_DATE_MOD_CS, validate_ShtDateModChgSrc,
				[new ("N", true, ChgSrcId.CI_NONE), new ("T", true, ChgSrcId.CI_SRC_T), new ("X", true, ChgSrcId.CI_SRC_X)],
				ref shtDateModCs);


			/* fam list */


			// sht fam list is dirty
			RegisterTest2("family list count", SHT_TST_FAT_COUNT_INT, validate_ShtFatCountInt,
			[
				new ("3", true, 3),
				new ("4", true, 4),
				new ("5", true, 5),
				new ("6", true, 6),
				new ("7", true, 7),
				new ("8", true, 8),
			], ref shtFatCountInt);


			// sht fam list is dirty
			RegisterTest2("family list wkg count", SHT_TST_FAT_COUNT_WKG_INT, validate_ShtFatCountWkgInt,
			[
				new ("3", true, 3),
				new ("4", true, 4),
				new ("5", true, 5),
				new ("6", true, 6),
				new ("7", true, 7),
				new ("8", true, 8),
			], ref shtFatCountIntWkg);


			// sht fam list is dirty
			RegisterTest2("family list is dirty?", SHT_TST_FAT_IS_DIRTY, validate_ShtFatIsDirty,
				[
					new ("F", false, true),
					new ("T", true, true)
				], ref shtFatIsDirty);

			// sht family list chg src
			RegisterTest2("family list chg src ==", SHT_TST_FAT_CS, validate_ShtFatChgSrc,
				[
					new ("N", true, ChgSrcId.CI_NONE), 
					new ("D", true, ChgSrcId.CI_SRC_D)
				], ref shtFatCs);

			// sht date mod chg src
			RegisterTest2("family list wkg has new", SHT_TST_FAT_WKG_HAS_NEW_B, validate_ShtFatWkgHasNewBool,
				[
					new ("F", false, true),
					new ("T", true, true)
				], ref shtFatWkgHasNew);


			// sht date mod chg src
			RegisterTest2("family list has key", SHT_TST_FAT_HAS_KEY_STR, validate_ShtFatHasKeyStr,
				[
					new ("Init", true, Faux_FatItemKey_Init),
					new ("InitF", false, Faux_FatItemKey_Init),
					new ("Alt1", true, Faux_FatItemKey_Alt1),
					new ("Alt2", true, Faux_FatItemKey_Alt2)
				], ref shtFatHasKey);

			// sht date mod chg src
			RegisterTest2("family list wkg has key", SHT_TST_FAT_HAS_KEY_WKG_STR, validate_ShtFatHasKeyWkgStr,
				[
					new ("Init", true, Faux_FatItemKey_Init),
					new ("InitF", false, Faux_FatItemKey_Init),
					new ("Alt1", true, Faux_FatItemKey_Alt1),
					new ("Alt2", true, Faux_FatItemKey_Alt2),
				], ref shtFatHasKeyWkg);


			/* ui */

			/* ui wbk */

			RegisterTest2("ui last id can undo", UI_WBK_LASTID_CAN_UNDO, verify_ui_wbk_lastid_can_undo,
			[
				new ("F", false,   null),
				new ("T", true, null),
				new (NA, true, null)
			], ref uiWbkLastIdCanUndo);


			RegisterTest2("ui desc can undo", UI_WBK_DESC_CAN_UNDO, verify_ui_wbk_desc_can_undo,

				// example 1 testing for "F" (not enabled) - field is dirty => T, apply button is enabled == !T == F
				// first test is T &= F => F - this is the answer (not enabled)
				// result is F == "F" => T - the test result T (which is correct)

				// example 2 testing for "F" (not enabled) -  field is dirty => T, apply button is disabled == !F == T
				// actual answer should be T 
				// first test is T &= T - this is the answer (enabled)
				// result is T == "F" => F - the test result is F (which means that the test failed - which is correct)

				// example 3 testing for "T" (enabled) - field is dirty => T, apply button is enabled == !T == F
				// first test is T &= F - this is the answer (not enabled)
				// the answer is F == "T" => F - the test result is F (which means that the test failed - which is correct)

				// example 2 testing for "T" -  field is dirty => T, apply button is disabled == !F == T
				// first test is T &= T - which is true; 
				// the answer is T == "T" => T - the test result T (which is correct)
				[
					new ("F", false, null),
					new ("T", true, null),
					new (NA, true, null)
				], ref uiWbkDescCanUndo);

			RegisterTest2("ui name mod can undo", UI_WBK_NAMEMOD_CAN_UNDO, verify_ui_wbk_namemod_can_undo,
			[
				new ("F", false,   null),
				new ("T", true, null),
				new (NA, true, null)
			], ref uiWbkNameModCanUndo);

			// only can edit when apply button is false
			RegisterTest2("ui wbk can edit", UI_WBK_CAN_EDIT, verify_ui_wbk_can_edit,
			[
				new ("F", false,   null),
				new ("T", true, null),
				new (NA, true, null)
			], ref uiWbkCanBeEdited);

			// only can edit when apply button is false
			RegisterTest2("ui wbk changed", UI_WBK_CHANGED, verify_ui_wbk_changed,
			[
				new ("F", false, null),
				new ("T", true, null),
				new (NA, true, null)
			], ref uiWbkChanged);

			// only can edit when apply button is false
			RegisterTest2("ui wbk apply btn enabled", UI_WBK_APPLY_BTN_ENABLED, verify_ui_wbk_apply_btn_enabled,
			[
				new ("F", false, null),
				new ("T", true, null),
				new (NA, true, null)
			], ref uiWbkApplyBtnEnabled);

			// only can edit when undo button is false
			RegisterTest2("ui wbk undo btn enabled", UI_WBK_UNDO_BTN_ENABLED, verify_ui_wbk_undo_btn_enabled,
			[
				new ("F", false, null),
				new ("T", true, null),
				new (NA, true, null)
			], ref uiWbkUndoBtnEnabled);


			/* ui shts list */

			// only can edit when apply button is false
			RegisterTest2("ui shts lst apply btn enabled", UI_XD_APPLY_BTN_SHTSLST_ENABLED, verify_ui_xd_apply_btn_shtslst_enabled,
			[
				new ("F", false, null),
				new ("T", true, null),
				new (NA, true, null)
			], ref uiXdApplyBtnShtsLstEnabled);

			// only can edit when undo button is false
			RegisterTest2("ui shts lst undo btn enabled", UI_XD_UNDO_BTN_SHTSLST_ENABLED, verify_ui_xd_undo_btn_shtslst_enabled,
			[
				new ("F", false, null),
				new ("T", true, null),
				new (NA, true, null)
			], ref uiXdUndoBtnShtsLstEnabled);


			/* ui sht */

			RegisterTest2("ui sht desc can undo", UI_SHT_DESC_CAN_UNDO, verify_ui_sht_desc_can_undo,

				// example 1 testing for "F" (not enabled) - field is dirty => T, apply button is enabled == !T == F
				// first test is T &= F => F - this is the answer (not enabled)
				// result is F == "F" => T - the test result T (which is correct)

				// example 2 testing for "F" (not enabled) -  field is dirty => T, apply button is disabled == !F == T
				// actual answer should be T 
				// first test is T &= T - this is the answer (enabled)
				// result is T == "F" => F - the test result is F (which means that the test failed - which is correct)

				// example 3 testing for "T" (enabled) - field is dirty => T, apply button is enabled == !T == F
				// first test is T &= F - this is the answer (not enabled)
				// the answer is F == "T" => F - the test result is F (which means that the test failed - which is correct)

				// example 2 testing for "T" -  field is dirty => T, apply button is disabled == !F == T
				// first test is T &= T - which is true; 
				// the answer is T == "T" => T - the test result T (which is correct)
				[
					new ("F", false, null),
					new ("T", true, null),
					new (NA, true, null)
				], ref uiShtDescCanUndo);

			RegisterTest2("ui sht name mod can undo", UI_SHT_NAMEMOD_CAN_UNDO, verify_ui_sht_namemod_can_undo,
			[
				new ("F", false,   null),
				new ("T", true, null),
				new (NA, true, null)
			], ref uiShtNameModCanUndo);

			RegisterTest2("ui sht upd rule can undo", UI_SHT_UPDRULE_CAN_UNDO, verify_ui_sht_updrule_can_undo,
			[
				new ("F", false,   null),
				new ("T", true, null),
				new (NA, true, null)
			], ref uiShtUpdRuleCanUndo);

			RegisterTest2("ui sht op seq can undo", UI_SHT_OPSEQ_CAN_UNDO, verify_ui_sht_opseq_can_undo,
			[
				new ("F", false,   null),
				new ("T", true, null),
				new (NA, true, null)
			], ref uiShtOpSeqCanUndo);

			// only can edit when apply button is false
			RegisterTest2("ui sht can edit", UI_SHT_CAN_EDIT, verify_ui_sht_can_edit,
			[
				new ("F", false,   null),
				new ("T", true, null),
				new (NA, true, null)
			], ref uiShtCanBeEdited);

			// only can edit when apply button is false
			RegisterTest2("ui sht changed", UI_SHT_CHANGED, verify_ui_sht_changed,
			[
				new ("F", false, null),
				new ("T", true, null),
				new (NA, true, null)
			], ref uiShtChanged);

			/* ui sht fam lst */

			RegisterTest2("ui is mod fam list wkg", UI_SHT_ISMODFAMLSTWKG, validate_ui_sht_ismodfamlstwkg,
			[
				new ("F", false, null),
				new ("T", true, null),
				new (NA, true, null)
			], ref uiShtIsModFamLstWkg);

			// only can edit when apply button is false
			RegisterTest2("ui sht apply btn enabled", UI_SHT_APPLY_BTN_ENABLED, verify_ui_sht_apply_btn_enabled,
			[
				new ("F", false, null),
				new ("T", true, null),
				new (NA, true, null)
			], ref uiShtApplyBtnEnabled);

			// only can edit when undo button is false
			RegisterTest2("ui sht undo btn enabled", UI_SHT_UNDO_BTN_ENABLED, verify_ui_sht_undo_btn_enabled,
			[
				new ("F", false, null),
				new ("T", true, null),
				new (NA, true, null)
			], ref uiShtUndoBtnEnabled);


			// must occur after the above configures the values
			Ts2_WbkStdTestsA = new ("Standard Tests A (Wbk) (desc, name mod, date mod, buttons",
			[
				wbkDescStr, wbkDescDirty, wbkDescCs,
				wbkNameModStr, wbkNameModDirty, wbkNameModCs,
				wbkDateModStr, wbkDateModDirty, wbkDateModCs,
				wbkIsModExo, wbkUndoBtn, wbkApplyBtn,
			]);

			Ts2_WbkStdTestsC = new ("Standard Tests C (Wbk) (desc, last id, name mod (by usr), date mod, buttons",
			[
				wbkDescStr, wbkDescDirty, wbkDescCs,
				wbkLastIdStr, wbkLastIdDirty, wbkLastIdCs,
				wbkNameModStr, wbkNameModDirty, wbkNameModCs,
				wbkDateModStr, wbkDateModDirty, wbkDateModCs,
				wbkIsModExo, wbkUndoBtn, wbkApplyBtn,
			]);


			Ts2_WbkShtLstTestsA = new ("Standard Sheets List Tests A - add a sheet, apply / undo sheets list",
			[
				wbkShtsLstInt, wbkShtsLstDirty, wbkShtsLstCs,
				wbkLastIdStr, wbkLastIdDirty, wbkLastIdCs,
				wbkNameModStr, wbkNameModDirty, wbkNameModCs,
				wbkDateModStr, wbkDateModDirty, wbkDateModCs,
				wbkIsModExo, wbkUndoBtn, wbkApplyBtn,
				xdApplyBtnShtsLst, xdUndoBtnShtsLst
			]);

			Ts2_WbkShtLstTestsB = new ("Standard Sheets List Tests B - change fields, add a sheet, apply sheets list",
			[
				wbkDescStr, wbkDescDirty, wbkDescCs,
				wbkLastIdStr, wbkLastIdPriorStr, wbkLastIdDirty, wbkLastIdCs,
				wbkNameModStr, wbkNameModDirty, wbkNameModCs,
				wbkDateModStr, wbkDateModDirty, wbkDateModCs,
				wbkShtsLstInt, wbkShtsLstDirty, wbkShtsLstCs,
				wbkIsModExo, wbkUndoBtn, wbkApplyBtn,
				xdApplyBtnShtsLst, xdUndoBtnShtsLst
			]);


			Ts2_WbkUiEndSequenceA = new ("Ui Std Verification Tests A (Wbk)",
			[
				uiWbkDescCanUndo, uiWbkLastIdCanUndo, uiWbkNameModCanUndo,
				uiWbkCanBeEdited, uiWbkChanged,
				uiWbkApplyBtnEnabled, uiWbkUndoBtnEnabled,
				uiXdApplyBtnShtsLstEnabled, uiXdUndoBtnShtsLstEnabled
			]);

			Ts2_ShtUiEndSequenceA = new ("Ui Std Verification Tests A (Sht)",
			[
				uiShtDescCanUndo, uiShtNameModCanUndo,
				uiShtCanBeEdited, uiShtChanged,
				uiShtApplyBtnEnabled, uiShtUndoBtnEnabled,
			]);

			Ts2_ShtUiEndSequenceB = new ("Ui Std Verification Tests B (Sht)",
			[
				uiShtDescCanUndo, uiShtUpdRuleCanUndo,uiShtOpSeqCanUndo,
				uiShtNameModCanUndo, uiShtCanBeEdited, uiShtChanged,
				uiShtApplyBtnEnabled, uiShtUndoBtnEnabled,
			]);

			Ts2_ShtUiEndSequenceC = new ("Ui Std Verification Tests C (Sht - Fat)",
			[
				uiShtIsModFamLstWkg,
				uiShtCanBeEdited, uiShtChanged,
				uiShtApplyBtnEnabled, uiShtUndoBtnEnabled
			]);


			Ts2_ShtStdTestsA = new ("Standard Tests A (Sht) (desc, name mod, date mod, buttons)",
			[
				shtDescStr, shtDescDirty, shtDescCs,
				shtNameModStr, shtNameModDirty, shtNameModCs,
				shtDateModStr, shtDateModDirty, shtDateModCs,
				shtIsModExo, shtUndoBtn, shtApplyBtn,
			]);

			Ts2_ShtStdTestsB = new ("Standard Tests A (Sht) (desc, opseq, updrule, name mod, date mod, buttons)",
			[
				shtDescStr, shtDescDirty, shtDescCs,
				shtUpdRuleEnum, shtUpdRuleDirty, shtUpdRuleCs,
				shtOpSeqStr, shtOpSeqDirty, shtOpSeqCs,
				shtNameModStr, shtNameModDirty, shtNameModCs,
				shtDateModStr, shtDateModDirty, shtDateModCs,
				shtIsModExo, shtUndoBtn, shtApplyBtn,
			]);

			Ts2_ShtStdTestsC= new ("Standard Tests C (Sht) (fam and type added)",
			[
				shtFatIsDirty, shtFatCs, shtFatWkgHasNew, shtFatHasKey, shtFatHasKeyWkg,
				shtFatCountInt, shtFatCountIntWkg,
				shtNameModStr, shtNameModDirty, shtNameModCs,
				shtDateModStr, shtDateModDirty, shtDateModCs,
				shtIsModExo, shtUndoBtn, shtApplyBtn,
			]);


		}
	}
}