using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using ExStoreTest2027.DebugAssist;
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


namespace ExStoreTest2027.Windows
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
		private bool wbkIsModExo;
		private bool xdUndoBtnShtsLst;
		private bool xdApplyBtnShtsLst;
		private bool currShtIsModFamLstWkg;
		private bool currShtFamLstIsDirty;
		private bool currShtUndoBtnStat;
		private bool currShtApplyBtnStat;
		private bool currShtIsModExo;
		private bool wbkUndoBtnStat;
		private bool wbkApplyBtnStat;
		private bool needsSaving;
		private bool restartReqd;

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
			// objectId = AppRibbon.ObjectIdx++;
			ObjectId = ExStorStartMgr.Instance?.AddObjId(nameof(MainWinModelUi)) ?? -1;

			R.ProcessMsg("init", true, ObjectId);

			Instance = this;
			xMgr = ExStorMgr.Instance!;
			xData = ExStorData.Instance;

			// xMgr.RestartReqdChanged += OnRestartReqdChanged;
			xMgr.PropChgd += OnPropChgdEvent;
			OnPropertyChanged(nameof(OpenModelCount));

			xData.PropChgd += OnPropChgdEvent;
			
			SecurityMgr.Instance.ResetPropChanged();
			SecurityMgr.Instance.PropertyChanged += SecMgr_PropertyChanged;

			// CmdFamListUndoChanges = new RelayCommandEx(CmdFamListUndoChgsExe,CmdFamListUndoChgsCanExe);
			// SaveNewFamilyListItem = new RelayCommandEx(SaveNewFamItemExe,SaveNewFamItemCanExe);

			R.ProcessMsg("init", false, ObjectId);

			// default property settings to ensure UI displays the correct value & description
			WbkIsModExo = false;
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

		/* status */

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

		/* UI status */

		/* xData status */


		/* Workbook status */

		public bool RestartReqd
		{
			get => restartReqd;
			set
			{
				if (value == restartReqd) return;
				restartReqd = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(RestartReqdDesc));
			}
		}

		public bool NeedsSaving
		{
			get => needsSaving;
			set
			{
				if (value == needsSaving) return;
				needsSaving = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(NeedsSavingDesc));
			}
		}

		public bool WbkIsModExo
		{
			get => wbkIsModExo;
			set
			{
				if (value == wbkIsModExo) return;
				wbkIsModExo = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(WbkIsModExoDesc));
			}
		}

		public bool WbkApplyBtnStat
		{
			get => wbkApplyBtnStat;
			set
			{
				if (value == wbkApplyBtnStat) return;
				wbkApplyBtnStat = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(WbkApplyBtnStatDesc));
			}
		}

		public bool WbkUndoBtnStat
		{
			get => wbkUndoBtnStat;
			set
			{
				if (value == wbkUndoBtnStat) return;
				wbkUndoBtnStat = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(WbkUndoBtnStatDesc));
			}
		}

		public bool CurrShtIsModExo
		{
			get => currShtIsModExo;
			set
			{
				if (value == currShtIsModExo) return;
				currShtIsModExo = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(CurrShtIsModExoDesc));
			}
		}

		public bool CurrShtApplyBtnStat
		{
			get => currShtApplyBtnStat;
			set
			{
				if (value == currShtApplyBtnStat) return;
				currShtApplyBtnStat = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(CurrShtApplyBtnStatDesc));
			}
		}

		public bool CurrShtUndoBtnStat
		{
			get => currShtUndoBtnStat;
			set
			{
				if (value == currShtUndoBtnStat) return;
				currShtUndoBtnStat = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(CurrShtUndoBtnStatDesc));
			}
		}

		public bool CurrShtFamLstIsDirty
		{
			get => currShtFamLstIsDirty;
			set
			{
				if (value == currShtFamLstIsDirty) return;
				currShtFamLstIsDirty = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(CurrShtFamLstIsDirtyDesc));
			}
		}

		public bool CurrShtIsModFamLstWkg
		{
			get => currShtIsModFamLstWkg;
			set
			{
				if (value == currShtIsModFamLstWkg) return;
				currShtIsModFamLstWkg = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(CurrShtIsModFamLstWkgDesc));
			}
		}

		public bool XdApplyBtnShtsLst
		{
			get => xdApplyBtnShtsLst;
			set
			{
				if (value == xdApplyBtnShtsLst) return;
				xdApplyBtnShtsLst = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(XdApplyBtnShtsLstDesc));
			}
		}

		public bool XdUndoBtnShtsLst
		{
			get => xdUndoBtnShtsLst;
			set
			{
				if (value == xdUndoBtnShtsLst) return;
				xdUndoBtnShtsLst = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(XdUndoBtnShtsLstDesc));
			}
		}

		public string RestartReqdDesc           => restartReqd           ? "System Restart IS Required" : "System Restart is NOT Required";
		public string NeedsSavingDesc           => needsSaving           ? "Data DOES need to be saved" : "Data does NOT need to be Saved";

		public string WbkIsModExoDesc           => wbkIsModExo           ? "Wbk IS Modified" : "Wbk is NOT Modified";
		public string WbkApplyBtnStatDesc       => wbkApplyBtnStat       ? "Wbk CAN Apply" : "Wbk CANNOT Apply";
		public string WbkUndoBtnStatDesc        => wbkUndoBtnStat        ? "Wbk CAN Undo" : "Wbk CANNOT Undo";
		public string CurrShtIsModExoDesc       => currShtIsModExo       ? "CurSht IS Modified" : "CurSht is NOT Modified";
		public string CurrShtApplyBtnStatDesc   => currShtApplyBtnStat   ? "CurSht CAN Apply"   : "CurSht CANNOT Apply";
		public string CurrShtUndoBtnStatDesc    => currShtUndoBtnStat    ? "CurSht CAN Undo"    : "CurSht CANNOT Undo";
		public string CurrShtFamLstIsDirtyDesc  => currShtFamLstIsDirty  ? "CurSht Fam List IS Dirty" : "CurSht Fam List is NOT Dirty";
		public string CurrShtIsModFamLstWkgDesc => currShtIsModFamLstWkg ? "CurSht Fam List Wkg IS Modified" : "CurSht Fam List Wkg is NOT Modified";
		public string XdApplyBtnShtsLstDesc     => xdApplyBtnShtsLst     ? "Xd CAN Apply Shts List" : "Xd CANNOT Apply Shts List";
		public string XdUndoBtnShtsLstDesc      => xdUndoBtnShtsLst      ? "Xd CAN Undo Shts List" : "Xd CANNOT Undo Shts List";


		#endregion


		/* event processing */

		private void onPropChgdEvent_Process(PropChgEvtArgs e)
		{
			// Debug.WriteLine($"got changed event | {e.PropId} | {e.Value}");

			if ((int) e.PropId >= (int) PropertyCategory.PC_START &&
				(int) e.PropId < (int) PropertyCategory.PC_BEGIN_UI_STATUS)
			{
				onPropChgdEvent_GenProcess(e);
			}
			else
			if ((int) e.PropId >= (int) PropertyCategory.PC_BEGIN_UI_STATUS &&
				(int) e.PropId < (int) PropertyCategory.PC_END_UI_STATUS)
			{
				onPropChgdEvent_UiProcess(e);
			}

		}

		private void onPropChgdEvent_GenProcess(PropChgEvtArgs e)
		{
			if (e.PropId == (PI_GEN_RUNNING_STAT))
			{
				// Debug.WriteLine($"got {PI_XDATA_WBK_SC} event");
				SystemRunningStatus = (RunningStatus) e.Value.AsEnum();
				return;
			}

			if (e.PropId == (PI_WBK_IS_MOD_EXO))
			{
				WbkIsModExo = e.Value.AsBool();
				return;
			}

			if (e.PropId == (PI_XDATA_WBK_SC))
			{
				SheetSchemaStatus = xData.GotShtSchema;
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

		private void onPropChgdEvent_UiProcess(PropChgEvtArgs e)
		{
			if (e.PropId == (PI_GEN_RESTART_REQD))
			{
				RestartStatus = e.Value.AsBool();
				RestartReqd = e.Value.AsBool();
				return;
			}

			if (e.PropId == (PI_GEN_NEEDS_SAVING))
			{
				NeedsSaving = e.Value.AsBool();
				return;
			}

			if (e.PropId == (PI_WBK_IS_MOD_EXO))
			{
				WbkIsModExo = e.Value.AsBool();
				return;
			}

			if (e.PropId == (PI_WBK_APPLY_BTN_STAT))
			{
				WbkApplyBtnStat = e.Value.AsBool();
				return;
			}

			if (e.PropId == (PI_WBK_UNDO_BTN_STAT))
			{
				WbkUndoBtnStat = e.Value.AsBool();
				return;
			}

			if (e.PropId == (PI_SHT_IS_MOD_EXO))
			{
				CurrShtIsModExo = e.Value.AsBool();
				return;
			}

			if (e.PropId == (PI_SHT_APPLY_BTN_STAT))
			{
				CurrShtApplyBtnStat = e.Value.AsBool();
				return;
			}

			if (e.PropId == (PI_SHT_UNDO_BTN_STAT))
			{
				CurrShtUndoBtnStat = e.Value.AsBool();
				return;
			}

			if (e.PropId == (PI_SHT_FAM_LST_IS_DIRTY))
			{
				CurrShtFamLstIsDirty = e.Value.AsBool();
				return;
			}

			if (e.PropId == (PI_SHT_IS_MOD_FAM_LST_WKG))
			{
				CurrShtIsModFamLstWkg = e.Value.AsBool();
				return;
			}

			if (e.PropId == (PI_XD_APPLY_BTN_SHTS_LST))
			{
				XdApplyBtnShtsLst = e.Value.AsBool();
				return;
			}

			if (e.PropId == (PI_XD_UNDO_BTN_SHTS_LST))
			{
				XdUndoBtnShtsLst = e.Value.AsBool();
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
		public void OnPropChgdEvent(object sender, PropChgEvtArgs e)
		{
			onPropChgdEvent_Process(e);
		}

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
		
	#region commands

	#endregion
	}
}