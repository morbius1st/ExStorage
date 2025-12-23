using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Interop;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using ExStoreTest2026;
using ExStoreTest2026.DebugAssist;
using ExStoreTest2026.ExStorSys;
using ExStoreTest2026.Windows;
using RevitLibrary;
using UtilityLibrary;
using static ExStorSys.DynaValue;
using static ExStorSys.ExStorStartMgr.TdKey;
using static ExStorSys.ExSysStatus;
using static ExStorSys.PropertyId;
using static ExStorSys.PropertyOwner;
using static ExStorSys.ValidateDataStorage;
using static ExStorSys.ValidateSchema;


// user name: jeffs
// created:   11/3/2025 5:47:16 PM

namespace ExStorSys
{
	public class ExStorStartMgr : APropChgdEvt
	{
		public int ObjectId;

		private MainWindow Mw;

		private MainWinModelUi xMui;

		private ExStorLib xLib;
		private ExStorMgr xMgr;
		private ExStorData xData;

		private string tdMsgXtra;

		private RunningStatus rs;

		// private ExSysStatus tempExSysStatus;
		// private RunningStatus tempRunStat;


	#region ctor

		#pragma warning disable CS8618, CS9264
		private ExStorStartMgr() { }
		#pragma warning restore CS8618, CS9264

		public static ExStorStartMgr? Instance { get; set; }

		public static ExStorStartMgr Create()
		{
			Instance = new ExStorStartMgr();
			Instance.init();

			return Instance;
		}

		private void init()
		{
			ObjectId = AddObjId(nameof(ExStorStartMgr));

			xLib = ExStorLib.Instance;
			xMui = MainWinModelUi.Create();
			xMgr = ExStorMgr.Create();
			xData = ExStorData.Instance;
			xMui.Init();
			Mw = new MainWindow();

			this.PropChgd += xMui.OnPropChgdEvent;

			reCat();

			// events??
		}

		public void Restore()
		{
			xLib = ExStorLib.Instance;

			MainWinModelUi.Instance = xMui;
			ExStorMgr.Instance =      xMgr;
			xMgr.Restore();
			xMui.Restore();
			xData = ExStorData.Instance;
			Mw = new MainWindow();

			// events??

			xMgr.MessageCache = String.Empty;
			// xMui.LaunchCode = LaunchCode.LC_NA;
			// xMui.ExSysStatus = ExSysStatus.ES_STARTED;
		}

	#endregion

		public Dictionary<string, List<int>> ObjectIdList = new ();

		/* start operator rules
		* use the results from LaunchManager
		* use the (4) validation status properties in MainWinModelUi
		*
		* not using LaunchCode beyond the LaunchManager
		*
		* process and status results from each StartOp Resolution Method
		* at entry into main method:
		* > second plus time window opened -> winOpen2ndPlus()
		*		* running status to -> RN_RUNNING_NORMAL
		*		* ExSysStatus to -> ES_START_DONE_GOOD
		*		* status, does not apply
		* > all good -> winOpen2ndPlus()  (read info all good / sheets exist)
		*		* running status to -> RN_RUNNING_NORMAL
		*		* ExSysStatus to -> ES_START_DONE_GOOD
		*		* status, does not apply
		* -> return without further processing / resolution processing
		*
		* resolution processing / return status results
		*/

		/* primary start routines */

		public void StartOpr()
		{
			// DialogTest();

			Msgs.ShowDebug = true;
			DebugRoutines.ShowObjectId(-1, -1);

			Msgs.CWriteLine($"\n\n*** Start Operator - Begin *** [ {R.FileName} ]\n");

			showStatus2("** init status");
			showStatus1("** status before switchboard");
			ShowStatus3("** data status before switchboard");

			// testStart();
			// return;

			int result;

			OnPropChgd(new PropChgEvtArgs( PI_GEN_RESTART, false));

			if (!(xMui.ExSysStatus == ES_START_DONE_GOOD &&
				xMui.SystemRunningStatus == RunningStatus.RN_RUNNING_NORMAL ||
				xMui.SystemRunningStatus == RunningStatus.RN_RUNNING_NEED_SHT))
			{
				// result = 0;

				result = switchBoardD();
				
				if (result == 1 && gotData() == false)
				{
					stdTaskDialogMsg(TD_CLOSEOPEN);
					return;
				}
			}
			else
			{
				Msgs.CWriteLine($"2nd + times through - just show window");
				result = 0;
			}

			showStatus1("status after switchboard");
			showStatus2();

			if (result == 0) winOpen2ndPlus();

			Msgs.CWriteLine($"\n*** Start Operator - End\n\n");
		}

		// ReSharper disable once InconsistentNaming
		private void winOpen2ndPlus()
		{
			// got here from 2nd+ times the window has been opened - normal start
			setStatus();
			Mw.ShowDialog();
		}

		/* processing switchboard */

		/// <summary>
		/// main processing switchboard<br/>
		/// possible results<br/>
		/// 0 = all good - no changes needed<br/>
		/// 1 = selected YES at an ask above, was repaired and now needs to be saved and restarted<br/>
		/// -1 = default answer == selected NO at an ask above<br/>
		/// -2 = selected ignore or off above<br/>
		/// -3 = none of the above - failed<br/>
		/// </summary>
		private int switchBoardD()
		{
			int result = -1;

			key = makeKey();

			string opt = procIndex[key];

			switch (opt)
			{
			case "S":
			case "0":
				{
					if (proc_sn())
					{
						OnPropChgdExs(ES_START_DONE_GOOD);

						OnPropChgdRn(opt.Equals("0") ? RunningStatus.RN_RUNNING_NORMAL : 
							RunningStatus.RN_RUNNING_NEED_SHT);

						result = 0;
					}
					// else -1 per above
					break;
				}
			case "1":
				{
					if (askActivate())
					{
						if (proc_nw()) { result = 1; }
					}
					break;
				}
			case "2":
				{
					if (askRepair())
					{
						if (proc_b()) result = 1;
					}
					break;
				}
			case "3":
				{
					if (askUpgrade())
					{
						if (proc_b())
						{
							if (proc_cs()) result = 1;
						}
					}
					break;
				}
			case "4":
				{
					if (askUpgrade())
					{
						if (proc_cs()) result = 1;
					}
					break;
				}
			case "6":
				{
					if (askFixModelInfo())
					{
						if (proc_d()) result = 1;
					}
					break;
				}
			case "7":
				{
					if (askRepair())
					{
						if (proc_cs())
						{
							if (proc_d()) result = 1;
						}
					}
					break;
				}
			case "O":
				{
					result = -2;
					OnPropChgdExs(ES_START_DONE_EXIT);
					OnPropChgdRn(RunningStatus.RN_NOT_RUNNING);
					break;
				}
			case "I":
				{
					result = -2;
					OnPropChgdExs(ES_START_DONE_EXIT);
					OnPropChgdRn(RunningStatus.RN_DEACTIVATE);
					break;
				}
			default:
				{
					result = -3;
					OnPropChgdExs(ES_START_DONE_EXIT);
					OnPropChgdRn(RunningStatus.RN_CANNOT_RUN_FAIL);
					break;
				}
			}

			// selected NO for an ask above
			if (result == -1)
			{
				OnPropChgdExs(ES_START_DONE_EXIT);
				OnPropChgdRn(RunningStatus.RN_DEACTIVATE);
			}

			// selected yes for an ask aboveand now needs to be saved and restarted
			if (result == 1)
			{
				OnPropChgdExs(ES_START_DONE_EXIT);
				OnPropChgdRn(RunningStatus.RN_CANNOT_RUN_RESTART);
			}

			return result;
		}

		/* primary processing methods */

		/// <summary>
		/// start up normal
		/// </summary>
		private bool proc_sn()
		{
			Msgs.CWriteLine("at proc_sn (start normal)");
			return startNormal();
		}

		/// <summary>
		/// create new
		/// </summary>
		private bool proc_nw()
		{
			Msgs.CWriteLine("at proc_nw (start activate)");

			if (startActivate()) return true;

			return false;
		}

		/// <summary>
		/// fix sheet version
		/// </summary>
		private bool proc_cs()
		{
			return true;
		}

		/// <summary>
		/// fix model name
		/// </summary>
		private bool proc_d()
		{
			return true;
		}

		/// <summary>
		/// create workbook
		/// </summary>
		private bool proc_b()
		{
			return true;
		}

		private bool? gotData()
		{
			bool resultW = xData.GotWbkSchema && xData.GotWbkDs;
			bool resultS =xData.GotShtSchema && xData.GotTempAnySheetsEx;

			return resultW ? (resultS ? true : null) : false;

		}

		private bool startNormal(bool reset = true)
		{
			if (reset) xMgr.ResetData();

			bool? result;

			result = xMgr.ProcTransWbk();

			if (result != false)
			{
				// return value does not matter
				// it is ok if there are no sheets to process
				xMgr.ProcTransSht();
			}

			return result != false;
		}

		private bool startActivate()
		{
			bool? result;

			xMgr.ResetData();

			result = xMgr.ProcCreateAndWriteWbk();
		
			if (result == true)
			{
				result = xMgr.ProcCreateSht();
			}

			ShowStatus3("at start activate");

			return result != false;
		}


		/* alt start routines */

		public void StartOprAlt()
		{
			winOpen2ndPlus();
		}

		public void StartOpr1()
		{
			Msgs.ShowDebug = true;

			showStatus2("init status");
			showStatus1("init status");
		}


		/* utilities */

		private int makeKey()
		{
			return getKey(
				xMui.WbkScResCode, 
				xMui.WbkDsResCode, 
				xMui.ShtScResCode,
				xMui.ShtDsResCode 
				);

		}

		private int getKey(ValidateSchema sw, ValidateDataStorage dw, ValidateSchema ss, ValidateDataStorage ds)
		{
			return (int)dw * 1000 + (int)ds * 100 + (int) sw * 10 + (int) ss;
		}

