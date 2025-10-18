using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStoreTest2026;
using ExStoreTest2026.DebugAssist;
using RevitLibrary;
using UtilityLibrary;
using static ExStorSys.WorkBookFieldKeys;

// username: jeffs
// created:  9/17/2025 11:01:00 PM


namespace ExStorSys
{
	/// <summary>
	/// Storage Manager - controls the primary admin operations
	/// </summary>
	public class ExStorMgr
	{
		public int ObjectId;

	#region objects

		public ExStorData xData;

		public ExStorLib xlib;
		private Schema? tempWbkSchema;
		private bool? restartRequired;

		/// <summary>
		/// primary storage of the root DataStorage information<br/>
		/// includes live schema, datastorage, and entity objects
		/// </summary>
		public WorkBook? WorkBook { get; protected set; }

		public Dictionary<string, Sheet> Sheets { get; protected set; }


		// temp objects used for various operations that do not need to be
		// kept in the workbook or sheet objects
		public Schema? TempWbkSchema
		{
			get => tempWbkSchema;
			set
			{
				if (value != null && value.Equals(tempWbkSchema)) return;
				tempWbkSchema = value;

				// SettingChangedEventArgs e = new SettingChangedEventArgs();
				// e.SettingId = SettingId.SI_GOT_TMP_WBK_SCHEMA;
				// DynaValue dv = new DynaValue(value);
				//
				// RaiseSettingChangedEvent(e);
			}
		}

		public IList<Schema>? TempWbkSchemaList { get; set; }
		public Schema? TempShtSchema { get; set; }
		public IList<Schema>? TempShtSchemaList { get; set; }

		public IList<DataStorage>? TempWbkDsList { get; set; }
		public DataStorage? TempWbkDs { get; set; }
		public Entity? TempWbkEntity { get; set; }

		public IList<DataStorage>? TempShtDsList { get; set; }
		public DataStorage? TempShtDs { get; set; }
		public Entity? TempShtEntity { get; set; }

		public string TempModelCode { get; private set; }



	#endregion

	#region private fields

	#endregion

	#region ctor

		// private static readonly Lazy<ExStorMgr> instance =
		// 	new Lazy<ExStorMgr>(() => new ExStorMgr());

		private ExStorMgr() { }

		private void init()
		{
			ObjectId = AppRibbon.ObjectIdx++;

			xData = ExStorData.Instance;

			xlib = ExStorLib.Instance;

			Exid = new Exid();

			WorkBook = WorkBook.CreateEmptyWorkBook();
			WorkBook.ExMgr = this;
			Sheets = new Dictionary<string, Sheet>();

			RestartRequired = null;
		}

		public static ExStorMgr Instance { get; set; }

		public static ExStorMgr Create()
		{
			Instance = new ExStorMgr();
			Instance.init();

			return Instance;
		}

		// public void Initialize()
		// {
		//
		// 	WorkBook = WorkBook.CreateEmptyWorkBook();
		// 	WorkBook.ExMgr = this;
		// 	Sheets = new Dictionary<string, Sheet>();
		//
		// 	IVokeCount = AppRibbon.InvokeCount++;
		// }

	#endregion

	#region public properties

		public Exid Exid { get; private set; }

		// shortcut

		// allowed to make access easier as this object can be
		// accessed when needed

		public bool? RestartRequired
		{
			get => restartRequired;
			private set
			{
				restartRequired = value;
				RaiseRestartReqdChangedEvent(value);
			}
		}

		public bool GotWorkBook => WorkBook != null;
		public bool GotSheets => (Sheets != null && Sheets.Count > 0);

		// true when workbook is not null (exists) and is empty
		public bool IsWorkBookEmpty => GotWorkBook && WorkBook!.IsEmpty;

		// verify that the datastorage object within workbook or sheet are still valid
		public bool GotWbkDs
		{
			get
			{
				if (WorkBook?.ExsDataStorage  == null) return false;

				if (WorkBook?.ExsDataStorage.IsValidObject ?? false) return true;

				// workbook is not a valid object, reset this object
				// todo - verify this is ok to remove
				// ResetWorkbook();

				return false;
			}
		}

		public bool GotShtDs(string name)
		{
			Sheet? sht;

			if (!Sheets.TryGetValue(name ?? "", out sht)) return false;

			if (sht.ExsDataStorage  == null) return false;

			if (sht.ExsDataStorage.IsValidObject) return true;

			// sheet is not a valid object, remove this object

			// todo - verify this is ok to remove
			// Sheets.Remove(name);

			return false;
		}

