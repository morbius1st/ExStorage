#region + Using Directives

using static ShExStorageN.ShSchemaFields.ShScSupport1.SchemaFieldDisplayLevel;
using static ShExStorageN.ShSchemaFields.ShScSupport1.RowUpdateRules;
using static ShExStorageN.ShSchemaFields.ShScSupport1.ShExConst;
using static ShExStorageC.ShSchemaFields1.ScSupport1.SchemaSheetKey;
using static ShExStorageC.ShSchemaFields1.ScSupport1.SchemaRowKey;
using static ShExStorageC.ShSchemaFields1.ScSupport1.SchemaLockKey;

#endregion

// user name: jeffs
// created:   10/24/2022 7:20:59 PM

namespace ShSchemaFields.ScSupport
{
	public static class ScInfoMeta
	{
		// private static readonly Lazy<ScInfoMeta> instance =
		// 	new Lazy<ScInfoMeta>(() => new ScInfoMeta());
		//
		// public static ScInfoMeta Instance => instance.Value;
		//
		// private ScInfoMeta() {}

		public const string MODEL_PATH = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\RevitProjects\ExStorage\.RevitFiles";
		public const string MODEL_NAME = @"HasDataStorage.rvt";
		public const string SF_SCHEMA_DESC = "Rows Sheet DataStorage";
		public const string CF_SCHEMA_DESC = "Rows Row DS";
		public const string LF_SCHEMA_DESC = "Rows lock DS";


		public static  void
			ConfigData<TKey>(Dictionary<TKey, ShScFieldDefData<TKey>> fields,
			Dictionary<TKey, ShScFieldDefMeta<TKey>> meta)
			where TKey : Enum
		{
			// Tdata d = new Tdata();

			foreach (KeyValuePair<TKey,  ShScFieldDefMeta<TKey>> kvp in meta)
			{
				fields.Add(kvp.Key,
					new ShScFieldDefData<TKey>(kvp.Value.Key, 
						new DynaValue(kvp.Value.Value), kvp.Value));
			}

		}


		public static Dictionary<SchemaSheetKey, ShScFieldDefMeta<SchemaSheetKey>> FieldsSheet => fieldsSheet;
		public static Dictionary<SchemaRowKey, ShScFieldDefMeta<SchemaRowKey>> FieldsRow => fieldsRow;
		public static Dictionary<SchemaLockKey, ShScFieldDefMeta<SchemaLockKey>> FieldsLock => fieldsLock;


		private static Dictionary<SchemaSheetKey, ShScFieldDefMeta<SchemaSheetKey>> fieldsSheet =
			new Dictionary<SchemaSheetKey, ShScFieldDefMeta<SchemaSheetKey>>
			{
			// @formatter:off

			// these are pre-set by the creation of the object
			{TK0_KEY,              new ShScFieldDefMeta<SchemaSheetKey> (TK0_KEY          , "Key"        , "Access Key"                   ,new DynaValue(TK0_KEY)                                      , DL_ADVANCED)},
			{TK0_SCHEMA_NAME,      new ShScFieldDefMeta<SchemaSheetKey> (TK0_SCHEMA_NAME  , "Name"       , "Schema Name"                  ,new DynaValue(K_NOT_DEFINED)                                , DL_ADVANCED)},
			{TK0_DESCRIPTION,      new ShScFieldDefMeta<SchemaSheetKey> (TK0_DESCRIPTION  , "Desc"       , "Description"                  ,new DynaValue(SF_SCHEMA_DESC)                               , DL_ADVANCED)},
			{TK0_VERSION,          new ShScFieldDefMeta<SchemaSheetKey> (TK0_VERSION      , "Version"    , "Sheet Schema Version"         ,new DynaValue("1.0")                                        , DL_MEDIUM)},
			{TK0_MODEL_PATH,       new ShScFieldDefMeta<SchemaSheetKey> (TK0_MODEL_PATH   , "ModelPath"  , "Path of Model"                ,new DynaValue(MODEL_PATH)                                   , DL_MEDIUM)},
			{TK0_MODEL_NAME,       new ShScFieldDefMeta<SchemaSheetKey> (TK0_MODEL_NAME   , "ModelName"  , "Name of Model"                ,new DynaValue(MODEL_NAME)                                   , DL_MEDIUM)},
			{TK1_DEVELOPER,        new ShScFieldDefMeta<SchemaSheetKey> (TK1_DEVELOPER    , "Developer"  , "Name of Developer"            ,new DynaValue(UtilityLibrary.CsUtilities.CompanyName)       , DL_MEDIUM)},

			// these are pre-set by the creation of the object & when the object is modified
			{TK2_USER_NAME,        new ShScFieldDefMeta<SchemaSheetKey> (TK2_USER_NAME    , "UserName"   , "User Name of Sheet Modifier"  ,new DynaValue(UtilityLibrary.CsUtilities.UserName)          , DL_ADVANCED)},
			{TK2_MODIFY_DATE,      new ShScFieldDefMeta<SchemaSheetKey> (TK2_MODIFY_DATE  , "ModifyDate" , "Date Modified (or Created)"   ,new DynaValue(DateTime.UtcNow.ToString())                   , DL_MEDIUM)},

			// these are variable depending on the database
			{TK9_GUID,             new ShScFieldDefMeta<SchemaSheetKey> (TK9_GUID         , "GUID"       , "Sheet GUID"                   ,new DynaValue(K_NOT_DEFINED)                                , DL_MEDIUM)}
			
				// @formatter:on
			};


