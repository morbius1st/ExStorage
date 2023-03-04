#region + Using Directives

using System;
using System.Collections.Generic;

using static ShExStorageN.ShSchemaFields.ShScSupport.SchemaFieldDisplayLevel;
using static ShExStorageN.ShSchemaFields.ShScSupport.CellUpdateRules;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExConstN;
using static ShExStorageC.ShSchemaFields.ShScSupport.ShExConstC;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaSheetKey;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaRowKey;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaLockKey;
using ShExStorageN.ShSchemaFields;


#endregion

// user name: jeffs
// created:   10/24/2022 7:20:59 PM

namespace ShExStorageC.ShSchemaFields.ShScSupport
{
	public static class ScInfoMeta
	{
		static ScInfoMeta() { }




		/// <summary>
		/// add all of the meta fields to a data field using default information
		/// </summary>
		public static void 
			ConfigData<TKey>(Dictionary<TKey, ScFieldDefData<TKey>> fields, Dictionary<TKey, ScFieldDefMeta<TKey>> meta)
			where TKey : Enum
		{
			foreach (KeyValuePair<TKey,  ScFieldDefMeta<TKey>> kvp in meta)
			{

				// add the key, the default value, and a reference to the meta info.
				fields.Add(kvp.Key,
					new ScFieldDefData<TKey>(kvp.Value.FieldKey, new DynaValue(kvp.Value.DyValue.Value), kvp.Value));
			}
		}

		public static Dictionary<SchemaSheetKey, ScFieldDefMeta<SchemaSheetKey>> MetaFieldsSheet => metaFieldsSheet;
		public static Dictionary<SchemaRowKey, ScFieldDefMeta<SchemaRowKey>> MetaFieldsRow => metaFieldsRow;
		public static Dictionary<SchemaLockKey, ScFieldDefMeta<SchemaLockKey>> MetaFieldsLock => metaFieldsLock;

		/// <summary>
		/// contains the meta information for each table property as well as the default information for the table property
		/// </summary>
		private static Dictionary<SchemaSheetKey, ScFieldDefMeta<SchemaSheetKey>> metaFieldsSheet =
			new Dictionary<SchemaSheetKey, ScFieldDefMeta<SchemaSheetKey>>
			{
			// @formatter:off

			// these are pre-set by the creation of the object
			{SK0_KEY,              new ScFieldDefMeta<SchemaSheetKey> (SK0_KEY          , KEY_FIELD_NAME, "Access Key"                  ,new DynaValue(SK0_KEY)                                      , DL_ADVANCED)},
			{SK0_SCHEMA_NAME,      new ScFieldDefMeta<SchemaSheetKey> (SK0_SCHEMA_NAME  , "Name"       , "Schema Name"                  ,new DynaValue(K_NOT_DEFINED_STR)                            , DL_ADVANCED)},
			{SK0_DESCRIPTION,      new ScFieldDefMeta<SchemaSheetKey> (SK0_DESCRIPTION  , "Desc"       , "Description"                  ,new DynaValue(SF_SCHEMA_DESC)                               , DL_ADVANCED)},
			{SK0_VERSION,          new ScFieldDefMeta<SchemaSheetKey> (SK0_VERSION      , "Version"    , "Sheet Schema Version"         ,new DynaValue("1.0")                                        , DL_MEDIUM)},

			{SK2_MODEL_PATH,       new ScFieldDefMeta<SchemaSheetKey> (SK2_MODEL_PATH   , "ModelPath"  , "Path of Model"                ,new DynaValue(K_NOT_DEFINED_STR)                            , DL_MEDIUM)},
			{SK2_MODEL_NAME,       new ScFieldDefMeta<SchemaSheetKey> (SK2_MODEL_NAME   , "ModelName"  , "Name of Model"                ,new DynaValue(K_NOT_DEFINED_STR)                            , DL_MEDIUM)},
			{SK2_DEVELOPER,        new ScFieldDefMeta<SchemaSheetKey> (SK2_DEVELOPER    , "Developer"  , "Name of Developer"            ,new DynaValue(UtilityLibrary.CsUtilities.CompanyName)       , DL_MEDIUM)},

			// these are pre-set by the creation of the object & when the object is modified
			{SK0_USER_NAME,        new ScFieldDefMeta<SchemaSheetKey> (SK0_USER_NAME    , "ModifierName" , "Name of Sheet Modifier (or Creator)"  ,new DynaValue(UtilityLibrary.CsUtilities.UserName), DL_ADVANCED)},
			{SK1_MODIFY_DATE,      new ScFieldDefMeta<SchemaSheetKey> (SK1_MODIFY_DATE  , "ModifyDate" , "Date Modified (or Created)"   ,new DynaValue(DateTime.UtcNow.ToString())                   , DL_MEDIUM)},

			// these are variable depending on the database
			{SK0_GUID,             new ScFieldDefMeta<SchemaSheetKey> (SK0_GUID         , "GUID"       , "Sheet GUID"                   ,new DynaValue(Guid.Empty)                                   , DL_MEDIUM)}
			
				// @formatter:on
			};