		public bool GotWbkSchema => WorkBook?.GotSchema ?? false;

		public bool GotShtSchema => (WorkBook?.ExsSheetSchema ?? null ) != null;


		public bool GotTempWbkSchema => (TempWbkSchema != null && TempWbkSchema.IsValidObject);
		public bool GotTempShtSchema => (TempShtSchema != null && TempShtSchema.IsValidObject);

		public bool GotTempWbkDs => (TempWbkDs != null && TempWbkDs.IsValidObject);
		public bool GotTempShtDs => (TempShtDs != null && TempShtDs.IsValidObject);

		public bool GotTempWbkEntity => (TempWbkEntity != null && TempWbkEntity.IsValid());
		public bool GotTempShtEntity => (TempShtEntity != null && TempShtEntity.IsValid());

		public bool GotTempWbkDsList => (TempWbkDsList != null && TempWbkDsList.Count > 0);
		public bool GotTempShtDsList => (TempShtDsList != null && TempShtDsList.Count > 0);

		public bool GotTempWbkSchemaList => (TempWbkSchemaList != null && TempWbkSchemaList.Count > 0);
		public bool GotTempShtSchemaList => (TempShtSchemaList != null && TempShtSchemaList.Count > 0);


	#endregion

	#region private properties

	#endregion

	#region methods

		/* utility */

		// A) if only one doc open, count components
		//		* count good => return true / do nothing
		//		* count bad => return false / => remove options
		// verify got both schema's and a valid wbk ds (and zero or more sht's)
		// b) if only one doc open, validate components, if right count.
		// c) remove
		//		* if one doc open, remove all (that apply)
		//		* if >1 doc open, remove wbk ds and sht ds
		// d) rename model name within the wbk ds

		// public bool CountExComponents()
		// {
		// 	// create schema list
		// 	// get wbk obj
		// 	// create sht list
		//
		// 	string schemaSearchName = ExStorConst.PRIMARY_SCHEMA_DESC;
		//
		// 	return true;
		// }

		/// <summary>
		/// reset the workbook object to null<br/>
		/// OUT == OUT_NORMAL_OP
		/// </summary>
		public void ResetWorkbook()
		{
			WorkBook = null;
		}

		public void ResetSheets()
		{
			Sheets = new Dictionary<string, Sheet>();
		}


		/* update entity field */

		public void UpdateWbkEntityField(WorkBookFieldKeys key, DynaValue value)
		{
			updateEntityField(key, WorkBook, value);
		}

		public void UpdateShtEntityField(string dsName, SheetFieldKeys key, DynaValue value)
		{
			updateEntityField(key, Sheets[dsName], value);
		}

		private void updateEntityField<Te>(Te key, ExStorDataObj<Te> exo, DynaValue value)
			where Te : Enum
		{
			xlib.UpdateField(key, exo, value);
		}


		/* workbook */

		private string getId()
		{
			string lastId = WorkBook.LastId;
			string nextId = ExStorConst.CreateNextIdCode(lastId);

			WorkBook.LastId = nextId;
			UpdateWbkEntityField(PK_AD_LAST_ID, new DynaValue(nextId));

			return nextId;
		}

		public void MakeWorkBook()
		{
			Msgs.WriteLineSpaced("step: MC|", ">>> start | create workbook");

			Msgs.WriteLine($"\t+++ MW1 got ds? {GotWbkDs} | is empty {IsWorkBookEmpty}  [{ExStorMgr.Instance.WorkBook?.IsEmpty}]");

			WorkBook wbk = WorkBook.CreateNewWorkBook();
			saveWorkBook(wbk);

			// Msgs.WriteLineSpaced("step: MC|", ">>> start | set model code");
			//
			// Exid.Instance.SetModelCode();
			//
			// // crete a workbook with default values
			// Msgs.WriteLineSpaced("step: CDw|", "wbk create default data");
			// WorkBook wb = WorkBook.CreateDefault();
			// // assign some actual values
			// Msgs.WriteLineSpaced("step: UMw|", "wbk update with actual data");
			// wb.UpdateWithCurrentData(Exid.ModelCode);

			// decision, create the sheet only when data exists

			// Msgs.WriteLineSpaced("step: CDw|", "sht create default data");
			// Sheet sht = Sheet.CreateDefault();
			//
			// Msgs.WriteLineSpaced("step: UMs|", "sht update model code");
			// sht.UpdateWithCurrentData(WorkBook.LastId);
			//
			// Msgs.WriteLineSpaced("step: SS|", "sht Save");
			// AddSheet(sht);
		}