		/// <summary>
		/// set the verify status for each component<br/>
		/// set launch code to LC_NA
		/// </summary>
		private void setStatus()
		{
			// status to set
			// these - validation status
			OnPropChgd(new PropChgEvtArgs( PI_VFY_WBK_SC, xMui.WbkScResCode));
			OnPropChgd(new PropChgEvtArgs( PI_VFY_WBK_DS, xMui.WbkDsResCode));
			OnPropChgd(new PropChgEvtArgs( PI_VFY_SHT_SC, xMui.ShtScResCode));
			OnPropChgd(new PropChgEvtArgs( PI_VFY_SHT_DS, xMui.ShtDsResCode));

			// launch
			OnPropChgd(new PropChgEvtArgs( PI_GEN_LAUNCHCODE, LaunchCode.LC_NA));

			// and
			// running status (included in the below)
			// exstorsys status - not before but after
			// restart status - before and after (included in the below)
		}

		private bool askActivate()
		{
			TaskDialogResult tdResult = stdTaskDialogMsg(TD_ACTIVATE);

			if (tdResult == TaskDialogResult.Yes) return true;

			return false;
		}

		private bool askUpgrade()
		{
			TaskDialogResult tdResult = stdTaskDialogMsg(TD_UPGRADE);

			if (tdResult == TaskDialogResult.Yes) return true;

			return false;
		}

		private bool askFixModelInfo()
		{
			TaskDialogResult tdResult = stdTaskDialogMsg(TD_MODEL_INFO_WRONG);

			if (tdResult == TaskDialogResult.Yes) return true;

			return false;
		}

		private bool askRepair()
		{
			TaskDialogResult tdResult = stdTaskDialogMsg(TD_FIX);

			if (tdResult == TaskDialogResult.Yes) return true;

			return false;
		}

		private bool askReActivate()
		{
			ask = "aa";
			askName = "re-activate";

			return true;

			TaskDialogResult tdResult = stdTaskDialogMsg(TD_REACTIVATE);

			if (tdResult == TaskDialogResult.Yes) return true;

			return false;
		}

		private void norifyCloseOpen()
		{
			TaskDialogResult tdResult = stdTaskDialogMsg(TD_REACTIVATE);
		}


		/* utilities */

		private void pMsg(string msg1, string msg2 = "")
		{
			Msgs.CWriteLine($"\n****** {msg1} ********", msg2);
		}

		private void showStatus1(string prefix = "", string suffix = "")
		{
			if (!prefix.IsVoid())  Msgs.CWriteLine($"\n**  {prefix}");

			Msgs.CWriteLine($"\n*** {xMui.WbkDsResCode} [{xMui.WbkDsResDesc}]");
			Msgs.CWriteLine($"*** {xMui.WbkScResCode} [{xMui.WbkScResDesc}]");
			Msgs.CWriteLine($"\n*** {xMui.ShtDsResCode} [{xMui.ShtDsResDesc}]");
			Msgs.CWriteLine($"*** {xMui.ShtScResCode} [{xMui.ShtScResDesc}]");
			if (!suffix.IsVoid()) Msgs.CWriteLine($"**  {suffix}\n");
		}

		private void showStatus2(string prefix = "", string suffix = "")
		{
			if (!prefix.IsVoid()) Msgs.CWriteLine($"\n{prefix}");
			Msgs.CWriteLine($"\n{xMui.ExSysStatus} [{xMui.ExSysStatusDesc}]");
			Msgs.CWriteLine($"{xMui.SystemRunningStatus} [{xMui.SystemRunningStatusDesc}]");
			if (!suffix.IsVoid()) Msgs.CWriteLine($"{suffix}\n");
		}

		public void ShowStatus3(string prefix = "", string suffix = "")
		{
			bool saved = Msgs.ShowDebug;
			Msgs.ShowDebug = true;

			if (!prefix.IsVoid()) Msgs.CWriteLine($"\n{prefix}");
			Msgs.CWriteLine($"got wbk        | {xData.GotWorkBook}");
			Msgs.CWriteLine($"got wbk schema | {xData.GotWbkSchema}");
			Msgs.CWriteLine($"got wbk ds     | {xData.GotWbkDs}");
			Msgs.CWriteLine($"got sht schema | {xData.GotShtSchema}");
			Msgs.CWriteLine($"got any sht ds | {xData.GotAnySheets}");

			Msgs.CWriteLine($"\ngot temp wbk schema | {xData.GotTempWbkSchema}");
			Msgs.CWriteLine($"got temp wbk ds     | {xData.GotTempWbkDs}");
			Msgs.CWriteLine($"got temp sht schema | {xData.GotTempShtSchema}");
			Msgs.CWriteLine($"got temp any sht ds | {xData.GotTempAnySheetsEx}");

			if (!suffix.IsVoid()) Msgs.CWriteLine($"{suffix}\n");

			Msgs.ShowDebug = saved;
		}

		private const int COL_A = -18;
		private const int COL_B = -20;
		private const int COL_C = -20;
		private const int COL_D = -20;


		public void ShowStatus4(string prefix = "", string suffix = "")
		{
			bool saved = Msgs.ShowDebug;
			Msgs.ShowDebug = true;

			Msgs.CWriteLine($"{" ".Repeat(26)}{"WbkDs",COL_A}\t{"WbkSc",COL_C}\t{"ShtDs",COL_B}\t{"ShtSc",COL_D}");
			Msgs.CWriteLine($"{"ignore LaunchCode",-26}{xMui.WbkDsResCode,COL_A}\t{xMui.WbkScResCode,COL_C}\t{xMui.ShtDsResCode,COL_B}\t{xMui.ShtScResCode, COL_D}\n");

			Msgs.ShowDebug = saved;
		}

		private void showStatus2(ExSysStatus status, string prefix = "", string suffix = "")
		{
			Msgs.CWriteLine("\nstatus 2");
			string msg = $"{prefix}{status} | {xMui.ExSysStatus} | {xMui.LaunchCode} | {xMui.SystemRunningStatus}{suffix}";
			Msgs.CWriteLine(msg);
		}

	#region task dialogs

		internal enum TdKey
		{
			TD_NA			 = 0,
			TD_CUSTOM			,
			TD_ACTIVATE			,
			TD_REACTIVATE		,
			TD_CONFIG_FAIL		,
			TD_MODEL_INFO_WRONG	,
			TD_UPGRADE			,
			TD_FIX				,
			TD_CLOSEOPEN		,
		}

		private static TaskDialogIcon infoTdIcon = TaskDialogIcon.TaskDialogIconInformation;

		//                                        1      2       3         4         5                        6                7
		//                                     dialog   main    main      exp
		//                                      title   inst    content   content
		private static Dictionary<TdKey,  Tuple<string, string, string[]?, string[]?, TaskDialogCommonButtons, TaskDialogResult, TaskDialogIcon>> TdData = new ()
		{
			// 0
			{
				TD_NA ,
				new ("Title", "Main Instruction", ["Main Content"], [""], TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel,
					TaskDialogResult.Ok, infoTdIcon )
			},
			// 1
			{
				TD_CUSTOM,
				new ($"{ExStorConst.APP_NAME} Status", $"{ExStorConst.APP_NAME} Status Information", ["This Content replaced"], [""], TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel,
					TaskDialogResult.Ok, infoTdIcon)
			},
			{
				// an
				TD_ACTIVATE,
				new ($"Create {ExStorConst.APP_NAME}", $"The {ExStorConst.APP_NAME} is not Active",
					[
						$"Do you want to Activate the {ExStorConst.APP_NAME} for this model?\n\n",
						$"Select Yes to Activate the {ExStorConst.APP_NAME} addin\n",
						$"Select No to continue without the {ExStorConst.APP_NAME}"
					], [""],
					TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No,
					TaskDialogResult.No, infoTdIcon)
			},

			{
				TD_REACTIVATE,
				new ($"{ExStorConst.APP_NAME}", "The {0} for this model",
					[$"Select Yes to Reactivate\nSelect No to continue without the {ExStorConst.APP_NAME}"], [""],
					TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No,
					TaskDialogResult.No, infoTdIcon)
			},

			{
				TD_CONFIG_FAIL,
				new ($"{ExStorConst.APP_NAME} Status", $"{ExStorConst.APP_NAME} Initial Configuration Failed",
					["For an unknown reason, the normal configuration\nprocess did not work as expected\nand must close and exit."], [""], TaskDialogCommonButtons.Ok,
					TaskDialogResult.Ok, infoTdIcon)
			},

			{
				// am
				TD_MODEL_INFO_WRONG,
				new ($"{ExStorConst.APP_NAME} Status", "Invalid or Out of Date information Found",
					[
						"For an unknown reason, the saved information does not match this model.\n",
						"Do you want to update the stored information for this model?",
						$"Selecting Yes will update the saved information and the {ExStorConst.APP_NAME} addin can continue.\n",
						$"Selecting No will preserve the incorrect information but the {ExStorConst.APP_NAME} addin cannot ",
						"continue and will exit"
					],
					[
						"This generally happens when the model is saved as a new model. ",
						"Or when the original model is duplicated with a new name."
					],
					TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No,
					TaskDialogResult.No, infoTdIcon)
			},

			{
				//ar
				TD_FIX,
				new ($"{ExStorConst.APP_NAME} Status", "Incorrect information Found",
					[
						"For an unknown reason, the saved information is missing or incorrect\n",
						"Do you want to adjust the stored information and activate the system for this model?",
						$"Selecting Yes will modify the saved information and the {ExStorConst.APP_NAME} addin can continue.\n",
						$"Selecting No will preserve the incorrect information but the {ExStorConst.APP_NAME} addin cannot ",
						"continue and will exit"
					],
					[
						"This generally happens when the model is saved or duplicated as a new model. ",
						"Or may happen if the model is moved to a new location"
					],
					TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No,
					TaskDialogResult.No, infoTdIcon)
			},

			{
				//au
				TD_UPGRADE,
				new ($"{ExStorConst.APP_NAME} Status", "Out-of-date information Found",
					[
						$"This model is using an old version of the saved information used by the {ExStorConst.APP_NAME} addin\n",
						"Do you want to upgrade the stored information and activate the system for this model?",
						$"Selecting Yes will upgrade the saved information and the {ExStorConst.APP_NAME} addin can continue.\n",
						$"Selecting No will preserve the old information but the {ExStorConst.APP_NAME} addin cannot ",
						"continue and will exit"
					],
					[  "" ],
					TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No,
					TaskDialogResult.No, infoTdIcon)
			},

			{
				//notice
				TD_CLOSEOPEN,
				new ($"{ExStorConst.APP_NAME} Status", "Model needs to be closed and re-opened",
					[
						$"This model has been modified in order for the {ExStorConst.APP_NAME} addin to operate\n",
						"However, there is outstanding and invalid information in memory that must be purged.  Please save, ",
						$"close, and re-open this model in order to remove any invalid information hanging around.",
						$"and to fully activate the {ExStorConst.APP_NAME} addin"
					],
					[  "" ],
					TaskDialogCommonButtons.Ok,
					TaskDialogResult.Ok, infoTdIcon)
			},
		};


