#region + Using Directives

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using static ShExStorageN.ShSchemaFields.ShScSupport.SchemaFieldDisplayLevel;
using static ShExStorageN.ShSchemaFields.ShScSupport.CellUpdateRules;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExConst;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaSheetKey;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaRowKey;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaLockKey;
using Autodesk.Revit.DB;
using ShExStorageN.ShSchemaFields;
using System.Windows.Input;

// using ShExStorageC.ShExStorage;
using ShExStorageC.ShSchemaFields;
using ShExStorageN.ShExStorage;

#endregion

// user name: jeffs
// created:   10/24/2022 7:20:59 PM

namespace ShExStorageC.ShSchemaFields.ShScSupport
{
	public static class ScInfoMeta1
	{
		static ScInfoMeta1() { }

		public const string KEY_FIELD_NAME = "Key";

		public const string MODEL_PATH = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\RevitProjects\ExStorage\.RevitFiles";
		public const string MODEL_NAME = @"HasDataStorage.rvt";
		public const string SF_SCHEMA_DESC = "Cells Sheet DataStorage";
		public const string CF_SCHEMA_DESC = "Cells Row DS";
		public const string LF_SCHEMA_DESC = "Cells lock DS";

		
		/// <summary>
		/// make the initial generic data set of sheet data
		/// </summary>
		public static ScDataSheet1 MakeInitialDataSheet1(ExId exid)
		{
			ScDataSheet1 shtd = new ScDataSheet1();

			// shtd.Fields[TK0_SCHEMA_NAME].SetValue = e.ExsIdSheet;
			shtd.Fields[SK0_SCHEMA_NAME].SetValue = exid.ExsIdSheetSchemaName;

			shtd.Fields[SK2_MODEL_NAME].SetValue = exid.Document.Title;
			shtd.Fields[SK2_MODEL_PATH].SetValue = exid.Document.PathName;

			shtd.Fields[SK0_GUID].SetValue = Guid.NewGuid();

			return shtd;
		}

		
		/// <summary>
		/// make the generic data set of row data
		/// </summary>
		public static ScDataRow1 MakeInitialDataRow1(ScDataSheet1 shtd)
		{
			ScDataRow1 rowd = new ScDataRow1();

			// dynamic a = shtd.Fields[TK0_MODEL_NAME].DyValue;
			// ScFieldDefData1<SchemaSheetKey> b = shtd.Fields[TK0_MODEL_NAME];
			// Dictionary<SchemaSheetKey, ScFieldDefData1<SchemaSheetKey>> c = shtd.Fields;

			rowd.Fields[RK0_SCHEMA_NAME].SetValue = K_NOT_DEFINED_STR;
			rowd.Fields[RK2_MODEL_NAME].SetValue = shtd.Fields[SK2_MODEL_NAME].GetValueAs<string>();
			rowd.Fields[RK2_MODEL_PATH].SetValue = shtd.Fields[SK2_MODEL_PATH].GetValueAs<string>(); ;

			rowd.Fields[RK0_GUID].SetValue = Guid.NewGuid();

			rowd.Fields[RK2_CELL_FAMILY_NAME].SetValue = K_NOT_DEFINED;
			rowd.Fields[RK2_SEQUENCE].SetValue = K_NOT_DEFINED;
			rowd.Fields[RK2_SKIP].SetValue = false;
			rowd.Fields[RK2_UPDATE_RULE].SetValue = UR_UNDEFINED;
			rowd.Fields[RK2_XL_FILE_PATH].SetValue = K_NOT_DEFINED;
			rowd.Fields[RK2_XL_WORKSHEET_NAME].SetValue = K_NOT_DEFINED;

			return rowd;
		}


		/// <summary>
		/// add all of the meta fields to a data field using default information
		/// </summary>
		public static void 
			ConfigData1<TKey>(Dictionary<TKey, ScFieldDefData1<TKey>> fields,
		Dictionary<TKey, ScFieldDefMeta1<TKey>> meta)
			where TKey : Enum
		{
			foreach (KeyValuePair<TKey,  ScFieldDefMeta1<TKey>> kvp in meta)
			{
				fields.Add(kvp.Key,
					new ScFieldDefData1<TKey>(kvp.Value.FieldKey, new DynaValue(kvp.Value.DyValue.Value), kvp.Value));
			}
		}

