#region using

#endregion

// username: jeffs
// created:  10/16/2022 3:17:56 PM


namespace ShSchemaFields
{
	public class ScDataRow1 : AShScInfoBase<SchemaRowKey, ShScFieldDefData<SchemaRowKey>>
	{
		public ScDataRow1()
		{
			init();

			Families = new Dictionary<string, string>();
		}

		public override string SchemaName => Fields[SchemaRowKey.RK0_SCHEMA_NAME].ValueAsString;

		public override Dictionary<SchemaRowKey, ShScFieldDefData<SchemaRowKey>> Fields { get; protected set; }

		/// <summary>
		/// the list of families associated with this row
		/// </summary>
		public Dictionary<string, string> Families { get; set; }


		public void init()
		{
			ScInfoMeta1.ConfigData(Fields, ScInfoMeta1.FieldsRow);

		}

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ScDataRow)}";
		}

	#endregion
	}
}