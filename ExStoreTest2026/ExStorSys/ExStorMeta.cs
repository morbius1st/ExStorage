
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ExStoreTest2026.Windows;
using RevitLibrary;
using UtilityLibrary;

using static ExStorSys.ExStorConst;
using static ExStorSys.ExSysStatus;
using static ExStorSys.ValidateSchema;
using static ExStorSys.ValidateDataStorage;
using static ExStorSys.LaunchCode;
using static ExStorSys.RunningStatus;
using static ExStorSys.Numbers;


// user name: jeffs
// created:   9/16/2025 10:25:35 PM

namespace ExStorSys
{
	public enum Numbers
	{
		N_NEG_IC = -99,
		N_DB = -10,

		N_NEG_THREE= -3,
		N_NEG_TWO = -2,
		N_NEG_ONE = -1,

		N_ZERO		= 0,
		N_ONE		= 1,
		N_TWO		= 2,
		N_THREE		= 3, 
		N_FOUR		= 4,
		N_FIVE		= 5,
		N_SIX		= 6,
		N_SEVEN		= 7,
		N_EIGHT		= 8,
		N_NINE		= 9,

		N_TWENTY    = 20,
		N_GOOD      = 30,

		// N_ONEOHONE = 101,
		// N_ONEOHTWO = 102,
		// N_ONEOHFOUR = 104,
	}


	public enum RunningStatus
	{
		RN_DEBUG				= -10,
		RN_NA					= 0,
		RN_DEACTIVATE			, 
		RN_CANNOT_RUN_FAIL		,
		RN_CANNOT_RUN_RESTART	,
		RN_NOT_RUNNING			,
		RN_READY_NOT_RUNNING	,
		RN_RUNNING_NEED_SHT		,
		RN_RUNNING_NORMAL		,
	}

	
	public enum LaunchCode
	{
		LC_DEBUG			= -10,
		LC_DEFAULT			= 0,
		LC_NA				= 1,
		LC_STARTED			= 2,
		LC_PROG_GOOD			, // progress - good so far
		LC_PROG_FAIL			, // progress - failed
		LC_DONE_RESTART			,
		LC_DONE_RNR				,
		LC_DONE_HOLD			,
		LC_DONE_UPGRADE			,
		LC_DONE_CREATE			,
		LC_DONE_MISSING			,
		LC_DONE_INVALID			,
		LC_DONE_BAD_VER			,
		LC_DONE_INVALID_BAD_VER	,
		// LC_DONE_MOD_CODE		,
		LC_DONE_GOOD		    ,
		LC_DONE_FAIL		    ,
	}


	public enum ExSysStatus
	{
		ES_SHT_DELETED				= -6,
		ES_WBK_DELETED				= -5,
		ES_RESTART_REQD				= -3,		
		ES_NOT_GOOD  				= -2,		

		// < 0 system has issues
		ES_DEFAULT					=  0,
		ES_NA						=  1,
		ES_STARTED					=  2,
		// > 0 system is normal and progressing to READY
		// codes 20 to 39 (max) = launch / verify / resolve

		ES_VRFY					   = 30,
		ES_VRFY_INIT_FAIL			,
		ES_VRFY_INIT_GOOD			,
		ES_VRFY_RSLV_FAIL			,
		ES_VRFY_RSLV_GOOD			,

		ES_VRFY_DONE_HOLD_SC		,
		ES_VRFY_DONE_HOLD_WBK		,
		ES_VRFY_DONE_HOLD_SHT		,
		ES_VRFY_DONE_HOLD_ACT		,

		ES_VRFY_DONE_FAIL			,
		ES_VRFY_DONE_RESTART		,
		ES_VRFY_DONE_RECONFIG		,
		ES_VRFY_DONE_ACT_OFF		,
		ES_VRFY_DONE_ACT_IGNORE		,
		ES_VRFY_DONE_MISSING		,
		ES_VRFY_DONE_MOD_NAME		,
		ES_VRFY_DONE_INVALID		,
		ES_VRFY_DONE_BAD_VER			,
		ES_VRFY_DONE_INVALID_OR_BAD_VER	,
		ES_VRFY_DONE_GOOD			,

		ES_WBK_SCHEMA_CREATED	    = 50,
		ES_SHT_SCHEMA_CREATED		,
		ES_WBK_STARTED			    ,
		ES_SHT_STARTED			    ,
		ES_WBK_CREATED			    ,
		ES_SHT_CREATED			    ,
		// >= ES_START_DONE_GOOD                  , system running normal
		ES_START_DONE_GOOD			,
		ES_START_DONE_DEACTIVATE	,
		ES_START_DONE_FAIL			,
		ES_START_DONE_EXIT			,

	}

