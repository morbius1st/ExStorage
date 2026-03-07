using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml.Linq;

using Autodesk.Revit.DB.ExtensibleStorage;

using ExStoreTest2026;
using ExStoreTest2026.DebugAssist;
using ExStorSys;
using ExStoreTest2026.Windows;

using RevitLibrary;

using UtilityLibrary;

using static ExStorSys.ExSysStatus;
using static ExStorSys.LaunchCode;

using static ExStorSys.ValidateDataStorage;
using static ExStorSys.ValidateSchema;

// username: jeffs
// created:  10/26/2025 7:40:36 AM

namespace ExStorSys
{
	// will only be used with sheet ds - so will only be good or have wrong version or is invalid
	// out of date does not apply as, if the number of fields change, the version
	// must change and that will flat the issue

	public class ExStorLaunchMgr : APropChgdEvt
	{

		// ReSharper disable once MemberCanBePrivate.Global
		public int ObjectId;
		public string ObjectIdStr;

	#region private fields

		private const string SP = "    ";

		// private ExStorLib xLib;
		private ExStorMgr xMgr;
		private ExStorData xData;
		private MainWinModelUi xMui;

		// private ExSysStatus exSysStatusLocal;

		private LaunchCode lCode;

		private ValidateSchema resultWbkSc;
		private ValidateDataStorage resultWbkDs;
		private ValidateSchema resultShtSc;
		private ValidateDataStorage resultShtDs;

		private int tabDepth;

		private bool? sendDebug = false;

			private const int COL_A = -18;
			private const int COL_B = -20;
			private const int COL_C = -20;
			private const int COL_D = -20;
			private const int COL_L = -18;
			private const int COL_E = -26;
			private const int COL_F = -16;
			private const int COL_G = -8;

	#endregion

	#region ctor


		public static ExStorLaunchMgr? Instance { get; set; }

		#pragma warning disable CS8618, CS9264
		private ExStorLaunchMgr() { }
		#pragma warning restore CS8618, CS9264

		public static ExStorLaunchMgr Create()
		{
			Instance = new ExStorLaunchMgr();
			Instance.init();

			return Instance;
		}

		private void init()
		{
			// ObjectId = AppRibbon.ObjectIdx++;

			ObjectId = ExStorStartMgr.Instance?.AddObjId(nameof(ExStorLaunchMgr)) ?? -1;

			Restore();
		}

		public void Restore()
		{
			xMgr = ExStorMgr.Instance!;
			// xLib = ExStorLib.Instance;
			xData = xMgr.xData;
			xMui = xMgr.Mui;

			this.PropChgd += xMui.OnPropChgdEvent;

			ObjectIdStr = $"lMgr {ObjectId} | xMgr {xMgr.ObjectId} | xData {xData.ObjectId}"; // | xLib {xLib.ObjectId}";
		}

	#endregion

	#region public properties

		private	LaunchCode _LaunchCode
		{
			get => lCode;
			set
			{
				lCode = value;
				xMgr.Mui.LaunchCode = value;
			}
		}

		private ExSysStatus _ExSysStatus
		{
			get => xMgr.Mui.ExSysStatus;
			set => xMgr.Mui.ExSysStatus = value;
		}

	#endregion

	#region launch routines

		public void Reset()
		{
			lCode = LaunchCode.LC_NA;

			resultWbkSc = VSC_NA;
			resultWbkDs = VDS_NA;
			resultShtSc = VSC_NA;
			resultShtDs = VDS_NA;

			tabDepth = 0;
		}

		/* process drivers */

		// A
		/// <summary>
		/// primary driver to verify and resolve the components
		/// of the Ex Storage System<br/>
		/// runs at system startup & when needed
		/// </summary>
		public void OnOpenDocLaunch()
		{
			sendDebug = true;

			writeLine($"*** begin OnOpenDocLaunch *** [ {R.FileName} ]");
			tabDepth++;

			OnPropChgdRn(RunningStatus.RN_NA);

			// set to base values
			_ExSysStatus = ExSysStatus.ES_NA;
			_LaunchCode = LaunchCode.LC_NA;

			bool result = OnOpenDocLaunchVfy();

			writeLine($"*** OnOpenDocLaunchVfy result| {result}");

			// showStatus();
			//
			// writeLine("\nbefore resolve");
			// writeLine($"{" ".Repeat(26)}{"WbkSc",COL_A}\t{"WbkDs",COL_C}\t{"ShtSc",COL_B}\t{"ShtDs",COL_D} | {"ExSysStatus",COL_E}");
			// writeLine($"{"ignore LaunchCode",-26}{resultWbkSc,COL_A}\t{resultWbkDs,COL_C}\t{resultShtSc,COL_B}\t{resultShtDs, COL_D} | {_ExSysStatus,COL_E} | result {result}\n");

			if (!result)
			{
				// onOpenDocLaunchRslv();

				_ExSysStatus = ExSysStatus.ES_VRFY_DONE_FAIL;
				_LaunchCode = LaunchCode.LC_DONE_FAIL;

				// showStatus()
			}
			else
			{
				_ExSysStatus = ExSysStatus.ES_VRFY_DONE_GOOD;
				_LaunchCode = LaunchCode.LC_DONE_GOOD;
			}

			// writeLine("\nafter resolve");
			writeLine(             $"{" ".Repeat(26)}{"WbkSc",COL_C}\t{"WbkDs",COL_A}\t{"ShtSc",COL_D}\t{"ShtDs",COL_B} | {"ExSysStatus",COL_E}");
			writeLine($"{"ignore LaunchCode",-26}{resultWbkSc,COL_C}\t{resultWbkDs,COL_A}\t{resultShtSc, COL_D}\t{resultShtDs,COL_B} | {_ExSysStatus,COL_E} | result {result}\n");
			
			tabDepth--;
			writeLine("*** complete OnOpenDocLaunch ***\n");
		}

		/// <summary>
		/// initial verification.  verifys that schema's and data objects exist<br/>
		/// true if both schema and ds are found and good<br/>
		/// false if any issues<br/>
		/// a code is returned that identifies the status of each of the four validations<br/>
		/// the code -1 is returned when a validation is not performed because a prior<br/>
		/// failed and testing cannot continue
		/// </summary>
		public	bool OnOpenDocLaunchVfy()
		{
			/* verify wbk */
			onOpenDocLaunchVfy2Wbk();

			/* verify sht */
			onOpenDocLaunchVfy2Sht();

			return xMui.ValidateStatus();
		}

		private void onOpenDocLaunchVfy2Wbk()
		{
			xData.TempWbkDsEx = null;
			xData.TempWbkDsList = null;
			xData.TempWbkSchemaEx = null;

			resultWbkSc = VSC_MISSING;

			/* wbk first / wbk first */

			// step 1a
			resultWbkDs = findWbkDataStorage2();

			// writeLine($"1a | resultWbkDs | {resultWbkDs} | got wbk ds | {xData.GotTempWbkDs}");
			//
			// if (resultWbkDs == VDS_GOOD || resultWbkDs == VDS_WRONG_VER)
			// {
				// only proceed if a valid ds is found - if no found, 
				// any schema does not apply
				
				// step 1b
				resultWbkSc = findAndVerifyWbkSchema2();
				// writeLine($"1b | resultWbkSc | {resultWbkSc} | got wbk sc | {xData.GotWbkSchema}");
				
				// the above can return missing, invalid, etc.
				// if return is good or wrong version

				if (resultWbkDs == VDS_GOOD || resultWbkDs == VDS_WRONG_VER)
				{

					if (resultWbkSc == VSC_GOOD || resultWbkSc == VSC_WRONG_VER)
					{
						// step 1c
						ValidateDataStorage result = verifyWbkDataStorage2();

						resultWbkDs = result == VDS_ACT_IGNORE ? VDS_ACT_IGNORE :
							result == VDS_GOOD ? resultWbkDs : resultWbkDs == VDS_GOOD ? result : VDS_MULTIPLE_MN_O;

						// writeLine($"1c | resultWbkDs | {resultWbkDs} | got wbk ds | {xData.GotTempWbkDs}");
					}
					else
					{
						resultWbkDs = VDS_INVALID;
					}
				}

			// }


			OnPropChgdWsc(resultWbkSc);
			OnPropChgdWds(resultWbkDs);
		}

		private void onOpenDocLaunchVfy2Sht()
		{
			xData.TempShtDsListEx = null;
			xData.TempShtDsList = null;
			xData.TempShtSchemaEx = null;

			resultShtSc = VSC_MISSING;

			/* sht second / sc first */

			// step 2a
			resultShtDs = findShtDataStorage2();
			// writeLine($"2a | resultShtDs | {resultShtDs} | got sht ds {xData.GotTempShtDs}");

			// if (resultShtDs == VDS_GOOD || resultShtDs == VDS_WRONG_VER)
			// {
				// step 2b
				resultShtSc = findAndVerifyShtSchema2();
				// writeLine($"2b | resultShtSc | {resultShtSc} | got sht sc {xData.GotShtSchema}");

				if (resultShtDs == VDS_GOOD || resultShtDs == VDS_WRONG_VER)
				{
					if (resultShtSc == VSC_GOOD || resultShtSc == VSC_WRONG_VER)
					{
						// step 2c
						ValidateDataStorage result = verifyShtDataStorage2();

						resultShtDs = result == VDS_GOOD ? resultShtDs :
							resultShtDs == VDS_GOOD ? result : VDS_MULTIPLE_MN_O;

						// writeLine($"2c | resultShtDs | {resultShtDs} | got sht ds {xData.GotTempShtDs}");
					}
					else
					{
						resultShtDs = VDS_INVALID;
					}
				}
			// }


			OnPropChgdSds(resultShtDs);
			OnPropChgdSsc(resultShtSc);
		}