		public void WriteWorkBook()
		{
			// ExStoreRtnCode rtnCode = xlib.WriteWorkBook();
			ExStoreRtnCode rtnCode = xlib.WriteWorkBook();

			if (rtnCode == ExStoreRtnCode.XRC_GOOD)
			{
				Msgs.WriteLine("\n**** WORKED ****\n");
			}
			else
			{
				Msgs.WriteLine("\n**** FAILED ****\n");
			}
		}

		public bool MakeEmptyWorkBook()
		{
			// xlib.MakeEmptyWorkBook();

			if (!GotWorkBook)
			{
				// is null
				WorkBook wbk = WorkBook.CreateEmptyWorkBook();
				saveWorkBook(wbk);
			}
			else
			{
				if (!IsWorkBookEmpty)
				{
					// is not null and is not empty - got a live workbook
					return false;
				}

				// is not null but is empty
			}

			return true;

			/* the empty workbook has these fields (that matter)
			* wbk schema name
			* sht schema name
			* model name
			* a guid
			* version
			*/
		}

		private void saveWorkBook(WorkBook wbk)
		{
			TempModelCode = wbk.ModelCode;
			Msgs.WriteLine($"\t+++ MW3 got ds? {GotWbkDs} | is empty {IsWorkBookEmpty}  [{ExStorMgr.Instance.WorkBook?.IsEmpty}]");
			WorkBook = wbk;
			Msgs.WriteLine($"\t+++ MW5 got ds? {GotWbkDs} | is empty {IsWorkBookEmpty}  [{ExStorMgr.Instance.WorkBook?.IsEmpty}]");
			WorkBook.ExMgr = this;
			Msgs.WriteLine($"\t+++ MW10 got ds? {GotWbkDs} | is empty {IsWorkBookEmpty}  [{ExStorMgr.Instance.WorkBook?.IsEmpty}]");
		}

		/* sheets */

		public void WriteSheet(string dsName)
		{
			Sheet s = GetSheet(dsName);

			ExStoreRtnCode rtnCode = xlib.WriteSheet(dsName);

			if (rtnCode == ExStoreRtnCode.XRC_GOOD)
			{
				Msgs.WriteLine("\n**** WORKED ****\n");
			}
			else
			{
				Msgs.WriteLine("\n**** FAILED ****\n");
			}
		}

		public string? MakeSheet(SheetCreationData sd)
		{
			string nextShtName = Exid.CreateShtDsName(getId());

			if (GotSheet(nextShtName)) return null;

			Sheet? s = Sheet.CreateSheet(nextShtName, sd);

			if (s == null) return null;

			AddSheet(s);

			return s.DsName;
		}

		public Sheet? MakeEmptySheet()
		{
			// gotta have a workbook - full not empty
			if (!GotWorkBook || IsWorkBookEmpty) return null;

			string nextShtName = Exid.CreateShtDsName(getId());

			Sheet? s = Sheet.CreateEmptySheet(nextShtName);

			if (s == null) return null;

			return s;
		}

		private void saveSheets(IList<DataStorage> dsList)
		{
			Sheet sht;

			foreach (DataStorage ds in dsList)
			{
				sht = Sheet.CreateEmptySheet(ds);
				sht.ExMgr = this;

				AddSheet(sht);
			}
		}


		/* sheets list */

		public bool AddSheet(Sheet sht)
		{
			return Sheets.TryAdd(sht.DsName, sht);
		}

		public bool UpdateSheet(Sheet sht)
		{
			if (!Sheets.ContainsKey(sht.DsName)) return false;

			Sheets[sht.DsName] = sht;

			return true;
		}

		public bool DeleteSheet(Sheet sht)
		{
			if (!Sheets.ContainsKey(sht.DsName)) return false;

			Sheets.Remove(sht.DsName);

			return true;
		}

		public Sheet GetSheet(string name)
		{
			if (!Sheets.ContainsKey(name)) return null;

			return Sheets[name];
		}

		public bool GotSheet(string name)
		{
			return Sheets.ContainsKey(name);
		}


		/* delete */

		public bool DeleteWbkSchema()
		{
			if (!GotWbkSchema) return false;

			if (eraseSchema(WorkBook!.ExsSchema))
			{
				WorkBook.ExsSchema = null;
				RestartRequired = true;
				return true;
			}

			return false;
		}


