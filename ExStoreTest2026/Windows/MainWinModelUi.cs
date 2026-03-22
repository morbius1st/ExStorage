using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using ExStoreTest2026.DebugAssist;
using ExStorSys;
using RevitLibrary;
using UtilityLibrary;
using static ExStorSys.ExStorConst;
using static ExStorSys.PropertyId;
using static ExStorSys.RunningStatus;


// projname: $projectname$
// itemname: MainWinModelUi
// username: jeffs
// created:  10/19/2025 6:44:32 PM


namespace ExStoreTest2026.Windows
{
	/// <summary>
	/// this class is to handle UI elements and to help with UI processing
	/// of information display
	/// </summary>
	public class MainWinModelUi : INotifyPropertyChanged
	{
	#region private fields

		public int ObjectId;

		private ExSysStatus exSysStatus;
		private bool? restartStatus;

		private ExStorMgr xMgr;
		private ExStorData xData;

		private bool workBookSchemaStatus;
		private bool sheetSchemaStatus;

		private ValidateSchema resultWbkSc = 0;
		private ValidateDataStorage resultWbkDs = 0;
		private ValidateSchema resultShtSc = 0;
		private ValidateDataStorage resultShtDs = 0;
		private LaunchCode launchCode;
		private RunningStatus systemRunningStatus;
		
		private string tempFamilyName;
		private string tempFamilyType;
		private string tempProps;
		private Sheet? selSheet;
		private  KeyValuePair<string, FamAndType>? selFamilyTypeValue;
		private  KeyValuePair<string, FamAndType>? selFamilyTypeItem;
		private string selValue;

	#endregion

	#region ctor

		// // ReSharper disable once InconsistentNaming
		// private static readonly Lazy<MainWinModelUi> instance =
		// 	new (() => new MainWinModelUi());


		private MainWinModelUi() {}

		public static MainWinModelUi Instance { get; set; }

		public static MainWinModelUi Create()
		{
			Instance = new ();
			return Instance;
		}

		public void Init()
		{
			// Debug.WriteLine($"\n*** MainWinModelUi init | begin");

			// objectId = AppRibbon.ObjectIdx++;
			ObjectId = ExStorStartMgr.Instance?.AddObjId(nameof(MainWinModelUi)) ?? -1;
			Instance = this;
			xMgr = ExStorMgr.Instance!;
			xData = ExStorData.Instance;

			// xMgr.RestartReqdChanged += OnRestartReqdChanged;
			xMgr.PropChgd += OnPropChgdEvent;
			OnPropertyChanged(nameof(OpenModelCount));

			xData.PropChgd += OnPropChgdEvent;
			
			SecurityMgr.Instance.ResetPropChanged();
			SecurityMgr.Instance.PropertyChanged += SecMgr_PropertyChanged;

			// CmdResetFamList = new RelayCommand(CmdResetFamListExe,CmdResetFamListCanExe);
			// SaveNewFamilyListItem = new RelayCommand(SaveNewFamItemExe,SaveNewFamItemCanExe);

			// Debug.WriteLine($"\n*** MainWinModelUi init | exit ({ObjectId})");
		}


		public void Restore()
		{
			xMgr = ExStorMgr.Instance!;
			xData = ExStorData.Instance;

			xMgr.PropChgd += OnPropChgdEvent;
			xData.PropChgd += OnPropChgdEvent;

			SecurityMgr.Instance.ResetPropChanged();
			SecurityMgr.Instance.PropertyChanged += SecMgr_PropertyChanged;
		}

	#endregion

		/*ui elements */

		public string UserName => SecurityMgr.Instance.UserName;
		public string? UserName2 => SecurityMgr.Instance.UserName2;
		public UserSecutityLevel UseSecLvl => SecurityMgr.Instance.UserSecurityLevel;
		public string SecurityLeveName => ExStorConst.UsserSecurityLevelDesc[UseSecLvl].Item1;
		public string SecurityLevelDesc => ExStorConst.UsserSecurityLevelDesc[UseSecLvl].Item2;

	#region status information

		public int OpenModelCount
		{
			get => R.OpenDocCount;
		}

		/* sys running status */
		public RunningStatus SystemRunningStatus
		{
			get => systemRunningStatus;
			set
			{
				if (value == systemRunningStatus) return;
				systemRunningStatus = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(SystemRunningStatusDesc));
			}
		}

		public string SystemRunningStatusDesc => RunningStatusDesc[systemRunningStatus];

