using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityLibrary;


// user name: jeffs
// created:   4/18/2026 6:15:15 PM

namespace ExStorSys
{
	public class Exid
	{
		/// <summary>
		/// CsCells_Sht_{ID}_v1_00 / example: CsCells_Sht_ABCD_v1_00<br/>
		/// model code removed, uses current version
		/// </summary>
		public string CreateShtDsName(string id)
		{
			return $"{ExStorConst.EXS_SHT_NAME_SEARCH}{id}_{ExStorConst.EXS_VERSION_SHT}";
		}
	}

	public class ExStorLib
	{
		/// <summary>
		/// Format the family name and family type name into a discionary key
		/// </summary>
		public static string FormatFamAndType(string famName, string? typeName)
		{
			string key = $"{famName}|{typeName}";

			return key;
		}

		/// <summary>
		/// Separate from the key the family name and the family type name
		/// </summary>
		public static bool DivideFamAndType(string? famAndType, out string? family, out string? famType)
		{
			family = null;
			famType = null;
			int pos;

			if (famAndType!.IsVoid()) return false;

			pos = famAndType.IndexOf('|');

			// false if not there, or at begining, or at end
			if (pos == -1 || pos == 0 /*|| pos == famAndType.Length-1* - removed - empty type name is ok */) return false;

			// family is from start to the dividing charater but not including the dividing charagter
			family = famAndType.Substring(0, pos);
			famType = famAndType.Substring(pos + 1, famAndType.Length - (pos + 1));

			return true;
		}

		/// <summary>
		/// Extract the Id code from a sheet name (used when converting from one version to another)
		/// </summary>
		public string? ExtractIdFromShtName(string? name, string searchName)
		{
			// sht ds
			//           1         2         3
			// 0123456789012345678901234567890
			// v----------v---v------e  
			// CsCells_SHT_AAAA_v1_00
			// look for search name
			if (name.IsVoid()) return null;

			if (name!.Length != searchName.Length + 10) return null;

			return name.Substring(searchName.Length, 4);
		}
	}

	public static class ExStorTest
	{
		public static void ChangeField(DynaValue d, string value) { }
	}
}