		private static Dictionary<SchemaRowKey, ScFieldDefMeta<SchemaRowKey>> metaFieldsRow =
			new Dictionary<SchemaRowKey, ScFieldDefMeta<SchemaRowKey>>
			{
			// @formatter:off

			// these are pre-set by the creation of the object
			{RK0_KEY        ,      new ScFieldDefMeta<SchemaRowKey> (RK0_KEY              , KEY_FIELD_NAME  , "Access Key"                 , new DynaValue(RK0_KEY)                                 , DL_ADVANCED)},
			{RK0_SCHEMA_NAME,      new ScFieldDefMeta<SchemaRowKey> (RK0_SCHEMA_NAME      , "Name"          , "Schema Name"                , new DynaValue(K_NOT_DEFINED_STR)                       , DL_ADVANCED)},
			{RK0_DESCRIPTION,      new ScFieldDefMeta<SchemaRowKey> (RK0_DESCRIPTION      , "Desc"          , "Description"                , new DynaValue(CF_SCHEMA_DESC)                          , DL_ADVANCED)},
			{RK0_VERSION    ,      new ScFieldDefMeta<SchemaRowKey> (RK0_VERSION          , "Version"       , "Row Schema Version"         , new DynaValue("1.0")                                   , DL_MEDIUM)},
			{RK2_MODEL_PATH ,      new ScFieldDefMeta<SchemaRowKey> (RK2_MODEL_PATH       , "ModelPath"     , "Path of Model"              , new DynaValue(K_NOT_DEFINED)                           , DL_MEDIUM)},
			{RK2_MODEL_NAME ,      new ScFieldDefMeta<SchemaRowKey> (RK2_MODEL_NAME       , "ModelName"     , "Name of Model"              , new DynaValue(K_NOT_DEFINED)                           , DL_MEDIUM)},
	
	
			// these are pre-set by the creation of the object & when the object is modified
			{RK0_USER_NAME  ,      new ScFieldDefMeta<SchemaRowKey> (RK0_USER_NAME        , "ModifierName"  , "Name of Row Modifier (or Creator)"  , new DynaValue(UtilityLibrary.CsUtilities.UserName)      , DL_ADVANCED)},
			{RK1_MODIFY_DATE,      new ScFieldDefMeta<SchemaRowKey> (RK1_MODIFY_DATE      , "ModifyDate"    , "Date Modified (or Created)" , new DynaValue(DateTime.UtcNow.ToString())              , DL_MEDIUM)},
	
	
			// these are variable depending on the database
			// value may not apply
			{RK2_SEQUENCE         ,new ScFieldDefMeta<SchemaRowKey> (RK2_SEQUENCE         , "Sequence"      , "Evaluation Sequence"        , new DynaValue(K_NOT_DEFINED)                           , DL_BASIC)},
			{RK2_UPDATE_RULE      ,new ScFieldDefMeta<SchemaRowKey> (RK2_UPDATE_RULE      , "UpdateRule"    , "Update Rule"                , new DynaValue(UR_UNDEFINED)                            , DL_BASIC)},
			{RK2_CELL_FAMILY_NAME ,new ScFieldDefMeta<SchemaRowKey> (RK2_CELL_FAMILY_NAME , "CellFamilyName", "Name of the Cell Family"    , new DynaValue(K_NOT_DEFINED_STR)                       , DL_BASIC)},
			{RK2_SKIP             ,new ScFieldDefMeta<SchemaRowKey> (RK2_SKIP             , "Skip"          , "Skip Updating"              , new DynaValue(false)                                   , DL_BASIC)},
			{RK2_XL_FILE_PATH     ,new ScFieldDefMeta<SchemaRowKey> (RK2_XL_FILE_PATH     , "XlFilePath"    , "File Path to the Excel File", new DynaValue(K_NOT_DEFINED)                           , DL_BASIC)},
			{RK2_XL_WORKSHEET_NAME,new ScFieldDefMeta<SchemaRowKey> (RK2_XL_WORKSHEET_NAME, "Xlworksheet"   , "Name of the Excel Worksheet", new DynaValue(K_NOT_DEFINED)                           , DL_BASIC)},
			{RK0_GUID             ,new ScFieldDefMeta<SchemaRowKey> (RK0_GUID             , "GUID"          , "Row GUID"                  , new DynaValue(Guid.Empty)								 , DL_DEBUG)},

			// @formatter:on
			};

