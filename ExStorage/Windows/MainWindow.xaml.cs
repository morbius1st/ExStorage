#region using

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Autodesk.Revit.Creation;
using ShExStorageR.ShExStorage;
using SharedApp.Windows.ShSupport;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using ShExStorageN.ShExStorage;
using ShStudyN.ShEval;
// using EnvDTE;
using RevitSupport;
using SettingsManager;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ShScSupport;
using ShExStorageN.ShSchemaFields;
using ShStudyN;
using SettingsManager;
using Document = Autodesk.Revit.DB.Document;

#endregion

// projname: ExStorage
// itemname: MainWindow
// username: jeffs
// created:  12/27/2021 7:15:47 PM


namespace ExStorage.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged, IWin
	{
	#region private fields

		public static ShDebugMessages M { get; private set; }
		private string messageBoxText;
		private string statusBoxText;
		private string codeMapText;

		private MainWindowModel mwModel;

		// private ExId exid;

		private UserSettings uStg;

	#endregion

	#region ctor

		public MainWindow()
		{
			InitializeComponent();

			M = new ShDebugMessages(this);

			StaticInfo.MainWin = this;

			config();
		}

	#endregion

	#region public properties

		public MainWindowModel MwModel => mwModel;

		public string MessageBoxText
		{
			get => messageBoxText;
			set
			{
				messageBoxText = value;
				OnPropertyChanged();
			}
		}

		public string StatusBoxText
		{
			get => statusBoxText;
			set
			{
				statusBoxText = value;
				OnPropertyChanged();
			}
		}

		public string CodeMapText
		{
			get => codeMapText;
			set
			{
				codeMapText = value;
				OnPropertyChanged();
			}
		}

		// public ExId Exid
		// {
		// 	get => exid;
		// 	set => exid = value;
		// }

		// public string DataStoreSchemaName => MwModel?.smR?.Sheet?.SchemaName ?? "<not init>";

		public ShtExId ShtExid => mwModel?.SheetExidCurrent;

		// public string smRHasData => mwModel?.smR?.HasData.ToString() ?? "<not init>";
		public string smRntCode => mwModel?.smR?.ReturnCode.ToString() ?? "<not init>";

	#endregion

	#region private properties


	#endregion

	#region public methods

		public void UpdateProperty(string property)
		{
			// M.WriteLineCodeMap();
			
			OnPropertyChanged(property);
		}

	#endregion

	#region private methods

		private void config()
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("before init mainwinmodel");

			AExId.Config(RvtCommand.RvtDoc);

			// mwModel = new MainWindowModel(M, ExId1.GetInstance(RvtCommand.RvtDoc));
			mwModel = new MainWindowModel(M, new ShtExId("Primary Sheet ExId", ShtExId.ROOT));

			StaticInfo.MainWinModel = mwModel;

			getUserSettings();

			ScDataRow r = null;
		}

		private void getUserSettings()
		{
			M.WriteLineCodeMap();
			UserSettings.Admin.Read();

			UserSettings.Data.UserSettingsValue += 1;

			UserSettings.Admin.Write();
		}

		// private void MwModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		// {
		// 	if (e.PropertyName.Equals(nameof(mwModel.SmRtnCode)))
		// 	{
		// 		OnPropertyChanged(nameof(smRntCode));
		// 	} 
		// 	else if (e.PropertyName.Equals(nameof(mwModel.SheetInit)))
		// 	{
		// 		OnPropertyChanged(nameof(MwModel));
		// 	}
		// }

	#endregion

	#region event consuming

		private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("begin mainwin loaded");

			mwModel.ShowSheet1();

			OnPropertyChanged(nameof(MwModel));

			M.WriteLineStatus("end mainwin loaded");
		}

		private void BtnExit_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			this.DialogResult = true;
			this.Close();
		}

		private void BtnClear_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("clear messages button");
			M.MsgClr();
		}

	#endregion

	#region event publishing

		public new event PropertyChangedEventHandler PropertyChanged;

		protected void  OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(MainWindow)}";
		}

	#endregion

	#region shows

		private void BtnShowExid_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("show Exid button");
			mwModel.ShowExid(ShtExid);
		}

		private void BtnShowSheet1_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("show sheet button");
			mwModel.ShowSheet1();
		}

		private void BtnShowLockA_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("show lock A button");
			mwModel.ShowLockA();
		}

		private void BtnShowLockB_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("show lock B button");
			mwModel.ShowLockB();
		}

	#endregion

	#region tests

		private void BtnReadTables_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("init sheet button");

			mwModel.GetTables();
		}

		private void BtnWhatExists10_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			mwModel.doTheyExist();
		}

		// sheet

		private void BtnMakeMakeSheetDataA_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			M.writeMsg("*** make sheet button ***\n", 1);
			M.WriteLineStatus("button: make data");

			mwModel.MakeSheetData();

			M.WriteLine("make sheet data - WORKED");
		}

		private void BtnWriteSheet9_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			M.writeMsg("*** write sheet button ***\n", 1);

			M.WriteLineStatus("write sheet button");

			M.WriteLine("\n*** begin process| WRITE SHEET | ***\n");

			// M.WriteLineStatus("make sheet data");
			//
			// mwModel.MakeSheetData();

			M.WriteLineStatus("before write sheet");

			// bool result = mwModel.WriteSheet(shtd);
			bool result = mwModel.TestWriteSheetCurrent();

			M.WriteLineStatus($"after write sheet| status| {result}");

			if (result)
			{
				M.WriteLine("\n*** WRITE SHEET worked ***\n");
			}
			else
			{
				M.WriteLine($"\n*** WRITE SHEET failed ***\n");
			}
		}

		private void BtnReadSheet9b_OnClick(object sender, RoutedEventArgs a)
		{
			M.WriteLineCodeMap();
			M.WriteLine("\n*** begin process| READ SHEET | ***\n");

			bool result = mwModel.TestReadSheetCurrent();

			if (result)
			{
				M.WriteLine("\n*** READ SHEET worked ***\n");
				// mwModel.ShowSheetData(mwModel.smR.Sheet);
				mwModel.ShowSheet1();
			}
			else
			{
				M.WriteLine($"\n*** READ SHEET failed ***");
				M.WriteLine($"*** Add some data first maybe? ***\n");
			}
		}

		private void BtnDeleteSheet12_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			mwModel.DeleteSheet();
		}

		private static int newNameCount = 1;

		private void BtnUpdateSheet_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			M.writeMsg("*** update sheet button ***\n", 1);

			M.WriteLineStatus("write sheet button");
			if (mwModel.SheetDataCurrent == null ||
				!mwModel.SheetDataCurrent.HasData)
			{
				M.WriteLine("FAILED: No Data");
				return;
			}

			ScDataSheet tempShtd = (ScDataSheet) mwModel.SheetDataCurrent.Clone();

			tempShtd.SetValue(SchemaSheetKey.SK2_MODEL_NAME, 
				$"this is the new name of the model - {newNameCount++}.rvt");
			
			M.WriteLine("before update sheet| orig model name|", mwModel.SheetDataCurrent.ModelName);

			M.WriteLine("before update sheet| temp model name|", tempShtd.ModelName);

			bool result = mwModel.UpdateSheetTest(tempShtd);

			if (result)
			{
				M.WriteLine("after update sheet| orig model name|", mwModel.SheetDataCurrent.ModelName);

				M.WriteLine("WORKED"); 
			}
			else
			{
				M.WriteLine("FAILED");
			}

		}

		// locks general
		private void BtnLockExist_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("lock exist button");

			mwModel.DoesLockExist();

			M.WriteLineStatus("lock exist complete");

		}

		// lock A

		private void BtnCreateLockA_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			M.writeMsg("*** create lock A button ***\n", 1);

			M.WriteLineStatus("write lock A button");
			
			bool result = mwModel.CreateLockCurrent();

			if (result)
			{
				M.WriteLine("write lock A WORKED");
			}
			else
			{
				M.WriteLine("write lock A FAILED");

			}
		}

		private void BtnReadLockA_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("read lock A button");
			ScDataLock lokd;
			mwModel.ReadLockCurrent(out lokd);
		}

		private void BtnGetLockAOwner_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("get lock A owner Id button");

			string owner;

			bool result = mwModel.GetUserNameLockCurrent(out owner);			
			
			if (result)
			{
				M.WriteLine($"read lock A owner Id is| {mwModel.LockDataCurrent?.UserName ?? "is null"}");
			}
		}

		private void BtnDelLockA_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();


			M.writeMsg("*** del lock A button ***\n", 1);

			M.WriteLineStatus("del lock A button");

			bool result = mwModel.DeleteLockCurrent();

			if (result)
			{
				M.WriteLine("del lock A WORKED");
			} else
			{
				M.WriteLine("del lock A FAILED");
			}

		}

		/*
		// private void BtnMakeLockAdata_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	M.WriteLineStatus("make data lock A button");
		// 	
		// 	mwModel.CreateLockData();
		// }*/


		// lock B

		private void BtnMakeLockBdata_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();


			M.WriteLineStatus("make data lock B button");
			
			mwModel.CreateLockBData();

			M.WriteLine("make lock B data WORKED");
		}

		private void BtnCreateLockB_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();


			M.WriteLineStatus("write lock B button");

			bool result = mwModel.CreateLockB();

			if (result)
			{
				M.WriteLine("write lock B WORKED");
			}
			else
			{
				M.WriteLine("write lock B FAILED");

			}

		}

		private void BtnReadLockB_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();

			M.WriteLineStatus("read lock B button");
			ScDataLock lokd;
			mwModel.ReadLockB(out lokd);

		}

		private void BtnGetLockBOwner_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();

			M.WriteLineStatus("get lock B owner Id button");

			string owner;

			bool result = mwModel.GetUserNameLockB(out owner);
			// todo allow for locktemp to be null
			if (result)
			{
				M.WriteLine($"read lock B owner Id is| {mwModel.LockDataTemp?.UserName ?? "is null"}");
			}

		}

		private void BtnDelLockBjeffs_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("del lock B jeffs button");

			bool result = mwModel.DeleteLockBjeffs();

			if (result)
			{
				M.WriteLine("del lock B jeffs WORKED - incorrect");
			} else
			{
				M.WriteLine("del lock B jeffs FAILED - correct");
			}

		}

		private void BtnDelLockBjohns_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("del lock B johns button");

			bool result = mwModel.DeleteLockBjohns();

			if (result)
			{
				M.WriteLine("del lock B johns WORKED - correct");
			} else
			{
				M.WriteLine("del lock B johns FAILED - incorrect");
			}

		}

		// lock A and B

		private void BtnCanDelLock_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap();
			M.WriteLineStatus("can del lock button");

			string username = UtilityLibrary.CsUtilities.UserName;

			bool result = mwModel.CanDeleteLockCurrent();

			M.WriteLine($"can del lock A?| {username} vs {mwModel.LockDataCurrent?.UserName ?? "null name"}");

			if (result)
			{
				M.WriteLine("can del lock A! - correct");
			}
			else
			{
				M.WriteLine("can NOT del lock A - incorrect");
			}

			M.NewLine();

			result = mwModel.CanDeleteLockB();

			M.WriteLine($"can del lock B?| {username} vs {mwModel.LockDataTemp?.UserName ?? "null name"}");

			if (result)
			{
				M.WriteLine("can del lock B! - correct");
			}
			else
			{
				M.WriteLine("can NOT del lock B - incorrect");
			}
		}


		#endregion

		#region old / not used

		// private void BtnMakeLockDataA_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	M.WriteLineStatus("make lock A button");
		//
		// 	mwModel.MakeLockDataA();
		// }

		// private void BtnMakeLockDataB_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	M.WriteLineStatus("make lock B button");
		//
		// 	mwModel.MakeLockDataB();
		// }


		// private void BtnInit11_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	mwModel.MakeDataGeneric();
		// }


		// private void BtnShowFn11_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	mwModel.ShowProcFn11();
		// }
		//
		// private void BtnShowFn12_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	mwModel.ShowProcFn12();
		// }

		// private void BtnTestFindAllDs1_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	mwModel.GetAllDs1();
		// }
		//
		// private void BtnTestFindDsByName3_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	mwModel.FindDsByName3();
		// }
		//
		// private void BtnTestDeleteSheetDs4_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	// includes a transaction
		// 	// mwModel.DeleteSheetDs(exid);
		// }
		//
		// private void BtnTestDeleteSheetEntity4b_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	// mwModel.DeleteSheetEntity(exid);
		// }
		//
		// private void BtnTestDeleteSheetSchema4c_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	// mwModel.DeleteSheetSchema(exid);
		// }
		//
		// private void BtnGetSchemaByName2_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	mwModel.GetSchemaByName2();
		// }
		//
		// private void BtnTestGetDsEntity5_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	mwModel.GetDsEntity5();
		// }
		//
		// private void BtnTestGetEntityData6_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	mwModel.GetEntityData6();
		// }
		//
		// private void BtnTestDoesDsExist7_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	mwModel.DoesDsExist7();
		// }
		//
		// private void BtnBegin8_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	// ExStoreRtnCode result = mwModel.TestBegin(Exid);
		// 	//
		// 	// if (result == ExStoreRtnCode.XRC_DS_EXISTS)
		// 	// {
		// 	// 	M.WriteLineAligned("DS already exists - cannot make new - exiting\n");
		// 	// }
		// }

		#endregion



		// // sheet 2
		//
		// private void BtnShowSheet2_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	M.WriteLineStatus("show sheet button");
		// 	mwModel.ShowSheet2();
		// }
		//
		//
		// private void BtnMakeMakeSheetDataA2_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	M.WriteLineStatus("make data button");
		//
		// 	mwModel.MakeSheetData2();
		//
		// 	M.WriteLine("make sheet data - WORKED");
		// }
		//
		// private void BtnWriteSheet92_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	M.WriteLineStatus("write sheet button");
		//
		// 	M.WriteLine("\n*** begin process| WRITE SHEET | ***\n");
		//
		// 	// M.WriteLineStatus("make sheet data");
		// 	//
		// 	// mwModel.MakeSheetData();
		//
		// 	M.WriteLineStatus("before write sheet");
		//
		// 	// bool result = mwModel.WriteSheet(shtd);
		// 	bool result = mwModel.TestWriteSheetCurrent2();
		//
		// 	M.WriteLineStatus($"after write sheet| status| {result}");
		//
		// 	if (result)
		// 	{
		// 		M.WriteLine("\n*** WRITE SHEET worked ***\n");
		// 	}
		// 	else
		// 	{
		// 		M.WriteLine($"\n*** WRITE SHEET failed ***\n");
		// 	}
		// }
		//
		// private void BtnReadSheet9b2_OnClick(object sender, RoutedEventArgs a)
		// {
		// 	M.WriteLine("\n*** begin process| READ SHEET | ***\n");
		//
		// 	bool result = mwModel.TestReadSheetCurrent2();
		//
		// 	if (result)
		// 	{
		// 		M.WriteLine("\n*** READ SHEET worked ***\n");
		// 		// mwModel.ShowSheetData(mwModel.smR.Sheet);
		// 		mwModel.ShowSheet12();
		// 	}
		// 	else
		// 	{
		// 		M.WriteLine($"\n*** READ SHEET failed ***");
		// 		M.WriteLine($"*** Add some data first maybe? ***\n");
		// 	}
		// }
		//
		// private void BtnDeleteSheet122_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	mwModel.DeleteSheet2();
		// }

	}
}