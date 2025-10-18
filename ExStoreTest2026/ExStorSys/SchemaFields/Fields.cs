
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using static ExStorSys.WorkBookFieldKeys;
using static ExStorSys.SheetFieldKeys;
using static ExStorSys.FieldEditLevel;
using static ExStorSys.FieldUsage;
using static ExStorSys.UpdateRules;
using static ExStorSys.ExStorConst;


// user name: jeffs
// created:   9/16/2025 11:06:53 PM

namespace ExStorSys
{
	/// <summary>
	/// The definition of each schema field for both primary and row schemas
	/// </summary>
	public class Fields
	{
		static Fields()
		{
			WBK_FIELDS_COUNT = WorkBookFields.Count;
			SHT_FIELDS_COUNT = SheetFields.Count;
		}

		public const string KEY_DS_NAME = "DSName";
		public const string KEY_WBK_SCHEMA_NAME = "WbkSchemaName";
		public const string KEY_SHT_SCHEMA_NAME = "ShtSchemaName";

		public static int WBK_FIELDS_COUNT { get; }
		public static int SHT_FIELDS_COUNT { get; }

		public static FieldData<WorkBookFieldKeys> GetWbkFieldData(WorkBookFieldKeys key)
		{
			FieldData<WorkBookFieldKeys> fd = new (WorkBookFields[key], WorkBookFields[key].FieldDefValue);
			return fd;
		}

		public static FieldData<SheetFieldKeys> GetShtFieldData(SheetFieldKeys key)
		{
			FieldData<SheetFieldKeys> fd = new (SheetFields[key], SheetFields[key].FieldDefValue);
			return fd;
		}

		// @formatter:off

		/// <summary>
		/// definition of each primary Sheet schema field
		/// this is stored in the data storage object named "workbook"
		/// </summary>
		public static Dictionary<WorkBookFieldKeys, FieldDef<WorkBookFieldKeys>> WorkBookFields {get;} = new ()
		{
			// field usage flag is not used at this time
			{PK_DS_NAME,              new (PK_DS_NAME,              KEY_DS_NAME,        KEY_DS_NAME,                      new DynaValue(KEY_DS_NAME)                 , FU_S_AND_DS, DL_BASIC) }   , // 1
			{PK_AD_DESC,              new (PK_AD_DESC,              "Desc",             "Description",                    new DynaValue(PRIMARY_SCHEMA_DESC)         , FU_S_AND_DS, DL_BASIC) }   , // 2
																																												 //
			{PK_AD_VENDORID,          new (PK_AD_VENDORID,          "VendorId",         "Vendor Id",                      new DynaValue(ExStorConst.VendorId)        , FU_S_AND_DS, DL_DEBUG) }   , // 
			// {PK_AD_ADDINID,           new (PK_AD_ADDINID,           "AddInId",          "AddIn Id",                       new DynaValue(K_NOT_DEFINED_STR)           , FU_S_AND_DS, DL_DEBUG) }   , // 

			// don't need - when a ds is deleted, it is gone - when a schema is deleted, I can only change its stored name
			// {PK_AD_DELETED,           new (PK_AD_DELETED,           "Deleted",          "Flagged as to be Deleted",       new DynaValue(false)                       , FU_S_AND_DS, DL_DEBUG) }   , // 
																																												 //
			{PK_AD_DATE_CREATED,      new (PK_AD_DATE_CREATED,      "CreateDate",       "Date Created",                   new DynaValue(DateTime.Now.ToString("s"))  , FU_S_AND_DS, DL_MEDIUM) }  , // 
			{PK_AD_NAME_CREATED,      new (PK_AD_NAME_CREATED,      "CreateName",       "Creator's Name",                 new DynaValue(K_NOT_DEFINED_STR)           , FU_S_AND_DS, DL_ADVANCED) }, // 
			{PK_AD_DATE_MODIFIED,     new (PK_AD_DATE_MODIFIED,     "ModifyDate",       "Date Modified",                  new DynaValue(K_NOT_DEFINED_STR)           , FU_S_AND_DS, DL_MEDIUM) }  , // 
			{PK_AD_NAME_MODIFIED,     new (PK_AD_NAME_MODIFIED,     "ModifyName",       "Modifier's Name",                new DynaValue(K_NOT_DEFINED_STR)           , FU_S_AND_DS, DL_ADVANCED) }, // 
			{PK_AD_MODEL_CODE,        new (PK_AD_MODEL_CODE,        "ModelCode",        "Model's Identification Code",    new DynaValue(K_NOT_DEFINED_STR)           , FU_S_AND_DS, DL_DEBUG) }   , // 
			{PK_AD_LAST_ID,           new (PK_AD_LAST_ID,           "LastId",           "Last DS Identification Code",    new DynaValue("9999")                      , FU_S_AND_DS, DL_DEBUG) }   , // 
																																												 // 
			{PK_SD_SCHEMA_VERSION,    new (PK_SD_SCHEMA_VERSION,    "SchemaVersion",    "Schema Version",                 new DynaValue("1.0")                       , FU_S_AND_DS, DL_ADVANCED) }, // 
			// voided fields
			// {PK_SD_WBK_SCHEMA_NAME,   new (PK_SD_WBK_SCHEMA_NAME,    KEY_WBK_SCHEMA_NAME, KEY_WBK_SCHEMA_NAME,            new DynaValue(KEY_WBK_SCHEMA_NAME)         , FU_S_AND_DS, DL_ADVANCED) }, // 
			// {PK_SD_SHT_SCHEMA_NAME,   new (PK_SD_SHT_SCHEMA_NAME,    KEY_SHT_SCHEMA_NAME, KEY_SHT_SCHEMA_NAME,            new DynaValue(KEY_SHT_SCHEMA_NAME)         , FU_S_AND_DS, DL_ADVANCED) }, // 
			// {PK_SD_SCHEMA_GUID,       new (PK_SD_SCHEMA_GUID,       "SchemaGuid",       "Schema GUID",                    new DynaValue(Guid.NewGuid())              , FU_S_AND_DS, DL_ADVANCED) }, // 
																																												 // 
			{PK_MD_MODEL_NAME,        new (PK_MD_MODEL_NAME,        "ModelName",        "Model Name",                     new DynaValue(K_NOT_DEFINED_STR)           , FU_S_AND_DS, DL_BASIC) }   , // 


		};

