#region + Using Directives

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ShExStorageN.ShSchemaFields;
using static ShExStorageN.ShSchemaFields.ShScSupport.SchemaFieldDisplayLevel;
using static ShExStorageN.ShSchemaFields.ShScSupport.RowUpdateRules;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExConst;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaTableKey;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaRowKey;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaLockKey;
using Autodesk.Revit.DB;

#endregion

// user name: jeffs
// created:   10/24/2022 7:20:59 PM

namespace ShExStorageC.ShSchemaFields.ScSupport
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
		public const string SF_SCHEMA_DESC = "Rows Table DataStorage";
		public const string CF_SCHEMA_DESC = "Rows Row DS";
		public const string LF_SCHEMA_DESC = "Rows lock DS";


		public static  void
			ConfigData<Tkey>(Dictionary<Tkey, ShScFieldDefData<Tkey>> fields,
			Dictionary<Tkey, ShScFieldDefMeta<Tkey>> meta)
			where Tkey : Enum
		{
			// Tdata d = new Tdata();

			foreach (KeyValuePair<Tkey,  ShScFieldDefMeta<Tkey>> kvp in meta)
			{
				fields.Add(kvp.Key,
					new ShScFieldDefData<Tkey>(kvp.Value.Key, new DynaValue(kvp.Value.Value), kvp.Value));
			}

		}


		public static Dictionary<SchemaTableKey, ShScFieldDefMeta<SchemaTableKey>> FieldsTable => fieldsTable;
		public static Dictionary<SchemaRowKey, ShScFieldDefMeta<SchemaRowKey>> FieldsRow => fieldsRow;
		public static Dictionary<SchemaLockKey, ShScFieldDefMeta<SchemaLockKey>> FieldsLock => fieldsLock;


		private static Dictionary<SchemaTableKey, ShScFieldDefMeta<SchemaTableKey>> fieldsTable =
			new Dictionary<SchemaTableKey, ShScFieldDefMeta<SchemaTableKey>>
			{
			// @formatter:off

			// these are pre-set by the creation of the object
			{SK0_KEY,              new ShScFieldDefMeta<SchemaTableKey> (SK0_KEY          , "Key"        , "Access Key"                   ,new DynaValue(SK0_KEY)                                      , DL_ADVANCED)},
			{SK0_SCHEMA_NAME,      new ShScFieldDefMeta<SchemaTableKey> (SK0_SCHEMA_NAME  , "Name"       , "Schema Name"                  ,new DynaValue(K_NOT_DEFINED)                                , DL_ADVANCED)},
			{SK0_DESCRIPTION,      new ShScFieldDefMeta<SchemaTableKey> (SK0_DESCRIPTION  , "Desc"       , "Description"                  ,new DynaValue(SF_SCHEMA_DESC)                               , DL_ADVANCED)},
			{SK0_VERSION,          new ShScFieldDefMeta<SchemaTableKey> (SK0_VERSION      , "Version"    , "Table Schema Version"         ,new DynaValue("1.0")                                        , DL_MEDIUM)},
			{SK0_MODEL_PATH,       new ShScFieldDefMeta<SchemaTableKey> (SK0_MODEL_PATH   , "ModelPath"  , "Path of Model"                ,new DynaValue(MODEL_PATH)                                   , DL_MEDIUM)},
			{SK0_MODEL_NAME,       new ShScFieldDefMeta<SchemaTableKey> (SK0_MODEL_NAME   , "ModelName"  , "Name of Model"                ,new DynaValue(MODEL_NAME)                                   , DL_MEDIUM)},
			{SK1_DEVELOPER,        new ShScFieldDefMeta<SchemaTableKey> (SK1_DEVELOPER    , "Developer"  , "Name of Developer"            ,new DynaValue(UtilityLibrary.CsUtilities.CompanyName)       , DL_MEDIUM)},

			// these are pre-set by the creation of the object & when the object is modified
			{SK2_USER_NAME,        new ShScFieldDefMeta<SchemaTableKey> (SK2_USER_NAME    , "UserName"   , "User Name of Table Modifier"  ,new DynaValue(UtilityLibrary.CsUtilities.UserName)          , DL_ADVANCED)},
			{SK2_MODIFY_DATE,      new ShScFieldDefMeta<SchemaTableKey> (SK2_MODIFY_DATE  , "ModifyDate" , "Date Modified (or Created)"   ,new DynaValue(DateTime.UtcNow.ToString())                   , DL_MEDIUM)},

			// these are variable depending on the database
			{SK9_GUID,             new ShScFieldDefMeta<SchemaTableKey> (SK9_GUID         , "GUID"       , "Table GUID"                   ,new DynaValue(K_NOT_DEFINED)                                , DL_MEDIUM)}
			
				// @formatter:on
			};


		private static Dictionary<SchemaRowKey, ShScFieldDefMeta<SchemaRowKey>> fieldsRow =
			new Dictionary<SchemaRowKey, ShScFieldDefMeta<SchemaRowKey>>
			{
			// @formatter:off

			// these are pre-set by the creation of the object
			{CK0_KEY        ,      new ShScFieldDefMeta<SchemaRowKey> (CK0_KEY              , "Key"           , "Access Key"                 , new DynaValue(CK0_KEY)                                 , DL_ADVANCED)},
			{CK0_SCHEMA_NAME,      new ShScFieldDefMeta<SchemaRowKey> (CK0_SCHEMA_NAME      , "Name"          , "Schema Name"                , new DynaValue(K_NOT_DEFINED)                           , DL_ADVANCED)},
			{CK0_DESCRIPTION,      new ShScFieldDefMeta<SchemaRowKey> (CK0_DESCRIPTION      , "Desc"          , "Description"                , new DynaValue(CF_SCHEMA_DESC)                          , DL_ADVANCED)},
			{CK0_VERSION    ,      new ShScFieldDefMeta<SchemaRowKey> (CK0_VERSION          , "Version"       , "Row Schema Version"        , new DynaValue("1.0")                                   , DL_MEDIUM)},
			{CK0_MODEL_PATH ,      new ShScFieldDefMeta<SchemaRowKey> (CK0_MODEL_PATH       , "ModelPath"     , "Path of Model"              , new DynaValue(K_NOT_DEFINED)                           , DL_MEDIUM)},
			{CK0_MODEL_NAME ,      new ShScFieldDefMeta<SchemaRowKey> (CK0_MODEL_NAME       , "ModelName"     , "Name of Model"              , new DynaValue(K_NOT_DEFINED)                           , DL_MEDIUM)},
	
	
			// these are pre-set by the creation of the object & when the object is modified
			{CK2_USER_NAME  ,      new ShScFieldDefMeta<SchemaRowKey> (CK2_USER_NAME        , "UserName"      , "User Name of Row Creator"  , new DynaValue(UtilityLibrary.CsUtilities.UserName)     , DL_ADVANCED)},
			{CK2_MODIFY_DATE,      new ShScFieldDefMeta<SchemaRowKey> (CK2_MODIFY_DATE      , "ModifyDate"    , "Date Modified (or Created)" , new DynaValue(DateTime.UtcNow.ToString())              , DL_MEDIUM)},
	
	
			// these are variable depending on the database
			// value may not apply
			{CK9_SEQUENCE         ,new ShScFieldDefMeta<SchemaRowKey> (CK9_SEQUENCE         , "Sequence"      , "Evaluation Sequence"        , new DynaValue(K_NOT_DEFINED)                           , DL_BASIC)},
			{CK9_UPDATE_RULE      ,new ShScFieldDefMeta<SchemaRowKey> (CK9_UPDATE_RULE      , "UpdateRule"    , "Update Rule"                , new DynaValue(UR_UNDEFINED)                            , DL_BASIC)},
			{CK9_ROW_FAMILY_NAME  ,new ShScFieldDefMeta<SchemaRowKey> (CK9_ROW_FAMILY_NAME  , "RowFamilyName" , "Name of the Rows Family"   , new DynaValue(K_NOT_DEFINED)                           , DL_BASIC)},
			{CK9_SKIP             ,new ShScFieldDefMeta<SchemaRowKey> (CK9_SKIP             , "Skip"          , "Skip Updating"              , new DynaValue(false)                                   , DL_BASIC)},
			{CK9_XL_FILE_PATH     ,new ShScFieldDefMeta<SchemaRowKey> (CK9_XL_FILE_PATH     , "XlFilePath"    , "File Path to the Excel File", new DynaValue(K_NOT_DEFINED)                           , DL_BASIC)},
			{CK9_XL_WORKSHEET_NAME,new ShScFieldDefMeta<SchemaRowKey> (CK9_XL_WORKSHEET_NAME, "Xlworksheet"   , "Name of the Excel Worksheet", new DynaValue(K_NOT_DEFINED)                           , DL_BASIC)},
			{CK9_GUID             ,new ShScFieldDefMeta<SchemaRowKey> (CK9_GUID             , "GUID"          , "Row GUID"                  , new DynaValue(K_NOT_DEFINED)                           , DL_DEBUG)},
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