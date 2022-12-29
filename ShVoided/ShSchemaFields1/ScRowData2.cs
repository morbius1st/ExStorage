#region + Using Directives

#endregion

// user name: jeffs
// created:   10/23/2022 9:12:42 PM

namespace ShSchemaFields
{
	public class ScRowData2 : AShScData<SchemaRowKey, ScDataDef<SchemaRowKey>, ScFieldDef<SchemaRowKey>> 
	{

		public ScRowData2()
		{
			Fields = new Dictionary<SchemaRowKey, ScDataDef<SchemaRowKey>>();
		}

		public override Dictionary<SchemaRowKey, ScDataDef<SchemaRowKey>> Fields { get; }
		
	}
}
