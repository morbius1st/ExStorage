#region + Using Directives

#endregion

// user name: jeffs
// created:   1/1/2023 8:06:39 AM

namespace ShExStorageC.ShSchemaFields.ScSupport
{
	public abstract class AScInfoMeta2
	{
		/// <summary>
		/// add all of the meta fields to a data field using default information
		/// </summary>
		public static void 
			ConfigData(Dictionary<KEY, ScFieldDefData2> fields, Dictionary<KEY, ScFieldDefMeta2> meta)
		{
			foreach (KeyValuePair<KEY,  ScFieldDefMeta2> kvp in meta)
			{
				fields.Add(kvp.Key,
					new ScFieldDefData2(kvp.Value.FieldKey, new DynaValue(kvp.Value.DyValue.Value), kvp.Value));
			}
		}

		public override string ToString()
		{
			return $"this is {nameof(AScInfoMeta2)}";
		}
	}
}