		/* 1a */
		// initial verifyication, locate a wbk ds
		/// <summary>
		/// initial verification step.  find a wbk ds and save
		/// this to xData
		/// </summary>
		private ValidateDataStorage findWbkDataStorage2()
		{
			string verStr;
			ValidateDataStorage status = VDS_GOOD;
			IList<DataStorage> dsList;
			ExListItem<DataStorage> dsx;
			

			if (!xMgr.FindAllWbkDs(out dsList)) return VDS_MISSING;
			if (dsList.Count != 1) return VDS_WRONG_COUNT;
			
			status = verifyDataStorage(dsList[0], ExStorConst.EXS_VERSION_WBK, 
				out dsx, out verStr);

			if (status != VDS_INVALID)
			{
				xData.TempWbkDsEx = dsx;
				xData.TempWbkVersion = verStr;
			}

			return status;
		}

		/* 1b */
		// there can be only one - but the find routine can find many
		// need to verify which, if any, are correct
		private ValidateSchema findAndVerifyWbkSchema2()
		{
			IList<Guid> guids;
			IList<Schema> scList;
			ExListItem<Schema> scx;

			if (!xMgr.FindAllWbkSchema(out scList)) return VSC_MISSING;

			if (!xData.GotTempWbkDs)
			{
				// no data storage - cannot verify schema
				// so must assume they are all good
				// use the first one found

				xData.TempWbkSchemaEx = new ExListItem<Schema>(scList[0].SchemaName, scList[0]);
				return VSC_GOOD;
			}

			guids = xData.TempWbkDsEx!.Item.GetEntitySchemaGuids();

			foreach (Schema sc in scList)
			{
				// if ( verifySchema(sc, xData.TempWbkVersion, out scx) == VSC_INVALID) continue;
				if ( verifySchema(sc, ExStorConst.EXS_VERSION_WBK, out scx) == VSC_INVALID) continue;

				// if resultWbkDs == wrong version && scx.version == true -> invalid -> continue
				// if ( resultWbkDs == VDS_WRONG_VER && !scx.Version) continue;

				if (!verifySchemaGuid(sc, guids)) continue;

				if (xData.TempWbkSchemaEx == null)
				{
					// found the first one
					xData.TempWbkSchemaEx = scx;
				}
				else
				{
					// found a second one - fail
					xData.TempWbkSchemaEx = null;
					return VSC_WRONG_COUNT;
				}
			}

			return (xData.TempWbkSchemaEx?.Version ?? false) ? VSC_GOOD : xData.TempWbkSchemaEx == null ? VSC_MISSING : VSC_WRONG_VER;
		}

		/* 1c */
		/// <summary>
		/// verify wbk ds part 2.  verify activation status / setting<br/>
		/// and verify the model name
		/// </summary>
		private ValidateDataStorage verifyWbkDataStorage2()
		{
			// gotten here - remove the initial not valid
			// xData.TempWbkDsEx!.SetValid();

			ActivateStatus actStat;
			// string modelName;

			// initial verification done
			ValidateDataStorage status = VDS_GOOD;

			if (xMgr.VerifyActivationIgnore() != VDS_GOOD) return VDS_ACT_IGNORE;
			if (xMgr.VerifyActivationOff() != VDS_GOOD)
			{
				status = VDS_ACT_OFF;
				xData.TempWbkDsEx!.SetActIsOff();
			}

			// validate activation and model name
			// actStat = xMgr.ReadActivationStatus(xData.TempWbkDsEx!.Item, xData.TempWbkSchemaEx!.Item);
			//
			// if (actStat == ActivateStatus.AS_IGNORE) return VDS_ACT_IGNORE;
			//
			// if (actStat == ActivateStatus.AS_INACTIVE)
			// {
			// 	status = VDS_ACT_OFF;
			// 	xData.TempWbkDsEx!.SetActIsOff();
			// }

			// modelName = xMgr.ReadModelName(xData.TempWbkDsEx!.Item, xData.TempWbkSchemaEx.Item) ?? "";
			//
			// if (!modelName.Equals(xMgr.Exid.ModelName))
			// {
			// 	status = status == VDS_GOOD ? VDS_WRONG_MODEL_NAME : VDS_MULTIPLE_MN_O;
			// 	xData.TempWbkDsEx!.SetWrongModelName();
			// }

			if (xMgr.VerifyModelName() != VDS_GOOD)
			{
				status = status == VDS_GOOD ? VDS_WRONG_MODEL_NAME : VDS_MULTIPLE_MN_O;
				xData.TempWbkDsEx!.SetWrongModelName();
			}


			return status;
		}

		/* 2a */
		/// <summary>
		/// initial verification step.  find sht ds's and save
		/// this to xData
		/// </summary>
		private ValidateDataStorage findShtDataStorage2()
		{
			string verStr;
			ValidateDataStorage result = VDS_GOOD;
			ValidateDataStorage status = VDS_GOOD;
			IList<DataStorage> dsList;
			ExListItem<DataStorage> dsx;
			
			if (!xMgr.FindAllShtDs(out dsList)) return VDS_MISSING;
			if (dsList.Count == 0) return VDS_WRONG_COUNT;

			xData.TempShtDsListEx = new ();

			foreach (DataStorage ds in dsList)
			{
				status = verifyDataStorage(ds, ExStorConst.EXS_VERSION_SHT, 
					out dsx, out verStr);

				if (status != VDS_GOOD && result == VDS_GOOD) result = status;

				if (status == VDS_INVALID) continue;

				xData.TempShtDsListEx.Add(dsx);
				xData.TempShtVersion = verStr;
			}

			if (result == VDS_GOOD && 
				xData.TempShtDsListEx.GoodItemsCount == 0) result = VDS_WRONG_COUNT;

			return result;
		}

		/* 2b */
		private ValidateSchema findAndVerifyShtSchema2()
		{
			IList<Guid> guids;
			IList<Schema> scList;
			ExListItem<Schema> scx;

			if (!xMgr.FindAllShtSchema(out scList)) return VSC_MISSING;

			if (!xData.GotTempShtDs || !xData.TempShtDsListEx!.GotOneGoodItem)
			{
				// no data storage - cannot verify schema
				// so must assume they are all good
				// use the first one found

				xData.TempShtSchemaEx = new ExListItem<Schema>(scList[0].SchemaName, scList[0]);
				return VSC_GOOD;
			}

			guids = xData.TempShtDsListEx!.GetGoodItem!.Item.GetEntitySchemaGuids();

			foreach (Schema sc in scList)
			{
				// if ( verifySchema(sc, xData.TempShtVersion, out scx) == VSC_INVALID) continue;
				if ( verifySchema(sc, ExStorConst.EXS_VERSION_SHT, out scx) == VSC_INVALID) continue;

				// if ( resultShtDs == VDS_WRONG_VER && !scx.Version) continue;

				if (!verifySchemaGuid(sc, guids)) continue;

				if (xData.TempShtSchemaEx == null)
				{
					// found the first one
					xData.TempShtSchemaEx = scx;
				}
				else
				{
					// found a second one - fail
					xData.TempShtSchemaEx = null;
					return VSC_WRONG_COUNT;
				}
			}

			return xData.TempShtSchemaEx!.Version ? VSC_GOOD : VSC_WRONG_VER;
		}

		/* 2c */
		private ValidateDataStorage verifyShtDataStorage2()
		{
			// ValidateDataStorage status = VDS_GOOD;
			ValidateDataStorage result = VDS_GOOD;

			// foreach ((string? key, ExListItem<DataStorage>? item) in xData.TempShtDsListEx!.GoodItems)
			// {
				// status = validateShtDataStorage();
			//
			// 	if (status == VDS_INVALID) continue;
			//
			// 	if (status != VDS_GOOD && result == VDS_GOOD) result = status;
			// }

			if ((xData.TempShtDsListEx?.GoodItemsCount ?? 0) == 0) result = VDS_WRONG_COUNT;

			return result;
		}

		/* backup routines */

		/// <summary>
		/// initial validation of a datastorage element
		/// </summary>
		private ValidateDataStorage verifyDataStorage(DataStorage ds, string verTestStr, 
			out ExListItem<DataStorage> dsx, out string verStr)
		{
			ValidateDataStorage status = VDS_GOOD;
			
			verStr = String.Empty;
			dsx = new ExListItem<DataStorage>(ds.Name, ds);

			if (!ds.IsValidObject || ds.GetEntitySchemaGuids().Count != 1)
			{
				dsx.SetNotValid();
				return VDS_INVALID;
			}

			verStr = xMgr.ExtractVersionFromName(ds.Name)!;
			
			if (verStr.IsVoid() || !verStr!.Equals(verTestStr))
			{
				dsx.SetWrongVersion();
				status = VDS_WRONG_VER;
			}

			return status;
		}

		// /// <summary>
		// /// data storage validation part two.  validate activation status and model name
		// /// </summary>
		// private ValidateDataStorage validateShtDataStorage()
		// {
		// 	ActivateStatus actStat;
		// 	string modelName;
		//
		// 	ValidateDataStorage status = VDS_GOOD;
		//
		// 	actStat = xMgr.ReadActivationStatus(xData.TempWbkDsEx!.Item, xData.TempWbkSchemaEx!.Item);
		//
		// 	if (actStat == ActivateStatus.AS_IGNORE) return VDS_ACT_IGNORE;
		//
		// 	if (actStat == ActivateStatus.AS_INACTIVE)
		// 	{
		// 		status = VDS_ACT_OFF;
		// 		xData.TempWbkDsEx!.SetActIsOff();
		// 	}
		//
		// 	modelName = xMgr.ReadModelName(xData.TempWbkDsEx!.Item, xData.TempWbkSchemaEx.Item) ?? "";
		//
		// 	if (!modelName.Equals(xMgr.Exid.ModelName))
		// 	{
		// 		status = status == VDS_GOOD ? VDS_WRONG_MODEL_NAME : VDS_MULTIPLE_MN_O;
		// 		xData.TempWbkDsEx!.SetWrongModelName();
		// 	}
		//
		// 	return status;
		// }

		/// <summary>
		/// verify if the schema is OK<br/>
		/// validates if an OK revit object and the version is correct
		/// </summary>
		private ValidateSchema  verifySchema(Schema sc, string verStr, out ExListItem<Schema> scx)
		{
			scx = new ExListItem<Schema>(sc.SchemaName, sc);

			if (!sc.IsValidObject) return VSC_INVALID;
			
			ValidateSchema status = VSC_GOOD;

			string? verStrTst = xMgr.ExtractVersionFromName(sc.SchemaName);

			if (verStrTst.IsVoid() || !verStrTst!.Equals(verStr))
			{
				scx.SetWrongVersion();
				status = VSC_WRONG_VER;
			}

			return status;
		}

