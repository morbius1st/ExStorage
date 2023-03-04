#region + Using Directives
using ShExStorageN.ShSchemaFields;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Autodesk.Revit.DB.ExtensibleStorage;
using ShExStorageC.ShSchemaFields.ShScSupport;
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

		protected override void init()
		{
			ScInfoMeta.ConfigData(Fields, ScInfoMeta.MetaFieldsRow);
		}

		public override T GetValue<T>(int key)
		{
			return Fields[(SchemaRowKey) key].GetValueAs<T>();
		}

		public override T GetValue<T>(SchemaRowKey key)
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
					Fields[SchemaRowKey.RK2_UPDATE_RULE].SetValue = k;
				}
				else
				{
					Fields[SchemaRowKey.RK2_UPDATE_RULE].SetValue = CellUpdateRules.UR_NEVER;
				}
			}
		}

		// protected override AShScRow<SchemaRowKey, ScFieldDefData<SchemaRowKey>> NewForClone()
		// {
		// 	return new ScDataRow();
		// }

		public override void SetValue(int key, dynamic value)
		{
			SetValue((SchemaRowKey) key, value);

			SetValue(SchemaRowKey.RK1_MODIFY_DATE, DateTime.Now);
			SetValue(SchemaRowKey.RK0_USER_NAME, UtilityLibrary.CsUtilities.UserName);
		}

		protected override AShScRow<SchemaRowKey, ScFieldDefData<SchemaRowKey>> NewForClone()
		{
			return new ScDataRow();
		}
	}
}
