using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;

using ExStoreTest2026;
using ExStoreTest2026.DebugAssist;
using ExStoreTest2026.Windows;

using ExStorSys;

using RevitLibrary;

using UtilityLibrary;

using static ExStorSys.ExSysStatus;
using static ExStorSys.PropertyId;
using static ExStorSys.ValidateDataStorage;
using static ExStorSys.ValidateSchema;
using static ExStorSys.WorkBookFieldKeys;

// username: jeffs
// created:  9/17/2025 11:01:00 PM


namespace ExStorSys
{

	/// <summary>
	/// Storage Manager - controls the primary admin operations<br/>
	/// and will be the status controller
	/// </summary>
	public class ExStorMgr : APropChgdEvt  // : INotifyPropertyChanged
	{
		public int ObjectId;

	#region objects

		// ExStorData must report here for its status
		//	> status reported
		//		* need to restart
		//		* xData.sheets list is modified
		//



		public MainWinModelUi Mui { get; set; }

		// ReSharper disable once InconsistentNaming
		public ExStorData xData;

		// ReSharper disable once InconsistentNaming
		public ExStorLib xLib;

		// private bool systemRunning;

		private bool? restartRequired;

	#endregion

	#region private fields

	#endregion

	#region ctor

		// private static readonly Lazy<ExStorMgr> instance =
		// 	new Lazy<ExStorMgr>(() => new ExStorMgr());

		private ExStorMgr() { }

		public static ExStorMgr? Instance { get; set; }

		public static ExStorMgr Create()
		{
			Instance = new ExStorMgr();
			Instance.init();

			return Instance;
		}

		private void init()
		{
			// Debug.WriteLine($"\n*** ExStorMgr init | begin");

			// ObjectId = AppRibbon.ObjectIdx++;
			ObjectId = ExStorStartMgr.Instance?.AddObjId(nameof(ExStorMgr)) ?? -1;
			xLib = ExStorLib.Instance;
			// after xLib / before data
			Exid = new Exid();			
			xData = ExStorData.Create();
			Mui = MainWinModelUi.Instance;
			xData.RestartRequiredChanged += XDataOnRestartRequiredChanged;

			RestartRequired = null;

			// Debug.WriteLine($"\n*** ExStorMgr init | exit  ({ObjectId})");
		}

		public void Restore()
		{
			xLib = ExStorLib.Instance;
			ExStorData.Instance = xData;
			xData.Restore();
			Mui = MainWinModelUi.Instance;

			// events??
		}

	#endregion

	#region public properties

		public Exid Exid { get; private set; }

		public bool? RestartRequired
		{
			get => restartRequired;
			private set
			{
				restartRequired = value;
				// RaiseRestartReqdChangedEvent(value);

				OnPropChgd(new PropChgEvtArgs( PI_GEN_RESTART, true));

				if (value == true) ExSysStatus = ES_RESTART_REQD;
			}
		}

		/// <summary>
		/// state of the system<br/>
		///  0 == started / configured, no schema, no ds
		///  1 == got wbk
		///  2 == got 1+ sheet(s)
		///  3 == all ready
		///  4 == restart required
		/// </summary>
		public ExSysStatus ExSysStatus
		{
			get => Mui.ExSysStatus;
			set => OnPropChgd(new PropChgEvtArgs(PI_XSYS_STATUS, value));
		}

	#endregion

	#region stagged properties

		public string? MessageCache { get; set; }

	#endregion

	#region private properties

	#endregion

	#region methods

	#region utility / misc

		/* utility */

		public string? ExtractVersionFromName(string name)
		{
			return xLib.ExtractVersionFromName(name);
		}

		/* system initialization support */

		/// <summary>
		/// reset the schema, workbook, and sheets to empty / null condition in xData
		/// </summary>
		public void ResetData()
		{
			xData.ResetSheets();
			xData.InitSheets();
			xData.ResetWorkBook();
			xData.ResetWorkBookSchemaSilent();
			xData.ResetSheetSchemaSilent();
		}

	#endregion

	#region update

		/* update field in xData */

		public bool UpdateWbkDataField(WorkBookFieldKeys key, DynaValue value)
		{
			WorkBook wbk;


			return true;
		}

		/* update field in entity */

		/// <summary>
		/// update a workbook field of the key provided with the value provided<br/>
		/// return true if OK | return false if schema or workbook are missing
		/// </summary>
		public bool UpdateWbkEntityField(WorkBookFieldKeys key, DynaValue value)
		{
			if (!xData.GotWbkSchema || !xData.GotWorkBook) return false;

			updateEntityField(key, xData.WorkBookSchema, xData.WorkBook, value);

			return true;
		}

		/// <summary>
		/// update a sheet field of the name provided of the key provided with the value provided<br/>
		/// return true if OK | return false if schema or workbook are missing
		/// </summary>
		public void UpdateShtEntityField(string dsName, SheetFieldKeys key, DynaValue value)
		{
			updateEntityField(key, xData.SheetSchema, xData.GetSheet(dsName), value);
		}

		/// <summary>
		/// update a field within the ds entity using a revit transaction
		/// </summary>
		private void updateEntityField<Te>(Te key, Schema? schema, ExStorDataObj<Te> exo, DynaValue value)
			where Te : Enum
		{
			xLib.UpdateEntityField(key, schema, exo, value);
		}

