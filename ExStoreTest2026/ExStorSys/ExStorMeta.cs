
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ExStoreTest2026.Windows;
using RevitLibrary;
using UtilityLibrary;

using static ExStorSys.ExStorConst;


// user name: jeffs
// created:   9/16/2025 10:25:35 PM

namespace ExStorSys
{
	public enum SettingId
	{
		// live
		SI_GOT_WBK_SCHEMA,
		// temp
		SI_GOT_TMP_WBK_SCHEMA,
		SI_GOT_TMP_SHT_SCHEMA,
		SI_GOT_TMP_WBK_DS,
		SI_GOT_TMP_SHT_DS,
		SI_GOT_TMP_WBK_DS_LIST,
		SI_GOT_TMP_SHT_DS_LIST,
		SI_GOT_TMP_WBK_SCHEMA_LIST,
		SI_GOT_TMP_SHT_SCHEMA_LIST,
	}

	// public enum OpState 
	// {
	// 	OS_DELETE_SHT	= -4,
	// 	OS_DELETE_WBK	= -2,
	// 	OS_NORMAL_OP	= 0,
	// 	OS_STARTED	    = 1,
	// 	OS_CREATE_WBK	= 2,
	// 	OS_CREATE_SHT	= 4,
	// }
	//
	// public enum OpUseTypes
	// {
	// 	OUT_DELETE	    = -2,
	// 	OUT_INFO	    = -1,
	// 	OUT_NORMAL_OP   = 0,
	// 	OUT_ANY		    = 1,
	// 	OUT_CREATE		= 2,
	// }

	public enum ExoCreationStatus
	{
		// < 0 do not use, must be replaced
		CS_CAN_DESTROY = -100,	// flagged to be deleted / may not need / use this
		CS_INVALID = -1,	// something is not correct (e.g., model name changed) or is a returned invalid object
		// == 0 && < 5 is incomplete / can be replaced
		CS_EMPTY = 1,		// created but with "empty" / basic information (init state)
		// >= 5, cannot replace - must delete first
		CS_CREATED = 5,		// created, has data, but DS or E missing
		CS_INIT = 6,		// created and has DS or E but not both
		CS_GOOD = 8,		// has data and DS and E
		CS_MODIFIED = 9,	// data revised but not written

	}

	public enum SheetOpStatus
	{
		SS_GOOD,
		SS_SKIP,
		SS_HOLD,
		SS_DELETE
	}

	public enum UpdateRules
	{
		UR_UNDEFINED    = -1,
		UR_NEVER        = 0,
		UR_AS_NEEDED    = 1,
		UR_UPON_REQUEST = 2,
		UR_COUNT        = 3
	}

	public enum FieldEditLevel
	{
		DL_DEBUG			= -1,
		DL_BASIC			= 0,
		DL_MEDIUM			= 1,
		DL_ADVANCED			= 2
	}

	public enum FieldUsage
	{
		FU_SCHEMA,
		FU_DATASTORAGE,
		FU_S_AND_DS,
	}


	// key information

	public static class ExStorConst
	{
		public static readonly string[,]  NAME_REPL_STRING =  new [,] {{ ".", "_"} } ;
		public static readonly string[]  DOC_NAME_REPL_STRING =  [ @"[^0-9a-zA-Z]", ""]  ;

		static ExStorConst()
		{
			CompanyId = CsStringUtil.CleanString(CsUtilities.CompanyName, NAME_REPL_STRING) ?? "";
			VendorId = CsStringUtil.CleanString(RevitAddinsUtil.GetVendorId(), NAME_REPL_STRING) ?? "";
			AppId = RevitAddinsUtil.GetAppId().ToLower();
		}

		// field type constants
		public const string K_NOT_DEFINED_TYPE = "";
		public const string K_NOT_DEFINED_STR = "<not defined>";

		public const string PRIMARY_SCHEMA_DESC = "Cells Primary DataStorage";

		public static List<string> K_ARRAY = new () { K_NOT_DEFINED_STR };

		/* info names*/

		public static string UserName => CsUtilities.UserName;
		public static string VendorId { get; private set; }
		public static string CompanyId { get; private set; }
		public static string AppId { get; private set; }

		// key constants

