#region + Using Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStorage.Windows;
using JetBrains.Annotations;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ShScSupport;
using ShExStorageN.ShExStorage;
using static ShExStorageN.ShExStorage.ExStoreRtnCode;
using ShExStorageN.ShSchemaFields;
using ShStudyN.ShEval;

#endregion

// user name: jeffs
// created:   11/18/2022 9:38:37 PM
//
namespace ShExStorageR.ShExStorage
{
	public static class Heading
	{
		public static string AppName { get; } = "Cells";
		public static string AppVersion { get; } = "0_8_0_0";
	}



	[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
	[Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
	public class ShExStorManagerR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds> : INotifyPropertyChanged
		where TSht : AShScSheet<TShtKey, TShtFlds, TRowKey, TRowFlds, TSht, TRow>, new()
		where TLok : AShScLock<TLokKey, TLokFlds>, new()
		where TShtKey : Enum
		where TShtFlds : ScFieldDefData<TShtKey>, new()
		where TRowKey : Enum
		where TRowFlds : ScFieldDefData<TRowKey>, new()
		where TRow : AShScRow<TRowKey, TRowFlds>, new()
		where TLokKey : Enum
		where TLokFlds : ScFieldDefData<TLokKey>, new()
	{

	#region fields

		private static readonly Lazy<ShExStorManagerR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>> instance =
			new Lazy<ShExStorManagerR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>>(() =>
				new ShExStorManagerR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>());

		public static ShExStorManagerR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds> Instance => instance.Value;

		private ShDebugMessages m;

		// private bool hasData;
		private ExStoreRtnCode rtnCode;

	#endregion

	#region config

		private ShExStorManagerR()
		{
			config();
		}

		private void config()
		{
			SchemaLibR = new ShExSchemaLibR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>();
			StorLibR = new ShExStorageLibR();
		}

		public void SetDebugMsg(ShDebugMessages m)
		{
			M = m;
		}

	#endregion

	#region base

		public ShDebugMessages M
		{
			get => m;
			set
			{
				m = value;
				StorLibR.M = value;
				SchemaLibR.M = value;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		[DebuggerStepThrough]
		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

		public override string ToString()
		{
			return $"this is {nameof(ShExStorManagerR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>)}";
		}

	#endregion

	#region public properties

		// schema creation and data saving methods
		public ShExSchemaLibR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds> SchemaLibR { get; set; }

		// data storage methods
		public ShExStorageLibR StorLibR { get; set; }

		// status

		public ExStoreRtnCode ReturnCode
		{
			get => rtnCode;
			private set
			{
				rtnCode = value;
				OnPropertyChanged();
			}
		}

	#endregion

	#region public methods table

		public  ExStoreRtnCode ReadTable<TTbl, TKey, TField>(AExId exid, out  TTbl table)
			where TTbl : AShScFields<TKey, TField>, new()
			where TKey : Enum
			where TField : IShScFieldData<TKey>, new()
		{
			table = new TTbl();
			Entity e;
			DataStorage ds;

			if ((StorLibR.FindEntity(exid, true, out ds, out e) != XRC_GOOD)) return XRC_LOCK_NOT_EXIST;

			table = ReadTable<TTbl, TKey, TField>(e);

			return XRC_GOOD;
		}

		public TTbl ReadTable<TTbl, TKey, TField>(Entity e)
			where TTbl : AShScFields<TKey, TField>, new()
			where TKey : Enum
			where TField : IShScFieldData<TKey>, new()
		{
			TTbl table = new TTbl();

			SchemaLibR.ReadData(e, table);

			return table;
		}

	#endregion


	#region private methods

		/// <summary>
		/// set the class return code 'rtnCode' and returns the supplied<br/>
		/// rtnCode if supplied rtnCode is not XRC_VOID<br/>
		/// and returns the last rtnCode is the supplied rtnCode<br/>
		/// is XRC_VOID (preset default)
		/// /// </summary>
		[DebuggerStepThrough]
		private ExStoreRtnCode SetRtnCodeE(ExStoreRtnCode rtnCode  = XRC_VOID)
		{
			if (rtnCode == XRC_VOID)
			{
				return ReturnCode;
			}

			ReturnCode = rtnCode;

			return rtnCode;
		}

		/// <summary>
		/// set the class return code 'rtnCode' and return<br/>
		/// true if rtnCode matches the testCode (which is XRC_GOOD by default)<br/>
		/// that is, by default, if the rtnCode is good, this returns true<br/>
		/// else, this returns false<br/>
		/// however, if no rtnCode is supplied, this compares the last<br/>
		/// rtnCode with the default (XRC_GOOD) or the supplied testCode
		/// </summary>
		[DebuggerStepThrough]
		private bool SetRtnCodeB(ExStoreRtnCode rtnCode = XRC_VOID, ExStoreRtnCode testCode = XRC_GOOD)
		{
			if (rtnCode != XRC_VOID)
			{
				ReturnCode = rtnCode;
			}

			return ReturnCode == testCode;
		}

	#endregion

	#region public methods sheet

		/// <summary>
		/// determine if a sheet lock exists<br/>
		/// true == exists |  false == does not exist
		/// </summary>
		/// <param name="lokExid"></param>
		/// <returns></returns>
		public bool DoesSheetLockExist(LokExId lokExid)
		{
			M.WriteLineCodeMap();

			string userName;

			return SetRtnCodeB(StorLibR.DoesLockExist(lokExid, out userName));
		}

		/// <summary>
		/// delete the schema's associated to the datastorage element<br/>
		/// this also deletes the associated entity<br/>
		/// Last, the datastorage element is deleted<br/>
		/// return<br/>
		/// XRC_GOOD | worked
		/// sub-method rtnCode ?
		/// XRC_FAIL | transaction popped an exception
		/// </summary>
		public bool DeleteSheet(ShtExId shtExid, LokExId lokExid)
		{
			M.WriteLineCodeMap();
			// must: 
			// remove the schema (main & all rows)
			// remove the entity (main & all rows)
			// remove the ds
			// set sheet to init
			// cannot work if locked

			string userName;

			if (SetRtnCodeB(StorLibR.DoesLockExist(lokExid, out userName))) return SetRtnCodeB(XRC_LOCK_EXISTS);

			ExStoreRtnCode rtnCode;
			DataStorage ds;
			Schema sl;
			Entity e;

			rtnCode = StorLibR.FindEntity(shtExid, false, out ds, out e);

			if (rtnCode != XRC_GOOD)
			{
				return SetRtnCodeB(rtnCode);
			}

			List<Entity> subEntities = StorLibR.getSubEntities(e);
			subEntities.Add(e);

			using (Transaction T = new Transaction(AExId.Document, "Remove Cells Data"))
			{
				try
				{
					T.Start();
					{
						for (var i = subEntities.Count - 1; i >= 0; i--)
						{
							// remove all schema & entities (this has a bug)
							AExId.Document.EraseSchemaAndAllEntities(subEntities[i].Schema);
						}
					}

					// remove the ds
					StorLibR.DelDs(ds);

					T.Commit();
					rtnCode = XRC_GOOD;
				}
				catch
				{
					T.RollBack();
					rtnCode = XRC_FAIL;
				}
			}

			return SetRtnCodeB(rtnCode);
		}

		/// <summary>
		/// read the sheet data into a pre-initialized object<br/>
		/// expectation is to do this once at the begining
		/// </summary>
		/// <param name="initSheet"></param>
		/// <returns></returns>
		public ExStoreRtnCode ReadSheet(ShtExId exid, out TSht shtd)
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("begin readsheet");
			// sheet must be populated.  it can have the current, used data set or
			// it can have a initialised data set.  this is needed as the data set
			// has the field information needed to read the data

			shtd = null;

			DataStorage ds;
			Entity e;

			if (!SetRtnCodeB(StorLibR.FindEntity(exid, true, out ds, out e)))
			{
				M.WriteLineStatus($"rtn false | {rtnCode}");
				return ReturnCode;
			}

			shtd = SchemaLibR.ReadSheet(e);

			M.WriteLineStatus("done & exit");

			return SetRtnCodeE();
		}

		/// <summary>
		/// try to save the data to the DB <br/>
		/// fail if <br/>
		/// XRC_FAIL | sheetPrior is null || exid is null<br/>
		/// XRC_DS_EXISTS | ds exists<br/>
		/// XRC_LOCK_EXISTS | is locked
		/// </summary>
		/// <returns></returns>
		public bool WriteSheet(ShtExId shtExid, LokExId lokExid, TSht shtd)
		{
			M.WriteLineCodeMap();
			string userName;

			if (shtd == null || shtExid == null) return SetRtnCodeB(XRC_FAIL);

			if (StorLibR.DoesDsExist(shtExid)) return SetRtnCodeB(XRC_DS_EXISTS);

			if (SetRtnCodeB(StorLibR.DoesLockExist(lokExid, out userName))) return SetRtnCodeB(XRC_LOCK_EXISTS);

			using (Transaction T = new Transaction(AExId.Document, "Cells| Write Sheet"))
			{
				T.Start();

				DataStorage ds;

				SetRtnCodeB(StorLibR.CreateDataStorage(shtExid, out ds));

				if (SetRtnCodeB())
				{
					// write lock

					M.WriteLineStatus("write sheet");

					if (SetRtnCodeB(SchemaLibR.WriteSheet(ds, shtd)))
					{
						M.WriteLineStatus($"sheet written| status| {ReturnCode}");
						T.Commit();
					}
					else
					{
						T.RollBack();
					}

					// delete lock
				}
			}

			if (!SetRtnCodeB()) M.WriteLineStatus("create ds failed");

			return SetRtnCodeB();
		}

		/*
		/// <summary>
		/// try to save the data to the DB <br/>
		/// fail if <br/>
		/// XRC_FAIL | sheet is null || exid is null<br/>
		/// XRC_DS_EXISTS | ds exists<br/>
		/// XRC_LOCK_EXISTS | is locked
		/// </summary>
		/// <returns></returns>
		public bool UpdateSheet(ShtExId shtExid, TSht shtd)
		{
			
			M.WriteLineCodeMap();
			string userName;

			if (shtd == null || shtExid == null) return SetRtnCodeB(XRC_FAIL);

			DataStorage ds;

			StorLibR.FindDs(shtExid, true, out ds);

			using (Transaction T = new Transaction(AExId.Document, "Cells| Write Sheet"))
			{
				T.Start();

				M.WriteLineStatus("write sheet");

				if (SetRtnCodeB(SchemaLibR.WriteSheet(ds, shtd)))
				{
					M.WriteLineStatus($"sheet written| status| {ReturnCode}");
					T.Commit();
				}
				else
				{
					T.RollBack();
				}
			}

			return SetRtnCodeB();
		}
		*/

	#endregion

	#region public methods lock

		/// <summary>
		/// write a lock for the subject provided
		/// </summary>
		/// <param name="lokExid"></param>
		/// <returns></returns>
		public bool WriteLock(LokExId lokExid, TLok lokd)
		{
			M.WriteLineCodeMap();

			M.WriteLineStatus("start write lock");

			// get the lock owner name
			// if we get a result, then the lock already exists
			// return false - cannot create the lock - lock owner
			// does not matter - only matters that the lock exists
			string owner;

			M.WriteLineStatus("does a lock exist?");

			// if returns true, lock exists - bail
			if (!SetRtnCodeB(ReadLockOwner(lokExid, out owner), XRC_LOCK_NOT_EXIST))
			{
				M.WriteLineStatus("does a lock exist?| yes - bail");
				return false;
			}

			M.WriteLineStatus("does a lock exist?| no - continue");

			using (Transaction T = new Transaction(AExId.Document, "Cells| Write Lock"))
			{
				T.Start();

				if (!SetRtnCodeB(SchemaLibR.WriteLock(lokExid, lokd)))
				{
					M.WriteLineStatus($"write lock storage | code| {ReturnCode}");
					T.RollBack();
					return false;
				}

				T.Commit();
			}

			return true;
		}

		/// <summary>
		/// Read and return the lock data
		/// </summary>
		public ExStoreRtnCode ReadLock(
			LokExId lokExid, bool matchUserName, out TLok lokd)
		{
			M.WriteLineCodeMap();
			lokd = null;
			Entity e;
			DataStorage ds;

			if ((StorLibR.FindEntity(lokExid, matchUserName, out ds, out e) != XRC_GOOD)) return XRC_LOCK_NOT_EXIST;

			lokd = SchemaLibR.ReadLock(e);

			return XRC_GOOD;
		}

		public ExStoreRtnCode ReadLock(LokExId lokExid, out TLok lokd,
			bool matchUserName, out DataStorage ds, out Schema s, out Entity e)
		{
			M.WriteLineCodeMap();
			lokd = null;
			ds = null;
			s = null;
			e = null;

			StorLibR.FindElements(lokExid, matchUserName, out ds, out s, out e);

			m.WriteLineStatus($"find element return code| {StorLibR.ReturnCode}");

			if (e == null || !e.IsValidObject) return SetRtnCodeE(XRC_LOCK_NOT_EXIST);

			lokd = SchemaLibR.ReadLock(e);

			return XRC_GOOD;
		}

		/// <summary>
		/// get the name of the lock owner and<br/>
		/// return true if a lock exists
		/// </summary>
		public bool GetLockOwnerFromName(LokExId exid, out string userName)
		{
			
			M.WriteLineCodeMap();
			return SetRtnCodeB(StorLibR.DoesLockExist(exid, out userName));
		}

		/// <summary>
		/// return the owner name string
		/// </summary>
		/// <param name="exid"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public ExStoreRtnCode ReadLockOwner(LokExId lokExid, out string owner)
		{
			
			M.WriteLineCodeMap();
			TLok lokd;
			owner = null;

			ExStoreRtnCode rtnCode = ReadLock(lokExid, true, out lokd);

			if (rtnCode != XRC_GOOD) return rtnCode;

			owner = lokd.UserName;

			return XRC_GOOD;
		}
		/*
		/// <summary>
		/// delete the lock
		/// </summary>
		/// <param name="exid"></param>
		/// <returns></returns>
		public ExStoreRtnCode DeleteLock(LokExId lokExid)
		{
			
			M.WriteLineCodeMap();
			ExStoreRtnCode rtnCode = XRC_GOOD;

			TLok lokd;
			DataStorage ds;
			Schema s;
			Entity e;

			// determines if the lock exists and gets the objects needed
			// must match username also
			rtnCode = ReadLock(lokExid, out lokd, true, out ds, out s, out e);

			if (!SetRtnCodeB(rtnCode)) return rtnCode;

			if (!lokExid.UserNameMatches(lokd.UserName)) return SetRtnCodeE(XRC_USERNAME_MISMATCH);

			using (Transaction T = new Transaction(AExId.Document, "Delete Lock"))
			{
				try
				{
					T.Start();

					AExId.Document.EraseSchemaAndAllEntities(s);

					AExId.Document.Delete(ds.Id);

					T.Commit();
					rtnCode = XRC_GOOD;
				}
				catch
				{
					T.RollBack();
					rtnCode = XRC_FAIL;
				}
			}

			return rtnCode;
		}
		*/

		/// <summary>
		/// delete the lock
		/// </summary>
		/// <param name="exid"></param>
		/// <returns></returns>
		public bool DeleteLock(LokExId lokExid)
		{
			
			M.WriteLineCodeMap();
			ExStoreRtnCode rtnCode = XRC_GOOD;

			TLok lokd;
			DataStorage ds;
			Schema s;
			Entity e;

			// determines if the lock exists and gets the objects needed
			// must match username also
			SetRtnCodeB(ReadLock(lokExid, out lokd, true, out ds, out s, out e));

			M.WriteLineStatus($"A return code| {ReturnCode}");

			if (!SetRtnCodeB()) return false;

			if (!lokExid.UserNameMatches(lokd.UserName)) return SetRtnCodeB(XRC_USERNAME_MISMATCH);

			using (Transaction T = new Transaction(AExId.Document, "Delete Lock"))
			{
				try
				{
					T.Start();

					AExId.Document.EraseSchemaAndAllEntities(s);

					AExId.Document.Delete(ds.Id);

					T.Commit();
					SetRtnCodeB(XRC_GOOD);
				}
				catch
				{
					T.RollBack();
					SetRtnCodeB(XRC_FAIL);
				}
			}

			return SetRtnCodeB();
		}

		// primitive version - assumes lock exists and is within a transaction
		public ExStoreRtnCode DeleteLockP(LokExId lokExid)
		{
			
			M.WriteLineCodeMap();
			ExStoreRtnCode rtnCode = XRC_GOOD;

			TLok lokd;
			DataStorage ds;
			Schema s;
			Entity e;

			// determines if the lock exists (returns true) and gets the objects needed
			// must match username also
			if (SetRtnCodeB(ReadLock(lokExid, out lokd, true, out ds, out s, out e)))
			{
				// lock exists

				ReturnCode = XRC_GOOD;

				// check username
				if (!lokExid.UserNameMatches(lokd.UserName)) return SetRtnCodeE(XRC_USERNAME_MISMATCH);

				lokExid.Doc.EraseSchemaAndAllEntities(s);
				lokExid.Doc.Delete(ds.Id);
			}

			return SetRtnCodeE();
		}



		/// <summary>
		/// determine if a lock exists and can be deleted (user name matches)<br/>
		/// return<br/>
		/// true if it can be deleted (found and has the same user name)<br/>
		/// false if it cannot delete (not found or has a different user name)
		/// </summary>
		public bool CanDeleteLock(LokExId lokExid, out TLok lokd)
		{
			
			M.WriteLineCodeMap();
			bool result = SetRtnCodeB(ReadLock(lokExid, true, out lokd));

			if (!result || lokd == null) return false;

			return true;
		}

	#endregion

	#region what elements exist

		public bool SheetDsFound { get; private set; }
		public bool SheetEntityFound { get; private set; }
		public bool SheetSchemaFound { get; private set; }

		public bool LockADsFound { get; private set; }
		public bool LockASchemaFound { get; private set; }
		public bool LockAEntityFound { get; private set; }

		public bool LockBDsFound { get; private set; }
		public bool LockBSchemaFound { get; private set; }
		public bool LockBEntityFound { get; private set; }


		public void doTheyExist(ShtExId shtExid, LokExId lokExid, LokExId lokBExid)
		{
			
			M.WriteLineCodeMap();
			M.WriteLine("\nChecking if sheet ds and sheet schema exist");

			TestWhatExists(shtExid, lokExid, lokBExid);

			M.WriteLine("sheet ds found           |", $"{SheetDsFound} | name| {shtExid.DsName}");
			M.WriteLine("sheet schema found       |", $"{SheetSchemaFound} | name| {shtExid.SchemaName}");
			M.WriteLine("sheet entity found       |", $"{SheetEntityFound}");

			M.WriteLine("lock A jeff Ds found     |", $"{LockADsFound} | name| {lokExid.DsName}");
			M.WriteLine("lock A jeff schema found |", $"{LockASchemaFound}");
			M.WriteLine("lock A jeff entity found |", $"{LockAEntityFound}");

			M.WriteLine("lock B john Ds found     |", $"{LockBDsFound} | name| {lokBExid.DsName}");
			M.WriteLine("lock B john schema found |", $"{LockBSchemaFound}");
			M.WriteLine("lock B john entity found |", $"{LockBEntityFound}");
		}

		public void TestWhatExists(ShtExId shtExid, LokExId lokExid, LokExId lokBExid)
		{
			
			M.WriteLineCodeMap();
			M.WriteLineStatus("begin whatexists");

			// a full load
			// (1) ds which will hold
			// (1) sheet entity / sheet schema
			// (1) lock entity / lock schema
			// (0) or more row entities / schemas

			getSheetStatus(shtExid);
			getLockElementStatus(lokExid);
			getLockTempElementStatus(lokBExid);
		}

		private void getSheetStatus(ShtExId shtExid)
		{
			
			M.WriteLineCodeMap();
			bool ds;
			bool s;
			bool e;

			StorLibR.DoElementsExist(
				shtExid, out ds, out s, out e);

			SheetDsFound     = ds;
			SheetSchemaFound = s;
			SheetEntityFound = e;
		}

		private void getLockElementStatus(LokExId lokExid)
		{
			
			M.WriteLineCodeMap();
			bool ds;
			bool s;
			bool e;

			StorLibR.DoElementsExist(
				lokExid, out ds, out s, out e);

			LockADsFound     = ds;
			LockASchemaFound = s;
			LockAEntityFound = e;
		}

		private void getLockTempElementStatus(LokExId lokBExid)
		{
			
			M.WriteLineCodeMap();
			bool ds;
			bool s;
			bool e;

			StorLibR.DoElementsExist(
				lokBExid, out ds, out s, out e);

			LockBDsFound     = ds;
			LockBSchemaFound = s;
			LockBEntityFound = e;
		}

	#endregion



		// A check that the date provided is not null
		// C	check that no lock exists
		// D	create lock
		// E	if current sheet exists - remove it
		// E	write the sheet
		// F	remove the lock
		public ExStoreRtnCode UpdateSheet(ShtExId shtExid, TSht shtd, out string lockOwner)
		{
			M.WriteLineCodeMap();

			lockOwner = null;

			m.WriteLineStatus("begin updateSheet");
			m.WriteLineStatus("step A");

			// A
			if (shtExid == null)  return SetRtnCodeE(XRC_DATA_NOT_FOUND);

			m.WriteLineStatus("step C - get lock owner");

			//C
			LokExId lokExid = new LokExId(shtExid, LokExId.PRIME);

			m.WriteLineStatus($"lock info| username| {lokExid.UserName}");
			m.WriteLineStatus($"lock info| sch name| {lokExid.SchemaName}");
			m.WriteLineStatus($"lock info|  ds name| {lokExid.DsName}");


			if (GetLockOwnerFromName(lokExid, out lockOwner))
			{
				return SetRtnCodeE(XRC_LOCK_EXISTS);
			}

			TLok lokd = new TLok();
			lokd.Configure(lokExid);

			m.WriteLineStatus("before transaction");

			using (Transaction T = new Transaction(shtExid.Doc, "Cells: Save Sheet Information"))
			{
				try
				{
					T.Start();
					m.WriteLineStatus("transaction started");

					m.WriteLineStatus("step D - write lock");
					m.WriteLineStatus($"write lock| ds name| {lokExid.DsName}");

					// D	write the lock
					if (SetRtnCodeB(SchemaLibR.WriteLock(lokExid, lokd)))
					{

						// TLok tl;

						// m.WriteLineStatus("step D.1 - read lock");
						// ExStoreRtnCode rc = ReadLock(lokExid, false, out tl);



						// if worked, proceed - else, bypass all
						
						m.WriteLineStatus("step D.2 - del sheet");
						// delete the existing sheet or not - does not matter
						StorLibR.DelSheet(shtExid);
						m.WriteLineStatus($"step D.2 - del sheet| status| {StorLibR.ReturnCode}");

						DataStorage ds;

						m.WriteLineStatus("step D.3 - create ds");

						if (SetRtnCodeB(StorLibR.CreateDataStorage(shtExid, out ds)))
						{

							m.WriteLineStatus("step E write sheet");
							// ds made, proceed
							//E
							if (SetRtnCodeB(SchemaLibR.WriteSheet(ds, shtd)))
							{
								// writing the sheet worked
								m.WriteLineStatus("step F - delete lock");
								// must always delete the lock
								// must always work or something very weird happened
								// F
								if (SetRtnCodeB(DeleteLockP(lokExid)))
								{
									m.WriteLineStatus("transaction commit");
									// updated - commit;
									T.Commit();
								}
								else
								{

									m.WriteLineStatus("transaction cancel 0| delete lock failed");
									m.WriteLineStatus($"return code| {ReturnCode}");
									T.RollBack();
								}
							}
							else
							{
								m.WriteLineStatus("transaction cancel 1| cannot write sheet");

								// if could not write the sheet, back-out
								// this will rollback the deletion of the sheet
								// and the creation of the lock
								// and the creation of the ds
								T.RollBack();
							}
						}
						else
						{
							m.WriteLineStatus("transaction cancel 2");
							// if could not make the new ds, back-out
							// this will rollback the deletion of the sheet and the creation of the lock
							T.RollBack();
						}
					}
				}
				catch
				{
					m.WriteLineStatus("transaction cancel 3");

					T.RollBack();
					SetRtnCodeE(XRC_FAIL);
				}

				m.WriteLineStatus("transaction end");
			}

			m.WriteLineStatus("complete");

			return SetRtnCodeE();;
		}
	}
}