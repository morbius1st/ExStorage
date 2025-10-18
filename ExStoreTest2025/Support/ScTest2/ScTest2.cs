
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStoreTest2025.Support.ScTest1;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ShScSupport;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields;
using ShExStorageR.ShExStorage;

using static ShExStorageN.ShExStorage.ExStoreRtnCode;


// user name: jeffs
// created:   9/13/2025 4:09:20 PM

namespace ExStoreTest2025.Support.ScTest2
{
	public class ScTest2
	{
		private ExId exid;

		public ShExStorManagerR<
			ScDataSheet1,
			ScDataRow1,
			ScDataLock1,
			SchemaSheetKey,
			ScFieldDefData1<SchemaSheetKey>,
			SchemaRowKey,
			ScFieldDefData1<SchemaRowKey>,
			SchemaLockKey,
			ScFieldDefData1<SchemaLockKey>
			> smR { get; private set; }


		public ScTest2(ExId exid)
		{
			Msgs.WriteLine("Main Window Model Created");

			smR = ShExStorManagerR<
				ScDataSheet1,
				ScDataRow1,
				ScDataLock1,
				SchemaSheetKey,
				ScFieldDefData1<SchemaSheetKey>,
				SchemaRowKey,
				ScFieldDefData1<SchemaRowKey>,
				SchemaLockKey,
				ScFieldDefData1<SchemaLockKey>>.Instance;

			smR.Init(exid);
			smR.StorLibR.smR = smR;
			smR.SchemaLibR.smR = smR;

			// smR.Sheet = ScInfoMeta1.MakeInitialDataSheet1(exid);
		}

		public void Test1()
		{
			DynaValue d = new DynaValue("Updated Description");

			if (smR.ChangeSheetField(SchemaSheetKey.SK0_DESCRIPTION, d))
			{
				Msgs.WriteLine("field change worked");
			}
			else
			{
				Msgs.WriteLine("field change failed");
			}

		}



		public override string ToString()
		{
			return $"this is {nameof(ScTest2)}";
		}
	}
}
