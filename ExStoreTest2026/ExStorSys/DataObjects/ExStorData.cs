using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Data;

using Autodesk.Revit.DB.ExtensibleStorage;

using ExStoreTest2026;
using ExStoreTest2026.DebugAssist;
using ExStoreTest2026.Windows;

using JetBrains.Annotations;

using RevitLibrary;

using UtilityLibrary;

using static ExStorSys.ExSysStatus;

// projname: $projectname$
// itemname: ExStorData
// username: jeffs
// created:  10/17/2025 10:48:45 PM

namespace ExStorSys
{
	public class ExStorData : INotifyPropertyChanged
	{
		public int ObjectId;

	#region private fields

		// ReSharper disable once InconsistentNaming
		// private static readonly Lazy<ExStorData> instance =
		// 	new (() => new ExStorData());

	#endregion

	#region objects

		// cannot be null
		private WorkBook wbk;
		private ObservableDictionary<string, Sheet> sheetsList;

		private Schema? wbkSchema;
		private Schema? shtSchema;

		// private MainWinModelUi xMui;

		private ExStorLib xLib;

		private bool? restartRequired;

		// private Sheet? currentSheet;
		public static string? selectSheet
		{
			get; 
			set;
		}

		private bool isModifiedSheetsList;
		private bool canUndoSheetsList;

	#endregion

	#region ctor

		#pragma warning disable CS8618, CS9264
		private ExStorData()
			#pragma warning restore CS8618, CS9264
		{
			// init();
		}

		public static ExStorData Instance { get; set; } // => instance.Value;

		public static ExStorData Create()
		{
			Instance = new ExStorData();
			Instance.init();

			return Instance;
		}

		private void init()
		{
			ObjectId = ExStorStartMgr.Instance?.AddObjId(nameof(ExStorData)) ?? -1;


			WorkBook = WorkBook.CreateEmptyWorkBook();
			
			sheetsList = new ObservableDictionary<string, Sheet>();
			
			ResetSheets();

// // ******* NEED THIS ?? ******
			InitSheets();
			
			// xMui = MainWinModelUi.Instance;
			xLib = ExStorLib.Instance;

			SheetsViewSource = new CollectionViewSource {Source = Sheets};
			SheetsViewSource.SortDescriptions.Add(new SortDescription("Value.OpSequence", ListSortDirection.Ascending));

			SheetsNoDeletedViewSource = new CollectionViewSource {Source = Sheets};
			SheetsNoDeletedViewSource.SortDescriptions.Add(new SortDescription("Value.OpSequence", ListSortDirection.Ascending));
			SheetsNoDeletedViewSource.Filter += SheetsViewSourceOnFilter;

			isModifiedSheetsList = false;


			Sheet.PropChgd += ChildOnPropChgd;
			WorkBook.PropChgd += ChildOnPropChgd;


		}

		/// <summary>
		/// restores prior information when the system is restarted (by start manager)
		/// </summary>
		public void Restore()
		{
			// xMui = MainWinModelUi.Instance;

			xLib = ExStorLib.Instance;

		}

		/// <summary>
		/// run once each time the window is opened
		/// </summary>
		public void Config()
		{
			setSelectedSheet();
		}

	#endregion

	#region general propertries

		private ExSysStatus exStorStatus
		{
			// get => xMui.ExSysStatus;
			set => OnPropChgd(new PropChgEvtArgs(PropertyId.PI_XSYS_STATUS, value));
		}

		/// <summary>
		/// flag that a restart of revit is required
		/// </summary>
		public bool? RestartRequired
		{
			get => restartRequired;
			private set
			{
				if (restartRequired == true) return;

				restartRequired = value;

				RaiseRestartRequiredEvent(value);
				exStorStatus = ES_RESTART_REQD;
			}
		}
		
		// status items to track
		// A - overall - anything has changed / this needs to be saved
		// B - a workbook field has been changed
		// C - the collection of sheets has changed (added or removed or modified)
		// D - a sheet field has been changed (this gets folded into 
		//	 +-> a family and type list has been change (this changes a sheet so is not separately tracked)