		public bool DeleteShtSchema()
		{
			if (!GotShtSchema) return false;

			bool result = eraseSchema(WorkBook!.ExsSheetSchema);

			return result;
		}



		public bool DeleteDsList(IList<DataStorage>? dsList)
		{
			if (dsList == null || dsList.Count == 0) return false;
			bool result = true;
			foreach (DataStorage ds in dsList)
			{
				string name = ds.Name;

				if (!deleteDs(ds))
				{
					result = false;
				}
			}

			return result;
		}

		public bool EraseScList(IList<Schema>? scList)
		{
			if (scList == null || scList.Count == 0) return false;
			RestartRequired = true;

			bool result = true;

			foreach (Schema sc in scList)
			{
				string name = sc.SchemaName;
				if (!eraseSchema(sc))
				{
					result = false;
				}
			}

			return result;
		}

		private bool deleteDs(DataStorage ds)
		{
			if (ds == null || !ds.IsValidObject) return false;

			return xlib.DeleteDs(ds) == ExStoreRtnCode.XRC_GOOD;
		}

		private bool eraseSchema(Schema? s)
		{
			if (s == null || !s.IsValidObject) return false;

			if (xlib.EraseSc(s) == ExStoreRtnCode.XRC_GOOD)
			{
				return true;
			}

			return false;
		}

		/* find */

		/// <summary>
		/// find a model's associated data storage object (if any)<br/>
		/// regardless of whether it already exists<br/>
		/// that is, search type == 0
		/// </summary>
		public bool FindWorkBookDs(out DataStorage? ds, out Entity? e, out Schema? s)
		{
			ds = null;
			e = null;
			s = null;

			if (GotWorkBook) return false;

			string searchText = ExStorConst.EXS_WBK_NAME_SEARCH;
			string matchText = Exid.Model_Name;


			return xlib.FindExStorInfo(0, ExStorConst.WbkSchemaName,
				searchText, matchText, out ds, out e, out s) == ExStoreRtnCode.XRC_GOOD;
		}

		/// <summary>
		/// find all data storage objects with the given name prefix<br/>
		/// return true if count > 0
		/// </summary>
		public bool FindAllDataStorages(string searchName, out IList<DataStorage>? dsList)
		{
			dsList = xlib.FindAllDataStorageByNamePrefix(searchName);

			return dsList.Count > 0;
		}

		/// <summary>
		/// find a model's associated data storage object (if any)<br/>
		/// using the model code and id code (that is, with a full DS name)<br/>
		/// that is, search type == 1
		/// </summary>
		public bool FindSheetDs(string modelCode, string id, out DataStorage? ds, out Entity? e, out Schema? s)
		{
			ds = null;
			e = null;
			s = null;

			string searchText = Exid.CreateShtDsName(id, modelCode);

			if (Sheets != null && Sheets.ContainsKey(searchText)) return false;

			string matchText = null;

			return xlib.FindExStorInfo(1, ExStorConst.ShtSchemaName,
				searchText, matchText, out ds, out e, out s) == ExStoreRtnCode.XRC_GOOD;
		}

		public bool FindAndSaveSheets()
		{
			if (!GotWorkBook) return false;

			IList<DataStorage> dsList;

			if (xlib.FindSheetsDs(WorkBook!, out dsList) != ExStoreRtnCode.XRC_GOOD) return false;

			saveSheets(dsList);

			return true;
		}

		public bool FindSheetsDs(out IList<DataStorage> dsList)
		{
			dsList = null;

			if (!GotWorkBook) return false;

			if (xlib.FindSheetsDs(WorkBook, out dsList) != ExStoreRtnCode.XRC_GOOD) return false;

			return true;
		}

		/// <summary>
		/// find all schema that start with the search name provided<br/>
		/// true == one+ found | false == none found
		/// </summary>
		public bool FindAllSchema(string searchName, out IList<Schema>? schemas)
		{
			return xlib.FindAllSchema(searchName, out schemas) == ExStoreRtnCode.XRC_GOOD;
		}

		public bool FindToTempWbkSchema()
		{
			IList<Schema>? scList;
			bool result = FindAllSchema(Exid.WbkSearchName, out scList);

			if (result) { TempWbkSchemaList = scList; }

			return scList != null && scList.Count > 0;
		}

