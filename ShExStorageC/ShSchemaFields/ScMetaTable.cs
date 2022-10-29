#region using

using System.Collections.Generic;
using ShExStorageN.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ScSupport;

#endregion

// username: jeffs
// created:  10/16/2022 3:17:56 PM

namespace ShExStorageC.ShSchemaFields
{
	public class ScMetaTable : AShScInfoBase<SchemaTableKey, ShScFieldDefMeta<SchemaTableKey>> 
	{
		public override Dictionary<SchemaTableKey, ShScFieldDefMeta<SchemaTableKey>> Fields { get; protected set; }

		public override string SchemaName => Fields[SchemaTableKey.SK0_SCHEMA_NAME].ValueAsString;

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ScMetaTable)}";
		}

		#endregion
	}
}