		public bool NeedsSaving
		{
			get
			{
				// string caller = new StackFrame(2, false).GetMethod()?.Name ?? "is null";


				bool w = WorkBook.IsModifiedExo;
				bool l = IsModifiedSheetsList;


				return w || l;
			}
		}

		/// <summary>
		/// flag that the sheets list has been modified<br/>
		/// but only after initialization (tracking turned on)
		/// </summary>
		public bool IsModifiedSheetsList
		{
			get => isModifiedSheetsList;
			private set
			{
				if (value == isModifiedSheetsList) return;
				isModifiedSheetsList = value;
				OnPropertyChanged();

				OnPropertyChanged(nameof(NeedsSaving));
			}
		}

		public bool CanUndoSheetsList
		{
			get => canUndoSheetsList;
			private set
			{
				if (value == canUndoSheetsList) return;
				canUndoSheetsList = value;
				OnPropertyChanged();
			}
		}

	#endregion

	#region reset

		/* reset */

		public void ResetAll()
		{
			ResetWorkBook();
			ResetWorkBookSchemaSilent();
			ResetSheets();
			InitSheets();
			ResetSheetSchemaSilent();
			ResetTemp();

			IsModifiedSheetsList = false;
		}

		/// <summary>
		/// reset the workbook to an empty workbook
		/// </summary>
		public bool ResetWorkBook()
		{
			if (GotAnySheets) return false;
			WorkBook = WorkBook.CreateEmptyWorkBook();

			return true;
		}

		/// <summary>
		/// reset a sheet (set to empty)<br/>
		/// but only if creationstation >= 5
		/// </summary>
		// ReSharper disable once UnusedMember.Global
		public void ResetSheet(string name)
		{
			if (name.IsVoid() || !sheetsList.TryGetValue(name, out Sheet? sheet)) return;

			sheetsList[name] = Sheet.CreateEmptySheet(name);
		}

		/// <summary>
		/// Reset the workboon schema to null
		/// </summary>
		public void ResetWorkBookSchemaSilent()
		{
			wbkSchema = null;
		}

		/// <summary>
		/// Reset the workboon schema to null
		/// </summary>
		public void ResetSheetSchemaSilent()
		{
			shtSchema = null;
		}

		/// <summary>
		/// reset all temp objects to null or empty
		/// </summary>
		public void ResetTemp()
		{
			TempWbkVersion = string.Empty;
			TempWbkSchemaEx  = null;
			TempWbkDsEx      = null;
			TempWbkEntity  = null;
			TempWbkDsList  = new List<DataStorage>();

			TempShtVersion = string.Empty;
			TempShtSchemaEx  = null;
			TempShtDsListEx = new ();
			// TempShtEntity  = null;
			// TempShtDsList  = new  List<DataStorage>();
		}

	#endregion

	#region workbook ops

		/* workbook OPS */
		
		/* workbook Schema */

		/// <summary>
		/// the workbook schema object<br/>
		/// can only be set to a schema once<br/>
		/// but can be set to null - which triggers the ResetRequired event
		/// </summary>
		public Schema? WorkBookSchema
		{
			get => wbkSchema;
			set
			{

				if (wbkSchema != null && value != null) return;
				wbkSchema = value;

				if (value == null)
				{
					RestartRequired = true;
					exStorStatus = ES_RESTART_REQD;
				}
				else
				{
					exStorStatus = ES_WBK_SCHEMA_CREATED;
				}

				OnPropChgd(PropertyId.PI_XDATA_WBK_SC, GotWbkSchema);
			}
		}


		/* workbook */

		/// <summary>
		/// Enables change tracking in the workbook
		/// </summary>
		public void WbkEnableTrackChanges()
		{
			if (!GotWorkBook) return;
			wbk.SetTrackChanges();
		}

		/// <summary>
		/// the workbook object<br/>
		/// cannot be set to null
		/// </summary>
		public WorkBook WorkBook
		{
			get => wbk;
			set
			{
				// // although wbk will not be null - it is the first time through, so ignore this

				// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
				if (value == null) return;

				wbk = value;

				exStorStatus = ES_WBK_CREATED;
				OnPropChgd(PropertyId.PI_XDATA_WBK, GotWorkBook);
			}
		}

