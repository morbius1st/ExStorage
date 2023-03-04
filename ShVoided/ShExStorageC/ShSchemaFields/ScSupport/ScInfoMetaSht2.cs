#region + Using Directives

using static ShExStorageN.ShSchemaFields.ShScSupport.SchemaFieldDisplayLevel;
using static ShExStorageN.ShSchemaFields.ShScSupport.CellUpdateRules;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExConstN;
using static ShExStorageC.ShSchemaFields.ShScSupport.ShExConstC;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaRowKey;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaLockKey;
using static ShExStorageC.ShSchemaFields.ShScSupport.ScShtKeys;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExNTblKeys;

#endregion

// user name: jeffs
// created:   10/24/2022 7:20:59 PM

namespace ShExStorageC.ShSchemaFields.ScSupport
{
	public class ScInfoMetaSht2
	{
		public static Dictionary<KEY , ScFieldDefMeta2> FieldsSheet => fieldsSheet;

		private static Dictionary<KEY, ScFieldDefMeta2> fieldsSheet =
			new Dictionary<KEY, ScFieldDefMeta2>
			{
			// @formatter:off
	
			// these are pre-set by the creation of the object
			{(sKey) TK_KEY,              new ScFieldDefMeta2 ((sKey) TK_KEY          , KEY_FIELD_NAME, "Access Key"                  ,new DynaValue(TK_KEY)                                       , DL_ADVANCED)},
			{(sKey) TK_SCHEMA_NAME,      new ScFieldDefMeta2 ((sKey) TK_SCHEMA_NAME  , "Name"       , "Schema Name"                  ,new DynaValue(K_NOT_DEFINED_STR)                            , DL_ADVANCED)},
			{(sKey) TK_DESCRIPTION,      new ScFieldDefMeta2 ((sKey) TK_DESCRIPTION  , "Desc"       , "Description"                  ,new DynaValue(SF_SCHEMA_DESC)                               , DL_ADVANCED)},
			{(sKey) TK_VERSION,          new ScFieldDefMeta2 ((sKey) TK_VERSION      , "Version"    , "Sheet Schema Version"         ,new DynaValue("1.0")                                        , DL_MEDIUM)},
			{(sKey) TK_MODEL_PATH,       new ScFieldDefMeta2 ((sKey) TK_MODEL_PATH   , "ModelPath"  , "Path of Model"                ,new DynaValue(K_NOT_DEFINED_STR)                            , DL_MEDIUM)},
			{(sKey) TK_MODEL_NAME,       new ScFieldDefMeta2 ((sKey) TK_MODEL_NAME   , "ModelName"  , "Name of Model"                ,new DynaValue(K_NOT_DEFINED_STR)                            , DL_MEDIUM)},
			{SK_DEVELOPER        ,       new ScFieldDefMeta2 (SK_DEVELOPER           , "Developer"  , "Name of Developer"            ,new DynaValue(UtilityLibrary.CsUtilities.CompanyName)       , DL_MEDIUM)},
	
			// these are pre-set by the creation of the object & when the object is modified
			{(sKey) TK_USERNAME,         new ScFieldDefMeta2 ((sKey)TK_USERNAME     , "UserName"   , "User Name of Sheet Modifier"  ,new DynaValue(UtilityLibrary.CsUtilities.UserName)          , DL_ADVANCED)},
			{(sKey) TK_DATE,             new ScFieldDefMeta2 ((sKey)TK_DATE         , "ModifyDate" , "Date Modified (or Created)"   ,new DynaValue(DateTime.UtcNow.ToString())                   , DL_MEDIUM)},
	
			// these are variable depending on the database
			{(sKey) TK_GUID,             new ScFieldDefMeta2 ((sKey)TK_GUID         , "GUID"       , "GUID for Sheet"               ,new DynaValue(Guid.Empty)                                   , DL_MEDIUM)}
			
				// @formatter:on
			};
	}
}