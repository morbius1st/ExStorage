#region using

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB.Electrical;
using ExStorage.TestProcedures;
using JetBrains.Annotations;
using SettingsManager;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ShScSupport;
using static ShExStorageN.ShExStorage.ExStoreRtnCode;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields;
using ShExStorageN.ShSchemaFields.ShScSupport;
using ShExStorageR.ShExStorage;
using ShStudyN.ShEval;
using TestProcedures01 = ExStorage.TestProcedures.TestProcedures01;

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
		private ShDebugMessages M;


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

		private MainModel mm;


		// public ShExStorManagerR2<
		// 	ScDataSheet2,
		// 	ScDataRow2,
		// 	ScDataLock2,
		// 	sKey,
		// 	ScFieldDefData2<sKey>,
		// 	rKey,
		// 	ScFieldDefData2<rKey>,
		// 	lKey,
		// 	ScFieldDefData2<lKey>
		// 	> smR2 { get; private set; }




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


		// private ScDataSheet2 sheetDataCurrent2;


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

		private void initDebug(ShDebugMessages msgs)
		{
			M.WriteLineCodeMap();
			sp01 = new ShowsProcedures01();
			tp01 = new TestProcedures01();

			shsp01 = new ShShowProcedures01(msgs);

			sl = new ShowLibrary(msgs);
			shFd = new ShFieldDisplayData();

			// not fixed data

			lockExidBJohn = new LokExId("Lock B john", LokExId.PRIME);
			lockExidBJohn.SetOverrideUserName("johns", LokExId.PRIME);


		}

		private void config(ShDebugMessages msgs, ShtExId exid)
		{
			M.WriteLineCodeMap();
			initDebug(msgs);

			M.WriteLineStatus("at start");

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
			if (ReadSheet(sheetExidCurrent, out sheetDataCurrent))
			{
				M.WriteLineStatus("existing sheet read");
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
			M.WriteLineCodeMap();
			M.WriteLineStatus("set sheet");

			sheetDataPrior = sheetDataCurrent;

			SheetDataCurrent = sht;
			// SheetLoaded = true;
		}


		private void RemoveSheet()
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("delete sheet");

			sheetDataPrior = sheetDataCurrent;

			SheetDataCurrent = null;

		}

		// sheet

		public bool UpdateSheet(ScDataSheet shtd, out string owner)
		{
			M.WriteLineCodeMap();

			return SetRtnCodeB(smR.UpdateSheet(SheetExidCurrent, shtd, out owner));
		}

		public void MakeSheetData()
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("init sheet data");

			ScDataSheet shtd = ScData.MakeInitialDataSheet1(SheetExidCurrent);

			shtd.HasData = true;

			TestMakeDataRow3(SheetExidCurrent, shtd);

			SetSheet(shtd);
		}

		private ScDataRow CreateFauxRow(ShtExId exid, ScDataRow row)
		{
			M.WriteLineCodeMap();
			tp01.MakeFauxRow(exid, row);

			return row;
		}

		public void TestMakeDataRow3(ShtExId exid, ScDataSheet shtd)
		{
			M.WriteLineCodeMap();
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
			M.WriteLineCodeMap();
			M.WriteLineStatus("initialize sheet");
		
			return ScData.MakeInitialDataSheet1(exid);
		}

		public bool ReadSheet(ShtExId shtExid, out ScDataSheet shtd)
		{
			M.WriteLineCodeMap();
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
			M.WriteLineCodeMap();
			if (shtd == null)
			{
				M.WriteLineStatus("sheet data is null");
				return false;
			}
		
			return smR.WriteSheet(shtExid,	lokExid, shtd);
		}

		/// <summary>
		/// remove the current sheet - ds & schemas
		/// </summary>
		public void DeleteSheet()
		{
			M.WriteLineCodeMap();
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

		public bool DoesSheetLockExist()
		{
			M.WriteLineCodeMap();
			return smR.DoesSheetLockExist(lockExidCurrent);
		}

		public bool GetUserNameLock(LokExId exid, out string owner)
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("get lock current owner Id");

			return smR.GetLockOwnerFromName(exid, out owner);
		}


		// lock

		public void CreateLockData(LokExId lokExid, out ScDataLock lokd)
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("lock set");

			lokd = new ScDataLock();
			lokd.Configure(lokExid);
		}

		private void createFixedLockData(out ScDataLock lokd)
		{
			M.WriteLineCodeMap();
			CreateLockData(lockExidCurrent, out lokd);
		}

		// lock current

		public bool CreateLockCurrent()
		{
			M.WriteLineCodeMap();
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
			M.WriteLineCodeMap();
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
			M.WriteLineCodeMap();
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
			M.WriteLineCodeMap();
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
			M.WriteLineCodeMap();

			
			if (!smR.DeleteLock(lockExidCurrent))
			{
				M.WriteLineStatus($"X return code| {smR.ReturnCode}");
				return false;
			}

			doTheyExist();

			return true;
		}

		// lock B johns

		public void CreateLockBData()
		{
			M.WriteLineCodeMap();
			ScDataLock lokd;

			CreateLockData(lockExidBJohn, out lokd);
			
			LockBDataJohn = lokd;

			M.WriteLineStatus($"lock data created");
		}

		public bool CreateLockB()
		{
			M.WriteLineCodeMap();
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
			M.WriteLineCodeMap();
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
			M.WriteLineCodeMap();
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
			M.WriteLineCodeMap();
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
			M.WriteLineCodeMap();
			if (!smR.DeleteLock(lockExidCurrent)) return false;

			doTheyExist();

			return true;
		}

		public bool DeleteLockBjohns()
		{
			M.WriteLineCodeMap();
			if (!smR.DeleteLock(lockExidBJohn)) return false;

			doTheyExist();

			return true;
		}

	#endregion


	#region tables

		public void GetTables()
		{
			M.WriteLineCodeMap();
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
			M.WriteLineCodeMap();
			M.WriteLineStatus("show exid");

			sp01.ShowEid(exid);
		}

		public void ShowSheet1()
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("show sheet");

			shsp01.ShowSheetDataGeneric(SheetDataCurrent);
		}



		public void ShowLockA()
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("show lock A");

			shsp01.ShowLockDataGeneric(LockDataCurrent);
		}

		public void ShowLockB()
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("show lock B");

			shsp01.ShowLockDataGeneric(LockBDataJohn);
		}

	#endregion

	#region tests

		// tests

		public void DoesLockExist()
		{
			M.WriteLineCodeMap();
			bool result;

			result = DoesSheetLockExist();

			if (result)
			{
				M.WriteLine("lock FOUND");
			}
			else
			{
				M.WriteLine("lock NOT FOUND");
			}

			string name;

			result = GetUserNameLock(lockExidCurrent, out name);

			if (name != null)
			{
				M.WriteLine($"name FOUND| {name}");
			}
			else
			{
				M.WriteLine("name NOT FOUND");
			}
		}

		public void doTheyExist()
		{
			M.WriteLineCodeMap();
			M.WriteLine("\nChecking if sheet ds and sheet schema exist");

			smR.doTheyExist(sheetExidCurrent, lockExidCurrent, lockExidBJohn);
		}

		// test - create modified data to 
		// test the update method - this uses the
		// tempEditing sheet
		// process 
		// A clone the current data into sheetDataEditing
		// B modify sheetDataEditing
		public void modifyDataBegin()
		{
			M.WriteLineCodeMap();
			// sheetDataEditing = sheetDataCurrent.CloneFields();
		}

		// once modification process complete
		// process the tempediting data
		// if it worked, clone back into current
		// if it failed, set to null
		public void modifyDataEnd()
		{
			M.WriteLineCodeMap();

		}

	#endregion

	#region more tests

		public bool TestWriteSheetCurrent()
		{
			M.WriteLineCodeMap();
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
			M.WriteLineCodeMap();
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
			M.WriteLineCodeMap();
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
			M.WriteLineCodeMap();
			M.WriteLineStatus("start read sheet");

			ScDataSheet shtd;

			bool result = ReadSheet(sheetExidCurrent, out shtd);

			SheetDataEditing = shtd;

			M.WriteLineStatus($"read sheet status| {result} | code| {smR.ReturnCode}");

			return result;
		}
		
		
		public bool UpdateSheetTest(ScDataSheet shtd)
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("begin| Update sheet");

			string owner;

			bool result =  UpdateSheet(shtd, out owner);

			M.WriteLineStatus($"Update| status| {result}| return code| {ReturnCode}| owner| {owner}");

			return SetRtnCodeB();;
		}







		// public ShExStorManagerR<object, object, object, object, object, object, object, object, object> ShExStorManagerR
		// {
		// 	get => default;
		// 	set
		// 	{
		// 	}
		// }

		// public ShtExId ShtExId
		// {
		// 	get => default;
		// 	set
		// 	{
		// 	}
		// }


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


				// // sheet 2
		//
		// public void ShowSheet12()
		// {
		// 	M.WriteLineStatus("show sheet");
		//
		// 	shsp01.ShowSheetDataGeneric(SheetDataCurrent);
		// }
		//
		//
		// private void SetSheet2(ScDataSheet2 shtd2)
		// {
		// 	M.WriteLineStatus("set sheet");
		//
		// 	// sheetDataPrior = sheetDataCurrent;
		//
		// 	SheetDataCurrent2 = shtd2;
		// 	// SheetLoaded = true;
		// }
		//
		// public void MakeSheetData2()
		// {
		// 	M.WriteLineStatus("init sheet data");
		//
		// 	ScDataSheet2 shtd2 = ScData2.MakeInitialDataSheet2(SheetExidCurrent);
		//
		// 	TestMakeDataRow32(SheetExidCurrent, shtd2);
		//
		// 	SetSheet2(shtd2);
		// }
		//
		// private ScDataRow2 CreateFauxRow2(ShtExId exid, ScDataRow2 row)
		// {
		// 	// tp01.MakeFauxRow(exid, row);
		// 	tp01.MakeFauxRow2(exid, row);
		//
		// 	return row;
		// }
		//
		// public void TestMakeDataRow32(ShtExId exid, ScDataSheet2 shtd2)
		// {
		// 	M.WriteLineStatus("init row data");
		//
		// 	// M.WriteLine("test make ROW data x3");
		// 	ScDataRow2 rowd;
		//
		// 	rowd = CreateFauxRow2(exid, ScData2.MakeInitialDataRow2(shtd2));
		// 	shtd2.AddRow(rowd);
		//
		// 	rowd = CreateFauxRow2(exid, ScData2.MakeInitialDataRow2(shtd2));
		// 	shtd2.AddRow(rowd);
		//
		// 	rowd = CreateFauxRow2(exid, ScData2.MakeInitialDataRow2(shtd2));
		// 	shtd2.AddRow(rowd);
		//
		// 	// UserSettings.Data.UserSettingsValue += 1;
		// 	//
		// 	// UserSettings.Admin.Write();
		// }
		//
		//
		// public bool TestWriteSheetCurrent2()
		// {
		// 	M.WriteLineStatus("start write sheet");
		//
		// 	bool result;
		//
		// 	result = WriteSheet2(sheetExidCurrent, lockExidCurrent, sheetDataCurrent2);
		//
		// 	M.WriteLineStatus($"write sheet status| {result} | code| {smR.ReturnCode}");
		//
		// 	// doTheyExist();
		//
		// 	StaticInfo.UpdateMainWinProperty(nameof(StaticInfo.MainWin.MwModel));
		//
		// 	return result;
		// }
		//
		// public bool TestReadSheetCurrent2()
		// {
		// 	M.WriteLineStatus("start read sheet");
		//
		// 	ScDataSheet2 shtd2;
		//
		// 	bool result = ReadSheet2(SheetExidCurrent, out shtd2);
		//
		// 	SheetDataCurrent2 = shtd2;
		//
		// 	M.WriteLineStatus($"read sheet status| {result} | code| {smR.ReturnCode}");
		//
		// 	if (!result)
		// 	{
		// 		RemoveSheet2();
		// 	}
		//
		// 	return result;
		// }
		//
		//
		// public bool ReadSheet2(ShtExId shtExid, out ScDataSheet2 shtd2)
		// {
		// 	if (!SetRtnCodeB(smR2.ReadSheet(shtExid, out shtd2)))
		// 	{
		// 		shtd2 = null;
		// 	}
		//
		// 	return SetRtnCodeB();
		// }
		//
		//
		//
		// /// <summary>
		// /// write the sheet data to the model<br/>
		// /// return <br/>
		// /// true if it worked<br/>
		// /// false if not (
		// /// </summary>
		// /// <param name="shtd"></param>
		// public bool WriteSheet2(ShtExId shtExid, LokExId lokExid, ScDataSheet2 shtd2)
		// {
		// 	if (shtd2 == null) return false;
		//
		// 	return smR2.WriteSheet(shtExid,	lokExid, shtd2);
		// }
		//
		// /// <summary>
		// /// remove the current sheet - ds & schemas
		// /// </summary>
		// public void DeleteSheet2()
		// {
		// 	M.WriteLine("sheet delete");
		//
		// 	bool result = smR2.DeleteSheet(SheetExidCurrent, lockExidCurrent);
		//
		// 	if (result)
		// 	{
		// 		M.WriteLine("sheet deleted - WORKED");
		// 		RemoveSheet2();
		// 	}
		// 	else
		// 	{
		// 		M.WriteLine("sheet delete - FAILED");
		// 	}
		// }
		//
		// private void RemoveSheet2()
		// {
		// 	M.WriteLineStatus("delete sheet");
		//
		// 	sheetDataPrior = sheetDataCurrent;
		//
		// 	SheetDataCurrent = null;
		//
		// }


		// public void ShowSheet2()
		// {
		// 	M.WriteLineStatus("show sheet");
		//
		// 	shsp01.ShowSheetDataGeneric2(SheetDataCurrent2);
		// }
	}
}