		/// <summary>
		/// get an empty workbook
		/// </summary>
		// ReSharper disable once UnusedMember.Global
		public WorkBook GetEmptyWorkBook()
		{
			return WorkBook.CreateEmptyWorkBook();
		}

		/// <summary>
		/// commit the change to the workbook to the model
		/// </summary>
		/// <returns></returns>
		public bool WorkbookApplyChgs(bool bypassAlt)
		{
			if (!WorkBook.IsModifiedExo) return false;

			bool found = false;

			bool canEditField;

			UserSecutityLevel usl = SecurityMgr.Instance.UserSecurityLevel;

			foreach ((WorkBookFieldKeys key, FieldData<WorkBookFieldKeys> fd) in WorkBook)
			{
				if ((fd.DyValue?.IsDirty ?? false))
				{
					canEditField = 
						SecurityMgr.ValidateFieldEditing(fd.Field!.FieldEditLevel, usl) == 
						FieldEditStatus.FES_CAN_EDIT;

					// todo - add logic
					// // if found AltSrcA
					// if (fd.Field!.IsAltSrcA)
					// {
					// 	// if bypassing alt - cont.
					// 	if (bypassAlt && !canEditField) continue;
					// }
					// else if (!fd.Field.IsAltSrcB) if (!bypassAlt) continue;

					xLib.UpdateEntityField(key, WorkBookSchema, WorkBook, fd.DyValue);
					found = true;
				}
			}

			if (found) WorkBook.ApplyOrUndoChanges(bypassAlt, true);

			return true;
		}

	#endregion

	#region Sheet Ops

		/* sheet OPS */

		/* sheet schema */

		/// <summary>
		/// the sheet schema object<br/>
		/// can only be set to a schema once<br/>
		/// but can be set to null - which triggers the ResetRequired event<br/>
		/// and, if set to null, can then be set
		/// </summary>
		public Schema? SheetSchema
		{
			get => shtSchema;
			set
			{
				if (shtSchema != null && value != null) return;
				shtSchema = value;

				if (value == null)
				{
					RestartRequired = true;
					exStorStatus = ES_RESTART_REQD;
				}
				else
				{
					exStorStatus = ES_SHT_SCHEMA_CREATED;
				}

				OnPropChgd(PropertyId.PI_XDATA_SHT_SC, GotShtSchema);
			}
		}

		/* sheet */

		/// <summary>
		/// enable change tracking in all sheets
		/// </summary>
		public void ShtsEnableTrackChanges()
		{
			if (!GotAnySheets) return;

			foreach ((string? key, Sheet? value) in sheetsList)
			{
				if (value.IsEmpty) continue;
				value.SetTrackChanges();
			}
		}

		/// <summary>
		/// find and return a sheet by its name<br/>
		/// return invalid sheet if not found
		/// </summary>
		public Sheet GetSheet(string name)
		{
			if (name.IsVoid() || !sheetsList.TryGetValue(name, out Sheet? sheet)) return Sheet.Invalid();

			return sheet;
		}

		/// <summary>
		/// find a sheet by name and return as an out parameter<br/>
		/// return true if found, else false
		/// </summary>
		public bool TryGetSheet(string name, out Sheet sht)
		{
			if (!GotSheet(name))
			{
				sht = Sheet.Invalid();
				return false;
			}

			sht = sheetsList[name];
			return true;
		}

		/// <summary>
		/// find a sheet by name<br/>
		/// if not found, return the first sheet in the list, if any<br/>
		/// else return null
		/// </summary>
		public bool? TryGetSheetEx(string name, out Sheet? sht)
		{
			if (GotSheet(name))
			{
				sht= sheetsList[name];
				return true;
			}

			if (sheetsList.Count > 0)
			{
				sht = sheetsList.First().Value;
				return null;
			}

			sht = null;
			return false;

		}

		/// <summary>
		/// determine if a sheet exists by its name 
		/// </summary>
		public bool GotSheet(string name)
		{
			if (name.IsVoid()) return false;
			return sheetsList.ContainsKey(name);
		}

		/// <summary>
		/// does the named sheet have a DS
		/// </summary>
		public bool GotShtDs(string name)
		{
			Sheet sht;
			if (!TryGetSheet(name, out sht) /*|| sht.IsInvalid*/ ) return false;

			return sht.GotDs;
		}