		private static Dictionary<SchemaLockKey, ScFieldDefMeta<SchemaLockKey>> metaFieldsLock =
			new Dictionary<SchemaLockKey, ScFieldDefMeta<SchemaLockKey>>
			{
			// @formatter:off

			// these are pre-set by the creation of the object
			{LK0_KEY         ,     new ScFieldDefMeta<SchemaLockKey> (LK0_KEY              , KEY_FIELD_NAME  , "Access Key"                 , new DynaValue(LK0_KEY)                                 , DL_ADVANCED)},
			{LK0_SCHEMA_NAME ,     new ScFieldDefMeta<SchemaLockKey> (LK0_SCHEMA_NAME      , "Name"          , "Schema Name"                , new DynaValue(K_NOT_DEFINED_STR)                       , DL_BASIC)},
			{LK0_DESCRIPTION ,     new ScFieldDefMeta<SchemaLockKey> (LK0_DESCRIPTION      , "Desc"          , "Description"                , new DynaValue(LF_SCHEMA_DESC)                          , DL_BASIC)},
			{LK0_VERSION     ,     new ScFieldDefMeta<SchemaLockKey> (LK0_VERSION          , "Version"       , "Row Schema Version"        , new DynaValue("1.0")                                   , DL_MEDIUM)},
			{LK2_MODEL_PATH  ,     new ScFieldDefMeta<SchemaLockKey> (LK2_MODEL_PATH       , "ModelPath"     , "Path of Model"              , new DynaValue(K_NOT_DEFINED)                           , DL_MEDIUM)},
			{LK2_MODEL_NAME  ,     new ScFieldDefMeta<SchemaLockKey> (LK2_MODEL_NAME       , "ModelName"     , "Name of Model"              , new DynaValue(K_NOT_DEFINED)                           , DL_MEDIUM)},

			// adjusted to require configuration when a lock object is created
			// {LK0_OWNER_ID   ,     new ScFieldDefMeta1<SchemaLockKey> (LK0_OWNER_ID          , "UserName"      , "User Name of Lock Creator"  , new DynaValue(UtilityLibrary.CsUtilities.UserName)     , DL_ADVANCED)},
			{LK0_USER_NAME   ,     new ScFieldDefMeta<SchemaLockKey> (LK0_USER_NAME        , "UserName"      , "User Name of Lock Owner"  , new DynaValue(K_NOT_DEFINED)                           , DL_ADVANCED)},

			{LK1_CREATE_DATE ,     new ScFieldDefMeta<SchemaLockKey> (LK1_CREATE_DATE      , "CreateDate"    , "Date Created"               , new DynaValue(DateTime.UtcNow.ToString())              , DL_MEDIUM)},
			{LK2_MACHINE_NAME,     new ScFieldDefMeta<SchemaLockKey> (LK2_MACHINE_NAME     , "MachineName"   , "Machine Name of Lock Owner", new DynaValue(UtilityLibrary.CsUtilities.MachineName) , DL_ADVANCED)},
	
	
			// these are variable depending on the database
			{LK0_GUID,             new ScFieldDefMeta<SchemaLockKey> (LK0_GUID             , "GUID"          , "Lock GUID"                  , new DynaValue(Guid.Empty)                           , DL_DEBUG)},
			
			// @formatter:on
			};
	}
}