		public static Dictionary<SchemaSheetKey, ScFieldDefMeta1<SchemaSheetKey>> FieldsSheet => fieldsSheet;
		public static Dictionary<SchemaRowKey, ScFieldDefMeta1<SchemaRowKey>> FieldsRow => fieldsRow;

		private static Dictionary<SchemaSheetKey, ScFieldDefMeta1<SchemaSheetKey>> fieldsSheet =
			new Dictionary<SchemaSheetKey, ScFieldDefMeta1<SchemaSheetKey>>
			{
			// @formatter:off

			// these are pre-set by the creation of the object
			{SK0_KEY,              new ScFieldDefMeta1<SchemaSheetKey> (SK0_KEY          , KEY_FIELD_NAME, "Access Key"                  ,new DynaValue(SK0_KEY)                                      , DL_ADVANCED)},
			{SK0_SCHEMA_NAME,      new ScFieldDefMeta1<SchemaSheetKey> (SK0_SCHEMA_NAME  , "Name"       , "Schema Name"                  ,new DynaValue(K_NOT_DEFINED_STR)                            , DL_ADVANCED)},
			{SK0_DESCRIPTION,      new ScFieldDefMeta1<SchemaSheetKey> (SK0_DESCRIPTION  , "Desc"       , "Description"                  ,new DynaValue(SF_SCHEMA_DESC)                               , DL_ADVANCED)},
			{SK0_VERSION,          new ScFieldDefMeta1<SchemaSheetKey> (SK0_VERSION      , "Version"    , "Sheet Schema Version"         ,new DynaValue("1.0")                                        , DL_MEDIUM)},

			{SK2_MODEL_PATH,       new ScFieldDefMeta1<SchemaSheetKey> (SK2_MODEL_PATH   , "ModelPath"  , "Path of Model"                ,new DynaValue(K_NOT_DEFINED_STR)                            , DL_MEDIUM)},
			{SK2_MODEL_NAME,       new ScFieldDefMeta1<SchemaSheetKey> (SK2_MODEL_NAME   , "ModelName"  , "Name of Model"                ,new DynaValue(K_NOT_DEFINED_STR)                            , DL_MEDIUM)},
			{SK2_DEVELOPER,        new ScFieldDefMeta1<SchemaSheetKey> (SK2_DEVELOPER    , "Developer"  , "Name of Developer"            ,new DynaValue(UtilityLibrary.CsUtilities.CompanyName)       , DL_MEDIUM)},

			// these are pre-set by the creation of the object & when the object is modified
			{SK0_USER_NAME,        new ScFieldDefMeta1<SchemaSheetKey> (SK0_USER_NAME    , "UserName"   , "User Name of Sheet Modifier"  ,new DynaValue(UtilityLibrary.CsUtilities.UserName)          , DL_ADVANCED)},
			{SK1_MODIFY_DATE,      new ScFieldDefMeta1<SchemaSheetKey> (SK1_MODIFY_DATE  , "ModifyDate" , "Date Modified (or Created)"   ,new DynaValue(DateTime.UtcNow.ToString())                   , DL_MEDIUM)},

			// these are variable depending on the database
			{SK0_GUID,             new ScFieldDefMeta1<SchemaSheetKey> (SK0_GUID         , "GUID"       , "Sheet GUID"                   ,new DynaValue(Guid.Empty)                                   , DL_MEDIUM)}
			
				// @formatter:on
			};


