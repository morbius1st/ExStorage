#region using

using System.Collections.Generic;
using ShExStorageC.ShSchemaFields.ScSupport;

using ShExStorageN.ShSchemaFields;

#endregion

// username: jeffs
// created:  10/16/2022 3:17:56 PM


namespace ShExStorageC.ShSchemaFields
{
	public class ScDataRow : AShScInfoBase<SchemaRowKey, ShScFieldDefData<SchemaRowKey>>
	{
		public ScDataRow()
		{
			init();

			Families = new Dictionary<string, string>();
		}

		public override string SchemaName => Fields[SchemaRowKey.CK0_SCHEMA_NAME].ValueAsString;

		public override Dictionary<SchemaRowKey, ShScFieldDefData<SchemaRowKey>> Fields { get; protected set; }

		/// <summary>
		/// the list of families associated with this row
		/// </summary>
		public Dictionary<string, string> Families { get; set; }


		public void init()
		{
			ScInfoMeta.ConfigData(Fields, ScInfoMeta.FieldsRow);

		}

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ScDataRow)}";
		}

	#endregion
	}
}