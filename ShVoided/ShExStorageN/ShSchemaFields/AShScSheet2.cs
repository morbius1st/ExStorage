// Solution:     ExStorage
// Project:       ExStorage
// File:             AShScSheet.cs
// Created:      2022-12-31 (3:50 PM)

namespace ShExStorageN.ShSchemaFields
{
	/// <summary>
	/// abstract class for a sheet - implements AShScFields & IShScRows1<br/>
	/// adds the collection of rows and the routine to add a row
	/// </summary>
	public abstract class AShScSheet2<TShtFlds, TRowFlds, TRow> :
		AShScFields2<TShtFlds>,
		IShScRows2<TRowFlds, TRow>
		where TShtFlds : ScFieldDefData2, new()
		where TRowFlds : ScFieldDefData2, new()
		where TRow : AShScRow2<TRowFlds>, new()
	{
		public AShScSheet2()
		{
			Fields = new Dictionary<KEY, TShtFlds>(kc);

			init();
		}

		protected abstract void init();

		public override Dictionary<KEY, TShtFlds> Fields { get; protected set; }

		public Dictionary<string, TRow> Rows { get; protected set; }

		public abstract void AddRow(TRow row);

	}



}