	/* property system */

	/* property classes */
	// public enum PropertyOwner  // that is, who generated the property event - not always the owner class
	// {
	// 	PO_GEN  = 0, 
	// 	PO_XSYS    ,
	// 	PO_XMGR    ,
	// 	PO_STMGR   ,
	// 	PO_XDATA   ,
	// 	PO_EXO     ,
	// 	PO_LMGR    ,
	// }

	/* property identifiers */
	public enum PropertyId
	{
		// general / multi-category
		PI_GEN_RUNNING_STAT	,
		PI_GEN_RESTART	    ,
		PI_GEN_LAUNCHCODE   ,

		// system status
		PI_XSYS_STATUS		,

		// verification 
		PI_VFY_WBK_SC		= 5,
		PI_VFY_WBK_DS		,
		PI_VFY_SHT_SC		,
		PI_VFY_SHT_DS		,

		// exstormgr
		// PI_XMGR_XSYS_RUN	,

		// exstordata
		PI_XDATA_WBK        ,
		PI_XDATA_WBK_SC     ,
		PI_XDATA_WBK_DS     ,
		PI_XDATA_SHT        ,
		PI_XDATA_SHT_SC     ,
		PI_XDATA_SHT_DS     ,

		// launch manager
		// PI_LMGR_LCODE		,
		// PI_LMGR_RESOLV       ,
	}

	/* startup verification */

	public enum ValidateSchema
	{
		VSC_DEBUG				= N_DB,        //
		VSC_VRFY_UNTESTED		= N_NEG_THREE,  //
		VSC_NG					= N_NEG_TWO ,  //
		VSC_NA					= N_NEG_ONE ,  //
		VSC_DEFAULT				= N_ZERO ,	   //

		VSC_GOOD				= N_GOOD ,	   // 30
		
		VSC_MISSING				= N_ONE,	   //  1
		VSC_WRONG_COUNT			= N_TWO,	   //  2
		VSC_INVALID				= N_THREE,     //  3
		VSC_WRONG_VER			= N_SIX,       //  6

		// invalid & wrong ver can occur at the same time in any combination
		// combinations invalid, wrong ver, both

		// voided
		// VSC_INVALID_OR_WRONG_VER = N_ONEOHFOUR,
		// VSC_OUT_OF_DATE			= N_ONEOHFOUR, - same as wrong version
	}

	public enum ValidateDataStorage
	{
		VDS_DEBUG				= N_DB,        //
		VDS_VRFY_UNTESTED		= N_NEG_THREE,  //
		VDS_NG					= N_NEG_TWO ,  //
		VDS_NA					= N_NEG_ONE ,  //
		VDS_DEFAULT				= N_ZERO ,	   //
		
		VDS_GOOD				= N_GOOD ,	   //

		VDS_MISSING				= N_ONE,	   //  1
		VDS_WRONG_COUNT			= N_TWO,	   //  2
		VDS_INVALID				= N_THREE,     //  3
											   //
		VDS_WRONG_VER			= N_SIX,       //  6
											   //
		VDS_WRONG_MODEL_NAME	= N_NINE,	   //  9
		
		VDS_ACT_IGNORE         	= N_TWENTY + N_ZERO,	// 20
		VDS_ACT_OFF            	= N_TWENTY + N_ONE,		// 21
		VDS_MULTIPLE_MN_O		= N_TWENTY + N_TWO,     // 22 - MN (model name) O (act off) 

		// voided
		// VDS_INVALID_N_WRONG_VER	= N_ONEOHFOUR,
		// VDS_WRONG_MODEL_CODE	= N_SIX,
		// VDS_NOT_BAD			,	// VOID
	}

	/* various enums */


	/* fields */

	/// <summary>
	/// controls who can access the field.  
	/// that is, a user's security level us used and the user
	/// can access a field if the field's edit level matches
	/// their security level or is a larger number.<br/>
	/// also, NONE, -1, cannot access anything
	/// </summary>
	public enum FieldEditLevel 
	{
		FEL_LOCKED			= -1,
		FEL_UNASSIGNED		= 0,
		FEL_VIEW_ONLY		= 5,
		FEL_DEBUG			= 10,
		FEL_ADVANCED		= 40,
		FEL_ADV_VIEW_ONLY	= FEL_ADVANCED + FelSubCat.FESC_VIEWONLY,
		FEL_BASIC			= 60,
		FEL_BAS_VIEW_ONLY	= FEL_BASIC + FelSubCat.FESC_VIEWONLY,
	}

