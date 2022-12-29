// Solution:     ExStorage
// Project:       ExStorage
// File:             ScData.cs
// Created:      2022-12-11 (8:57 AM)

using System;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields.ShScSupport;

namespace ShExStorageC.ShSchemaFields.ScSupport
{
	public static class ScData
	{
		/// <summary>
		/// make the initial generic data set of sheet data
		/// </summary>
		public static ScDataSheet MakeInitialDataSheet1(ShtExId exid)
		{
			ScDataSheet shtd = new ScDataSheet();

			// shtd.Fields[TK0_SCHEMA_NAME].SetValue = e.ExsIdSheet;
			shtd.Fields[SchemaSheetKey.SK0_SCHEMA_NAME].SetValue = exid.SchemaName;

			shtd.Fields[SchemaSheetKey.SK2_MODEL_NAME].SetValue = exid.Doc.Title;
			shtd.Fields[SchemaSheetKey.SK2_MODEL_PATH].SetValue = exid.Doc.PathName;

			shtd.Fields[SchemaSheetKey.SK0_GUID].SetValue = Guid.NewGuid();

			return shtd;
		}

		/// <summary>
		/// make the generic data set of row data
		/// </summary>
		public static ScDataRow MakeInitialDataRow1(ScDataSheet shtd)
		{
			ScDataRow rowd = new ScDataRow();

			// dynamic a = shtd.Fields[TK0_MODEL_NAME].DyValue;
			// ScFieldDefData1<SchemaSheetKey> b = shtd.Fields[TK0_MODEL_NAME];
			// Dictionary<SchemaSheetKey, ScFieldDefData1<SchemaSheetKey>> c = shtd.Fields;

			rowd.Fields[SchemaRowKey.RK0_SCHEMA_NAME].SetValue = ShExStorageN.ShSchemaFields.ShScSupport.ShExConst.K_NOT_DEFINED_STR;
			rowd.Fields[SchemaRowKey.RK2_MODEL_NAME].SetValue = shtd.Fields[SchemaSheetKey.SK2_MODEL_NAME].GetValueAs<string>();
			rowd.Fields[SchemaRowKey.RK2_MODEL_PATH].SetValue = shtd.Fields[SchemaSheetKey.SK2_MODEL_PATH].GetValueAs<string>(); ;

			rowd.Fields[SchemaRowKey.RK0_GUID].SetValue = Guid.NewGuid();

			rowd.Fields[SchemaRowKey.RK2_CELL_FAMILY_NAME].SetValue = ShExStorageN.ShSchemaFields.ShScSupport.ShExConst.K_NOT_DEFINED;
			rowd.Fields[SchemaRowKey.RK2_SEQUENCE].SetValue = ShExStorageN.ShSchemaFields.ShScSupport.ShExConst.K_NOT_DEFINED;
			rowd.Fields[SchemaRowKey.RK2_SKIP].SetValue = false;
			rowd.Fields[SchemaRowKey.RK2_UPDATE_RULE].SetValue = CellUpdateRules.UR_UNDEFINED;
			rowd.Fields[SchemaRowKey.RK2_XL_FILE_PATH].SetValue = ShExStorageN.ShSchemaFields.ShScSupport.ShExConst.K_NOT_DEFINED;
			rowd.Fields[SchemaRowKey.RK2_XL_WORKSHEET_NAME].SetValue = ShExStorageN.ShSchemaFields.ShScSupport.ShExConst.K_NOT_DEFINED;

			return rowd;
		}

		/// <summary>
		/// make the generic set of lock data
		/// </summary>
		public static ScDataLock MakeInitialDataLock1(LokExId exid, int index = 0)
		{
			ScDataLock lokd = new ScDataLock();

			lokd.Fields[SchemaLockKey.LK0_SCHEMA_NAME].SetValue = exid.SchemaName;
			lokd.Fields[SchemaLockKey.LK2_MODEL_NAME].SetValue = exid.Doc.Title;
			lokd.Fields[SchemaLockKey.LK2_MODEL_PATH].SetValue = exid.Doc.PathName;

			lokd.Fields[SchemaLockKey.LK0_GUID].SetValue = Guid.NewGuid();

			return lokd;
		}
	}
}