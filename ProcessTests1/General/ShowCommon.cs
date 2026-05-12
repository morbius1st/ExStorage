
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExStorSys;
using UtilityLibrary;


// user name: jeffs
// created:   5/4/2026 8:26:11 PM

namespace ProcessTests1.General
{
	public static class ShowCommon
	{

		public static void ShowCurrent(string title, ExStorData _xData, WorkBook _wbkA)
		{
			WorkBook? _wbkB = _xData?.WorkBook;

			R.WriteLine($"\n*****\n**  {title}\n****");

			if (_wbkB != null)
			{
				R.WriteLine("\n*** show workbook");
				R.WriteLine($"name {_wbkB.DsName}");
				R.WriteLine($"last id (A){_wbkA.LastId}");
				R.WriteLine($"last id (B){_wbkB.LastId}");
			}
			else
			{
				R.WriteLine("\n*** workbook is null");
			}

			if (_xData != null)
			{
				R.WriteLine("\n*** show xData");
				R.WriteLine($"current sheet is {_xData.CurrentSheet}");
				R.WriteLine($"sheet list count {_xData.SheetsCount}");
				R.WriteLine($"selected sheet {_xData.SelectSheet}");


				if (_xData.CurrentSheet != null)
				{
					R.WriteLine("\n*** show curr sheet");
					Sheet sht = _xData.CurrentSheet;
					R.WriteLine($"is modified? {sht.IsModifiedExo}");
					R.WriteLine($"fam list modified? {sht.IsModifiedFamList}");
					R.WriteLine($"is fam list field dirty? {sht.FamilyListField.IsDirty()}");
				}
				else
				{
					R.WriteLine("\n*** current sheet is null");
				}

				R.WriteLine("\nsheet list");
				foreach (var kvp in _xData.Sheets)
				{
					R.WriteLine($"{kvp.Key}");
				}

			}
			else
			{
				R.WriteLine("\n*** xData is null\n");
			}

			R.WriteLine("\n*** complete ****\n");

		}


	}
}