		/// <summary>
		/// definition of each row schema field
		/// </summary>
		public static Dictionary<SheetFieldKeys, FieldDef<SheetFieldKeys>> SheetFields {get; } = new ()
		{
			// field usage flag is not used at this time
			{RK_DS_NAME,              new (RK_DS_NAME,              KEY_DS_NAME,        KEY_DS_NAME,                      new DynaValue(KEY_DS_NAME)                 , FU_S_AND_DS, DL_DEBUG) }   ,
			{RK_AD_DESC,              new (RK_AD_DESC,              "Desc",             "Description",                    new DynaValue(PRIMARY_SCHEMA_DESC)         , FU_S_AND_DS, DL_BASIC) }   ,
																																								     
			{RK_AD_VENDORID,          new (RK_AD_VENDORID,          "VendorId",         "Vendor Id",                      new DynaValue(ExStorConst.VendorId)        , FU_S_AND_DS, DL_DEBUG) }   ,
			// {RK_AD_ADDINID,           new (RK_AD_ADDINID,           "AddInId",          "AddIn Id",                       new DynaValue(K_NOT_DEFINED_STR)           , FU_S_AND_DS, DL_DEBUG) }   , // 

			// don't need - when a ds is deleted, it is gone - when a schema is deleted, I can only change its stored name
			// {RK_AD_DELETED,           new (RK_AD_DELETED,           "Deleted",          "Flagged as to be Deleted",       new DynaValue(false)                       , FU_S_AND_DS, DL_DEBUG) }   , // 
																																								     
			{RK_AD_DATE_CREATED,      new (RK_AD_DATE_CREATED,      "CreateDate",       "Date Created",                   new DynaValue(K_NOT_DEFINED_STR)           , FU_S_AND_DS, DL_MEDIUM) }  ,
			{RK_AD_NAME_CREATED,      new (RK_AD_NAME_CREATED,      "CreateName",       "Creator's Name",                 new DynaValue(K_NOT_DEFINED_STR)           , FU_S_AND_DS, DL_ADVANCED) },
			{RK_AD_DATE_MODIFIED,     new (RK_AD_DATE_MODIFIED,     "ModifyDate",       "Date Modified",                  new DynaValue(K_NOT_DEFINED_STR)           , FU_S_AND_DS, DL_MEDIUM) }  ,
			{RK_AD_NAME_MODIFIED,     new (RK_AD_NAME_MODIFIED,     "ModifyName",       "Modifier's Name",                new DynaValue(K_NOT_DEFINED_STR)           , FU_S_AND_DS, DL_ADVANCED) },
																																								     
			{RK_SD_SCHEMA_VERSION,    new (RK_SD_SCHEMA_VERSION,    "SchemaVersion",    "Schema Version",                 new DynaValue("1.0")                       , FU_S_AND_DS, DL_ADVANCED) },
			// {RK_SD_SCHEMA_GUID,       new (RK_SD_SCHEMA_GUID,       "SchemaGuid",       "Schema GUID",                    new DynaValue(Guid.NewGuid())              , FU_S_AND_DS, DL_ADVANCED) },
																																								     
			{RK_ED_XL_FILE_PATH,      new (RK_ED_XL_FILE_PATH,      "XlFileName",       "Excel File Name",                new DynaValue(K_NOT_DEFINED_STR)           , FU_S_AND_DS, DL_MEDIUM) }  ,
			{RK_ED_XL_SHEET_NAME,     new (RK_ED_XL_SHEET_NAME,     "XlSheetName",      "Excel Sheet Name",               new DynaValue(K_NOT_DEFINED_STR)           , FU_S_AND_DS, DL_MEDIUM) }  ,
																																								     
			{RK_OD_STATUS,            new (RK_OD_STATUS,            "Status",           "Operation Status",               new DynaValue(SheetOpStatus.SS_GOOD)       , FU_S_AND_DS, DL_ADVANCED) },
			{RK_OD_SEQUENCE,          new (RK_OD_SEQUENCE,          "Sequence",         "Operation Sequence",             new DynaValue("A00")                       , FU_S_AND_DS, DL_ADVANCED) },
			{RK_OD_UPDATE_RULE,       new (RK_OD_UPDATE_RULE,       "UpdateRule",       "Update Rule",                    new DynaValue(UR_UNDEFINED)                , FU_S_AND_DS, DL_ADVANCED) },
			{RK_OD_UPDATE_SKIP,       new (RK_OD_UPDATE_SKIP,       "UpdateSkip",       "Update Bypass",                  new DynaValue(false)                       , FU_S_AND_DS, DL_MEDIUM) }  ,
																																								     
			{RK_RD_FAMILY_LIST,       new (RK_RD_FAMILY_LIST,        "Rows",             "List of Families (& types)",     new DynaValue(K_ARRAY)                    , FU_S_AND_DS, DL_MEDIUM) }  ,
																																								     
		};

		// @formatter:on
	}
}

