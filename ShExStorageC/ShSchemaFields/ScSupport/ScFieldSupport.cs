#region + Using Directives
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaFieldKey;
using static ShExStorageC.ShSchemaFields.ScSupport.ShExConst;
#endregion

// user name: jeffs
// created:   10/15/2022 10:41:41 AM

namespace ShExStorageC.ShSchemaFields.ScSupport
{
	public static class ShExConst
	{

		public const int K_KEY			= 0;
		public const int K_SCHEMA_NAME	= 1;
		public const int K_DESCRIPTION	= 2;
		public const int K_VERSION		= 3;
		public const int K_USERNAME		= 4;
		public const int K_GUID		    = 5;
		public const int K_DATE		    = 6;
		public const int K_MODEL_PATH	= 7;
		public const int K_MODEL_NAME	= 8;
		public const int K_LOCK_STATUS	= 9;
	}



	public enum SchemaFieldKey
	{
		FK0_INVALID           = -1 ,
		FK0_KEY               = ShExConst.K_KEY,
		FK0_SCHEMA_NAME       = ShExConst.K_SCHEMA_NAME,
		FK0_DESCRIPTION       = ShExConst.K_DESCRIPTION,
		FK0_VERSION           = ShExConst.K_VERSION    ,
		FK0_USERNAME          = ShExConst.K_USERNAME   ,
		FK0_GUID              = ShExConst.K_GUID       ,
		FK1_DATE              = ShExConst.K_DATE       ,
		FK2_MODEL_PATH        = ShExConst.K_MODEL_PATH , 
		FK2_MODEL_NAME        = ShExConst.K_MODEL_NAME ,

	}

	public enum SchemaSheetKey
	{
		// base level 0 keys
		SK0_KEY               = FK0_KEY,
		SK0_SCHEMA_NAME       = FK0_SCHEMA_NAME,
		SK0_DESCRIPTION       = FK0_DESCRIPTION,
		SK0_VERSION           = FK0_VERSION,
		SK0_USER_NAME         = FK0_USERNAME,
		SK0_GUID              = FK0_GUID,

		// base level 1 keys
		SK1_MODIFY_DATE       = FK1_DATE,

		// cells specific keys
		SK2_MODEL_PATH        = FK2_MODEL_PATH,
		SK2_MODEL_NAME        = FK2_MODEL_NAME,

		SK2_DEVELOPER         ,

		SK0_INVALID           = FK0_INVALID 
	}

	public enum SchemaRowKey
	{
		// base level 0 keys
		RK0_KEY               = FK0_KEY,
		RK0_SCHEMA_NAME       = FK0_SCHEMA_NAME,
		RK0_DESCRIPTION       = FK0_DESCRIPTION,
		RK0_VERSION           = FK0_VERSION,
		RK0_USER_NAME         = FK0_USERNAME,
		RK0_GUID              = FK0_GUID,

		// base level 1 keys
		RK1_MODIFY_DATE       = FK1_DATE,

		// cells specific keys
		RK2_MODEL_PATH        = FK2_MODEL_PATH,
		RK2_MODEL_NAME        = FK2_MODEL_NAME,

		RK2_SEQUENCE          ,
		RK2_UPDATE_RULE       ,
		RK2_CELL_FAMILY_NAME  ,
		RK2_SKIP              ,
		RK2_XL_FILE_PATH      ,
		RK2_XL_WORKSHEET_NAME ,

		RK0_INVALID           = FK0_INVALID 
	}

	public enum SchemaLockKey
	{
		// base level 0 keys
		LK0_KEY               = FK0_KEY,
		LK0_SCHEMA_NAME       = FK0_SCHEMA_NAME,
		LK0_DESCRIPTION       = FK0_DESCRIPTION,
		LK0_VERSION           = FK0_VERSION,
		LK0_USER_NAME         = FK0_USERNAME,
		LK0_GUID              = FK0_GUID,

		// base level 1 keys
		LK1_CREATE_DATE       = FK1_DATE,

		// cells specific keys
		LK2_MODEL_PATH        = FK2_MODEL_PATH,
		LK2_MODEL_NAME        = FK2_MODEL_NAME,

		LK2_MACHINE_NAME      ,

		LK0_INVALID           = FK0_INVALID 

	}

}