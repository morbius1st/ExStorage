#region + Using Directives

using System;
using System.Collections.Generic;
using ShExStorageN.ShSchemaFields;
using static ShExStorageN.ShSchemaFields.ShScSupport.ScFieldColumns;
using static ShStudy.ShEval.JustifyHoriz;
using static ShStudy.ShEval.ColData;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShSchemaFields.ShScSupport;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaLockKey;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaTableKey;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaRowKey;

#endregion

// user name: jeffs
// created:   10/15/2022 7:58:54 AM

namespace ShStudy.ShEval
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

	#endregion

	#region row record entries

		public readonly List<SchemaTableKey> SchemaTableKeyOrder = new List<SchemaTableKey>()
		{
			SK0_KEY             , // orig order: 0
			SK0_SCHEMA_NAME     , // orig order: 1
			SK0_DESCRIPTION     , // orig order: 2
			SK0_VERSION         , // orig order: 6
			SK2_MODIFY_DATE     , // orig order: 7
			SK9_GUID            , // orig order: 8
			SK2_USER_NAME       , // orig order: 9
			SK1_DEVELOPER       , // orig order: 10
			SK0_MODEL_PATH      , // orig order: 4
			SK0_MODEL_NAME      , // orig order: 5
		};


		public readonly List<SchemaRowKey> SchemaRowKeyOrder = new List<SchemaRowKey>()
		{
			CK0_KEY               , // orig order: 0
			CK0_SCHEMA_NAME       , // orig order: 1
			CK0_DESCRIPTION       , // orig order: 2
			CK0_VERSION           , // orig order: 6
			CK2_MODIFY_DATE       , // orig order: 7
			CK9_GUID              , // orig order: 8
			CK2_USER_NAME         , // orig order: 9
			CK9_SEQUENCE          , // orig order: 10
			CK9_UPDATE_RULE       , // orig order: 11
			CK9_SKIP              , // orig order: 13
			CK9_ROW_FAMILY_NAME  , // orig order: 12
			CK9_XL_FILE_PATH      , // orig order: 15
			CK9_XL_WORKSHEET_NAME , // orig order: 14
			CK0_MODEL_PATH        , // orig order: 4
			CK0_MODEL_NAME        , // orig order: 5
		};

		public readonly List<SchemaLockKey> SchemaLockKeyOrder = new List<SchemaLockKey>()
		{
			LK0_KEY              , // orig order: 0
			LK0_SCHEMA_NAME      , // orig order: 1
			LK0_DESCRIPTION	     , // orig order: 2
			LK1_CREATE_DATE	     , // orig order: 7
			LK0_VERSION		     , // orig order: 6
			LK9_GUID			 , // orig order: 8
			LK1_USER_NAME	     , // orig order: 9
			LK1_MACHINE_NAME	 , // orig order: 10
			LK0_MODEL_NAME	     , // orig order: 5
			LK0_MODEL_PATH	     , // orig order: 4
		};





	#endregion
	}
}