		public bool FindToTempWbkDs()
		{
			IList<DataStorage>? dsList;
			bool result = FindAllDataStorages(Exid.WbkSearchName, out dsList);

			if (result) { TempWbkDsList = dsList; }

			return dsList != null && dsList.Count > 0;
		}

		public bool FindToTempShtSchema()
		{
			IList<Schema>? scList;
			bool result = FindAllSchema(Exid.ShtSearchName, out scList);

			if (result) { TempShtSchemaList = scList; }

			return scList != null && scList.Count > 0;
		}

		public bool FindToTempShtDs()
		{
			IList<DataStorage>? dsList;
			bool result = FindAllDataStorages(Exid.ShtSearchName, out dsList);

			if (result) { TempShtDsList = dsList; }

			return dsList != null && dsList.Count > 0;
		}

		/* read */

		public string? ReadModelCode(Entity e)
		{
			if (!e.IsValid()) return null;

			FieldData<WorkBookFieldKeys> f = Fields.GetWbkFieldData(PK_AD_MODEL_CODE);

			return xlib.ReadField(e, f)?.Value;
		}

		public bool ReadWorkBook(Entity? e)
		{
			bool result = false;

			if (WorkBook == null || !WorkBook.IsEmpty || e == null) return false;

			if (xlib.ReadFields(e, WorkBook) == ExStoreRtnCode.XRC_GOOD)
			{
				TempModelCode = WorkBook.ModelCode;
				result = true;
			}

			return result;
		}

		public bool ReadSheets()
		{
			if (!GotWorkBook) return false;

			bool result = false;

			ExStoreRtnCode rtnCode;

			Sheet sht;
			//
			// DataStorage? ds;
			// Entity? e;
			Schema? s;

			IList<DataStorage> dsList;

			string schemaName =	Exid.ShtSchemaName;

			if (xlib.FindSchema(schemaName, out s) != ExStoreRtnCode.XRC_GOOD) return false;
			if (xlib.FindSheetsDs(WorkBook!, out dsList) != ExStoreRtnCode.XRC_GOOD) return false;

			foreach (DataStorage dx in dsList)
			{
				if (readSheet(dx, s, out sht)) result = true;

				// e = dx.GetEntity(s);
				// sht = Sheet.CreateEmptySheet(dx);
				//
				// sht.SetReadStart();
				//
				// sht.ExsSchema = s;
				// sht.ExsEntity = e;
				//
				// if (xlib.ReadFields(e, sht) == ExStoreRtnCode.XRC_GOOD)
				// {
				// 	sht.SetReadComplete();
				// 	result = true;
				// }
				//
				// AddSheet(sht);
			}

			return result;
		}

		private bool readSheet(DataStorage ds, Schema s, out Sheet sht)
		{
			bool result = false;
			Entity? e;

			e = ds.GetEntity(s);
			sht = Sheet.CreateEmptySheet(ds);
			sht.ExMgr = this;

			sht.SetReadStart();

			sht.ExsSchema = s;
			sht.ExsEntity = e;

			if (xlib.ReadFields(e, sht) == ExStoreRtnCode.XRC_GOOD)
			{
				sht.SetReadComplete();
				result = true;
			}

			AddSheet(sht);

			return result;
		}


		/* verification resolver */

		public bool VerifyResolver(int resultWbkSc, int resultWbkDs, int resultShtSc, int resultShtDs)
		{
			bool result = true;

			Msgs.WriteLine("*** BEGIN - verification resolver ***");

			// get here when verification shows something wrong
			// that is, one of the args is not zero

			if (resultWbkSc != 0)
			{
				Msgs.WriteLine("\t*** BEGIN - wbk schema resolver ***");
				showResolveStatus(0, 0, resultWbkSc);


				Msgs.WriteLine("\t*** END - wbk schema resolver ***");
			}


			if (resultWbkDs != 0)
			{
				Msgs.WriteLine("\t*** BEGIN - wbk datastorage resolver ***");

				showResolveStatus(0, 1, resultWbkDs);

				Msgs.WriteLine("\t*** END - wbk datastorage resolver ***");
			}


			if (resultShtSc != 0)
			{
				Msgs.WriteLine("\t*** BEGIN - sht schema resolver ***");

				showResolveStatus(1, 0, resultShtSc);

				Msgs.WriteLine("\t*** END - sht schema resolver ***");
			}


			if (resultShtDs != 0)
			{
				Msgs.WriteLine("\t*** BEGIN - sht datastorage resolver ***");

				showResolveStatus(1, 1, resultShtDs);

				Msgs.WriteLine("\t*** END - sht datastorage resolver ***");
			}

			Msgs.WriteLine("*** End - verification resolver ***");

			return result;
		}

