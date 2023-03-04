﻿#region + Using Directives

using static ShExStorageN.ShSchemaFields.ShScSupport.ScFieldColumns;
using static ShStudyN.ShEval.JustifyHoriz;
using static ShStudyN.ShEval.ColData;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaLockKey;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaSheetKey;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaRowKey;

#endregion

// user name: jeffs
// created:   10/15/2022 7:58:54 AM

namespace ShEval
{
	public class ShFieldDisplayData
	{
	#region row record fields

		public static readonly Dictionary<ScFieldColumns, string>
			ScRecordHeaderTitles =
				new Dictionary<ScFieldColumns, string>()
				{
					{ SFC_KEY            , "Key" },
					{ SFC_NAME           , "Name" },
					{ SFC_DESC           , "Description" },
					{ SFC_VALUE          , "Dynamic Value" },
					{ SFC_VALUE_STR      , "Value as String" },
					{ SFC_VALUE_TYPE     , "Value Type" },
					{ SFC_REVIT_TYPE     , "Revit SpecTypeId" },
					{ SFC_FIELD          , "Associated Field Key" },
					{ SFC_DISPLAY_LEVEL  , "Display Level" }
				};

		public readonly Dictionary<ScFieldColumns, ColData>
			ScFieldsColDefinitions =
				Mz(
					Tz(SFC_KEY            , ScRecordHeaderTitles[SFC_KEY          ], CENTER, 24),
					Tz(SFC_NAME           , ScRecordHeaderTitles[SFC_NAME         ], CENTER, 14),
					Tz(SFC_DESC           , ScRecordHeaderTitles[SFC_DESC         ], CENTER, 30),
					Tz(SFC_VALUE          , ScRecordHeaderTitles[SFC_VALUE        ], CENTER, 30),
					Tz(SFC_VALUE_STR      , ScRecordHeaderTitles[SFC_VALUE_STR    ], CENTER, 30),
					Tz(SFC_VALUE_TYPE     , ScRecordHeaderTitles[SFC_VALUE_TYPE   ], CENTER, 16),
					Tz(SFC_REVIT_TYPE     , ScRecordHeaderTitles[SFC_REVIT_TYPE   ], CENTER, 32),
					Tz(SFC_FIELD          , ScRecordHeaderTitles[SFC_FIELD        ], CENTER, 30),
					Tz(SFC_DISPLAY_LEVEL  , ScRecordHeaderTitles[SFC_DISPLAY_LEVEL], CENTER, 16)
					);


		public readonly List<ScFieldColumns>
			ScFieldsColOrder = new List<ScFieldColumns>()
			{
				SFC_KEY  ,
				SFC_NAME ,
				SFC_DESC ,
				SFC_DISPLAY_LEVEL ,
				SFC_VALUE_TYPE,
				SFC_VALUE
			};

		public readonly List<ScFieldColumns>
			ScDataColOrder = new List<ScFieldColumns>()
			{
				SFC_KEY  ,
				SFC_NAME ,
				SFC_DESC ,
				// SFC_FIELD ,
				SFC_REVIT_TYPE,
				SFC_VALUE_TYPE,
				SFC_VALUE
			};

		public readonly List<ScFieldColumns>
			ScDataColOrderLight = new List<ScFieldColumns>()
			{
				SFC_KEY  ,
				SFC_NAME ,
				// SFC_DESC ,
				// SFC_FIELD ,
				// SFC_REVIT_TYPE,
				SFC_VALUE_TYPE,
				SFC_VALUE
			};

	#endregion

	#region row record entries

		public readonly List<SchemaSheetKey> SchemaSheetKeyOrder = new List<SchemaSheetKey>()
		{
			SK0_KEY             , 
			SK0_SCHEMA_NAME     , 
			SK0_DESCRIPTION     , 
			SK0_VERSION         , 
			SK1_MODIFY_DATE     , 
			SK0_GUID            , 
			SK0_USER_NAME       ,
			SK2_DEVELOPER       , 
			SK2_MODEL_PATH      , 
			SK2_MODEL_NAME      , 
		};


		public readonly List<SchemaRowKey> SchemaRowKeyOrder = new List<SchemaRowKey>()
		{
			RK0_KEY               , 
			RK0_SCHEMA_NAME       , 
			RK0_DESCRIPTION       , 
			RK0_VERSION           , 
			RK1_MODIFY_DATE       , 
			RK0_GUID              , 
			RK0_USER_NAME         , 
			RK2_SEQUENCE          , 
			RK2_UPDATE_RULE       , 
			RK2_SKIP              , 
			RK2_CELL_FAMILY_NAME  , 
			RK2_XL_FILE_PATH      , 
			RK2_XL_WORKSHEET_NAME , 
			RK2_MODEL_PATH        , 
			RK2_MODEL_NAME        , 
		};

		public readonly List<SchemaLockKey> SchemaLockKeyOrder = new List<SchemaLockKey>()
		{
			LK0_KEY              , 
			LK0_SCHEMA_NAME      , 
			LK0_DESCRIPTION	     , 
			LK1_CREATE_DATE	     , 
			LK0_VERSION		     , 
			LK0_GUID			 , 
			LK0_USER_NAME	     , 
			LK2_MACHINE_NAME	 , 
			LK2_MODEL_NAME	     , 
			LK2_MODEL_PATH	     , 
		};

		
		public readonly List<KEY> SchemaLockKeyOrder2 = new List<KEY>()
		{
			/*
			(lKey) ShExNTblKeys.TK_KEY          , 
			(lKey) ShExNTblKeys.TK_SCHEMA_NAME  , 
			(lKey) ShExNTblKeys.TK_DESCRIPTION  , 
			(lKey) ShExNTblKeys.TK_DATE	        , 
			(lKey) ShExNTblKeys.TK_VERSION		, 
			(lKey) ShExNTblKeys.TK_GUID			, 
			(lKey) ShExNTblKeys.TK_USERNAME	    , 
			ScLokKeys.LK_MACHINE_NAME	        , 
			(lKey) ShExNTblKeys.TK_MODEL_NAME   , 
			(lKey) ShExNTblKeys.TK_MODEL_PATH   , 
			*/
		};

		public readonly List<KEY> SchemaSheetKeyOrder2 = new List<KEY>()
		{
			/*
			SK0_KEY             , 
			SK0_SCHEMA_NAME     , 
			SK0_DESCRIPTION     , 
			SK0_VERSION         , 
			SK1_MODIFY_DATE     , 
			SK0_GUID            , 
			SK0_USER_NAME       ,
			SK2_DEVELOPER       , 
			SK2_MODEL_PATH      , 
			SK2_MODEL_NAME      , 
			*/
		};


		public readonly List<KEY> SchemaRowKeyOrder2 = new List<KEY>()
		{
		/*
			RK0_KEY               , 
			RK0_SCHEMA_NAME       , 
			RK0_DESCRIPTION       , 
			RK0_VERSION           , 
			RK1_MODIFY_DATE       , 
			RK0_GUID              , 
			RK0_USER_NAME         , 
			RK2_SEQUENCE          , 
			RK2_UPDATE_RULE       , 
			RK2_SKIP              , 
			RK2_CELL_FAMILY_NAME  , 
			RK2_XL_FILE_PATH      , 
			RK2_XL_WORKSHEET_NAME , 
			RK2_MODEL_PATH        , 
			RK2_MODEL_NAME        , 
			*/
		};




	#endregion
	}
}