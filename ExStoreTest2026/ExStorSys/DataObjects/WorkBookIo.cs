
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using ExStorSys;
using JetBrains.Annotations;
using static ExStorSys.WorkBookFieldKeys;


// user name: jeffs
// created:   1/18/2026 6:28:22 PM

// namespace ExStorSys
// {
//
// 	public class WorkBookIo : INotifyPropertyChanged
// 	{
// 		public WorkBookIo(ref Dictionary<WorkBookFieldKeys, FieldData<WorkBookFieldKeys>> rows)
// 		{
// 			this.rows = rows;
// 		}
//
// 		private Dictionary<WorkBookFieldKeys, FieldData<WorkBookFieldKeys>> rows;
//
// 		public string DsName => rows[PK_DS_NAME].DyValue!.Value;
// 		public FieldDef<WorkBookFieldKeys> DsNameField => rows[PK_DS_NAME].Field;
// 		
// 		public string Desc 
// 		{
// 			get => rows[PK_AD_DESC].DyValue!.Value;
// 			set
// 			{
// 				rows[PK_AD_DESC].DyValue!.ChangeValue(value);
// 				OnPropertyChanged();
// 			}
// 		}
// 		public FieldDef<WorkBookFieldKeys> DescField => rows[PK_AD_DESC].Field;
//
// 		public ActivateStatus Status
// 		{
// 			get => (ActivateStatus)rows[PK_AD_STATUS].DyValue!.Value;
// 			set
// 			{
// 				rows[PK_AD_STATUS].DyValue!.ChangeValue((int)value);
// 				OnPropertyChanged();
// 			}
// 		}
// 		public FieldData<WorkBookFieldKeys> StatusField => rows[PK_AD_STATUS];
//
//
// 		public event PropertyChangedEventHandler PropertyChanged;
//
// 		[DebuggerStepThrough]
// 		[NotifyPropertyChangedInvocator]
// 		private void OnPropertyChanged([CallerMemberName] string memberName = "")
// 		{
// 			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
// 		}
// 	}
// }
