using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStoreTest2026;
using UtilityLibrary;
using static ExStorSys.ExoCreationStatus;


// projname: $projectname$
// itemname: ExStorData
// username: jeffs
// created:  10/17/2025 10:48:45 PM

namespace ExStorSys
{
	public class ExStorData
	{
		public int ObjectId;

	#region private fields

		private static readonly Lazy<ExStorData> instance =
			new Lazy<ExStorData>(() => new ExStorData());

	#endregion

	#region objects

		// cannot be null
		private WorkBook wbk;
		private Dictionary<string, Sheet> sheets;

		private Schema? wbkSchema;
		private Schema? shtSchema;

		private bool? restartRequired;

	#endregion

	#region ctor

		private ExStorData()
		{
			init();
		}

		public static ExStorData Instance => instance.Value;

		private void init()
		{
			ObjectId = AppRibbon.ObjectIdx++;

			WorkBook = WorkBook.CreateEmptyWorkBook();
			ResetSheets();
		}

	#endregion

	#region public objects and properties

		/// <summary>
		/// flag that a restart of revit is required
		/// </summary>
		public bool? RestartRequired
		{
			get => restartRequired;
			set
			{
				if (restartRequired == true) return;

				restartRequired = value;

				RaiseRestartRequiredEvent(value);
			}
		}

		/* workbook Schema */

		public bool GotWkbSchema => wbkSchema != null || wbkSchema.IsValidObject;
		
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
				RestartRequired = value == null ? true : false;
			}
		}

		/* workbook */

		/// <summary>
		/// the workbook object<br/>
		/// cannot be set to null
		/// </summary>
		public WorkBook WorkBook
		{
			get => wbk;
			set
			{
				if (wbk.CreationStatus >= CS_GOOD) return;

				// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
				if (value == null) return;

				wbk = value;
			}
		}

		/// <summary>
		/// get an empty workbook
		/// </summary>
		public WorkBook GetEmptyWorkBook()
		{
			return WorkBook.CreateEmptyWorkBook();
		}

		/// <summary>
		/// determine if the workbook is empty
		/// </summary>
		public bool IsWorkBookEmpty => WorkBook.IsEmpty;

		/// <summary>
		/// reset the workbook to an empty workbook
		/// </summary>
		public void ResetWorkBook()
		{
			wbk = WorkBook.CreateEmptyWorkBook();
		}


		/* sheet schema */

		public bool GotShtSchema => shtSchema != null || shtSchema.IsValidObject;
		
		/// <summary>
		/// the sheet schema object<br/>
		/// can only be set to a schema once<br/>
		/// but can be set to null - which triggers the ResetRequired event
		/// </summary>
		public Schema? SheetSchema => shtSchema;

		/* sheet list */

		/// <summary>
		/// does the sheets list have any sheets
		/// </summary>
		public bool GotAnySheets => sheets.Count > 0;

		/// <summary>
		/// initialize sheets to an empty list
		/// </summary>
		public void ResetSheets()
		{
			sheets = new Dictionary<string, Sheet>();
		}

		/// <summary>
		/// true if the named sheet is flagged as empty
		/// </summary>
		public bool IsSheetEmpty(string name)
		{
			return GetSheet(name).IsEmpty;
		}
		
		/// <summary>
		/// add a sheet to the sheets list<br/>
		/// but only if creationstation > 0
		/// </summary>
		public bool AddSheet(Sheet sht)
		{
			if (sht.CreationStatus <= 0) return false;

			return sheets.TryAdd(sht.DsName, sht);
		}

		/// <summary>
		/// update a sheet (replace) with a sheet<br/>
		/// but only if the replacement sheet creationstation > 0
		/// </summary>
		public bool UpdateSheet(Sheet sht)
		{
			if (sht.CreationStatus <= 0) return false;

			if (!sheets.ContainsKey(sht.DsName)) return false;

			sheets[sht.DsName] = sht;

			return true;
		}

		/// <summary>
		/// remove a sheet from the sheets list<br/>
		/// but only if creationstation < 5
		/// </summary>
		public bool RemoveSheet(Sheet sht)
		{
			if (sht.CreationStatus >= CS_CREATED) return false;

			if (!sheets.ContainsKey(sht.DsName)) return false;

			sheets.Remove(sht.DsName);

			return true;
		}

		/// <summary>
		/// reset a sheet (set to empty)<br/>
		/// but only if creationstation >= 5
		public void ResetSheet(string name)
		{
			if (name.IsVoid() || !sheets.ContainsKey(name)) return;

			if (sheets[name].CreationStatus < CS_CREATED) return;

			sheets[name] = Sheet.CreateEmptySheet(name);
		}

		/// <summary>
		/// find and return a sheet by its name<br/>
		/// return invalid sheet if not found
		/// </summary>
		public Sheet GetSheet(string name)
		{
			if (name.IsVoid() || !sheets.ContainsKey(name)) return Sheet.Invalid();

			return sheets[name];
		}

		/// <summary>
		/// determine if a sheet exists by its name 
		/// </summary>
		public bool GotSheet(string name)
		{
			return sheets.ContainsKey(name);
		}


		/* temp objects */

		// temp objects used for various operations that do not need to be
		// kept in the workbook or sheet objects
		public Schema? TempWbkSchema { get; set; }
		public IList<Schema>? TempWbkSchemaList { get; set; }

		public Schema? TempShtSchema { get; set; }
		public IList<Schema>? TempShtSchemaList { get; set; }

		public DataStorage? TempWbkDs { get; set; }
		public IList<DataStorage>? TempWbkDsList { get; set; }
		public Entity? TempWbkEntity { get; set; }

		public DataStorage? TempShtDs { get; set; }
		public IList<DataStorage>? TempShtDsList { get; set; }
		public Entity? TempShtEntity { get; set; }

		public string TempModelCode { get; private set; }

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

	#region event consuming

	#endregion

	#region event publishing

		public delegate void RestartRequiredEventHandler(object sender, bool? e);

		public event ExStorData.RestartRequiredEventHandler RestartRequiredChanged;

		protected virtual void RaiseRestartRequiredEvent(bool? e)
		{
			RestartRequiredChanged?.Invoke(this, e);
		}

	#endregion

	#region system overrides

		public override string ToString()
		{
			string w = $"{wbk.DsName} [{wbk.Model_Name}] [{wbk.ModelCode}]";
			string s = sheets.Count > 0 ? sheets.ToArray()[0].Key : "empty";
			return $"WBK| {w} | SHT| {s}";
		}

	#endregion
	}
}