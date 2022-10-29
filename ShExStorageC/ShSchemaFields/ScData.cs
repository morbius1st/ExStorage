#region using directives

using System;
using System.Collections.Generic;
using ShExStorageC.ShSchemaData;
using ShExStorageN.ShSchemaFields;
using static ShExStorageN.ShSchemaFields.SchemaFieldDisplayLevel;
using static ShExStorageC.ShSchemaFields.SchemaLockKey;
using Autodesk.Revit.DB;
using ShExStorageC.ShSchemaFields;

#endregion


// projname: $projectname$
// itemname: ScData
// username: jeffs
// created:  10/10/2022 9:57:09 PM

namespace ShExStorageC.ShSchemaData
{
	public class ScData
	{
	#region private fields

		private static readonly Lazy<ScData> instance =
			new Lazy<ScData>(() => new ScData());

		/// <summary>
		/// the one and only data store for a single revit DB<br/>
		/// this will then hold the list of associated cells which <br/>
		/// will hold the list of associated families
		/// </summary>
		private ScSheetData sheetData;


	#endregion

	#region ctor

		private ScData()
		{
			init();
		}

	#endregion

	#region public properties

		public static ScData Instance => instance.Value;

		public ScSheetData ScSheetData => sheetData;

		// public Dictionary<SchemaSheetKey, ScDataDef<SchemaSheetKey>> SheetData => sheetData.SheetData;

	#endregion

	#region private properties

	#endregion

	#region public methods

		public void ResetSheetData()
		{
			sheetData.Reset();
		}

	#endregion

	#region private methods

		private void init()
		{
			sheetData = new ScSheetData();
			// cells = new Dictionary<string, ScCellData>();
		}

	#endregion

	#region event consuming

	#endregion

	#region event publishing

	#endregion

	#region system overrides

		public override string ToString()
		{
			return "this is ScData";
		}

	#endregion
	}
}