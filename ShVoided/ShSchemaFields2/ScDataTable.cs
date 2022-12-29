#region using

#endregion

// username: jeffs
// created:  10/16/2022 3:17:56 PM


namespace ShSchemaFields
{
	public class ScDataSheet1 :
		AShScInfoBase<SchemaSheetKey, ShScFieldDefData<SchemaSheetKey>>,
		IShScBaseData<SchemaSheetKey, ShScFieldDefData<SchemaSheetKey>,
		SchemaRowKey, ScDataRow
		>

	{
		public ScDataSheet1()
		{
			init();
		}

		// the list of parameters specific to sheet data
		public override Dictionary<SchemaSheetKey, ShScFieldDefData<SchemaSheetKey>> Fields { get; protected set; }

		// the set of rows associated to this sheet
		public Dictionary<string, ScDataRow> Rows { get; protected set; }

		public override string SchemaName => Fields[SchemaSheetKey.TK0_SCHEMA_NAME].ValueAsString;

		public void init()
		{
			ScInfoMeta1.ConfigData(Fields, ScInfoMeta1.FieldsSheet);

			Rows = new Dictionary<string, ScDataRow>();
		}


	#region public methods

		public void AddRow(ScDataRow1 rowd)
		{
			// ListCollectionView v1 = new ListCollectionView(rowsList);
			// v1.SortDescriptions.Clear();
			// v1.SortDescriptions.Add(
			// 	new SortDescription(
			// 		rowsList[0].SchemaName, ListSortDirection.Ascending));


			Rows.Add(rowd.Fields[SchemaRowKey.RK0_SCHEMA_NAME].Value, rowd);
		}

	#endregion


	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ScDataSheet)}";
		}

	#endregion
	}


	//
	//
	// #region private fields
	//
	// 	private List<ScRowData> rowsList;
	//
	// #endregion
	//
	// #region ctor
	//
	// 	public ScDataSheet1()
	// 	{
	// 		init();
	// 	}
	//
	// #endregion
	//
	// #region public properties
	//
	// 	// public Dictionary<SchemaSheetKey, ScDataDef<SchemaSheetKey>> SheetData => Data;
	// 	public List<ScRowData> RowsList => rowsList;
	//
	// #endregion
	//
	// #region private properties
	//
	// #endregion
	//
	// #region public methods
	//
	// 	public void AddRow(ScRowData rowd)
	// 	{
	// 		
	// 		// ListCollectionView v1 = new ListCollectionView(rowsList);
	// 		// v1.SortDescriptions.Clear();
	// 		// v1.SortDescriptions.Add(
	// 		// 	new SortDescription(
	// 		// 		rowsList[0].SchemaName, ListSortDirection.Ascending));
	//
	//
	// 		rowsList.Add(rowd);
	// 	}
	//
	// 	public void RemoveRow(ScRowData cd)
	// 	{
	// 		rowsList.Remove(cd);
	// 	}
	//
	// 	public ScDataSheet1 FindRow(string findName)
	// 	{
	// 		return null;
	// 	}
	//
	// #endregion
	//
	// #region private methods
	//
	// 	private new void init()
	// 	{
	// 		rowsList = new List<ScRowData>(3);
	// 	}
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
	//
}