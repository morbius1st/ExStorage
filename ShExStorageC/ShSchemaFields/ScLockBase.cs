#region using

using System;
using System.Collections.Generic;
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
	// public abstract class ScLockBase : ShScFieldsBase<SchemaLockKey>
	// {
	// 	public const string LF_SCHEMA_NAME = "Cells>Schema>Fields>lock";
	// 	public const string LF_SCHEMA_DESC = "Cells lock DS";
	//
	// #region private fields
	//
	// 	// protected override Dictionary<SchemaLockKey, ScFieldDef<SchemaLockKey>> fields { get; set; }
	// 	// protected override Dictionary<SchemaLockKey, ScDataDef<SchemaLockKey>> data { get; set; }
	//
	// #endregion
	//
	// #region ctor
	//
	// 	public ScLockBase()
	// 	{
	// 		init();
	// 	}
	//
	// #endregion
	//
	// #region public properties
	//
	// 	public string SchemaName => Fields[LK0_SCHEMA_NAME].DyValue.AsString();
	// 	public string UserName => Fields[LK1_USER_NAME].DyValue.AsString();
	// 	public string ModifyDate => Fields[LK1_CREATE_DATE].DyValue.AsString();
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
	// 		Fields = new Dictionary<SchemaLockKey, ShScFieldDefMeta<SchemaLockKey>>(15);
	// 		Data = new Dictionary<SchemaLockKey, ShScFieldDefData<SchemaLockKey>>(15);
	//
	// 		// these are pre-set by the creation of the object
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaLockKey> (LK0_KEY              , "Key"           , "Access Key"                 , new DynaValue(LK0_KEY)                              , DL_ADVANCED));
	// 		Add(/*SO_FIELD,*/ new ShScFieldDefMeta<SchemaLockKey> (LK0_SCHEMA_NAME      , "Name"          , "Schema Name"                , new DynaValue(K_NOT_DEFINED)                       , DL_BASIC));
	// 		Add(/*SO_FIELD,*/ new ShScFieldDefMeta<SchemaLockKey> (LK0_DESCRIPTION      , "Desc"          , "Description"                , new DynaValue(LF_SCHEMA_DESC)                      , DL_BASIC));
	// 		Add(/*SO_FIELD,*/ new ShScFieldDefMeta<SchemaLockKey> (LK0_VERSION          , "Version"       , "Cell Schema Version"        , new DynaValue("1.0")                               , DL_MEDIUM));
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaLockKey> (LK0_MODEL_PATH       , "ModelPath"     , "Path of Model"              , new DynaValue(K_NOT_DEFINED)                       , DL_MEDIUM));
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaLockKey> (LK0_MODEL_NAME       , "ModelName"     , "Name of Model"              , new DynaValue(K_NOT_DEFINED)                       , DL_MEDIUM));
	//
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaLockKey> (LK1_USER_NAME        , "UserName"      , "User Name of Lock Creator"  , new DynaValue(UtilityLibrary.CsUtilities.UserName) , DL_ADVANCED));
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaLockKey> (LK1_CREATE_DATE      , "CreateDate"    , "Date Created"               , new DynaValue(DateTime.UtcNow.ToString())          , DL_MEDIUM));
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaLockKey> (LK1_MACHINE_NAME     , "MachineName"   , "Machine Name of Lock Creator", new DynaValue(UtilityLibrary.CsUtilities.MachineName) , DL_ADVANCED));
	//
	//
	// 		// these are variable depending on the database
	// 		// Add(/*SO_ALL  ,*/ new ScFieldDef<SchemaLockKey> (LK_VALUE            , "Value"         , "Value"                      , new DynaValue(0.0)                                 , DL_BASIC));
	// 		Add(/*SO_ALL  ,*/ new ShScFieldDefMeta<SchemaLockKey> (LK9_GUID             , "GUID"          , "Lock GUID"                  , new DynaValue(K_NOT_DEFINED)                       , DL_DEBUG));
	// 	}
	//
	// #endregion
	//
	// #region private methods
	//
	// 	// public void Add(SchemaStoreOptions where, ScFieldDef<SchemaLockKey> fieldDef)
	// 	// {
	// 	// 	if (where == SO_FIELD || where == SO_ALL)
	// 	// 	{
	// 	// 		AddField(fieldDef);
	// 	// 	}
	// 	//
	// 	// 	if (where == SO_ALL || where == SO_DATA)
	// 	// 	{
	// 	// 		AddData(new ScDataDef<SchemaLockKey>(fieldDef.Key, fieldDef.Value, fieldDef.DisplayLevel));
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
	// 		return "this is ScFieldsCell";
	// 	}
	//
	// #endregion
	// }
	//

}