	public enum FelSubCat
	{
		FESC_VIEWONLY	=1,
	}

	public enum FieldEditStatus
	{
		FES_NONE		= -1,
		FES_CAN_VIEW	=  0,
		FES_CAN_EDIT	=  1,
	}

	public enum FieldStatus
	{
		FS_DIRTY  = -1,
		FS_IGNORE = 0,
		FS_CLEAN  = 1,
	}

	[Flags]
	public enum FieldCopyType
	{
		FC_IGNORE   = -1,
		FC_NEVER    = 0, // none
		FC_ALWAYS   = 7, // 1, 2, & 4
		FC_TYPE_1   = 1,
		FC_TYPE_2   = 2,
		FC_TYPE_12  = 3,
		FC_TYPE_4   = 4,
		FC_TYPE_14  = 5,
		FC_TYPE_15  = 6,
		
	}

	public enum ItemUsage
	{
		IU_SCHEMA,
		IU_DATASTORAGE,
		IU_S_AND_DS,
		IU_WBK,
		IU_SHT
	}

	/*family / type enums */

	public enum FamTypeCategories
	{
		PROCESS,
	}

	public enum FamTypeRequirement
	{
		REQUIRED,
		OPTIONAL
	}

	// for these enums, the enum name will be converted to 
	// string for presentation to the user.  the name will get
	// converted to proper case and underscores will be converted
	// to spaces
	public enum FamTypeProcessProp
	{
		ACTIVE,
		INACTIVE,
		AT_OPEN,
		AT_CLOSE
	}
	
	/* enums saved into a DS / Entity via a schema
	 * privide a default value in the cost class below
	 */

	public enum ActivateStatus
	{
		AS_NA		= N_NEG_ONE,
		AS_DEFAULT	= N_ZERO,
		AS_ACTIVE	,		// system is active for this model
		AS_INACTIVE	,	// system has been put on hold for this model
		AS_IGNORE		// ignore this model - do not add to this model
	}

	public enum SheetOpStatus
	{
		SS_GOOD,
		SS_SKIP,
		SS_HOLD,
		SS_DELETE,
		SS_NA		= N_NEG_ONE,
	}

	public enum UpdateRules
	{
		UR_UNDEFINED    = -1,
		UR_NEVER        = 0,
		UR_AS_NEEDED    = 1,
		UR_UPON_REQUEST = 2,

	}

	// key information

	public static class ExStorConst
	{
		public const string APP_NAME = "ExStorage System";
		/* static default enum values that are saved into a DS / entity via a schema */

		public const UpdateRules DEFAULT_UPDATE_RULE = UpdateRules.UR_NEVER;
		public const ActivateStatus DEFAULT_ACTIVATE_STATUS = ActivateStatus.AS_INACTIVE;
		public const SheetOpStatus DEFAULT_SHEET_OP_STATUS = SheetOpStatus.SS_GOOD;


		public static readonly string[,]  NAME_REPL_STRING =  new [,] {{ ".", "_"} } ;
		public static readonly string[]  DOC_NAME_REPL_STRING =  [ @"[^0-9a-zA-Z]", ""]  ;

		static ExStorConst()
		{
			CompanyId = CsStringUtil.CleanString(CsUtilities.CompanyName, NAME_REPL_STRING) ?? "";
			VendorId = CsStringUtil.CleanString((RevitAddinsUtil.GetVendorId() ?? "missing"), NAME_REPL_STRING) ?? "";
			AppId = (RevitAddinsUtil.GetAppId() ?? "missing").ToLower();
		}

		/* methods */

		// /// <summary>
		// /// the model code is a unique value that is associated with a specific model but<br/>
		// /// does not identify a specific model.  that is, this code cannot be used to find a specific <br/>
		// /// model by itself.  it can only be used to validate objects with this code all belong to a<br/>
		// /// specific model
		// /// </summary>
		// public static string CreateModelCode()
		// {
		// 	return DateTime.Now.ToString("yyMMdd_HHmmss");
		// }

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

		/* field type constants */

