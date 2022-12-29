#region using

#endregion

// username: jeffs
// created:  10/16/2022 3:17:56 PM

namespace ShSchemaFields
{
	public class ScMetaSheet : AShScInfoBase<SchemaSheetKey, ShScFieldDefMeta<SchemaSheetKey>> 
	{
		public override Dictionary<SchemaSheetKey, ShScFieldDefMeta<SchemaSheetKey>> Fields { get; protected set; }

		public override string SchemaName => Fields[SchemaSheetKey.TK0_SCHEMA_NAME].ValueAsString;

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ScMetaSheet)}";
		}

		#endregion
	}
}