		/// <summary>
		/// validate the schema's guid against a list of guids.  the list of guids
		/// should come from the data storage object
		/// </summary>
		private bool verifySchemaGuid(Schema sc, IList<Guid> guids)
		{
			foreach (Guid guid in guids)
			{
				if (sc.GUID.Equals(guid)) return true;
			}

			return false;
		}

	#endregion

	#region private utility methods

		private void showStatus()
		{
			showStatus(0, 0, true); // wbk, schema
			showStatus(0, 1, true); // wbk, ds
			showStatus(1, 0, true); // sht, schema
			showStatus(1, 1, true); // sht, ds

			writeLineMid($"{"ExSysStatus",-21}value {xMui.ExSysStatus} [{xMui.ExSysStatusDesc}] - should be n/a");
		}

		private void showStatus(int which1, int which2, bool basic = false )//, ValidateSchema codeSc, ValidateDataStorage codeDs)
		{
			string whichA = ExStorConst.DataClassAbbrevUc[which1]; // which 1 == 0 wbk, 1 sht
			string whichF = ExStorConst.DataClassFull[which1];     // which 1 == 0 workbook, 1 sheet
			string whichB = ExStorConst.DataContainerFull[which2]; // which 2 == 0 schema, 1 datastorage
			string result;
			string resolve;

			ValidateSchema codeSc;
			ValidateDataStorage codeDs;

			string msg1 = "| has issues | status code ";

			if (which2 == 0) // schema
			{
				codeSc = which1 == 0 ? resultWbkSc : resultShtSc; // if == 0, workbook

				// either which 1 = 0 or 1 - wbk or sht
				result = ExStorConst.ValidateSchemaDesc[codeSc].Item2;
				resolve = ExStorConst.ValidateSchemaDesc[codeSc].Item3;

				writeLineMid($"{whichF,-9} {whichB,-12}{msg1,-30}{codeSc}");
				if (!basic) writeLineMid($"  {codeSc,-32} | {whichA, -6} {result} | resolve {resolve}");
			}
			else // ds
			{
				codeDs = which1 == 0 ? resultWbkDs : resultShtDs; // if == 0, workbook

				result = ExStorConst.ValidateDataStorageDesc[codeDs].Item2;
				resolve = ExStorConst.ValidateDataStorageDesc[codeDs].Item3;

				writeLineMid($"{whichF, -9} {whichB, -12}{msg1,-30}{codeDs}");
				if (!basic) writeLineMid($"  {codeDs, -32} | {whichA, -6} {result} | resolve {resolve}");
			}
		}

	#endregion

	#region private methods

		private void writeLineBeg(string msg, string preface = "")
		{
			writeLine($"{preface}{SP.Repeat(++tabDepth)}{msg}");
		}

		private void writeLineMid(string msg, string preface = "")
		{
			writeLine($"{preface}{SP.Repeat(tabDepth + 1)}{msg}");
		}

		private void writeLineEnd(string msg, string preface = "")
		{
			writeLine($"{preface}{SP.Repeat(tabDepth--)}{msg}");
			
			tabDepth = tabDepth < 0 ? 0 : tabDepth;
		}

		// ReSharper disable once InconsistentNaming
		private void write(string msg)
		{
			if (sendDebug == true || !sendDebug.HasValue)
			{
				Debug.Write(!msg.IsVoid() && !msg.Trim().Equals("\n") ? $"***| {msg}" : $"{msg}");
			}
			else if (sendDebug == false || !sendDebug.HasValue)
			{
				xMgr.MessageCache += msg;
			}
		}


		// ReSharper disable once UnusedMember.Local
		private void writeLine(string msg)
		{
			write(msg + "\n");
		}

