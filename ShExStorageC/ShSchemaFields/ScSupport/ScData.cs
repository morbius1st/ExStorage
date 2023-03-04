// Solution:     ExStorage
// Project:       ExStorage
// File:             ScData.cs
// Created:      2022-12-11 (8:57 AM)

using System;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields.ShScSupport;

namespace ShExStorageC.ShSchemaFields.ShScSupport
{
	public static class ScData
	{
		/// <summary>
		/// make the initial generic data set of sheet data
		/// </summary>
		public static ScDataSheet MakeInitialDataSheet1(ShtExId exid)
		{
			ScDataSheet shtd = new ScDataSheet();

			shtd.SetValue(SchemaSheetKey.SK0_SCHEMA_NAME, exid.SchemaName);

			shtd.SetValue(SchemaSheetKey.SK2_MODEL_NAME , exid.Doc.Title);
			shtd.SetValue(SchemaSheetKey.SK2_MODEL_PATH , exid.Doc.PathName);
			shtd.SetValue(SchemaSheetKey.SK0_GUID       , Guid.NewGuid());


			// shtd.Fields[SchemaSheetKey.SK0_SCHEMA_NAME].SetValue = exid.SchemaName;
			//
			// shtd.Fields[SchemaSheetKey.SK2_MODEL_NAME].SetValue = exid.Doc.Title;
			// shtd.Fields[SchemaSheetKey.SK2_MODEL_PATH].SetValue = exid.Doc.PathName;
			//
			// shtd.Fields[SchemaSheetKey.SK0_GUID].SetValue = Guid.NewGuid();

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

			rowd.SetValue(SchemaRowKey.RK0_SCHEMA_NAME      , ShExConstN.K_NOT_DEFINED_STR);
			rowd.SetValue(SchemaRowKey.RK2_MODEL_NAME       , shtd.GetValue<string>(SchemaSheetKey.SK2_MODEL_NAME));
			rowd.SetValue(SchemaRowKey.RK2_MODEL_PATH       , shtd.GetValue<string>(SchemaSheetKey.SK2_MODEL_PATH));
			rowd.SetValue(SchemaRowKey.RK0_GUID             , Guid.NewGuid());
			rowd.SetValue(SchemaRowKey.RK2_CELL_FAMILY_NAME , ShExConstN.K_NOT_DEFINED);
			rowd.SetValue(SchemaRowKey.RK2_SEQUENCE         , ShExConstN.K_NOT_DEFINED);
			rowd.SetValue(SchemaRowKey.RK2_SKIP             , false);
			rowd.SetValue(SchemaRowKey.RK2_UPDATE_RULE      , CellUpdateRules.UR_UNDEFINED);
			rowd.SetValue(SchemaRowKey.RK2_XL_FILE_PATH     , ShExConstN.K_NOT_DEFINED);
			rowd.SetValue(SchemaRowKey.RK2_XL_WORKSHEET_NAME, ShExConstN.K_NOT_DEFINED);

			return rowd;
		}

		// /// <summary>
		// /// make the generic set of lock data
		// /// </summary>
		// public static ScDataLock2 MakeInitialDataLock1(LokExId exid, int index = 0)
		// {
		// 	ScDataLock2 lokd = new ScDataLock2();
		//
		// 	lokd.Fields[(lKey) ShExNTblKeys.TK_SCHEMA_NAME].SetValue = exid.SchemaName;
		// 	lokd.Fields[(lKey) ShExNTblKeys.TK_MODEL_NAME].SetValue = exid.Doc.Title;
		// 	lokd.Fields[(lKey) ShExNTblKeys.TK_MODEL_PATH].SetValue = exid.Doc.PathName;
		// 	lokd.Fields[(lKey) ShExNTblKeys.TK_GUID].SetValue = Guid.NewGuid();
		//
		// 	return lokd;
		// }
	}
}