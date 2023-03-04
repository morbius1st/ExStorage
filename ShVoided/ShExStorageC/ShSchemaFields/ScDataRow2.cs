#region + Using Directives

using static ShExStorageC.ShSchemaFields.ShScSupport.ShExConstC;
#endregion

// user name: jeffs
// created:   10/29/2022 5:22:41 PM

namespace ShExStorageC.ShSchemaFields
{
	/// <summary>
	/// a single row which contains the row fields
	/// </summary>
	public class ScDataRow2 : AShScRow2<ScFieldDefData2>
	{

		protected override void init()
		{
			AScInfoMeta2.ConfigData(Fields, ScInfoMetaRow2.FieldsRow);
		}

	#region from fieldsbase1

	#endregion


		public override T GetValue<T>(KEY key)
		{
			return Fields[key].GetValueAs<T>();
		}

		public override void ParseEnum(Type t, string enumName)
		{
			if (t == typeof(CellUpdateRules))
			{
				CellUpdateRules k;
				bool result = Enum.TryParse(enumName, out k);
				if (result)
				{
					Fields[ScRowKeys.RK_UPDATE_RULE].SetValue = k;
				}
				else
				{
					Fields[ScRowKeys.RK_UPDATE_RULE].SetValue = CellUpdateRules.UR_NEVER;
				}
			}
		}

	}
}
