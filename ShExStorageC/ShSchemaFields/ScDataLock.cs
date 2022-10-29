#region using

using System.Collections.Generic;
using ShExStorageC.ShSchemaFields.ScSupport;

using ShExStorageN.ShSchemaFields;

#endregion

// username: jeffs
// created:  10/16/2022 3:17:56 PM


namespace ShExStorageC.ShSchemaFields
{
	public class ScDataLock : AShScInfoBase<SchemaLockKey, ShScFieldDefData<SchemaLockKey>>
	{
		public ScDataLock()
		{
			init();
		}

		public override Dictionary<SchemaLockKey, ShScFieldDefData<SchemaLockKey>> Fields { get; protected set; }

		public override string SchemaName => Fields[SchemaLockKey.LK0_SCHEMA_NAME].ValueAsString;

		public void init()
		{
			ScInfoMeta.ConfigData(Fields, ScInfoMeta.FieldsLock);

		}

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ScDataLock)}";
		}

	#endregion
	}
}