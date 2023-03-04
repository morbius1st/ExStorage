#region + Using Directives

#endregion

// user name: jeffs
// created:   10/29/2022 5:22:41 PM

namespace ShExStorageC.ShSchemaFields
{
	// this is project specific
	public class ScDataLock2 : AShScLock2<ScFieldDefData2>
	{

		protected override void init()
		{
			// configure the initial field information
			AScInfoMeta2.ConfigData(Fields, ScInfoMetaLok2.FieldsLock);
		}

		public override void Configure(LokExId lokExid)
		{
			Fields[ShExNTblKeys.TK_SCHEMA_NAME].SetValue = lokExid.SchemaName;
			Fields[ScRowKeys.RK_SEQUENCE].SetValue = lokExid.SchemaName;
			Fields[(lKey) ShExNTblKeys.TK_USERNAME].SetValue = lokExid.UserName;
			Fields[(lKey) ShExNTblKeys.TK_MODEL_NAME].SetValue = AExId.Document.Title;
			Fields[(lKey) ShExNTblKeys.TK_MODEL_PATH].SetValue = AExId.Document.PathName;
			Fields[(lKey) ShExNTblKeys.TK_GUID].SetValue = Guid.NewGuid();
		}

		public override T GetValue<T>(KEY key)
		{
			return Fields[key].GetValueAs<T>();
		}

		public override void ParseEnum(Type t, string enumName) { }

		public bool UserNameMatches(string testName)
		{
			return testName.Equals(UserName);
		}
	}
}