using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using Autodesk.Revit.DB.ExtensibleStorage;

using ExStoreTest2026;
using ExStoreTest2026.DebugAssist;
using ExStoreTest2026.Windows;

using JetBrains.Annotations;

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

		private MainWinModelUi xMui;

		private bool? restartRequired;

		// private Sheet? currentSheet;
		private string? selectSheet;

		private bool isModifiedShtsList;

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
			// Debug.WriteLine($"\n*** ExStorData init | begin");

			// ObjectId = AppRibbon.ObjectIdx++;
			ObjectId = ExStorStartMgr.Instance?.AddObjId(nameof(ExStorData)) ?? -1;
			WorkBook = WorkBook.CreateEmptyWorkBook();
			
			sheetsList = new ObservableDictionary<string, Sheet>();
			
			ResetSheets();
			InitSheets();
			IsModifiedShtsList = false;

			xMui = MainWinModelUi.Instance;

			SheetsViewSource = new CollectionViewSource {Source = Sheets};
			SheetsViewSource.SortDescriptions.Add(new SortDescription("Value.OpSequence", ListSortDirection.Ascending));

			SheetsNoDeletedViewSource = new CollectionViewSource {Source = Sheets};
			SheetsViewSource.SortDescriptions.Add(new SortDescription("Value.OpSequence", ListSortDirection.Ascending));
			SheetsNoDeletedViewSource.Filter += SheetsViewSourceOnFilter;

			// Debug.WriteLine($"\n*** ExStorData | exit ({ObjectId})");
		}

		public void Restore()
		{
			xMui = MainWinModelUi.Instance;
		}

	#endregion

	#region general propertries

		public ExSysStatus ExStorStatus
		{
			get => xMui.ExSysStatus;
			private set => OnPropChgd(new PropChgEvtArgs(PropertyId.PI_XSYS_STATUS, value));
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
				ExStorStatus = ES_RESTART_REQD;
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

			IsModifiedShtsList = false;
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
					ExStorStatus = ES_RESTART_REQD;
				}
				else
				{
					ExStorStatus = ES_WBK_SCHEMA_CREATED;
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

				ExStorStatus = ES_WBK_CREATED;
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

		// public FieldData<WorkBookFieldKeys> GetWbkFieldData2(WorkBookFieldKeys key)
		// {
		// 	FieldDef<WorkBookFieldKeys> a = Fields.WorkBookFields[key];
		//
		// 	// if (!GotWorkBook) return FieldData<WorkBookFieldKeys>.Empty();
		//
		// 	return WorkBook.GetField(key);
		// }

		// public FieldDef<WorkBookFieldKeys> GetWbkFieldDef(WorkBookFieldKeys key)
		// {
		// 	return Fields.WorkBookFields[key];
		// }

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
					ExStorStatus = ES_RESTART_REQD;
				}
				else
				{
					ExStorStatus = ES_SHT_SCHEMA_CREATED;
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
			get => selectSheet;
			set
			{
				if ((value ?? "").Equals(selectSheet)) return;
				
				selectSheet = value;

				OnPropertyChanged();

				OnPropertyChanged(nameof(CurrentSheet));
			}
		}


	#endregion

	#region Sheet List

		/* sheet list */

		/* sheet list properties */

		public CollectionViewSource SheetsViewSource {get; private set;}
		public CollectionViewSource SheetsNoDeletedViewSource {get; private set;}

		private void updateSheetsListProps()
		{
			SheetsViewSource.View.Refresh();
			OnPropertyChanged(nameof(SheetsViewSource));

			SheetsNoDeletedViewSource.View.Refresh();
			OnPropertyChanged(nameof(SheetsNoDeletedViewSource));
		}

		/// <summary>
		/// flag that the sheets list has been modified<br/>
		/// but only after initialization (tracking turned on)
		/// </summary>
		public bool IsModifiedShtsList
		{
			get => isModifiedShtsList;
			private set
			{
				if (value == isModifiedShtsList) return;
				isModifiedShtsList = value;
				OnPropertyChanged();
			}
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


		/// <summary>
		/// add a sheet to the sheets list - before system initialized
		/// </summary>
		public void AddSheetPreInit(Sheet sht)
		{
			sht.SheetStatus = SheetStatus.SS_EXISTING;
			addSheet(sht);

			updateSheetsListProps();
		}

		/// <summary>
		/// add a sheet to the sheets list - after system initialized<br/>
		/// i.e., sets is modified
		/// </summary>
		public void AddSheet(Sheet sht)
		{
			sht.SheetStatus = SheetStatus.SS_NEW;
			addSheet(sht);

			IsModifiedShtsList = true;

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

			IsModifiedShtsList = true;
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
			sheetsList[name].SheetStatus = SheetStatus.SS_DELETED;

			// SelectSheet = null;

			IsModifiedShtsList = true;
			
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

			validateSheetsListStatus();
			updateSheetsListProps();

			// must follow update props
			setSelectedSheet();
		}

		private void validateSheetsListStatus()
		{
			bool isMod = false;

			foreach ((string key, Sheet sht) in sheetsList)
			{
				if (sht.SheetStatus != SheetStatus.SS_EXISTING)
				{
					isMod = true;
					break;
				}
			}

			IsModifiedShtsList = isMod;
		}

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


		/* private sheet list */

		private void addSheet(Sheet sht)
		{
			if (!sheetsList.TryAdd(sht.DsName, sht)) return;

			sht.Config();

			ExStorStatus = ES_SHT_CREATED;

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
		public Entity? TempWbkEntity { get; set; }

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

	#endregion

	#region event publishing

		public event PropertyChangedEventHandler? PropertyChanged;
		
		[DebuggerStepThrough]
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

		public delegate void PropChgdEventHandler(object sender, PropChgEvtArgs e);

		public event ExStorData.PropChgdEventHandler PropChgd;

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

		public event ExStorData.RestartRequiredEventHandler RestartRequiredChanged;

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