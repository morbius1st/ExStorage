#region using

using System;
using System.Collections.Generic;
using RevitSupport;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExConst;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaRowKey;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaTableKey;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaLockKey;

#endregion

// username: jeffs
// created:  10/22/2022 11:08:15 AM

namespace ShExStorageC.ShExStorage
{
	public class ExStorageLibraryC
	{
	#region private fields

	#endregion

	#region ctor

		public ExStorageLibraryC() { }

	#endregion

	#region public properties

	#endregion

	#region private properties

	#endregion

	#region public methods

		/// <summary>
		/// make the generic data set of table data
		/// </summary>
		public ScDataTable MakeInitialDataTable(ExId e)
		{
			ScDataTable tbld = new ScDataTable();

			tbld.Fields[SK0_SCHEMA_NAME].SetValue = e.ExsIdTable;
			tbld.Fields[SK0_MODEL_NAME].SetValue = RvtCommand.RvtDoc.Title;
			tbld.Fields[SK0_MODEL_PATH].SetValue = RvtCommand.RvtDoc.PathName;

			tbld.Fields[SK9_GUID].SetValue = Guid.NewGuid().ToString();

			return tbld;
		}

		/// <summary>
		/// make the generic data set of row data
		/// </summary>
		public ScDataRow MakeInitialDataRow(ExId e, ScDataTable tbld)
		{
			ScDataRow rowd = new ScDataRow();

			dynamic a = tbld.Fields[SK0_MODEL_NAME].DyValue;
			ShScFieldDefData<SchemaTableKey> b = tbld.Fields[SK0_MODEL_NAME];
			Dictionary<SchemaTableKey, ShScFieldDefData<SchemaTableKey>> c = tbld.Fields;

			rowd.Fields[CK0_SCHEMA_NAME].SetValue = e.ExsIdRow;
			rowd.Fields[CK0_MODEL_NAME].SetValue = tbld.Fields[SK0_MODEL_NAME].GetValue<string>();
			rowd.Fields[CK0_MODEL_PATH].SetValue = tbld.Fields[SK0_MODEL_PATH].GetValue<string>(); ;

			rowd.Fields[CK9_GUID].SetValue = Guid.NewGuid().ToString();

			rowd.Fields[CK9_ROW_FAMILY_NAME].SetValue = K_NOT_DEFINED;
			rowd.Fields[CK9_SEQUENCE].SetValue = K_NOT_DEFINED;
			rowd.Fields[CK9_SKIP].SetValue = K_NOT_DEFINED;
			rowd.Fields[CK9_UPDATE_RULE].SetValue = K_NOT_DEFINED;
			rowd.Fields[CK9_XL_FILE_PATH].SetValue = K_NOT_DEFINED;
			rowd.Fields[CK9_XL_WORKSHEET_NAME].SetValue = K_NOT_DEFINED;

			return rowd;
		}

		/// <summary>
		/// make the generic set of lock data
		/// </summary>
		public ScDataLock MakeInitialDataLock(ExId e, ScDataTable tbld)
		{
			ScDataLock lokd = new ScDataLock();

			lokd.Fields[LK0_SCHEMA_NAME].SetValue = "need to work out id";
			lokd.Fields[LK0_MODEL_NAME].SetValue = tbld.Fields[SK0_MODEL_NAME].GetValue<string>();
			lokd.Fields[LK0_MODEL_PATH].SetValue = tbld.Fields[SK0_MODEL_PATH].GetValue<string>();

			lokd.Fields[LK9_GUID].SetValue = Guid.NewGuid().ToString();


			return lokd;
		} 

	#endregion

	#region private methods

	#endregion

	#region event consuming

	#endregion

	#region event publishing

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ExStorageLibraryC)}";
		}

	#endregion
	}
}