#region + Using Directives

#endregion

// user name: jeffs
// created:   10/23/2022 9:17:49 PM

namespace ShSchemaFields
{
	public class ScRowFields2 : AShScFields<SchemaRowKey, ScFieldDef<SchemaRowKey>> 
	{
		public ScRowFields2()
		{
			Fields = new Dictionary<SchemaRowKey, ScFieldDef<SchemaRowKey>>();
		}

		public override Dictionary<SchemaRowKey, ScFieldDef<SchemaRowKey>> Fields { get; }


	}
}