		public const int K_DS_NAME			= 0;
		public const int K_DESCRIPTION	    = 1;
		public const int K_VENDORID  	    = 2;
		public const int K_ADDINID          = 3;
		public const int K_DELETED			= 10;
		public const int K_DATE_CREATED	    = 20;
		public const int K_NAME_CREATED	    = 21;
		public const int K_DATE_MODIFIED	= 22;
		public const int K_NAME_MODIFIED	= 23;
		
		public const int K_SCHEMA_VERSION	= 100;
		// public const int K_WBK_SCHEMA_NAME	= 101;
		// public const int K_SHT_SCHEMA_NAME	= 102;
		// public const int K_SCHEMA_GUID	    = 103;

		// data storage and schema naming constants
		// todo replace this with an source from the external app

		public static readonly int ID_CODE_LENGTH = EXS_SHT_FIRST_ID_CODE.Length;

		public const string APP_CODE = "CsCells";
		public const string EXS_SCHEMA_NAME_CODE = "Schema";
		public const string EXS_WKB_NAME_CODE = "WKB";
		// public const string EXS_WKB_ID = "9999"; // last possible value
		public const string EXS_SHT_FIRST_ID_CODE = "AAAA"; // first possible value

		public const string EXS_VERSION = "v1_00";

		public const string EXS_SHT_NAME_CODE = "SHT";

		// const names
		// workbook schema name == WbkSchemaName          == CsCells_WBK_Schema_v1_00
		// sheet schema name    == ShtSchemaName          == CsCells_SHT_Schema_v1_00
		
		// WbkDsName     == 

		// general workbook search name == EXS_WBK_NAME_SEARCH == CsCells_WBK_
		// general sheet search name    == EXS_SHT_NAME_SEARCH == CsCells_SHT_

				
		/// <summary>
		/// always: CsCells_Wbk_Schema_v1_00
		/// </summary>
		public static string WbkSchemaName => $"{ExStorConst.EXS_WBK_DS_NAME_PREFIX}{ExStorConst.EXS_SCHEMA_NAME_CODE}_{ExStorConst.EXS_VERSION}";

		/// <summary>
		/// always: CsCells_Sht_Schema_v1_00
		/// </summary>
		public static string ShtSchemaName => $"{ExStorConst.EXS_SHT_NAME_SEARCH}{ExStorConst.EXS_SCHEMA_NAME_CODE}_{ExStorConst.EXS_VERSION}";

		// workbook name prefix - also the "base" name used for searching
		/// <summary>
		/// base name for a Wbk Ds (e.g. CsCells_WKB)
		/// </summary>
		public const string EXS_WBK_DS_NAME_PREFIX = $"{APP_CODE}_{EXS_WKB_NAME_CODE}_";

		/// <summary>
		/// name used to search for Wbk Ds (same as EXS_WBK_DS_NAME_PREFIX) (e.g. CsCells_WKB)
		/// </summary>
		public const string EXS_WBK_NAME_SEARCH = $"{EXS_WBK_DS_NAME_PREFIX}";
		
		// sheet name prefix - also the "base" name used for searching
		/// <summary>
		///  name used to search for Sht Ds (e.g. CsCells_SHT)
		/// </summary>
		public const string EXS_SHT_NAME_SEARCH = $"{APP_CODE}_{EXS_SHT_NAME_CODE}_";
		
		// sheet name prefix - also the "base" name used for searching - for the first DS
		// sheet name prefix - also the "base" name used for searching
		/// <summary>
		///  name used to search for the first Sht Ds (e.g. CsCells_SHT_AAAA)
		/// </summary>
		public const string EXS_SHT_DS_NAME_PREFIX_FIRST = $"{APP_CODE}_{EXS_SHT_NAME_CODE}_{EXS_SHT_FIRST_ID_CODE}";


		/* validation const's*/

		public static string[] DataClassAbbrevUc = ["WBK", "SHT"];
		public static string[] DataClassAbbrevTc = ["Wbk", "Sht"];
		public static string[] DataClassFull =     ["WorkBook", "Sheet"];
		public static string[] DataContainerFull = ["Schema", "DataStorage"];


		public static string[] ScValidateResults = [ "schema good", "no schema found", "one+ schema invalid", "more than one schema found" ];
		public static string[] DsValidateResults = [ "datastorage good", "no datastorage found", "one+ datastorage invalid", "more than one datastorage found" ];

