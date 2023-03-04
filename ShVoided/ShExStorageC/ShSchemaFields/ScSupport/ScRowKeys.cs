// Solution:     ExStorage
// Project:       ExStoreDev
// File:             ScRowKeys.cs
// Created:      2023-01-11 (8:22 PM)

namespace ShExStorageC.ShSchemaFields.ScSupport
{
	public class ScRowKeys : ShExNTblKeys
	{
		static ScRowKeys()
		{
			KeysRow = new List<rKey>();
			KeysRow.Add(RK_SEQUENCE         );
			KeysRow.Add(RK_UPDATE_RULE      );
			KeysRow.Add(RK_CELL_FAMILY_NAME );
			KeysRow.Add(RK_SKIP             );
			KeysRow.Add(RK_XL_FILE_PATH     );
			KeysRow.Add(RK_XL_WORKSHEET_NAME);
		}

		public static readonly rKey RK_SEQUENCE            = new rKey(nameof(RK_SEQUENCE         ));
		public static readonly rKey RK_UPDATE_RULE         = new rKey(nameof(RK_UPDATE_RULE      ));
		public static readonly rKey RK_CELL_FAMILY_NAME    = new rKey(nameof(RK_CELL_FAMILY_NAME ));
		public static readonly rKey RK_SKIP                = new rKey(nameof(RK_SKIP             ));
		public static readonly rKey RK_XL_FILE_PATH        = new rKey(nameof(RK_XL_FILE_PATH     ));
		public static readonly rKey RK_XL_WORKSHEET_NAME   = new rKey(nameof(RK_XL_WORKSHEET_NAME));


		public static List<rKey> KeysRow { get; }
	}
}