	#endregion

		
	// 	// B
	// 	/// <summary>
	// 	/// primary driver for the resolver - this determines the final
	// 	/// status codes - actual resolution done "off" from the main window
	// 	/// </summary>
	// 	private void onOpenDocLaunchRslv2()
	// 	{
	// 		writeLineBeg("B *** BEGIN Launch Resolve ***", "\n");
	// 		writeLineMid($"model name{xMgr.Exid.ModelTitle}");
	//
	// 		try
	// 		{
	// 			verifyResolver();
	// 		}
	// 		catch (Exception e)
	// 		{
	// 			writeLineMid($"*** exception | {e.Message}");
	// 			_LaunchCode = LaunchCode.LC_DONE_INVALID;
	// 			_ExSysStatus = ExSysStatus.ES_VRFY_DONE_FAIL;
	// 		}
	//
	// 		// writeLineMid($"launch status {_LaunchCode}");
	// 		// writeLineMid($"ExSys status  {_ExSysStatus}\n");
	//
	// 		// writeLineMid($"WBK Schema  {resultWbkSc}");
	// 		// writeLineMid($"SHT Schema  {resultShtSc}");
	// 		// // ReSharper disable once StringLiteralTypo
	// 		// writeLineMid($"WBK D_Stor  {resultWbkDs}");
	// 		// // ReSharper disable once StringLiteralTypo
	// 		// writeLineMid($"SHT D_Stor  {resultShtDs}\n");
	//
	// 		writeLine("\nafter resolve");
	// 		writeLine($"{"ES", COL_E}\t{"LC",COL_L}\t{"WbkSc",COL_A}\t{"ShtSc",COL_B}\t{"WbkDs",COL_C}\tShtDs");
	// 		writeLine($"{_ExSysStatus, COL_E}\t{_LaunchCode,COL_L}\t{resultWbkSc,COL_A}\t{resultShtSc,COL_B}\t{resultWbkDs,COL_C}\t{resultShtDs}\n");
	//
	//
	// 		writeLineEnd("B *** END Launch Resolve ***", "\n");
	// 	}
	//
	// #endregion
	//
	// #region verification methods
	//
	// 	// E
	// 	/// <summary>
	// 	/// initial verification.  verifys that schema's and data objects exist<br/>
	// 	/// true if both schema and ds are found and good<br/>
	// 	/// false if any issues<br/>
	// 	/// a code is returned that identifies the status of each of the four validations<br/>
	// 	/// the code -1 is returned when a validation is not performed because a prior<br/>
	// 	/// failed and testing cannot continue
	// 	/// </summary>
	// 	private	bool OnOpenDocLaunchVfy2()
	// 	{
	// 		// set to base values
	// 		_ExSysStatus = ExSysStatus.ES_NA;
	// 		_LaunchCode = LaunchCode.LC_NA;
	//
	// 		writeLineBeg("E *** begin OnOpenDocLaunchVfy ***", "\n");
	//
	// 		/* verify */
	//
	// 		resultWbkSc = findAndVerifyWbkSchema();
	// 		OnPropChgdWsc(resultWbkSc);
	//
	// 		resultShtSc = findAndVerifyShtSchema();
	// 		OnPropChgdSsc(resultShtSc);
	//
	// 		resultWbkDs = findAndVerifyWbkDataStorage();
	// 		OnPropChgdWds(resultWbkDs);	
	// 		
	// 		resultShtDs = findAndVerifyShtDataStorage();
	// 		OnPropChgdSds(resultShtDs);
	//
	// 		writeLineEnd("E *** end OnOpenDocLaunchVfy ***\n", "\n");
	//
	// 		return xMui.ValidateStatus();
	// 	}
	//
	// 	/* verify schema */
	//
	// 	/// <summary>
	// 	/// find all valid wbk schema<br/>
	// 	/// return 0 == good - only one schema, 1 == none found, 2 == one+ bad, 3 == got multiple schema<br/>
	// 	/// if a good schema is found, it is saved to TempWbkSchemaEx
	// 	/// </summary>
	// 	private ValidateSchema findAndVerifyWbkSchema()
	// 	{
	// 		IList<Schema> scList;
	//
	// 		xData.TempWbkSchemaEx = null;
	//
	// 		bool status = xMgr.FindAllWbkSchema(out scList);
	//
	// 		if (!status || scList.Count != 1) return VSC_WRONG_COUNT;
	//
	// 		xData.TempWbkSchemaEx = new ExListItem<Schema>(scList[0].SchemaName, scList[0]);
	// 		
	// 		return VSC_GOOD;
	// 	}
	//
	// 	// /// <summary>
	// 	// /// find all valid workbook schema<br/>
	// 	// /// if a good schema is found, it is saved to TempWbkSchemaEx
	// 	// /// </summary>
	// 	// private ValidateSchema findAndVerifyWbkSchema()
	// 	// {
	// 	// 	ExList<Schema> exList;
	// 	//
	// 	// 	ValidateSchema status = 
	// 	// 		findAndVerifySchemaByName(xMgr.Exid.WbkSearchName, ExStorConst.EXS_VERSION_WBK, out exList);
	// 	//
	// 	// 	xData.TempWbkSchemaList = exList;
	// 	// 	xData.TempWbkSchemaEx = null;
	// 	//
	// 	// 	if (status == VSC_GOOD)
	// 	// 	{
	// 	// 		if (exList.Count == 1)
	// 	// 		{
	// 	// 			xData.TempWbkSchemaEx = exList.First!.Item;
	// 	// 		}
	// 	// 		else
	// 	// 		{
	// 	// 			status = VSC_WRONG_COUNT;
	// 	// 		}
	// 	// 	}
	// 	//
	// 	// 	// set initial status - to be updated later - sometimes
	// 	// 	resultWbkDs = (ValidateDataStorage) status;
	// 	//
	// 	// 	return status;
	// 	// }
	//
	// 	// /// <summary>
	// 	// /// find all valid sheet schema<br/>
	// 	// /// return 0 == good - only one schema, 1 == none found, 2 == one+ bad, 3 == got multiple schema<br/>
	// 	// /// if a good schema is found, it is saved to TempShtSchemaEx
	// 	// /// </summary>
	// 	// private ValidateSchema findAndVerifyShtSchema()
	// 	// {
	// 	// 	ExList<Schema> scList;
	// 	//
	// 	// 	ValidateSchema status = findAndVerifySchemaByName(xMgr.Exid.ShtSearchName, out scList);
	// 	//
	// 	// 	xData.TempShtSchemaList = scList;
	// 	// 	xData.TempShtSchemaEx = null;
	// 	//
	// 	// 	// 0 if all good, 1 if none found, 2 if one+ bad
	// 	// 	if (status == VSC_GOOD)
	// 	// 	{
	// 	// 		// status is 0 so all found have correct number of fields
	// 	// 		// if only one found, save it
	// 	// 		// if not, status is 3
	// 	// 		if ((scList?.Count ?? 0) == 1)
	// 	// 		{
	// 	// 			if (scList?[0].ListFields().Count != Fields.SHT_FIELDS_COUNT)
	// 	// 			{
	// 	// 				status = VSC_OUT_OF_DATE;
	// 	// 			}
	// 	// 			else
	// 	// 			{
	// 	// 				xData.TempShtSchemaEx = scList[0];
	// 	// 			}
	// 	// 		}
	// 	// 		else
	// 	// 		{
	// 	// 			status = VSC_WRONG_COUNT;
	// 	// 		}
	// 	// 	}
	// 	// 	
	// 	// 	resultShtDs = (ValidateDataStorage) status;
	// 	//
	// 	// 	return status;
	// 	// }
	//
	// 	/// <summary>
	// 	/// find all valid sheet schema<br/>
	// 	/// if a good schema is found, it is saved to TempShtSchemaEx
	// 	/// </summary>
	// 	private ValidateSchema findAndVerifyShtSchema()
	// 	{
	// 		IList<Schema> scList;
	//
	// 		xData.TempShtSchemaEx = null;
	//
	// 		bool status = xMgr.FindAllShtSchema(out scList);
	//
	//
	// 		if (!status || scList.Count != 1) return VSC_WRONG_COUNT;
	//
	// 		xData.TempShtSchemaEx =  new ExListItem<Schema>(scList[0].SchemaName, scList[0]);
	//
	// 		return VSC_GOOD;
	// 	}
	//
	// 	/* verify datastorage */
	//
	// 	// /// <summary>
	// 	// /// find all valid wbk data storage objects<br/>
	// 	// /// return 0 == one found and good , 1 == none found, 2 == one+ bad, 3 == one+ found, all good<br/>
	// 	// /// </summary>
	// 	// private ValidateDataStorage findAndVerifyWbkDataStorage()
	// 	// {
	// 	// 	xData.TempWbkDsEx = null;
	// 	// 	xData.TempWbkDsEx = null;
	// 	// 	// xData.TempModelCode = null;
	// 	//
	// 	// 	IList<DataStorage>? dsList;
	// 	//
	// 	// 	ValidateDataStorage status = findAndValidateWbkDs(xMgr.Exid.WbkSearchName, xMgr.Exid.ModelName, out dsList);
	// 	//
	// 	// 	if (status == VDS_GOOD || status == VDS_ACT_OFF || status == VDS_ACT_IGNORE)
	// 	// 	{
	// 	// 		xData.TempWbkDsList = dsList;
	// 	// 		
	// 	// 		if (dsList!.Count > 1)
	// 	// 		{
	// 	// 			status = VDS_WRONG_COUNT;
	// 	// 		}
	// 	// 		else
	// 	// 		{
	// 	// 			xData.TempWbkDsEx = dsList[0];
	// 	//
	// 	// 			// if (xData.GotTempWbkSchema)
	// 	// 			// {
	// 	// 			// 	xData.TempModelCode = xMgr.ReadModelCode(xData.TempWbkDsEx, xData.TempWbkSchemaEx);
	// 	// 			// }
	// 	// 		}
	// 	//
	// 	// 	}
	// 	//
	// 	// 	return status;
	// 	// }
	//
	// 	/// <summary>
	// 	/// find and validate wbk data storage objects<br/>
	// 	/// require only one to be found and used - more than one is invalid -> wrong count<br/>
	// 	/// also, if wbk schema is not good / is not out of date, wbk ds is not good<br/>
	// 	/// the final, "approved" ds is saved in xData.TempWbkDsEx
	// 	/// </summary>
	// 	private ValidateDataStorage findAndVerifyWbkDataStorage()
	// 	{
	// 		// for wbk, can have only one ds - with or without the correct version
	// 		// and, this cannot have the wrong version if the ds schema has the correct version
	// 		// the final, "approved" ds is saved in xData.TempWbkDsEx
	//
	// 		ValidateDataStorage status = VDS_GOOD;
	// 		ActivateStatus actStat;
	// 		string modelName;
	// 		string? verStrTst;
	// 		IList<DataStorage> dsList;
	//
	// 		xData.TempWbkDsEx = null;
	//
	// 		// test 1
	// 		if (xMui.WbkScResCode == VSC_INVALID || xMui.WbkScResCode == VSC_MISSING) return VDS_INVALID;
	//
	// 		// find any wbk ds before checking if wbk schema is OK - allows 
	// 		// the possible removal of all of the ds
	// 		// proc 1 / test 2
	// 		if (!xMgr.FindAllWbkDs(out dsList)) return VDS_INVALID;
	//
	// 		// save the this list in case need to delete them later
	// 		xData.TempWbkDsList = dsList;
	// 		
	// 		// if got only one good item, ok - else wrong count
	// 		// test 3
	// 		if (dsList.Count != 1) return VDS_WRONG_COUNT;
	//
	//
	// 		// for the remainder, there is only one ds
	// 		DataStorage ds = dsList[0];
	//
	// 		// test 4
	// 		if (!ds.IsValidObject || ds.GetEntitySchemaGuids().Count != 1) return VDS_INVALID;
	//
	// 		ExListItem<DataStorage> exi = new (ds.Name, ds);
	//
	// 		if (xMui.WbkScResCode == VSC_GOOD) 
	// 		{
	// 			modelName = xMgr.ReadModelName(ds, xData.TempWbkSchemaEx.Item) ?? "";
	// 			actStat = xMgr.ReadActivationStatus(ds, xData.TempWbkSchemaEx.Item);
	// 			
	// 			// test 5
	// 			if (actStat == ActivateStatus.AS_IGNORE)
	// 			{
	// 				return VDS_ACT_IGNORE;
	// 			}
	// 			
	// 			// test 6
	// 			if (actStat == ActivateStatus.AS_INACTIVE)
	// 			{
	// 				status = VDS_ACT_OFF;
	// 				exi.SetActIsOff();
	// 			}
	//
	// 			// test 7
	// 			if (!modelName.Equals(xMgr.Exid.ModelName))
	// 			{
	// 				status = status == VDS_GOOD ? VDS_WRONG_MODEL_NAME : VDS_MULTIPLE_MN_O;
	// 				exi.SetWrongModelName();
	// 			}
	// 		}
	// 		else
	// 		if (xMui.WbkScResCode == VSC_WRONG_VER)
	// 		{
	// 			verStrTst = xMgr.ExtractVersionFromName(ds.Name);
	//
	// 			// test 8
	// 			if (verStrTst.IsVoid() || !ExStorConst.EXS_VERSION_WBK.Equals(verStrTst))
	// 			{
	// 				status = VDS_WRONG_VER;
	// 				exi.SetWrongVersion();
	// 			}
	// 		}
	//
	// 		xData.TempWbkDsEx = exi;
	//
	// 		return status;
	// 	}
	//
	// 	// /// <summary>
	// 	// /// find all valid sht data storage objects<br/>
	// 	// /// it is ok to have one+ sht ds objects however, each must be found<br/>
	// 	// /// good with a correct version name and correct number of fields<br/>
	// 	// /// also, if sht schema is not good / is not out of date, all ds are no good
	// 	// /// </summary>
	// 	// private ValidateDataStorage findAndVerifyShtDataStorage()
	// 	// {
	// 	// 	IList<DataStorage>? dsList;
	// 	//
	// 	// 	ValidateDataStorage status = findAndValidateShtDs(xMgr.Exid.ShtSearchName, out dsList);
	// 	//
	// 	// 	if (status == VDS_GOOD)
	// 	// 	{
	// 	// 		xData.TempShtDsList = dsList;
	// 	// 	}
	// 	//
	// 	// 	return status;
	// 	// }
	//
	// 	/// <summary>
	// 	/// find all valid sht data storage objects<br/>
	// 	/// it is ok to have one+ sht ds objects however, each must be found<br/>
	// 	/// good with a correct version name and correct number of fields<br/>
	// 	/// also, if sht schema is not good / is not out of date, all ds are no good
	// 	/// </summary>
	// 	private ValidateDataStorage findAndVerifyShtDataStorage()
	// 	{
	// 		ExList<DataStorage>? exList;
	//
	// 		ValidateDataStorage status = findAndValidateShtDs(xMgr.Exid.ShtSearchName, 
	// 			ExStorConst.EXS_VERSION_SHT, out exList);
	//
	// 		if (status == VDS_GOOD)
	// 		{
	// 			xData.TempShtDsListEx = exList;
	// 		}
	//
	// 		return status;
	// 	}
	//
	//
	// 	/* verify schema */
	//
	// 	// /// <summary>
	// 	// /// find all schema that starts with the search name and <br/>
	// 	// /// have the correct number of fields. <br/>
	// 	// /// return 0 == all good, 1 == none found, 2 == one+ bad<br/>
	// 	// /// </summary>
	// 	// private ValidateSchema findAndVerifySchemaByName(string searchName, string verString, out Schema? sc)
	// 	// {
	// 	// 	ExList<Schema>? scList;
	// 	//
	// 	// 	sc = null;
	// 	//
	// 	// 	// this does not filter for name version
	// 	//
	// 	// 	// if (xMgr.FindAllSchemaByNamePrefix(searchName, out scList) 
	// 	// 	if (xMgr.find(searchName, out scList) 
	// 	// 		!= ExStoreRtnCode.XRC_GOOD) return VSC_MISSING;
	// 	//
	// 	//
	// 	// 	if ((scList?.AllItemsCount ?? 0) != 1) return VSC_WRONG_COUNT;
	// 	//
	// 	// 	sc = scList!.GoodItems.First().Value.Item;
	// 	// 	
	// 	// 	string? verStrTst =  xLib.ExtractVersionFromName(sc.SchemaName);
	// 	//
	// 	// 	if (!(sc?.IsValidObject ?? false)) return VSC_INVALID;
	// 	// 	if (verStrTst.IsVoid() || !verStrTst!.Equals(verString)) return VSC_WRONG_VER;
	// 	//
	// 	// 	return VSC_GOOD;
	// 	// }
	//
	//
	// 	// /// <summary>
	// 	// /// find all schema that starts with the search name and <br/>
	// 	// /// have the correct number of fields. <br/>
	// 	// /// return ValidateSchema as applies<br/>
	// 	// /// </summary>
	// 	// private ValidateSchema findAndVerifySchemaByName(string searchName, string verName, out ExList<Schema> exList)
	// 	// {
	// 	// 	exList = new ExList<Schema>();
	// 	// 	
	// 	// 	IList<Schema>? scList;
	// 	// 	
	// 	// 	if (xLib.FindAllSchemaByNamePrefix(searchName, out scList) != ExStoreRtnCode.XRC_GOOD) return VSC_MISSING;
	// 	//
	// 	// 	bool result = validateSchemaList(scList, verName, out exList);
	// 	//
	// 	// 	return result ? VSC_GOOD : convertToVsc(exList);
	// 	// }
	//
	// 	// private ValidateSchema convertToVsc(ExList<Schema> exList)
	// 	// {
	// 	// 	ValidateSchema result = VSC_GOOD;
	// 	//
	// 	// 	if (exList.InvalidCount > 0 && exList.WrongVersionCount > 0) result = VSC_INVALID_OR_WRONG_VER;
	// 	// 	else if (exList.InvalidCount > 0) result = VSC_INVALID;
	// 	// 	else if (exList.WrongVersionCount > 0) result =  VSC_WRONG_VER;
	// 	//
	// 	// 	return result;
	// 	// }
	//
	// 	// /// <summary>
	// 	// /// check if the schema list is all valid objects<br/>
	// 	// /// list provided is assumed to be non-null and have one+ items<br/>
	// 	// /// return true if all good<br/>
	// 	// /// return false if any not valid
	// 	// /// </summary>
	// 	// private bool validateSchemaList(IList<Schema>? scList, string verStr)
	// 	// {
	// 	// 	if (scList == null) return false;
	// 	//
	// 	// 	string? verStrTst;
	// 	// 	bool result = true;
	// 	//
	// 	// 	foreach (Schema sc in scList)
	// 	// 	{
	// 	// 		verStrTst = xLib.ExtractVersionFromName(sc.SchemaName);
	// 	//
	// 	// 		if (!sc.IsValidObject || verStrTst.IsVoid() || !verStrTst!.Equals(verStr))
	// 	// 		{
	// 	// 			result = false;
	// 	// 			break;
	// 	// 		}
	// 	// 	}
	// 	//
	// 	// 	return result;
	// 	// }
	//
	// 	// /// <summary>
	// 	// /// check if the schema list is all valid objects<br/>
	// 	// /// list provided is assumed to be non-null and have one+ items<br/>
	// 	// /// return the list of schema items with the result of the validation<br/>
	// 	// /// return true if all good<br/>
	// 	// /// return false if any is not good
	// 	// /// </summary>
	// 	// private bool validateSchemaList(IList<Schema>? scList, string verStr, out ExList<Schema> exList)
	// 	// {
	// 	// 	exList = new ExList<Schema>();
	// 	//
	// 	// 	if (scList == null || scList.Count == 0) return false;
	// 	//
	// 	// 	string? verStrTst;
	// 	// 	string name;
	// 	//
	// 	// 	foreach (Schema sc in scList)
	// 	// 	{
	// 	// 		name = sc.SchemaName;
	// 	// 		exList.Add(name, sc);
	// 	// 		
	// 	// 		if (!sc.IsValidObject)
	// 	// 		{
	// 	// 			exList.SetInvalid(name);
	// 	// 		}
	// 	// 		
	// 	// 		verStrTst = xLib.ExtractVersionFromName(name);
	// 	// 		
	// 	// 		if (verStrTst.IsVoid() || !verStrTst!.Equals(verStr)) exList.SetWrongVersion(name);
	// 	// 	}
	// 	//
	// 	// 	return !exList.GotNoGood;
	// 	// }
	//
	// 	/* verify datastorage */
	//
	// 	// /// <summary>
	// 	// /// check if the ds list is all valid objects<br/>
	// 	// /// list provided is assumed to be non-null and have one+ items<br/>
	// 	// /// return 0 if all good<br/>
	// 	// /// return 2 if any bad
	// 	// /// </summary>
	// 	// private ValidateDataStorage findAndValidateWbkDs(string searchName, string tstModelName, out IList<DataStorage>? dsList)
	// 	// {
	// 	// 	// this search name gets all datastorages regardless of model code fot this model
	// 	// 	// 0 if good, 1 if not
	// 	// 	ValidateDataStorage status = VDS_GOOD;
	// 	// 	ActivateStatus actStat;
	// 	// 	string modelName;
	// 	//
	// 	// 	Debug.WriteLine("\n\nfind workbook ds");
	// 	//
	// 	// 	// if return false, none found, return 1
	// 	// 	// if (FindAllDataStorages(searchName, out dsList) == false) return 1;
	// 	// 	if (!xLib.FindAllDataStorageByNamePrefix(searchName, out dsList)) return VDS_MISSING;
	// 	//
	// 	// 	foreach (DataStorage ds in dsList)
	// 	// 	{
	// 	//
	// 	// 		// this must come before the tests which use the schema
	// 	// 		if (!ds.IsValidObject || ds.GetEntitySchemaGuids().Count != 1)
	// 	// 		{
	// 	// 			status = VDS_INVALID;
	// 	// 			break;
	// 	// 		}
	// 	//
	// 	// 		if (xMui.WbkScResCode == VSC_GOOD)
	// 	// 		{
	// 	// 			modelName = xMgr.ReadModelName(ds, xData.TempWbkSchemaEx) ?? "";
	// 	// 			actStat = xLib.ReadActStatus(ds, xData.TempWbkSchemaEx);
	// 	//
	// 	// 			if (actStat == ActivateStatus.AS_INACTIVE)
	// 	// 			{
	// 	// 				status = VDS_ACT_OFF;
	// 	// 				break;
	// 	// 			}
	// 	//
	// 	// 			if (actStat == ActivateStatus.AS_IGNORE)
	// 	// 			{
	// 	// 				status = VDS_ACT_IGNORE;
	// 	// 				break;
	// 	// 			}
	// 	//
	// 	// 			if (!modelName.Equals(tstModelName))
	// 	// 			{
	// 	// 				status = VDS_WRONG_MODEL_NAME;
	// 	// 				break;
	// 	// 			}
	// 	// 		}
	// 	// 	}
	// 	//
	// 	// 	// when here, if result is false, at least one ds is bad
	// 	// 	return status;
	// 	// }
	//
	// 	// /// <summary>
	// 	// /// check if the ds list is all valid objects<br/>
	// 	// /// list provided is assumed to be non-null and have one+ items<br/>
	// 	// /// return 0 if all good<br/>
	// 	// /// return 2 if any bad
	// 	// /// </summary>
	// 	// private ValidateDataStorage findAndValidateShtDs(string searchName, out IList<DataStorage>? dsList)
	// 	// {
	// 	// 	// this search name gets all datastorages regardless of model code fot this model
	// 	// 	// 0 if good, 1 if not
	// 	// 	ValidateDataStorage status = VDS_GOOD;
	// 	// 	// string modelCode = "";
	// 	//
	// 	// 	// if return false, none found, return 1
	// 	// 	// if (FindAllDataStorages(searchName, out dsList) == false) return 1;
	// 	//
	// 	// 	Debug.WriteLine("\n\nfind sheet ds");
	// 	//
	// 	// 	if (!xLib.FindAllDataStorageByNamePrefix(searchName, out dsList)) return VDS_MISSING;
	// 	//
	// 	// 	foreach (DataStorage ds in dsList)
	// 	// 	{
	// 	// 		// possible issues
	// 	// 		// invalid = revit says invalid
	// 	// 		// wrong version - version in name is worng
	// 	//
	// 	// 		if (!ds.IsValidObject || ds.GetEntitySchemaGuids().Count != 1)
	// 	// 		{
	// 	// 			status = VDS_INVALID;
	// 	// 			break;
	// 	// 		}
	// 	// 	}
	// 	//
	// 	// 	return status;
	// 	// }
	//
	// 	/// <summary>
	// 	/// check if the ds list is all valid objects<br/>
	// 	/// list provided is assumed to be non-null and have one+ items<br/>
	// 	/// return 0 if all good<br/>
	// 	/// return 2 if any bad
	// 	/// </summary>
	// 	private ValidateDataStorage findAndValidateShtDs(string searchName, 
	// 		string verTest, out ExList<DataStorage>? exList)
	// 	{
	// 		// for sht, can have none or more ds - all with or without the current version
	//
	// 		ValidateDataStorage status = VDS_GOOD;
	// 		IList<DataStorage>? dsList;
	// 		ExListItem<DataStorage> exi;
	// 		string name;
	// 		string? verStrTst;
	//
	// 		exList = new ();
	// 		xData.TempShtDsList = null;
	//
	// 		// test 1
	// 		// if schema is no good, this is also no good
	// 		if (xMui.ShtScResCode != VSC_GOOD && xMui.ShtScResCode != VSC_WRONG_VER) return VDS_INVALID;
	// 		// if (xMui.ShtScResCode == VSC_INVALID || xMui.ShtScResCode == VSC_MISSING) return VDS_INVALID;
	//
	// 		// proc 1 / test 2
	// 		// find the sht ds - if none, invalid
	//
	// 		// if (!xLib.FindAllDataStorageByNamePrefix(searchName, out dsList)) return VDS_INVALID;
	// 		if (!xMgr.FindSheetsDs(out dsList)) return VDS_INVALID;
	//
	// 		// save the this list in case need to delete them later
	// 		xData.TempShtDsList = dsList;
	//
	// 		// got some, any with the correct version (greater than zero is OK)
	// 		// zero is also OK but gets processed different
	// 		// test 3
	// 		if (dsList.Count == 0) return VDS_WRONG_COUNT;
	//
	// 		// per test 1, VSC must be good or wrong ver
	// 		// process the list of good items and validate + add to exlist
	// 		foreach (DataStorage ds in dsList)
	// 		{
	// 			name = ds.Name;
	//
	// 			exi = new ExListItem<DataStorage>(name, ds);
	//
	// 			// validate
	// 			// test 4
	// 			if (!ds.IsValidObject || ds.GetEntitySchemaGuids().Count != 1)
	// 			{
	// 				status = VDS_INVALID;
	// 				exi.SetNotValid();
	// 			}
	//
	// 			// per test 1, VSC must be good or wrong ver
	// 			// only if VSC is wrong ver, may have wrong ver here
	// 			if (xMui.ShtScResCode == VSC_WRONG_VER)
	// 			{
	// 				verStrTst = xMgr.ExtractVersionFromName(ds.Name);
	//
	// 				// test 8
	// 				if (verStrTst.IsVoid() || !ExStorConst.EXS_VERSION_SHT.Equals(verStrTst))
	// 				{
	// 					status = status == VDS_GOOD ? VDS_WRONG_VER : VDS_MULTIPLE_MN_O;
	// 					exi.SetWrongVersion();
	// 				}
	// 			}
	//
	// 			exList.Add(exi);
	// 		}
	//
	// 		return status;
	// 	}
	//
	//
	// #endregion
	//
	// #region private resolve methods
	//
	// 	// in all cases, the resolution methods below just flag what nees to be done
	// 	// when the window is activated
	//
	// 	// F
	// 	/// <summary>
	// 	/// driver to attempt to resolve any startup issues
	// 	/// </summary>
	// 	private void verifyResolver()
	// 	{
	// 		writeLineBeg("F *** BEGIN - verification resolver ***\n", "\n");
	// 		
	// 		// pre-assign as good & prove wrong
	// 		// exSysStatusLocal = ES_VRFY_RSLV_GOOD;
	//
	// 		ValidateSchema wSc = resultWbkSc ;
	// 		ValidateSchema sSc = resultShtSc ;
	// 		ValidateDataStorage wDs = resultWbkDs ;
	// 		ValidateDataStorage sDs = resultShtDs ;
	//
	// 		// if (!useAltTest)
	// 		// {
	// 		//
	// 		// 	if (vfyRslvWs())
	// 		// 	{
	// 		// 		if (vfyRslvSs())
	// 		// 		{
	// 		// 			if (vfyRslvWd())
	// 		// 			{
	// 		// 				vfyRslvSd();
	// 		// 			}
	// 		// 		}
	// 		// 	}
	// 		// }
	// 		// else
	// 		// {
	// 			vfyRslvWs();
	// 			vfyRslvWd();
	//
	// 			vfyRslvSs();
	// 			vfyRslvSd();
	// 		// }
	// 		// ExSysStatus = exSysStatusLocal;
	//
	// 		writeLineEnd("F *** END - verification resolver ***\n", "\n");
	// 	}
	//
	// 	/// <summary>
	// 	/// resolve any issues related to workbook schema
	// 	/// </summary>
	// 	private bool vfyRslvWs()
	// 	{
	// 		// possible inputs & outputs
	// 		//						set ExSysStatus
	// 		// VSC_GOOD				ES_VRFY_DONE_GOOD
	// 		// VSC_MISSING			ES_VRFY_DONE_MISSING
	// 		// VSC_INVALID			ES_VRFY_DONE_INVALID
	// 		// VSC_OUT_OF_DATE		(ignore for now) ES_VRFY_DONE_BAD_VER
	//
	// 		writeLineBeg($"resolve Wbk Sc? {resultWbkSc != VSC_GOOD} [{resultWbkSc}]");
	//
	// 		// if issue, attempt to resolve
	// 		if (resultWbkSc != VSC_GOOD)
	// 		{
	// 			if (resultWbkSc == VSC_MISSING) _ExSysStatus = ES_VRFY_DONE_MISSING;
	// 			else if (resultWbkSc == VSC_INVALID) _ExSysStatus = ES_VRFY_DONE_INVALID;
	// 			else if (resultWbkSc == VSC_WRONG_VER) _ExSysStatus = ES_VRFY_DONE_BAD_VER;
	// 			else if (resultWbkSc == VSC_WRONG_COUNT) _ExSysStatus = ES_VRFY_DONE_INVALID;
	//
	// 			// if (resultWbkSc == VSC_MISSING || resultWbkSc == VSC_INVALID)
	// 			// {
	// 			// 	_ExSysStatus = ES_VRFY_DONE_RESTART;
	// 			// 	_LaunchCode = LC_DONE_RESTART;
	// 			// }
	// 			// else
	// 			// if (resultWbkSc == VSC_OUT_OF_DATE)
	// 			// {
	// 			// 	resultWbkDs = VDS_OUT_OF_DATE;
	// 			//
	// 			// 	_ExSysStatus = ES_VRFY_DONE_RECONFIG;
	// 			// 	_LaunchCode = LC_DONE_UPGRADE;
	// 			// }
	// 			// else
	// 			// {
	// 			// 	_ExSysStatus = ES_VRFY_DONE_FAIL;
	// 			// 	_LaunchCode = LC_DONE_INVALID;
	// 			// }
	//
	// 			writeLineEnd($"wbk schema resolver result -> {_ExSysStatus}", "\n");
	//
	// 			return false;
	// 		}
	//
	// 		_ExSysStatus = ES_VRFY_DONE_GOOD;
	//
	// 		writeLineEnd($"wbk schema resolver result -> {_ExSysStatus}", "\n");
	//
	// 		return true;
	// 	}
	//
	// 	/// <summary>
	// 	/// resolve issues related to sheet schema
	// 	/// </summary>
	// 	private bool vfyRslvSs()
	// 	{
	// 		writeLineBeg($"resolve sht Sc? {resultShtSc != VSC_GOOD} [{resultShtSc}]");
	//
	// 		// if issue, attempt to resolve
	// 		if (resultShtSc != VSC_GOOD )
	// 		{
	// 			if (resultShtSc == VSC_MISSING) _LaunchCode = LC_DONE_MISSING;
	// 			else if (resultShtSc == VSC_INVALID) _LaunchCode = LC_DONE_INVALID;
	// 			else if (resultShtSc == VSC_WRONG_VER) _LaunchCode = LC_DONE_BAD_VER;
	//
	// 			// if (resultShtSc == VSC_MISSING || resultShtSc == VSC_INVALID)
	// 			// {
	// 			// 	_LaunchCode = LC_DONE_RESTART;
	// 			// 	_ExSysStatus = ES_VRFY_DONE_RESTART;
	// 			// }
	// 			// else
	// 			// if (resultShtSc == VSC_OUT_OF_DATE)
	// 			// {
	// 			// 	resultShtDs = VDS_OUT_OF_DATE;
	// 			//
	// 			// 	_LaunchCode = LC_DONE_UPGRADE;
	// 			// 	_ExSysStatus = ES_VRFY_DONE_RECONFIG;
	// 			// }
	// 			// else
	// 			// {
	// 			// 	_LaunchCode = LC_DONE_INVALID;
	// 			// 	_ExSysStatus = ES_VRFY_DONE_FAIL;
	// 			// }
	//
	// 			writeLineEnd($"sht schema resolver result -> {_LaunchCode}", "\n");
	//
	// 			return false;
	// 		}
	//
	// 		_LaunchCode = LC_DONE_GOOD;
	//
	// 		writeLineEnd($"sht schema resolver result -> {_LaunchCode}");
	//
	// 		return true;
	// 	}
	//
	// 	/// <summary>
	// 	/// resolve issues related to workbook datastorage element
	// 	/// </summary>
	// 	private bool vfyRslvWd()
	// 	{
	// 		// possible inputs & outputs
	// 		//							set ExSysStatus
	// 		// VDS_GOOD					ignore, pass through
	// 		// VDS_WRONG_MODEL_NAME		ES_VRFY_DONE_MOD_NAME
	// 		// VDS_ACT_OFF				ES_VRFY_DONE_ACT_OFF
	// 		// VDS_ACT_IGNORE			ES_VRFY_DONE_ACT_IGNORE
	// 		// VDS_MISSING				ES_VRFY_DONE_MISSING
	// 		// VDS_INVALID				ES_VRFY_DONE_INVALID
	// 		// VDS_OUT_OF_DATE			(ignore for now) ES_VRFY_DONE_BAD_VER
	//
	// 		writeLineBeg($"resolve wbk ds? {resultWbkDs != VDS_GOOD} [{resultWbkDs}]");
	//
	// 		// if issue, attempt to resolve
	// 		if (resultWbkDs != VDS_GOOD)
	// 		{
	// 			if (resultWbkDs == VDS_WRONG_MODEL_NAME) _ExSysStatus = ES_VRFY_DONE_MOD_NAME;
	// 			else if (resultWbkDs == VDS_ACT_OFF)     _ExSysStatus = ES_VRFY_DONE_ACT_OFF;
	// 			else if (resultWbkDs == VDS_ACT_IGNORE)  _ExSysStatus = ES_VRFY_DONE_ACT_IGNORE;
	// 			else if (resultWbkDs == VDS_MISSING)     _ExSysStatus = ES_VRFY_DONE_MISSING;
	// 			else if (resultWbkDs == VDS_INVALID)     _ExSysStatus = ES_VRFY_DONE_INVALID;
	// 			else if (resultWbkDs == VDS_WRONG_VER)   _ExSysStatus = ES_VRFY_DONE_BAD_VER;
	//
	//
	// 			// if (resultWbkDs == VDS_MISSING)
	// 			// {
	// 			// 	if (resultShtDs == VDS_GOOD || resultShtDs == VDS_MISSING)
	// 			// 	{
	// 			// 		_LaunchCode = LC_DONE_INVALID;
	// 			// 		_ExSysStatus = ES_VRFY_DONE_RECONFIG;
	// 			// 		
	// 			// 		// result = resultShtDs;
	// 			// 	}
	// 			// 	else
	// 			// 	{
	// 			// 		_LaunchCode = LC_DONE_RESTART;
	// 			// 		_ExSysStatus = ES_VRFY_DONE_HOLD_WBK;
	// 			// 	}
	// 			// }
	// 			// else if (resultWbkDs == VDS_INVALID)
	// 			// {
	// 			// 	_LaunchCode = LC_DONE_RESTART;
	// 			// 	_ExSysStatus = ES_VRFY_DONE_HOLD_WBK;
	// 			// }
	// 			// else if (resultWbkDs == VDS_OUT_OF_DATE)
	// 			// {
	// 			// 	_LaunchCode = LC_DONE_UPGRADE;
	// 			// 	_ExSysStatus = ES_VRFY_DONE_RECONFIG;
	// 			// }
	// 			// else if (resultWbkDs == VDS_WRONG_MODEL_NAME)
	// 			// {
	// 			// 	_LaunchCode = LC_DONE_INVALID;
	// 			// 	_ExSysStatus = ES_VRFY_DONE_RECONFIG;
	// 			// }
	// 			// else if (resultWbkDs == VDS_ACT_OFF || resultWbkDs == VDS_ACT_IGNORE)
	// 			// {
	// 			// 	_LaunchCode = LC_DONE_HOLD;
	// 			// 	_ExSysStatus = ES_VRFY_DONE_HOLD_ACT;
	// 			// }
	// 			// else
	// 			// {
	// 			// 	_LaunchCode = LC_DONE_INVALID;
	// 			// 	_ExSysStatus = ES_VRFY_DONE_FAIL;
	// 			// }
	//
	// 			writeLineEnd($"wbk ds resolver result  -> {_ExSysStatus}\n");
	// 			
	// 			return false;
	// 		}
	//
	// 		// do not set when this is good -  this allows the sc setting 
	// 		// to pass through
	// 		// _ExSysStatus = ES_VRFY_DONE_GOOD;
	//
	// 		writeLineEnd($"wbk ds resolver result -> {_ExSysStatus}");
	//
	// 		return true;
	// 	}
	//
	// 	/// <summary>
	// 	/// resolve issues related to sheet datastorage element
	// 	/// </summary>
	// 	private void vfyRslvSd()
	// 	{
	// 		// possible inputs & outputs
	// 		//							set LaunchCode
	// 		// VDS_GOOD					ignore, pass through
	// 		// VDS_WRONG_MODEL_CODE		LC_DONE_MOD_CODE
	// 		// VDS_MISSING				LC_DONE_MISSING
	// 		// VDS_INVALID				LC_DONE_INVALID
	// 		// VDS_OUT_OF_DATE			(ignore for now) LC_DONE_BAD_VER
	//
	// 		writeLineBeg($"resolve sht ds? {resultShtDs != VDS_GOOD} [{resultShtDs}]");
	//
	// 		// if issue, attempt to resolve
	// 		if (resultShtDs != VDS_GOOD )
	// 		{
	// 			// if (resultShtDs == VDS_WRONG_MODEL_CODE) _LaunchCode = LC_DONE_MOD_CODE;
	// 			if (resultShtDs == VDS_MISSING) _LaunchCode = LC_DONE_MISSING;
	// 			else if (resultShtDs == VDS_INVALID)   _LaunchCode = LC_DONE_INVALID;
	// 			else if (resultShtDs == VDS_WRONG_VER) _LaunchCode = LC_DONE_BAD_VER;
	//
	// 			// if (resultShtDs == VDS_MISSING)
	// 			// {
	// 			// 	_LaunchCode = LC_DONE_GOOD;
	// 			// 	_ExSysStatus = ES_VRFY_DONE_HOLD_SHT;
	// 			// }
	// 			// else if (resultShtDs == VDS_INVALID)
	// 			// {
	// 			// 	_LaunchCode = LC_DONE_INVALID;
	// 			// 	_ExSysStatus = ES_VRFY_DONE_HOLD_SHT;
	// 			// }
	// 			// else if (resultShtDs == VDS_OUT_OF_DATE)
	// 			// {
	// 			// 	_LaunchCode = LC_DONE_UPGRADE;
	// 			// 	_ExSysStatus = ES_VRFY_DONE_RECONFIG;
	// 			// }
	// 			// else if (resultShtDs == VDS_WRONG_MODEL_CODE)
	// 			// {
	// 			// 	_LaunchCode = LC_DONE_INVALID;
	// 			// 	_ExSysStatus = ES_VRFY_DONE_RECONFIG;
	// 			// }
	// 			// else
	// 			// {
	// 			// 	_LaunchCode = LC_DONE_INVALID;
	// 			// 	_ExSysStatus = ES_VRFY_DONE_FAIL;
	// 			// }
	//
	// 			writeLineEnd($"sht ds resolver result -> {_LaunchCode}\n");
	//
	// 			return;
	// 		}
	//
	// 		writeLineEnd($"sht ds resolver result -> {_LaunchCode}");
	// 	}
	//
	// #endregion


