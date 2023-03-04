#region + Using Directives

using static ShExStorageN.ShSchemaFields.ShScSupport.SchemaFieldDisplayLevel;
using static ShExStorageN.ShSchemaFields.ShScSupport.CellUpdateRules;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExConstN;
using static ShExStorageC.ShSchemaFields.ShScSupport.ShExConstC;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaSheetKey;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaRowKey;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaLockKey;
using static ShExStorageC.ShSchemaFields.ShScSupport.ScRowKeys;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExNTblKeys;

#endregion

// user name: jeffs
// created:   10/24/2022 7:20:59 PM

namespace ShExStorageC.ShSchemaFields.ScSupport
{
	public static class ScInfoMetaRow2
	{

		public static Dictionary<KEY, ScFieldDefMeta2> FieldsRow => fieldsRow;



		private static Dictionary<KEY, ScFieldDefMeta2> fieldsRow =
			new Dictionary<KEY, ScFieldDefMeta2>
			{
			// @formatter:off

			// these are pre-set by the creation of the object
			{(rKey) TK_KEY        ,      new ScFieldDefMeta2 ((rKey) TK_KEY              , KEY_FIELD_NAME  , "Access Key"                 , new DynaValue(RK0_KEY)                                 , DL_ADVANCED)},
			{(rKey) TK_SCHEMA_NAME,      new ScFieldDefMeta2 ((rKey) TK_SCHEMA_NAME      , "Name"          , "Schema Name"                , new DynaValue(K_NOT_DEFINED_STR)                       , DL_ADVANCED)},
			{(rKey) TK_DESCRIPTION,      new ScFieldDefMeta2 ((rKey) TK_DESCRIPTION      , "Desc"          , "Description"                , new DynaValue(CF_SCHEMA_DESC)                          , DL_ADVANCED)},
			{(rKey) TK_VERSION    ,      new ScFieldDefMeta2 ((rKey) TK_VERSION          , "Version"       , "Row Schema Version"         , new DynaValue("1.0")                                   , DL_MEDIUM)},
			{(rKey) TK_MODEL_PATH ,      new ScFieldDefMeta2 ((rKey) TK_MODEL_PATH       , "ModelPath"     , "Path of Model"              , new DynaValue(K_NOT_DEFINED)                           , DL_MEDIUM)},
			{(rKey) TK_MODEL_NAME ,      new ScFieldDefMeta2 ((rKey) TK_MODEL_NAME       , "ModelName"     , "Name of Model"              , new DynaValue(K_NOT_DEFINED)                           , DL_MEDIUM)},
	
	
			// these are pre-set by the creation of the object & when the object is modified
			{(rKey) TK_USERNAME   ,      new ScFieldDefMeta2 ((rKey) TK_USERNAME         , "UserName"      , "User Name of Row Creator"   , new DynaValue(UtilityLibrary.CsUtilities.UserName)      , DL_ADVANCED)},
			{(rKey) TK_DATE       ,      new ScFieldDefMeta2 ((rKey) TK_DATE             , "ModifyDate"    , "Date Modified (or Created)" , new DynaValue(DateTime.UtcNow.ToString())              , DL_MEDIUM)},
	
	
			// these are variable depending on the database
			// value may not apply
			{RK_SEQUENCE         ,       new ScFieldDefMeta2 (RK_SEQUENCE                , "Sequence"      , "Evaluation Sequence"        , new DynaValue(K_NOT_DEFINED)                           , DL_BASIC)},
			{RK_UPDATE_RULE      ,       new ScFieldDefMeta2 (RK_UPDATE_RULE             , "UpdateRule"    , "Update Rule"                , new DynaValue(UR_UNDEFINED)                            , DL_BASIC)},
			{RK_CELL_FAMILY_NAME ,       new ScFieldDefMeta2 (RK_CELL_FAMILY_NAME        , "CellFamilyName", "Name of the Cell Family"    , new DynaValue(K_NOT_DEFINED_STR)                       , DL_BASIC)},
			{RK_SKIP             ,       new ScFieldDefMeta2 (RK_SKIP                    , "Skip"          , "Skip Updating"              , new DynaValue(false)                                   , DL_BASIC)},
			{RK_XL_FILE_PATH     ,       new ScFieldDefMeta2 (RK_XL_FILE_PATH            , "XlFilePath"    , "File Path to the Excel File", new DynaValue(K_NOT_DEFINED)                           , DL_BASIC)},
			{RK_XL_WORKSHEET_NAME,       new ScFieldDefMeta2 (RK_XL_WORKSHEET_NAME       , "Xlworksheet"   , "Name of the Excel Worksheet", new DynaValue(K_NOT_DEFINED)                           , DL_BASIC)},
			{(rKey) TK_GUID      ,       new ScFieldDefMeta2 ((rKey) TK_GUID             , "GUID"          , "Row GUID"                   , new DynaValue(Guid.Empty)								 , DL_DEBUG)},

			// @formatter:on
			};

	}
}