		private static Dictionary<SchemaRowKey, ScFieldDefMeta1<SchemaRowKey>> fieldsRow =
			new Dictionary<SchemaRowKey, ScFieldDefMeta1<SchemaRowKey>>
			{
			// @formatter:off

			// these are pre-set by the creation of the object
			{RK0_KEY        ,      new ScFieldDefMeta1<SchemaRowKey> (RK0_KEY              , KEY_FIELD_NAME  , "Access Key"                 , new DynaValue(RK0_KEY)                                 , DL_ADVANCED)},
			{RK0_SCHEMA_NAME,      new ScFieldDefMeta1<SchemaRowKey> (RK0_SCHEMA_NAME      , "Name"          , "Schema Name"                , new DynaValue(K_NOT_DEFINED_STR)                       , DL_ADVANCED)},
			{RK0_DESCRIPTION,      new ScFieldDefMeta1<SchemaRowKey> (RK0_DESCRIPTION      , "Desc"          , "Description"                , new DynaValue(CF_SCHEMA_DESC)                          , DL_ADVANCED)},
			{RK0_VERSION    ,      new ScFieldDefMeta1<SchemaRowKey> (RK0_VERSION          , "Version"       , "Row Schema Version"        , new DynaValue("1.0")                                   , DL_MEDIUM)},
			{RK2_MODEL_PATH ,      new ScFieldDefMeta1<SchemaRowKey> (RK2_MODEL_PATH       , "ModelPath"     , "Path of Model"              , new DynaValue(K_NOT_DEFINED)                           , DL_MEDIUM)},
			{RK2_MODEL_NAME ,      new ScFieldDefMeta1<SchemaRowKey> (RK2_MODEL_NAME       , "ModelName"     , "Name of Model"              , new DynaValue(K_NOT_DEFINED)                           , DL_MEDIUM)},
	
	
			// these are pre-set by the creation of the object & when the object is modified
			{RK0_USER_NAME  ,      new ScFieldDefMeta1<SchemaRowKey> (RK0_USER_NAME        , "UserName"      , "User Name of Row Creator"  , new DynaValue(UtilityLibrary.CsUtilities.UserName)     , DL_ADVANCED)},
			{RK1_MODIFY_DATE,      new ScFieldDefMeta1<SchemaRowKey> (RK1_MODIFY_DATE      , "ModifyDate"    , "Date Modified (or Created)" , new DynaValue(DateTime.UtcNow.ToString())              , DL_MEDIUM)},
	
	
			// these are variable depending on the database
			// value may not apply
			{RK2_SEQUENCE         ,new ScFieldDefMeta1<SchemaRowKey> (RK2_SEQUENCE         , "Sequence"      , "Evaluation Sequence"        , new DynaValue(K_NOT_DEFINED)                           , DL_BASIC)},
			{RK2_UPDATE_RULE      ,new ScFieldDefMeta1<SchemaRowKey> (RK2_UPDATE_RULE      , "UpdateRule"    , "Update Rule"                , new DynaValue(UR_UNDEFINED)                            , DL_BASIC)},
			{RK2_CELL_FAMILY_NAME ,new ScFieldDefMeta1<SchemaRowKey> (RK2_CELL_FAMILY_NAME , "CellFamilyName", "Name of the Cell Family"    , new DynaValue(K_NOT_DEFINED_STR)                       , DL_BASIC)},
			{RK2_SKIP             ,new ScFieldDefMeta1<SchemaRowKey> (RK2_SKIP             , "Skip"          , "Skip Updating"              , new DynaValue(false)                                   , DL_BASIC)},
			{RK2_XL_FILE_PATH     ,new ScFieldDefMeta1<SchemaRowKey> (RK2_XL_FILE_PATH     , "XlFilePath"    , "File Path to the Excel File", new DynaValue(K_NOT_DEFINED)                           , DL_BASIC)},
			{RK2_XL_WORKSHEET_NAME,new ScFieldDefMeta1<SchemaRowKey> (RK2_XL_WORKSHEET_NAME, "Xlworksheet"   , "Name of the Excel Worksheet", new DynaValue(K_NOT_DEFINED)                           , DL_BASIC)},
			{RK0_GUID             ,new ScFieldDefMeta1<SchemaRowKey> (RK0_GUID             , "GUID"          , "Row GUID"                  , new DynaValue(Guid.Empty)								 , DL_DEBUG)},
			// Add(/*SO_ALL  ,*/   new ScFieldDef<SchemaRowKey> (RK_VALUE            , "Value"         , "Value"                      , new DynaValue(0.0)                                            , DL_ADVANCED)},

				// @formatter:on
			};

	}
}