		private TaskDialogResult statDialog(string msg2 = "", string? msg3 = "", TaskDialogCommonButtons btns = TaskDialogCommonButtons.None)
		{
			string msg;
			msg = $"{xMui.ExSysStatus} [{xMui.ExSysStatusDesc}\n{xMui.LaunchCode} [{xMui.LaunchStatusDesc}]";
			if (!msg2.IsVoid()) msg = $"{msg2}\n{msg}";
			if (!msg3.IsVoid()) msg = $"{msg}\n{msg3}";
			return stdTaskDialogMsg("", msg, btns);
		}

		private TaskDialogResult DialogTest()
		{
			// this content is extra long in order to determine how wide the task dialog can get.  I wonder if this text will be divided into multiple lines
			TaskDialog td = new TaskDialog("this is a title");
			td.MainInstruction = "Main Instruction";
			td.MainContent = "Main Content. this content is extra long in order to determine how wide the task dialog can get.  I wonder if this text will be divided into multiple lines.  this text has line breaks here\nand another two line breaks here\n\nand still another three line breaks to see\n\n\nif this will show all of these line breaks";
			td.MainIcon = TaskDialogIcon.TaskDialogIconInformation;
			td.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel | TaskDialogCommonButtons.None;
			td.ExpandedContent = "this is expanded content";
			td.FooterText = "this is footer text";
			td.DefaultButton = TaskDialogResult.Ok;

			TaskDialogResult result = td.Show();

			return result;
		}

		private TaskDialogResult stdTaskDialogMsg(string mainInst, string mainCont,
			TaskDialogCommonButtons btns = TaskDialogCommonButtons.None)
		{
			return stdTaskDialogMsg(TD_CUSTOM, mainInst, mainCont, btns);
		}

		private TaskDialogResult stdTaskDialogMsg(  TdKey which, string mainInst = "", string mainCont = "", TaskDialogCommonButtons btns = TaskDialogCommonButtons.None)
		{
			string mainContent = "";
			string expContent = "";

			if (!mainCont.IsVoid())
			{
				mainContent = mainCont;
			}
			else if (TdData[which].Item3 != null && TdData[which].Item3!.Length > 0)
			{
				foreach (string s in TdData[which].Item3!)
				{
					mainContent += s;
				}
			}

			if (TdData[which].Item4 != null && TdData[which].Item4!.Length > 0)
			{
				foreach (string s in TdData[which].Item4!)
				{
					expContent += s;
				}
			}


			TaskDialog td = new TaskDialog(TdData[which].Item1);
			td.MainInstruction = mainInst.IsVoid() ? TdData[which].Item2 : mainInst;
			td.MainContent = mainContent;
			td.ExpandedContent = expContent;
			td.CommonButtons = btns == TaskDialogCommonButtons.None ?  TdData[which].Item5 : btns;
			td.DefaultButton = TdData[which].Item6;
			td.MainIcon = TdData[which].Item7;

			return td.Show();
		}

	#endregion

	#region object id list management

		public int AddObjId(string who)
		{
			int id = AppRibbon.objectIdx++;

			List<int>? objIds;

			if (ObjectIdList.TryGetValue(who, out objIds))
			{
				if (objIds.Count < 5)
				{
					objIds.Add(id);
					ObjectIdList[who] = objIds;
				}
			}
			else
			{
				// does not contain the key
				objIds = [ id ];
				ObjectIdList.Add(who, objIds);
			}

			return id;
		}

		public bool HasObjIdFor(string who)
		{
			return ObjectIdList.ContainsKey(who);
		}

		public int GetFirstObjIdFor(string who)
		{
			if (HasObjIdFor(who) && ObjectIdList[who].Count > 0)
			{
				return ObjectIdList[who][0];
			}

			return -1;
		}

	#endregion

	#region events

	#endregion

	#region system overrides

		public override string ToString()
		{
			string objs = $"xmgr [{xMgr.ObjectId}] | xlib [{xLib.ObjectId}] | xdata [{xData.ObjectId}]";
			return $"{nameof(ExStorStartMgr)} [{ObjectId}] | {objs}";
		}

	#endregion

	#region test procedures

		/* processing switchboard */

		private bool t_switchBoardD()
		{
			bool result = false;

			key = makeKey();

			string opt = procIndex[key];

			switch (opt)
			{
			case "S":
			case "0":
				{
					proc = opt;
					ask = "-";
					p2 = "";

					pyn = proc.Equals("0") ? "good" : "-";

					result = true;
					break;
				}
			case "1":
				{
					proc = "1";

					if (t_askActivate())
					{
						if (t_proc_n(out pyn, out procNameyn)) result = true;
					}

					// result = t_proc_1();
					break;
				}
			case "2":
				{
					proc = "2";

					if (t_askRepair())
					{
						if (t_proc_b(out pyn, out procNameyn)) result = true;
					}

					// result = t_proc_2();
					break;
				}
			case "3":
				{
					proc = "3";

					if (t_askUpgrade())
					{
						if (t_proc_b(out pyn, out procNameyn))
						{
							if (t_proc_cs(out p2, out procName2)) result = true;
						}
					}

					// result = t_proc_3();
					break;
				}
			case "4":
				{
					proc = "4";

					if (t_askUpgrade())
					{
						if (t_proc_cs(out pyn, out procNameyn)) result = true;
					}

					// result = t_proc_4();
					break;
				}
			case "6":
				{
					proc = "6";

					if (t_askFixModelInfo())
					{
						if (t_proc_d(out pyn, out procNameyn)) result = true;
					}

					// result = t_proc_6();
					break;
				}
			case "7":
				{
					proc = "7";

					if (t_askRepair())
					{
						if (t_proc_cs(out pyn, out procNameyn))
						{
							if (t_proc_d(out p2, out procName2)) result = true;
						}
					}

					// result = t_proc_7();
					break;
				}
			case "O":
				{
					proc = "O";
					ask = "-";
					pyn = "-";
					result = false;
					break;
				}
			case "I":
				{
					proc = "I";
					ask = "-";
					pyn = "-";
					result = false;
					break;
				}
				default:
				{
					proc = $"NONE (failed)";
					result = false;
					break;
				}
			}

			ans = result;

			procMatch(result);

			return true;
		}

		/* processing methods */

		private bool t_proc_n(out string answer, out string pName)
		{
			pName = "make new";
			answer = "n";
			return true;
		}

		private bool t_proc_cs(out string answer, out string pName)
		{
			pName  = "fix sht ver";
			answer = "cs";
			return true;
		}

		private bool t_proc_d(out string answer, out string pName)
		{
			pName  = "fix mod name";
			answer = "d";
			return true;
		}

		private bool t_proc_b(out string answer, out string pName)
		{
			pName  = "new wbk";
			answer = "b";
			return true;
		}

		private bool t_askActivate()
		{
			ask = "an";
			askName = "activate";

			return true;
		}

		private bool t_askUpgrade()
		{
			ask = "au";
			askName = "upgrade";

			return true;
		}

		private bool t_askFixModelInfo()
		{
			ask = "am";
			askName = "mod info";

			return true;
		}

		private bool t_askRepair()
		{
			ask = "ar";
			askName = "repair";

			return true;
		}

		private struct tstResults
		{
			public string Id { get; set; }
			public string Procedure { get; set; }
			public string Path { get; set; }
			public string WhichAsk { get; set; }
			public string IfYesNo { get; set; }
			public string IfYes2 { get; set; }
			public bool Result { get; set; }

			public tstResults(    string id, string procedure, string path,
				string whichAsk, string ifYesNo, string ifYes2, bool result)
			{
				Id = id;
				Procedure = procedure;
				Path = path;
				WhichAsk = whichAsk;
				IfYesNo = ifYesNo;
				IfYes2 = ifYes2;
				Result = result;
			}
		}

		// private string answer;
		private tstResults tstRes;

		private string? askName;
		private string? procNameyn;
		private string? procName2;

		private string? ask;

		private int key;
		private string? proc;
		private string? pyn;
		private string? p2;
		private bool ans;

		public void testStart()
		{
			reCat();

			// showlist1();
			//
			// showList2();


			foreach ((ValidateSchema item1, ValidateDataStorage item2, ValidateSchema item3, ValidateDataStorage item4, tstResults item5) in _tstData1)
			{
				StringBuilder result = new StringBuilder();

				askName = "";
				proc    = "";
				ask     = "";
				pyn     = "";
				p2      = "";
				ans     = false;

				tstRes = item5;

				xMui.WbkScResCode = item1;
				result.Append(ExStorConst.ValidateSchemaDesc[item1].Item3).Append(" / ");

				xMui.WbkDsResCode = item2;
				result.Append(ExStorConst.ValidateDataStorageDesc[item2].Item3).Append(" / ");

				xMui.ShtScResCode = item3;
				result.Append(ExStorConst.ValidateSchemaDesc[item3].Item3).Append(" / ");

				xMui.ShtDsResCode = item4;
				result.Append(ExStorConst.ValidateDataStorageDesc[item4].Item3);

				Msgs.Write($"test {tstRes.Id,-5} | {result.ToString()} | ");

				// switchBoardC();
				t_switchBoardD();
			}
		}

