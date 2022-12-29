#region using

using static ShExStorageN.ShSchemaFields.ScFieldColumns;
using static ShExStorageC.ShSchemaFields1.SchemaLockKey;
using static ShExStorageC.ShSchemaFields1.SchemaSheetKey;
using static ShExStorageC.ShSchemaFields1.SchemaRowKey;
using static ShExStorageN.ShSchemaFields.SchemaFieldDisplayLevel;


#endregion

// username: jeffs
// created:  10/10/2022 7:36:03 PM

namespace ShSCFields
{
	public abstract class ShScFields<TKey> where TKey : Enum
	{
		public const string MODEL_PATH = @"C:\Users\jeffs\Documents\Programming\VisualStudioProjects\RevitProjects\ExStorage\.RevitFiles";
		public const string MODEL_NAME = @"HasDataStorage.rvt";

	#region private fields

		protected Dictionary<TKey, ScFieldDef<TKey>> fields;

	#endregion

	#region ctor

		public ShScFields()
		{
			fields = new Dictionary<TKey, ScFieldDef<TKey>>();
		}

	#endregion

	#region public properties

		public List<Dictionary<ScFieldColumns, string>> ScFieldsValues { get; protected set; }

	#endregion

	#region private properties

	#endregion

	#region public methods

		public void Add<TD>(TKey key,
			string name,
			string description,
			TD value,
			SchemaFieldDisplayLevel displayLevel)
		{
			fields.Add(key,
				new ScFieldDef<TKey>(
					key,
					name,
					description,
					new DynaValue(value),
					displayLevel));
		}

		/// <summary>
		/// create the list of values per a field's fields<br/>
		/// this is used to display the values in the fields
		/// </summary>
		protected void setFieldsValues()
		{
			ScFieldsValues = new List<Dictionary<ScFieldColumns, string>>();

			foreach (KeyValuePair<TKey, ScFieldDef<TKey>> kvp in fields)
			{
				ScFieldDef<TKey> lf = kvp.Value;

				Dictionary<ScFieldColumns, string> f = new Dictionary<ScFieldColumns, string>();

				f.Add(SFC_KEY,           lf.Key.ToString());
				f.Add(SFC_NAME,          lf.Name);
				f.Add(SFC_DESC,          lf.Description);
				f.Add(SFC_VALUETYPE,     lf.Value.TypeIs.Name);
				f.Add(SFC_VALUE,         lf.Value.AsString());
				f.Add(SFC_DISPLAY_LEVEL, lf.DisplayLevel.ToString());

				ScFieldsValues.Add(f);
			}
		}



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
			return "this is ScFields";
		}

	#endregion
	}
}