#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStorage.TestProcedures;
using JetBrains.Annotations;
using SettingsManager;
using SharedApp.Windows.ShSupport;
using ShExStorageC.ShExStorage;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ScSupport;
using static ShExStorageN.ShExStorage.ExStoreRtnCode;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields;
using ShExStorageR.ShExStorage;
using ShStudy.ShEval;
using TestProcedures01 = ExStorage.TestProcedures.TestProcedures01;
using Autodesk.Revit.UI;
using ShExStorageN.ShSchemaFields.ShScSupport;

#endregion

// username: jeffs
// created:  10/2/2022 9:37:41 AM

namespace ExStorage.Windows
{
	public class MainWindowModel : INotifyPropertyChanged
		//: ShDebugSupport
	{
	#region private fields

		private ShFieldDisplayData shFd;
		private ShowLibrary sl;
		private ShDebugMessages M { get; set; }

		private ExStoreRtnCode rtnCode;

		private TestProcedures01 tp01;
		private ShowsProcedures01 sp01;
		private ShShowProcedures01 shsp01;


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

		// fixed values
		private readonly ShtExId sheetExidCurrent;
		private readonly LokExId lockExidCurrent;

		private readonly ScDataLock lockDataCurrent;


		// not fixed values

		// sheet
		private ScDataSheet sheetDataCurrent;
		private ScDataSheet sheetDataEditing;

		private ScDataSheet sheetDataTemp;
		private ScDataSheet sheetDataPrior;

		// row
		private ScDataRow row;

		// lock
		private ScDataLock lockDataTemp;
		private ScDataLock lockDataBJohn;
		private ScDataLock lockDataPrior;

		// exid 

		private ShtExId sheetExidTemp;
		private LokExId lockExidTemp;
		private LokExId lockExidBJohn;


		// private bool sheetDsFound;
		// private DataStorage sheetDs;
		//
		// private DataStorage lockDs;
		// private DataStorage lockTempDs;


	#endregion

	#region ctor

		public MainWindowModel(ShDebugMessages msgs, ShtExId exid)
		{
			M = msgs;

			// fixed data

			sheetExidCurrent = exid;
			OnPropertyChanged(nameof(SheetExidCurrent));

			lockExidCurrent = new LokExId("Lock jeff", LokExId.PRIME);
			OnPropertyChanged(nameof(LockExidCurrent));

			createFixedLockData(out lockDataCurrent);

			config(msgs, exid);

			// this.exid = exid;

			// UnitType u = UnitType.UT_Acceleration;
			//
			// ForgeTypeId ut = SpecTypeId.Length;
			// ForgeTypeId dt = UnitTypeId.Feet;
			// ForgeTypeId st = SymbolTypeId.FootSingleQuote;
		}

	#endregion

	#region public properties

		// return code

		public ExStoreRtnCode ReturnCode
		{
			get => rtnCode;
			set
			{
				if (value==rtnCode) return;
				rtnCode = value;
				OnPropertyChanged();
			}
		}

		// Exid

		public ShtExId SheetExidCurrent
		{
			get => sheetExidCurrent;

			// private set
			// {
			// 	exidShtCurrent = value;
			// 	OnPropertyChanged();
			// }
		}
		public LokExId LockExidCurrent
		{
			get => lockExidCurrent;
			
			// set
			// {
			// 	if (Equals(value, lockExidCurrent)) return;
			// 	lockExidCurrent = value;
			// 	OnPropertyChanged();
			// }
		}

		public ScDataLock LockDataCurrent
		{
			get => lockDataCurrent;
			
			// set
			// {
			// 	if (Equals(value, lockDataCurrent)) return;
			// 	lockDataCurrent = value;
			// 	OnPropertyChanged();
			// 	OnPropertyChanged(nameof(HasLockCurrent));
			// }
		}

		// sheet

		public ScDataSheet SheetDataCurrent
		{
			get => sheetDataCurrent;
			private set
			{
				sheetDataCurrent = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasSheet));
			}
		}

		public ScDataSheet SheetDataEditing
		{
			get => sheetDataEditing;
			private set
			{
				sheetDataEditing = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasSheetForEdit));
			}
		}

		public ScDataSheet SheetDataTemp
		{
			get => sheetDataTemp;
			private set
			{
				if (Equals(value, sheetDataTemp)) return;
				sheetDataTemp = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasSheetTemp));
			}
		}

		// lock

		public ScDataLock LockDataTemp
		{
			get => lockDataTemp;
			
			set
			{
				if (Equals(value, lockDataTemp)) return;
				lockDataTemp = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasLockTemp));
			}
		}

		public ScDataLock LockBDataJohn
		{
			get => lockDataBJohn;
			
			set
			{
				if (Equals(value, lockDataBJohn)) return;
				lockDataBJohn = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasLockBJohn));
			}
		}

		public ScDataLock LockDataPrior => lockDataPrior;

		// exid

		public ShtExId SheetExidTemp => sheetExidTemp;

		public LokExId LockExidATemp
		{
			get => lockExidTemp;
			
			set
			{
				if (Equals(value, lockExidTemp)) return;
				lockExidTemp = value;
				OnPropertyChanged();
			}
		}

		public LokExId LockExidBJohn
		{
			
			get => lockExidBJohn;
			
			set
			{
				if (Equals(value, lockExidBJohn)) return;
				lockExidBJohn = value;
				OnPropertyChanged();
			}
		}

		// status

		public bool HasSheet => SheetDataCurrent != null;
		public bool HasSheetForEdit => SheetDataCurrent != null;
		public bool HasSheetTemp => SheetDataTemp != null;
		public bool HasLockCurrent => LockDataCurrent != null;
		public bool HasLockTemp => LockDataTemp != null;
		public bool HasLockBJohn => LockBDataJohn != null;


	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(MainWindowModel)}";
		}

	#endregion

	#region private methods

		private void config(ShDebugMessages msgs, ShtExId exid)
		{
			M.WriteLineStatus("at start");

			sp01 = new ShowsProcedures01();
			tp01 = new TestProcedures01();

			shsp01 = new ShShowProcedures01(msgs);

			sl = new ShowLibrary(msgs);
			shFd = new ShFieldDisplayData();

			// not fixed data

			lockExidBJohn = new LokExId("Lock B john", LokExId.PRIME);
			lockExidBJohn.SetOverrideUserName("johns", LokExId.PRIME);

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

			M.WriteLineStatus("before config smR");

			smR.SetDebugMsg(msgs);
			smR.StorLibR.smR = smR;
			smR.SchemaLibR.smR = smR;

			M.WriteLineStatus("after config smr");

			StaticInfo.UpdateMainWinProperty(nameof(StaticInfo.MainWin.MwModel));

			// read the current data or initialize sheet
			if (!ReadSheet(sheetExidCurrent, out sheetDataCurrent))
			{
				sheetDataCurrent = null;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		[DebuggerStepThrough]
		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

	#endregion


	#region final routines

		// general

		private bool SetRtnCodeB(ExStoreRtnCode rtnCode = XRC_VOID, ExStoreRtnCode testCode = XRC_GOOD)
		{
			if (rtnCode != XRC_VOID)
			{
				ReturnCode = rtnCode;
			}

			return this.rtnCode == testCode;
		}

		private void SetSheet(ScDataSheet sht)
		{
			M.WriteLineStatus("set sheet");

			sheetDataPrior = sheetDataCurrent;

			SheetDataCurrent = sht;
			// SheetLoaded = true;
		}

		private void RemoveSheet()
		{
			M.WriteLineStatus("delete sheet");

			sheetDataPrior = sheetDataCurrent;

			SheetDataCurrent = null;

		}

		// sheet

		public void MakeSheetData()
		{
			M.WriteLineStatus("init sheet data");

			ScDataSheet shtd = ScData.MakeInitialDataSheet1(SheetExidCurrent);

			TestMakeDataRow3(SheetExidCurrent, shtd);

			SetSheet(shtd);
		}

		private ScDataRow CreateFauxRow(ShtExId exid, ScDataRow row)
		{
			tp01.MakeFauxRow(exid, row);

			return row;
		}

		public void TestMakeDataRow3(ShtExId exid, ScDataSheet shtd)
		{
			M.WriteLineStatus("init row data");

			// M.WriteLine("test make ROW data x3");
			ScDataRow rowd;

			rowd = CreateFauxRow(exid, ScData.MakeInitialDataRow1(shtd));
			shtd.AddRow(rowd);

			rowd = CreateFauxRow(exid, ScData.MakeInitialDataRow1(shtd));
			shtd.AddRow(rowd);

			rowd = CreateFauxRow(exid, ScData.MakeInitialDataRow1(shtd));
			shtd.AddRow(rowd);

			UserSettings.Data.UserSettingsValue += 1;

			UserSettings.Admin.Write();
		}

		public ScDataSheet InitSheet(ShtExId exid)
		{
			M.WriteLineStatus("initialize sheet");
		
			return ScData.MakeInitialDataSheet1(exid);
		}

		public bool ReadSheet(ShtExId shtExid, out ScDataSheet shtd)
		{
			if (!SetRtnCodeB(smR.ReadSheet(shtExid, out shtd)))
			{
				shtd = null;
			}

			return SetRtnCodeB();
		}


		/// <summary>
		/// read the sheet data - this is done at the start<br/>
		/// when the dialog is opened
		/// </summary>
		// public bool ReadSheet(out ScDataSheet shtd)
		// {
		// 	return SetRtnCodeB(smR.ReadSheet(SheetExidCurrent, out shtd));
		// }

		/// <summary>
		/// write the sheet data to the model<br/>
		/// return <br/>
		/// true if it worked<br/>
		/// false if not (
		/// </summary>
		/// <param name="shtd"></param>
		public bool WriteSheet(ShtExId shtExid, LokExId lokExid, ScDataSheet shtd)
		{
			if (shtd == null) return false;
		
			return smR.WriteSheet(shtExid,	lokExid, shtd);
		}

		/// <summary>
		/// remove the current sheet - ds & schemas
		/// </summary>
		public void DeleteSheet()
		{
			M.WriteLine("sheet delete");

			bool result = smR.DeleteSheet(SheetExidCurrent, lockExidCurrent);

			if (result)
			{
				M.WriteLine("sheet deleted - WORKED");
				RemoveSheet();
			}
			else
			{
				M.WriteLine("sheet delete - FAILED");
			}
		}

		public void DoesSheetLockExist()
		{
			smR.DoesSheetLockExist(lockExidCurrent);
		}

		public bool GetUserNameLock(LokExId exid, out string owner)
		{
			M.WriteLineStatus("get lock current owner Id");

			return smR.GetLockOwnerFromName(exid, out owner);
		}



		// lock

		public void CreateLockData(LokExId lokExid, out ScDataLock lokd)
		{
			M.WriteLineStatus("lock set");

			lokd = new ScDataLock();
			lokd.Configure(lokExid);
		}

		private void createFixedLockData(out ScDataLock lokd)
		{
			CreateLockData(lockExidCurrent, out lokd);
		}

		// lock current

		public bool CreateLockCurrent()
		{
			bool result;
			M.WriteLineStatus("start write lock Current");

			result = smR.WriteLock(lockExidCurrent, lockDataCurrent);

			M.WriteLineStatus($"write lock data | {result}");

			if (!result) return false;

			// doTheyExist();

			StaticInfo.UpdateMainWinProperty(nameof(StaticInfo.MainWin.MwModel));

			M.WriteLine($"lock Current created| {lockExidCurrent.DsName}");

			return result;

		}

		public bool ReadLockCurrent(out ScDataLock lokd)
		{
			M.WriteLineStatus($"read lock Current | lockcurrent set to null");
			// lockDataCurrent = null;

			rtnCode = smR.ReadLock(lockExidCurrent, true, out lokd);

			if (rtnCode != XRC_GOOD)
			{
				M.WriteLine($"lock current read FAILED");
				return SetRtnCodeB(XRC_LOCK_NOT_EXIST);
			}

			M.WriteLine($"lock current read WORKED");

			M.NewLine();

			ShowLockA();

			return true;
		}

		public bool GetUserNameLockCurrent(out string owner)
		{
			M.WriteLineStatus("get lock current owner Id");

			return smR.GetLockOwnerFromName(lockExidCurrent, out owner);

			// rtnCode = smR.ReadLockOwner(lockExidCurrent, out owner);
			//
			// if (rtnCode != XRC_GOOD)
			// {
			// 	M.WriteLineStatus("get lock current owner Id failed");
			// 	return false;
			// }
			//
			// M.WriteLineStatus("got lock current owner Id");
			// return true;
		}

		public bool CanDeleteLockCurrent()
		{
			ScDataLock lokd;

			bool result = smR.CanDeleteLock(lockExidCurrent, out lokd);

			// if (result && lokd != null)
			// {
			// 	lockDataCurrent = lokd;
			// }

			return result;
		}

		public bool DeleteLockCurrent()
		{
			ExStoreRtnCode rtnCode =
				smR.DeleteLock(lockExidCurrent);

			if (rtnCode != XRC_GOOD) return false;

			doTheyExist();

			return true;
		}

		// lock B johns

		public void CreateLockBData()
		{
			ScDataLock lokd;

			CreateLockData(lockExidBJohn, out lokd);
			
			LockBDataJohn = lokd;

			M.WriteLineStatus($"lock data created");
		}

		public bool CreateLockB()
		{
			bool result;
			M.WriteLineStatus("start write lock B");

			// result = CreateLockData(exidBJohn, out lockBJohn);
			// OnPropertyChanged(nameof(LockBJohn));
			//
			// M.WriteLineStatus($"create lock data | {result}");
			//
			// if (!result) return false;

			result = smR.WriteLock(lockExidBJohn, lockDataBJohn);

			M.WriteLineStatus($"write lock data | {result}");

			if (!result) return false;

			// doTheyExist();

			StaticInfo.UpdateMainWinProperty(nameof(StaticInfo.MainWin.MwModel));

			M.WriteLine($"lock B created| {lockExidBJohn.DsName} user name| {lockDataBJohn.UserName}");

			return result;

			//
			// M.WriteLineStatus("start write lock B");
			// bool result;
			//
			// // get the lock B owner name
			// // if we get a result, then the lock already exists
			// // return false - cannot create the lock
			// string owner;
			//
			// result = GetUserNameLockB(out owner);
			//
			// if (result) return false;
			//
			// using (Transaction T = new Transaction(AExId.Document, "create cells lock B"))
			// {
			// 	T.Start();
			//
			// 	DataStorage ds;
			//
			// 	result = CreateLockData(exidBJohn, out lockTemp);
			//
			// 	if (!result)
			// 	{
			// 		M.WriteLineStatus($"write lock B status (create lock data) | {result} | code| {smR.ReturnCode}");
			// 		T.RollBack();
			// 		return false;
			// 	}
			//
			// 	result = SetRtnCode(smR.StorLibR.CreateDataStorage(exidBJohn, out ds));
			//
			// 	if (!result)
			// 	{
			// 		M.WriteLineStatus($"write lock B status (create data storage) | {result} | code| {smR.ReturnCode}");
			// 		T.RollBack();
			// 		return false;
			// 	}
			//
			// 	result = SetRtnCode(smR.SchemaLibR.WriteLock(ds, lockTemp));
			//
			// 	if (!result)
			// 	{
			// 		M.WriteLineStatus($"write lock B status (write lock) | {result} | code| {smR.ReturnCode}");
			// 		T.RollBack();
			// 		return false;
			// 	}
			//
			// 	T.Commit();
			// }
			//
			// doTheyExist();
			//
			// StaticInfo.UpdateMainWinProperty(nameof(StaticInfo.MainWin.MwModel));
			//
			// M.WriteLine($"lock B created| {exidBJeff.DsName}");
			//
			// return result;
		}

		public bool ReadLockB(out ScDataLock lokd)
		{
			M.WriteLineStatus($"read lock B | lockTemp set to null");
			lockDataBJohn = null;

			rtnCode = smR.ReadLock(lockExidBJohn, true, out lokd);

			if (rtnCode != XRC_GOOD)
			{
				M.WriteLine($"lock B read FAILED");
				return SetRtnCodeB(XRC_LOCK_NOT_EXIST);
			}

			M.WriteLine($"lock B read WORKED");

			M.NewLine();

			ShowLockB();

			return true;
		}

		public bool GetUserNameLockB(out string owner)
		{
			M.WriteLineStatus("get lock B owner Id");

			rtnCode = smR.ReadLockOwner(lockExidCurrent, out owner);

			if (rtnCode != XRC_GOOD)
			{
				M.WriteLineStatus("get lock B owner Id failed");
				return false;
			}

			M.WriteLineStatus("got lock B owner Id");
			return true;
		}

		public bool CanDeleteLockB()
		{
			ScDataLock lokd;

			bool result = smR.CanDeleteLock(lockExidBJohn, out lokd);

			if (result && lokd != null)
			{
				lockDataBJohn = lokd;
			}

			return result;
		}

		public bool DeleteLockBjeffs()
		{
			ExStoreRtnCode rtnCode =
				smR.DeleteLock(lockExidCurrent);

			if (rtnCode != XRC_GOOD) return false;

			doTheyExist();

			return true;
		}

		public bool DeleteLockBjohns()
		{
			ExStoreRtnCode rtnCode =
				smR.DeleteLock(lockExidBJohn);

			if (rtnCode != XRC_GOOD) return false;

			doTheyExist();

			return true;
		}

	#endregion


	#region tables

		public void GetTables()
		{
			ScDataSheet sheetA;
			ScDataLock lockA;
			ScDataLock lockJeff;
			ScDataLock lockJohn;


			ExStoreRtnCode rtnCode;

			// rtnCode = smR.ReadTable<
			// 	ScDataSheet1,
			// 	SchemaSheetKey, 
			// 	ScFieldDefData1<SchemaSheetKey>
			// 	>(exidSht, out sheetA);
			//
			// if (rtnCode == XRC_GOOD)
			// {
			// 	M.WriteLine($"Read Sheet Table WORKED| {sheetA.SchemaName}");
			// }
			// else
			// {
			// 	M.WriteLine("Read Sheet Table FAILED");
			// }

			rtnCode = smR.ReadTable<
				ScDataLock,
				SchemaLockKey, 
				ScFieldDefData<SchemaLockKey>
				>(lockExidCurrent, out lockA);

			if (rtnCode == XRC_GOOD)
			{
				M.WriteLine($"Read Lock A Table WORKED| {lockA.SchemaName}");
				shsp01.ShowLockDataGeneric(lockA);
			}
			else
			{
				M.WriteLine("Read Lock A Table FAILED");
			}

			
			rtnCode = smR.ReadTable<
				ScDataLock,
				SchemaLockKey, 
				ScFieldDefData<SchemaLockKey>
				>(lockExidBJohn, out lockJohn);

			if (rtnCode == XRC_GOOD)
			{
				M.WriteLine($"Read Lock B-john Table WORKED| {lockJohn.SchemaName}");
				shsp01.ShowLockDataGeneric(lockJohn);
			}
			else
			{
				M.WriteLine("Read Lock B-john Table FAILED");
			}
		}

	#endregion


	#region show methods

		// show
		public void ShowExid(AExId exid)
		{
			M.WriteLineStatus("show exid");

			sp01.ShowEid(exid);
		}

		public void ShowSheet1()
		{
			M.WriteLineStatus("show sheet");

			shsp01.ShowSheetDataGeneric(SheetDataCurrent);
		}

		public void ShowLockA()
		{
			M.WriteLineStatus("show lock A");

			shsp01.ShowLockDataGeneric(LockDataCurrent);
		}

		public void ShowLockB()
		{
			M.WriteLineStatus("show lock B");

			shsp01.ShowLockDataGeneric(LockDataTemp);
		}

	#endregion

	#region tests

		// tests

		public void doTheyExist()
		{
			M.WriteLine("\nChecking if sheet ds and sheet schema exist");

			smR.doTheyExist(sheetExidCurrent, lockExidCurrent, lockExidBJohn);
		}

		// does it exist?

		// public bool SheetDsFound { get; private set; }
		//
		// public bool SheetEntityFound { get; private set; }
		// public bool SheetSchemaFound { get; private set; }
		//
		// public bool LockADsFound { get; private set; }
		// public bool LockASchemaFound { get; private set; }
		// public bool LockAEntityFound { get; private set; }
		//
		// public bool LockBDsFound { get; private set; }
		// public bool LockBSchemaFound { get; private set; }
		// public bool LockBEntityFound { get; private set; }


		// 	TestWhatExists();
		//
		// 	M.WriteLine("sheet ds found           |", $"{SheetDsFound} | name| {SheetExidCurrent.DsName}");
		// 	M.WriteLine("sheet entity found       |", $"{SheetEntityFound}");
		// 	M.WriteLine("sheet schema found       |", $"{SheetSchemaFound} | name| {SheetExidCurrent.SchemaName}");
		//
		// 	M.WriteLine("lock A jeff Ds found     |", $"{LockADsFound} | name| {lockExidCurrent.DsName}");
		// 	M.WriteLine("lock A jeff entity found |", $"{LockAEntityFound}");
		// 	M.WriteLine("lock A jeff schema found |", $"{LockASchemaFound}");
		//
		// 	M.WriteLine("lock B john Ds found     |", $"{LockBDsFound} | name| {lockExidBJohn.DsName}");
		// 	M.WriteLine("lock B john entity found |", $"{LockBEntityFound}");
		// 	M.WriteLine("lock B john schema found |", $"{LockBSchemaFound}");


		// public void TestWhatExists()
		// {
		// 	M.WriteLineStatus("begin whatexists");
		//
		// 	// a full load
		// 	// (1) ds which will hold
		// 	// (1) sheet entity / sheet schema
		// 	// (1) lock entity / lock schema
		// 	// (0) or more row entities / schemas
		//
		// 	getSheetStatus();
		// 	getLockElementStatus();
		// 	getLockTempElementStatus();
		// }
		//
		// private void getSheetStatus()
		// {
		// 	bool ds;
		// 	bool s;
		// 	bool e;
		//
		// 	smR.StorLibR.DoElementsExist(
		// 		lockExidCurrent, out ds, out s, out e);
		//
		// 	SheetDsFound     = ds;
		// 	SheetSchemaFound = s;
		// 	SheetEntityFound = e;
		// }
		//
		// private void getLockElementStatus()
		// {
		// 	bool ds;
		// 	bool s;
		// 	bool e;
		//
		// 	smR.StorLibR.DoElementsExist(
		// 		lockExidCurrent, out ds, out s, out e);
		//
		// 	LockADsFound     = ds;
		// 	LockASchemaFound = s;
		// 	LockAEntityFound = e;
		// }
		//
		// private void getLockTempElementStatus()
		// {
		// 	bool ds;
		// 	bool s;
		// 	bool e;
		//
		// 	smR.StorLibR.DoElementsExist(
		// 		lockExidBJohn, out ds, out s, out e);
		//
		// 	LockBDsFound     = ds;
		// 	LockBSchemaFound = s;
		// 	LockBEntityFound = e;
		// }

	#endregion

	#region final tests

		public bool TestWriteSheetCurrent()
		{
			M.WriteLineStatus("start write sheet");

			bool result;

			result = WriteSheet(sheetExidCurrent, lockExidCurrent, sheetDataCurrent);

			M.WriteLineStatus($"write sheet status| {result} | code| {smR.ReturnCode}");

			// doTheyExist();

			StaticInfo.UpdateMainWinProperty(nameof(StaticInfo.MainWin.MwModel));

			return result;
		}

		public bool TestReadSheetCurrent()
		{
			M.WriteLineStatus("start read sheet");

			ScDataSheet shtd;

			bool result = ReadSheet(SheetExidCurrent, out shtd);

			SheetDataCurrent = shtd;

			M.WriteLineStatus($"read sheet status| {result} | code| {smR.ReturnCode}");

			if (!result)
			{
				RemoveSheet();
			}

			return result;
		}


		
		public bool TestWriteSheetEditing()
		{
			M.WriteLineStatus("start write sheet");

			bool result;

			result = WriteSheet(sheetExidCurrent, lockExidCurrent, sheetDataEditing);

			M.WriteLineStatus($"write sheet status| {result} | code| {smR.ReturnCode}");

			// doTheyExist();

			StaticInfo.UpdateMainWinProperty(nameof(StaticInfo.MainWin.MwModel));

			return result;
		}

		public bool TestReadSheetEditing()
		{
			M.WriteLineStatus("start read sheet");

			ScDataSheet shtd;

			bool result = ReadSheet(sheetExidCurrent, out shtd);

			SheetDataEditing = shtd;

			M.WriteLineStatus($"read sheet status| {result} | code| {smR.ReturnCode}");

			return result;
		}


	#endregion

		// *** voided ***

		// public void InitLock()
		// {
		// 	M.WriteLineStatus("initialize lock");
		//
		// 	lockPrior = lockCurrent;
		//
		// 	lockCurrent = smR.SchemaLibR.MakeInitLock();
		// 	LockInit = true;
		// }



				//
		// public void GetAllDs1()
		// {
		// 	tp01.TestGetAllDs1(smR.StorLibR);
		// }
		//
		// // public void FindDsByName3()
		// // {
		// // 	tp01.TestFindDsByName3(smR.StorLibR);
		// // }
		// //
		// // public void GetSchemaByName2()
		// // {
		// // 	tp01.TestFindExistSchema2(smR.StorLibR);
		// // }
		// //
		// // public void GetDsEntity5()
		// // {
		// // 	tp01.TestGetDsEntity5(smR.StorLibR);
		// // }
		// //
		// // public void GetEntityData6()
		// // {
		// // 	tp01.TestGetEntityData6(smR.StorLibR);
		// // }
		// //
		// // public void DoesDsExist7()
		// // {
		// // 	tp01.TestDoesDsExist7(smR.StorLibR);
		// // }
		//
		// public void DeleteSheetDs(ShtExId exid)
		// {
		// 	smR.StorLibR.DelSheetDs(exid);
		// }
		//
		// // delete the sheet entity
		// public void DeleteSheetEntity(ShtExId exid)
		// {
		// 	ExStoreRtnCode rtnCode;
		//
		// 	DataStorage ds;
		//
		// 	M.WriteMsg("Delete sheet entity| ");
		//
		// 	rtnCode = smR.StorLibR.FindDs(exid, true, out ds);
		//
		// 	if (rtnCode == ExStoreRtnCode.XRC_GOOD)
		// 	{
		// 		Schema s;
		//
		// 		rtnCode = smR.StorLibR.FindSchema(exid, true, out s);
		//
		// 		if (rtnCode == ExStoreRtnCode.XRC_GOOD)
		// 		{
		// 			using (Transaction T = new Transaction(AExId.Document, "Delete DataStorage Entity"))
		// 			{
		// 				T.Start();
		// 				if (!ds.DeleteEntity(s))
		// 				{
		// 					rtnCode = ExStoreRtnCode.XRC_FAIL;
		// 				}
		//
		// 				T.Commit();
		// 			}
		// 		}
		// 	}
		//
		// 	if (rtnCode == ExStoreRtnCode.XRC_GOOD)
		// 	{
		// 		M.WriteLine("worked");
		// 	}
		// 	else
		// 	{
		// 		M.WriteLine("failed");
		// 	}
		// }
		//
		// // delete the sheet schema and associated entities
		// public void DeleteSheetSchema(ShtExId exid)
		// {
		// 	ExStoreRtnCode rtnCode;
		//
		// 	DataStorage ds;
		//
		// 	M.WriteMsg("Delete sheet schema| ");
		//
		// 	rtnCode = smR.StorLibR.FindDs(exid, true, out ds);
		//
		// 	if (rtnCode == ExStoreRtnCode.XRC_GOOD)
		// 	{
		// 		Schema s;
		//
		// 		rtnCode = smR.StorLibR.FindSchema(exid, true, out s);
		//
		// 		if (rtnCode == ExStoreRtnCode.XRC_GOOD)
		// 		{
		// 			using (Transaction T = new Transaction(AExId.Document, "Delete DataStorage Schema"))
		// 			{
		// 				T.Start();
		// 				AExId.Document.EraseSchemaAndAllEntities(s);
		// 				T.Commit();
		// 			}
		// 		}
		// 	}
		//
		// 	if (rtnCode == ExStoreRtnCode.XRC_GOOD)
		// 	{
		// 		M.WriteLine("worked");
		// 	}
		// 	else
		// 	{
		// 		M.WriteLine("failed");
		// 	}
		// }



		

		// // get the lock A owner name
		// // if we get a result, then the lock already exists
		// // return false - cannot create the lock
		// string owner;
		//
		// result = GetUserNameLockA(out owner);
		//
		// if (result) return false;
		//
		// using (Transaction T = new Transaction(AExId.Document, "create cells lock A"))
		// {
		// 	T.Start();
		//
		// 	DataStorage ds;
		//
		// 	result = CreateLockData(exidA, out lockCurrent);
		//
		// 	if (!result)
		// 	{
		// 		M.WriteLineStatus($"write lock A status (create lock data) | {result} | code| {smR.ReturnCode}");
		// 		T.RollBack();
		// 		return false;
		// 	}
		//
		// 	result = SetRtnCode(smR.StorLibR.CreateDataStorage(exidA, out ds));
		//
		// 	if (!result)
		// 	{
		// 		M.WriteLineStatus($"write lock A status (create data storage) | {result} | code| {smR.ReturnCode}");
		// 		T.RollBack();
		// 		return false;
		// 	}
		//
		// 	result = SetRtnCode(smR.SchemaLibR.WriteLock(ds, lockCurrent));
		//
		// 	if (!result)
		// 	{
		// 		M.WriteLineStatus($"write lock A status (write lock) | {result} | code| {smR.ReturnCode}");
		// 		T.RollBack();
		// 		return false;
		// 	}
		//
		// 	T.Commit();
		// }
		//
		// doTheyExist();
		//
		// StaticInfo.UpdateMainWinProperty(nameof(StaticInfo.MainWin.MwModel));
		//
		// M.WriteLine($"lock A created| {exidA.DsName}");
		//
		// return result;


		
		// lock A

		// public bool CreateLockAData()
		// {
		// 	bool result;
		// 	M.WriteLineStatus("start make lock A data");
		//
		// 	ScDataLock lokd;
		//
		// 	CreateLockData(exidLokAJeff, out lokd);
		// 	LockCurrent = lokd;
		//
		// 	if (result)
		// 	{
		// 	}
		//
		// 	M.WriteLineStatus($"create lock data | {result}");
		//
		// 	return result;
		// }
	}
}