		//                key   list of proc
		private Dictionary<int, List<string>> keyProc;
		private Dictionary<int, string> procIndex;
		private Dictionary<string, List<int>> keyIdx;

		private void reCat()
		{
			keyProc = new ();
			procIndex = new ();
			keyIdx = new ();

			foreach ((ValidateSchema sw, ValidateDataStorage dw, ValidateSchema ss, ValidateDataStorage ds, tstResults t) in _tstData1)
			{
				int key = getKey(sw, dw, ss, ds);

				string val = t.Procedure;

				if (keyProc.TryGetValue(key, out List<string>? procs))
				{
					procs.Add(val);
					keyProc[key] = procs;
				}
				else
				{
					keyProc.Add(key, [ val ]);

					procIndex.Add(key, val);
				}

				if (keyIdx.TryGetValue(val, out List<int>? idxs))
				{
					idxs.Add(key);
					keyIdx[val] = idxs;
				}
				else
				{
					keyIdx.Add(val, [ key ]);
				}
			}
		}

		private readonly Tuple<ValidateSchema, ValidateDataStorage, ValidateSchema, ValidateDataStorage, tstResults>[] _tstData1 =
		[
			// 	wbk sc		        wbk ds			       sht sc			       sht ds	                            id   proc  path     ask   pyn      p2     ans
			new (VSC_GOOD         , VDS_GOOD	         , VSC_MISSING            , VDS_MISSING         , new tstResults("1",  "S" , "G-A"  , "-"  , "-"    , ""   , true)),  // g/g   - A //
			new (VSC_GOOD         , VDS_GOOD			 , VSC_MISSING            , VDS_INVALID         , new tstResults("5",  "1" , "G-B"  , "an" , "n"    , ""   , true)),  // g/mn  - B //
			new (VSC_GOOD         , VDS_GOOD             , VSC_GOOD               , VDS_GOOD            , new tstResults("10", "0" , "G-C"  , "-"  , "good" , ""   , true)),  // g/g   - C //
			new (VSC_GOOD         , VDS_GOOD             , VSC_GOOD               , VDS_MISSING         , new tstResults("13", "S" , "G-E"  , "-"  , "-"    , ""   , true)),  // g/g   - E //
			new (VSC_GOOD         , VDS_GOOD             , VSC_WRONG_VER          , VDS_WRONG_VER       , new tstResults("15", "4" , "G-D"  , "au" , "cs"   , ""   , true)),  // g/g   - D //
																																														   //
			new (VSC_GOOD         , VDS_ACT_IGNORE       , VSC_VRFY_UNTESTED      , VDS_VRFY_UNTESTED   , new tstResults("20", "I" , "H-any", "-"  , "-"    , ""   , false)), // g/ai  - I //
																																														   //
			new (VSC_GOOD         , VDS_ACT_OFF          , VSC_VRFY_UNTESTED      , VDS_VRFY_UNTESTED   , new tstResults("25", "O" , "I-any", "-"  , "-"    , ""   , false)), // g/ai  - O //
																																														   //
			new (VSC_GOOD         , VDS_WRONG_MODEL_NAME , VSC_MISSING            , VDS_MISSING         , new tstResults("30", "1" , "J-A"  , "an" , "n"    , ""   , true)),  // g/mn  - A //
			new (VSC_GOOD         , VDS_WRONG_MODEL_NAME , VSC_MISSING            , VDS_INVALID         , new tstResults("35", "1" , "J-B"  , "an" , "n"    , ""   , true)),  // g/mn  - B //
			new (VSC_GOOD         , VDS_WRONG_MODEL_NAME , VSC_GOOD               , VDS_GOOD            , new tstResults("40", "6" , "J-C"  , "am" , "d"    , ""   , true)),  // g/mn  - C //
			new (VSC_GOOD         , VDS_WRONG_MODEL_NAME , VSC_GOOD               , VDS_MISSING         , new tstResults("43", "6" , "J-E"  , "am" , "d"    , ""   , true)),  // g/g   - E //
			new (VSC_GOOD         , VDS_WRONG_MODEL_NAME , VSC_WRONG_VER          , VDS_WRONG_VER       , new tstResults("45", "7" , "J-D"  , "ar" , "cs"   , "d"  , true)),  // g/mn  - D //
																																														   //
			new (VSC_GOOD         , VDS_MULTIPLE         , VSC_VRFY_UNTESTED      , VDS_VRFY_UNTESTED   , new tstResults("50", "O" , "K-Any", "-"  , "-"    , ""   , false)), // g/mx  - x //
																																														   //
			new (VSC_GOOD         , VDS_MISSING	         , VSC_GOOD               , VDS_MISSING         , new tstResults("53", "1" , "R-E"  , "an" , "n"    , ""   , true)),  // g/g   - B //
																																														   //
			new (VSC_MISSING      , VDS_MISSING          , VSC_MISSING            , VDS_MISSING         , new tstResults("55", "1" , "L-A"  , "an" , "n"    , ""   , true)),  // ms/ms - A //
			new (VSC_MISSING      , VDS_MISSING          , VSC_MISSING            , VDS_INVALID         , new tstResults("60", "1" , "L-B"  , "an" , "n"    , ""   , true)),  // ms/ms - B //
			new (VSC_MISSING      , VDS_MISSING          , VSC_GOOD               , VDS_GOOD            , new tstResults("65", "2" , "L-C"  , "ar" , "b"    , ""   , true)),  // ms/ms - C //
			new (VSC_MISSING      , VDS_MISSING          , VSC_GOOD               , VDS_MISSING         , new tstResults("68", "2" , "L-E"  , "ar" , "b"    , ""   , true)),  // ms/ms - E //
			new (VSC_MISSING      , VDS_MISSING          , VSC_WRONG_VER          , VDS_WRONG_VER       , new tstResults("70", "3" , "L-D"  , "au" , "b"    , "cs" , true)),  // ms/ms - D //
																																														   //
			new (VSC_MISSING      , VDS_INVALID          , VSC_MISSING            , VDS_MISSING         , new tstResults("75", "1" , "M-A"  , "an" , "n"    , ""   , true)),  // ms/ms - A //
																																														   //
			new (VSC_INVALID      , VDS_WRONG_VER        , VSC_MISSING            , VDS_MISSING         , new tstResults("80", "1" , "N-A"  , "an" , "n"    , ""   , true)),  // ms/ms - A //
			new (VSC_INVALID      , VDS_WRONG_VER        , VSC_MISSING            , VDS_INVALID         , new tstResults("85", "1" , "N-B"  , "an" , "n"    , ""   , true)),  // ms/ms - B //
			new (VSC_INVALID      , VDS_WRONG_VER        , VSC_GOOD               , VDS_GOOD            , new tstResults("90", "2" , "N-C"  , "ar" , "b"    , ""   , true)),  // ms/ms - C //
			new (VSC_INVALID      , VDS_WRONG_VER        , VSC_GOOD               , VDS_MISSING         , new tstResults("93", "2" , "N-E"  , "ar" , "b"    , ""   , true)),  // ms/ms - C //
			new (VSC_INVALID      , VDS_WRONG_VER        , VSC_WRONG_VER          , VDS_WRONG_VER       , new tstResults("95", "3" , "N-D"  , "au" , "b"    , "cs" , true)),  // ms/ms - D //
																																														   //
			new (VSC_WRONG_VER    , VDS_WRONG_VER        , VSC_MISSING            , VDS_MISSING         , new tstResults("100", "1", "P-A" , "an"  , "n"    , ""   , true)),  // ms/ms - A //
			new (VSC_WRONG_VER    , VDS_WRONG_VER        , VSC_MISSING            , VDS_INVALID         , new tstResults("105", "1", "P-B" , "an"  , "n"    , ""   , true)),  // ms/ms - B //
			new (VSC_WRONG_VER    , VDS_WRONG_VER        , VSC_GOOD               , VDS_GOOD            , new tstResults("110", "2", "P-C" , "ar"  , "b"    , ""   , true)),  // ms/ms - C //
			new (VSC_WRONG_VER    , VDS_WRONG_VER        , VSC_GOOD               , VDS_MISSING         , new tstResults("113", "2", "P-E" , "ar"  , "b"    , ""   , true)),  // ms/ms - C //
			new (VSC_WRONG_VER    , VDS_WRONG_VER        , VSC_WRONG_VER          , VDS_WRONG_VER       , new tstResults("115", "3", "P-D" , "au"  , "b"    , "cs" , true)),  // ms/ms - D //
		];

