#region + Using Directives

using static ShExStorageN.ShSchemaFields.ShScSupport.SchemaFieldDisplayLevel;
using static ShExStorageN.ShSchemaFields.ShScSupport.CellUpdateRules;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExConstN;
using static ShExStorageC.ShSchemaFields.ShScSupport.ShExConstC;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaSheetKey;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaRowKey;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaLockKey;
using static ShExStorageC.ShSchemaFields.ShScSupport.ScLokKeys;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExNTblKeys;
#endregion

// user name: jeffs
// created:   10/24/2022 7:20:59 PM

namespace ShExStorageC.ShSchemaFields.ScSupport
{
	public static class ScInfoMetaLok2
	{

		public static Dictionary<KEY, ScFieldDefMeta2> FieldsLock => fieldsLock;


		private static Dictionary<KEY, ScFieldDefMeta2> fieldsLock =
			new Dictionary<KEY, ScFieldDefMeta2>
			{
			// @formatter:off

			// these are pre-set by the creation of the object
			{(lKey) TK_KEY         ,     new ScFieldDefMeta2 ((lKey) TK_KEY              , KEY_FIELD_NAME  , "Access Key"                 , new DynaValue(LK0_KEY)                                 , DL_ADVANCED)},
			{(lKey) TK_SCHEMA_NAME ,     new ScFieldDefMeta2 ((lKey) TK_SCHEMA_NAME      , "Name"          , "Schema Name"                , new DynaValue(K_NOT_DEFINED_STR)                       , DL_BASIC)},
			{(lKey) TK_DESCRIPTION ,     new ScFieldDefMeta2 ((lKey) TK_DESCRIPTION      , "Desc"          , "Description"                , new DynaValue(LF_SCHEMA_DESC)                          , DL_BASIC)},
			{(lKey) TK_VERSION     ,     new ScFieldDefMeta2 ((lKey) TK_VERSION          , "Version"       , "Row Schema Version"         , new DynaValue("1.0")                                   , DL_MEDIUM)},
			{(lKey) TK_MODEL_PATH  ,     new ScFieldDefMeta2 ((lKey) TK_MODEL_PATH       , "ModelPath"     , "Path of Model"              , new DynaValue(K_NOT_DEFINED)                           , DL_MEDIUM)},
			{(lKey) TK_MODEL_NAME  ,     new ScFieldDefMeta2 ((lKey) TK_MODEL_NAME       , "ModelName"     , "Name of Model"              , new DynaValue(K_NOT_DEFINED)                           , DL_MEDIUM)},

			// adjusted to require configuration when a lock object is created
			// {LK0_OWNER_ID   ,     new ScFieldDefMeta1<SchemaLockKey> (LK0_OWNER_ID          , "UserName"      , "User Name of Lock Creator"  , new DynaValue(UtilityLibrary.CsUtilities.UserName)     , DL_ADVANCED)},
			{(lKey) TK_USERNAME    ,     new ScFieldDefMeta2 ((lKey) TK_USERNAME          , "UserName"      , "User Name of Lock Creator"  , new DynaValue(K_NOT_DEFINED)                           , DL_ADVANCED)},
			{(lKey) TK_DATE        ,     new ScFieldDefMeta2 ((lKey) TK_DATE              , "CreateDate"    , "Date Created"               , new DynaValue(DateTime.UtcNow.ToString())              , DL_MEDIUM)},
			{LK_MACHINE_NAME       ,     new ScFieldDefMeta2 (LK_MACHINE_NAME             , "MachineName"   , "Machine Name of Lock Creator", new DynaValue(UtilityLibrary.CsUtilities.MachineName) , DL_ADVANCED)},
	
	
			// these are variable depending on the database
			{(lKey) TK_GUID,             new ScFieldDefMeta2 ((lKey) TK_GUID              , "GUID"          , "Lock GUID"                  , new DynaValue(Guid.Empty)                           , DL_DEBUG)},
			
			// @formatter:on
			};
	}
}