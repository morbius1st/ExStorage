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
using ShExStorageN.ShExStorage;


#endregion

// user name: jeffs
// created:   10/29/2022 5:22:41 PM

namespace ShExStorageC.ShSchemaFields
{
	// this is project specific
	public class ScDataLock : AShScLock<SchemaLockKey, ScFieldDefData<SchemaLockKey>>
	{

		protected override void init()
		{
			// configure the initial field information
			ScInfoMeta.ConfigData(Fields, ScInfoMeta.MetaFieldsLock);
		}

		public override void Configure(LokExId lokExid)
		{
			Fields[SchemaLockKey.LK0_SCHEMA_NAME].SetValue = lokExid.SchemaName;
			Fields[SchemaLockKey.LK0_USER_NAME].SetValue = lokExid.UserName;
			Fields[SchemaLockKey.LK2_MODEL_NAME].SetValue = AExId.Document.Title;
			Fields[SchemaLockKey.LK2_MODEL_PATH].SetValue = AExId.Document.PathName;
			Fields[SchemaLockKey.LK0_GUID].SetValue = Guid.NewGuid();
		}

		public override T GetValue<T>(int key)
		{
			return Fields[(SchemaLockKey) key].GetValueAs<T>();
		}

		public override T GetValue<T>(SchemaLockKey key)
		{
			return Fields[key].GetValueAs<T>();
		}

		public override void ParseEnum(Type t, string enumName) { }

		public bool UserNameMatches(string testName)
		{
			return testName.Equals(UserName);
		}

		public override void SetValue(int key, dynamic value)
		{
			SetValue((SchemaLockKey) key, value);
		}
	}
}