		private void showResolveStatus(int which1, int which2, int code)
		{
			string whichA = ExStorConst.DataClassAbbrevUc[which1]; // which 1 == 0 wbk, 1 sht
			string whichF = ExStorConst.DataClassFull[which1];     // which 1 == 0 workbook, 1 sheet
			string whichB = ExStorConst.DataContainerFull[which2]; // which 2 == 0 schema, 1 datastorage
			string result;
			string resolve;

			if (which2 == 0) // schema
			{
				result = ExStorConst.ScValidateResults[code];

				resolve = ExStorConst.ScValidateResolve[code];
			}
			else // ds
			{
				result = ExStorConst.ScValidateResults[code];

				if (which1 == 0) // wbk
				{
					resolve = ExStorConst.DsWbkValidateResolve[code];
				}
				else
				{
					resolve = ExStorConst.DsShtValidateResolve[code];
				}
			}


			Msgs.WriteLine($"\t\t{whichF} {whichB} has issues | status code {code}");
			Msgs.WriteLine($"\t\t  {code} | {whichA} {result} | resolve {resolve}");
		}

		// 0, 0 = wbk, schema & 
		private void resolveWbkSc(int code)
		{
			// A wbk / schema: 0 == good & only one,    1 == none found, 2 == one+ bad, 3 == one+ good (not allowed)
			// resolve 1 == create new | 2 == delete until one valid | 3 == delete all except one
		}


		/* verify */

		/// <summary>
		/// initial verification.  verifys that schema's and data objects exist<br/>
		/// true if bot schema and ds are found and good<br/>
		/// false if any issues
		/// </summary>
		public bool InitVerify(out int ansWbkSc, out int ansWbkDs, out int ansShtSc, out int ansShtDs)
		{
			// possible results
			// A wbk / schema: 0 == good & only one,    1 == none found, 2 == one+ bad, 3 == one+ good (not allowed)
			// B wbk / ds:     0 == good & only one,    1 == none found, 2 == one+ bad, 3 == one+ found, all good (not allowed)
			// C sht / schema: 0 == good & only one,    1 == none found, 2 == one+ bad, 3 == one+ good (not allowed)
			// D sht / ds:     0 == one+ found and good,1 == none found, 2 == one+ bad, 3 == one+ good (allowed)
			// meanings
			// A, B, C, & D == 0 / all good / note that there may be one+ sheet ds found

			bool result;

			// workbook

			// workbook schema ok?
			// result 0 == good & only one, 1 == none found, 2 == one+ bad, 3 = one+ good
			ansWbkSc = findAndVerifyWbkSchema();
			result = GotTempWbkSchema;
			showResult(result, "\t", "wbk schema GOOD", "wbk schema BAD");

			// result = 0 == one found and good, 1 == none found, 2 == one+ bad, 3 == one+ found, all good
			ansWbkDs = findAndVerifyWbkDataStorage();
			result = GotTempWbkDsList;
			showResult(result, "\t", $"wbk Ds GOOD ({TempWbkDsList?.Count})", "wbk Ds BAD");
			if (result) showListResult(TempWbkDsList);


			// sheet

			// result 0 == good & only one, 1 == none found, 2 == one+ bad, 3 = one+ good
			ansShtSc =  findAndVerifyShtSchema();
			result = GotTempShtSchema;
			showResult(result, "\t", "sht schema GOOD", "sht schema BAD");

			// result = 0 == one found and good, 1 == none found, 2 == one+ bad
			// multiple good is ok for sheet ds
			ansShtDs = findAndVerifyShtDataStorage();
			result = GotTempShtDsList;
			showResult(result, "\t", $"sht Ds GOOD ({TempShtDsList?.Count})", "sht Ds BAD");
			if (result) showListResult(TempShtDsList);

			// final, similar to the above (one+ good sheet ds is 0)

			// only true if all above are true
			return ansWbkSc == 0 && ansShtSc == 0;
		}

		public bool VerifyModelName(Entity? e, string testName, out string? modelName)
		{
			modelName = null;

			if (!e.IsValid()) return false;

			return xlib.ValidateModelName(testName, e, out modelName);
		}

		/* verify schema */


