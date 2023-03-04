#region + Using Directives

using System;
using System.Collections.Generic;
using System.Windows.Documents;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExConstN;
#endregion

// user name: jeffs
// created:   10/9/2022 9:07:11 PM



namespace ShExStorageN.ShSchemaFields.ShScSupport
{
	// public interface IKEY
	// {
	// 	string Value { get; }
	// }
	//
	//
	public abstract class KEY //: IEqualityComparer<KEY>
	{
		public string Value { get; }
	
		public KEY(string value) { Value = value; }

		public new int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}
	

	public class tKey : KEY // , IEqualityComparer<tKey>
	{
		public tKey(string value)  : base(value) { }
	}



	public class sKey : KEY // , IEqualityComparer<sKey>
	{
		public sKey(string value) : base(value) { }

		// public static explicit operator sKey(tKey v)
		// { 
		// 	return new sKey(v.Value);
		// }
	}

	public class rKey : KEY //, IComparer<rKey>
	{
		public rKey(string value) : base(value) { }

		// public static explicit operator rKey(tKey v)
		// { 
		// 	return new rKey(v.Value);
		// }

	}

	public class lKey : KEY //, IComparer<lKey>
	{
		public lKey(string value) : base(value) { }
		
		// public static explicit operator lKey(tKey v)
		// { 
		// 	return new lKey(v.Value);
		// }
	}

	
	
	public class KeyEqualityComparer : IEqualityComparer<KEY>
	{
		public bool Equals(KEY x, KEY y)
		{
			return x.Value.Equals(y.Value);
		}

		public int GetHashCode(KEY k)
		{
			return k.Value.GetHashCode();
		}
	}


	public abstract class ShExNTblKeys
	{
		// standard keys used by a table
		// public static readonly tKey TK_INVALID      = new tKey(nameof(TK_INVALID    ));
		public static readonly tKey TK_KEY          = new tKey(nameof(TK_KEY        ));
		public static readonly tKey TK_SCHEMA_NAME  = new tKey(nameof(TK_SCHEMA_NAME));
		public static readonly tKey TK_DESCRIPTION  = new tKey(nameof(TK_DESCRIPTION));
		public static readonly tKey TK_VERSION	    = new tKey(nameof(TK_VERSION    ));
		public static readonly tKey TK_USERNAME	    = new tKey(nameof(TK_USERNAME   ));
		public static readonly tKey TK_GUID		    = new tKey(nameof(TK_GUID       ));
		public static readonly tKey TK_DATE		    = new tKey(nameof(TK_DATE       ));
		public static readonly tKey TK_MODEL_PATH   = new tKey(nameof(TK_MODEL_PATH ));
		public static readonly tKey TK_MODEL_NAME   = new tKey(nameof(TK_MODEL_NAME ));


		// public static List<tKey> KeysTbl = new List<tKey>()
		// {
		// 	{TK_KEY        },
		// 	{TK_SCHEMA_NAME},
		// 	{TK_DESCRIPTION},
		// 	{TK_VERSION    },
		// 	{TK_USERNAME   },
		// 	{TK_GUID       },
		// 	{TK_DATE       },
		// 	{TK_MODEL_PATH },
		// 	{TK_MODEL_NAME }
		// };
	}

	public static class ShExConstN
	{
		// standard / used by all
		public const int K_INVALID     = -1;
		public const int K_KEY         = 0;
		public const int K_SCHEMA_NAME = 1;
		public const int K_DESCRIPTION = 2;
		public const int K_VERSION     = 3;
		public const int K_USERNAME    = 4;
		public const int K_GUID        = 5;
		public const int K_DATE        = 6;
		public const int K_MODEL_PATH  = 7;
		public const int K_MODEL_NAME  = 8;

		// data storage type
		public const int DT_LOCK	= 0;
		public const int DT_SHEET	= 1;
		public const int DT_ROW		= 2;

		public const string K_NOT_DEFINED = "";
		public const string K_NOT_DEFINED_STR = "<not defined>";
	}

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
		SFC_KEY,           // y				y
		SFC_NAME,          // y				frm field
		SFC_DESC,          // y				frm field
		SFC_VALUE,         // y				y
		SFC_VALUE_STR,     // derived			derived
		SFC_VALUE_TYPE,    // frm value		frm value
		SFC_REVIT_TYPE,    // frm value		frm value
		SFC_FIELD,         // n				y
		SFC_DISPLAY_LEVEL, // y				frm field

	}


}