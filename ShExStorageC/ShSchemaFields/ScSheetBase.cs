#region using

using System;
using System.Collections.Generic;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields;
// using static ShExStorageN.ShSchemaFields.SchemaStoreOptions;
using static ShExStorageN.ShSchemaFields.SchemaFieldDisplayLevel;
using static ShExStorageN.ShSchemaFields.CellUpdateRules;
using static ShExStorageN.ShSchemaFields.ShExConst;
// using static ShExStorageC.ShSchemaFields.SchemaSheetKey;

#endregion

// username: jeffs
// created:  10/16/2022 3:17:56 PM

namespace ShExStorageC.ShSchemaFields
{

	//
	// public abstract class ScSheetBase : ShScFieldsBase<SchemaSheetKey>
	// {
	// 	public const string SF_SCHEMA_NAME = "Cells>Schema>Fields>Sheet";
	// 	public const string SF_SCHEMA_DESC = "Cells Sheet DS";
	//
	// #region private fields
	//
	// 	// protected override Dictionary<SchemaSheetKey, ScFieldDef<SchemaSheetKey>> fields { get; set; } 
	// 	// protected override Dictionary<SchemaSheetKey, ScDataDef<SchemaSheetKey>> data { get; set; }
	//
	// #endregion
	//
	// #region ctor
	//
	// 	public ScSheetBase()
	// 	{
	// 		init();
	// 	}
	//
	// #endregion
	//
	// #region public properties
	//
	// 	public string SchemaName => Fields[SK0_SCHEMA_NAME].DyValue.AsString();
	// 	public string UserName   => Fields[SK2_USER_NAME].DyValue.AsString();
	// 	public string ModifyDate => Fields[SK2_MODIFY_DATE].DyValue.AsString();
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
	// 		Fields = new Dictionary<SchemaSheetKey, ScFieldDef<SchemaSheetKey>>(15);
	// 		Data = new Dictionary<SchemaSheetKey, ScDataDef<SchemaSheetKey>>(15);
	//
	// 		// these are pre-set by the creation of the object
	// 		Add( /*SO_ALL  ,*/ new ScFieldDef<SchemaSheetKey> (SK0_KEY          , "Key"        , "Access Key"                   , new DynaValue(SK0_KEY)                                          , DL_ADVANCED));
	// 		Add( /*SO_FIELD,*/ new ScFieldDef<SchemaSheetKey> (SK0_SCHEMA_NAME  , "Name"       , "Schema Name"                  , new DynaValue(K_NOT_DEFINED)                                    , DL_ADVANCED));
	// 		Add( /*SO_FIELD,*/ new ScFieldDef<SchemaSheetKey> (SK0_DESCRIPTION  , "Desc"       , "Description"                  , new DynaValue(SF_SCHEMA_DESC)                                   , DL_ADVANCED));
	// 		Add( /*SO_FIELD,*/ new ScFieldDef<SchemaSheetKey> (SK0_VERSION      , "Version"    , "Sheet Schema Version"         , new DynaValue("1.0")                                            , DL_MEDIUM));
	// 		Add( /*SO_ALL  ,*/ new ScFieldDef<SchemaSheetKey> (SK0_MODEL_PATH   , "ModelPath"  , "Path of Model"                , new DynaValue(MODEL_PATH)                                       , DL_MEDIUM));
	// 		Add( /*SO_ALL  ,*/ new ScFieldDef<SchemaSheetKey> (SK0_MODEL_NAME   , "ModelName"  , "Name of Model"                , new DynaValue(MODEL_NAME)   , DL_MEDIUM));
	//
	// 		Add( /*SO_ALL  ,*/ new ScFieldDef<SchemaSheetKey> (SK1_DEVELOPER    , "Developer"  , "Name of Developer"            , new DynaValue(UtilityLibrary.CsUtilities.CompanyName)           , DL_MEDIUM));
	//
	//
	// 		// these are pre-set by the creation of the object & when the object is modified
	// 		Add( /*SO_ALL  ,*/ new ScFieldDef<SchemaSheetKey> (SK2_USER_NAME    , "UserName"   , "User Name of Sheet Modifier"  , new DynaValue(UtilityLibrary.CsUtilities.UserName)              , DL_ADVANCED));
	// 		Add( /*SO_ALL  ,*/ new ScFieldDef<SchemaSheetKey> (SK2_MODIFY_DATE  , "ModifyDate" , "Date Modified (or Created)"   , new DynaValue(DateTime.UtcNow.ToString())                       , DL_MEDIUM));
	//
	//
	// 		// these are variable depending on the database
	// 		Add( /*SO_ALL  ,*/ new ScFieldDef<SchemaSheetKey> (SK9_GUID         , "GUID"       , "Sheet GUID"                   , new DynaValue(K_NOT_DEFINED)                                    , DL_MEDIUM));
	// 		// Add(/*SO_ALL        ,*/ new ScFieldDef<SchemaSheetKey> (SK_VALUE , "Value" , "Value" , new DynaValue(1.0) , DL_BASIC));
	// 	}
	//
	// #endregion
	//
	// #region private methods
	//
	// 	// public void Add(SchemaStoreOptions where, ScFieldDef<SchemaSheetKey> fieldDef)
	// 	// {
	// 	// 	if (where == SO_FIELD || where == SO_ALL)
	// 	// 	{
	// 	// 		AddField(fieldDef);
	// 	// 	}
	// 	//
	// 	// 	if (where == SO_ALL || where == SO_DATA)
	// 	// 	{
	// 	// 		AddData(new ScDataDef<SchemaSheetKey>(fieldDef.Key, fieldDef.Value, fieldDef.DisplayLevel));
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
	// 		return $"this is {nameof(ScSheetBase)}";
	// 	}
	//
	// #endregion
	// }
	//

}