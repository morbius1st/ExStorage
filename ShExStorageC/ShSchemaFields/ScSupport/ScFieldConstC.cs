#region + Using Directives

using System;
using System.Windows.Documents;
using static ShExStorageC.ShSchemaFields.ShScSupport.ShExConstC;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExConstN;
#endregion

// user name: jeffs
// created:   10/15/2022 10:41:41 AM

namespace ShExStorageC.ShSchemaFields.ShScSupport
{
	public static class ShExConstC
	{ 
		// sheet specific
		public const int K_DEVELOPER		= 9;

		// row specific
		public const int K_SEQUENCE          =9;
		public const int K_UPDATE_RULE       =10;
		public const int K_CELL_FAMILY_NAME  =11;
		public const int K_SKIP              =12;
		public const int K_XL_FILE_PATH      =13;
		public const int K_XL_WORKSHEET_NAME =14;

		// lock specific 
		public const int K_MACHINE_NAME      =9;

		public const string KEY_FIELD_NAME = "Key";
		public const string SF_SCHEMA_DESC = "Cells Sheet DataStorage";
		public const string SF_SCHEMA_NAME = "CellsSheetSchema";
		public const string CF_SCHEMA_DESC = "Cells Row DS";
		public const string LF_SCHEMA_DESC = "Cells lock DS";

		public const string MODEL_PATH = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\RevitProjects\ExStorage\.RevitFiles";
		public const string MODEL_NAME = @"HasDataStorage.rvt";

	}

	public enum SchemaSheetKey
	{
		// base level 0 keys
		SK0_KEY               = K_KEY,
		SK0_SCHEMA_NAME       = K_SCHEMA_NAME,
		SK0_DESCRIPTION       = K_DESCRIPTION,
		SK0_VERSION           = K_VERSION,
		SK0_USER_NAME         = K_USERNAME,
		SK0_GUID              = K_GUID,

		// base level 1 keys
		SK1_MODIFY_DATE       = K_DATE,

		// cells specific keys
		SK2_MODEL_PATH        = K_MODEL_PATH,
		SK2_MODEL_NAME        = K_MODEL_NAME,

		SK2_DEVELOPER         = K_DEVELOPER,
	}

	public enum SchemaRowKey
	{
		// base level 0 keys
		RK0_KEY               = K_KEY,
		RK0_SCHEMA_NAME       = K_SCHEMA_NAME,
		RK0_DESCRIPTION       = K_DESCRIPTION,
		RK0_VERSION           = K_VERSION,
		RK0_USER_NAME         = K_USERNAME,
		RK0_GUID              = K_GUID,

		// base level 1 keys
		RK1_MODIFY_DATE       = K_DATE,

		// cells specific keys
		RK2_MODEL_PATH        = K_MODEL_PATH,
		RK2_MODEL_NAME        = K_MODEL_NAME,

		RK2_SEQUENCE          = K_SEQUENCE         ,
		RK2_UPDATE_RULE       = K_UPDATE_RULE      ,
		RK2_CELL_FAMILY_NAME  = K_CELL_FAMILY_NAME ,
		RK2_SKIP              = K_SKIP             ,
		RK2_XL_FILE_PATH      = K_XL_FILE_PATH     ,
		RK2_XL_WORKSHEET_NAME = K_XL_WORKSHEET_NAME
	}

	public enum SchemaLockKey
	{
		// base level 0 keys
		LK0_KEY               = K_KEY,
		LK0_SCHEMA_NAME       = K_SCHEMA_NAME,
		LK0_DESCRIPTION       = K_DESCRIPTION,
		LK0_VERSION           = K_VERSION,
		LK0_USER_NAME         = K_USERNAME,
		LK0_GUID              = K_GUID,

		// base level 1 keys
		LK1_CREATE_DATE       = K_DATE,

		// cells specific keys
		LK2_MODEL_PATH        = K_MODEL_PATH,
		LK2_MODEL_NAME        = K_MODEL_NAME,

		LK2_MACHINE_NAME      = K_MACHINE_NAME
	}

}