	// #region event consuming
	//
	// #endregion
	//
	// #region event publishing
	//
	// #endregion
	//
	// #region system overrides
	//
	// 	public override string ToString()
	// 	{
	// 		return $"{nameof(ExStorLaunchMgr)} [{ObjectId}]";
	// 	}
	//
	// #endregion
	//
	// 	private bool? sendDebug = false;
	//
	// 	private readonly Tuple<ValidateSchema, ValidateSchema, ValidateDataStorage, ValidateDataStorage>[] _tstData1 = 
	// 	[
	// 		new (VSC_GOOD                , VSC_GOOD                  , VDS_GOOD                 , VDS_GOOD),
	// 		new (VSC_NA                  , VSC_NA                    , VDS_NA                   , VDS_NA),
	// 		new (VSC_MISSING             , VSC_VRFY_UNTESTED         , VDS_VRFY_UNTESTED        , VDS_VRFY_UNTESTED),
	// 		new (VSC_INVALID			 , VSC_VRFY_UNTESTED         , VDS_INVALID				, VDS_VRFY_UNTESTED),
	// 		new (VSC_GOOD                , VSC_MISSING               , VDS_VRFY_UNTESTED        , VDS_VRFY_UNTESTED),
	// 		new (VSC_GOOD                , VSC_INVALID				 , VDS_VRFY_UNTESTED        , VDS_INVALID),
	// 		new (VSC_GOOD                , VSC_GOOD                  , VDS_MISSING              , VDS_GOOD),
	// 		new (VSC_GOOD                , VSC_GOOD                  , VDS_MISSING              , VDS_MISSING),
	// 		new (VSC_GOOD                , VSC_GOOD                  , VDS_MISSING              , VDS_INVALID),
	// 		new (VSC_GOOD                , VSC_GOOD                  , VDS_INVALID              , VDS_GOOD),
	// 		new (VSC_GOOD                , VSC_GOOD                  , VDS_WRONG_MODEL_NAME     , VDS_GOOD),
	// 		new (VSC_GOOD                , VSC_GOOD                  , VDS_ACT_OFF              , VDS_GOOD),
	// 		new (VSC_GOOD                , VSC_GOOD                  , VDS_ACT_IGNORE           , VDS_GOOD),
	// 		new (VSC_GOOD                , VSC_GOOD                  , VDS_GOOD                 , VDS_MISSING),
	// 		new (VSC_GOOD                , VSC_GOOD                  , VDS_GOOD                 , VDS_INVALID),
	// 		// removed
	// 		// new (VSC_INVALID             , VSC_VRFY_UNTESTED         , VDS_VRFY_UNTESTED        , VDS_VRFY_UNTESTED),
	// 		// new (VSC_GOOD                , VSC_INVALID               , VDS_VRFY_UNTESTED        , VDS_VRFY_UNTESTED),
	// 		// new (VSC_GOOD        , VSC_GOOD         ,  VDS_MISSING         , VDS_WRONG_MODEL_CODE),
	// 		// new (VSC_GOOD        , VSC_GOOD         ,  VDS_GOOD            , VDS_WRONG_MODEL_CODE),
	// 	];
	//
	// 	private readonly Tuple<ValidateSchema, ValidateSchema, ValidateDataStorage, ValidateDataStorage, ExSysStatus, LaunchCode>[] _tstData2 =
	// 		[
	// 			new (VSC_GOOD       	, VSC_GOOD					, VDS_GOOD					, VDS_GOOD          , ES_VRFY_DONE_GOOD         , LC_DONE_GOOD      ), 
	// 			new (VSC_GOOD       	, VSC_GOOD					, VDS_WRONG_MODEL_NAME		, VDS_GOOD          , ES_VRFY_DONE_MOD_NAME     , LC_DONE_GOOD      ), 
	// 			new (VSC_GOOD       	, VSC_GOOD					, VDS_ACT_OFF				, VDS_GOOD          , ES_VRFY_DONE_ACT_OFF      , LC_DONE_GOOD      ), 
	// 			new (VSC_GOOD       	, VSC_GOOD					, VDS_ACT_IGNORE			, VDS_GOOD          , ES_VRFY_DONE_ACT_IGNORE   , LC_DONE_GOOD      ), 
	// 			new (VSC_GOOD       	, VSC_INVALID				, VDS_GOOD					, VDS_INVALID		, ES_VRFY_DONE_GOOD         , LC_DONE_BAD_VER   ), 
	// 			new (VSC_GOOD       	, VSC_INVALID				, VDS_WRONG_MODEL_NAME		, VDS_INVALID		, ES_VRFY_DONE_MOD_NAME     , LC_DONE_BAD_VER   ), 
	// 			new (VSC_GOOD       	, VSC_INVALID				, VDS_ACT_OFF				, VDS_INVALID		, ES_VRFY_DONE_ACT_OFF      , LC_DONE_BAD_VER   ), 
	// 			new (VSC_GOOD       	, VSC_INVALID				, VDS_ACT_IGNORE			, VDS_INVALID		, ES_VRFY_DONE_ACT_IGNORE   , LC_DONE_BAD_VER   ), 
	// 			new (VSC_GOOD       	, VSC_MISSING				, VDS_GOOD					, VDS_MISSING       , ES_VRFY_DONE_GOOD         , LC_DONE_MISSING   ), 
	// 			new (VSC_MISSING    	, VSC_NA					, VDS_MISSING				, VDS_NA            , ES_VRFY_DONE_MISSING      , LC_DEFAULT        ), 
	// 			new (VSC_INVALID		, VSC_GOOD					, VDS_INVALID				, VDS_GOOD          , ES_VRFY_DONE_BAD_VER      , LC_DONE_GOOD      ), 
	// 			new (VSC_INVALID		, VSC_MISSING				, VDS_INVALID				, VDS_MISSING       , ES_VRFY_DONE_BAD_VER      , LC_DONE_MISSING   ), 
	// 			new (VSC_INVALID		, VSC_INVALID				, VDS_INVALID				, VDS_INVALID		, ES_VRFY_DONE_BAD_VER      , LC_DONE_BAD_VER   ), 
	// 			// removed
	// 			// new (VSC_GOOD       			, VSC_INVALID				, VDS_GOOD					, VDS_INVALID           	, ES_VRFY_DONE_GOOD         , LC_DONE_INVALID   ), 
	// 			// new (VSC_INVALID    			, VSC_NA					, VDS_INVALID				, VDS_NA                	, ES_VRFY_DONE_INVALID      , LC_DEFAULT        ), 
	// 			// new (VSC_INVALID	, VSC_INVALID				, VDS_INVALID	, VDS_INVALID           	, ES_VRFY_DONE_BAD_VER          , LC_DONE_INVALID   ), 
	// 			// new (VSC_GOOD       	, VSC_GOOD			, VDS_WRONG_MODEL_NAME		, VDS_WRONG_MODEL_CODE  	, ES_VRFY_DONE_MOD_NAME     , LC_DONE_MOD_CODE  ), 
	// 			// new (VSC_GOOD       	, VSC_GOOD			, VDS_ACT_OFF				, VDS_WRONG_MODEL_CODE  	, ES_VRFY_DONE_ACT_OFF      , LC_DONE_MOD_CODE  ), 
	// 			// new (VSC_GOOD       	, VSC_GOOD			, VDS_ACT_IGNORE			, VDS_WRONG_MODEL_CODE  	, ES_VRFY_DONE_ACT_IGNORE   , LC_DONE_MOD_CODE  ), 
	// 			// new (VSC_OUT_OF_DATE	, VSC_GOOD			, VDS_OUT_OF_DATE			, VDS_WRONG_MODEL_CODE  	, ES_VRFY_DONE_BAD_VER          , LC_DONE_MOD_CODE  ), 
	// 		];
	//

