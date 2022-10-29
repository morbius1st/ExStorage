#region using directives

using System;
using System.Collections.Generic;
using ShExStorageN.ShSchemaFields;
using Autodesk.Revit.DB;
using ShExStorageC.ShSchemaFields.ScSupport;

#endregion


// projname: $projectname$
// itemname: ScFields
// username: jeffs
// created:  10/10/2022 9:57:09 PM

namespace ShExStorageC.ShSchemaFields
{
	/*
	public class ScFields
	{
	#region private fields

		private static readonly Lazy<ScFields> instance =
			new Lazy<ScFields>(() => new ScFields());

		private ScMetaLock lockFields;
		private ScMetaSheet sheetFields;
		private ScMetaCell cellFields;


	#endregion

	#region ctor

		private ScFields()
		{
			init();

			ForgeTypeId ut = UnitTypeId.Feet;
		}

	#endregion

	#region public properties

		public static ScFields Instance => instance.Value;

		public ScMetaLock ScFieldsLock => lockFields;
		public ScMetaCell ScFieldsCell => cellFields;
		public ScMetaSheet ScFieldsSheet => sheetFields;

		public Dictionary<SchemaLockKey, ShScFieldDefMeta<SchemaLockKey>> LockFields => lockFields.Fields;
		public Dictionary<SchemaSheetKey, ShScFieldDefMeta<SchemaSheetKey>> SheetFields => sheetFields.Fields;
		public Dictionary<SchemaCellKey, ShScFieldDefMeta<SchemaCellKey>> CellFields => cellFields.Fields;

	#endregion

	#region private properties

	#endregion

	#region public methods



	#endregion

	#region private methods

		private void init()
		{
			lockFields = new ScMetaLock();
			sheetFields = new ScMetaSheet();
			cellFields = new ScMetaCell();
		}


	#endregion

	#region event consuming

	#endregion

	#region event publishing

	#endregion

	#region system overrides

		public override string ToString()
		{
			return "this is ScFields";
		}

	#endregion


	}
		*/
}