		public const string K_NOT_DEFINED_TYPE = "";
		public const string K_NOT_DEFINED_STR = "<not defined>";
		public const string K_FAM_LIST_INIT_ENTRY = "<Undefined|Unset>";
		public const string K_FAM_LIST_INIT_ENTRY_OLD = "Undefined|Unset";

		public const string PRIMARY_SCHEMA_DESC = "Cells Primary DataStorage";

		// public static List<string> K_DICT = new () {K_FAM_LIST_INIT_ENTRY };
		public static Dictionary<string, string> K_DICT = new () { {K_FAM_LIST_INIT_ENTRY, K_NOT_DEFINED_STR } };
		
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
		// public static readonly int MODEL_CODE_LENGTH = "250101_000000".Length;

		public const string APP_CODE = "CsCells";
		public const string EXS_SCHEMA_NAME_CODE = "Schema";
		// public const string EXS_WKB_ID = "9999"; // last possible value
		public const string EXS_SHT_FIRST_ID_CODE = "AAAA"; // first possible value

		/* workbook version information */
		public const string EXS_WKB_NAME_CODE = "WKB";
		public const string EXS_VERSION_WBK = "v1_00";
		// next version, adjust to include the version number per the current sheet guid
		public static readonly Guid WbkSchemaGuid = new Guid("A35D2205-CFFA-4EE0-ACED-DECADE20250A" );

		/* sheet version information */
		public const string EXS_SHT_NAME_CODE = "SHT";
		public const string EXS_VERSION_SHT = "v1_10";
		public static readonly Guid ShtSchemaGuid = new Guid("A35D2205-CFFA-0110-ACED-DECADE20260B" );


		// const names
		// workbook schema name == WbkSchemaName          == CsCells_WBK_Schema_v1_00
		// sheet schema name    == ShtSchemaName          == CsCells_SHT_Schema_v1_00
		
		// WbkDsName     == 

		// general workbook search name == EXS_WBK_NAME_SEARCH == CsCells_WBK_
		// general sheet search name    == EXS_SHT_NAME_SEARCH == CsCells_SHT_

		/// <summary>
		/// always: CsCells_Wbk_Schema_v1_00
		/// </summary>
		public static string WbkSchemaName => $"{ExStorConst.EXS_WBK_DS_NAME_PREFIX}{ExStorConst.EXS_SCHEMA_NAME_CODE}_{ExStorConst.EXS_VERSION_WBK}";

		/// <summary>
		/// always: CsCells_Sht_Schema_v1_00
		/// </summary>
		public static string ShtSchemaName => $"{ExStorConst.EXS_SHT_NAME_SEARCH}{ExStorConst.EXS_SCHEMA_NAME_CODE}_{ExStorConst.EXS_VERSION_SHT}";

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

		/* ex stor status const */

		public static Dictionary<RunningStatus, string> RunningStatusDesc = new ()
		{
			{ RN_DEBUG				, "debug status"},
			{ RN_NA					, "n/a"},
			{ RN_DEACTIVATE	        , "system deactivated for this model"},
			{ RN_CANNOT_RUN_FAIL	, "Failure, cannot run"},
			{ RN_CANNOT_RUN_RESTART	, "Restart Required, cannot run"},
			{ RN_NOT_RUNNING		, "system NOT running"},
			{ RN_READY_NOT_RUNNING	, "ready but not running"},
			{ RN_RUNNING_NEED_SHT   , "system running normal but needs sheets"},
			{ RN_RUNNING_NORMAL	    , "system running normally"},
		};

		/* ExSysStatus - alternate status system
		 * > struct with (3) levels
		 * Level 1 - overall status = inactive / starting (booting) / initializing / active
		 * level 2 - intermediate status = launch / initializing | launch | verifying
		 * level 3 - low level status = good / bad / working
		 *
		 * or
		 *
		 * just adjust the current system to have each status point to be
		 * multilevel - e.g.
		 * ES_LN_START, ES_LN_VFRG, ES_LN_VFRG_BAD,
		 * ES_LN_VFRG_GOOD (and segrate by value e.g. all ES_LN = 20 to 29)
		 *
		 * or
		 *
		 * hybrid
		 *
		 * have (2) levels - primary and sub
		 * have the primary level register as primary and all other status
		 * assignments are saved as secondary so they can report back to the primary
		 *
		 */

