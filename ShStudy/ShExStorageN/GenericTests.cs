#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ShScSupport;
using ShExStorageN.ShSchemaFields;

#endregion

// user name: jeffs
// created:   11/23/2022 9:02:37 PM

namespace ShStudyN.ShExStorageN
{
	public class GenericTests<TT, TL, TR, TShtKey, TShtFlds, TRowKey, TRowFlds, TRow, TLokKey, TLokFlds> 
	where TT : AShScSheet<TShtKey, TShtFlds, TRowKey, TRowFlds, TT, TRow>, new()
	where TL : AShScFields<TLokKey, TLokFlds>, new()
	where TR : AShScRow<TRowKey, TRowFlds>, new()
	where TShtKey : Enum
	where TShtFlds : ScFieldDefData<TShtKey>, new()
	where TRowKey : Enum
	where TRowFlds : ScFieldDefData<TRowKey>, new()
	where TRow : AShScRow<TRowKey, TRowFlds>, new()
	where TLokKey : Enum
	where TLokFlds : ScFieldDefData<TLokKey>, new()
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
