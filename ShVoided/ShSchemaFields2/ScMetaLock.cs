#region using

#endregion

// username: jeffs
// created:  10/16/2022 3:17:56 PM

namespace ShSchemaFields
{
	public class ScMetaLock  : AShScInfoBase<SchemaLockKey, ShScFieldDefMeta<SchemaLockKey>>
	{

		public override Dictionary<SchemaLockKey, ShScFieldDefMeta<SchemaLockKey>> Fields { get; protected set; }

		public override string SchemaName => Fields[SchemaLockKey.LK0_SCHEMA_NAME].ValueAsString;

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ScMetaLock)}";
		}

	#endregion
	}
}