		private void updateSysRunStatus()
		{
			// account for
			// restart status
			// ExSysStatus
			// ResolveStatus

			if (launchCode == LaunchCode.LC_DEBUG)
			{
				SystemRunningStatus = RN_DEBUG;
				return;
			}

			if (RestartStatus == true)
			{
				SystemRunningStatus = RN_CANNOT_RUN_RESTART;
				exSysStatus = ExSysStatus.ES_RESTART_REQD;
				OnPropertyChanged(nameof(ExSysStatus));
				OnPropertyChanged(nameof(ExSysStatusDesc));
				return;
			}

			if (RestartStatus != true)
			{
				if (launchCode == LaunchCode.LC_DONE_INVALID)
				{
					SystemRunningStatus = RN_CANNOT_RUN_FAIL;
					return;
				}
				else if (launchCode == LaunchCode.LC_DONE_GOOD)
				{
					LaunchCode = LaunchCode.LC_DONE_GOOD;
				}

				if (LaunchCode == LaunchCode.LC_DONE_GOOD)
				{
					if (ExSysStatus == ExSysStatus.ES_START_DONE_GOOD)
					{
						SystemRunningStatus = RN_RUNNING_NORMAL;
						return;
					}

					if (ExSysStatus != ExSysStatus.ES_VRFY_DONE_GOOD)
					{
						SystemRunningStatus = RN_READY_NOT_RUNNING;
						return;
					}
				}
			}
		}

