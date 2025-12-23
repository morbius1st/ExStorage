using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStoreTest2026;
using ExStoreTest2026.Windows;
using UtilityLibrary;
using static ExStorSys.ExSysStatus;

// projname: $projectname$
// itemname: ExStorData
// username: jeffs
// created:  10/17/2025 10:48:45 PM

namespace ExStorSys
{
	public class ExStorData  //: INotifyPropertyChanged
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
		private Dictionary<string, Sheet> sheets;

		private Schema? wbkSchema;
		private Schema? shtSchema;

		private MainWinModelUi xMui;

		private bool? restartRequired;
		private ExSysStatus exStorStatus;

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
			// ObjectId = AppRibbon.ObjectIdx++;
			ObjectId = ExStorStartMgr.Instance?.AddObjId(nameof(ExStorData)) ?? -1;
			WorkBook = WorkBook.CreateEmptyWorkBook();
			ResetSheets();

			xMui = MainWinModelUi.Instance;
		}

		public void Restore()
		{
			xMui = MainWinModelUi.Instance;
		}

	#endregion

	#region public objects and properties

		public ExSysStatus ExStorStatus
		{
			get => exStorStatus;
			private set
			{
				exStorStatus = value;
				// RaiseExStorStatusChangedEvent();

				OnPropChgd(new PropChgEvtArgs(PropertyId.PI_XSYS_STATUS, value));
			}
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

		/* utility */

		/* reset */

		public void ResetAll()
		{
			ResetWorkBook();
			ResetWorkBookSchemaSilent();
			ResetSheets();
			ResetSheetSchemaSilent();
			ResetTemp();
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
			if (name.IsVoid() || !sheets.TryGetValue(name, out Sheet? sheet)) return;

			sheets[name] = Sheet.CreateEmptySheet(name);
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
		/// initialize sheets to an empty list
		/// </summary>
		public void ResetSheets()
		{
			sheets = new Dictionary<string, Sheet>();
		}

		/// <summary>
		/// reset all temp objects to null or empty
		/// </summary>
		public void ResetTemp()
		{
			TempWbkVersion = string.Empty;
			TempWbkSchema  = null;
			TempWbkDs      = null;
			TempWbkEntity  = null;			
			TempWbkDsList  = new List<DataStorage>();

			TempShtVersion = string.Empty;
			TempShtSchema  = null;
			TempShtDsExList= new ();
			TempShtEntity  = null;
			TempShtDsList  = new  List<DataStorage>();
		}


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

		// public void DeleteWbkDs()
		// {
		// 	wbk.ExsDataStorage = null;
		// }

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


		/* sheet OPS */

		/* sheet schema */

		/// <summary>
		/// the sheet schema object<br/>
		/// can only be set to a schema once<br/>
		/// but can be set to null - which triggers the ResetRequired event
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
		/// find and return a sheet by its name<br/>
		/// return invalid sheet if not found
		/// </summary>
		public Sheet GetSheet(string name)
		{
			if (name.IsVoid() || !sheets.TryGetValue(name, out Sheet? sheet)) return Sheet.Invalid();

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

			sht = sheets[name];
			return true;
		}

		/// <summary>
		/// determine if a sheet exists by its name 
		/// </summary>
		public bool GotSheet(string name)
		{
			return sheets.ContainsKey(name);
		}

		/// <summary>
		/// does the named sheet have a DS
		/// </summary>
		public bool GotShtDs(string dsName)
		{
			Sheet sht;
			if (!TryGetSheet(dsName, out sht) /*|| sht.IsInvalid*/ ) return false;

			return sht.GotDs;
		}

		/* sheet list */

		/// <summary>
		/// return the sheets list
		/// </summary>
		public Dictionary<string, Sheet> Sheets => sheets;

		/// <summary>
		/// add a sheet to the sheets list<br/>
		/// but only if creationstation > 0
		/// </summary>
		public void AddSheet(Sheet sht)
		{
			if (sheets.TryAdd(sht.DsName, sht))
			{
				ExStorStatus = ES_SHT_CREATED;
				OnPropChgd(PropertyId.PI_XDATA_SHT, GotAnySheets);
			}
		}

		/// <summary>
		/// update a sheet (replace) with a sheet<br/>
		/// but only if the replacement sheet creationstation > 0
		/// </summary>
		// ReSharper disable once UnusedMember.Global
		public bool UpdateSheet(Sheet sht)
		{
			if (!sheets.ContainsKey(sht.DsName)) return false;

			sheets[sht.DsName] = sht;

			return true;
		}

		/// <summary>
		/// remove a sheet from the sheets list<br/>
		/// but only if creationstation less than 5
		/// </summary>
		// ReSharper disable once UnusedMember.Global
		public bool RemoveSheet(Sheet sht)
		{

			if (!sheets.ContainsKey(sht.DsName)) return false;

			sheets.Remove(sht.DsName);

			return true;
		}

		/// <summary>
		/// return the number of sheets in the list
		/// </summary>
		public int SheetsCount => sheets.Count;


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
		public bool GotAnySheets => sheets.Count > 0;


		/* temp objects */

		// temp objects used for various operations that do not need to be
		// kept in the workbook or sheet objects

		/* workbook */

		public string TempWbkVersion { get; set; }

		public ExListItem<Schema>? TempWbkSchema { get; set; }

		// single item
		public ExListItem<DataStorage>? TempWbkDs { get; set; }
		public Entity? TempWbkEntity { get; set; }

		// not used for validation but for lib routines
		public IList<DataStorage>? TempWbkDsList { get; set; }
		

		/* sheet */

		public string TempShtVersion { get; set; }

		public ExListItem<Schema>? TempShtSchema { get; set; }

		// list of items
		public ExList<DataStorage>? TempShtDsExList { get; set; }
		
		public Entity? TempShtEntity { get; set; }

		// todo fix name
		// not used for validation but for lib routines
		public IList<DataStorage>? TempShtDsList { get; set; }



		// public bool GotTempModelCode => !TempModelCode!.IsVoid();

		/* workbook */

		public bool GotTempWbkVersion => !TempWbkVersion.IsVoid();

		/// <summary>
		/// temp workbook schema is not null and is valid
		/// </summary>
		public bool GotTempWbkSchema => (TempWbkSchema != null && TempWbkSchema.Item.IsValidObject);
		
		/// <summary>
		/// temp workbook DS is not null and is valid
		/// </summary>
		public bool GotTempWbkDs => (TempWbkDs!= null && TempWbkDs.Item.IsValidObject);

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
		public bool GotTempShtSchema => (TempShtSchema != null && TempShtSchema.Item.IsValidObject);

		/// <summary>
		/// temp sheet DS is not null and is valid
		/// </summary>
		public bool GotTempShtDs => TempShtDsExList != null;

		/// <summary>
		/// temp entity is not null and is valid
		/// </summary>
		public bool GotTempShtEntity => (TempShtEntity != null && TempShtEntity.IsValid());

		/// <summary>
		/// list is not null and has > 0 elements
		/// </summary>
		public bool GotTempAnySheets => (TempShtDsList != null && TempShtDsList.Count > 0);

		/// <summary>
		/// Exlist is not null and has > 0 elements
		/// </summary>
		public bool GotTempAnySheetsEx => (TempShtDsExList != null && TempShtDsExList.GoodItemsCount > 0);

		#endregion

		#region event consuming

		#endregion

		#region event publishing

		// public event PropertyChangedEventHandler? PropertyChanged;
		//
		// [DebuggerStepThrough]
		// private void OnPropertyChanged([CallerMemberName] string memberName = "")
		// {
		// 	PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		// }

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
			string w = $"{wbk.DsName} [{wbk.Model_Name}]";
			string s = sheets.Count > 0 ? sheets.ToArray()[0].Key : "empty";
			return $"WBK| {w} | SHT| {s}";
		}

	#endregion
	}
}