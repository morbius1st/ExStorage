using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

using Autodesk.Revit.DB.ExtensibleStorage;

using ExStoreTest2026;
using ExStoreTest2026.DebugAssist;
using ExStoreTest2026.ExStorSys;
using ExStoreTest2026.Windows;

using RevitLibrary;

using UtilityLibrary;

using static ExStorSys.ExSysStatus;
using static ExStorSys.PropertyId;
using static ExStorSys.PropertyOwner;
using static ExStorSys.ValidateDataStorage;
using static ExStorSys.ValidateSchema;
using static ExStorSys.WorkBookFieldKeys;

// username: jeffs
// created:  9/17/2025 11:01:00 PM


namespace ExStorSys
{

	/// <summary>
	/// Storage Manager - controls the primary admin operations
	/// </summary>
	public class ExStorMgr : APropChgdEvt  // : INotifyPropertyChanged
	{
		public int ObjectId;

	#region objects

		public MainWinModelUi Mui { get; set; }

		// ReSharper disable once InconsistentNaming
		public ExStorData xData;

		// ReSharper disable once InconsistentNaming
		public ExStorLib xLib;

		private bool systemRunning;

		private bool? restartRequired;
		private ExSysStatus exSysStatus;

		// private int resultWbkSc;
		// private int resultWbkDs;
		// private int resultShtSc;
		// private int resultShtDs;

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
			// ObjectId = AppRibbon.ObjectIdx++;
			ObjectId = ExStorStartMgr.Instance?.AddObjId(nameof(ExStorMgr)) ?? -1;
			xLib = ExStorLib.Instance;
			// after xLib / before data
			Exid = new Exid();			
			xData = ExStorData.Create();
			Mui = MainWinModelUi.Instance;
			xData.RestartRequiredChanged += XDataOnRestartRequiredChanged;

			RestartRequired = null;
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
			get => exSysStatus;
			set
			{
				exSysStatus = value;
				OnPropChgd(new PropChgEvtArgs(PI_XSYS_STATUS, value));
			}
		}


	#endregion

	#region stagged properties

		public string? MessageCache { get; set; }

	#endregion

	#region private properties

	#endregion

	#region methods


		/* utility */

		public string? ExtractVersionFromName(string name)
		{
			return xLib.ExtractVersionFromName(name);
		}


		/* update field in entity */

		/// <summary>
		/// update a workbook field of the key provided with the value provided<br/>
		/// return true if OK | return false if schema or workbook are missing
		/// </summary>
		public bool UpdateWbkField(WorkBookFieldKeys key, DynaValue value)
		{
			if (!xData.GotWbkSchema || !xData.GotWorkBook) return false;

			updateField(key, xData.WorkBookSchema, xData.WorkBook, value);

			return true;
		}

		/// <summary>
		/// update a sheet field of the name provided of the key provided with the value provided<br/>
		/// return true if OK | return false if schema or workbook are missing
		/// </summary>
		public void UpdateShtField(string dsName, SheetFieldKeys key, DynaValue value)
		{
			updateField(key, xData.SheetSchema, xData.GetSheet(dsName), value);
		}

		/// <summary>
		/// update a field within the ds entity using a revit transaction
		/// </summary>
		private void updateField<Te>(Te key, Schema? schema, ExStorDataObj<Te> exo, DynaValue value)
			where Te : Enum
		{
			xLib.UpdateField(key, schema, exo, value);
		}

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

		/* workbook */

		private string getId()
		{
			string lastId = xData.WorkBook.LastId;
			string nextId = ExStorConst.CreateNextIdCode(lastId);

			xData.WorkBook.LastId = nextId;
			UpdateWbkField(PK_AD_LAST_ID, new DynaValue(nextId));

			return nextId;
		}