		private void procMatch(bool result)
		{
			bool resFinal = true;

			string res1 = tstRes.Procedure.Equals(proc) ? "YES" : "NO";
			resFinal = resFinal && (tstRes.Procedure.Equals(proc));

			string res2 = result == tstRes.Result ? "YES" : "NO";
			resFinal = resFinal && (result == tstRes.Result);

			string res2a = result ? "T" : "F";
			string res2b = tstRes.Result ? "T" : "F";
			res2 = $"{res2} ({res2a} vs {res2b})";

			string res4 = ask.Equals(tstRes.WhichAsk) ? "YES" : "NO";

			resFinal = resFinal && (ask.Equals(tstRes.WhichAsk));

			string res4a = $"({ask} vs {tstRes.WhichAsk})";
			res4 = $"{res4} {res4a,-10}";

			string res3 = $"pyn {$"{pyn.Equals(tstRes.IfYesNo)}",-6} {$"({tstRes.IfYesNo} vs {pyn})",-16}  {$"[{procNameyn}]", -14}";

			resFinal = resFinal && (pyn.Equals(tstRes.IfYesNo));

			if (result)
			{
				if (!tstRes.IfYes2.IsVoid())
				{
					res3 = $"{res3} | p2 {$"{p2.Equals(tstRes.IfYes2)}",-6} {$"({tstRes.IfYes2} vs {p2})",-16}   {$"[{procName2}]", -14}";
				}
				else
				{
					res3 = $"{res3} | p2 (n/a)";
				}
			}

			string finalAnswer = resFinal ? "** GOOD" : "failed";

			StringBuilder sb = new StringBuilder();
			sb.Append($"key {key,-6}| {finalAnswer,-10} | ");
			sb.Append($"ask {askName,-10} | ");
			sb.Append($"procedure {proc,-2} {$"(vs {tstRes.Procedure})", -7} {res1,-5} | ");
			sb.Append($"path {tstRes.Path,-6} | ");
			sb.Append($"result match? {res2,-15} | ");
			sb.Append($"ask match? {res4,-15} | ");
			sb.Append($"results {res3}");




			Msgs.WriteLine(sb.ToString());
		}

		private void showlist1()
		{
			Msgs.WriteLine($"number index & associated procedures");

			foreach ((int i, List<string>? value) in keyProc)
			{
				Msgs.Write($"** idx  {i,-5} | ");

				foreach (string s in value)
				{
					Msgs.Write($"{s,-4}");
				}

				Msgs.WriteLine("");
			}
		}

		private void showList2()
		{
			Msgs.WriteLine($"proc index & associated indices");

			foreach ((string? s, List<int>? value) in keyIdx)
			{
				Msgs.Write($"** proc {s,-5} | ");

				foreach (int i in value)
				{
					Msgs.Write($"{i,-6}");
				}

				Msgs.WriteLine("");
			}
		}


	#endregion


	#region saved

				/* voided
		private bool t_switchBoardE()
		{
			bool result;
		
			key = makeKey();
		
			string opt = procIndex[key];
		
			switch (opt)
			{
			case "0":
				{
					proc = "0";
					ask = "-";
					pyn = "good";
					p2 = "";
					result = true;
					break;
				}
			case "1":
				{
					result = t_proc_1();
					break;
				}
			case "2":
				{
					result = t_proc_2();
					break;
				}
			case "3":
				{
					result = t_proc_3();
					break;
				}
			case "4":
				{
					result = t_proc_4();
					break;
				}
			case "6":
				{
					result = t_proc_6();
					break;
				}
			case "7":
				{
					result = t_proc_7();
					break;
				}
			case "O":
				{
					result = t_proc_Off();
					break;
				}
			case "I":
				{
					result = t_proc_Ignore();
					break;
				}
				default:
				{
					proc = $"NONE (failed)";
					result = false;
					break;
				}
			}
		
			ans = result;
		
			procMatch(result);
		
			return true;
		}
		
		private bool t_proc_Ignore()
		{
			proc = "I";
			ask = "-";
			pyn = "-";
			// p2 = "";
			return false;
		}

		private bool t_proc_Off()
		{
			proc = "O";
			ask = "-";
			pyn = "-";
			// p2 = "";
			return false;
		}

		private bool t_proc_1()
		{
			if (!t_askActivate()) return false;

			proc = "1";
			if (!t_proc_n(out pyn, out procNameyn)) return false;
			// p2 = "";
			return true;
		}

		private bool t_proc_2()
		{
			if (!t_askRepair()) return false;

			proc = "2";

			if (!t_proc_b(out pyn, out procNameyn)) return false;
			// p2 = "";
			return true;
		}

		private bool t_proc_3()
		{
			if (!t_askUpgrade()) return false;

			proc = "3";

			if (!t_proc_b(out pyn, out procNameyn)) return false;

			if (!t_proc_cs(out p2, out procName2)) return false;
			return true;
		}

		private bool t_proc_4()
		{
			if (!t_askUpgrade()) return false;

			proc = "4";

			if (!t_proc_cs(out pyn, out procNameyn)) return false;
			// p2 = "";
			return true;
		}

		private bool t_proc_6()
		{
			if (!t_askFixModelInfo()) return false;

			proc = "6";

			if (!t_proc_d(out pyn, out procNameyn)) return false;
			// p2 = "";
			return true;
		}

		private bool t_proc_7()
		{
			if (!t_askRepair()) return false;

			proc = "7";

			if (!t_proc_cs(out pyn, out procNameyn)) return false;

			if (!t_proc_d(out p2, out procName2)) return false;
			return true;
		}
		*/



		// private bool switchBoardC()
		// {
		// 	// key = getKey(xMui.WbkScResCode, xMui.WbkDsResCode, xMui.ShtScResCode, xMui.ShtDsResCode);
		// 	key = makeKey();
		//
		// 	bool result;
		//
		// 	// 0
		// 	if (xMui.WbkDsResCode == VDS_GOOD &&
		// 		xMui.ShtDsResCode == VDS_GOOD)
		// 	{
		// 		proc = "0";
		// 		ask = "-";
		// 		pyn = "good";
		// 		p2 = "";
		// 		result = true;
		// 	}
		// 	else
		// 		// 5 through 7 - special cases before general cases
		//
		// 	if (xMui.WbkDsResCode == VDS_ACT_IGNORE)
		// 	{
		// 		result = t_proc_Ignore();
		// 	}
		// 	else
		// 		// X, 6, 7
		// 		// wbk sc is good
		// 	if (xMui.WbkDsResCode == VDS_ACT_OFF
		// 		|| xMui.WbkDsResCode == VDS_MULTIPLE )
		// 	{
		// 		result = t_proc_Off();
		// 	}
		// 	else
		// 		// 6
		// 	if (xMui.WbkDsResCode == VDS_WRONG_MODEL_NAME
		// 		&& xMui.ShtDsResCode == VDS_GOOD)
		// 	{
		// 		result = t_proc_6();
		// 	}
		// 	else
		// 		// 7
		// 	if (xMui.WbkDsResCode == VDS_WRONG_MODEL_NAME
		// 		&& xMui.ShtDsResCode == VDS_WRONG_VER)
		// 	{
		// 		result = t_proc_7();
		// 	}
		// 	else
		//
		// 		// 1 through 4 - general cases
		//
		// 		// 1
		// 		// if (xMui.ShtScResCode == VSC_MISSING)
		// 	if (xMui.ShtDsResCode == VDS_INVALID)
		// 	{
		// 		result = t_proc_1();
		// 	}
		// 	else
		// 		// 1
		// 		// if (xMui.ShtScResCode == VSC_MISSING)
		// 	if (xMui.ShtDsResCode == VDS_MISSING)
		// 	{
		// 		result = t_proc_1();
		// 	}
		// 	else
		// 		// 2
		// 	if (xMui.WbkDsResCode != VDS_GOOD
		// 		&& xMui.ShtDsResCode == VDS_GOOD )
		// 	{
		// 		result = t_proc_2();
		// 	}
		// 	else
		// 		// 3
		// 		// != VDS_GOOD means,  
		// 	if (xMui.WbkDsResCode != VDS_GOOD
		// 		&& xMui.ShtDsResCode == VDS_WRONG_VER )
		// 	{
		// 		result = t_proc_3();
		// 	}
		// 	else
		// 		// 4
		// 	if (xMui.WbkDsResCode == VDS_GOOD
		// 		&& xMui.ShtDsResCode == VDS_WRONG_VER )
		// 	{
		// 		result = t_proc_4();
		// 	}
		// 	else
		// 	{
		// 		proc = $"NONE (failed)";
		// 		result = false;
		// 	}
		//
		// 	procMatch(result);
		//
		// 	return result;
		// }





		// private bool switchBoardB()
		// {
		// 	bool? result;
		//
		// 	// activation ignore - do nothing
		// 	if (xMui.WbkDsResCode == VDS_ACT_IGNORE) return false;
		//
		// 	if (!ask()) return false;
		//
		// 	// preface - setup for the following procedures
		// 	// B ( & F)
		// 	xMgr.ResetData();
		//
		// 	// process workbook procedures
		// 	result = procWbk2();
		//
		// 	if (result != false)
		// 	{
		// 		// process sheet procedures
		// 		result = procSht2();
		// 	}
		//
		// 	// clear temp data
		// 	xData.ResetTemp();
		//
		// 	// suffix - set final status
		// 	return xMgr.SetStartResultStatus(result);
		// }
		//
		// private bool? procWbk2()
		// {
		// 	if (xMui.WbkDsResCode == VDS_GOOD)
		// 	{
		// 		return xMgr.ProcTransWbk();
		// 	}
		//
		// 	if (xMui.WbkDsResCode == VDS_MISSING || xMui.WbkDsResCode == VDS_INVALID)
		// 	{
		// 		return xMgr.ProcCreateAndWriteWbk();
		// 	}
		//
		// 	if (xMui.WbkDsResCode == VDS_WRONG_MODEL_NAME)
		// 	{
		// 		return xMgr.ProcFixModelName();
		// 	}
		//
		// 	if (xMui.WbkDsResCode == VDS_ACT_OFF)
		// 	{
		// 		return xMgr.ProcFixActivationOff();
		// 	}
		//
		// 	return false;
		// }
		//
		// private bool? procSht2()
		// {
		// 	if (xMui.ShtDsResCode == VDS_GOOD)
		// 	{
		// 		return xMgr.ProcTransSht();;
		// 	}
		//
		// 	if (xMui.ShtDsResCode == VDS_MISSING || xMui.ShtDsResCode == VDS_INVALID)
		// 	{
		// 		return xMgr.ProcCreateSht();
		// 	}
		//
		// 	// if (xMui.ShtDsResCode == VDS_WRONG_MODEL_CODE)
		// 	// {
		// 	// 	Msgs.WriteLine("fix wrong model code");
		// 	// 	return true;
		// 	// }
		// 	return false;
		// }