		/// <summary>
		/// Retrieves the data for a specified field from a sheet with the given name.
		/// </summary>
		public FieldData<SheetFieldKeys> GetShtFieldData(string name, SheetFieldKeys key)
		{
			if (!GotAnySheets) return FieldData<SheetFieldKeys>.Empty();

			Sheet sht = GetSheet(name);

			if (sht.IsEmpty) return FieldData<SheetFieldKeys>.Empty();

			return sht.GetField(key);
		}

		/// <summary>
		/// the currently selected sheet from the Sheets list
		/// </summary>
		public Sheet? CurrentSheet
		{
			get
			{

				if (selectSheet!.IsVoid()) return null;
				return !sheetsList.ContainsKey(selectSheet!) ? null : sheetsList[selectSheet!];
			}
		}

		/// <summary>
		/// the selected sheet in a UI list
		/// </summary>
		public string? SelectSheet
		{
			get
			{
				return selectSheet;
			}
			set
			{
				if ((value ?? "").Equals(selectSheet)) return;

				selectSheet = value;

				OnPropertyChanged();

				OnPropertyChanged(nameof(ExStorData));
				OnPropertyChanged(nameof(CurrentSheet));
			}
		}

		public bool CurrSheetApplyChgs(bool bypassAlt)
		{
			if (!CurrentSheet!.IsModifiedExo) return false;

			bool found = false;

			bool canEditField;

			UserSecutityLevel usl = SecurityMgr.Instance.UserSecurityLevel;

			foreach ((SheetFieldKeys key, FieldData<SheetFieldKeys> fd) in CurrentSheet)
			{
				if ((fd.DyValue?.IsDirty ?? false))
				{
					canEditField = 
						SecurityMgr.ValidateFieldEditing(fd.Field!.FieldEditLevel, usl) == 
						FieldEditStatus.FES_CAN_EDIT;

					// todo - add logic
					// // if found AltSrcA
					// if (fd.Field!.IsAltSrcA)
					// {
					// 	// if bypassing alt - cont.
					// 	if (bypassAlt && !canEditField) continue;
					// }
					// else if (!fd.Field.IsAltSrcB) if (!bypassAlt) continue;

					ExStorMgr.Instance?.UpdateShtEntityField(CurrentSheet.DsName,key, fd.DyValue);
					found = true;
				}
			}

			if (found) CurrentSheet.ApplyOrUndoChanges(bypassAlt, true);

			return true;
		}

	#endregion

	#region Sheet List

		/* sheet list */

		/* sheet list properties */

		public CollectionViewSource SheetsViewSource {get; private set;}
		public CollectionViewSource SheetsNoDeletedViewSource {get; private set;}

		private void updateSheetsListProps()
		{

			// note for the clear sheets list can exe method to work correctly
			// this view must be updated before the regular view
			// or use multi converter with multibinding
			SheetsNoDeletedViewSource.View.Refresh();
			OnPropertyChanged(nameof(SheetsNoDeletedViewSource));


			SheetsViewSource.View.Refresh();
			OnPropertyChanged(nameof(SheetsViewSource));

		}


		/// <summary>
		/// return the number of sheets in the list
		/// </summary>
		public int SheetsCount => sheetsList.Count;

		/// <summary>
		/// return the sheets list values
		/// </summary>
		// public Dictionary<string, Sheet>.ValueCollection? Sheets => sheetsList.Values;
		public ObservableDictionary<string, Sheet>? Sheets => sheetsList;

		public void FinalizeSheetListInit()
		{
			updateSheetsListProps();

			OnPropertyChanged(nameof(NeedsSaving));
		}

		/// <summary>
		/// add a sheet to the sheets list - before system initialized
		/// </summary>
		public void AddSheetPreInit(Sheet sht)
		{
			sht.SheetStatus = SheetStatus.SS_EXISTING;
			addSheet(sht);

		}

		/// <summary>
		/// add a sheet to the sheets list - after system initialized<br/>
		/// i.e., sets is modified
		/// </summary>
		public void AddSheet(Sheet sht)
		{
			sht.SheetStatus = SheetStatus.SS_NEW;
			
			addSheet(sht);

			validateSheetStatus();

			updateSheetsListProps();

			// must follow update props
			SelectSheet = sht.DsName;
		}

