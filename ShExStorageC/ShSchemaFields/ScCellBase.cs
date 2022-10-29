#region using

using System;
using System.Collections.Generic;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields;
// using static ShExStorageN.ShSchemaFields.SchemaStoreOptions;
using static ShExStorageN.ShSchemaFields.SchemaFieldDisplayLevel;
using static ShExStorageN.ShSchemaFields.CellUpdateRules;
using static ShExStorageN.ShSchemaFields.ShExConst;



#endregion

// username: jeffs
// created:  10/16/2022 3:17:56 PM

namespace ShExStorageC.ShSchemaFields
{
	//
	// public abstract class ScCellBase : ShScFieldsBase<SchemaCellKey>
	// {
	// 	public const string CF_SCHEMA_NAME = "Cells>Schema>Fields>Cell";
	// 	public const string CF_SCHEMA_DESC = "Cells Cell DS";
	//
	// 	#region private fields
	//
	// 	#endregion
	//
	// 	#region ctor
	//
	// 	public ScCellBase()
	// 	{
	// 		init();
	// 	}
	//
	//
	// 	#endregion
	//
	// 	#region public properties
	//
	// 	public string SchemaName => Fields[CK0_SCHEMA_NAME].DyValue.AsString();
	//
	//
	// #endregion
	//
	// #region private properties
	//
	// #endregion
	//
	// #region public methods
	//
	// 	public void init()
	// 	{
	// 		Fields = new Dictionary<SchemaCellKey, ShScFieldDefMeta<SchemaCellKey>>(15);
	// 		Data = new Dictionary<SchemaCellKey, ShScFieldDefData<SchemaCellKey>>(15);
	//
	// 		// these are pre-set by the creation of the object
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaCellKey> (CK0_KEY              , "Key"           , "Access Key"                 , new DynaValue(CK0_KEY)                             , DL_ADVANCED));
	// 		Add(/*SO_FIELD,*/ new ShScFieldDefMeta<SchemaCellKey> (CK0_SCHEMA_NAME      , "Name"          , "Schema Name"                , new DynaValue(K_NOT_DEFINED)                         , DL_ADVANCED));
	// 		Add(/*SO_FIELD,*/ new ShScFieldDefMeta<SchemaCellKey> (CK0_DESCRIPTION      , "Desc"          , "Description"                , new DynaValue(CF_SCHEMA_DESC)                      , DL_ADVANCED));
	// 		Add(/*SO_FIELD,*/ new ShScFieldDefMeta<SchemaCellKey> (CK0_VERSION          , "Version"       , "Cell Schema Version"        , new DynaValue("1.0")                               , DL_MEDIUM));
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaCellKey> (CK0_MODEL_PATH       , "ModelPath"     , "Path of Model"              , new DynaValue(K_NOT_DEFINED)                       , DL_MEDIUM));
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaCellKey> (CK0_MODEL_NAME       , "ModelName"     , "Name of Model"              , new DynaValue(K_NOT_DEFINED)                       , DL_MEDIUM));
	//
	//
	// 		// these are pre-set by the creation of the object & when the object is modified
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaCellKey> (CK2_USER_NAME        , "UserName"      , "User Name of Cell Creator"  , new DynaValue(UtilityLibrary.CsUtilities.UserName) , DL_ADVANCED));
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaCellKey> (CK2_MODIFY_DATE      , "ModifyDate"    , "Date Modified (or Created)" , new DynaValue(DateTime.UtcNow.ToString())          , DL_MEDIUM));
	//
	//
	// 		// these are variable depending on the database
	// 		// value may not apply
	// 		// Add(/*SO_ALL  ,*/ new ScFieldDef<SchemaCellKey> (CK_VALUE            , "Value"         , "Value"                      , new DynaValue(0.0)                                 , DL_ADVANCED));
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaCellKey> (CK9_SEQUENCE         , "Sequence"      , "Evaluation Sequence"        , new DynaValue(K_NOT_DEFINED)                       , DL_BASIC));
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaCellKey> (CK9_UPDATE_RULE      , "UpdateRule"    , "Update Rule"                , new DynaValue(UR_UNDEFINED)                        , DL_BASIC));
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaCellKey> (CK9_CELL_FAMILY_NAME , "CellFamilyName", "Name of the Cells Family"   , new DynaValue(K_NOT_DEFINED)                       , DL_BASIC));
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaCellKey> (CK9_SKIP             , "Skip"          , "Skip Updating"              , new DynaValue(false)                               , DL_BASIC));
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaCellKey> (CK9_XL_FILE_PATH     , "XlFilePath"    , "File Path to the Excel File", new DynaValue(K_NOT_DEFINED)                       , DL_BASIC));
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaCellKey> (CK9_XL_WORKSHEET_NAME, "XlWorksheet"   , "Name of the Excel Worksheet", new DynaValue(K_NOT_DEFINED)                       , DL_BASIC));
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaCellKey> (CK9_GUID             , "GUID"          , "Cell GUID"                  , new DynaValue(K_NOT_DEFINED)                       , DL_DEBUG));
	//
	//
	// 	}
	//
	// #endregion
	//
	// #region private methods
	//
	// 	// public void Add(SchemaStoreOptions where, ScFieldDef<SchemaCellKey> fieldDef)
	// 	// {
	// 	// 	if (where == SO_FIELD || where == SO_ALL)
	// 	// 	{
	// 	// 		AddField(fieldDef);
	// 	// 	}
	// 	//
	// 	// 	if (where == SO_ALL || where == SO_DATA)
	// 	// 	{
	// 	// 		AddData(new ScDataDef<SchemaCellKey>(fieldDef.Key, fieldDef.Value, fieldDef.DisplayLevel));
	// 	// 	}
	// 	// }
	//
	// #endregion
	//
	// #region event consuming
	//
	// #endregion
	//
	// #region event publishing
	//
	// #endregion
	//
	// #region system overrides
	//
	// 	public override string ToString()
	// 	{
	// 		return $"this is {nameof(ScCellBase)}";
	// 	}
	//
	// #endregion
	// }

}