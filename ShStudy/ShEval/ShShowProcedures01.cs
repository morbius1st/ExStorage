#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageC.ShSchemaFields;

using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields;

using static ShExStorageC.ShSchemaFields.ScSupport.SchemaLockKey;

#endregion

// user name: jeffs
// created:   10/16/2022 8:46:15 AM

namespace ShStudy.ShEval
{
	public class ShShowProcedures01
	{
		private ShDebugMessages M { get; set; }

		private ShowLibrary sl;
		private ShFieldDisplayData shFd;

		public ShShowProcedures01(IWin w, ShDebugMessages msgs)
		{
			M = msgs;

			sl = new ShowLibrary(w);
			shFd = new ShFieldDisplayData();
		}

		private void showDynaValue(DynaValue dv)
		{
			M.NewLine();
			M.WriteLineAligned($"DynaValue Value is   | {dv.Value ?? "null"} of type {dv.Value?.GetType() ?? "null"}");
			M.WriteLineAligned($"DynaValue typeis is  | {dv.TypeIs}");
			M.WriteLineAligned($"DynaValue is string? | {dv.IsString}");
			M.WriteLineAligned($"DynaValue is enum?   | {dv.IsEnum}");
			M.WriteLineAligned($"DynaValue is double? | {dv.IsDouble}");
			M.WriteLineAligned($"DynaValue is int?    | {dv.IsInt}");
			M.WriteLineAligned($"DynaValue is bool?   | {dv.IsBool}");

			M.WriteLineAligned($"DynaValue as string?                | {dv.AsString()}");
			M.WriteLineAligned($"DynaValue value returned Valid?     | {dv.LastValueReturnedIsValid}");
			// string s = dv.AsString();
			M.WriteLineAligned($"DynaValue as enum?                  | {dv.AsEnum()}");
			M.WriteLineAligned($"DynaValue value returned Valid?     | {dv.LastValueReturnedIsValid}");
			// Enum e = dv.AsEnum();
			M.WriteLineAligned($"DynaValue as double?                | {dv.AsDouble()}");
			M.WriteLineAligned($"DynaValue value returned Valid?     | {dv.LastValueReturnedIsValid}");
			// double d = dv.AsDouble();
			M.WriteLineAligned($"DynaValue as int?                   | {dv.AsInt()}");
			M.WriteLineAligned($"DynaValue value returned Valid?     | {dv.LastValueReturnedIsValid}");
			// int i = dv.AsInt();
			M.WriteLineAligned($"DynaValue as bool?                  | {dv.AsBool()}");
			M.WriteLineAligned($"DynaValue value returned Valid?     | {dv.LastValueReturnedIsValid}");
			// bool b = dv.AsBool();


			M.WriteLineAligned($"DynaValue ConvertValueTo as string? | {dv.ConvertValueTo<string> ()}");
			M.WriteLineAligned($"DynaValue value returned Valid?     | {dv.LastValueReturnedIsValid}");
			M.WriteLineAligned($"DynaValue ConvertValueTo as enum?   | {dv.ConvertValueTo<Enum  > ()}");
			M.WriteLineAligned($"DynaValue value returned Valid?     | {dv.LastValueReturnedIsValid}");
			M.WriteLineAligned($"DynaValue ConvertValueTo as double? | {dv.ConvertValueTo<double> ()}");
			M.WriteLineAligned($"DynaValue value returned Valid?     | {dv.LastValueReturnedIsValid}");
			M.WriteLineAligned($"DynaValue ConvertValueTo as int?    | {dv.ConvertValueTo<int   > ()}");
			M.WriteLineAligned($"DynaValue value returned Valid?     | {dv.LastValueReturnedIsValid}");
			M.WriteLineAligned($"DynaValue ConvertValueTo as bool?   | {dv.ConvertValueTo<bool  > ()}");
			M.WriteLineAligned($"DynaValue value returned Valid?     | {dv.LastValueReturnedIsValid}");

		}

