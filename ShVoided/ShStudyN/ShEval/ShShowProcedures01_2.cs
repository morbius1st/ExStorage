#region + Using Directives

using static ShExStorageN.ShSchemaFields.ShScSupport.ShExConstN;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaLockKey;

#endregion

// user name: jeffs
// created:   10/16/2022 8:46:15 AM

namespace ShStudyN.ShEval
{
	public class ShShowProcedures01
	{
		private ShDebugMessages M { get; set; }

		private ShowLibrary sl;
		private ShowLibrary2 sl2;
		private ShFieldDisplayData shFd;

		public ShShowProcedures01(ShDebugMessages msgs)
		{
			M = msgs;

			sl = new ShowLibrary(msgs);
			sl2 = new ShowLibrary2(msgs);
			shFd = new ShFieldDisplayData();
		}


	#region keep methods

		public void ShowExid(AExId exid)
		{
			M.WriteLine("\nshowEXID\n");

			M.WriteLineAligned($"ExId is ready?| {exid != null}");

			if (exid == null)
			{
				return;
			}

			M.NewLine();
			M.WriteLineAligned($"ExId         | {AExId.Exid}");
			M.WriteLineAligned($"ExId ds name | {exid.DsName}");
			M.WriteLineAligned($"ExId sch name| {exid.SchemaName}");
			M.WriteLineAligned($"ExId row     | {exid.RowSchemaName(K_NOT_DEFINED_STR)}");
			M.WriteLineAligned($"DocumentName | {AExId.DocumentName}");
			M.WriteLineAligned($"DocName      | {AExId.DocNameClean}");
			M.WriteLineAligned($"VendorId     | {AExId.VendorId}");
			M.WriteLineAligned($"CompanyId    | {AExId.CompanyId}");
			M.NewLine();
		}

