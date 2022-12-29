#region + Using Directives

using ShExStorageN.ShSchemaFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Autodesk.Revit.DB.ExtensibleStorage;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShExStorage;


#endregion

// user name: jeffs
// created:   10/29/2022 5:22:41 PM

namespace ShExStorageC.ShSchemaFields
{
	public class ScDataLock : AShScLock<SchemaLockKey, ScFieldDefData<SchemaLockKey>>
	{
		protected override void init()
		{
			// configure the initial field information
			ScInfoMeta.ConfigData1(Fields, ScInfoMeta.FieldsLock);
		}

		public override void Configure(LokExId lokExid)
		{
			Fields[SchemaLockKey.LK0_SCHEMA_NAME].SetValue = lokExid.SchemaName;
			Fields[SchemaLockKey.LK0_USER_NAME].SetValue = lokExid.UserName;
			Fields[SchemaLockKey.LK2_MODEL_NAME].SetValue = AExId.Document.Title;
			Fields[SchemaLockKey.LK2_MODEL_PATH].SetValue = AExId.Document.PathName;
			Fields[SchemaLockKey.LK0_GUID].SetValue = Guid.NewGuid();
		}

		public override string SchemaKey => Fields[SchemaLockKey.LK0_KEY].GetValueAs<string>();
		public override string SchemaVersion => Fields[SchemaLockKey.LK0_VERSION].GetValueAs<string>();
		public override string UserName => Fields[SchemaLockKey.LK0_USER_NAME].GetValueAs<string>();

		public override string SchemaName => Fields[SchemaLockKey.LK0_SCHEMA_NAME].GetValueAs<string>();
		public override string SchemaDesc => Fields[SchemaLockKey.LK0_DESCRIPTION].GetValueAs<string>();
		public override Guid SchemaGuid => Fields[SchemaLockKey.LK0_GUID].DyValue.AsGuid();

		public override string ModelName => Fields[SchemaLockKey.LK2_MODEL_NAME].GetValueAs<string>();
		public override string ModelPath => Fields[SchemaLockKey.LK2_MODEL_PATH].GetValueAs<string>();
		public override string MachineName => Fields[SchemaLockKey.LK2_MACHINE_NAME].GetValueAs<string>();

		public override string Date => Fields[SchemaLockKey.LK1_CREATE_DATE].GetValueAs<string>();

		public override void ParseEnum(Type t, string enumName) { }

		public bool UserNameMatches(string testName)
		{
			return testName.Equals(UserName);
		}
	}
}