		public static Dictionary<ExSysStatus, string> ExStorStatDesc = new()
		{
			{ES_DEFAULT             , "Default value, UnSet"}                       ,
			{ES_NA                  , "Not applicable / UnSet"}                     ,
			{ES_STARTED             , "Started & Configured"}                       ,
			
			/* verify */

			{ES_VRFY_INIT_GOOD		, "Verify, Init Good"}                          ,
			{ES_VRFY_INIT_FAIL		, "Verify, Init Fail"}                          ,
			
			{ES_VRFY_RSLV_GOOD		, "Verify, Resolve Good"}                       ,
			{ES_VRFY_RSLV_FAIL		, "Verify, Resolve Fail"}                       ,
			
			{ES_VRFY_DONE_HOLD_SC	, "Verify Done, setup, schema up"}              , // start from scratch
			{ES_VRFY_DONE_HOLD_WBK	, "Verify Done, setup, workbook datastorage up"}, // almost normal, just need to setup workbook & sheets
			{ES_VRFY_DONE_HOLD_SHT	, "Verify Done, setup, sheet datastorage up"}   , // basically normal, just need to setup some sheets
			{ES_VRFY_DONE_HOLD_ACT	, "Verify Done, hold due to activation"}        , // activation is off or ignore
			
			{ES_VRFY_DONE_FAIL		, "Verify Done, and Failed"}                    , // something went wrong - abort
			{ES_VRFY_DONE_RESTART	, "Verify Done, need to restart the system"}    , // something is wrong, need to start over
			{ES_VRFY_DONE_RECONFIG	, "Verify Done, need to re-configure the system"}, // cannot use the current system - need to clear and restart
			{ES_VRFY_DONE_ACT_OFF	, "Verify Done, activation is off"}             ,
			{ES_VRFY_DONE_ACT_IGNORE, "Verify Done, activation is ignore"}          ,
			{ES_VRFY_DONE_MOD_NAME	, "Verify Done, incorrect model name"}          ,
			{ES_VRFY_DONE_MISSING	, "Verify Done, element is missing"}            ,
			{ES_VRFY_DONE_INVALID	, "Verify Done, element is invalid"}            ,
			{ES_VRFY_DONE_BAD_VER		, "Verify Done, element is out of date"}        ,
			{ES_VRFY_DONE_INVALID_OR_BAD_VER	, "Verify Done, element is invalid and has wrong version"} ,
			{ES_VRFY_DONE_GOOD		, "Verify Done and Good, but nothing read"}     , // all went well, proceed, read data

			/* initialization */

			{ES_WBK_SCHEMA_CREATED  , "WorkBook Schema Created"}                    ,
			{ES_SHT_SCHEMA_CREATED  , "Sheet Schema Created"}                       ,
			{ES_WBK_STARTED         , "WorkBook Complete"}                          ,
			{ES_SHT_STARTED         , "Sheet(s) Complete"}                          ,
			{ES_WBK_CREATED         , "WorkBook Complete"}                          ,
			{ES_SHT_CREATED         , "Sheet(s) Complete"}                          ,

			/* start status */

			{ES_START_DONE_GOOD     , "Start Done; Ready"}                          ,
			{ES_START_DONE_DEACTIVATE, "Start Done; Processing Deactivated"}        ,
			{ES_START_DONE_FAIL     , "Start Done; Failed"}                         ,
			{ES_START_DONE_EXIT     , "Start Done; User Chose to Exit"}             ,

			/* issues */

			{ES_RESTART_REQD        , "Restart Required"}                           ,
			{ES_WBK_DELETED         , "WorkBook Deleted"}                           ,
			{ES_SHT_DELETED         , "Sheet Deleted"}                              ,

			// no ... schema deleted - once either is deleted, restart is required
		};

		public static Dictionary<LaunchCode, string> LaunchCodeDesc = new ()
		{
			{ LC_DEBUG,				"Launch debug" },
			{ LC_DEFAULT,			"Default value, LC not set" },
			{ LC_NA,				"Not Applicable, Launch does not apply" },
			{ LC_STARTED,			"Launch Progressing, Good" },
			{ LC_PROG_GOOD,			"Launch Progressing, Fail" },
			{ LC_PROG_FAIL,			"Launch Started" },
			{ LC_DONE_RESTART,		"Launch done, start from scratch" },
			{ LC_DONE_RNR,			"Launch done, schema and/or ds needs repair then restart" },
			{ LC_DONE_HOLD,			"Launch done, on hold" },
			{ LC_DONE_UPGRADE,		"Launch done, schema / datastore is out of date" },
			{ LC_DONE_CREATE,		"Launch done, a datastore needs to be created" },
			{ LC_DONE_MISSING,		"Launch done, element is missing" },
			{ LC_DONE_INVALID,		"Launch done, element is invalid" },
			{ LC_DONE_BAD_VER,		"Launch done, element has the wrong version" },
			{ LC_DONE_INVALID_BAD_VER,	"Launch done, element is invalid and has the wrong version" },
			// { LC_DONE_MOD_CODE,		"Launch done, incorrect model code" },
			{ LC_DONE_GOOD,			"Launch done, good" },
			{ LC_DONE_FAIL,			"Launch done, failed" },
		
		};

