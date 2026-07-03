
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;


// user name: jeffs
// created:   2/8/2026 8:09:36 AM

namespace UtilityLibrary
{
	public class GeneralLib
	{
		public static string CvtEnumToString(Enum e)
		{
			CultureInfo ci = CultureInfo.CurrentCulture;
			TextInfo ti = ci.TextInfo;

			string name = e.ToString();
			name = name.Replace('_', ' ');

			return ti.ToTitleCase(name);
		}
		
	}
}