		/// <summary>
		/// find all valid sbk schema<br/>
		/// return 0 == good - only one schema, 1 == none found, 2 == one+ bad, 3 == got multiple schema<br/>
		/// </summary>
		private int findAndVerifyWbkSchema()
		{
			IList<Schema>? scList;

			int status = findAndVerifySchema(Exid.WbkSearchName,
				Fields.WBK_FIELDS_COUNT, out scList);

			TempWbkSchemaList = scList;
			TempWbkSchema = null;

			// 0 if all good, 1 if none found, 2 if one+ bad
			if (status == 0)
			{
				// status is 0 so all found have correct number of fields
				// if only one found, save it
				// if not, status is 4
				if ((scList?.Count ?? 0) == 1)
				{
					TempWbkSchema = scList?[0];
				}
				else
				{
					status = 3;
				}
			}

			return status;
		}

		/// <summary>
		/// find all valid sheet schema<br/>
		/// return 0 == good - only one schema, 1 == none found, 2 == one+ bad, 3 == got multiple schema<br/>
		/// </summary>
		private int findAndVerifyShtSchema()
		{
			IList<Schema>? scList;

			int status = findAndVerifySchema(Exid.ShtSearchName,
				Fields.SHT_FIELDS_COUNT, out scList);

			TempShtSchemaList = scList;
			TempShtSchema = null;

			// 0 if all good, 1 if none found, 2 if one+ bad
			if (status == 0)
			{
				// status is 0 so all found have correct number of fields
				// if only one found, save it
				// if not, status is 4
				if ((scList?.Count ?? 0) == 1)
				{
					TempShtSchema = scList?[0];
				}
				else
				{
					status = 3;
				}
			}

			return status;
		}

		/// <summary>
		/// find all schema that starts with the search name and <br/>
		/// have the correct number of fields. <br/>
		/// return 0 == all good, 1 == none found, 2 == one+ bad<br/>
		/// </summary>
		private int findAndVerifySchema(string searchName, int fieldsCount, out  IList<Schema>? scList )
		{
			int status = 0;

			if (FindAllSchema(searchName, out scList) == false) return 1;

			return validateSchemaList(fieldsCount, scList) ? 0 : 2;
		}

		/// <summary>
		/// check if the schema list is all valid objects<br/>
		/// list provided is assumed to be non-null and have one+ items<br/>
		/// return true if all good<br/>
		/// return false if any not valid
		/// </summary>
		private bool validateSchemaList(int fieldCount, IList<Schema>? scList)
		{
			bool result = true;

			foreach (Schema sc in scList)
			{
				if (!sc.IsValidObject || sc.ListFields().Count != fieldCount)
				{
					result = false;
					break;
				}
			}

			return result;
		}


		/* verify datastorage */

		/// <summary>
		/// find all valid wbk data storage objects<br/>
		/// return 0 == one found and good , 1 == none found, 2 == one+ bad, 3 == one+ found, all good<br/>
		/// </summary>
		private int findAndVerifyWbkDataStorage()
		{
			IList<DataStorage> dsList;

			int status = findAndVerifyDs(Exid.WbkSearchName, out dsList);

			if (status == 0)
			{
				TempWbkDsList = dsList;

				if (dsList.Count > 1) status = 3;
			}

			return status;
		}

		/// <summary>
		/// find all valid sht data storage objects<br/>
		/// it is ok to have one+ sht ds so 40 does not apply<br/>
		/// return 0 == one+ found and all good , 1 == none found, 2 == one+ bad, 3 == one+ good found<br/>
		/// </summary>
		private int findAndVerifyShtDataStorage()
		{
			IList<DataStorage> dsList;

			int status = findAndVerifyDs(Exid.ShtSearchName, out dsList);

			if (status == 0)
			{
				TempShtDsList = dsList;
				if (dsList.Count > 1) status = 3;
			}

			return status;
		}

		/// <summary>
		/// find all Ds that starts with the search name provided<br/>
		/// validate all Ds found are valid objects<br/>
		/// return 0 == all good, 1 == none found, 2 == one+ bad, 
		/// </summary>
		private int findAndVerifyDs(string searchName, out IList<DataStorage> dsList)
		{
			// this search name gets all datastorages regardless  of model code
			// (applies to any open document)
			// 0 if good, 1 if not
			int status = 0;

			// if return false, none found, return 1
			if (FindAllDataStorages(searchName, out dsList) == false) return 1;

			// validate all ds
			// 0 if good, 2 if not
			status = validateDsList(dsList) ? 0 : 2;

			return status;
		}