		/* restart status */
		public bool? RestartStatus
		{
			get => restartStatus;
			set
			{
				if (value == restartStatus) return;
				restartStatus = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(RestartStatusDesc));
				updateSysRunStatus();
			}
		}

		public string RestartStatusDesc => RestartStatus.HasValue ? (RestartStatus.Value ? RestartStatDesc[1] : RestartStatDesc[0]) : RestartStatDesc[2];

		/* ex sys status */
		public ExSysStatus ExSysStatus
		{
			get => exSysStatus;
			set
			{
				if (value == exSysStatus) return;
				exSysStatus = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(ExSysStatusDesc));
				updateSysRunStatus();
			}
		}

		public string ExSysStatusDesc => ExStorStatDesc[exSysStatus];

		/* validation status */


		/* launch manager status */

		public LaunchCode LaunchCode
		{
			get => launchCode;
			set
			{
				if (value == launchCode) return;
				launchCode = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(LaunchStatusDesc));
			}
		}


		/* launch manager validation status */

		public bool ValidateStatus() => WbkScResCode == ValidateSchema.VSC_GOOD && ShtScResCode == ValidateSchema.VSC_GOOD &&
			ShtDsResCode == ValidateDataStorage.VDS_GOOD && WbkDsResCode == ValidateDataStorage.VDS_GOOD;

		public ValidateSchema WbkScResCode
		{
			get => resultWbkSc;
			set
			{
				if (value == resultWbkSc) return;
				resultWbkSc = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(WbkScResDesc));
			}
		}

		public string WbkScResDesc => $"WBK {ValidateSchemaDesc[resultWbkSc].Item2}";

		public ValidateDataStorage WbkDsResCode
		{
			get => resultWbkDs;
			set
			{
				if (value == resultWbkDs) return;
				resultWbkDs = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(WbkDsResDesc));
			}
		}

		public string WbkDsResDesc => $"WBK {ValidateDataStorageDesc[resultWbkDs].Item2}";

		public ValidateSchema ShtScResCode
		{
			get => resultShtSc;
			set
			{
				if (value == resultShtSc) return;
				resultShtSc = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(ShtScResDesc));
			}
		}

		public string ShtScResDesc => $"SHT {ValidateSchemaDesc[resultShtSc].Item2}";

		public ValidateDataStorage ShtDsResCode
		{
			get => resultShtDs;
			set
			{
				if (value == resultShtDs) return;
				resultShtDs = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(ShtDsResDesc));
			}
		}

		public string ShtDsResDesc => $"SHT {ValidateDataStorageDesc[resultShtDs].Item2}";

		/* xData status */

		public bool WorkBookSchemaStatus
		{
			get => workBookSchemaStatus;
			set
			{
				if (value == workBookSchemaStatus) return;
				workBookSchemaStatus = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(WorkBookSchemaStatusDesc));
			}
		}

		public string WorkBookSchemaStatusDesc => workBookSchemaStatus ? "Got WorkBook Schema" : "Don't Got WorkBook Schema";

		public bool SheetSchemaStatus
		{
			get => sheetSchemaStatus;
			set
			{
				if (value == sheetSchemaStatus) return;
				sheetSchemaStatus = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(SheetSchemaStatusDesc));
			}
		}

		public string SheetSchemaStatusDesc => sheetSchemaStatus ? "Got Sheet Schema" : "Don't Got Sheet Schema";

		public string LaunchStatusDesc => ExStorConst.LaunchCodeDesc[LaunchCode];
		
	#endregion


		/* event processing */

		private void onPropChgdEvent_Process(PropChgEvtArgs e)
		{
			// Debug.WriteLine($"got changed event | {e.PropId} | {e.Value}");

			if (e.PropId == (PI_GEN_RUNNING_STAT))
			{
				// Debug.WriteLine($"got {PI_XDATA_WBK_SC} event");
				SystemRunningStatus = (RunningStatus) e.Value.AsEnum();
				return;
			}

			if (e.PropId == (PI_XDATA_WBK_SC))
			{
				// Debug.WriteLine($"got {PI_XDATA_WBK_SC} event");
				WorkBookSchemaStatus = xData.GotWbkSchema;
				return;
			}

			if (e.PropId == (PI_XDATA_SHT_SC))
			{
				// Debug.WriteLine($"got {PI_XDATA_SHT_SC} event");
				SheetSchemaStatus = xData.GotShtSchema;
				return;
			}

			if (e.PropId == (PI_VFY_WBK_SC))
			{
				// Debug.WriteLine($"got {PI_VFY_WBK_SC} event");
				WbkScResCode = (ValidateSchema) e.Value.AsEnum();
				return;
			}

			if (e.PropId == (PI_VFY_WBK_DS))
			{
				// Debug.WriteLine($"got {PI_VFY_WBK_DS} event");
				WbkDsResCode = (ValidateDataStorage) e.Value.AsEnum();
				return;
			}

			if (e.PropId == (PI_VFY_SHT_SC))
			{
				// Debug.WriteLine($"got {PI_VFY_SHT_SC} event");
				ShtScResCode = (ValidateSchema) e.Value.AsEnum();
				return;
			}

			if (e.PropId == (PI_VFY_SHT_DS))
			{
				// Debug.WriteLine($"got {PI_VFY_SHT_DS} event");
				ShtDsResCode = (ValidateDataStorage) e.Value.AsEnum();
				return;
			}

			if (e.PropId == (PI_GEN_RESTART))
			{
				// Debug.WriteLine($"got {PI_VFY_SHT_DS} event");
				RestartStatus = e.Value.AsBool();
				return;
			}

			if (e.PropId == (PI_GEN_LAUNCHCODE))
			{
				// Debug.WriteLine($"got {PI_VFY_SHT_DS} event");
				LaunchCode = (LaunchCode) e.Value.AsEnum();
				return;
			}

			if (e.PropId == (PI_XSYS_STATUS))
			{
				// Debug.WriteLine($"got {PI_XSYS_STATUS} event");
				ExSysStatus = (ExSysStatus) e.Value.AsEnum();
				return;
			}
		}

		/* event consuming */

		private void SecMgr_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(SecurityMgr.UserName): { OnPropertyChanged(nameof(UserName));  break; }
				case nameof(SecurityMgr.UserName2): { OnPropertyChanged(nameof(UserName2));  break; }
				case nameof(SecurityMgr.UserSecurityLevel):
					{
						OnPropertyChanged(nameof(UseSecLvl));  
						OnPropertyChanged(nameof(SecurityLeveName));  
						OnPropertyChanged(nameof(SecurityLevelDesc));  
						
						break;
					}
			}
		}


		/// <summary>
		///  handle property changes from remote sources
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnPropChgdEvent(object sender, PropChgEvtArgs e)
		{
			onPropChgdEvent_Process(e);
		}


		/// <summary>
		/// Flag noting that no other Ex Storage operations can occur until the<br/>
		/// system is restarted.  Also, this can only happen when there is a<br/>
		/// single model open
		/// </summary>
		private void OnRestartReqdChanged(object sender, bool? e)
		{
			RestartStatus = e;
		}

		// /// <summary>
		// /// property change from manager & data objects
		// /// </summary>
		// private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
		// {
		// 	switch (e.PropertyName)
		// 	{
		// 	case nameof(xMgr.ExSysStatus):
		// 		{
		// 			ExSysStatus = xMgr.ExSysStatus;
		// 			break;
		// 		}
		// 	case nameof(xData.WorkBookSchema):
		// 		{
		// 			WorkBookSchemaStatus = xData.GotWbkSchema;
		// 			break;
		// 		}
		// 	}
		// }


		/* event publishing */

		public event PropertyChangedEventHandler? PropertyChanged;

		/* property status */

		/* notes:
		* the plan is to allow any class that needs to publish a property status can do so
		* via the property status system.   this is an event driven system.
		*
		*/

		[DebuggerStepThrough]
		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

	#region system overrides

		public override string ToString()
		{
			return $"{nameof(MainWinModelUi)} [{ObjectId}]";
		}

	#endregion


		/* workbook */

		// public KeyValuePair<ActivateStatus, Tuple<string, string, SolidColorBrush>> AsValuePair =>
		// 	new (ActivateStatus.AS_ACTIVE, ActiveStatusDesc[ActivateStatus.AS_ACTIVE]);


	#region commands

	#endregion
	}
}