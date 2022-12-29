#region + Using Directives

using ShExStorageN.ShSchemaFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShSchemaFields.ShScSupport;

#endregion

// user name: jeffs
// created:   10/29/2022 5:41:28 PM

namespace ShExStorageC.ShSchemaFields
{
	/*
	fields

	key
	name
	desc
	ver
	mod path
	mod name
	dev
	user
	date
	guid

	*/

	public class ScDataSheet :
		AShScSheet< SchemaSheetKey,
		ScFieldDefData<SchemaSheetKey>,
		SchemaRowKey,
		ScFieldDefData<SchemaRowKey>,
		ScDataRow
		>
	{
		public ScDataSheet()
		{
			// Fields = new Dictionary<SchemaSheetKey, ScFieldDefData1<SchemaSheetKey>>(12);
			Rows = new Dictionary<string, ScDataRow>(1);

			init();
		}

		private void init()
		{
			// configure the initial field information
			ScInfoMeta.ConfigData1(Fields, ScInfoMeta.FieldsSheet);

			Fields[SchemaSheetKey.SK0_SCHEMA_NAME].SetValue = ScInfoMeta.SF_SCHEMA_NAME;
		}

	#region from fieldbase

		public override string SchemaKey => Fields[SchemaSheetKey.SK0_KEY].GetValueAs<string>();
		public override string SchemaVersion => Fields[SchemaSheetKey.SK0_VERSION].GetValueAs<string>();
		public override string UserName => Fields[SchemaSheetKey.SK0_USER_NAME].GetValueAs<string>();

		public override string SchemaName => Fields[SchemaSheetKey.SK0_SCHEMA_NAME].GetValueAs<string>();
		public override string SchemaDesc => Fields[SchemaSheetKey.SK0_DESCRIPTION].GetValueAs<string>();
		public override Guid SchemaGuid => Fields[SchemaSheetKey.SK0_GUID].DyValue.AsGuid();

		public override string ModelName => Fields[SchemaSheetKey.SK2_MODEL_NAME].GetValueAs<string>();
		public override string ModelPath => Fields[SchemaSheetKey.SK2_MODEL_PATH].GetValueAs<string>();

		public override string Date => Fields[SchemaSheetKey.SK1_MODIFY_DATE].GetValueAs<string>();

	#endregion

	#region from rows1

		public override void AddRow(ScDataRow row)
		{
			string key = row.Fields[SchemaRowKey.RK0_SCHEMA_NAME].GetValueAs<string>();

			Rows.Add(key, row);
		}

	#endregion

	#region from sheet

		public IShScFieldMeta1<SchemaSheetKey> Meta1Field => Fields[SchemaSheetKey.SK0_KEY].Meta1Field;

	#endregion

		public override void ParseEnum(Type t, string enumName)
		{
			// na for this structure
		}
	}
}