		/// <summary>
		/// update a sheet (replace) with a sheet<br/>
		/// but only if the replacement sheet creationstation > 0
		/// </summary>
		// ReSharper disable once UnusedMember.Global
		public bool UpdateSheet(Sheet sht)
		{
			if (!sheetsList.ContainsKey(sht.DsName)) return false;

			sheetsList[sht.DsName] = sht;

			IsModifiedSheetsList = true;
			updateSheetsListProps();

			return true;
		}

		/// <summary>
		/// remove a sheet from the sheets list<br/>
		/// but only if creationstation less than 5
		/// </summary>
		// ReSharper disable once UnusedMember.Global
		public bool RemoveCurrentSheet(string name)
		{
			if (sheetsList[name].SheetStatus == SheetStatus.SS_EXISTING)
			{
				sheetsList[name].SheetStatus = SheetStatus.SS_DELETED;
			}
			else if (sheetsList[name].SheetStatus == SheetStatus.SS_NEW)
			{
				sheetsList[name].SheetStatus = SheetStatus.SS_NEW_DELETED;
			}
			else if (sheetsList[name].SheetStatus == SheetStatus.SS_MODIFIED)
			{
				sheetsList[name].SheetStatus = SheetStatus.SS_MOD_DELETED;
			}
			

			// SelectSheet = null;
			// IsModifiedSheetsList = true;

			validateSheetStatus();
			
			updateSheetsListProps();

			// must follow update props
			setSelectedSheet();

			return true;
		}

		/// <summary>
		/// revert a deleted sheet to active (restores its prior status)
		/// </summary> 
		public void UndoRemoveCurrentSheet(string name)
		{
			sheetsList[name].UndoSheetStatus();

			// SelectSheet = null;

			validateSheetStatus();
			updateSheetsListProps();

			// must follow update props
			setSelectedSheet();
		}

		/// <summary>
		/// restore the sheets list - e.g., undo the changes<br/>
		/// except that, any added sheet is marked to delete rather than being
		/// removed - this allows for a 2nd level of undo
		/// </summary>
		public void SheetsListUndoChgs()
		{
			bool result = false;
			// restore does two things	
			// for each sheet in the list that can undo, it un-dose
			// for each new item, it deletes it (and allows undo)

			foreach ((string key, Sheet sht) in sheetsList)
			{
				if (sht.SheetStatus == SheetStatus.SS_DELETED)
				{
					sht.UndoSheetStatus();
				}
				else if (sht.SheetStatus == SheetStatus.SS_NEW_DELETED)
				{
					sht.UndoSheetStatus();
					result = true;
				}
				else if (sht.SheetStatus == SheetStatus.SS_NEW)
				{
					sht.SheetStatus = SheetStatus.SS_NEW_DELETED;
				}
			}

			// this removed because the change date and the last id code
			// need to be kept because the new sheet was not deleted, it was
			// just flagged for deletion / not saved to allow its deletion to be
			// undone
			// wbk.ApplyOrUndoChanges(false, false);

			IsModifiedSheetsList = result;

			updateSheetsListProps();

			// must follow update props
			setSelectedSheet();
		}

		/// <summary>
		///  clears the sheets list - except that, it does not remove
		/// any of the sheets, it just marks them as deleted
		/// </summary>
		public void ClearSheetsList()
		{
			foreach ((string key, Sheet sht) in sheetsList)
			{
				sht.SheetStatus = SheetStatus.SS_DELETED;
			}

			IsModifiedSheetsList = true;

			updateSheetsListProps();
		}

		public void ResetSheetsList()
		{
			string last = "";
			string idCode;


			for (int i = sheetsList.Count - 1; i >= 0; i--)
			{
				if (sheetsList[i].Value.SheetStatus != SheetStatus.SS_EXISTING)
				{
					sheetsList.Remove(sheetsList[i].Key);
					continue;
				}

				idCode = xLib.ExtractIdFromShtName(sheetsList[i].Value.DsName, ExStorConst.EXS_SHT_NAME_SEARCH)!;

				// ReSharper disable once StringCompareToIsCultureSpecific
				if (idCode.CompareTo(last) > 0)
				{
					last = idCode;
				}
			}

			if (!last.IsVoid()) WorkBook.SetLastId(last);


			updateSheetsListProps();

			setSelectedSheet();

		}

