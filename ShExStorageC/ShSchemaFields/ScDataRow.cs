#region + Using Directives
using ShExStorageN.ShSchemaFields;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShSchemaFields.ShScSupport;

#endregion

// user name: jeffs
// created:   10/29/2022 5:22:41 PM

namespace ShExStorageC.ShSchemaFields
{
	/// <summary>
	/// a single row which contains the row fields
	/// </summary>
	public class ScDataRow : AShScRow<SchemaRowKey, ScFieldDefData<SchemaRowKey>>
	{
		public ScDataRow()
		{
			// init();
		}

		protected override void init()
		{
			ScInfoMeta.ConfigData1(Fields, ScInfoMeta.FieldsRow);
		}

	#region from fieldsbase1

		public override string SchemaKey => Fields[SchemaRowKey.RK0_KEY].GetValueAs<string>();
		public override string SchemaVersion => Fields[SchemaRowKey.RK0_VERSION].GetValueAs<string>();
		public override string UserName => Fields[SchemaRowKey.RK0_USER_NAME].GetValueAs<string>();

		public override string SchemaName => Fields[SchemaRowKey.RK0_SCHEMA_NAME].GetValueAs<string>();
		public override string SchemaDesc => Fields[SchemaRowKey.RK0_DESCRIPTION].GetValueAs<string>();
		public override Guid SchemaGuid => Fields[SchemaRowKey.RK0_GUID].DyValue.AsGuid();

		public override string ModelName => Fields[SchemaRowKey.RK2_MODEL_NAME].GetValueAs<string>();
		public override string ModelPath => Fields[SchemaRowKey.RK2_MODEL_PATH].GetValueAs<string>();

		public override string Date => Fields[SchemaRowKey.RK1_MODIFY_DATE].GetValueAs<string>();





	#endregion

		public override void ParseEnum(Type t, string enumName)
		{
			if (t == typeof(CellUpdateRules))
			{
				CellUpdateRules k;
				bool result = Enum.TryParse(enumName, out k);
				if (result)
				{
					Fields[SchemaRowKey.RK2_UPDATE_RULE].SetValue = k;
				}
				else
				{
					Fields[SchemaRowKey.RK2_UPDATE_RULE].SetValue = CellUpdateRules.UR_NEVER;
				}
			}

			// if (t == typeof(SchemaRowKey))
			// {
			// 	SchemaRowKey k;
			// 	bool result = Enum.TryParse(enumName, out k);
			// 	if (result)
			// 	{
			// 		Fields[k].SetValue = k;
			// 	}
			// 	else
			// 	{
			// 		Fields[k].SetValue = SchemaRowKey.RK0_INVALID;
			// 	}
			// }
		}

	}
}
