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

#endregion

// user name: jeffs
// created:   10/29/2022 5:22:41 PM

namespace ShExStorageC.ShSchemaFields
{
	public class ScDataLock1 : AShScFields<SchemaLockKey, ScFieldDefData1<SchemaLockKey>>
	{
		public ScDataLock1()
		{
			// Fields = new Dictionary<SchemaLockKey, ScFieldDefData1<SchemaLockKey>>();

			init();
		}

		private void init()
		{
			// configure the initial field information
			// ScInfoMeta1.ConfigData1(Fields, ScInfoMeta1.FieldsLock);
		}

		public override string SchemaName  => Fields[SchemaLockKey.LK0_SCHEMA_NAME].GetValueAs<string>();
		public override string SchemaDesc => Fields[SchemaLockKey.LK0_DESCRIPTION].GetValueAs<string>();
		public override Guid SchemaGuid => Fields[SchemaLockKey.LK0_GUID].DyValue.AsGuid();

		public override void ParseEnum(Type t, string enumName)
		{
			// if (t == typeof(SchemaLockKey))
			// {
			// 	SchemaLockKey k;
			// 	bool result = Enum.TryParse(enumName, out k);
			// 	if (result)
			// 	{
			// 		Fields[k].SetValue = k;
			// 	}
			// 	else
			// 	{
			// 		Fields[k].SetValue = SchemaLockKey.LK0_INVALID;
			// 	}
			// }
		}
	}


}
