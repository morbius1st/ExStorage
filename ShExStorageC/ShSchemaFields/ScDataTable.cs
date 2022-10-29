#region using

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShSchemaFields;

#endregion

// username: jeffs
// created:  10/16/2022 3:17:56 PM


namespace ShExStorageC.ShSchemaFields
{
	public class ScDataTable :
		AShScInfoBase<SchemaTableKey, ShScFieldDefData<SchemaTableKey>>,
		IShScBaseData<SchemaTableKey, ShScFieldDefData<SchemaTableKey>,
		SchemaRowKey, ScDataRow
		>

	{
		public ScDataTable()
		{
			init();
		}

		// the list of parameters specific to table data
		public override Dictionary<SchemaTableKey, ShScFieldDefData<SchemaTableKey>> Fields { get; protected set; }

		// the set of rows associated to this table
		public Dictionary<string, ScDataRow> Rows { get; protected set; }

		public override string SchemaName => Fields[SchemaTableKey.SK0_SCHEMA_NAME].ValueAsString;

		public void init()
		{
			ScInfoMeta.ConfigData(Fields, ScInfoMeta.FieldsTable);

			Rows = new Dictionary<string, ScDataRow>();
		}


	#region public methods

		public void AddRow(ScDataRow rowd)
		{
			// ListCollectionView v1 = new ListCollectionView(rowsList);
			// v1.SortDescriptions.Clear();
			// v1.SortDescriptions.Add(
			// 	new SortDescription(
			// 		rowsList[0].SchemaName, ListSortDirection.Ascending));


			Rows.Add(rowd.Fields[SchemaRowKey.CK0_SCHEMA_NAME].Value, rowd);
		}

	#endregion


	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ScDataTable)}";
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
	// 	public ScDataTable()
	// 	{
	// 		init();
	// 	}
	//
	// #endregion
	//
	// #region public properties
	//
	// 	// public Dictionary<SchemaTableKey, ScDataDef<SchemaTableKey>> TableData => Data;
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
	// 	public ScDataTable FindRow(string findName)
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