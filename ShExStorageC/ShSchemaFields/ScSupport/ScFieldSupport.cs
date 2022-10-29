#region + Using Directives

#endregion

// user name: jeffs
// created:   10/15/2022 10:41:41 AM

namespace ShExStorageC.ShSchemaFields.ScSupport
{
	public static class ShExConst
	{
		// public const int K_VALUE		= 3;
		public const int K_KEY			= 0;
		public const int K_SCHEMA_NAME	= 1;
		public const int K_DESCRIPTION	= 2;
		public const int K_VERSION		= 3;
		public const int K_MODEL_PATH	= 4;
		public const int K_MODEL_NAME	= 5;
	}

	public enum SchemaLockKey
	{
		// LK_VALUE             = ShExConst.K_VALUE,
		LK0_KEY               = ShExConst.K_KEY,
		LK0_SCHEMA_NAME       = ShExConst.K_SCHEMA_NAME,
		LK0_DESCRIPTION       = ShExConst.K_DESCRIPTION,
		LK0_VERSION           = ShExConst.K_VERSION    ,
		LK0_MODEL_PATH        = ShExConst.K_MODEL_PATH,
		LK0_MODEL_NAME        = ShExConst.K_MODEL_NAME,

		LK1_USER_NAME         ,
		LK1_CREATE_DATE       ,

		LK1_MACHINE_NAME      ,

		LK9_GUID              ,


	}

	public enum SchemaTableKey
	{

		// TK_VALUE             = ShExConst.K_VALUE,
		SK0_KEY               = ShExConst.K_KEY,
		SK0_SCHEMA_NAME       = ShExConst.K_SCHEMA_NAME,
		SK0_DESCRIPTION       = ShExConst.K_DESCRIPTION,
		SK0_VERSION           = ShExConst.K_VERSION    ,
		SK0_MODEL_PATH        = ShExConst.K_MODEL_PATH,
		SK0_MODEL_NAME        = ShExConst.K_MODEL_NAME,

		SK1_DEVELOPER        ,

		
		SK2_USER_NAME        ,
		SK2_MODIFY_DATE      ,

		SK9_GUID             ,
	}

	public enum SchemaRowKey
	{
		// RK_VALUE             = ShExConst.K_VALUE,
		CK0_KEY               = ShExConst.K_KEY,
		CK0_SCHEMA_NAME       = ShExConst.K_SCHEMA_NAME,
		CK0_DESCRIPTION       = ShExConst.K_DESCRIPTION,
		CK0_VERSION           = ShExConst.K_VERSION    ,
		CK0_MODEL_PATH        = ShExConst.K_MODEL_PATH,
		CK0_MODEL_NAME        = ShExConst.K_MODEL_NAME,


		CK2_USER_NAME         ,
		CK2_MODIFY_DATE       ,

		CK9_SEQUENCE          ,
		CK9_UPDATE_RULE       ,
		CK9_ROW_FAMILY_NAME  ,
		CK9_SKIP              ,
		CK9_XL_FILE_PATH      ,
		CK9_XL_WORKSHEET_NAME ,

		CK9_GUID              ,
	}
}