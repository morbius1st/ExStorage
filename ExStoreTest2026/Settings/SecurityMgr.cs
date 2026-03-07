using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ExStorSys;
using JetBrains.Annotations;
using RevitLibrary;
using UtilityLibrary;


// projname: $projectname$
// itemname: SecurityMgr
// username: jeffs
// created:  1/1/2026 9:47:03 PM

namespace ExStorSys
{
	/* security */

	public enum UserSecutityLevel
	{
		SL_UNASSIGNED		= 0,
		SL_DEBUG			= 10,
		SL_ADVANCED			= 40,
		SL_BASIC			= 60,
		SL_VIEW_ONLY		= 100,
		SL_LIMITED			= 1000,
		SL_NONE				= 100000,
	}


	public class SecurityMgr : INotifyPropertyChanged
	{
	#region private fields

		private static FieldEditLevel fieldEditLevel;
		private static string userName = "unassigned";
		private static string? userName2 = "unassigned";

		private static readonly Lazy<SecurityMgr> instance =
			new Lazy<SecurityMgr>(() => new SecurityMgr());

	#endregion

	#region ctor

		private SecurityMgr() { }

		public static SecurityMgr Instance => instance.Value;

	#endregion

	#region public properties

		public void Init(string? userName, string? rvtUsderName)
		{
			UserName = userName!;
			UserName2 = rvtUsderName;

			if (tempAccess.TryGetValue(userName, out userSecurityLevel))
			{
				OnPropertyChanged(nameof(UserSecurityLevel));
			}

		}

		public string UserName
		{
			get => userName;
			private set
			{
				// if (userName.Equals(value)) return;
				userName = value;
				OnPropertyChanged();
			}
		}

		public string? UserName2
		{
			get => userName2;
			private set
			{
				// if (userName2.Equals(value)) return;
				userName2 = value;
				OnPropertyChanged();
			}
		}

		public UserSecutityLevel UserSecurityLevel
		{
			get => userSecurityLevel;
			private set
			{
				// if (value == userSecurityLevel) return;
				userSecurityLevel = value;
				OnPropertyChanged();
			}
		}

		public  FieldEditLevel FieldEditLevel
		{
			get => fieldEditLevel;
			set
			{
				// if (fieldEditLevel == value) return;
				fieldEditLevel = value;
				OnPropertyChanged();

				OnPropertyChanged(nameof(EditLeveName));
				OnPropertyChanged(nameof(EditLevelDesc));
			}
		}

		public string EditLeveName => ExStorConst.FieldEditLevelDesc[fieldEditLevel].Item1;
		public string EditLevelDesc => ExStorConst.FieldEditLevelDesc[fieldEditLevel].Item2;

	#endregion

	#region private properties

	#endregion

	#region public methods

		public void ResetPropChanged()
		{
			PropertyChanged = null;
		}

		public static FieldEditStatus ValidateFieldEditing(FieldEditLevel fl, UserSecutityLevel ul)
		{
			int fel = (int) fl;
			int usl = (int) ul;

			int felInt;
			int felFract;
			
			FieldEditStatus fes = FieldEditStatus.FES_NONE;

			if (ul == UserSecutityLevel.SL_UNASSIGNED) return fes;

			if (fel >= usl)
			{
				felInt = (fel / 10) * 10;
				felFract = fel - felInt;

				if (felInt == usl)
				{
					if (fel == usl)
					{
						fes = FieldEditStatus.FES_CAN_EDIT;
					}
					else if (felFract == (int) FelSubCat.FESC_VIEWONLY)
					{
						fes = FieldEditStatus.FES_CAN_VIEW;
					}
				}
				else
				{
					fes = FieldEditStatus.FES_CAN_EDIT;
				}
			}
			else
			{
				if (fl == FieldEditLevel.FEL_LOCKED && ul == UserSecutityLevel.SL_DEBUG) 
					fes = FieldEditStatus.FES_CAN_VIEW;
				else
				if (fl == FieldEditLevel.FEL_VIEW_ONLY && ul <= UserSecutityLevel.SL_VIEW_ONLY)
					fes = FieldEditStatus.FES_CAN_VIEW;
				else
				if (ul == UserSecutityLevel.SL_VIEW_ONLY 
					&& fl >= FieldEditLevel.FEL_BASIC) fes = FieldEditStatus.FES_CAN_VIEW;
				else
				if (ul == UserSecutityLevel.SL_LIMITED 
					&& fl == FieldEditLevel.FEL_BAS_VIEW_ONLY) fes = FieldEditStatus.FES_CAN_VIEW;
			}

			return fes;
		}

	#endregion

	#region private methods

	#endregion

		/* temp data */

		private Dictionary<string, UserSecutityLevel> tempAccess = new ()
		{
			{ "johns", UserSecutityLevel.SL_BASIC},
			{ "jimmys",UserSecutityLevel.SL_NONE},
			{ "jacks", UserSecutityLevel.SL_ADVANCED},
			{ "jeffs", UserSecutityLevel.SL_DEBUG },
			
		};

		private UserSecutityLevel userSecurityLevel;

	#region event consuming

	#endregion

	#region event publishing


		// used by Mui and reset to null by Mui
		public event PropertyChangedEventHandler PropertyChanged;

		[DebuggerStepThrough]
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(SecurityMgr)}";
		}

	#endregion
	}
}