		private static Dictionary<SchemaRowKey, ShScFieldDefMeta<SchemaRowKey>> fieldsRow =
			new Dictionary<SchemaRowKey, ShScFieldDefMeta<SchemaRowKey>>
			{
			// @formatter:off

			// these are pre-set by the creation of the object
			{RK0_KEY        ,      new ShScFieldDefMeta<SchemaRowKey> (RK0_KEY              , "Key"           , "Access Key"                 , new DynaValue(RK0_KEY)                                 , DL_ADVANCED)},
			{RK0_SCHEMA_NAME,      new ShScFieldDefMeta<SchemaRowKey> (RK0_SCHEMA_NAME      , "Name"          , "Schema Name"                , new DynaValue(K_NOT_DEFINED)                           , DL_ADVANCED)},
			{RK0_DESCRIPTION,      new ShScFieldDefMeta<SchemaRowKey> (RK0_DESCRIPTION      , "Desc"          , "Description"                , new DynaValue(CF_SCHEMA_DESC)                          , DL_ADVANCED)},
			{RK0_VERSION    ,      new ShScFieldDefMeta<SchemaRowKey> (RK0_VERSION          , "Version"       , "Row Schema Version"        , new DynaValue("1.0")                                   , DL_MEDIUM)},
			{RK0_MODEL_PATH ,      new ShScFieldDefMeta<SchemaRowKey> (RK0_MODEL_PATH       , "ModelPath"     , "Path of Model"              , new DynaValue(K_NOT_DEFINED)                           , DL_MEDIUM)},
			{RK0_MODEL_NAME ,      new ShScFieldDefMeta<SchemaRowKey> (RK0_MODEL_NAME       , "ModelName"     , "Name of Model"              , new DynaValue(K_NOT_DEFINED)                           , DL_MEDIUM)},
	
	
			// these are pre-set by the creation of the object & when the object is modified
			{RK2_USER_NAME  ,      new ShScFieldDefMeta<SchemaRowKey> (RK2_USER_NAME        , "UserName"      , "User Name of Row Creator"  , new DynaValue(UtilityLibrary.CsUtilities.UserName)     , DL_ADVANCED)},
			{RK2_MODIFY_DATE,      new ShScFieldDefMeta<SchemaRowKey> (RK2_MODIFY_DATE      , "ModifyDate"    , "Date Modified (or Created)" , new DynaValue(DateTime.UtcNow.ToString())              , DL_MEDIUM)},
	
	
			// these are variable depending on the database
			// value may not apply
			{RK9_SEQUENCE         ,new ShScFieldDefMeta<SchemaRowKey> (RK9_SEQUENCE         , "Sequence"      , "Evaluation Sequence"        , new DynaValue(K_NOT_DEFINED)                           , DL_BASIC)},
			{RK9_UPDATE_RULE      ,new ShScFieldDefMeta<SchemaRowKey> (RK9_UPDATE_RULE      , "UpdateRule"    , "Update Rule"                , new DynaValue(UR_UNDEFINED)                            , DL_BASIC)},
			{RK9_CELL_FAMILY_NAME  ,new ShScFieldDefMeta<SchemaRowKey> (RK9_CELL_FAMILY_NAME  , "RowFamilyName" , "Name of the Rows Family"   , new DynaValue(K_NOT_DEFINED)                           , DL_BASIC)},
			{RK9_SKIP             ,new ShScFieldDefMeta<SchemaRowKey> (RK9_SKIP             , "Skip"          , "Skip Updating"              , new DynaValue(false)                                   , DL_BASIC)},
			{RK9_XL_FILE_PATH     ,new ShScFieldDefMeta<SchemaRowKey> (RK9_XL_FILE_PATH     , "XlFilePath"    , "File Path to the Excel File", new DynaValue(K_NOT_DEFINED)                           , DL_BASIC)},
			{RK9_XL_WORKSHEET_NAME,new ShScFieldDefMeta<SchemaRowKey> (RK9_XL_WORKSHEET_NAME, "Xlworksheet"   , "Name of the Excel Worksheet", new DynaValue(K_NOT_DEFINED)                           , DL_BASIC)},
			{RK9_GUID             ,new ShScFieldDefMeta<SchemaRowKey> (RK9_GUID             , "GUID"          , "Row GUID"                  , new DynaValue(K_NOT_DEFINED)                           , DL_DEBUG)},
			// Add(/*SO_ALL  ,*/   new ScFieldDef<SchemaRowKey> (RK_VALUE            , "Value"         , "Value"                      , new DynaValue(0.0)                                            , DL_ADVANCED)},

				// @formatter:on
			};

