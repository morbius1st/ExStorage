#region + Using Directives

using static ShExStorageN.ShExStorage.ExStoreRtnCode;

#endregion

// user name: jeffs
// created:   11/18/2022 9:38:37 PM

namespace ShExStorageR.ShExStorage
{
	[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
	[Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
	public class ShExStorManagerR2<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds> : INotifyPropertyChanged
		where TSht : AShScSheet2<TShtKey, TShtFlds, TRowKey, TRowFlds, TRow>, new()
		where TRow : AShScRow2<TRowKey, TRowFlds>, new()
		where TLok : AShScLock2<TLokKey, TLokFlds>, new()
		where TShtKey : sKey
		where TShtFlds : ScFieldDefData2<TShtKey>, new()
		where TRowKey : rKey
		where TRowFlds : ScFieldDefData2<TRowKey>, new()
		where TLokKey : lKey
		where TLokFlds : ScFieldDefData2<TLokKey>, new()
	{
	#region fields

		private static readonly Lazy<ShExStorManagerR2<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>> instance =
			new Lazy<ShExStorManagerR2<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>>(() =>
				new ShExStorManagerR2<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>());

		public static ShExStorManagerR2<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds> Instance => instance.Value;

		private ShDebugMessages m;
		
		// private bool hasData;
		private ExStoreRtnCode rtnCode;

	#endregion

	#region config

		private ShExStorManagerR2()
		{
			config();
		}

		private void config()
		{
			SchemaLibR2 = new ShExSchemaLibR2<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>();
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
				SchemaLibR2.M = value;
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
			return $"this is {nameof(ShExStorManagerR2<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>)}";
		}

	#endregion

	#region public properties

		// schema creation and data saving methods
		public ShExSchemaLibR2<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds> SchemaLibR2 { get; set; }

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
			where TTbl : AShScFields2<TKey, TField>, new()
			where TKey : KEY
			where TField : IShScFieldData2<TKey>, new()
		{
			table = new TTbl();
			Entity e;
			DataStorage ds;

			if ((StorLibR.FindEntity(exid, true, out ds, out e) != XRC_GOOD)) return XRC_LOCK_NOT_EXIST;

			table = ReadTable<TTbl, TKey, TField>(e);

			return XRC_GOOD;
		}

		public TTbl ReadTable<TTbl, TKey, TField>(Entity e)
			where TTbl : AShScFields2<TKey, TField>, new()
			where TKey : KEY
			where TField : IShScFieldData2<TKey>, new()
		{
			TTbl table = new TTbl();

			SchemaLibR2.ReadData(e, table);

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
		private bool SetRtnCodeB(ExStoreRtnCode rtnCode = XRC_VOID, ExStoreRtnCode testCode = XRC_GOOD)
		{
			if (rtnCode != XRC_VOID)
			{
				ReturnCode = rtnCode;
			}

			return this.rtnCode == testCode;
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

			shtd = SchemaLibR2.ReadSheet(e);

			// OnPropertyChanged(nameof(Sheet));

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

					if (SetRtnCodeB(SchemaLibR2.WriteSheet(ds, shtd)))
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

	#endregion

	#region public methods lock

		/// <summary>
		/// write a lock for the subject provided
		/// </summary>
		/// <param name="lokExid"></param>
		/// <returns></returns>
		public bool WriteLock(LokExId lokExid, TLok lokd)
		{
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

				DataStorage ds;

				if (!SetRtnCodeB(StorLibR.CreateDataStorage(lokExid, out ds)))
				{
					M.WriteLineStatus($"create data storage | code| {ReturnCode}");
					T.RollBack();
					return false;
				}

				if (!SetRtnCodeB(SchemaLibR2.WriteLock(ds, lokd)))
				{
					M.WriteLineStatus($"write lock | code| {ReturnCode}");
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
			lokd = null;
			Entity e;
			DataStorage ds;

			if ((StorLibR.FindEntity(lokExid, matchUserName, out ds, out e) != XRC_GOOD)) return XRC_LOCK_NOT_EXIST;

			lokd = SchemaLibR2.ReadLock(e);

			return XRC_GOOD;
		}

		public ExStoreRtnCode ReadLock(LokExId lokExid, out TLok lokd, 
			bool matchUserName, out DataStorage ds, out Schema s, out Entity e)
		{
			lokd = null;
			ds = null;
			s = null;
			e = null;

			StorLibR.FindElements(lokExid, matchUserName, out ds, out s, out e);

			if (e == null || !e.IsValidObject) return SetRtnCodeE(XRC_LOCK_NOT_EXIST);

			lokd = SchemaLibR2.ReadLock(e);

			return XRC_GOOD;
		}

		/// <summary>
		/// get the name of the lock owner and<br/>
		/// return true if a lock exists
		/// </summary>
		public bool GetLockOwnerFromName(LokExId exid, out string userName)
		{
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
			TLok lokd;
			owner = null;

			ExStoreRtnCode rtnCode = ReadLock(lokExid, true, out lokd);

			if (rtnCode != XRC_GOOD) return rtnCode;

			owner = lokd.UserName;

			return XRC_GOOD;
		}

		/// <summary>
		/// delete the lock
		/// </summary>
		/// <param name="exid"></param>
		/// <returns></returns>
		public ExStoreRtnCode DeleteLock(LokExId lokExid)
		{
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

		/// <summary>
		/// determine if a lock exists and can be deleted (user name matches)<br/>
		/// return<br/>
		/// true if it can be deleted (found and has the same user name)<br/>
		/// false if it cannot delete (not found or has a different user name)
		/// </summary>
		public bool CanDeleteLock(LokExId lokExid, out TLok lokd)
		{
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

		// public ShExSchemaLibR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds> ShExSchemaLibR
		// {
		// 	get => default;
		// 	set
		// 	{
		// 	}
		// }
		//
		// public ShExStorageLibR ShExStorageLibR
		// {
		// 	get => default;
		// 	set
		// 	{
		// 	}
		// }
	}
}