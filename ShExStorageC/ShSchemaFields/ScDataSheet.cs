#region + Using Directives

using ShExStorageN.ShSchemaFields;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using ShExStorageC.ShSchemaFields.ShScSupport;
using static ShExStorageC.ShSchemaFields.ShScSupport.ShExConstC;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExConstN;

#endregion

// user name: jeffs
// created:   10/29/2022 5:41:28 PM

	/*
	stored fields 

	key
	name
	desc
	ver
	mod path
	mod name
	dev
	user
	date
	guid

	*/


namespace ShExStorageC.ShSchemaFields
{

	public class ScDataSheet :
		AShScSheet< 
		SchemaSheetKey, ScFieldDefData<SchemaSheetKey>,
		SchemaRowKey, ScFieldDefData<SchemaRowKey>,
		ScDataSheet,
		ScDataRow>
	{

		public ScDataSheet()
		{
			// configure the initial field information
			ScInfoMeta.ConfigData(Fields, ScInfoMeta.MetaFieldsSheet);

			// temp name
			Fields[SchemaSheetKey.SK0_SCHEMA_NAME].SetValue = SF_SCHEMA_NAME;

			HasData = false;

			Rows = new Dictionary<string, ScDataRow>(1);

		}

		// initialization - basic configuration and adds the default data
		// some of the default data is correct as is and does not change later
		// some is variable
		// protected override void init()
		// {
		// 	// configure the initial field information
		// 	ScInfoMeta.ConfigData(Fields, ScInfoMeta.MetaFieldsSheet);
		//
		// 	// temp name
		// 	Fields[SchemaSheetKey.SK0_SCHEMA_NAME].SetValue = SF_SCHEMA_NAME;
		//
		// 	HasData = false;
		// }

	#region from fieldbase

		public override T GetValue<T>(int key)
		{
			return Fields[(SchemaSheetKey) key].GetValueAs<T>();
		}

		public override T GetValue<T>(SchemaSheetKey key)
		{
			return Fields[key].GetValueAs<T>();
		}

		public override void SetValue(int key, dynamic value)
		{
			SetValue((SchemaSheetKey) key, value);

			SetValue(SchemaSheetKey.SK1_MODIFY_DATE, DateTime.Now);
			SetValue(SchemaSheetKey.SK0_USER_NAME, UtilityLibrary.CsUtilities.UserName);
		}

	#endregion

	#region from rows1

		public override void AddRow(ScDataRow row)
		{
			// string key = row.Fields[SchemaRowKey.RK0_SCHEMA_NAME].GetValueAs<string>();
			string key = row.GetValue<string>(SchemaRowKey.RK0_SCHEMA_NAME);

			Rows.Add(key, row);
		}

	#endregion

	#region from sheet

	#endregion

		public override void ParseEnum(Type t, string enumName)
		{
			// na for this structure
		}
	}
}