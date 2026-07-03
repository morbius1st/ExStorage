using static ExStorSys.WorkBookFieldKeys;
using static ExStorSys.SheetFieldKeys;
using static ExStorSys.FieldEditLevel;
using static ExStorSys.ItemUsage;
using static ExStorSys.UpdateRules;
using static ExStorSys.ActivateStatus;
using static ExStorSys.ExStorConst;
using static ExStorSys.FieldCopyType;
using static UtilityLibrary.ChgSrcId;

using UtilityLibrary;

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
		{ }

		private const bool T = true;
		private const bool F = false;

		public const string KEY_DS_NAME = "DSName";
		public const string KEY_DS_DESC = "Data Storage Name";
		public const string KEY_WBK_SCHEMA_NAME = "WbkSchemaName";
		public const string KEY_SHT_SCHEMA_NAME = "ShtSchemaName";

		public static FieldData<WorkBookFieldKeys> GetWbkFieldData(WorkBookFieldKeys key)
		{
			FieldData<WorkBookFieldKeys> fd = new (WorkBookFields[key], WorkBookFields[key].FieldDefValue);
			return fd;
		}

		public static FieldDef<WorkBookFieldKeys> GetWbkFieldDef(WorkBookFieldKeys key)
		{
			return GetWbkFieldData(key).Field;
		}

		public static FieldData<SheetFieldKeys> GetShtFieldData(SheetFieldKeys key)
		{
			FieldData<SheetFieldKeys> fd = new (SheetFields[key], SheetFields[key].FieldDefValue);
			return fd;
		}

		public static FieldDef<SheetFieldKeys> GetShtFieldDef(SheetFieldKeys key)
		{
			return GetShtFieldData(key).Field;
		}

		// @formatter:off

		/// <summary>
		/// definition of each primary Sheet schema field
		/// this is stored in the data storage object named "workbook"
		/// </summary>
		public static Dictionary<WorkBookFieldKeys, FieldDef<WorkBookFieldKeys>> WorkBookFields {get;} = new ()
		{
			// field usage flag is not used at this time
			{PK_DS_NAME,             new (PK_DS_NAME,              KEY_DS_NAME,        KEY_DS_DESC                   , null                           , new DynaValue(KEY_DS_NAME)                , IU_SND_BY_USER    , FEL_VIEW_ONLY		 , FC_ALWAYS  , [CI_NOP  , CI_NOP  ])} ,
			{PK_AD_DESC,             new (PK_AD_DESC,              "Desc",             "WorkBook Description"        , nameof(WorkBook.Desc)          , new DynaValue(PRIMARY_SCHEMA_DESC)        , IU_SND_BY_USER    , FEL_BAS_VIEW_ONLY	 , FC_NEVER   , [CI_SRC_A, CI_SRC_A])} ,
			{PK_AD_STATUS,           new (PK_AD_STATUS,            "Status",           "Activate Status"             , nameof(WorkBook.Status)        , new DynaValue(AS_INACTIVE)                , IU_SND_BY_USER    , FEL_BAS_VIEW_ONLY	 , FC_NEVER   , [CI_SRC_A, CI_SRC_A])} ,
			{PK_MD_MODEL_TITLE,      new (PK_MD_MODEL_TITLE,       "ModelName",        "Model Name"                  , nameof(WorkBook.ModelTitle)    , new DynaValue(K_NOT_DEFINED_STR)          , IU_SND_BY_USER    , FEL_VIEW_ONLY		 , FC_TYPE_4  , [CI_NOP  , CI_NOP  ])} ,
			{PK_AD_VENDORID,         new (PK_AD_VENDORID,          "VendorId",         "Vendor Id"                   , nameof(WorkBook.VendorId)      , new DynaValue(ExStorConst.VendorId)       , IU_SND_ALT_SRC_B  , FEL_DEBUG			 , FC_NEVER   , [CI_SRC_A, CI_SRC_A])} ,
			{PK_AD_LAST_ID,          new (PK_AD_LAST_ID,           "LastId",           "Last DS Identification Code" , nameof(WorkBook.LastId)        , new DynaValue("9999")                     , IU_SND_ALT_SRC_A  , FEL_DEBUG			 , FC_TYPE_4  , [CI_SRC_A, CI_SRC_E])} ,
			{PK_AD_NAME_CREATED,     new (PK_AD_NAME_CREATED,      "CreateName",       "Creator's Name"              , nameof(WorkBook.NameCreated)   , new DynaValue(K_NOT_DEFINED_STR)          , IU_SND_ALT_SRC_B  , FEL_ADV_VIEW_ONLY	 , FC_TYPE_14 , [CI_SRC_X, CI_SRC_T])} ,
			{PK_AD_DATE_CREATED,     new (PK_AD_DATE_CREATED,      "CreateDate",       "Date Created"                , nameof(WorkBook.DateCreated)   , new DynaValue(DateTime.Now.ToString("s")) , IU_SND_BY_USER    , FEL_VIEW_ONLY		 , FC_TYPE_14 , [CI_NOP  , CI_NOP  ])} ,
			{PK_AD_NAME_MODIFIED,    new (PK_AD_NAME_MODIFIED,     "ModifyName",       "Modifier's Name"             , nameof(WorkBook.NameModified)  , new DynaValue(K_NOT_DEFINED_STR)          , IU_SND_ALT_SRC_B  , FEL_ADV_VIEW_ONLY	 , FC_ALWAYS  , [CI_SRC_X, CI_SRC_T])} ,
			{PK_AD_DATE_MODIFIED,    new (PK_AD_DATE_MODIFIED,     "ModifyDate",       "Date Modified"               , nameof(WorkBook.DateModified)  , new DynaValue(K_NOT_DEFINED_STR)          , IU_SND_CTRL_FLD   , FEL_VIEW_ONLY		 , FC_ALWAYS  , [CI_NOP  , CI_SRC_T])} ,
			{PK_SD_SCHEMA_VERSION,   new (PK_SD_SCHEMA_VERSION,    "SchemaVersion",    "Schema Version"              , nameof(WorkBook.SchemaVersion) , new DynaValue("1.0")                      , IU_SND_BY_USER    , FEL_LOCKED           , FC_IGNORE  , [CI_SRC_A, CI_SRC_A])} ,

			// control fields - not saved to the DS																																																		    
			{PK_CD_SHEETS_LIST,      new (PK_CD_SHEETS_LIST,       "SheetsList",       "Sheets List Control"         , nameof(WorkBook.SheetsList)      , new DynaValue(0)                        , IU_CTRL_ONLY      , FEL_DEBUG           , FC_IGNORE   , [CI_NONE , CI_SRC_E])} ,
																																													   
			// full model name in case needs to be shown to the user / used to confirm working wiht the correct data																  				                                    
			// voided fields																																						   
			// {PK_AD_MODEL_CODE,        new (PK_AD_MODEL_CODE,        "ModelCode",        "Model's Identification Code",    new DynaValue(K_NOT_DEFINED_STR)           , IU_S_AND_DS  , FEL_DEBUG) }   ,
			// {PK_AD_ADDINID,           new (PK_AD_ADDINID,           "AddInId",          "AddIn Id",                       new DynaValue(K_NOT_DEFINED_STR)           , IU_S_AND_DS  , FEL_DEBUG) }   ,
			// don't need - when a ds is deleted, it is gone - when a schema is deleted, I can only change its stored name															   
			// {PK_AD_DELETED,           new (PK_AD_DELETED,           "Deleted",          "Flagged as to be Deleted",       new DynaValue(false)                       , IU_S_AND_DS  , FEL_DEBUG) }   ,
			// {PK_SD_WBK_SCHEMA_NAME,   new (PK_SD_WBK_SCHEMA_NAME,    KEY_WBK_SCHEMA_NAME, KEY_WBK_SCHEMA_NAME,            new DynaValue(KEY_WBK_SCHEMA_NAME)         , IU_S_AND_DS  , FEL_ADVANCED) },
			// {PK_SD_SHT_SCHEMA_NAME,   new (PK_SD_SHT_SCHEMA_NAME,    KEY_SHT_SCHEMA_NAME, KEY_SHT_SCHEMA_NAME,            new DynaValue(KEY_SHT_SCHEMA_NAME)         , IU_S_AND_DS  , FEL_ADVANCED) },
			// {PK_SD_SCHEMA_GUID,       new (PK_SD_SCHEMA_GUID,       "SchemaGuid",       "Schema GUID",                    new DynaValue(Guid.NewGuid())              , IU_S_AND_DS  , FEL_ADVANCED) },
		};

		/// <summary>
		/// definition of each row schema field
		/// </summary>
		public static Dictionary<SheetFieldKeys, FieldDef<SheetFieldKeys>> SheetFields {get; } = new ()
		{
			// field usage flag is not used at this time
			{RK_DS_NAME,              new (RK_DS_NAME,              KEY_DS_NAME,        KEY_DS_DESC                   , nameof(Sheet.DsName)         , new DynaValue(KEY_DS_NAME)             , IU_SND_BY_USER   , FEL_VIEW_ONLY       , FC_ALWAYS  , [CI_NOP   , CI_NOP   ])} ,
			{RK_AD_DESC,              new (RK_AD_DESC,              "Desc",             "Sheet Description"           , nameof(Sheet.Desc)           , new DynaValue(PRIMARY_SCHEMA_DESC)     , IU_SND_BY_USER   , FEL_BAS_VIEW_ONLY   , FC_NEVER   , [CI_SRC_A , CI_SRC_A ])} ,
 			{RK_AD_VENDORID,          new (RK_AD_VENDORID,          "VendorId",         "Vendor Id"                   , nameof(Sheet.VendorId)       , new DynaValue(ExStorConst.VendorId)    , IU_SND_ALT_SRC_B , FEL_DEBUG           , FC_NEVER   , [CI_SRC_A , CI_SRC_A ])} ,
			{RK_AD_NAME_CREATED,      new (RK_AD_NAME_CREATED,      "CreateName",       "Creator's Name"              , nameof(Sheet.NameCreated)    , new DynaValue(K_NOT_DEFINED_STR)       , IU_SND_ALT_SRC_B , FEL_ADV_VIEW_ONLY   , FC_TYPE_14 , [CI_SRC_X , CI_SRC_T ])} ,
 			{RK_AD_DATE_CREATED,      new (RK_AD_DATE_CREATED,      "CreateDate",       "Date Created"                , nameof(Sheet.DateCreated)    , new DynaValue(K_NOT_DEFINED_STR)       , IU_SND_BY_USER   , FEL_VIEW_ONLY       , FC_TYPE_14 , [CI_NOP   , CI_NOP   ])} ,
			{RK_AD_NAME_MODIFIED,     new (RK_AD_NAME_MODIFIED,     "ModifyName",       "Modifier's Name"             , nameof(Sheet.NameModified)   , new DynaValue(K_NOT_DEFINED_STR)       , IU_SND_ALT_SRC_B , FEL_ADV_VIEW_ONLY   , FC_ALWAYS  , [CI_SRC_X , CI_SRC_T ])} ,
			{RK_AD_DATE_MODIFIED,     new (RK_AD_DATE_MODIFIED,     "ModifyDate",       "Date Modified"               , nameof(Sheet.DateModified)   , new DynaValue(K_NOT_DEFINED_STR)       , IU_SND_CTRL_FLD  , FEL_VIEW_ONLY       , FC_ALWAYS  , [CI_NOP   , CI_SRC_T ])} ,
 			{RK_SD_SCHEMA_VERSION,    new (RK_SD_SCHEMA_VERSION,    "SchemaVersion",    "Schema Version"              , nameof(Sheet.SchemaVersion)  , new DynaValue("1.0")                   , IU_SND_BY_USER   , FEL_LOCKED          , FC_IGNORE  , [CI_SRC_A , CI_SRC_A ])} ,
 			{RK_ED_XL_FILE_PATH,      new (RK_ED_XL_FILE_PATH,      "XlFileName",       "Excel File Name"             , nameof(Sheet.XlFilePath)     , new DynaValue(K_NOT_DEFINED_STR)       , IU_SND_BY_USER   , FEL_ADVANCED        , FC_NEVER   , [CI_SRC_A , CI_SRC_A ])} ,
			{RK_ED_XL_SHEET_NAME,     new (RK_ED_XL_SHEET_NAME,     "XlSheetName",      "Excel Sheet Name"            , nameof(Sheet.XlSheetName)    , new DynaValue(K_NOT_DEFINED_STR)       , IU_SND_BY_USER   , FEL_ADVANCED        , FC_NEVER   , [CI_SRC_A , CI_SRC_A ])} ,
 			{RK_OD_STATUS,            new (RK_OD_STATUS,            "Status",           "Operation Status"            , nameof(Sheet.OpStatus)       , new DynaValue(SheetOpStatus.SOS_GOOD)  , IU_SND_BY_USER   , FEL_BAS_VIEW_ONLY   , FC_NEVER   , [CI_SRC_A , CI_SRC_A ])} ,
			{RK_OD_SEQUENCE,          new (RK_OD_SEQUENCE,          "Sequence",         "Operation Sequence"          , nameof(Sheet.OpSequence)     , new DynaValue("A00")                   , IU_SND_BY_USER   , FEL_BAS_VIEW_ONLY   , FC_NEVER   , [CI_SRC_A , CI_SRC_A ])} ,
			{RK_OD_UPDATE_RULE,       new (RK_OD_UPDATE_RULE,       "UpdateRule",       "Update Rule"                 , nameof(Sheet.UpdateRule)     , new DynaValue(UR_UNDEFINED)            , IU_SND_BY_USER   , FEL_BAS_VIEW_ONLY   , FC_NEVER   , [CI_SRC_A , CI_SRC_A ])} ,
			{RK_OD_UPDATE_SKIP,       new (RK_OD_UPDATE_SKIP,       "UpdateSkip",       "Update Bypass"               , nameof(Sheet.UpdateSkip)     , new DynaValue(false)                   , IU_SND_BY_USER   , FEL_BAS_VIEW_ONLY   , FC_NEVER   , [CI_SRC_A , CI_SRC_A ])} ,
 			
			// for the family list, since the dictionary cannot be complex (e.g. <string, string>), the key for each entry must be a combination of family + type so that the entry is unique.
			// so the value then can contain settings for the family + type combo
			{RK_RD_FAMILY_LIST,      new (RK_RD_FAMILY_LIST,       "FamilyList",       "List of Families (& types)"  , null                         , new DynaValue(K_DICT)                  , IU_SND_ALT_SRC_A  , FEL_ADVANCED        , FC_NEVER   , [CI_SRC_D , CI_SRC_D ])} , // family & types
																																																										  // 
			// voided fields																																						   
			// {RK_AD_ADDINID,           new (RK_AD_ADDINID,           "AddInId",          "AddIn Id",                       new DynaValue(K_NOT_DEFINED_STR)           , IU_S_AND_DS  , FEL_DEBUG) }   ,
			// {RK_SD_SCHEMA_GUID,       new (RK_SD_SCHEMA_GUID,       "SchemaGuid",       "Schema GUID",                    new DynaValue(Guid.NewGuid())              , IU_S_AND_DS  , FEL_ADVANCED) },
																																													   
			// don't need - when a ds is deleted, it is gone - when a schema is deleted, I can only change its stored name															   
			// {RK_AD_DELETED,           new (RK_AD_DELETED,           "Deleted",          "Flagged as to be Deleted",       new DynaValue(false)                       , IU_S_AND_DS  , FEL_DEBUG) }   ,
		};

		// @formatter:on
	}
}

