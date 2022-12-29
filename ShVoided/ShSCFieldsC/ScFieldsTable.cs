#region using

using static ShExStorageC.ShSchemaFields1.SchemaSheetKey;
using static ShExStorageN.ShSchemaFields.SchemaFieldDisplayLevel;

#endregion

// username: jeffs
// created:  10/10/2022 11:30:35 PM

namespace ShSCFieldsC
{
	public class ScFieldsSheet :  ShScFields<SchemaSheetKey>
	{
		public const string SF_SUBSCHEMA_NAME = "RowsSubSchemaName_";

		public const string SF_SCHEMA_NAME = "Rows>Schema>Fields>Sheet";
		public const string SF_SCHEMA_DESC = "Rows Sheet DS";
		
		public ScFieldsSheet()
		{
			init();
		}

	#region public properties

		public  Dictionary<SchemaSheetKey, ScFieldDef<SchemaSheetKey>> SheetFields => fields;

		public string SchemaName => fields[TK_SCHEMA_NAME].Name;
		public string UserName   => fields[TK_USER_NAME].Name;
		public string ModifyData => fields[TK_MODIFY_DATE].Name;
		
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
			fields = new Dictionary<SchemaSheetKey, ScFieldDef<SchemaSheetKey>>();

			Add(TK_SCHEMA_NAME , "Name"       , "Name"                            , SF_SCHEMA_NAME          , DL_BASIC);
			Add(TK_DESCRIPTION , "Description", "Description"                     , SF_SCHEMA_DESC          , DL_BASIC);
			Add(TK_MODEL_PATH  , "ModelPath"  , "Path of Model"                   , MODEL_PATH                 , DL_MEDIUM);
			Add(TK_MODEL_NAME  , "ModelName"  , "Name of Model"                   , MODEL_NAME                 , DL_MEDIUM);
			Add(TK_VERSION     , "Version"    , "Sheet Schema Version"            , "1.0"                      , DL_MEDIUM);
			Add(TK_USER_NAME   , "UserName"   , "User Name of Sheet Creator"      , UtilityLibrary.CsUtilities.UserName     , DL_ADVANCED);
			Add(TK_MODIFY_DATE , "ModifyDate" , "Date Modified (or Created)"      , DateTime.UtcNow.ToString() , DL_MEDIUM);
			Add(TK_DEVELOPER   , "Developer"  , "Name of Developer"               , UtilityLibrary.CsUtilities.CompanyName                 , DL_MEDIUM);
			Add(TK_GUID        , "GUID"       , "Sheet GUID"                      , "{not set}"                , DL_BASIC);
		}

		
		public Tuple<string, Guid> SubSchemaField()
		{
			string uniqueName = SF_SUBSCHEMA_NAME+ System.IO.Path.GetRandomFileName().Replace('.', '_');

			return new Tuple<string, Guid>(uniqueName, Guid.NewGuid());
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