		//                                           issue                  test /
		// 0 = ignore | 1 = add to main desc     reporting   issue   issue  verify 
		// 2 = add to ext desc                      code v  desc v   fix v  code v
		//                                               v       v       v       v
		public static Dictionary<ValidateSchema, Tuple<int, string, string, string>> ValidateSchemaDesc = new ()
		{
			{VSC_DEBUG			     , new (0, "For Debugging"					  , "debug only"                      , "db" ) }, //
			{VSC_VRFY_UNTESTED       , new (0, "Validation Cannot be Performed"   , "n/a"                             , "ut" ) }, //
			{VSC_NG				     , new (0, "Not good value"	                  , "n/a"                             , "na" ) }, // 
			{VSC_NA				     , new (0, "N/A value, Unset"	              , "n/a"                             , "na" ) }, // 
			{VSC_DEFAULT		     , new (0, "Default value, Unset"	          , "default"                         , "df" ) }, // 
			{VSC_GOOD			     , new (0, "schema good"                      , "n/a"                             , "gd" ) }, //
			
			{VSC_MISSING		     , new (2, "No Schema was found"              , "The system must be created new"  , "ms" ) }, // 
			{VSC_WRONG_COUNT		 , new (0, "one+ good schema found"           , "n/a cannot happen"               , "wc" ) }, // 
			{VSC_INVALID		     , new (2, "The Schema is invalid"            , "The system must be created new"  , "iv" ) }, // 
			{VSC_WRONG_VER	         , new (1, "The Schema has the Wrong Version" , "The System must be updated"      , "wv" ) }, // 
								     
			// voided
			// {VSC_INVALID_OR_WRONG_VER , new ("schema is invalid or has wrong version"  , "need to upgrade") },	// 
		};

		public static Dictionary<ValidateDataStorage,  Tuple<int, string, string, string>> ValidateDataStorageDesc = new ()
		{
			{VDS_DEBUG			     , new (0, "For Debugging"						              , "debug only"                             , "db" ) }, //
			{VDS_VRFY_UNTESTED       , new (0, "Validation Cannot be Performed"                   , "n/a"                                    , "ut" ) }, //
			{VDS_NG	                 , new (0, "Not good value"			                          , "n/a"                                    , "na" ) }, //
			{VDS_NA	                 , new (0, "N/A value, Unset"			                      , "n/a"                                    , "na" ) }, //
			{VDS_DEFAULT		     , new (0, "Default value, Unset"				              , "default"                                , "df" ) }, // 
			{VDS_GOOD                , new (0, "datastorage good"                                 , "n/a"                                    , "gd" ) }, //
			
			{VDS_MISSING             , new (2, "The DataStorage object was NOT found"             , "The system must be created new"         , "ms" ) }, //
			{VDS_WRONG_COUNT         , new (0, "one+ good datastorage found"                      , "Delete Until Only One Valid Remains"    , "wc" ) }, //
			{VDS_INVALID             , new (2, "The DataStorage object is invalid"                , "The system must be created new"         , "iv" ) }, //
			{VDS_WRONG_VER			 , new (1, "The DataStorage has the Wrong Version"            , "The System must be updated"             , "wv" ) }, //
			{VDS_ACT_OFF             , new (1, "ExStorSystem has been deactivated"                , "The System must be re-activated"        , "ao" ) }, //
			{VDS_ACT_IGNORE          , new (1, "ExStorSystem has been disabled"                   , "The System must be re-activated"        , "ai" ) }, //
			{VDS_WRONG_MODEL_NAME    , new (1, "The DataStorage object has the Wrong Model Name"  , "The System must be updated"             , "mn" ) }, //
			{VDS_MULTIPLE_MN_O       , new (0, "datastorage multiple issues"                      , "some, Request to Fix"              , "mu" ) }, //

			// voided
			// {VDS_WRONG_MODEL_CODE  , new ("datastorage has the Wrong Model Code"  , "Request to Fix") },	//
			// {VDS_INVALID_N_WRONG_VER , new ("datastorage is invalid and has wrong version" , "need to upgrade") },   //
		};

