using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStoreTest2026;
using RevitLibrary;
using RevitLibrary;
using UtilityLibrary;
using static ExStorSys.WorkBookFieldKeys;


// projname: $projectname$
// itemname: ExStorId
// username: jeffs
// created:  9/20/2025 4:33:42 PM

namespace ExStorSys
{
	public class Exid
	{
	#region private fields

	#endregion

	#region ctor

		// private static readonly Lazy<Exid> instance =
		// 	new Lazy<Exid>(() => new Exid());


		public Exid()
		{
			RevitAddinsUtil.ReadManifest(CsUtilities.AssemblyName);

			// ModelName = R.RvtDoc?.Title ?? "";
			// Model_Name = ExStorLib.Instance.CleanName(R.RvtDoc?.Title);
		}

	#endregion

	#region public properties

		/* revit names */

		/// <summary>
		/// Actual model name (title) (un-cleaned)
		/// </summary>
		public string ModelName => R.RvtDoc?.Title ?? "";

		/// <summary>
		/// Cleaned actual model name (title)
		/// </summary>
		public string Model_Name =>ExStorLib.Instance.CleanName(R.RvtDoc?.Title);


		/* ex stor names */

		// official location to retrieve
		// public string TempModelCode => ExStorMgr.Instance.TempModelCode;

	#endregion

	#region private properties

	#endregion

	#region public methods

		/* utility */

		public string? GetNextId(string lastId)
		{
			string nextId = ExStorConst.CreateNextIdCode(lastId);

			return nextId;
		}

		// in ExStorConst
		//
		// const names
		// workbook schema name         == WbkSchemaName          == CsCells_WBK_Schema_v1_00
		// sheet schema name            == ShtSchemaName          == CsCells_SHT_Schema_v1_00
		
		// general workbook search name == EXS_WBK_NAME_SEARCH == CsCells_WBK_
		// general sheet search name    == EXS_SHT_NAME_SEARCH == CsCells_SHT_

		// in here
		//
		// workbook ds name             == CreateWbkDsName        == CsCells_WBK_ + {model code} + {version} 
		//		==, for example, CsCells_WBK_250101_160101_v1_00
		//
		// sheet ds name                == CreateShtDsName        == CsCells_SHT_ + {model code} + {id code} + {version}
		//		==, for example, CsCells_SHT_250101_16010_ABCD_v1_00


		/* search names */

		/// <summary>
		/// includes app code + wkb name code - does not include model code
		/// </summary>
		public string WbkSearchName => ExStorConst.EXS_WBK_NAME_SEARCH;

		/// <summary>
		/// includes app code + sheet name code - does not include model code
		/// </summary>
		public string ShtSearchName => ExStorConst.EXS_SHT_NAME_SEARCH;

		// /// <summary>
		// /// includes app code + sheet name code + model code
		// /// </summary>
		// public string ShtDsSearchNameModelSpecific => $"{ExStorConst.EXS_SHT_NAME_SEARCH}{ExStorMgr.Instance.xData.TempModelCode}";


		/* schema names (constants) */

		/// <summary>
		/// Full name of the workbook schema
		/// </summary>
		public string WbkSchemaName => ExStorConst.WbkSchemaName;

		/// <summary>
		/// Full name of the sheet schema
		/// </summary>
		public string ShtSchemaName => ExStorConst.ShtSchemaName;

		/* ds names */

		/// <summary>
		/// CsCells_WBK_v1_00 / example: CsCells_WBK_v1_00<br/>
		/// id code removed
		/// </summary>
		public string CreateWbkDsName()
		{
			return $"{ExStorConst.EXS_WBK_NAME_SEARCH}{ExStorConst.EXS_VERSION_WBK}";
		}

		// /// <summary>
		// /// CsCells_Sht_{model code}_{ID}_v1_00 / example: CsCells_SHT_250101_16010_ABCD_v1_00<br/>
		// /// model code / id code reversed
		// /// </summary>
		// public string CreateShtDsName(string id)
		// {
		// 	return $"{ExStorConst.EXS_SHT_NAME_SEARCH}{ExStorMgr.Instance.xData.TempModelCode}_{id}_{ExStorConst.EXS_VERSION_SHT}";
		// }

		/// <summary>
		/// CsCells_Sht_{model code}_{ID}_v1_00 / example: CsCells_Sht_250101_16010_ABCD_v1_00<br/>
		/// model code / id code reversed now
		/// </summary>
		public string CreateShtDsName(string id)
		{
			return $"{ExStorConst.EXS_SHT_NAME_SEARCH}{id}_{ExStorConst.EXS_VERSION_SHT}";
		}

		/// <summary>
		///CsCells_Sht_250101_16010_AAAA_v1_00<br/>
		/// model code / id code reversed
		/// </summary>
		public string CreateFirstShtDsName()
		{
			return $"{ExStorConst.EXS_SHT_NAME_SEARCH}{ExStorConst.EXS_SHT_FIRST_ID_CODE}_{ExStorConst.EXS_VERSION_SHT}";
		}


	#endregion

	#region private methods

	#endregion

	#region event consuming

	#endregion

	#region event publishing

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(Exid)}";
		}

	#endregion
	}
}