		public void ShowDynaValue()
		{
			DynaValue dv;

			
			// as Guid
			dv = new DynaValue(Guid.NewGuid());
			showDynaValue(dv);

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

		private void showDynaValue(DynaValue dv)
		{
			M.NewLine();
			M.WriteLineAligned($"DynaValue Value is   | {dv.Value ?? "null"} of type {((Type) dv.Value?.GetType())?.Name ?? "null"}");
			M.WriteLineAligned($"DynaValue typeis is  | {dv.TypeIs?.Name ?? "is null"}");

			M.NewLine();
			M.WriteLineAligned($"DynaValue is string? | {dv.IsString}");
			M.WriteLineAligned($"DynaValue is enum?   | {dv.IsEnum}");
			M.WriteLineAligned($"DynaValue is double? | {dv.IsDouble}");
			M.WriteLineAligned($"DynaValue is int?    | {dv.IsInt}");
			M.WriteLineAligned($"DynaValue is guid?   | {dv.IsGuid}");
			M.WriteLineAligned($"DynaValue is bool?   | {dv.IsBool}");

			M.NewLine();
			M.WriteLineAligned($"DynaValue as string?            | {dv.AsString()}");
			M.WriteLineAligned($"DynaValue value returned Valid? | {dv.LastValueReturnedIsValid}");

			M.WriteLineAligned($"DynaValue as enum?              | {dv.AsEnum()}");
			M.WriteLineAligned($"DynaValue value returned Valid? | {dv.LastValueReturnedIsValid}");

			M.WriteLineAligned($"DynaValue as double?            | {dv.AsDouble()}");
			M.WriteLineAligned($"DynaValue value returned Valid? | {dv.LastValueReturnedIsValid}");

			M.WriteLineAligned($"DynaValue as int?               | {dv.AsInt()}");
			M.WriteLineAligned($"DynaValue value returned Valid? | {dv.LastValueReturnedIsValid}");

			M.WriteLineAligned($"DynaValue as guid?              | {dv.AsGuid()}");
			M.WriteLineAligned($"DynaValue value returned Valid? | {dv.LastValueReturnedIsValid}");

			M.WriteLineAligned($"DynaValue as bool?              | {dv.AsBool()}");
			M.WriteLineAligned($"DynaValue value returned Valid? | {dv.LastValueReturnedIsValid}");

			M.NewLine();
			M.WriteLineAligned($"DynaValue ConvertValueTo as string? | {dv.GetValueAs<string> ()}");
			M.WriteLineAligned($"DynaValue value returned Valid?     | {dv.LastValueReturnedIsValid}");
			M.WriteLineAligned($"DynaValue ConvertValueTo as enum?   | {dv.GetValueAs<Enum  > ()}");
			M.WriteLineAligned($"DynaValue value returned Valid?     | {dv.LastValueReturnedIsValid}");
			M.WriteLineAligned($"DynaValue ConvertValueTo as double? | {dv.GetValueAs<double> ()}");
			M.WriteLineAligned($"DynaValue value returned Valid?     | {dv.LastValueReturnedIsValid}");
			M.WriteLineAligned($"DynaValue ConvertValueTo as int?    | {dv.GetValueAs<int   > ()}");
			M.WriteLineAligned($"DynaValue value returned Valid?     | {dv.LastValueReturnedIsValid}");
			M.WriteLineAligned($"DynaValue ConvertValueTo as guid?   | {dv.GetValueAs<Guid  > ()}");
			M.WriteLineAligned($"DynaValue value returned Valid?     | {dv.LastValueReturnedIsValid}");
			M.WriteLineAligned($"DynaValue ConvertValueTo as bool?   | {dv.GetValueAs<bool  > ()}");
			M.WriteLineAligned($"DynaValue value returned Valid?     | {dv.LastValueReturnedIsValid}");

			M.NewLine();
		}


		//
		// field information
		//
		public void ShowLockFields()
		{
			M.WriteLine("\nLOCK fields\n");

			ScValues<SchemaLockKey> values = new ScValues<SchemaLockKey>();
			values.setFieldsValues(ScInfoMeta.FieldsLock);

			ShowFieldsHeader();

			sl.WriteRows(
				shFd.ScFieldsColOrder,
				shFd.ScFieldsColDefinitions,
				shFd.SchemaLockKeyOrder,
				values.ScFieldValues,
				10, JustifyVertical.TOP, false, false);
		}

		public void ShowSheetFields()
		{
			M.WriteLine("\nSHEET fields\n");

			ScValues<SchemaSheetKey> values = new ScValues<SchemaSheetKey>();
			values.setFieldsValues(ScInfoMeta.FieldsSheet);

			ShowFieldsHeader();

			sl.WriteRows(
				shFd.ScFieldsColOrder,
				shFd.ScFieldsColDefinitions,
				shFd.SchemaSheetKeyOrder,
				values.ScFieldValues,
				10, JustifyVertical.TOP, false, false);
		}

		public void ShowRowFields()
		{
			M.WriteLine("\nRow fields\n");

			ScValues<SchemaRowKey> values = new ScValues<SchemaRowKey>();
			values.setFieldsValues(ScInfoMeta.FieldsRow);

			ShowFieldsHeader();

			sl.WriteRows(
				shFd.ScFieldsColOrder,
				shFd.ScFieldsColDefinitions,
				shFd.SchemaRowKeyOrder,
				values.ScFieldValues,
				10, JustifyVertical.TOP, false, false);
		}


		public void ShowFieldsHeader()
		{
			sl.WriteRow(
				shFd.ScFieldsColOrder,
				shFd.ScFieldsColDefinitions,
				ShFieldDisplayData.ScRecordHeaderTitles,
				10, JustifyVertical.BOTTOM, true, true);
		}

		public void ShowDataHeader(List<ScFieldColumns> order)
		{
			sl.WriteRow(
				order,
				shFd.ScFieldsColDefinitions,
				ShFieldDisplayData.ScRecordHeaderTitles,
				10, JustifyVertical.TOP, true, true);
		}

		public void ShowSheetFieldsGeneric<TShtKey, TShtFld, TRowKey,
			TRowFld, TRow>(
			AShScSheet<TShtKey, TShtFld, TRowKey,
				TRowFld, TRow> shtd)
			where TShtKey : Enum
			where TShtFld : ScFieldDefData<TShtKey>, new()
			where TRowKey : Enum
			where TRowFld : ScFieldDefData<TRowKey>, new()
			where TRow : AShScRow<TRowKey, TRowFld>, new()
		{
			M.WriteLine("\nSHEET fields\n");

			showField(shtd, SchemaSheetKey.SK0_KEY);
			showField(shtd, SchemaSheetKey.SK0_SCHEMA_NAME);
			showField(shtd, SchemaSheetKey.SK0_DESCRIPTION);
			showField(shtd, SchemaSheetKey.SK0_GUID);

			M.WriteLine("\nROW fields\n");

			if (shtd.Rows.Count > 0)
			{
				foreach (KeyValuePair<string, TRow> kvp in shtd.Rows)
				{
					showField(kvp.Value, SchemaRowKey.RK0_KEY);
					showField(kvp.Value, SchemaRowKey.RK0_SCHEMA_NAME);
					showField(kvp.Value, SchemaRowKey.RK0_DESCRIPTION);
					showField(kvp.Value, SchemaRowKey.RK0_GUID);
				}
			}
			else
			{
				M.WriteLine("\nNO ROW data\n");
			}
		}

		// save - sheet specific example
		//
		// private void ShowShtField<TShtKey, TShtFld, TRowKey,
		// 	TRowFld, TRow>(
		// 	AShScFieldSheet<TShtKey, TShtFld, TRowKey,
		// 		TRowFld, TRow> shtd, Enum fieldKey)
		// 	where TShtKey : Enum
		// 	where TShtFld : IShScFieldData1<TShtKey>, new()
		// 	where TRowKey : Enum
		// 	where TRowFld : IShScFieldData1<TRowKey>, new()
		// 	where TRow : IShScFieldsBase1<TRowKey, TRowFld>, new()
		// {
		// 	TShtFld f1 = shtd.GetField(fieldKey);
		// 	string s1= f1.FieldName;
		// 	string s2= shtd.GetValue<string>(fieldKey);
		//
		// 	M.WriteLine($"Field| {s1}", $"value| {s2}");
		// }

		private void showField<TRowKey, TRowFld>
			(AShScFields<TRowKey, TRowFld> data, Enum fieldKey)
			where TRowKey : Enum
			where TRowFld : IShScFieldData<TRowKey>, new()
		{
			TRowFld f1 = data.GetField(fieldKey);
			string s1= f1.FieldName;
			string s2= data.GetValueAs<string>(fieldKey);

			M.WriteLine($"Field| {s1}", $"value| {s2}");
		}

		public void ShowSheetDataGeneric<TShtKey, TShtFlds, TRowKey,
			TRowFlds, TRow>(
			AShScSheet<TShtKey, TShtFlds, TRowKey,
				TRowFlds, TRow> shtd
			)
			where TShtKey : Enum
			where TRowKey : Enum
			where TShtFlds : ScFieldDefData<TShtKey>, new()
			where TRowFlds : ScFieldDefData<TRowKey>, new()
			where TRow : AShScRow<TRowKey, TRowFlds>, new()
		{
			if (shtd == null )
			{
				M.WriteLineStatus("show sheet data| fail - no data to show");

				return;
			}

			M.WriteLine($"for sheet| {shtd.SchemaName}");
			
			ShowSheetDataGeneric(shtd.Fields);

			if (shtd.Rows.Count > 0)
			{
				foreach (KeyValuePair<string, TRow> kvp in shtd.Rows)
				{
					M.WriteLine($"\n\nfor row| {kvp.Value.SchemaName}");

					ShowRowDataGeneric(kvp.Value.Fields);
				}
			}
			else
			{
				M.WriteLine("\nNO ROW data\n");
			}
		}



		//
		// data information
		//
		public void ShowSheetDataGeneric<TShtKey, TShtFlds>
			(Dictionary<TShtKey, TShtFlds> fields)
			where TShtKey : Enum
			where TShtFlds : IShScFieldData<TShtKey>, new()

		{
			ScValues<TShtKey> values = new ScValues<TShtKey>();
			values.setDataValues(fields);

			M.WriteLine("\nshow SHEET data| start\n");

			ShowDataHeader(shFd.ScDataColOrder);

			sl.WriteRows(
				shFd.ScDataColOrder,
				shFd.ScFieldsColDefinitions,
				shFd.SchemaSheetKeyOrder as List<TShtKey>,
				values.ScDataValues,
				10, JustifyVertical.TOP, false, false);
		}

		public void ShowRowDataGeneric<TRowKey, TRowFlds>
			(Dictionary<TRowKey, TRowFlds> fields)
			where TRowKey : Enum
			where TRowFlds : IShScFieldData<TRowKey>, new()

		{
			ScValues<TRowKey> values = new ScValues<TRowKey>();
			values.setDataValues(fields);

			M.WriteLine("\nROW data\n");

			ShowDataHeader(shFd.ScDataColOrder);

			sl.WriteRows(
				shFd.ScDataColOrder,
				shFd.ScFieldsColDefinitions,
				shFd.SchemaRowKeyOrder as List<TRowKey>,
				values.ScDataValues,
				10, JustifyVertical.TOP, false, false);
		}

		public void ShowLockDataGeneric<TLokKey, 
			TLokFlds>(AShScFields<TLokKey, TLokFlds> lok)
			where TLokKey : Enum
			where TLokFlds : IShScFieldData<TLokKey>, new()
		{
			M.WriteLineStatus("show lock data| begin");

			if (lok == null)
			{
				M.WriteLineStatus("show lock data| fail - no data to show");
				return;
			}

			showLockDataGeneric(lok.Fields);

			M.WriteLineStatus("show lock data| complete");
		}


		private void showLockDataGeneric<TLockKey, TLockFlds>
			(Dictionary<TLockKey, TLockFlds> fields)
			where TLockKey : Enum
			where TLockFlds : IShScFieldData<TLockKey>, new()

		{
			ScValues<TLockKey> values = new ScValues<TLockKey>();
			values.setDataValues(fields);

			M.WriteLine("\nLOCK data\n");

			ShowDataHeader(shFd.ScDataColOrderLight);

			sl.WriteRows(
				shFd.ScDataColOrderLight,
				shFd.ScFieldsColDefinitions,
				shFd.SchemaLockKeyOrder as List<TLockKey>,
				values.ScDataValues,
				10, JustifyVertical.TOP, false, false);
		}

	#endregion


	#region keep methods 2


		public void ShowLockDataGeneric2<TLokKey, 
			TLokFlds>(AShScFields2<TLokFlds> lok)
			where TLokFlds : IShScFieldData2, new()
		{
			M.WriteLineStatus("show lock data| begin");

			if (lok == null)
			{
				M.WriteLineStatus("show lock data| fail - no data to show");
				return;
			}

			showLockDataGeneric2(lok.Fields);

			M.WriteLineStatus("show lock data| complete");
		}


		private void showLockDataGeneric2<TLockFlds>
			(Dictionary<KEY, TLockFlds> fields)
			where TLockFlds : IShScFieldData2, new()

		{
			ScValues2 values = new ScValues2();
			values.setDataValues2(fields);

			M.WriteLine("\nLOCK data\n");

			ShowDataHeader(shFd.ScDataColOrderLight);

			sl2.WriteRows2(
				shFd.ScDataColOrderLight,
				shFd.ScFieldsColDefinitions,
				shFd.SchemaLockKeyOrder2 as List<KEY>,
				values.ScDataValues,
				10, JustifyVertical.TOP, false, false);
		}

		public void ShowSheetDataGeneric2<TShtFlds,
			TRowFlds, TRow>(
			AShScSheet2<TShtFlds, TRowFlds, TRow> shtd
			)
			where TShtFlds : ScFieldDefData2, new()
			where TRowFlds : ScFieldDefData2, new()
			where TRow : AShScRow2<TRowFlds>, new()
		{
			if (shtd == null )
			{
				M.WriteLineStatus("show sheet data| fail - no data to show");

				return;
			}

			M.WriteLine($"for sheet| {shtd.SchemaName}");
			
			ShowSheetDataGeneric2(shtd.Fields);

			if (shtd.Rows.Count > 0)
			{
				foreach (KeyValuePair<string, TRow> kvp in shtd.Rows)
				{
					M.WriteLine($"\n\nfor row| {kvp.Value.SchemaName}");

					ShowRowDataGeneric2(kvp.Value.Fields);
				}
			}
			else
			{
				M.WriteLine("\nNO ROW data\n");
			}
		}

		public void ShowSheetDataGeneric2<TShtFlds>
			(Dictionary<KEY, TShtFlds> fields)
			where TShtFlds : IShScFieldData2, new()

		{
			ScValues2 values = new ScValues2();
			values.setDataValues2(fields);

			M.WriteLine("\nshow SHEET data| start\n");

			ShowDataHeader(shFd.ScDataColOrder);

			sl2.WriteRows2(
				shFd.ScDataColOrder,
				shFd.ScFieldsColDefinitions,
				shFd.SchemaSheetKeyOrder2,
				values.ScDataValues,
				10, JustifyVertical.TOP, false, false);
		}

		public void ShowRowDataGeneric2<TRowFlds>
			(Dictionary<KEY, TRowFlds> fields)
			where TRowFlds : IShScFieldData2, new()

		{
			ScValues2 values = new ScValues2();
			values.setDataValues2(fields);

			M.WriteLine("\nROW data\n");

			ShowDataHeader(shFd.ScDataColOrder);

			sl2.WriteRows2(
				shFd.ScDataColOrder,
				shFd.ScFieldsColDefinitions,
				shFd.SchemaRowKeyOrder2,
				values.ScDataValues,
				10, JustifyVertical.TOP, false, false);
		}


	#endregion

	}
}