	//
	//
	//
	// 	public void TestVfy2()
	// 	{
	// 		sendDebug = null;
	//
	// 		Debug.WriteLine($"\n{"WbkSc", COL_A} , {"ShtSc",COL_B} , {"WbkDs",COL_C} , {"ShtDs",COL_D} | {" ",COL_F}| {"LC",COL_L} / {"ES", COL_E} | {"WbkSc",COL_A} , {"ShtSc",COL_B} , {"WbkDs",COL_C} , ShtDs");
	//
	// 		foreach ((ValidateSchema wSc, ValidateSchema sSc, ValidateDataStorage wDs, ValidateDataStorage sDs) in _tstData1)
	// 		{
	// 			resultWbkSc = wSc;
	// 			resultShtSc = sSc;
	// 			resultWbkDs = wDs;
	// 			resultShtDs = sDs;
	//
	// 			bool result = wSc == VSC_GOOD && sSc == VSC_GOOD && wDs == VDS_GOOD && sDs == VDS_GOOD;
	//
	// 			// WriteLine("\nA *** begin TestVfy1 ***");
	// 			// tabDepth++;
	//
	// 			// WriteLineMid($"A TestVfy1 result {result}", "\n");
	//
	// 			// showStatus(0, 0, true); // wbk, schema
	// 			// showStatus(1, 0, true); // sht, schema
	// 			// showStatus(0, 1, true); // wbk, ds
	// 			// showStatus(1, 1, true); // sht, ds
	//
	// 			if (!result)
	// 			{
	// 				// onOpenDocLaunchRslv();
	// 			}
	//
	// 			// onOpenDocLaunchFinish();
	//
	// 			// tabDepth--;
	// 			// WriteLine("\nA *** complete TestVfy1 ***\n");
	//
	// 			Debug.WriteLine($"{wSc,COL_A} , {sSc,COL_B} , {wDs,COL_C} , {sDs, COL_D} | {"final answer",COL_F}| {_LaunchCode,COL_L} / {_ExSysStatus, COL_E} | {resultWbkSc,COL_A} , {resultShtSc,COL_B} , {resultWbkDs,COL_C} , {resultShtDs, COL_D} | {result}");
	//
	// 		}
	//
	// 		sendDebug = false;
	// 	}
	//
	//
	// 	private bool useAltTest;
	//
	//
	// 	public void TestVfy1()
	// 	{
	// 		useAltTest = true;
	// 		Msgs.WriteLine("\n\nAlt Test\n");
	//
	// 		for (int i = 0; i < 2; i++)
	// 		{
	// 			string s1;
	// 			string s2;
	//
	// 			sendDebug = null;
	// 			
	// 			// Msgs.WriteLine($"\n{"WbkSc", COL_A} , {"ShtSc",COL_B} , {"WbkDs",COL_C} , {"ShtDs",COL_D} | {" ",COL_F}| {"ES", COL_E} {"[match]",COL_G} / {"LC",COL_L}  {"[match]",COL_G} | {"WbkSc",COL_A} , {"ShtSc",COL_B} , {"WbkDs",COL_C} , ShtDs");
	// 			Msgs.WriteLine($"\n{"WbkSc", COL_A} , {"ShtSc",COL_B} , {"WbkDs",COL_C} , {"ShtDs",COL_D} | {" ",COL_F}| {"[match]",COL_G} {"ES", COL_E} / {"[match]",COL_G} {"LC",COL_L}");
	//
	// 			foreach ((ValidateSchema wSc, ValidateSchema sSc, ValidateDataStorage wDs, ValidateDataStorage sDs, ExSysStatus ex, LaunchCode lc) in _tstData2)
	// 			{
	//
	// 				resultWbkSc = wSc;
	// 				resultShtSc = sSc;
	// 				resultWbkDs = wDs;
	// 				resultShtDs = sDs;
	//
	// 				_ExSysStatus = ES_DEFAULT;
	// 				_LaunchCode  = LC_DEFAULT;
	//
	// 				bool result = wSc == VSC_GOOD && sSc == VSC_GOOD && wDs == VDS_GOOD && sDs == VDS_GOOD;
	// 				
	// 				// onOpenDocLaunchRslv();
	//
	// 				s1 = _ExSysStatus == ex ? "[ yep ]" : "[ NOPE ]";
	// 				s2 = _LaunchCode == lc ? "[ yep ]" : "[ NOPE ]";
	//
	// 				Msgs.WriteLine($"{wSc,COL_A} , {sSc,COL_B} , {wDs,COL_C} , {sDs, COL_D} | {"final answer",COL_F}| {s1,COL_G} {_ExSysStatus, COL_E} / {s2,COL_G} {_LaunchCode,COL_L} | {result}");
	//
	// 			}
	//
	// 			sendDebug = false;
	//
	// 			break;
	//
	// 			// Msgs.CWriteCache();
	//
	// 			// useAltTest = true;
	// 			//
	// 			// Msgs.WriteLine("\n\nAlt Test\n");
	// 		}
	//
	// 	}
	//



