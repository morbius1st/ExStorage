#region using

#endregion

// username: jeffs
// created:  10/16/2022 3:17:56 PM

namespace ShSchemaFields
{
	public class ScMetaRow : AShScInfoBase<SchemaRowKey, ShScFieldDefMeta<SchemaRowKey>>
	{

		public override Dictionary<SchemaRowKey, ShScFieldDefMeta<SchemaRowKey>> Fields { get; protected set; }

		public override string SchemaName => Fields[SchemaRowKey.RK0_SCHEMA_NAME].ValueAsString;

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ScMetaRow)}";
		}

	#endregion
	}
}