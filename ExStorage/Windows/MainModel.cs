#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Structure;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ShScSupport;
using ShExStorageN.ShExStorage;
using ShExStorageR.ShExStorage;
using ShStudyN.ShEval;

#endregion

// user name: jeffs
// created:   1/10/2023 8:17:11 PM

namespace ExStorage.Windows
{
	public class MainModel
	{
		private ShDebugMessages M { get; set; }
		private MainWindowModel mw;

		public ShExStorManagerR<
			ScDataSheet,
			ScDataRow,
			ScDataLock,
			SchemaSheetKey,
			ScFieldDefData<SchemaSheetKey>,
			SchemaRowKey,
			ScFieldDefData<SchemaRowKey>,
			SchemaLockKey,
			ScFieldDefData<SchemaLockKey>
			> smR { get; private set; }

		public MainModel(
			MainWindowModel mw, 
			ShDebugMessages m)
		{
			M = m;
			this.mw = mw;

			config();


		}

		private void config()
		{
			smR = ShExStorManagerR<
				ScDataSheet,
				ScDataRow,
				ScDataLock,
				SchemaSheetKey,
				ScFieldDefData<SchemaSheetKey>,
				SchemaRowKey,
				ScFieldDefData<SchemaRowKey>,
				SchemaLockKey,
				ScFieldDefData<SchemaLockKey>>.Instance;
		}


		// modification procedure
		// modify the data
		// 1. update user name (modifier name)
		// 2. update modify date
		// 3. row 1
		// 3a. change cell family name
		// 3b. change skip
		// 3c. change modify date
		// 3d. change user name

		// A	check that sheet exists (has been read)
		// B	begin the modification process
		// C	check that no lock exists
		// D	create lock
		// E	write the data
		// F	remove the lock
		// 

		public bool ModifyTestPartA(ScDataSheet shtdUpdated, ShtExId shtExid, out string lockOwner)
		{
			bool result;
			lockOwner = null;

			M.WriteLineSteps("step: 00|", "begin modify");

			// step A
			M.WriteLineSteps("step: A1|", "check: do we have live data?");

			if (!mw.SheetDataCurrent.HasData)
			{
				M.WriteLineSteps("step: A2|", "nope & done");
				return false;
			}

			M.WriteLineSteps("step: A1|", "ok.  got data");

			// step B
			M.WriteLineSteps("step: B1|", "begin the modification process");

			// step C
			M.WriteLineSteps("step: C1|", "check? no lock exists?");

			LokExId lokExid = new LokExId(shtExid, LokExId.PRIME);

			if (smR.GetLockOwnerFromName(lokExid, out lockOwner))
			{
				M.WriteLineSteps("step: C2|", "nope (exists) & done");
				return false;
			}

			M.WriteLineSteps("step: C2|", "ok. no lock found");

			// step D - make a lock
			M.WriteLineSteps("step: D1|", "make a lock");

			M.WriteLineSteps("step: D2|", "make a lock data");

			ScDataLock lokd;

			lokd = new ScDataLock();
			lokd.Configure(lokExid);

			M.WriteLineSteps("step: D3|", "data made, write the lock");

			if (!smR.WriteLock(lokExid, lokd))
			{
				M.WriteLineSteps("step: D4|", "could not make lock & done");
				return false;
			}

			M.WriteLineSteps("step: D5|", "ok. lock made");

			M.WriteLineSteps("step: E1|", "write the data");

			// if (!smR.UpdateSheet(shtExid, shtdUpdated))
			// {
			// 	M.WriteLineSteps("step: E2|", $"write data| FAILED| {smR.ReturnCode}");
			// }
			// else
			// {
			// 	M.WriteLineSteps("step: E2|", $"write data|	WORKED| {smR.ReturnCode}");
			// }

			M.WriteLineSteps("step: F1|", "remove the lock");

			if (!smR.DeleteLock(lokExid))
			{
				M.WriteLineSteps("step: F1|", "remove lock| FAILED");
			}
			else
			{
				M.WriteLineSteps("step: F1|", "remove lock| WORKED");
			}


			return true;
		}





		public override string ToString()
		{
			return $"this is {nameof(MainModel)}";
		}
	}
}
