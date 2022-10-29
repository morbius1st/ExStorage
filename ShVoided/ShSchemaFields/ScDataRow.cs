#region using

#endregion

// username: jeffs
// created:  10/16/2022 3:17:56 PM

namespace ShSchemaFields
{
	public class ScDataRow2 : ScFieldRowBase2
	{
		#region private fields



		#endregion

		#region ctor

		public ScDataRow2() { }

		#endregion

		#region public properties

			public Dictionary<SchemaRowKey, ScDataDef2<SchemaRowKey>> RowData => data;

		#endregion

		#region private properties



		#endregion

		#region public methods



		#endregion

		#region private methods



		#endregion

		#region event consuming



		#endregion

		#region event publishing



		#endregion

		#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ScDataRow2)}";
		}

		#endregion
	}
}