		/// <summary>
		/// check the status of each sheet to determine if it
		/// is modified in some way - new, deleted, modified
		/// </summary>
		/// <returns></returns>
		private void validateSheetStatus()
		{
			bool isMod = false;
			bool canUndo = false;

			// watching sheet statuses existing, deleted, new, new deleted, mod deleted
			// not watching modified

			// for existing == no apply changes / no undo changes
			// for deleted == apply changes / undo changes
			// for new ==  apply changes / undo changes
			// for new deleted == no apply changes / can undo changes
			// for mod deleted == no apply changes / can undo changes

			// is mod when any sheet status is not existing and  is not new deleted

			foreach ((string key, Sheet sht) in sheetsList)
			{
				// if (sht.SheetStatus != SheetStatus.SS_EXISTING && 
				// 	sht.SheetStatus != SheetStatus.SS_NEW_DELETED)
				// {
				// 	isMod = true;
				// 	break;
				// }

				if (sht.SheetStatus == SheetStatus.SS_DELETED || 
					sht.SheetStatus == SheetStatus.SS_NEW)
				{
					isMod = true;
					canUndo = true;
					break;
				}

				if (sht.SheetStatus == SheetStatus.SS_NEW_DELETED || 
					sht.SheetStatus == SheetStatus.SS_MOD_DELETED)
				{
					canUndo = true;
				}
			}


			IsModifiedSheetsList = isMod;
			CanUndoSheetsList = canUndo;
		}

		/// <summary>
		/// find and select a sheet that is not deleted - when a sheet exists
		/// </summary>
		private void setSelectedSheet()
		{
			string? selSht = null;

			foreach ((string key, Sheet sht) in sheetsList)
			{
				if (sht.SheetStatus != SheetStatus.SS_DELETED)
				{
					selSht = sht.DsName;
					break;
				}
			}


			SelectSheet = selSht;
		}

		/// <summary>
		/// initialize a sheet by adding a place holder sheet
		/// </summary>
		public void InitSheets([CallerFilePath] string path = "", [CallerMemberName] string name = "")
		{
			Sheet sht = Sheet.PlaceHolder();
			sheetsList.TryAdd(sht.DsName, sht);
		}

		/// <summary>
		/// remove the temporary, place holder sheet, if it exists
		/// </summary>
		public void RemovePlaceHolderSheet()
		{
			// if (sheetsList.Count == 0) return;
			sheetsList.Remove(ExStorConst.K_SHT_PLACE_HOLDER_NAME);
		}
		
		/// <summary>
		/// initialize sheets to an empty list, flag as NOT modified,
		/// and add a placeholder sheet - use this pre-initialize
		/// </summary>
		public void ResetSheets()
		{
			sheetsList.Clear();

			// InitSheets();
		}

		public void SheetListApplyChgs()
		{
			int status;
			foreach ((string key, Sheet sht) in sheetsList)
			{
				status = sheetStatus(sht.SheetStatus);
				
				// existing or created - do nothing
				if (status == 0 ) continue;

				// deleted or mod_deleted - delete the ds
				if (status == -1)
				{
					// ExStorMgr.Instance.sheet
					// -1 = delete a sheet
					if (sht.GotDs) ExStorLib.Instance.DeleteDs(sht.ExsDataStorage!);

					continue;
				}

				// modified - update (not handeled here)
				if (status == 1) throw new InvalidOperationException("Sheet should not be modified");
				
				// new - create the new sheet
				if (status == 2)
				{
					// todo only continue if good to go
					ExStoreRtnCode a = ExStorLib.Instance.WriteNewSheet(sht, shtSchema);

					sht.SheetStatus = SheetStatus.SS_EXISTING;

					// WorkBook.CommitAltChanges(wbkSchema);
					// todo is this correct?  or just save the correct field
					WorkbookApplyChgs(false);
				}
			}

			updateSheetsListProps();

			IsModifiedSheetsList = false;
			
		}