		/// <summary>
		/// check if the ds list is all valid objects<br/>
		/// list provided is assumed to be non-null and have one+ items<br/>
		/// return true if all good<br/>
		/// return false if any not valid
		/// </summary>
		private bool validateDsList(IList<DataStorage>? dsList)
		{
			bool result = true;

			foreach (DataStorage ds in dsList)
			{
				if (!ds.IsValidObject)
				{
					result = false;
					break;
				}
			}

			// when here, if result is false, at least one ds is bad
			return result;
		}


		/* verify debug */

		private void showListResult(IList<DataStorage>? dsList)
		{
			if (dsList == null) return;

			for (var i = 0; i < dsList.Count; i++)
			{
				Msgs.WriteLine($"\t\t{i} | {dsList[i].Name}");
			}

			Msgs.NewLine();
		}

		private void showResult(bool result, string preface, string msgGood, string msgBad)
		{
			if (result)
			{
				Msgs.WriteLine($"{preface}*** {msgGood} ***");
			}
			else
			{
				Msgs.WriteLine($"{preface}*** {msgBad} ***");
			}
		}

	#endregion

	#region event consuming

	#endregion

	#region event publishing

		// public delegate void SettingChangedEventHandler(object sender, SettingChangedEventArgs e);
		//
		// public event ExStorMgr.SettingChangedEventHandler SettingChanged;
		//
		// protected virtual void RaiseSettingChangedEvent(SettingChangedEventArgs e)
		// {
		// 	SettingChanged?.Invoke(this, e);
		// }


		public delegate void RestartReqdChangedEventHandler(object sender, bool? e);

		public event ExStorMgr.RestartReqdChangedEventHandler RestartReqdChanged;

		protected virtual void RaiseRestartReqdChangedEvent(bool? e)
		{
			RestartReqdChanged?.Invoke(this, e);
		}

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ExStorMgr)}";
		}

	#endregion
	}


	// /// <summary>
	// /// once a schema or DS has been deleted, only limited operations<br/>
	// /// are allowed until revit is closed and restarted<br/>
	// /// also, until a schema / DS is configured only limited operations<br/>
	// /// are allowed (different then when deleted)<br/>
	// /// </summary>
	// public OpState UseAllowed { get; set; } = OpState.OS_STARTED;
	//
	// // public bool IsOpAllowed(OpUseTypes ot)
	// // {
	// // 	if (UseAllowed == OpState.OS_NORMAL_OP) 
	// // 		return ot == OpUseTypes.OUT_NORMAL_OP || ot == OpUseTypes.OUT_ANY;
	// //
	// // 	if (UseAllowed == OpState.OS_STARTED) 
	// // 		return ot == OpUseTypes.OUT_CREATE || ot == OpUseTypes.OUT_ANY;;
	// //
	// // 	if (UseAllowed == OpState.OS_CREATE_WBK || UseAllowed == OpState.OS_CREATE_SHT) 
	// // 		return ot == OpUseTypes.OUT_CREATE;
	// //
	// // 	if (UseAllowed == OpState.OS_DELETE_WBK || UseAllowed == OpState.OS_DELETE_SHT) 
	// // 		return ot == OpUseTypes.OUT_DELETE || ot == OpUseTypes.OUT_ANY;;;
	// //
	// // 	return false;
	// // }
	//
	// /// <summary>
	// /// determine if the protected function can run based on the currentOpState<br/>
	// /// this determined "DisAllowed" so <br/>
	// /// true means, do not proceed (true = is disallowed)<br/>
	// /// false means, proceed (false = is not disallowed  i.e. is allowed)
	// /// </summary>
	// public bool OpIsDisAllowed(OpUseTypes ot)
	// {
	// 	if (ot == OpUseTypes.OUT_INFO) return false;
	// 	if (ot == OpUseTypes.OUT_DELETE) return UseAllowed != OpState.OS_CREATE_WBK && UseAllowed != OpState.OS_DELETE_SHT;
	// 	if (ot == OpUseTypes.OUT_NORMAL_OP) return UseAllowed != OpState.OS_NORMAL_OP;
	// 	if (ot == OpUseTypes.OUT_ANY) return UseAllowed == OpState.OS_CREATE_WBK || UseAllowed == OpState.OS_DELETE_SHT;
	//
	// 	return UseAllowed == OpState.OS_DELETE_SHT || UseAllowed == OpState.OS_CREATE_WBK || UseAllowed == OpState.OS_NORMAL_OP;
	// }
}