		/// <summary>
		/// create the workbook and store in xData<br/>
		/// configure workbook with typical data and model code 
		/// </summary>
		public void MakeWorkBook()
		{
			Msgs.WriteLineSpaced("step: MC|", ">>> start | create workbook");

			Msgs.WriteSpaced("step: GS|", ">>> got schema? | ");
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

			Msgs.WriteLine("*** WORKED ***");

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

			if (rtnCode == ExStoreRtnCode.XRC_GOOD)
			{
				Msgs.WriteLine("\n**** WORKED ****\n");
			}
			else
			{
				Msgs.WriteLine("\n**** FAILED ****\n");
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

		/* entity */

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

		/* sheets */

		public bool WriteSheet(string dsName)
		{
			Sheet sht;

			if (!xData.GotShtSchema) return false;

			if (!xData.TryGetSheet(dsName, out sht)) return false;

			ExStoreRtnCode rtnCode = xLib.WriteSheet(sht, xData.SheetSchema);

			if (rtnCode == ExStoreRtnCode.XRC_GOOD)
			{
				Msgs.WriteLine("\n**** WORKED ****\n");
			}
			else
			{
				Msgs.WriteLine("\n**** FAILED ****\n");
				xData.SheetSchema = null;
			}

			return rtnCode == ExStoreRtnCode.XRC_GOOD;
		}

		public string? MakeSheet(SheetCreationData sd)
		{
			if (!xData.GotShtSchema) return null;

			string nextShtName = Exid.CreateShtDsName(getId());

			if (xData.GotSheet(nextShtName)) return null;

			Sheet? sht = Sheet.CreateSheet(nextShtName, sd);

			if (sht == null) return null;

			xData.AddSheet(sht);

			return sht.DsName;
		}

		public Sheet MakeEmptySheet()
		{
			// gotta have a workbook - full not empty
			if (!xData.GotWorkBook || xData.IsWorkBookEmpty) return null;

			string nextShtName = Exid.CreateShtDsName(getId());

			Sheet sht = Sheet.CreateEmptySheet(nextShtName);

			return sht;
		}

		private void saveSheets(IList<DataStorage> dsList)
		{
			Sheet sht;

			foreach (DataStorage ds in dsList)
			{
				sht = Sheet.CreateEmptySheet(ds);

				xData.AddSheet(sht);
			}
		}
		
		
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
			string matchText = Exid.Model_Name;


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

			// if (Sheets != null && Sheets.ContainsKey(searchText)) return false;
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
		/// find all sheet DS and place into temp variable
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



		/* read */

		public bool ReadWorkBook()
		{
			return ReadWorkBook(xData.WorkBook.ExsEntity);
		}

		public bool ReadWorkBook(Entity? e)
		{
			bool result = false;

			if (!xData.GotWbkSchema) return result;

			if (xData.WorkBook == null || !xData.WorkBook.IsEmpty || e == null) return false;

			if (xLib.ReadFields(e, xData.WorkBook) == ExStoreRtnCode.XRC_GOOD)
			{
				// xData.TempModelCode = xData.WorkBook.ModelCode;
				result = true;
			}

			return result;
		}

		public bool ReadSheets()
		{
			bool result = false;

			if (!xData.GotWorkBook) return result;
			if (!xData.GotShtSchema) return result;

			Sheet sht;

			IList<DataStorage>? dsList;

			string dsSearchName = Exid.ShtSearchName;

			if (xLib.FindSheetsDs(dsSearchName, out dsList) != ExStoreRtnCode.XRC_GOOD) return false;
			foreach (DataStorage ds in dsList)
			{
				if (readSheet(ds, xData.SheetSchema, out sht))
				{
					xData.AddSheet(sht);
					result = true;
				}
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

			sht = Sheet.CreateEmptySheet(ds);

			sht.ExsEntity = e;

			if (xLib.ReadFields(e, sht) == ExStoreRtnCode.XRC_GOOD) result = true;

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
		
			return xLib.ReadModelName(e);
		}

		/// <summary>
		/// Reads the model name from the workbook DS
		/// </summary>
		public ActivateStatus ReadActivationStatus(DataStorage ds, Schema? s)
		{
			Entity? e;
		
			if (!xLib.GetEntity(ds, s, out e)) return ActivateStatus.AS_NA;

			xData.TempWbkEntity = e;
		
			return xLib.ReadActStatus(e!);
		}



		/* system initialization support */

		/// <summary>
		/// reset the schema, workbook, and sheets to empty / null condition in xData
		/// </summary>
		public void ResetData()
		{
			xData.ResetSheets();
			xData.ResetWorkBook();

			xData.ResetSheetSchemaSilent();
			xData.ResetWorkBookSchemaSilent();
		}


		/* combo routines */


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

			// Vt - transfer sht ds - if applies
			return transTempExSheetsToXdata();
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
		public bool? ProcFixModelName()
		{
			// Sc
			// transfer or create a wbk schema
			if (!procTransOrCreateWbkSchema()) return false;

			// G, part 1
			if (!transTempWbkDsToXdata()) return false;

			// G, part 2
			if (!xData.WorkBook.UpdateRow(PK_MD_MODEL_NAME, new DynaValue(Exid.Model_Name))) return false;

			// W
			if (!WriteWorkBook()) return false;

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
		// 	if (!xData.WorkBook.UpdateRow(PK_AD_MODEL_CODE, new DynaValue(mc))) return false;
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
			if (!xData.WorkBook.UpdateRow(PK_AD_STATUS, new DynaValue(ActivateStatus.AS_ACTIVE))) return false;

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
			// Sc
			if (!transTempWkbSchemaToXdata())
			{
				if (!CreateWorkBookSchema()) return false;
			}
			
			OnPropChgdWsc(VSC_GOOD);

			return true;
		}

		

		/* workbook */

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

			xData.WorkBook.ExsDataStorage = xData.TempWbkDs.Item;

			// wbk entity
			if (!xLib.GetEntity(xData.WorkBook.ExsDataStorage!, 
					xData.WorkBookSchema, out e)) return false;

			xData.WorkBook.ExsEntity = e;

			return true;
		}

		/// <summary>
		/// transfer the temp version of workbook schema to its
		/// standard location in xdata / workbook<br/>
		/// true if got temp wbk schema and it was saved in xData<br/>
		/// false if no temp wbk schema
		/// </summary>
		private bool transTempWkbSchemaToXdata()
		{
			Msgs.CWriteLineSpaced("", ">>> save temp workbook schema ... ");

			// wbk schema
			if (!xData.GotTempWbkSchema) return false;

			xData.WorkBookSchema = xData.TempWbkSchema.Item;

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

			xData.WorkBook.ExsDataStorage = xData.TempWbkDs.Item;

			// wbk entity
			if (!xLib.GetEntity(xData.WorkBook.ExsDataStorage!, 
					xData.WorkBookSchema, out e)) return false;

			xData.WorkBook.ExsEntity = e;

			ReadWorkBook(e);

			return true;
		}

		// /// <summary>
		// /// activate the workbook - save the temp version or make a schema.
		// /// make a workboon
		// /// </summary>
		// /// <returns></returns>
		// private bool makeAndWriteWorkbook()
		// {
		// 	if (!transTempWkbSchemaToXdata())
		// 	{
		// 		// could not transfer - probably does not exist - create
		// 		if (!CreateWorkBookSchema()) return false;
		// 	}
		//
		// 	// got workbook schema
		// 	// make and write workbook
		//
		// 	MakeWorkBook();
		//
		// 	return WriteWorkBook();
		// }

		// /// <summary>
		// /// set the status of ExSysStatus and RunningStatus depending on the
		// /// result of a startup process<br/>
		// /// </summary>
		// public bool SetStartResultStatus(bool? result)
		// {
		// 	int idx = result == true ? 1 : result == false ? -1 : 0;
		//
		// 	return SetStartResultStatus(idx);
		// }
		//
		// public bool SetStartResultStatus(int idx)
		// {
		// 	bool result = false;
		//
		// 	switch (idx)
		// 	{
		// 	case -1:
		// 		{
		// 			OnPropChgdExs(ES_START_DONE_EXIT);
		// 			OnPropChgdRn(RunningStatus.RN_CANNOT_RUN_FAIL);
		// 			result = false;
		// 			break;
		// 		}
		// 	case 1:
		// 		{
		// 			OnPropChgdExs(ES_START_DONE_GOOD);
		// 			OnPropChgdRn(RunningStatus.RN_RUNNING_NORMAL);
		// 			result = true;
		// 			break;
		// 		}
		// 	case 0:
		// 		{
		// 			OnPropChgdExs(ES_START_DONE_GOOD);
		// 			OnPropChgdRn(RunningStatus.RN_RUNNING_NEED_SHT);
		// 			result = true;
		// 			break;
		// 		}
		// 	case -2:
		// 		{
		// 			OnPropChgdExs(ES_START_DONE_EXIT);
		// 			OnPropChgdRn(RunningStatus.RN_DEACTIVATE);
		// 			result = false;
		// 			break;
		// 		}
		// 	}
		//
		// 	OnPropChgd(PI_GEN_RESTART, false);
		//
		// 	return result;
		// }


		/* sheet */

		// /// <summary>
		// /// scan the temp sht ds list and get the list of possible model codes.  if
		// /// only one found, return this.<br/>
		// /// if more than one found, return string.empty
		// /// if none found, return null
		// /// </summary>
		// public string? GetTempModelCodes()
		// {
		// 	string mc;
		//
		// 	List<string> codes = new ();
		//
		// 	if (xData.GotTempAnySheets)
		// 	{
		// 		foreach (DataStorage ds in xData.TempShtDsList!)
		// 		{
		// 			mc = xLib.ExtractModelCodeFromName(ds.Name, Exid.ShtSearchName) ?? "";
		//
		// 			if (!mc.IsVoid())
		// 			{
		// 				if (!codes.Contains(mc)) codes.Add(mc);
		// 			}
		// 		}
		// 	}
		//
		// 	return codes.Count == 1 ? codes[0] : null;
		// }


		/// <summary>
		/// transfer the temp version of sheet objects to their
		/// standard location in the sheet(s)
		/// </summary>
		public bool? TransTempShtObjectsToXdata()
		{
			Msgs.CWriteLineSpaced("step StSo |", ">>> save temp sheet objects ... ");

			Sheet sht;
			Entity? e;

			if (!transTempShtSchemaToXdata())
			{
				// Msgs.WriteLine("FAIL");

				return false;
			}

			// transfer temp sheets - if any

			// bool? result = transTempSheetsToXdata();
			bool? result = transTempExSheetsToXdata();
			
			// if (result == true)
			// {
			// 	Msgs.WriteLine("GOOD");
			// }
			// else
			// {
			// 	Msgs.WriteLine("FAIL");
			// }

			return result;
		}

		/// <summary>
		/// transfer the temp version of sheet schema to its
		/// standard location in the sheet<br/>
		/// true if got temp sheet schema and it was saved in xData<br/>
		/// false if no temp sheet schema
		/// </summary>
		private bool transTempShtSchemaToXdata()
		{
			Msgs.CWriteLineSpaced("", ">>> save temp sheet schema ... ");

			// sht schema
			if (!xData.GotTempShtSchema) return false;

			xData.SheetSchema = xData.TempShtSchema.Item;

			return true;
		}

		/*
		/// <summary>
		/// transfer all of the sheets in the temp sheet list
		/// to their standard location in xdata / sheets<br/>
		/// true if got sheets and transfered<br/>
		/// null if there are no sheets to transfer<br/>
		/// false if the transfer process did not work<br/>
		/// note: reset the sheets list before using this method
		/// </summary>
		private bool? transTempSheetsToXdata()
		{
			Msgs.CWriteLineSpaced("", ">>> save temp sheets ... ");
			Sheet sht;
			Entity? e;

			if (!xData.GotTempAnySheets) return null;

			foreach (DataStorage ds in xData.TempShtDsList)
			{
				if (readSheet(ds, xData.SheetSchema!, out sht!)) xData.AddSheet(sht!);
			}

			return xData.GotAnySheets ? true : null;
		}
		*/

		/// <summary>
		/// transfer all of the sheets in the temp sheet list
		/// to their standard location in xdata / sheets<br/>
		/// true if got sheets and transfered<br/>
		/// null if there are no sheets to transfer<br/>
		/// false if the transfer process did not work<br/>
		/// note: reset the sheets list before using this method
		/// </summary>
		private bool? transTempExSheetsToXdata()
		{
			Msgs.CWriteLineSpaced("", ">>> save temp sheets ... ");
			Sheet sht;
			Entity? e;

			if (!xData.GotTempAnySheetsEx) return null;

			foreach ((string? key, ExListItem<DataStorage>? value) in xData.TempShtDsExList.GoodItems)
			{
				if (readSheet(value.Item, xData.SheetSchema!, out sht!)) xData.AddSheet(sht!);
			}

			return xData.GotAnySheets ? true : null;
		}

		/// <summary>
		/// prep for sheets - transfer the sheet schema and reset the sheets list.<br/>
		/// does not make any sheets.  returns false only if unable to <br/>
		/// create a missing sheet schema
		/// </summary>
		/// <returns></returns>
		private bool setupSheets()
		{
			if (!transTempShtSchemaToXdata())
			{
				if (!CreateSheetSchema()) return false;
			}

			xData.ResetSheets();

			return true;
		}

	#endregion

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


	// /// <summary>
	// /// when the windows opens for the first time, for this model, this routine will
	// /// activate the exstorage system for this model.
	// /// </summary>
	// /// <returns></returns>
	// public bool? ProcStartActivate()
	// {
	// 	// process:
	// 	// B: clear the old data
	// 	// S: create wbk schema
	// 	// H: create workbook
	// 	// U: transfer or create a sheet schema
	// 	// F: init sheet list
	// 	// P: set status & flag good but need sheets
	//
	// 	// B
	// 	ResetData();
	//
	// 	// s
	// 	if (!CreateWorkBookSchema()) return false;
	//
	// 	OnPropChgdWsc(VSC_GOOD);
	//
	// 	// H
	// 	MakeWorkBook();
	//
	// 	if (!WriteWorkBook()) return false;
	//
	// 	OnPropChgdWds(VDS_GOOD);
	//
	// 	// U
	// 	if (!saveTempShtSchema())
	// 	{
	// 		if (!CreateSheetSchema()) return false;
	// 	}
	//
	// 	OnPropChgdSsc(VSC_GOOD);
	// 	OnPropChgdSds(VDS_MISSING);
	// 	
	// 	// F (done via ResetData)
	//
	// 	// P
	// 	return null;
	// }



	// /// <summary>
	// /// Performs the normal startup process for the system, initializing and transferring necessary data.
	// /// </summary>
	// /// <remarks>This method handles the initialization of system data, including clearing old data, transferring
	// /// workbook  and sheet schemas, and setting appropriate statuses and flags. The specific steps performed depend on
	// /// the  current state of the system and the data being processed.</remarks>
	// public bool? ProcStartNormal()
	// {
	// 	bool? result;
	// 	// process:
	// 	// B: clear the old data
	// 	// S: transfer wbk schema
	// 	// T: Transfer wbk ds
	// 	// U: transfer sheet schema
	// 	//		V: transfer sheet ds(s) ->
	// 	//		Q: set status & flag good
	// 	// or
	// 	// U: create sheet schema
	// 	//		F: init sheet ds list (done with ResetData()
	// 	//		P: set status & flag good but need sheets
	//
	// 	// B
	// 	ResetData();
	//
	// 	// S
	// 	if (!saveTempWkbSchema()) return false;
	//
	// 	// T
	// 	if (!saveTempWbkDs()) return false;
	//
	// 	// U
	// 	if (!saveTempShtSchema()) return false;
	//
	// 	// V or F
	// 	return saveTempSheets();
	// }



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