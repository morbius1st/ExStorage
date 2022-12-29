#region using

#endregion

// username: jeffs
// created:  10/16/2022 3:17:56 PM


namespace ShSchemaFields
{
	public class ScDataLock1 : AShScInfoBase<SchemaLockKey, ShScFieldDefData<SchemaLockKey>>
	{
		public ScDataLock1()
		{
			init();
		}

		public override Dictionary<SchemaLockKey, ShScFieldDefData<SchemaLockKey>> Fields { get; protected set; }

		public override string SchemaName => Fields[SchemaLockKey.LK0_SCHEMA_NAME].ValueAsString;

		public void init()
		{
			ScInfoMeta1.ConfigData(Fields, ScInfoMeta1.FieldsLock);

		}

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ScDataLock)}";
		}

	#endregion
	}
}