		/// <summary>
		/// process sheet status to determine next step<br/>
		/// -1 delete this
		/// 0 = ignore
		/// 1 update this
		/// 2 save this
		/// </summary>
		private int sheetStatus(SheetStatus ss)
		{
			if (ss == SheetStatus.SS_CREATED)
			{
				return 0;
			}
			if (ss == SheetStatus.SS_NEW)
			{
				return 2;
			}
			if (ss == SheetStatus.SS_NEW_DELETED)
			{
				return 0;
			}
			if (ss == SheetStatus.SS_EXISTING)
			{
				return 0;
			} 
			if (ss == SheetStatus.SS_DELETED)
			{
				return -1;
			} 
			if (ss == SheetStatus.SS_MODIFIED)
			{
				return 1;
			} 
			if (ss == SheetStatus.SS_MOD_DELETED)
			{
				return -1;
			}

			return -1;
		}

		/* private sheet list */

		private void addSheet(Sheet sht)
		{
			if (!sheetsList.TryAdd(sht.DsName, sht)) return;

			sht.Config();

			OnPropChgd(PropertyId.PI_XDATA_SHT, GotAnySheets);

			if (CurrentSheet == null) SelectSheet = sht.DsName;
		}
		
		/*  NOTES
		 * save sheet list
		 */ // xMgr.write sheets ()


	#endregion

	#region status

		/* status */

		/// <summary>
		/// got a valid workbook schema
		/// </summary>
		public bool GotWbkSchema => wbkSchema != null &&  wbkSchema.IsValidObject;

		/// <summary>
		/// got a valid sheet schema
		/// </summary>
		public bool GotShtSchema => shtSchema != null && shtSchema.IsValidObject;

		/// <summary>
		/// got a defined workbook?
		/// </summary>
		public bool GotWorkBook => /*!WorkBook.IsInvalid &&*/ !wbk.IsEmpty;

		/// <summary>
		/// determine if the workbook is empty
		/// </summary>
		public bool IsWorkBookEmpty => wbk.IsEmpty;

		/// <summary>
		/// does the workbook object have a DS
		/// </summary>
		public bool GotWbkDs => wbk.GotDs;

		/// <summary>
		/// true if the named sheet is flagged as empty
		/// </summary>
		// ReSharper disable once UnusedMember.Global
		public bool IsSheetEmpty(string name)
		{
			return GetSheet(name).IsEmpty;
		}

		/// <summary>
		/// does the sheets list have any sheets
		/// </summary>
		public bool GotAnySheets => sheetsList.Count > 0;

	#endregion

	#region temp objects

		/* temp objects */

		// temp objects used for various operations that do not need to be
		// kept in the workbook or sheet objects

		/* workbook */

		/// <summary>
		/// temp wbk version
		/// </summary>
		public string TempWbkVersion { get; set; }

		/// <summary>
		/// temp ExListItem for a schema data storage
		/// </summary>
		public ExListItem<Schema>? TempWbkSchemaEx { get; set; }

		// single item
		/// <summary>
		/// temp ExListItem for a workbook data storage
		/// </summary>
		public ExListItem<DataStorage>? TempWbkDsEx { get; set; }
		
		/// <summary>
		/// temp workbook entity
		/// </summary>
		public Entity? TempWbkEntity
		{
			get; 
			set;
		}

		// not used for validation but for lib routines
		/// <summary>
		/// temp list of workbook datastorages<br/>
		/// not used for validation but for lib routines
		/// </summary>
		public IList<DataStorage>? TempWbkDsList { get; set; }


		/* sheet */

		/// <summary>
		/// temp sht version
		/// </summary>
		public string TempShtVersion { get; set; }

		/// <summary>
		/// temp schema ExListItem
		/// </summary>
		public ExListItem<Schema>? TempShtSchemaEx { get; set; }

		/// <summary>
		/// temp list of ExListItems\DataStorage\
		/// </summary>
		public ExList<DataStorage>? TempShtDsListEx { get; set; }

		// // public Entity? TempShtEntity { get; set; }
		//
		// // todo fix name
		// // not used for validation but for lib routines
		// public IList<DataStorage>? TempShtDsList { get; set; }

		/* workbook */

