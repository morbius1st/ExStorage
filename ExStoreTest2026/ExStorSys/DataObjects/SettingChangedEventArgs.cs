
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExStorSys;


// user name: jeffs
// created:   10/15/2025 10:20:41 PM

namespace ExStorSys
{
	public class SettingChangedEventArgs 
	{
		public SettingChangedEventArgs(SettingId settingId, DynaValue value)
		{
			SettingId = settingId;
			Value = value;
		}

		public SettingId SettingId { get; set; }
		public DynaValue Value { get; set; }
	}
}
