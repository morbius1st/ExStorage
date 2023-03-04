// Solution:     ExStorage
// Project:       ExStoreDev
// File:             ScLokKeys.cs
// Created:      2023-01-11 (8:22 PM)

namespace ShExStorageC.ShSchemaFields.ScSupport
{
	public class ScLokKeys : ShExNTblKeys
	{
		static ScLokKeys()
		{
			KeysLok = new List<lKey>();
			KeysLok.Add(LK_MACHINE_NAME);
		}

		public static readonly lKey LK_MACHINE_NAME   = new lKey(nameof(LK_MACHINE_NAME));

		public static List<lKey> KeysLok { get; }
	}
}