	#endregion

	#region workbook schema

		/* workbook schema */

		/// <summary>
		/// create a schema for workbook - if needed<br/>
		/// return true when made or already exists
		/// else return false
		/// </summary>
		public bool CreateWorkBookSchema()
		{
			if (xData.GotWbkSchema) return true;

			WorkBook wbk = WorkBook.CreateEmptyWorkBook();

			Schema? schema = xLib.MakeSchema(wbk);

			if (schema == null || !schema.IsValidObject)
			{
				xData.WorkBookSchema = null;
				return false;
			}

			xData.WorkBookSchema = schema;
			return true;
		}

	#endregion

	#region workbook

		/* workbook */

		private string getId()
		{
			string lastId = xData.WorkBook.LastId;
			string nextId = ExStorConst.CreateNextIdCode(lastId);

			xData.WorkBook.LastId = nextId;
			UpdateWbkEntityField(PK_AD_LAST_ID, new DynaValue(nextId));

			return nextId;
		}

		private string? createSheetNameForNewVersion(string oldShtName)
		{
			string? shtNameId = xLib.ExtractIdFromShtName(oldShtName, Exid.ShtSearchName);

			if (shtNameId.IsVoid()) return null;

			return Exid.CreateShtDsName(shtNameId!);
		}

		/// <summary>
		/// create the workbook and store in xData<br/>
		/// configure workbook with typical data and model code 
		/// </summary>
		public void MakeWorkBook()
		{
			// Msgs.WriteLineSpaced("step: MC|", ">>> start | create workbook");

			// Msgs.WriteSpaced("step: GS|", ">>> got schema? | ");
			if (!xData.GotWbkSchema)
			{
				Msgs.WriteLine("*** FAILED - schema missing");
				return;
			}

			if (xData.GotWorkBook)
			{
				Msgs.WriteLine("*** FAILED - workbook exists");
				return;
			}

			// Msgs.WriteLine("*** WORKED ***");

			WorkBook wbk = WorkBook.CreateNewWorkBook();

			SaveWorkBook(wbk);
		}

		/// <summary>
		/// Writes the workbook data to the external storage using the defined schema.
		/// </summary>
		/// <remarks>This method attempts to write the workbook data to an external storage location.  The operation
		/// requires that the workbook schema is defined prior to invocation.  If the write operation fails, the workbook
		/// schema is reset to null.</remarks>
		public bool WriteWorkBook()
		{
			
			if (!xData.GotWbkSchema) return false;

			ExStoreRtnCode rtnCode = xLib.WriteWorkBook(xData.WorkBook, xData.WorkBookSchema);

			if (rtnCode != ExStoreRtnCode.XRC_GOOD)
			{
				// Msgs.WriteLine("\n**** FAILED ****\n");
				xData.WorkBookSchema = null;
			}

			return rtnCode == ExStoreRtnCode.XRC_GOOD;
		}