		public void ShowDynaValue()
		{
			DynaValue dv;


			// as enum
			dv = new DynaValue(LK0_KEY);
			showDynaValue(dv);


			// as string
			dv = new DynaValue("one");
			showDynaValue(dv);


			// as double
			dv = new DynaValue(1.0);
			showDynaValue(dv);

			// as int
			dv = new DynaValue(1);
			showDynaValue(dv);

			// as bool
			dv = new DynaValue(true);
			showDynaValue(dv);

			// as null
			dv = new DynaValue(null);
			showDynaValue(dv);
		}

		public void ShowExid(ExId e)
		{
			M.WriteLine("\nshowEXID\n");

			M.NewLine();
			M.WriteLineAligned($"Ex Id      | {e.ExsId}");
			M.WriteLineAligned($"Ex Id table| {e.ExsIdTable}");
			M.WriteLineAligned($"Ex Id row | {e.ExsIdRow}");
			M.WriteLineAligned($"DocName    | {e.DocName}");
			M.WriteLineAligned($"VendorId   | {e.VendorId}");
			M.WriteLineAligned($"GotDoc?    | {e.GotDocument}");
			M.NewLine();
		}


		public void ShowFieldsHeader()
		{
			sl.WriteRow(
				shFd.ScFieldsColOrder,
				shFd.ScFieldsColDefinitions,
				ShFieldDisplayData.ScRecordHeaderTitles,
				10, JustifyVertical.BOTTOM, true, true);
		}


		public void ShowDataHeader()
		{
			sl.WriteRow(
				shFd.ScDataColOrder,
				shFd.ScFieldsColDefinitions,
				ShFieldDisplayData.ScRecordHeaderTitles,
				10, JustifyVertical.TOP, true, true);
		}

		
		public void ShowTableData(ScDataTable tbld)
		{
			M.WriteLine("\nTABLE data\n");

			ScValues<SchemaTableKey> values = new ScValues<SchemaTableKey>();
			values.setDataValues(tbld.Fields);

			M.MarginUp();

			ShowDataHeader();

			sl.WriteRows(
				shFd.ScDataColOrder,
				shFd.ScFieldsColDefinitions,
				shFd.SchemaTableKeyOrder,
				values.ScDataValues,
				10, JustifyVertical.TOP, false, false);

			M.MarginDn();
		}

		public void ShowLockData(ScDataLock lokd)
		{
			M.WriteLine("\nLOCK data\n");

			ScValues<SchemaLockKey> values = new ScValues<SchemaLockKey>();
			values.setDataValues(lokd.Fields);

			ShowDataHeader();

			sl.WriteRows(
				shFd.ScDataColOrder,
				shFd.ScFieldsColDefinitions,
				shFd.SchemaLockKeyOrder,
				values.ScDataValues,
				10, JustifyVertical.TOP, false, false);
		}

		public void ShowRowData(ScDataRow rowd)
		{
			M.WriteLine("\nROW data\n");

			ScValues<SchemaRowKey> values = new ScValues<SchemaRowKey>();
			values.setDataValues(rowd.Fields);

			ShowDataHeader();

			sl.WriteRows(
				shFd.ScDataColOrder,
				shFd.ScFieldsColDefinitions,
				shFd.SchemaRowKeyOrder,
				values.ScDataValues,
				10, JustifyVertical.TOP, false, false);
		}

		public void ShowTableDataGeneric<Tkey, Tdata, TCkey, TCdata>(

			IShScBaseData<Tkey, Tdata, TCkey, TCdata> BaseData) 
			where Tkey : Enum
			where TCkey : Enum
			where Tdata : ShScFieldDefData<Tkey>, new()
			where TCdata : AShScInfoBase<TCkey, ShScFieldDefData<TCkey>>, new()
		{
			foreach (KeyValuePair<Tkey, Tdata> kvp in BaseData.Fields)
			{
				
			}

			foreach (KeyValuePair<string, TCdata> aShScInfoBase in BaseData.Rows)
			{
				
			}
		}
	}
}