		public static Dictionary<FieldEditLevel, Tuple<string, string, SolidColorBrush>> FieldEditLevelDesc = new ()
		{
			{ FieldEditLevel.FEL_LOCKED         , new ("Locked",               "User is not known / Has no permissions"    , Brushes.Red) },
			{ FieldEditLevel.FEL_DEBUG          , new ("Debug (Only)",         "Editable by Debugging Users"               , Brushes.LawnGreen) },
			{ FieldEditLevel.FEL_ADVANCED       , new ("Advanced",             "Editable by Advanced Users"                , Brushes.Blue) },
			{ FieldEditLevel.FEL_ADV_VIEW_ONLY  , new ("Advanced (View Only)", "Viewable by Advanced Users"                , Brushes.DodgerBlue) },
			{ FieldEditLevel.FEL_BASIC          , new ("Basic",                "Editable by Basic Users"                   , Brushes.DarkViolet) },
			{ FieldEditLevel.FEL_BAS_VIEW_ONLY  , new ("Basic (View Only)",    "Viewable by Users"                         , Brushes.MediumPurple) },
			{ FieldEditLevel.FEL_VIEW_ONLY      , new ("View Only",            "Can only be viewed"                        , Brushes.Yellow) },
			{ FieldEditLevel.FEL_UNASSIGNED     , new ("Unassigned",           "Edit level not assigned"                   , Brushes.OrangeRed) },
		};

		public static Dictionary<FieldStatus, Tuple<string, string, SolidColorBrush>> FieldStatusDesc = new ()
		{
			{ FieldStatus.FS_IGNORE , new ("Ignore",     "Field is fixed / cannot be changed"                       , Brushes.White) },
			{ FieldStatus.FS_CLEAN  , new ("Unmodified", "Field has not been changed"                               , Brushes.White) },
			{ FieldStatus.FS_DIRTY  , new ("Revised",    "Field has been changed"                                   , Brushes.White) },
		};

		public static Dictionary<UserSecutityLevel, Tuple<string, string, SolidColorBrush>> UsserSecurityLevelDesc = new ()
		{
			{ UserSecutityLevel.SL_DEBUG    , new ("Debug (Only)", "Unrestricted access"                            , Brushes.LawnGreen) },
			{ UserSecutityLevel.SL_ADVANCED , new ("Advanced",   "Advanced access"                                  , Brushes.Blue) },
			{ UserSecutityLevel.SL_BASIC    , new ("Basic",      "Limited, basic access"                            , Brushes.DarkViolet) },
			{ UserSecutityLevel.SL_LIMITED  , new ("Limited",    "Limited to viewing basic view only fields"        , Brushes.Fuchsia) },
			{ UserSecutityLevel.SL_VIEW_ONLY, new ("View Only",  "Limited to viewing basic fields"                  , Brushes.Yellow) },
			{ UserSecutityLevel.SL_UNASSIGNED,new ("Unassigned", "No access permitted"								, Brushes.Violet) },
			{ UserSecutityLevel.SL_NONE     , new ("None", "No access permitted"									, Brushes.MediumSeaGreen) },
		};

		public static Dictionary<UpdateRules, Tuple<string, string, SolidColorBrush>> UpdateRulesDesc = new ()
		{
			{ UpdateRules.UR_UNDEFINED     , new ("Not Assigned", "Not Assigned"                         , Brushes.White) },
			{ UpdateRules.UR_AS_NEEDED     , new ("As Needed"   , "Automatic, when Needed"               , Brushes.Blue) },
			{ UpdateRules.UR_UPON_REQUEST  , new ("Upon Request", "Not Until Requested"                  , Brushes.LawnGreen) },
			{ UpdateRules.UR_NEVER         , new ("Never"       , "Never Update"                         , Brushes.Red) },
		};

		public static Dictionary<SheetOpStatus, Tuple<string, string, SolidColorBrush>> SheetOpStatusDesc = new ()
		{
			{ SheetOpStatus.SS_NA,     new ("Not Assigned",   "Not Assigned"                             , Brushes.White) },
			{ SheetOpStatus.SS_HOLD,   new ("Hold",   "Hold for this Session"                            , Brushes.Blue) },
			{ SheetOpStatus.SS_DELETE, new ("Delete", "Delete and remove all operations"                 , Brushes.Red) },
			{ SheetOpStatus.SS_GOOD,   new ("Good",   "Good, proceed normal"                             , Brushes.LawnGreen) },
			{ SheetOpStatus.SS_SKIP,   new ("Skip",   "Skip these operations for until reset"            , Brushes.Yellow) },

		};