		// private bool switchBoardA()
		// {
		// 	// activation ignore - do nothing
		// 	if (xMui.WbkDsResCode == VDS_ACT_IGNORE) return false;
		//
		// 	// normal
		// 	if (xMui.WbkDsResCode == VDS_GOOD &&
		// 		(xMui.ShtDsResCode == VDS_GOOD ||
		// 		xMui.ShtDsResCode == VDS_MISSING ||
		// 		xMui.ShtDsResCode == VDS_INVALID ))
		// 	{
		// 		// sequences 1, 2, 3
		// 		return procStartNormal();
		// 	}
		//
		// 	// activalt the system
		// 	if ((xMui.WbkDsResCode == VDS_MISSING ||
		// 		xMui.WbkDsResCode == VDS_INVALID) &&
		// 		(xMui.ShtDsResCode == VDS_MISSING ||
		// 		xMui.ShtDsResCode == VDS_INVALID) )
		// 	{
		// 		// sequences 12, 13 , 22, 23, 103, 103
		// 		return procStartActivate();
		// 	}
		//
		// 	// wrong model code
		// 	if (xMui.ShtDsResCode == VDS_WRONG_MODEL_CODE)
		// 	{
		// 		// sequences 4
		// 		return procWrongModelCode();
		//
		// 	}
		//
		// 	return false;
		// }
		//
		// /// <summary>
		// /// procedure, startup normal<br/>
		// /// covers, all good, wbk good + sht good but no sheets
		// /// </summary>
		// /// <returns></returns>
		// private bool procStartNormal()
		// {
		// 	bool? result;
		//
		// 	// preface
		// 	// B (& F)
		// 	xMgr.ResetData();
		// 	
		// 	// St/Tt
		// 	result = xMgr.ProcTransWbk();
		//
		// 	if (result != false)
		// 	{
		// 		// Ut/Vt
		// 		result = xMgr.ProcTransSht();
		// 	}
		//
		// 	// suffix
		// 	// P or Q or fail
		// 	return xMgr.SetStartResultStatus(result);
		// }
		//
		// private bool procStartActivate()
		// {
		// 	bool? result;
		//
		// 	// ask?
		// 	if (!askActivate()) return xMgr.SetStartResultStatus(-2);
		//
		// 	// preface
		// 	// B (& F)
		// 	xMgr.ResetData();
		//
		// 	// Sc / Tc / W
		// 	result = xMgr.ProcCreateAndWriteWbk();
		//
		// 	if (result == true)
		// 	{
		// 		// Uc/Tc/W
		// 		result = xMgr.ProcCreateSht();
		// 	}
		//
		// 	return xMgr.SetStartResultStatus(result);
		// }
		//
		// private bool procWrongModelCode()
		// {
		// 	// ask?
		// 	if (!askFixModelInfo()) return xMgr.SetStartResultStatus(-2);
		//
		// 	// preface
		// 	// B (& F)
		// 	xMgr.ResetData();
		//
		// 	if (!procWbk()) return false;
		//
		//
		// 	return true;
		// }
		//
		// private bool procWbk()
		// {
		// 	// options:
		// 	// St/Tt | St/Tc/W | St/G/W | St/C/W
		//
		// 	if (xMui.WbkDsResCode == VDS_GOOD) return xMgr.ProcTransWbk() == true;
		// 	if (xMui.WbkDsResCode == VDS_MISSING || xMui.WbkDsResCode == VDS_INVALID) return xMgr.ProcCreateAndWriteWbk() == true;
		// 	
		// 	if (xMui.WbkDsResCode == VDS_WRONG_MODEL_NAME) return false;
		// 	if (xMui.WbkDsResCode == VDS_ACT_OFF) return false;
		//
		//
		// 	return false;
		// }



		/*
private bool askModelInfo()
{
	TaskDialogResult tdRes = stdTaskDialogMsg(TD_MODEL_INFO_WRONG);

	if (tdRes == TaskDialogResult.No)
	{
		OnPropChgdExs(ES_START_DONE_EXIT);
		OnPropChgdRn(RunningStatus.RN_DEACTIVATE);
		return false;
	}



	return true;
}

/// <summary>
/// the ExSystem is not active on this system - request add?<br/>
/// if yes, activate the system.<br/>
/// returns ES_START_DONE_FAIL if activation fails<br/>
/// returns ES_START_DONE_DEACTIVATE if user says no, and<br/>
///	➜ sets running status to RN_NOT_RUNNING<br/>
/// returns ES_START_DONE_GOOD if activation works, and<br/>
/// ➜ sets running status to RN_RUNNING_NEED_SHT<br/>
/// </summary>
/// <returns></returns>
private ExSysStatus activateSystem()
{
	if (!askActivate())
	{
		OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(RunningStatus.RN_DEACTIVATE));
		return ES_START_DONE_DEACTIVATE;
	}

	// proceed and activate the system

	if (!xMgr.ActivateSystem()) return ES_START_DONE_FAIL;

	rs = RunningStatus.RN_RUNNING_NEED_SHT;

	OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(rs));

	return ES_START_DONE_GOOD;
}

/// <summary>
/// the activate status flag has been set to deactivate for this model<br/>
/// either deactivate or ignore
/// </summary>
/// <returns></returns>
private ExSysStatus reActivateSystem()
{
	bool? result = false;

	ExSysStatus status = ES_START_DONE_GOOD;

	tdMsgXtra = ExStorConst.ValidateDataStorageDesc[xMui.WbkDsResCode].Item1;

	string mainInst = string.Format(TdData[TD_REACTIVATE].Item2, tdMsgXtra);

	if (stdTaskDialogMsg(TD_REACTIVATE, mainInst) != TaskDialogResult.Yes)
	{
		OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(RunningStatus.RN_DEACTIVATE));
		return ES_START_DONE_DEACTIVATE;
	}

	// result = xMgr.ConfigAllFromTemp();

	if (result == false)
	{
		OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(RunningStatus.RN_CANNOT_RUN_FAIL));

		return ES_START_DONE_FAIL;
	}

	if (!xMgr.ReadWorkBook())
	{
		OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(RunningStatus.RN_CANNOT_RUN_FAIL));
		return ES_START_DONE_FAIL;
	}

	if (!xMgr.ReadSheets())
	{
		OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(RunningStatus.RN_CANNOT_RUN_FAIL));
		return ES_START_DONE_FAIL;
	}

	if (xMgr.UpdateWbkField(WorkBookFieldKeys.PK_AD_STATUS,
			new DynaValue(ActivateStatus.AS_INACTIVE)))
	{
		OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(RunningStatus.RN_RUNNING_NORMAL));
		return ES_START_DONE_GOOD;
	}

	rs = RunningStatus.RN_CANNOT_RUN_FAIL;

	OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(rs));

	return ES_START_DONE_FAIL;
}

// /// <summary>
// /// Normal startup configuration of all parts - if possible<br/>
// /// true - all parts read and exist<br/>
// /// false - something did not work correct<br/>
// /// null - ok except that needs sheets
// /// </summary>
// private bool procAllGood()
// {
// 	bool? result = xMgr.ConfigAllFromTemp();
//
// 	SetStartResultStatus(result);
//
// 	return result ==  true;
// }

private bool preProcess()
{
	bool result = true;

	if (xMui.WbkDsResCode == VDS_WRONG_MODEL_NAME || xMui.WbkDsResCode == VDS_WRONG_MODEL_CODE)
	{
		TaskDialogResult tdRes = stdTaskDialogMsg(TD_MODEL_INFO_WRONG);

		if (tdRes == TaskDialogResult.No)
		{
			OnPropChgdExs(ES_START_DONE_EXIT);
			OnPropChgdRn(RunningStatus.RN_DEACTIVATE);
			result = false;
		}
	}
	else if (xMui.WbkDsResCode == VDS_ACT_OFF)
	{
		TaskDialogResult tdRes = stdTaskDialogMsg(TD_REACTIVATE);

		if (tdRes == TaskDialogResult.No)
		{
			OnPropChgdExs(ES_START_DONE_EXIT);
			OnPropChgdRn(RunningStatus.RN_DEACTIVATE);
			result = false;
		}
	}
	else if (xMui.WbkDsResCode == VDS_ACT_IGNORE)
	{
		result = false;
	}

	return result;
}

private bool procModelName()
{
	// process:
	// B: clear the old data
	// S: transfer wbk schema
	// G: transfer & repair wbk ds
	// U: transfer sheet schema
	//		V: transfer sheet ds(s) ->
	//		Q: set status & flag good
	// or
	// U: create sheet schema
	//		F: init sheet ds list
	//		P: set status & flag good but need sheets

	// proc B
	xMgr.ResetData();

	// proc S
	if (!xMgr.TransTempWbkObjectsToXdata()) { return false; }


	return true;
}

private bool? SetStartResultStatus(bool? result)
{
	if (result == false)
	{
		OnPropChgdExs(ES_START_DONE_EXIT);
		OnPropChgdRn(RunningStatus.RN_CANNOT_RUN_FAIL);
	}
	else
	if (result == true)
	{
		OnPropChgdExs(ES_START_DONE_GOOD);
		OnPropChgdRn(RunningStatus.RN_RUNNING_NORMAL);
	}
	else
	{
		OnPropChgdExs(ES_START_DONE_GOOD);
		OnPropChgdRn(RunningStatus.RN_RUNNING_NEED_SHT);
	}

	return result;
}
*/