		private static Dictionary<SchemaLockKey, ShScFieldDefMeta<SchemaLockKey>> fieldsLock =
			new Dictionary<SchemaLockKey, ShScFieldDefMeta<SchemaLockKey>>
			{
			// @formatter:off

			// these are pre-set by the creation of the object
			{LK0_KEY         ,     new ShScFieldDefMeta<SchemaLockKey> (LK0_KEY              , "Key"           , "Access Key"                 , new DynaValue(LK0_KEY)                                 , DL_ADVANCED)},
			{LK0_SCHEMA_NAME ,     new ShScFieldDefMeta<SchemaLockKey> (LK0_SCHEMA_NAME      , "Name"          , "Schema Name"                , new DynaValue(K_NOT_DEFINED)                           , DL_BASIC)},
			{LK0_DESCRIPTION ,     new ShScFieldDefMeta<SchemaLockKey> (LK0_DESCRIPTION      , "Desc"          , "Description"                , new DynaValue(LF_SCHEMA_DESC)                          , DL_BASIC)},
			{LK0_VERSION     ,     new ShScFieldDefMeta<SchemaLockKey> (LK0_VERSION          , "Version"       , "Row Schema Version"        , new DynaValue("1.0")                                   , DL_MEDIUM)},
			{LK0_MODEL_PATH  ,     new ShScFieldDefMeta<SchemaLockKey> (LK0_MODEL_PATH       , "ModelPath"     , "Path of Model"              , new DynaValue(K_NOT_DEFINED)                           , DL_MEDIUM)},
			{LK0_MODEL_NAME  ,     new ShScFieldDefMeta<SchemaLockKey> (LK0_MODEL_NAME       , "ModelName"     , "Name of Model"              , new DynaValue(K_NOT_DEFINED)                           , DL_MEDIUM)},
			{LK1_USER_NAME   ,     new ShScFieldDefMeta<SchemaLockKey> (LK1_USER_NAME        , "UserName"      , "User Name of Lock Creator"  , new DynaValue(UtilityLibrary.CsUtilities.UserName)     , DL_ADVANCED)},
			{LK1_CREATE_DATE ,     new ShScFieldDefMeta<SchemaLockKey> (LK1_CREATE_DATE      , "CreateDate"    , "Date Created"               , new DynaValue(DateTime.UtcNow.ToString())              , DL_MEDIUM)},
			{LK1_MACHINE_NAME,     new ShScFieldDefMeta<SchemaLockKey> (LK1_MACHINE_NAME     , "MachineName"   , "Machine Name of Lock Creator", new DynaValue(UtilityLibrary.CsUtilities.MachineName) , DL_ADVANCED)},
	
	
			// these are variable depending on the database
			{LK9_GUID,             new ShScFieldDefMeta<SchemaLockKey> (LK9_GUID             , "GUID"          , "Lock GUID"                  , new DynaValue(K_NOT_DEFINED)                           , DL_DEBUG)},
			// Add(/*SO_ALL  ,*/   new ScFieldDef<SchemaLockKey> (LK_VALUE            , "Value"         , "Value"                      , new DynaValue(0.0)                                            , DL_BASIC));

				// @formatter:on
			};
	}
}