#region using

using static ShExStorageC.ShSchemaFields1.SchemaRowKey;
using static ShExStorageN.ShSchemaFields.ShExConst;
using static ShExStorageN.ShSchemaFields.SchemaFieldDisplayLevel;
using static ShExStorageN.ShSchemaFields.RowUpdateRules;

#endregion

// username: jeffs
// created:  10/10/2022 11:30:35 PM



namespace ShSchemaFields
{
	public abstract class ScRowBase : ShScFieldsBase<SchemaRowKey>
	{
		public const string CF_SCHEMA_NAME = "Rows>Schema>Fields>Row";
		public const string CF_SCHEMA_DESC = "Rows Row DS";
		

		public ScRowBase()
		{
			init();
		}

	#region public properties

		public  Dictionary<SchemaRowKey, ScFieldDef<SchemaRowKey>> RowFields => fields;

		public string SchemaName => fields[RK_SCHEMA_NAME].Name;
		public string UserName => fields[RK_USER_NAME].Name;
		public string ModifyData => fields[RK_MODIFY_DATE].Name;
		
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
			fields = new Dictionary<SchemaRowKey, ScFieldDef<SchemaRowKey>>();

			Add(RK_SCHEMA_NAME       , "Name"              , "Name"                            , CF_SCHEMA_NAME                        , DL_BASIC);
			Add(RK_DESCRIPTION       , "Description"       , "Description"                     , CF_SCHEMA_DESC                        , DL_BASIC);
			Add(RK_MODEL_PATH        , "ModelPath"         , "Path of Model"                   , MODEL_PATH                              , DL_MEDIUM);
			Add(RK_MODEL_NAME        , "ModelName"         , "Name of Model"                   , MODEL_NAME                              , DL_MEDIUM);
			Add(RK_VERSION           , "Version"           , "Row Schema Version"             , "1.0"                                   , DL_MEDIUM);
			Add(RK_USER_NAME         , "UserName"          , "User Name of Row Creator"       , UtilityLibrary.CsUtilities.UserName     , DL_ADVANCED);
			Add(RK_MODIFY_DATE       , "ModifyDate"        , "Date Modified (or Created)"      , DateTime.UtcNow.ToString()              , DL_MEDIUM);
			Add(RK_SEQUENCE          , "Sequence"          , "Evaluation Sequence"             , "A1.0"                                  , DL_BASIC);
			Add(RK_UPDATE_RULE       , "UpdateRule"        , "Update Rule"                     , UR_UPON_REQUEST                         , DL_BASIC);
			Add(RK_ROW_FAMILY_NAME  , "RowFamilyName"    , "Name of the Rows Family"        , K_NOT_DEFINED                           , DL_BASIC);
			Add(RK_SKIP              , "Skip"              , "Skip Updating"                   , false                                   , DL_BASIC);
			Add(RK_XL_FILE_PATH      , "XlFilePath"        , "File Path to the Excel File"     , K_NOT_DEFINED                           , DL_BASIC);
			Add(RK_XL_WORKSHEET_NAME , "Xlworksheet"       , "Name of the Excel worksheet"     , K_NOT_DEFINED                           , DL_BASIC); 
		}


	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ScFieldsLock)}| username| {UserName} | model| {ModifyData}";
		}

	#endregion
	}
}