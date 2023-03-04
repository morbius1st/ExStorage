#region + Using Directives
using ShExStorageN.ShSchemaFields;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using ShExStorageC.ShSchemaFields.ShScSupport;

#endregion

// user name: jeffs
// created:   10/29/2022 5:41:28 PM

namespace ShExStorageC.ShSchemaFields
{
	public class ScDataSheet1 : 
		AShScSheet< SchemaSheetKey, 
		ScFieldDefData1<SchemaSheetKey>,
		SchemaRowKey,
		ScFieldDefData1<SchemaRowKey>,
		ScDataRow1
		>
	{
		public ScDataSheet1()
		{
			// Fields = new Dictionary<SchemaSheetKey, ScFieldDefData1<SchemaSheetKey>>(12);
			Rows = new Dictionary<string, ScDataRow1>(1);

			init();
		}

		private void init()
		{
			// configure the initial field information
			ScInfoMeta1.ConfigData1(Fields, ScInfoMeta1.FieldsSheet);
		}

	#region from fieldbase1

		public override string SchemaName => Fields[SchemaSheetKey.SK0_SCHEMA_NAME].GetValueAs<string>();
		public override string SchemaDesc => Fields[SchemaSheetKey.SK0_DESCRIPTION].GetValueAs<string>();
		public override Guid SchemaGuid => Fields[SchemaSheetKey.SK0_GUID].DyValue.AsGuid();

	#endregion

	#region from rows1

		public override void AddRow(ScDataRow1 row)
		{
			string key = row.Fields[SchemaRowKey.RK0_SCHEMA_NAME].GetValueAs<string>();

			Rows.Add(key, row);
		}

	#endregion

	#region from sheet1

		public IShScFieldMeta1<SchemaSheetKey> Meta1Field => Fields[SchemaSheetKey.SK0_KEY].Meta1Field;

	#endregion

		public override void ParseEnum(Type t, string enumName)
		{
			// if (t == typeof(SchemaSheetKey))
			// {
			// 	SchemaSheetKey k;
			// 	bool result = Enum.TryParse(enumName, out k);
			// 	if (result)
			// 	{
			// 		Fields[k].SetValue = k;
			// 	}
			// 	else
			// 	{
			// 		Fields[k].SetValue = SchemaSheetKey.TK0_INVALID;
			// 	}
			// }
		}

	}
}
