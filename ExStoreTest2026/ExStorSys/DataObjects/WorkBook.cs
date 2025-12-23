using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStoreTest2026;
using ExStoreTest2026.DebugAssist;
using RevitLibrary;
using ExStorSys;

using static ExStorSys.WorkBookFieldKeys;
using static ExStorSys.ActivateStatus;



// user name: jeffs
// created:   9/17/2025 9:06:55 PM

namespace ExStorSys
{
	/// <summary>
	/// the primary data object stored in the data storage object 
	/// </summary>
	public class WorkBook : ExStorDataObj<WorkBookFieldKeys>
	{
		public int ObjectId;

		private Schema? exsSchema;
		private bool? isEmpty;

		private WorkBook()
		{
			// ObjectId = AppRibbon.ObjectIdx++;

			ObjectId = ExStorStartMgr.Instance?.AddObjId(nameof(WorkBook)) ?? -1;

			Rows = new ();
			init(Fields.WorkBookFields);
			}

		/* primary objects */

		// public override Schema? ExsSchema { get; set; }

		// public Schema? ExsSheetSchema { get; set; }

		/* shortcuts & properties */

		public override int WbkOrSht => 0;

		/// <summary>
		/// the root name for searching for the actual DS - does not include
		/// model code or thereafter.
		/// </summary>
		public override string DsSearchName => ExStorConst.EXS_WBK_NAME_SEARCH;

		/// <summary>
		/// the name for the data storage object.  assigned when the workbook is created
		/// </summary>
		public override string DsName => Rows[PK_DS_NAME].DyValue!.Value;

		/// <summary>
		/// shortcut for access to the description of the workbook
		/// </summary>
		public override string Desc => Rows[PK_AD_DESC].DyValue!.Value;

		// /// <summary>
		// /// the derived name for all sheet entities.  assigned when the workbook is created
		// /// </summary>
		// public string SheetSchemaName => Rows[PK_SD_SHT_SCHEMA_NAME].DyValue!.Value;

		/// <summary>
		/// the name for the workbook schema. fixed value.  assigned when the workbook is created
		/// </summary>
		public override string SchemaName => ExStorMgr.Instance.Exid.WbkSchemaName;
		
		/// <summary>
		/// the description for the workbook schema
		/// </summary>
		public override string SchemaDesc => $"Schema for {ExStorConst.EXS_WBK_NAME_SEARCH}";

		/// <summary>
		/// the guid for the workbook's schema.  assigned when the workbook is created
		/// </summary>
		public override Guid SchemaGuid   => ExStorConst.WbkSchemaGuid;

		// public string ModelCode  => Rows[PK_AD_MODEL_CODE].DyValue!.Value;
		public string Model_Name  => Rows[PK_MD_MODEL_NAME].DyValue!.Value;
		
		public string LastId
		{
			get => Rows[PK_AD_LAST_ID].DyValue!.Value;
			set
			{
				UpdateRow(PK_AD_LAST_ID, new DynaValue(value));
			}
		}

		/* status */
		
		/* methods */

		/// <summary>
		/// update the DS & E objects (S to be removed)
		/// </summary>
		public bool UpdateExsObjects(DataStorage ds, Entity e, Schema s)
		{
			if (!IsEmpty) return false;

			ExsDataStorage = ds;
			ExsEntity = e;
			// ExsSchema = s;

			IsEmpty = false;

			return true;
		}

		/// <summary>
		/// create an "invalid" workbook
		/// </summary>
		public static WorkBook Invalid()
		{
			WorkBook wbk = WorkBook.CreateEmptyWorkBook();

			return wbk;
		}

		/// <summary>
		/// create a workbook with initial / basic data
		/// </summary>
		public static WorkBook CreateEmptyWorkBook()
		{
			WorkBook wbk = new WorkBook();

			wbk.updateWithInitialData();
			wbk.Populated = false;
			return wbk;
		}

		/// <summary>
		/// create a workbook with typical data and with a model code
		/// </summary>
		public static WorkBook CreateNewWorkBook()
		{
			WorkBook wbk = new WorkBook();

			// string mc = ExStorConst.CreateModelCode();

			wbk.updateWithCurrentData();

			wbk.Populated = true;

			return wbk;
		}

		private void updateWithInitialData()
		{
			// UpdateRow(PK_SD_WBK_SCHEMA_NAME, new DynaValue(ExStorConst.WbkSchemaName));
			// UpdateRow(PK_SD_SHT_SCHEMA_NAME, new DynaValue(ExStorConst.ShtSchemaName));
			
			UpdateRow(PK_MD_MODEL_NAME, new DynaValue(ExStorMgr.Instance.Exid.ModelName));
			
			// set to active status
			UpdateRow(PK_AD_STATUS, new DynaValue(AS_ACTIVE));
		}

		private void updateWithCurrentData()
		{
			IsEmpty = false;

			// must be first
			// UpdateRow(PK_AD_MODEL_CODE, new DynaValue(modelCode));

			UpdateRow(PK_DS_NAME, new DynaValue(ExStorMgr.Instance.Exid.CreateWbkDsName()));
			UpdateRow(PK_AD_DESC, new DynaValue($"Workbook for {ExStorMgr.Instance.Exid.ModelName}"));

			UpdateRow(PK_AD_DATE_CREATED  , new DynaValue(DateTime.Now.ToString("s")));
			UpdateRow(PK_AD_NAME_CREATED  , new DynaValue(ExStorConst.UserName));
			UpdateRow(PK_AD_DATE_MODIFIED , new DynaValue(DateTime.Now.ToString("s")));
			UpdateRow(PK_AD_NAME_MODIFIED , new DynaValue(ExStorConst.UserName));
			
			updateWithInitialData();
		}

		public override string ToString()
		{
			return $"{nameof(WorkBook)} [{ObjectId}]";
		}

	}
}
