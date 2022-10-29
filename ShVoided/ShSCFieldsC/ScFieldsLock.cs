#region using

using static ShExStorageC.ShSchemaFields.SchemaLockKey;
using static ShExStorageN.ShSchemaFields.SchemaFieldDisplayLevel;

#endregion

// username: jeffs
// created:  10/10/2022 11:30:35 PM

namespace ShSCFieldsC
{
	public class ScFieldsLock :  ShScFields<SchemaLockKey>
	{
		public const string LF_SCHEMA_NAME = "Rows>Schema>Fields>Lock";
		public const string LF_SCHEMA_DESC = "Rows Lock DS";

		public ScFieldsLock()
		{
			init();
		}

	#region public properties

		public  Dictionary<SchemaLockKey, ScFieldDef<SchemaLockKey>> LockFields => fields;

		public string SchemaName => fields[LK_SCHEMA_NAME].Name;
		public string UserName => fields[LK_USER_NAME].Name;
		public string MachineName => fields[LK_MACHINE_NAME].Name;
		public string RevitModel => fields[LK_REVIT_MODEL].Name;

		// public ScFieldsLock2 LockFields2 = new ScFieldsLock2();


	#endregion

	#region methods

		private void init()
		{
			setFields();
			setFieldsValues();
		}

		/// <summary>
		/// create the fields in for the lock schema
		/// </summary>
		private void setFields()
		{
			fields = new Dictionary<SchemaLockKey, ScFieldDef<SchemaLockKey>>();

			Add(LK_SCHEMA_NAME , "Name"       , "Name"                            , LF_SCHEMA_NAME           , DL_BASIC);
			Add(LK_DESCRIPTION , "Description", "Description"                     , LF_SCHEMA_DESC           , DL_BASIC);
			Add(LK_MODEL_PATH  , "ModelPath"  , "Path of Model"                   , MODEL_PATH                 , DL_MEDIUM);
			Add(LK_MODEL_NAME  , "ModelName"  , "Name of Model"                   , MODEL_NAME                 , DL_MEDIUM);
			Add(LK_VERSION     , "Version"    , "Lock Schema Version"             , "1.0"                      , DL_MEDIUM);
			Add(LK_CREATE_DATE , "ModifyDate" , "Date Modified (or Created)"      , DateTime.UtcNow.ToString() , DL_MEDIUM);
			Add(LK_USER_NAME   , "UserName"   , "User Name of Lock Creator"       , UtilityLibrary.CsUtilities.UserName     , DL_ADVANCED);

			Add(LK_MACHINE_NAME, "MachineName", "Machine Name of Lock Creator"    , UtilityLibrary.CsUtilities.MachineName  , DL_ADVANCED);
			Add(LK_REVIT_MODEL , "RevitModel" , "Path and Name of Revit Model"    , "{not set}"                , DL_ADVANCED);
			Add(LK_GUID        , "GUID"       , "Lock GUID"                       , "{not set}"                , DL_DEBUG);
		}

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ScFieldsLock)}| username| {UserName} | model| {RevitModel}";
		}

	#endregion
	}
}