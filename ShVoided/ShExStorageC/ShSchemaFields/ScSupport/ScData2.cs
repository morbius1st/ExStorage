// Solution:     ExStorage
// Project:       ExStorage
// File:             ScData.cs
// Created:      2022-12-11 (8:57 AM)

namespace ShExStorageC.ShSchemaFields.ScSupport
{
	public static class ScData2
	{
		/// <summary>
		/// make the initial generic data set of sheet data
		/// </summary>
		public static ScDataSheet2 MakeInitialDataSheet2(ShtExId exid)
		{
			ScDataSheet2 shtd = new ScDataSheet2();
		
			// shtd.Fields[TK0_SCHEMA_NAME].SetValue = e.ExsIdSheet;
			shtd.Fields[(sKey) ShExNTblKeys.TK_SCHEMA_NAME].SetValue = exid.SchemaName;
			shtd.Fields[(sKey) ShExNTblKeys.TK_MODEL_NAME].SetValue = exid.Doc.Title;
			shtd.Fields[(sKey) ShExNTblKeys.TK_MODEL_PATH].SetValue = exid.Doc.PathName;
			shtd.Fields[(sKey) ShExNTblKeys.TK_GUID].SetValue = Guid.NewGuid();
		
			return shtd;
		}
		
		/// <summary>
		/// make the generic data set of row data
		/// </summary>
		public static ScDataRow2 MakeInitialDataRow2(ScDataSheet2 shtd)
		{
			ScDataRow2 rowd = new ScDataRow2();
		
			// dynamic a = shtd.Fields[TK0_MODEL_NAME].DyValue;
			// ScFieldDefData1<SchemaSheetKey> b = shtd.Fields[TK0_MODEL_NAME];
			// Dictionary<SchemaSheetKey, ScFieldDefData1<SchemaSheetKey>> c = shtd.Fields;
		
			rowd.Fields[(rKey) ShExNTblKeys.TK_SCHEMA_NAME].SetValue = ShExConstN.K_NOT_DEFINED_STR;
			rowd.Fields[(rKey) ShExNTblKeys.TK_MODEL_NAME].SetValue = shtd.Fields[(sKey) ShExNTblKeys.TK_MODEL_NAME].GetValueAs<string>();
			rowd.Fields[(rKey) ShExNTblKeys.TK_MODEL_PATH].SetValue = shtd.Fields[(sKey) ShExNTblKeys.TK_MODEL_PATH].GetValueAs<string>(); ;
			rowd.Fields[(rKey) ShExNTblKeys.TK_GUID].SetValue = Guid.NewGuid();
			rowd.Fields[ScRowKeys.RK_CELL_FAMILY_NAME].SetValue = ShExConstN.K_NOT_DEFINED;
			rowd.Fields[ScRowKeys.RK_SEQUENCE].SetValue = ShExConstN.K_NOT_DEFINED;
			rowd.Fields[ScRowKeys.RK_SKIP].SetValue = false;
			rowd.Fields[ScRowKeys.RK_UPDATE_RULE].SetValue = CellUpdateRules.UR_UNDEFINED;
			rowd.Fields[ScRowKeys.RK_XL_FILE_PATH].SetValue = ShExConstN.K_NOT_DEFINED;
			rowd.Fields[ScRowKeys.RK_XL_WORKSHEET_NAME].SetValue = ShExConstN.K_NOT_DEFINED;
		
			return rowd;
		}

		/// <summary>
		/// make the generic set of lock data
		/// </summary>
		public static ScDataLock2 MakeInitialDataLock2(LokExId exid, int index = 0)
		{
			ScDataLock2 lokd = new ScDataLock2();

			lokd.Fields[(lKey) ShExNTblKeys.TK_SCHEMA_NAME].SetValue = exid.SchemaName;
			lokd.Fields[(lKey) ShExNTblKeys.TK_MODEL_NAME].SetValue = exid.Doc.Title;
			lokd.Fields[(lKey) ShExNTblKeys.TK_MODEL_PATH].SetValue = exid.Doc.PathName;
			lokd.Fields[(lKey) ShExNTblKeys.TK_GUID].SetValue = Guid.NewGuid();

			return lokd;
		}
	}
}