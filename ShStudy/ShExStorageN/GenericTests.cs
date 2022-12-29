#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShSchemaFields;

#endregion

// user name: jeffs
// created:   11/23/2022 9:02:37 PM

namespace ShStudy.ShExStorageN
{
	public class GenericTests<TT, TL, TR, TShtKey, TShtFlds, TRowKey, TRowFlds, TRow, TLokKey, TLokFlds> 
	where TT : AShScSheet<TShtKey, TShtFlds, TRowKey, TRowFlds, TRow>, new()
	where TL : AShScFields<TLokKey, TLokFlds>, new()
	where TR : AShScRow<TRowKey, TRowFlds>, new()
	where TShtKey : Enum
	where TShtFlds : IShScFieldData1<TShtKey>, new()
	where TRowKey : Enum
	where TRowFlds : IShScFieldData1<TRowKey>, new()
	where TRow : AShScRow<TRowKey, TRowFlds>, new()
	where TLokKey : Enum
	where TLokFlds : IShScFieldData1<TLokKey>, new()
	{


		public TT sheet;


	}


	public class GenericTest1
	{
		private GenericTests 
			< ScDataSheet,
			ScDataLock,
			ScDataRow,
			SchemaSheetKey,
			ScFieldDefData<SchemaSheetKey>,
			SchemaRowKey,
			ScFieldDefData<SchemaRowKey>,
			ScDataRow,
			SchemaLockKey,
			ScFieldDefData<SchemaLockKey>
			> gta;

		public GenericTest1()
		{
			gta = new GenericTests<
				ScDataSheet,
				ScDataLock,
				ScDataRow, SchemaSheetKey,
				ScFieldDefData<SchemaSheetKey>,
				SchemaRowKey,
				ScFieldDefData<SchemaRowKey>,
				ScDataRow, SchemaLockKey,
				ScFieldDefData<SchemaLockKey>>();


			ScDataSheet st1 = new ScDataSheet();

			gta.sheet = st1;

			st1 = gta.sheet;

		}
	}
}