		public bool MakeEmptyWorkBook()
		{
			// xlib.MakeEmptyWorkBook();

			if (!xData.GotWorkBook)
			{
				// is null
				WorkBook wbk = WorkBook.CreateEmptyWorkBook();
				SaveWorkBook(wbk);
			}
			else
			{
				if (!xData.IsWorkBookEmpty)
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

		public void SaveWorkBook(WorkBook wbk)
		{
			// xData.TempModelCode = wbk.ModelCode;
			xData.WorkBook = wbk;
		}

		// public void EnableWbkTrackChanges()
		// {
		// 	xData.WbkEnableTrackChanges();
		// }
		

	#endregion

	#region sheet schema

		/* sheet schema */

		/// <summary>
		/// create a schema for sheet - if needed<br/>
		/// return true when made or already exists
		/// else return false
		/// </summary>
		public bool CreateSheetSchema()
		{
			if (xData.GotShtSchema) return true;

			Sheet sht = Sheet.CreateEmptySheet("empty");

			Schema? schema = xLib.MakeSchema(sht);

			if (schema == null || !schema.IsValidObject)
			{
				xData.SheetSchema = null;
				return true;
			}

			xData.SheetSchema = schema;
			return true;
		}
	#endregion

	#region sheets

		/* sheets */

		/// <summary>
		/// create a new, basically empty, sheet using the sheet creation data<br/>
		/// and sets initial values<br/>
		/// the sheet status is flagged as SS_NEW
		/// </summary>
		public Sheet CreateNewSheet(SheetCreationData sd)
		{
			if (!xData.GotShtSchema) return null;

			string nextShtName = Exid.CreateShtDsName(getId());

			if (xData.GotSheet(nextShtName)) return null;

			Sheet? sht = Sheet.CreateSheet(nextShtName, sd);

			if (sht == null) return null;

			sht.SheetStatus = SheetStatus.SS_NEW;

			return sht;
		}

		// test routine
		public bool WriteSheets()
		{
			if (!xData.GotShtSchema) return false;
		
			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_GOOD;
		
			foreach ((string key, Sheet sht) in xData.Sheets)
			{
				rtnCode = xLib.WriteSheet(sht, xData.SheetSchema);
		
				if (rtnCode != ExStoreRtnCode.XRC_GOOD) break;
			}
		
			return rtnCode == ExStoreRtnCode.XRC_GOOD;
		}

		// test routine
		public bool WriteSheet(string dsName)
		{
			Sheet sht;

			if (!xData.GotShtSchema) return false;

			if (!xData.TryGetSheet(dsName, out sht)) return false;

			ExStoreRtnCode rtnCode = xLib.WriteSheet(sht, xData.SheetSchema);

			if (rtnCode != ExStoreRtnCode.XRC_GOOD)
			{
				// Msgs.WriteLine("\n**** FAILED ****\n");
				xData.SheetSchema = null;
			}
			// else
			// {
			// 	Msgs.WriteLine("\n**** WORKED ****\n");
			// }

			return rtnCode == ExStoreRtnCode.XRC_GOOD;
		}

		// test routine
		public string? MakeSheet(SheetCreationData sd)
		{
			if (!xData.GotShtSchema) return null;

			string nextShtName = Exid.CreateShtDsName(getId());

			if (xData.GotSheet(nextShtName)) return null;

			Sheet? sht = Sheet.CreateSheet(nextShtName, sd);

			if (sht == null) return null;

			xData.AddSheetPreInit(sht);

			return sht.DsName;
		}

		// test routine
		public Sheet MakeEmptySheet()
		{
			// gotta have a workbook - full not empty
			if (!xData.GotWorkBook || xData.IsWorkBookEmpty) return null;

			string nextShtName = Exid.CreateShtDsName(getId());

			Sheet sht = Sheet.CreateEmptySheet(nextShtName);

			return sht;
		}

		// test routine
		public bool AddSheetFamily(string fam, string type, string props)
		{
			return xData.CurrentSheet != null 
				&& xData.CurrentSheet.AddFamAndType(fam, type, props);
		}

		// test routine
		public bool RemoveSheetFamily(string fam, string type)
		{
			return xData.CurrentSheet != null 
				&& xData.CurrentSheet.RemoveFamAndType(fam, type);
		}

	#endregion

	#region delete

		/* delete */

		/* notes
		 * needed:
		 * delete a sheet DS (no longer being used)
		 * delete system (complete [DS + schema]
		 *		> can only have a single model open
		 *		> restart required (save to app ribbon for presistence)
		 *		> ui disabled
		 * delete system (single model [DS but NOT schema]
		 *		> most ui disabled
		 */

		public bool DeleteWbkSchema()
		{
			if (!xData.GotWbkSchema) return false;

			if (eraseSchema(xData.WorkBookSchema))
			{
				xData.WorkBookSchema = null;
				RestartRequired = true;
				return true;
			}

			return false;
		}

		public bool DeleteShtSchema()
		{
			if (!xData.GotShtSchema) return false;

			bool result = eraseSchema(xData.SheetSchema);

			return result;
		}

		// public bool DeleteDsList(IList<DataStorage>? dsList, bool onlyOne = false)
		/// <summary>
		/// delete the DS in the list provided<br/>
		/// if "onlyOne" onlu delete the first DS
		/// </summary>
		/// <param name="dsList"></param>
		/// <param name="onlyOne"></param>
		/// <returns></returns>
		public bool DeleteDsList(IList<DataStorage>?  dsList, bool onlyOne = false)
		{
			if (dsList == null || dsList.Count == 0) return false;

			bool result = true;

			foreach (DataStorage ds in dsList)
			{
				if (ds.IsValidObject)
				{
					if (!deleteDs(ds))
					{
						result = false;
					}
					else if (onlyOne)
					{
						break;
					}
				}

				ds.Dispose();
			}

			return result;
		}

		public void DeleteLocalDs(string name, ItemUsage iu)
		{
			if (iu == ItemUsage.IU_WBK && xData.GotWbkDs)
			{
				if (xData.WorkBook.ExsDataStorage.Name.Equals(name))
				{
					
				}
			}
		}

		public bool EraseScList(IList<Schema>? scList)
		{
			if (scList == null || scList.Count == 0) return false;
			RestartRequired = true;

			bool result = true;

			foreach (Schema sc in scList)
			{
				// string name = item.Item.SchemaName;

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

			return xLib.DeleteDs(ds) == ExStoreRtnCode.XRC_GOOD;
		}

		private bool eraseSchema(Schema? s)
		{
			if (s == null || !s.IsValidObject) return false;

			if (xLib.EraseSc(s) == ExStoreRtnCode.XRC_GOOD)
			{
				return true;
			}

			return false;
		}

	#endregion

	#region find

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

			if (xData.GotWorkBook) return false;

			string searchText = ExStorConst.EXS_WBK_NAME_SEARCH;
			string matchText = Exid.ModelName;


			return xLib.FindExStorInfo(0, ExStorConst.WbkSchemaName,
				searchText, matchText, out ds, out e, out s) == ExStoreRtnCode.XRC_GOOD;
		}

		// /// <summary>
		// /// find all data storage objects with the given name prefix<br/>
		// /// return true if count > 0
		// /// </summary>
		// public bool FindAllDataStorages(string searchName, out IList<DataStorage>? dsList)
		// {
		// 	dsList = xLib.FindAllDataStorageByNamePrefix(searchName);
		//
		// 	return dsList.Count > 0;
		// }

		/// <summary>
		/// find a model's associated data storage object (if any)<br/>
		/// using the model code and id code (that is, with a full DS name)<br/>
		/// that is, search type == 1
		/// </summary>
		public bool FindSheetDs(string id, out DataStorage? ds, out Entity? e, out Schema? s)
		{
			ds = null;
			e = null;
			s = null;

			string searchText = Exid.CreateShtDsName(id);

			// if (SheetsList != null && SheetsList.ContainsKey(searchText)) return false;
			if (xData.GotSheet(searchText)) return false;

			string matchText = null;

			return xLib.FindExStorInfo(1, ExStorConst.ShtSchemaName,
				searchText, matchText, out ds, out e, out s) == ExStoreRtnCode.XRC_GOOD;
		}

		// public bool FindAndSaveSheets()
		// {
		// 	if (!xData.GotWorkBook) return false;
		//
		// 	IList<DataStorage> dsList;
		//
		// 	if (xlib.FindSheetsDs(xData.WorkBook!, out dsList) != ExStoreRtnCode.XRC_GOOD) return false;
		//
		// 	saveSheets(dsList);
		//
		// 	return true;
		// }

		/// <summary>
		/// {\rtf1\ansi\pard this is {\b bold} text\par}
		/// </summary>
		public bool FindSheetsDs(out IList<DataStorage>? dsList)
		{
			dsList = null;

			if (!xData.GotWorkBook) return false;

			string dsSearchName = Exid.ShtSearchName;

			if (xLib.FindSheetsDs(dsSearchName, out dsList) != ExStoreRtnCode.XRC_GOOD) return false;

			return true;
		}

		// find elements and place into temp variables

		/// <summary>
		/// find all workbook schemas and return them as a list<br/>
		/// based on the search name but not using the version
		/// </summary>
		/// <remarks>
		///true == one+ found | false == none found
		/// </remarks>
		public bool FindAllWbkSchema(out IList<Schema> scList)
		{
			return findAllSchemaByName(Exid.WbkSearchName, out scList);
		}

		/// <summary>
		/// \b1find\b0 all workbook DS<br/>
		/// **true** == one+ found | false == none found
		/// </summary>
		public bool FindAllWbkDs(out IList<DataStorage> dsList)
		{
			return findAllDsByName(Exid.WbkSearchName, out dsList);
		}

		/// <summary>
		/// find all sheet schemas and place into temp variable
		/// </summary>
		public bool FindAllShtSchema(out IList<Schema> scList)
		{
			return findAllSchemaByName(Exid.ShtSearchName, out scList);
		}

		/// <summary>
		/// find all sheet DS and return via "out"<br/>
		/// true == one+ found | false == none found
		/// </summary>
		public bool FindAllShtDs(out IList<DataStorage> dsList)
		{
			return findAllDsByName(Exid.ShtSearchName, out dsList);
		}

		/// <summary>
		/// find all schema that start with the search name provided<br/>
		/// true == one+ found | false == none found
		/// </summary>
		private bool findAllSchemaByName(string searchName, out IList<Schema> schemas)
		{
			return xLib.FindAllSchemaByNamePrefix(searchName, out schemas) == ExStoreRtnCode.XRC_GOOD;
		}

		/// <summary>
		/// find all datastorage that start with the search name provided<br/>
		/// true == one+ found | false == none found
		/// </summary>
		private bool findAllDsByName(string searchName, out IList<DataStorage> dsList)
		{
			return xLib.FindAllDataStorageByNamePrefix(searchName, out dsList) == ExStoreRtnCode.XRC_GOOD;
		}

	#endregion

	#region read

		/* read */

		// public bool ReadWorkBook(out WorkBook? wbk)
		// {
		// 	wbk = WorkBook.CreateEmptyWorkBook();
		//
		// 	if (!xData.GotWbkSchema) return false;
		// 	if (!xData.GotTempWbkEntity) return false;
		//
		// 	if (xLib.ReadEntityFields(xData.WorkBook.ExsEntity, wbk) != ExStoreRtnCode.XRC_GOOD) return false;
		//
		// 	wbk.ExsEntity = xData.WorkBook.ExsEntity;
		// 	wbk.ExsDataStorage = xData.WorkBook.ExsDataStorage;
		//
		// 	ExStorData exd = ExStorData.Instance;
		// 	return true;
		// }

		public bool ReadWorkBookViaTempInfo(out WorkBook? wbk, out ObservableDictionary<string, Sheet>? shts)
		{
			wbk = WorkBook.CreateEmptyWorkBook();
			shts = new ObservableDictionary<string, Sheet>();
			Sheet sht;

			Entity shtE;

			if (!xData.GotTempShtSchema || 
				!xData.GotTempWbkEntity ||
				!xData.GotTempWbkDs) return false;


			if (xLib.ReadEntityFields(xData.TempWbkEntity, wbk) != ExStoreRtnCode.XRC_GOOD) return false;
			wbk.UpdateExsObjects(xData.TempWbkDsEx!.Item, xData.TempWbkEntity!, xData.TempWbkSchemaEx!.Item);

			if (!xData.GotShtSchema || 
				!xData.GotTempShtDs) return false;

			foreach ((string? key, ExListItem<DataStorage>? value) in xData.TempShtDsListEx.GoodItems)
			{
				if (!xLib.GetEntity(value.Item, xData.TempShtSchemaEx!.Item, out shtE)) continue;

				if (readSheet(value.Item, xData.TempShtSchemaEx!.Item, out sht))
				{
					if (shts.TryAdd(key, sht))
					{
						sht.Config();
					}
				}
			}

			return true;
		}

		// /// <summary>
		// /// read a workbook from the DS into the locak workbook<br/>
		// /// the locak workbook must have the DS, Entity, and Schema configured
		// /// </summary>
		// private bool readWorkBook()
		// {
		// 	bool result = false;
		//
		// 	if (!xData.GotWbkSchema) return result;
		//
		// 	if (!xData.WorkBook.IsEmpty) return false;
		//
		// 	if (xLib.ReadEntityFields(xData.WorkBook.ExsEntity, xData.WorkBook) == ExStoreRtnCode.XRC_GOOD)
		// 	{
		// 		result = true;
		// 	}
		//
		// 	return result;
		// }

		private bool readWorkBook(DataStorage? ds, Schema? s)
		{
			bool result = false;
			Entity? e;
			if (!xLib.GetEntity(ds!, s, out e)) return false;

			xData.WorkBook = WorkBook.CreateEmptyWorkBook();
			xData.WorkBook.UpdateExsObjects(ds!, e!, s);

			if (xLib.ReadEntityFields(e, xData.WorkBook) == ExStoreRtnCode.XRC_GOOD)
			{
				result = true;
			}

			return result;
		}

		/// <summary>
		/// read all of the field values into a sheet
		/// </summary>
		private bool readSheet(DataStorage ds, Schema s, out Sheet? sht)
		{
			bool result = false;
			Entity? e;
			sht = null;

			if (!xLib.GetEntity(ds, s, out e)) return false;

			sht = Sheet.CreateEmptySheet(ds.Name);
			sht.UpdateExsObjects(ds, e, s);
			// sht.ExsEntity = e;

			if (xLib.ReadEntityFields(e, sht) == ExStoreRtnCode.XRC_GOOD) result = true;

			return result;
		}

		/// <summary>
		/// Reads the model name from the workbook DS
		/// </summary>
		public string? ReadModelName(DataStorage ds, Schema? s)
		{
			Entity? e;
		
			if (!xLib.GetEntity(ds, s, out e)) return null;

			xData.TempWbkEntity = e;

			// FieldData<WorkBookFieldKeys> f = xData.GetWbkFieldData(PK_MD_MODEL_TITLE);
			// FieldDef<WorkBookFieldKeys> f = xData.GetWbkFieldDef(PK_MD_MODEL_TITLE);
			FieldDef<WorkBookFieldKeys> f = Fields.GetWbkFieldDef(PK_MD_MODEL_TITLE);

			return xLib.CleanName(xLib.ReadFieldDyn(e, f).Value);
		}

		/// <summary>
		/// Reads the model name from the workbook DS
		/// </summary>
		public ActivateStatus ReadActivationStatus(DataStorage ds, Schema? s)
		{
			Entity? e;
		
			if (!xLib.GetEntity(ds, s, out e)) return ActivateStatus.AS_NA;

			xData.TempWbkEntity = e;

			// FieldDef<WorkBookFieldKeys> f = xData.GetWbkFieldDef(PK_AD_STATUS);
			FieldDef<WorkBookFieldKeys> f = Fields.GetWbkFieldDef(PK_AD_STATUS);

			// if (f.Field == null) return ActivateStatus.AS_INACTIVE;

			ActivateStatus act = xLib.ReadField2(e, f) ?? ActivateStatus.AS_INACTIVE;

			return act;
		}
		

	#endregion

	#region launch, start, initialization, and validation routines

		/* launch, start, and validation routines */

		/// <summary>
		/// combo routine to transfer the workbook schema and the workbook ds
		/// </summary>
		public bool? ProcTransWbk()
		{
			// St - transfer workbook schema
			if (!transTempWkbSchemaToXdata()) return false;

			// Tt - transfer the workbook ds and field values
			return transTempWbkDsToXdata();
		}

		/// <summary>
		/// combo routine to transfer a sht schema and the sht ds list (if exists)
		/// </summary>
		public bool? ProcTransSht()
		{
			// Ut - transfer sht schema
			if (!transTempShtSchemaToXdata()) return false;

			xData.ResetSheets();

			// Vt - transfer sht ds - if applies
			return TransTempExSheetsToXdata();
		}

		/// <summary>
		/// combo routine to create a wbk schema and a wbk ds
		/// </summary>
		public bool? ProcCreateAndWriteWbk()
		{
			// Sc
			// if the temp version exists, save this else create
			if (!procTransOrCreateWbkSchema()) return false;
			
			OnPropChgdWsc(VSC_GOOD);

			// Tc
			MakeWorkBook();

			// W
			if (!WriteWorkBook()) return false;

			OnPropChgdWds(VDS_GOOD);

			return true;
		}

		/// <summary>
		/// combo routine to create a sht schema and the sht ds list
		/// </summary>
		public bool? ProcCreateSht()
		{
			// Uc
			// if the temp version exists, save this
			// else create
			if (!transTempShtSchemaToXdata())
			{
				if (!CreateSheetSchema()) return false;
			}

			OnPropChgdSsc(VSC_GOOD);

			// F (may be duplicate)
			xData.ResetSheets();

			OnPropChgdSds(VDS_MISSING);

			return null;
		}
		
		/// <summary>
		/// combo routine to transfer the wbk schema and ds, update the model
		/// name with the current model name, then write the updated informtion
		/// back into the model's ds / entity
		/// </summary>
		public bool? ProcFixModelName2()
		{
			if (!xData.WorkBook.SetInitValueDym(PK_MD_MODEL_TITLE, Exid.ModelName)) return false;

			if (!UpdateWbkEntityField(PK_MD_MODEL_TITLE, new DynaValue(Exid.ModelName))) return false;

			if (VerifyModelName() != VDS_GOOD) return false;

			return true;
		}


		/// <summary>
		/// duplicate one sheet to another sheet with the same ID but uses the
		/// current Sheet version
		/// </summary>
		public bool? ProcTransShtToNewVer(Schema schemaOld)
		{
			if (!xData.GotTempAnySheetsEx) return false;

			DataStorage dsOld;
			Sheet? oldSheet;
			Sheet? newSheet;

			xData.RemovePlaceHolderSheet();

			foreach ((string? key, ExListItem<DataStorage>? exItemOld) in xData.TempShtDsListEx!.GoodItems)
			{
				dsOld = exItemOld.Item;

				if (!readSheet(dsOld, schemaOld, out oldSheet)) return false;

				if (oldSheet == null) continue;

				if (!transShtV1ToShtV2(oldSheet, out newSheet)) continue;

				if (xLib.WriteSheet(newSheet!, xData.SheetSchema) != ExStoreRtnCode.XRC_GOOD) return false;
				
				// xData.SheetsList.Add(newSheet!.DsName, newSheet);

				xData.AddSheetPreInit(newSheet);
			}

			return true;
		}

		// /// <summary>
		// /// combo routine to transfer the wbk schema and ds, update the model
		// /// code, then write the updated informtion
		// /// back into the model's ds / entity
		// /// </summary>
		// // procedure ●
		// public bool? ProcFixModelCode(string mc)
		// {
		// 	// Sc
		// 	// transfer or create a wbk schema
		// 	if (!procTransOrCreateWbkSchema()) return false;
		//
		// 	// G, part 1
		// 	if (!transTempWbkDsToXdata()) return false;
		//
		// 	// G, part 2
		// 	if (!xData.WorkBook.SetValue(PK_AD_MODEL_CODE, new DynaValue(mc))) return false;
		//
		// 	// W
		// 	if (!WriteWorkBook()) return false;
		//
		// 	return true;
		// }

		/// <summary>
		/// combo routine to transfer the wbk schema and ds, update the 
		/// activate status to true, then write the updated informtion
		/// back into the model's ds / entity
		/// </summary>
		public bool? ProcFixActivationOff()
		{
			// Sc
			// if the temp version exists, save this else create
			if (!procTransOrCreateWbkSchema()) return false;

			// C, part 1
			if (!transTempWbkDsToXdata()) return false;

			// C, part 2
			if (!xData.WorkBook.SetInitValueDym(PK_AD_STATUS, ActivateStatus.AS_ACTIVE)) return false;

			// W
			if (!WriteWorkBook()) return false;

			return true;
		}

		// public bool? ProcFixModelCode()
		// {
		// 	// by fix the model code, change the model code saved in the workbook ds to
		// 	// the model code used by the sheets - this means that first, must determine the
		// 	// correct model code to use.  this routine applies because, the wrong model code 
		// 	// was found which menas that there is a temp list of found sht ds's
		//
		//
		// 	// if one found, mc == result -> transfer and fix
		// 	// if 1+ found , mc == ""     -> reset
		// 	// if 0 found  , mc == null   -> reset
		// 	string? mc = GetTempModelCodes();
		//
		// 	if (mc.IsVoid()) return ProcCreateSht();
		//
		// 	// got the one and only model code - fix the workbook with this
		// 	// model code
		//
		//
		// 	return true;
		// }

		/// <summary>
		/// procedure to transfer or create a wbk schema
		/// </summary>
		private bool procTransOrCreateWbkSchema()
		{
			if (!transTempWkbSchemaToXdata())
			{
				if (!CreateWorkBookSchema()) return false;
			}
			
			OnPropChgdWsc(VSC_GOOD);

			return true;
		}

		/// <summary>
		/// verify if the temp workbook DS has the correct model name
		/// </summary>
		public ValidateDataStorage VerifyModelName()
		{
			if (!xData.GotTempWbkSchema) return VDS_NG;

			ValidateDataStorage status = VDS_GOOD;

			string modelName = ReadModelName(xData.TempWbkDsEx!.Item, xData.TempWbkSchemaEx.Item) ?? "";

			if (!modelName.Equals(Exid.ModelName))
			{
				status = VDS_WRONG_MODEL_NAME;
			}

			return status;
		}

		/// <summary>
		/// verify if the current temp workbook DS has activation ignore set
		/// </summary>
		public ValidateDataStorage VerifyActivationIgnore()
		{
			if (!xData.GotTempWbkSchema) return VDS_NG;

			ValidateDataStorage status = VDS_GOOD;
			ActivateStatus actStat;

			actStat = ReadActivationStatus(xData.TempWbkDsEx!.Item, xData.TempWbkSchemaEx!.Item);

			if (actStat == ActivateStatus.AS_IGNORE) status = VDS_ACT_IGNORE;

			return status;
		}

		/// <summary>
		/// verify if the current temp workbook DS has activation off set
		/// </summary>
		public ValidateDataStorage VerifyActivationOff()
		{
			if (!xData.GotTempWbkSchema) return VDS_NG;

			ValidateDataStorage status = VDS_GOOD;
			ActivateStatus actStat;

			actStat = ReadActivationStatus(xData.TempWbkDsEx!.Item, xData.TempWbkSchemaEx!.Item);

			if (actStat == ActivateStatus.AS_INACTIVE) status = VDS_ACT_OFF;

			return status;
		}


		/* transfer */

		/// <summary>
		/// transfer the data from an old sheet version to the new version<br/>
		/// real routine but bogus as new and old are the same class<br/>
		/// but the concept is worked out.<br/>
		/// new sheet needs to be pre-configured with basic info (name, model name, etc.)
		/// </summary>
		private bool transShtV1ToShtV2(Sheet oldSht, out Sheet? newSht)
		{
			newSht = null;

			string? newShtName = createSheetNameForNewVersion(oldSht.DsName);
			if (newShtName.IsVoid()) return false;

			Dictionary<SheetFieldKeys, DynaValue> data = MakeCpyShtDataForNewVer(
				new DynaValue(DateTime.Now.ToString("s")),
				new DynaValue(ExStorConst.UserName));

			newSht = xLib.DuplicateSheet(2, oldSht, newShtName!, data);

			return true;
		}

		/// <summary>
		/// create the dictionary with the updated values when a sheet is duplicated
		/// specific for a type 2 (new version) copy
		/// </summary>
		public Dictionary<SheetFieldKeys, DynaValue> MakeCpyShtDataForNewVer(			
			DynaValue modDt, 
			DynaValue modBy)
		{
			return MakeCpyShtData(DynaValue.InValid(), 
				DynaValue.InValid(), modDt, modBy);
		}


		/// <summary>
		/// create the dictionary with the updated values when a sheet is duplicated
		/// </summary>
		public Dictionary<SheetFieldKeys, DynaValue> MakeCpyShtData(
			DynaValue createdDt, 
			DynaValue createdBy, 
			DynaValue modDt, 
			DynaValue modBy)
		{
			Dictionary<SheetFieldKeys, DynaValue> values = new ()
			{
				{ SheetFieldKeys.RK_AD_DATE_CREATED, createdDt },
				{ SheetFieldKeys.RK_AD_NAME_CREATED, createdBy },
				{ SheetFieldKeys.RK_AD_DATE_MODIFIED, modDt },
				{ SheetFieldKeys.RK_AD_NAME_MODIFIED, modBy }
			};

			return values;
		}


		/// <summary>
		/// transfer the temp version of workbook schema to its
		/// standard location in xdata / workbook<br/>
		/// true if got temp wbk schema and it was saved in xData<br/>
		/// false if no temp wbk schema
		/// </summary>
		private bool transTempWkbSchemaToXdata()
		{
			// Msgs.CWriteLineSpaced("", ">>> save temp workbook schema ... ");

			// wbk schema
			if (!xData.GotTempWbkSchema) return false;

			xData.WorkBookSchema = xData.TempWbkSchemaEx.Item;

			return true;
		}

		/// <summary>
		/// transfers the model's wbk to the xData object and save the
		/// ds entity inxData
		/// </summary>
		private bool transTempWbkDsToXdata()
		{
			Entity e;

			// wbk ds
			if (!xData.GotTempWbkDs) return false;

			xData.WorkBook = WorkBook.CreateEmptyWorkBook();

			// wbk entity
			if (!xLib.GetEntity(xData.TempWbkDsEx!.Item, 
					xData.WorkBookSchema, out e)) return false;

			xData.WorkBook.UpdateExsObjects(xData.TempWbkDsEx.Item,
				e!, xData.WorkBookSchema!);

			return readWorkBook(xData.TempWbkDsEx!.Item, xData.WorkBookSchema);

			// // changed from true to this
			// return readWorkBook();
		}

		/// <summary>
		/// transfer the temp version of sheet schema to its
		/// standard location in the sheet<br/>
		/// true if got temp sheet schema and it was saved in xData<br/>
		/// false if no temp sheet schema
		/// </summary>
		private bool transTempShtSchemaToXdata()
		{
			// Msgs.CWriteLineSpaced("", ">>> save temp sheet schema ... ");

			// sht schema
			if (!xData.GotTempShtSchema) return false;

			xData.SheetSchema = xData.TempShtSchemaEx.Item;

			return true;
		}

		/// <summary>
		/// transfer all of the sheets in the temp sheet list
		/// to their standard location in xdata / sheets<br/>
		/// true if got sheets and transfered<br/>
		/// null if there are no sheets to transfer<br/>
		/// false if the transfer process did not work<br/>
		/// note: reset the sheets list before using this method
		/// </summary>
		public bool? TransTempExSheetsToXdata()
		{
			// Msgs.CWriteLineSpaced("", ">>> save temp sheets ... ");
			Sheet sht;
			Entity? e;

			if (!xData.GotTempAnySheetsEx) return null;

			xData.RemovePlaceHolderSheet();

			foreach ((string? key, ExListItem<DataStorage>? value) in xData.TempShtDsListEx.GoodItems)
			{
				if (readSheet(value.Item, xData.SheetSchema!, out sht!))
				{
					xData.AddSheetPreInit(sht!);
				}
			}

			return xData.GotAnySheets ? true : null;
		}

		// /// <summary>
		// /// prep for sheets - transfer the sheet schema and reset the sheets list.<br/>
		// /// does not make any sheets.  returns false only if unable to <br/>
		// /// create a missing sheet schema
		// /// </summary>
		// /// <returns></returns>
		// private bool setupSheets()
		// {
		// 	if (!transTempShtSchemaToXdata())
		// 	{
		// 		if (!CreateSheetSchema()) return false;
		// 	}
		//
		// 	xData.ResetSheets();
		//
		// 	return true;
		// }
		

	#endregion


		// temp routine - don't use
		/// <summary>
		/// transfer the temp version of workbook objects (schema, ds, and entity)
		/// to their standard location in xData
		/// </summary>
		public bool TransTempWbkObjectsToXdata()
		{
			Entity? e;

			Msgs.CWriteLineSpaced("step StWo |", ">>> save temp workbook objects ... ");

			if (!transTempWkbSchemaToXdata())
			{
				return false;
			}

			xData.WorkBook = WorkBook.CreateEmptyWorkBook();

			// wbk ds
			if (!xData.GotTempWbkDs) return false;

			Msgs.CWriteLineSpaced("", ">>> save temp workbook ds ... ");

			xData.WorkBook.ExsDataStorage = xData.TempWbkDsEx.Item;

			// wbk entity
			if (!xLib.GetEntity(xData.WorkBook.ExsDataStorage!, 
					xData.WorkBookSchema, out e)) return false;

			xData.WorkBook.ExsEntity = e;

			return true;
		}

		// temp routine - don't use
		/// <summary>
		/// transfer the temp version of sheet objects to their
		/// standard location in the sheet(s)
		/// </summary>
		public bool? TransTempShtObjectsToXdata()
		{
			Msgs.CWriteLineSpaced("step StSo |", ">>> save temp sheet objects ... ");

			Sheet sht;
			Entity? e;

			if (!transTempShtSchemaToXdata()) return false;

			// transfer temp sheets - if any

			// bool? result = transTempSheetsToXdata();
			bool? result = TransTempExSheetsToXdata();
			
			return result;
		}

		/* field Data */

		// public FieldData<WorkBookFieldKeys> GetWbkFieldData(WorkBookFieldKeys key)
		// {
		// 	if (!xData.GotWorkBook) return FieldData<WorkBookFieldKeys>.Empty();
		//
		// 	return xData.WorkBook.GetField(key);
		// }
		//
		// public FieldData<SheetFieldKeys> GetSheetFieldData(string shtName, SheetFieldKeys key)
		// {
		// 	if (!xData.GotAnySheets) return FieldData<SheetFieldKeys>.Empty();
		//
		// 	Sheet sht = xData.GetSheet(shtName);
		//
		// 	if (sht.IsEmpty) return FieldData<SheetFieldKeys>.Empty();
		//
		// 	return sht.GetField(key);
		// }

		/** entity **/

		// private bool getEntity(DataStorage ds, Schema? s, out Entity? e)
		// {
		// 	e = null;
		//
		// 	if (!ds.IsValidObject) return false;
		// 	if (s == null || !s.IsValidObject) return false;
		//
		// 	e = ds.GetEntity(s);
		// 	if (!e.IsValidObject) return false;
		//
		// 	return true;
		// }


	#endregion methods

	#region event consuming

		// private void XDataOnExStorStatusChanged(object sender)
		// {
		// 	ExSysStatus stat = xData.ExStorStatus;
		//
		// 	if (stat == ES_SHT_CREATED && xData.GotWorkBook) stat = ES_START_DONE_GOOD;
		//
		// 	ExSysStatus = stat;
		// }

		private void XDataOnRestartRequiredChanged(object sender, bool? e)
		{
			RestartRequired = e;
		}


	#endregion

	#region event publishing

		// public delegate void PropChgdEventHandler(object sender, PropChgEvtArgs e);
		//
		// public event ExStorMgr.PropChgdEventHandler PropChgd;
		//
		// protected virtual void OnPropChgdEvent(PropChgEvtArgs e)
		// {
		// 	PropChgd?.Invoke(this, e);
		// }

		/*
		public delegate void PropChgdEventHandler(object sender, PropChgEvtArgs e);

		public event ExStorMgr.PropChgdEventHandler PropChgd;

		protected virtual void OnPropChgd(PropChgEvtArgs e)
		{
			PropChgd?.Invoke(this, e);
		}
		*/

		// protected void OnPropChgd(PropertyId pid, DynaValue value)
		// {
		// 	OnPropChgd(new PropChgEvtArgs(PO_XMGR, pid, value));
		// }

		// protected void OnPropChgd(PropertyId pi, dynamic value)
		// {
		// 	PropChgd?.Invoke(this, new PropChgEvtArgs(PO_XMGR, pi, value));
		// }

		// // restart required event
		//
		// public delegate void RestartReqdChangedEventHandler(object sender, bool? e);
		//
		// public event ExStorMgr.RestartReqdChangedEventHandler RestartReqdChanged;
		//
		// protected virtual void RaiseRestartReqdChangedEvent(bool? e)
		// {
		// 	RestartReqdChanged?.Invoke(this, e);
		// }

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"{nameof(ExStorMgr)} [{ObjectId}]";
		}

	#endregion

	}

}