		public static Dictionary<ActivateStatus, Tuple<string, string, SolidColorBrush>> ActiveStatusDesc = new ()
		{
			{ ActivateStatus.AS_NA       , new ("Not Assigned", "Not Assigned"               , Brushes.White) },
			{ ActivateStatus.AS_DEFAULT  , new ("Unassigned"  , "Activation Not Set"         , Brushes.Red) },
			{ ActivateStatus.AS_IGNORE   , new ("Ignore"      , "Ignore this item"           , Brushes.Blue) },
			{ ActivateStatus.AS_INACTIVE , new ("Inactive"    , "Activation Disabled"        , Brushes.Yellow) },
			{ ActivateStatus.AS_ACTIVE   , new ("Active"      , "Activation Enabled"         , Brushes.LawnGreen) },

		};

		public static Dictionary<ActivateStatus, Tuple<string, string, SolidColorBrush>> ActiveStatusDescUi = new ()
		{
			{ ActivateStatus.AS_ACTIVE, ActiveStatusDesc[ActivateStatus.AS_ACTIVE] },
			{ ActivateStatus.AS_INACTIVE, ActiveStatusDesc[ActivateStatus.AS_INACTIVE] },
			{ ActivateStatus.AS_IGNORE, ActiveStatusDesc[ActivateStatus.AS_IGNORE] },
		};

		public static Dictionary<FamTypeCategories, Tuple<List<Tuple<Enum, string, SolidColorBrush>>, 
			FamTypeRequirement, Enum, 
			string, SolidColorBrush>> FamilyAndTypeProperties = new ()
		{
			{FamTypeCategories.PROCESS, new (new ()
			{
				new ( FamTypeProcessProp.ACTIVE,   "Process this item normally", Brushes.LawnGreen), 
				new ( FamTypeProcessProp.INACTIVE, "Never process this item", Brushes.OrangeRed),
				new ( FamTypeProcessProp.AT_OPEN,  "Process this item once when the model is opened", Brushes.Blue),
				new ( FamTypeProcessProp.AT_CLOSE, "Process this item once when the model is opened", Brushes.DarkGray),
			} , 
				FamTypeRequirement.REQUIRED, FamTypeProcessProp.ACTIVE,// is this property required and which setting is the default
				"Determines how to process this Family Type", Brushes.White) }
		};

		/* restart const's*/

		public static string[] RestartStatDesc = ["No Restart Needed", "Restart Required", "System Not Active"];

		/* validation const's*/

		public static string[] DataClassAbbrevUc = ["WBK", "SHT"];
		public static string[] DataClassAbbrevTc = ["Wbk", "Sht"];
		public static string[] DataClassFull =     ["WorkBook", "Sheet"];
		public static string[] DataContainerFull = ["Schema", "DataStorage"];
	}

	// ds field names
	public enum WorkBookFieldKeys
	{
		PK_DS_NAME                = K_DS_NAME,
		PK_AD_DESC                = K_DESCRIPTION,
		PK_AD_VENDORID            = K_VENDORID,
		PK_AD_STATUS				,
		// PK_AD_ADDINID             = K_ADDINID,

		PK_AD_DELETED             = K_DELETED,

		PK_AD_DATE_CREATED        = K_DATE_CREATED,
		PK_AD_NAME_CREATED        = K_NAME_CREATED,
		PK_AD_DATE_MODIFIED       = K_DATE_MODIFIED,
		PK_AD_NAME_MODIFIED       = K_NAME_MODIFIED,

		PK_AD_LAST_ID			  ,
		// PK_AD_MODEL_CODE          ,

		PK_SD_SCHEMA_VERSION      = K_SCHEMA_VERSION,
		// PK_SD_WBK_SCHEMA_NAME     = K_WBK_SCHEMA_NAME,
		// PK_SD_SHT_SCHEMA_NAME     = K_SHT_SCHEMA_NAME,
		// PK_SD_SCHEMA_GUID         = K_SCHEMA_GUID,

		PK_MD_MODEL_TITLE          ,
	}

	public enum SheetFieldKeys
	{
		RK_MGMT_FAM_COUNT		  = -1,

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
		
		RK_RD_FAMILY_LIST          ,
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

}