		public bool GotTempWbkVersion => !TempWbkVersion.IsVoid();

		/// <summary>
		/// temp workbook schema is not null and is valid
		/// </summary>
		public bool GotTempWbkSchema => (TempWbkSchemaEx != null && TempWbkSchemaEx.Item.IsValidObject);

		/// <summary>
		/// temp workbook DS is not null and is valid
		/// </summary>
		public bool GotTempWbkDs => (TempWbkDsEx != null && TempWbkDsEx.Item.IsValidObject);

		/// <summary>
		/// temp entity is not null and is valid
		/// </summary>
		public bool GotTempWbkEntity => (TempWbkEntity != null && TempWbkEntity.IsValid());

		/// <summary>
		/// list of not null and has > 0 elements
		/// </summary>
		public bool GotTempWbkDsList => (TempWbkDsList != null && TempWbkDsList.Count > 0);

		/* sheet */

		public bool GotTempShtVersion => !TempShtVersion.IsVoid();

		/// <summary>
		/// temp sheet schema is not null and is valid
		/// </summary>
		public bool GotTempShtSchema => (TempShtSchemaEx != null && TempShtSchemaEx.Item.IsValidObject);

		/// <summary>
		/// temp sheet DS is not null and is valid
		/// </summary>
		public bool GotTempShtDs => TempShtDsListEx != null;

		// /// <summary>
		// /// temp entity is not null and is valid
		// /// </summary>
		// public bool GotTempShtEntity => (TempShtEntity != null && TempShtEntity.IsValid());

		// /// <summary>
		// /// list is not null and has > 0 elements
		// /// </summary>
		// public bool GotTempAnySheets => (TempShtDsList != null && TempShtDsList.Count > 0);

		/// <summary>
		/// Exlist is not null and has > 0 elements
		/// </summary>
		public bool GotTempAnySheetsEx => (TempShtDsListEx != null && TempShtDsListEx.GoodItemsCount > 0);

	#endregion

	#region event consuming

		private void ChildOnPropChgd(object sender, PropChgEvtArgs e)
		{
			if (e.PropId == PropertyId.PI_XDATA_SHT_MOD)
			{
				validateSheetStatus();
			}
			else if (e.PropId == PropertyId.PI_XDATA_WBK_MOD)
			{
				OnPropertyChanged(nameof(WorkBook));
				OnPropertyChanged(nameof(NeedsSaving));
			}
		}

	#endregion

	#region event publishing

		[DebuggerStepThrough]
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}
		public event PropertyChangedEventHandler? PropertyChanged;



		public delegate void PropChgdEventHandler(object sender, PropChgEvtArgs e);

		public event PropChgdEventHandler PropChgd;

		protected void OnPropChgd(PropertyId pi, dynamic value)
		{
			PropChgd?.Invoke(this, new PropChgEvtArgs(pi, value));
		}

		protected virtual void OnPropChgd(PropChgEvtArgs e)
		{
			PropChgd?.Invoke(this, e);
		}


		// public delegate void  ExStorStatusChangedEventHandler(object sender);
		//
		// public event ExStorData.ExStorStatusChangedEventHandler  ExStorStatusChanged;
		//
		// protected void RaiseExStorStatusChangedEvent()
		// {
		// 	ExStorStatusChanged?.Invoke(this);
		// }


		public delegate void RestartRequiredEventHandler(object sender, bool? e);

		public event RestartRequiredEventHandler RestartRequiredChanged;

		protected void RaiseRestartRequiredEvent(bool? e)
		{
			RestartRequiredChanged?.Invoke(this, e);
		}

	#endregion

	#region system overrides

		public override string ToString()
		{
			string w = $"{wbk.DsName} [{wbk.ModelTitle}]";
			string s = sheetsList.Count > 0 ? sheetsList.ToArray()[0].Key : "empty";
			return $"WBK| {w} | SHT| {s}";
		}

	#endregion

		private void SheetsViewSourceOnFilter(object sender, FilterEventArgs e)
		{
			if (e.Item is KeyValuePair<string, Sheet> kvp)
			{
				e.Accepted = kvp.Value.SheetStatus != SheetStatus.SS_DELETED;
			}

			else e.Accepted = false;
		}



	}
}