		// /// <summary>
		// /// normal configuration of all parts (that can be set)<br/>
		// /// sets running status to RN_RUNNING_NORMAL if all good<br/>
		// ///		and returns ES_START_DONE_GOOD<br/>
		// /// sets running status to RN_CANNOT_RUN_FAIL if the temp transfer failed<br/>
		// ///		and returns ES_START_DONE_FAIL<br/>
		// /// returns ES_START_DONE_FAIL if launch code is not LC_DONE_GOOD
		// /// </summary>
		// private ExSysStatus configNormal()
		// {
		// 	bool? result;
		//
		// 	if (xMui.LaunchCode != LaunchCode.LC_DONE_GOOD) return ES_START_DONE_FAIL;
		//
		// 	result = xMgr.ConfigAllFromTemp();
		//
		// 	if (result == false)
		// 	{
		// 		OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(RunningStatus.RN_CANNOT_RUN_FAIL));
		//
		// 		return ES_START_DONE_FAIL;
		// 	}
		//
		// 	// update status
		// 	// running
		// 	rs = RunningStatus.RN_RUNNING_NORMAL;
		// 	OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(rs));
		//
		// 	return ES_START_DONE_GOOD;
		// }

		// op A


		// private void processRtnStat(ExSysStatus status, TdKey tdMsg) //, ExSysStatus tmp1, LaunchCode  tmp2)
		// {
		// 	if (status == ES_START_DONE_GOOD)
		// 	{
		// 		setStatus();
		// 		Mw.ShowDialog();
		// 	}
		// 	{
		// 		tdMsg = TD_CONFIG_FAIL;
		// 		statDialog("start op fail");
		// 		OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(RunningStatus.RN_CANNOT_RUN_FAIL));
		// 	}
		//
		//
		// 	if (tdMsg != TD_NA)
		// 	{
		// 		stdTaskDialogMsg(tdMsg);
		// 	}
		// }


		// public void StartOpr()
		// {
		// 	Msgs.ShowDebug = true;
		// 	rs = RunningStatus.RN_NA;
		//
		// 	TdKey tdMsg;
		// 	ExSysStatus status= xMui.ExSysStatus;
		//
		// 	ExSysStatus tmp1 = xMui.ExSysStatus;
		// 	LaunchCode  tmp2 = xMui.LaunchCode;
		//
		// 	Msgs.CWriteLine($"\n\n*** Start Operator - Begin\n");
		//
		// 	showStatus1();
		// 	
		// 	clearStatus();
		//
		// 	if (xMui.ExSysStatus == ES_START_DONE_GOOD && xMui.LaunchCode == LaunchCode.LC_NA) { winOpen2ndPlus(); return; }
		// 	// if (xMui.ShtScResCode == VSC_GOOD &&
		// 	// 	xMui.ShtDsResCode == VDS_GOOD) { winOpen2ndPlus(); return; }
		//
		// 	/* status from launch manager / verify */
		// 	/* each must set the running status */
		//
		// 	Debug.WriteLine($"\nbefore switchBoardA | Exsysstatus {xMui.ExSysStatus} | LC {xMui.LaunchCode}");
		//
		// 	// process the launch results
		// 	status = switchBoardA(out tdMsg);
		//
		// 	Debug.WriteLine($"\nafter switchBoardA | status {status} | Exsysstatus {xMui.ExSysStatus} | LC {xMui.LaunchCode}");
		//
		// 	showStatus2(status, "\n\n","\n\n");
		//
		// 	// process return status
		// 	processRtnStat(status, tdMsg, tmp1, tmp2);
		//
		// 	Debug.WriteLine($"\nafter processRtnStat | status {status}");
		//
		// 	Msgs.CWriteLine($"\n*** Start Operator - End\n\n");
		// }

		// OLD
		// for a launch failure
		// -> exstorsys == ES_VRFY_DONE_FAIL / launch code == LC_DONE_INVALID
		//		* set running status to -> RN_CANNOT_RUN_FAIL
		// methods rules:
		// start with the result of exstorverify
		// routines return
		// -> ES_START_DONE_FAIL when start process did / cannot work
		//		* set running status to -> RN_CANNOT_RUN_FAIL
		//		* set launch code to -> LC_NA
		// -> ES_START_DONE_DEACTIVATE when start process connot continue because not activated
		//		* set running status to -> RN_DEACTIVATE
		//		* set launch code to -> LC_NA
		// -> ES_START_DONE_GOOD when start process COMPLETELY works (workbook & sheets)
		//		* set running status to -> RN_RUNNING_NORMAL
		//		* set launch code to -> LC_NA
		// -> ES_START_DONE_GOOD when start process PARTIALLY works (workbook but not sheets)
		//		* set running status to -> RN_RUNNING_NEED_SHT
		//		* set launch code to -> LC_NA

		// 		private ExSysStatus switchBoardA(out TdKey tdMsg)
		// 		{
		// 			tdMsg = TD_NA;
		//
		// 			ExSysStatus status= xMui.ExSysStatus;
		//
		// 			ExSysStatus tmp1 = xMui.ExSysStatus;
		// 			LaunchCode  tmp2 = xMui.LaunchCode;
		//
		// 			
		//
		// 			/* status from launch manager / verify */
		// 			/* each must set the running status */
		//
		// /* P05 - typical all found and good*/
		// 			if (xMui.ExSysStatus == ES_VRFY_DONE_GOOD && xMui.LaunchCode == LaunchCode.LC_DONE_GOOD)
		// 			{
		// 				pMsg("P05");
		// 				status = configNormal();
		//
		// 				if (status == ES_START_DONE_FAIL) tdMsg = TD_CONFIG_FAIL;
		// 			}
		// 			else
		// /* P10 / P15 - full create / activate the system */
		// 			if (
		// 				(xMui.ExSysStatus == ES_VRFY_DONE_RESTART &&
		// 				xMui.LaunchCode == LaunchCode.LC_DONE_RESTART &&
		// 				xMui.WbkScResCode == ValidateSchema.VSC_MISSING) ||
		//
		// 				(xMui.ExSysStatus == ES_VRFY_DONE_FAIL &&
		// 				xMui.LaunchCode == LaunchCode.LC_DONE_INVALID)
		// 				)
		// 			{
		// 				pMsg("P10 & P15");
		// 				status = activateSystem();
		//
		// 				if (status == ES_START_DONE_FAIL) tdMsg = TD_CONFIG_FAIL;
		// 			}
		// 			else
		// 			if (xMui.ExSysStatus == ES_VRFY_DONE_HOLD_ACT)
		// 			{
		// 				pMsg("P70");
		// 				status = reActivateSystem();
		//
		// 				if (status == ES_START_DONE_FAIL) tdMsg = TD_CONFIG_FAIL;
		// 			}
		// 			
		// 			else
		// 			if (xMui.ExSysStatus == ES_VRFY_DONE_RESTART &&
		// 				xMui.LaunchCode == LaunchCode.LC_DONE_RESTART) 
		// 			{
		// 				if (xMui.WbkScResCode == VSC_MISSING)
		// 				{
		// 					Msgs.CWriteLine($"*** operation (A) ***");
		//
		// 					status = ES_START_DONE_GOOD;
		// 					rs = RunningStatus.RN_RUNNING_NEED_SHT;
		//
		// 					xMui.ExSysStatus = ES_START_DONE_GOOD;
		// 					xMui.LaunchCode = LaunchCode.LC_DONE_GOOD;
		//
		// 					OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(rs));
		// 				}
		// 				else
		// 				{
		// 					Msgs.CWriteLine($"*** operation (G) ***");
		//
		// 					
		// 					status = ES_START_DONE_GOOD;
		// 					rs = RunningStatus.RN_RUNNING_NEED_SHT;
		//
		// 					xMui.ExSysStatus = ES_START_DONE_GOOD;
		// 					xMui.LaunchCode = LaunchCode.LC_DONE_GOOD;
		//
		// 					OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(rs));
		// 				}
		// 			}
		//
		// 			else
		// 			if (xMui.ExSysStatus == ES_VRFY_DONE_HOLD_SHT &&
		// 				xMui.LaunchCode == LaunchCode.LC_DONE_GOOD) 
		// 			{
		// 				Msgs.CWriteLine($"*** operation (B) ***");
		//
		// 				status = ES_START_DONE_GOOD;
		// 				rs = RunningStatus.RN_RUNNING_NEED_SHT;
		// 				OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(rs));
		// 			}
		//
		// 			else
		// 			if (xMui.ExSysStatus == ES_VRFY_DONE_HOLD_WBK &&
		// 				xMui.LaunchCode == LaunchCode.LC_DONE_RESTART) 
		// 			{
		// 				if (xMui.ShtDsResCode != VDS_GOOD)
		// 				{
		// 					Msgs.CWriteLine($"*** operation (E) ***");
		//
		// 					status = ES_START_DONE_GOOD;
		// 					rs = RunningStatus.RN_RUNNING_NEED_SHT;
		// 				
		// 					xMui.ExSysStatus = ES_START_DONE_GOOD;
		// 					xMui.LaunchCode = LaunchCode.LC_DONE_GOOD;
		//
		// 					OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(rs));
		// 				}
		// 				else
		// 				{
		// 					Msgs.CWriteLine($"*** operation (F) ***");
		//
		// 					status = ES_START_DONE_GOOD;
		// 					rs = RunningStatus.RN_RUNNING_NORMAL;
		// 				
		// 					xMui.ExSysStatus = ES_START_DONE_GOOD;
		// 					xMui.LaunchCode = LaunchCode.LC_DONE_GOOD;
		//
		// 					OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(rs));
		// 				}
		// 			}
		// 			
		// 			else
		// 			if (xMui.ExSysStatus == ES_VRFY_DONE_RECONFIG &&
		// 				xMui.LaunchCode == LaunchCode.LC_DONE_INVALID) 
		// 			{
		// 				if (xMui.WbkDsResCode == VDS_WRONG_MODEL_NAME)
		// 				{
		// 					Msgs.CWriteLine($"*** operation (H) ***");
		//
		// 					status = ES_START_DONE_GOOD;
		// 					rs = RunningStatus.RN_RUNNING_NEED_SHT;
		// 					OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(rs));
		// 				}
		// 				else
		// 				if (xMui.WbkDsResCode == VDS_WRONG_MODEL_CODE)
		// 				{
		// 					Msgs.CWriteLine($"*** operation (I) ***");
		//
		// 					status = ES_START_DONE_GOOD;
		// 					OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(RunningStatus.RN_RUNNING_NORMAL));
		// 				}
		// 				else
		// 				{
		// 					Msgs.CWriteLine($"*** >>> INVALID <<< ***");
		//
		// 					status = ES_START_DONE_FAIL;
		// 					rs = RunningStatus.RN_CANNOT_RUN_FAIL;
		// 					OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(rs));
		//
		// 					showStatus1();
		// 				}
		// 			}
		//
		// 			else
		// 			{
		// 				Msgs.CWriteLine($"*** >>> unknown status sequence INVALID <<< ***");
		//
		// 				status = ES_START_DONE_FAIL;
		// 				rs = RunningStatus.RN_CANNOT_RUN_FAIL;
		// 				OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(rs));
		//
		// 				showStatus1();
		// 			}
		//
		// 			return status;
		// 		}
		// 		private void processRtnStat(ExSysStatus status, TdKey tdMsg) //, ExSysStatus tmp1, LaunchCode  tmp2)
		// 		{
		// 			if (status == ES_START_DONE_GOOD)
		// 			{
		// 				setStatus();
		// 				Mw.ShowDialog();
		// 			}
		// 			// else
		// 			// if (status == ES_START_DONE_DEACTIVATE)
		// 			// {
		// 			// 	xMui.ExSysStatus = tmp1;
		// 			// 	xMui.LaunchCode  = tmp2;
		// 			// }
		// 			else
		// 			{
		// 				tdMsg = TD_CONFIG_FAIL;
		// 				statDialog("start op fail");
		// 				OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(RunningStatus.RN_CANNOT_RUN_FAIL));
		// 			}
		//
		//
		// 			if (tdMsg != TD_NA)
		// 			{
		// 				stdTaskDialogMsg(tdMsg);
		// 			}
		// 		}
		// //
		// 		/// <summary>
		// 		/// process items 25.1, 25.2, and 30
		// 		/// </summary>
		// 		/// <returns></returns>
		// 		private ExSysStatus switchBoard2()
		// 		{
		// 			bool result;
		// 			ExSysStatus status = ES_START_DONE_GOOD;
		//
		// 			if (xMui.LaunchCode != LaunchCode.LC_DONE_RESTART ||
		// 				xMui.WbkScResCode != VSC_GOOD ||
		// 				xMui.ShtScResCode != VSC_GOOD) return ES_START_DONE_FAIL;
		//
		// 			/* P25 .1 or .2 ? */	
		// 			if (xMui.WbkDsResCode == VDS_MISSING)
		// 			{
		// 				/* P25.1 */	
		// 				if (xMui.ShtDsResCode == VDS_MISSING)
		// 				{
		// 					pMsg("P25.1");
		// 					// both missing - probably system never activated but schema
		// 					// left over from a prior model - just activate here if the
		// 					// user says yes
		// 					status = activateSystem();
		//
		// 					// update status
		// 					// running
		// 					if (status == ES_START_DONE_GOOD)
		// 						OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(RunningStatus.RN_RUNNING_NORMAL));
		// 				}
		// 				else
		// 				{
		// 					/* P25.2 */
		// 					pMsg("P25.2");
		// 					result = true;
		//
		// 					// wbk ds missing but sheet ds other than good
		// 					// delete then activate the system
		// 					if (xData.GotTempAnySheets)
		// 					{
		// 						result = xMgr.DeleteDsList(xData.TempShtDsList);
		// 					}
		//
		// 					if (result)
		// 					{
		// 						status = activateSystem();
		//
		// 						// update status
		// 						// running
		// 						if (status == ES_START_DONE_GOOD)
		// 							OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(RunningStatus.RN_RUNNING_NORMAL));
		// 					}
		// 					else
		// 					{
		// 						status = ES_START_DONE_FAIL;
		// 					}
		// 				}
		// 			}
		// 			else if (xMui.WbkDsResCode == VDS_INVALID)
		// 			{
		// 				/* P30 */	
		// 				pMsg("P30");
		// 				// both schemas are good
		// 				// wkb ds is not valid - don't know if this can happen
		// 				// remove the workbook ds and sht ds
		//
		// 				if (xData.GotTempWbkDsList) xMgr.DeleteDsList(xData.TempWbkDsList);
		// 				if (xData.GotTempAnySheets) xMgr.DeleteDsList(xData.TempShtDsList);
		// 			}
		// 			// other
		// 			else status = ES_START_DONE_FAIL;
		//
		// 			return status;
		// 		}
		// 		