		public static string[] ScValidateResolve =
			["n/a", "Create new", "Delete until one valid", "Delete all except one"];
		//   ^ good, ^ none        ^ one+ invalid,           ^ good but 1+ found

		// public static string[] ScValidateResolveAndNoDsFound =
		// 	["n/a", "Create new", "Delete all; Create new", "Delete All; Create new"];
		// //   ^ good, ^ none        ^ one+ invalid,           ^ good but 1+ found

		// assume that schema exists at this point
		public static string[] DsWbkValidateResolve =
			["n/a", "Create new", "Delete until one valid & matches sheet model code",  "Delete until one valid & matches sheet model code"];
		//   ^ good, ^ none        ^ one+ invalid,                                       ^ good but 1+ found


		public static string[] DsShtValidateResolve =
			["n/a", "Create new", "Delete invalid & wrong model code", "Delete any with wrong model code"];
		//   ^ good, ^ none        ^ one+ invalid,                                       ^ good but 1+ found

		/// <summary>
		/// the model code is a unique value that is associated with a specific model but<br/>
		/// does not identify a specific model.  that is, this code cannot be used to find a specific <br/>
		/// model by itself.  it can only be used to validate objects with this code all belong to a<br/>
		/// specific model
		/// </summary>
		public static string CreateModelCode()
		{
			return DateTime.Now.ToString("yyMMdd_HHmmss");
		}

		public static string CreateNextIdCode(string last)
		{
			char[] c = last.ToCharArray();

			for (int i = 3; i >= 0; i--)
			{
				if (c[i] == 'Z')
				{
					c[i] = '0';
					break;
				}
				
				if (c[i] == '9')
				{
					c[i] = 'A';
					continue;
				}

				c[i]++;
				break;
			}

			return new string(c);
		}
		
	}

	// ds field names
	public enum WorkBookFieldKeys
	{
		PK_DS_NAME                = K_DS_NAME,
		PK_AD_DESC                = K_DESCRIPTION,
		PK_AD_VENDORID            = K_VENDORID,
		// PK_AD_ADDINID             = K_ADDINID,

		PK_AD_DELETED             = K_DELETED,

		PK_AD_DATE_CREATED        = K_DATE_CREATED,
		PK_AD_NAME_CREATED        = K_NAME_CREATED,
		PK_AD_DATE_MODIFIED       = K_DATE_MODIFIED,
		PK_AD_NAME_MODIFIED       = K_NAME_MODIFIED,

		PK_AD_LAST_ID			  ,
		PK_AD_MODEL_CODE          ,

		PK_SD_SCHEMA_VERSION      = K_SCHEMA_VERSION,
		// PK_SD_WBK_SCHEMA_NAME     = K_WBK_SCHEMA_NAME,
		// PK_SD_SHT_SCHEMA_NAME     = K_SHT_SCHEMA_NAME,
		// PK_SD_SCHEMA_GUID         = K_SCHEMA_GUID,

		PK_MD_MODEL_NAME          ,
	}

	public enum SheetFieldKeys
	{
		RK_DS_NAME                = K_DS_NAME,
		RK_AD_DESC                = K_DESCRIPTION,
		RK_AD_VENDORID            = K_VENDORID,
		// RK_AD_ADDINID             = K_ADDINID,

		RK_AD_DELETED             = K_DELETED,

		RK_AD_DATE_CREATED        = K_DATE_CREATED,
		RK_AD_NAME_CREATED        = K_NAME_CREATED,
		RK_AD_DATE_MODIFIED       = K_DATE_MODIFIED,
		RK_AD_NAME_MODIFIED       = K_NAME_MODIFIED,

		RK_SD_SCHEMA_VERSION      = K_SCHEMA_VERSION,
		// RK_SD_SCHEMA_GUID         = K_SCHEMA_GUID,

		RK_ED_XL_FILE_PATH        ,
		RK_ED_XL_SHEET_NAME       ,

		RK_OD_STATUS              ,
		RK_OD_SEQUENCE            ,
		RK_OD_UPDATE_RULE         ,
		RK_OD_UPDATE_SKIP         ,
		
		RK_RD_FAMILY_LIST            ,
	}




}
