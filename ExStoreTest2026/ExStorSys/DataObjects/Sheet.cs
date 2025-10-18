
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB.ExtensibleStorage;

using static ExStorSys.ExoCreationStatus;


using static System.Runtime.InteropServices.JavaScript.JSType;
using static ExStorSys.SheetFieldKeys;

// user name: jeffs
// created:   9/25/2025 7:25:25 PM

namespace ExStorSys
{
	public struct SheetCreationData
	{
		public SheetCreationData(string xlPath, string xlSheetName)
		{
			XlFilePath = xlPath;
			XlSheetName = xlSheetName;
		}

		public string XlFilePath { get; set; }
		public string XlSheetName { get; set; }

		public SheetOpStatus OpStatus { get; set; } = SheetOpStatus.SS_GOOD;
		public string Sequence { get; set; } = "A00";

		public UpdateRules UpdateRule { get; set; } = UpdateRules.UR_UPON_REQUEST;

		public IList<string> FamililyAndType = new List<string>() { "Undefined|Unset" };

		public bool Skip { get; set; } = false;
	}

	/// <summary>
	/// the secondary data object(s) stored in the data storage object 
	/// </summary>
	public class Sheet : ExStorDataObj<SheetFieldKeys>
	{
		private Sheet()
		{
			Rows = new ();
			init(Fields.SheetFields);

			CreationStatus = CS_CREATED;

			ExMgr = ExStorMgr.Instance;
		}

		/*overrides*/

		public override Schema? ExsSchema
		{
			get => ExMgr.WorkBook.ExsSheetSchema;
			set => ExMgr.WorkBook.ExsSheetSchema = value;
		}

		/*shortcuts & properties*/

		public override int WbkOrSht => 1;

		/// <summary>
		/// the root name for searching for the actual DS - does not include
		/// model code or thereafter.
		/// </summary>
		public override string DsSearchName => ExStorConst.EXS_SHT_NAME_SEARCH;

		public override string DsName => Rows[RK_DS_NAME].DyValue!.Value;
		public override string Desc => Rows[RK_AD_DESC].DyValue!.Value;

		public override string SchemaName => ExStorMgr.Instance.Exid.ShtSchemaName;
		public override string SchemaDesc => $"Sheet Schema for {ExStorConst.EXS_SHT_NAME_SEARCH}";
		public override Guid SchemaGuid =>   Guid.NewGuid();

		/// <summary>
		/// create an "invalid" sheet - used as a return value rather than null
		/// </summary>
		public static Sheet Invalid()
		{
			Sheet sht = Sheet.CreateEmptySheet("Invalid");
			sht.CreationStatus = CS_INVALID;

			return sht;
		}

		/// <summary>
		/// create a named & empty sheet, which will contain the DS
		/// </summary>
		public static Sheet CreateEmptySheet(DataStorage ds)
		{
			Sheet sht = CreateEmptySheet(ds.Name);

			sht.ExsDataStorage = ds;

			sht.CreationStatus = CS_CREATED;

			return sht;
		}

		/// <summary>
		/// create a named & empty sheet
		/// </summary>
		public static Sheet CreateEmptySheet(string shtName)
		{
			Sheet sht = new Sheet();

			sht.updateWithInitialData(shtName);

			sht.CreationStatus = CS_EMPTY;

			return sht;
		}

		/// <summary>
		/// create a complete sheet populated with sheetCreationData
		/// </summary>
		public static Sheet? CreateSheet(string shtName, SheetCreationData sd)
		{
			Sheet sht = new Sheet();

			sht.updateWithCurrentData(shtName, sd);

			sht.CreationStatus = ExoCreationStatus.CS_CREATED;

			return sht;
		}

		private void updateWithInitialData(string shtName)
		{
			UpdateRow(RK_DS_NAME, new DynaValue(shtName));

		}

		private void updateWithCurrentData(string shtName, SheetCreationData sd)
		{
			IsEmpty = false;
			
			updateWithInitialData(shtName);
			
			UpdateRow(RK_AD_DESC, new DynaValue($"Sheet for {ExStorMgr.Instance.Exid.ModelName}"));

			UpdateRow(RK_AD_DATE_CREATED  , new DynaValue(DateTime.Now.ToString("s")));
			UpdateRow(RK_AD_NAME_CREATED  , new DynaValue(ExStorConst.UserName));
			UpdateRow(RK_AD_DATE_MODIFIED , new DynaValue(DateTime.Now.ToString("s")));
			UpdateRow(RK_AD_NAME_MODIFIED , new DynaValue(ExStorConst.UserName));

			UpdateRow(RK_ED_XL_FILE_PATH  , new DynaValue(sd.XlFilePath));
			UpdateRow(RK_ED_XL_SHEET_NAME , new DynaValue(sd.XlSheetName));
			UpdateRow(RK_OD_STATUS        , new DynaValue(sd.OpStatus));
			UpdateRow(RK_OD_SEQUENCE      , new DynaValue(sd.Sequence));
			UpdateRow(RK_OD_UPDATE_RULE   , new DynaValue(sd.UpdateRule)); 
			UpdateRow(RK_OD_UPDATE_SKIP   , new DynaValue(sd.Skip));
			UpdateRow(RK_RD_FAMILY_LIST, new DynaValue(sd.FamililyAndType));
		}

		public override string ToString()
		{
			return $"DS Name {DsName}";
		}

	}

}