		// public void StartOpr2()
		// {
		// 	ExSysStatus tmp1 = xMui.ExSysStatus;
		// 	LaunchCode  tmp2 = xMui.LaunchCode;
		//
		// 	string msg = $"{xMui.WbkScResCode} [{xMui.WbkScResDesc}\n{xMui.ShtScResCode} [{xMui.ShtScResDesc}]\n{xMui.WbkDsResCode} [{xMui.WbkDsResDesc}]\n{xMui.ShtDsResCode} [{xMui.ShtDsResDesc}]";
		//
		// 	statDialog("start op entry", null, TaskDialogCommonButtons.Ok);
		//
		// 	ExSysStatus status = xMui.ExSysStatus;
		//
		// 	clearStatus();
		//
		// 	if (status != ES_START_DONE_GOOD)
		// 	{
		// 		// !P00 which falls through
		// 		status = switchBoardX();
		// 		// exstorsys
		// 		OnPropChgd(new PropChgEvtArgs(PO_XSYS, PI_XSYS_STATUS, status));
		// 	}
		// 	else
		// 	{
		// 		pMsg("P00");
		// 	}
		//
		// 	if (status == ES_START_DONE_GOOD)
		// 	{
		// 		setStatus();
		//
		// 		Mw.ShowDialog();
		// 	}
		// 	else if (status == ES_START_DONE_DEACTIVATE)
		// 	{
		// 		xMui.ExSysStatus = tmp1;
		// 		xMui.LaunchCode  = tmp2;
		// 	}
		// 	else
		// 	{
		// 		// default case - all failed
		// 		statDialog("start op fail");
		// 		OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(RunningStatus.RN_CANNOT_RUN_FAIL));
		// 		return;
		// 	}
		// }


		// 		/* all ex P00 */
		// 		private ExSysStatus switchBoardX()
		// 		{
		// 			pMsg("SB-A");
		// 			ExSysStatus status = ES_START_DONE_FAIL;
		// 			TaskDialogResult taskResult;
		//
		// 			Msgs.CWriteLine($"*** Begin Start");
		//
		// 			status = xMui.ExSysStatus;
		//
		// 			// normal startup / all parts found
		// 			if (status == ES_VRFY_DONE_GOOD)
		// 			{
		// /* P05 */		
		// 				pMsg("P05");
		// 				status = configNormal();
		// 			}
		// 			// normal startup, no parts found / crdeate system
		// 			else if (
		// 				(xMui.ExSysStatus == ES_VRFY_DONE_RESTART &&
		// 				xMui.LaunchCode == LaunchCode.LC_DONE_RESTART &&
		// 				xMui.WbkScResCode == ValidateSchema.VSC_MISSING) ||
		//
		// 				(xMui.ExSysStatus == ES_VRFY_DONE_FAIL &&
		// 				xMui.LaunchCode == LaunchCode.LC_DONE_INVALID)
		// 				)
		// 			{
		// /* P10 / P15 */
		// 				pMsg("P10 & P15");
		// 				status = activateSystem();
		//
		// 				// update status
		// 				// running
		// 				if (status == ES_START_DONE_GOOD)
		// 					OnPropChgd(PI_GEN_RUNNING_STAT, new DynaValue(RunningStatus.RN_RUNNING_NORMAL));
		// 			}
		// 			else if (status == ES_VRFY_DONE_HOLD_WBK)
		// 			{
		// 				status = switchBoardB();
		// 			}
		// 			else
		// 			{
		// // other
		// 				status = ES_START_DONE_FAIL;
		// 			}
		//
		// 			Msgs.CWriteLine($"*** End Start");
		//
		// 			return status;
		// 		}

		// when ExSysStatus == ES_VRFY_DONE_HOLD_WBK


		/* ops */

		// op X


		// public bool Start()
		// {
		// 	bool result = true;
		//
		// 	ExSysStatus status = ES_VRFY_DONE_GOOD;
		//
		// 	Msgs.CWriteLine($"*** Begin Start");
		//
		// 	// if (xMui.ExSysStatus != ES_VRFY_DONE_GOOD) result = false;
		//
		// 	if (xMui.ExSysStatus != ES_VRFY_DONE_GOOD)
		// 	{
		// 		// normal, everything found, read into memory
		// 		status = Read();
		// 	}
		//
		// 	Msgs.CWriteLine($"*** End Start");
		//
		// 	xMui.ExSysStatus = status;
		//
		// 	return result;
		// }

		// public ExSysStatus StartDriver()
		// {
		// 	Msgs.WriteLineSpaced("\n*** status",$"{xMui.LaunchCode,-15} / {xMui.ExSysStatus, -15} | {xMui.WbkScResCode,-15} / {xMui.ShtScResCode,-15} / {xMui.WbkDsResCode,-15} / {xMui.ShtDsResCode, -15}");
		// 	Msgs.WriteLineSpaced($"\n know object ids | {this.ToString()}");
		//
		// 	if (xMui.ExSysStatus == ES_VRFY_DONE_GOOD) return ES_VRFY_DONE_GOOD;
		//
		// 	TaskDialogResult result = DialogTest();
		//
		// 	if (result == TaskDialogResult.Cancel) return ES_VRFY_DONE_FAIL;
		//
		// 	return ES_VRFY_DONE_GOOD;
		// }

	#endregion

	}
}