		// /// <summary>
		// /// find and validate workbook data storage.
		// /// this must run after the schema is found as this needs the schema
		// /// to validate
		// /// </summary>
		// /// <remarks>
		// /// possible return values:<br/>
		// /// VDS_GOOD, VDS_INVALID, VDS_MISSING, VDS_WRONG_COUNT,
		// /// VDS_ACT_OFF, VDS_ACT_IGNORE, VDS_WRONG_MODEL_NAME, VDS_WRONG_VER, VDS_MULTIPLE_MN_O<br/>
		// /// the found ds, if any, is converted to an ExListItem and placed in xData
		// /// </remarks>
		// private ValidateDataStorage VerifyWbkDataStorage2()
		// {
		// 	ValidateDataStorage status = VDS_GOOD;
		// 	ActivateStatus actStat;
		// 	string modelName;
		// 	string? verStrTst;
		// 	IList<DataStorage> dsList;
		//
		// 	if (!xMgr.FindAllWbkDs(out dsList)) return VDS_MISSING;
		// 	if (dsList.Count != 1) return VDS_WRONG_COUNT;
		//
		// 	ExListItem<DataStorage> ds = new (dsList[0].Name, dsList[0]);
		//
		// 	if (!ds.Item.IsValidObject || ds.Item.GetEntitySchemaGuids().Count != 1) return VDS_INVALID;
		// 	if (!(xData.TempWbkSchemaEx?.IsValid ?? false)) return VDS_INVALID;
		//
		// 	actStat = xMgr.ReadActivationStatus(ds.Item, xData.TempWbkSchemaEx.Item);
		// 	if (actStat == ActivateStatus.AS_IGNORE) return VDS_ACT_IGNORE;
		//
		// 	if (actStat == ActivateStatus.AS_INACTIVE)
		// 	{
		// 		status = VDS_ACT_OFF;
		// 		ds.SetActIsOff();
		// 	}
		//
		// 	modelName = xMgr.ReadModelName(ds.Item, xData.TempWbkSchemaEx.Item) ?? "";
		//
		// 	if (!modelName.Equals(xMgr.Exid.ModelName))
		// 	{
		// 		status = status == VDS_GOOD ? VDS_WRONG_MODEL_NAME : VDS_MULTIPLE_MN_O;
		// 		ds.SetWrongModelName();
		// 	}
		//
		// 	verStrTst = xMgr.ExtractVersionFromName(ds.Name);
		//
		// 	// test 8
		// 	if (verStrTst.IsVoid() || !ExStorConst.EXS_VERSION_WBK.Equals(verStrTst))
		// 	{
		// 		status = status == VDS_GOOD ? VDS_WRONG_VER : VDS_MULTIPLE_MN_O;
		// 		ds.SetWrongVersion();
		// 	}
		//
		// 	xData.TempWbkDsEx = ds;
		//
		// 	return status;
		// }
		//

	}
}