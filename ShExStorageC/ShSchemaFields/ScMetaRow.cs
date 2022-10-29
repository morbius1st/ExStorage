#region using

using System.Collections.Generic;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShSchemaFields;


#endregion

// username: jeffs
// created:  10/16/2022 3:17:56 PM

namespace ShExStorageC.ShSchemaFields
{
	public class ScMetaRow : AShScInfoBase<SchemaRowKey, ShScFieldDefMeta<SchemaRowKey>>
	{

		public override Dictionary<SchemaRowKey, ShScFieldDefMeta<SchemaRowKey>> Fields { get; protected set; }

		public override string SchemaName => Fields[SchemaRowKey.CK0_SCHEMA_NAME].ValueAsString;

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ScMetaRow)}";
		}

	#endregion
	}
}