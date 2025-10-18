#region + Using Directives

#endregion

// user name: jeffs
// created:   10/9/2022 9:07:11 PM



namespace ShExStorageN.ShSchemaFields.ShScSupport
{
	public static class ShExConst
	{
		// public const int K_SCHEMA_NAME	= 0;
		// public const int K_DESCRIPTION	= 1;
		// public const int K_VERSION		= 2;
		// public const int K_MODIFY_DATE	= 3;


		// data storage type
		public const int DT_LOCK	= 0;
		public const int DT_SHEET	= 1;
		public const int DT_ROW		= 2;

		public const string K_NOT_DEFINED = "";
		public const string K_NOT_DEFINED_STR = "<not defined>";
	}

	// public enum SchemaStoreOptions
	// {
	// 	SO_ALL,
	// 	SO_FIELD,
	// 	SO_DATA
	// }

	public enum SchemaFieldDisplayLevel
	{
		DL_DEBUG			= -1,
		DL_BASIC			= 0,
		DL_MEDIUM			= 1,
		DL_ADVANCED			= 2
	}

	public enum CellUpdateRules
	{
		UR_UNDEFINED    = -1,
		UR_NEVER        = 0,
		UR_AS_NEEDED    = 1,
		UR_UPON_REQUEST = 2,
		UR_COUNT        = 3
	}

	public enum ScFieldColumns
	{
							// @ field		@ data
		SFC_KEY,           // y			y
		SFC_NAME,          // y			frm field
		SFC_DESC,          // y			frm field
		SFC_VALUE,         // y			y
		SFC_VALUE_STR,     // derived		derived
		SFC_VALUE_TYPE,    // frm value	frm value
		SFC_REVIT_TYPE,    // frm value	frm value
		SFC_FIELD,         // n			y
		SFC_DISPLAY_LEVEL, // y			frm field

	}


}