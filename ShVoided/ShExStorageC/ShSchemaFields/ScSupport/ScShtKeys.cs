// Solution:     ExStorage
// Project:       ExStoreDev
// File:             ScShtKeys.cs
// Created:      2023-01-11 (8:22 PM)

namespace ShExStorageC.ShSchemaFields.ScSupport
{
	public class ScShtKeys : ShExNTblKeys
	{
		static ScShtKeys()
		{
			KeysSht = new List<sKey>();

			KeysSht.Add(SK_DEVELOPER);
			
			KeysSht.Add(STK_KEY );
			KeysSht.Add(STK_SCHEMA_NAME);
			KeysSht.Add(STK_DESCRIPTION);
			KeysSht.Add(STK_VERSION );
			KeysSht.Add(STK_USERNAME );
			KeysSht.Add(STK_GUID );
			KeysSht.Add(STK_DATE );
			KeysSht.Add(STK_MODEL_PATH );
			KeysSht.Add(STK_MODEL_NAME );




		}

		public static readonly sKey SK_DEVELOPER    = new sKey(nameof(SK_DEVELOPER  ));

		public static readonly sKey STK_KEY         = new sKey(nameof(TK_KEY        ));
		public static readonly sKey STK_SCHEMA_NAME = new sKey(nameof(TK_SCHEMA_NAME));
		public static readonly sKey STK_DESCRIPTION = new sKey(nameof(TK_DESCRIPTION));
		public static readonly sKey STK_VERSION     = new sKey(nameof(TK_VERSION    ));
		public static readonly sKey STK_USERNAME    = new sKey(nameof(TK_USERNAME   ));
		public static readonly sKey STK_GUID        = new sKey(nameof(TK_GUID       ));
		public static readonly sKey STK_DATE        = new sKey(nameof(TK_DATE       ));
		public static readonly sKey STK_MODEL_PATH  = new sKey(nameof(TK_MODEL_PATH ));
		public static readonly sKey STK_MODEL_NAME  = new sKey(nameof(TK_MODEL_NAME ));



		public static List<sKey> KeysSht { get; }


	}
}