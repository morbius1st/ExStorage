using System.Diagnostics;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStoreTest2026.Windows;
using ExStorSys;
using RevitLibrary;
using UtilityLibrary;
using static ExStorSys.ExStorConst;


namespace ExStoreTest2026.DebugAssist
{	
	
	public static class DebugRoutines
	{
		public static void TestDynaValueChanges()
		{
			DynaValue dv10 = new DynaValue(10.0);
			DynaValue dv20 = new DynaValue(20);

			bool sd = Msgs.ShowDebug;
			Msgs.ShowDebug = true;

			Msgs.Col1Width = 34;

			tstDyanValU(dv10, nameof(dv10));
			tstDyanValU(dv20, nameof(dv20));

			Msgs.ShowDebug = sd;
		}

		private static void tstDyanValU(DynaValue dv, string name)
		{
			dynamic dy = dv.Value;
			double d = dv.IsDouble ? dv.Value : -1.0;
			int i =	dv.IsInt ? dv.Value : -1;
			string? s = dv.AsString();



			Msgs.WriteLineSpaced($"{name} | original value", $"{dv.AsString()} | {dv.TypeIs.Name}");

			dynamic dvx = dv.Value + 10;

			dv.ChangeValue(dvx);

			Msgs.WriteLineSpaced($"{name}'s add 10", $"{dv.AsString()} | {dv.TypeIs.Name} | is modified? {dv.ChangeQty}");
			Msgs.WriteLineSpaced($"{name}'s prior value", $"{dv.PriorValue.ToString()} | is clean? {dv.IsClean} is dirty? {dv.IsDirty}");

			dv.UndoChange();

			Msgs.WriteLineSpaced($"{name} value un-done", $"{dv.AsString()} | {dv.TypeIs.Name} | is modified? {dv.ChangeQty}");
			Msgs.WriteLineSpaced($"{name}'s prior value", $"{dv.PriorValue.ToString()} | is clean? {dv.IsClean} is dirty? {dv.IsDirty}");

			dvx = dv.Value + 20;

			dv.ChangeValue(dvx);
			dv.ApplyChange();

			Msgs.WriteLineSpaced($"{name}'s add 20 / apply change", $"{dv.AsString()} | {dv.TypeIs.Name} | is modified? {dv.ChangeQty}");
			Msgs.WriteLineSpaced($"{name}'s prior value", $"{dv.PriorValue?.ToString() ?? "null"} | is clean? {dv.IsClean} is dirty? {dv.IsDirty}");


		}

		public static void ShowObjectId()
		{
			int i;

			Msgs.WriteLineSpaced("\n**** object id's ***");

			foreach ((string? key, List<int>? value) in ExStorStartMgr.Instance.ObjectIdList)
			{
				Msgs.Write($"{key,-24}");

				if (value.Count > 0)
				{
					
					// Msgs.Write($"\t");

					for (i = 0; i < value.Count - 1; i++)
					{
						Msgs.Write($"{value[i]}, ");
					}

					Msgs.WriteLine($"{value[i]}");
				}
				else
				{
					Msgs.WriteLine($"none");
				}
			}
		}

		public static void ShowObjectId(int mainWinId, int winModelId)
		{
			ExStorMgr? xm = ExStorMgr.Instance;

			string xlibOid  = "null 1";
			string xmgrOid  = "null 1";
			string xdataOid = "null 1";
			string wbkOid   = "null 1";
			string muiOid	= "null 1";
			string lmgrOid  = "null 1";
			string stmgrOid = "null 1";

			if (xm != null)
			{
				xmgrOid	= xm.ObjectId.ToString();
				xlibOid = xm.xLib.ObjectId.ToString();

				xdataOid = xm.xData.ObjectId.ToString();
				lmgrOid = ExStorLaunchMgr.Instance?.ObjectIdStr ?? "null 2";
				wbkOid = xm.xData.WorkBook.ObjectId.ToString();
				stmgrOid = ExStorStartMgr.Instance?.ObjectId.ToString() ?? "null 2";

				muiOid = MainWinModelUi.Instance?.ObjectId.ToString() ?? "null 2";
			}

			Msgs.WriteLineSpaced("\n**** object id's ***");

			Msgs.WriteLineSpaced("\n** static objects");
			
			Msgs.WriteLineSpaced("\nstatic / does not change");
			Msgs.WriteLineSpaced($"lib id", xlibOid);

			Msgs.WriteLineSpaced("\n** per model objects");

			Msgs.WriteLineSpaced("\nfixed / does not change between models");
			Msgs.WriteLineSpaced($"start mgr id", stmgrOid);
			Msgs.WriteLineSpaced($"ex mgr id", xmgrOid);
			Msgs.WriteLineSpaced($"data id", xdataOid);
			Msgs.WriteLineSpaced($"wbk id", wbkOid);
			Msgs.WriteLineSpaced($"window model ui id", muiOid);

			Msgs.WriteLineSpaced("\none time only");
			Msgs.WriteLineSpaced($"launch mgr id", lmgrOid);

			Msgs.WriteLineSpaced("\n** per window objects");

			Msgs.WriteLineSpaced("\nvariable / changes each window invocation");
			Msgs.WriteLineSpaced($"window main id", mainWinId.ToString());
			Msgs.WriteLineSpaced($"window model id", winModelId.ToString());


		}

		public static void ShowR()
		{
			Msgs.Col1Width = 32;

			Msgs.WriteLineSpaced("\n*** R constants ***\n");

			Msgs.WriteLineSpaced($"file name", R.FileName);
			Msgs.WriteLineSpaced($"file path", R.FilePath);
			Msgs.WriteLineSpaced($"title", R.RvtDoc.Title);
			Msgs.WriteLineSpaced($"got app", (R.RvtApp != null).ToString());
			Msgs.WriteLineSpaced($"got ui app", (R.RvtUiApp != null).ToString());
			Msgs.WriteLineSpaced($"got ui doc", (R.RvtUidoc != null).ToString());
		}

		public static void ShowConsts()
		{
			Msgs.Col1Width = 32;

			Msgs.WriteLineSpaced("\n*** storage and name constants ***\n");
			Msgs.WriteLineSpaced($"company id", ExStorConst.CompanyId);
			Msgs.WriteLineSpaced($"vendor id", ExStorConst.VendorId);
			Msgs.WriteLineSpaced($"user name", ExStorConst.UserName);
			
			Msgs.NewLine();

			Msgs.WriteLineSpaced($"app code", APP_CODE);
			Msgs.WriteLineSpaced($"stor ver", EXS_VERSION_WBK);
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"schema name code", EXS_SCHEMA_NAME_CODE);
			
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"wbk sch name", ExStorConst.WbkSchemaName);
			Msgs.WriteLineSpaced($"wkb name code", EXS_WKB_NAME_CODE);
			Msgs.WriteLineSpaced($"wbk ds name prefix", EXS_WBK_DS_NAME_PREFIX);
			Msgs.WriteLineSpaced($"wbk ds search name", EXS_WBK_NAME_SEARCH);
			
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"sht sch name", ExStorConst.ShtSchemaName);
			Msgs.WriteLineSpaced($"sht name code", EXS_SHT_NAME_CODE);
			Msgs.WriteLineSpaced($"sht ds search name", EXS_SHT_NAME_SEARCH);
			Msgs.WriteLineSpaced($"sht first ds name", EXS_SHT_FIRST_ID_CODE);
			Msgs.WriteLineSpaced($"sht ds name prefix first", EXS_SHT_DS_NAME_PREFIX_FIRST);
			
			// Msgs.NewLine();
			// Msgs.WriteLineSpaced($"created model code", CreateModelCode());

		}

		public static void ShowWorkBookFields()
		{
			Msgs.Col1Width = 32;

			Msgs.WriteLineSpaced("\n*** workbook fields ***\n");

			foreach ((WorkBookFieldKeys key, FieldDef<WorkBookFieldKeys>? value) in Fields.WorkBookFields)
			{
				Msgs.WriteLineSpaced($"field {key}", $"{value.FieldName, -15} | desc {value.FieldDesc}");
			}
		}

		public static void ShowSheetFields()
		{
			Msgs.Col1Width = 32;

			Msgs.WriteLineSpaced("\n*** sheet fields ***\n");

			foreach ((SheetFieldKeys key, FieldDef<SheetFieldKeys>? value) in Fields.SheetFields)
			{
				Msgs.WriteLineSpaced($"field {key}", $"{value.FieldName, -15} | desc {value.FieldDesc}");
			}
		}

		public static void ShowExid()
		{
			Exid ex = ExStorMgr.Instance.Exid;
			// string modelCode = ExStorConst.CreateModelCode();

			Msgs.WriteLineSpaced("\n*** exid properties ***\n");

			Msgs.WriteLine($"model names");
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"*** document name ***", ex.ModelTitle);
			Msgs.WriteLineSpaced($"doc name", ex.ModelName);

			Msgs.NewLine();
			Msgs.WriteLine($"DS names");
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"create - as in if making new");
			Msgs.WriteLineSpaced($"create wbk ds name", ex.CreateWbkDsName());
			Msgs.WriteLineSpaced($"create 1st sht ds name", ex.CreateFirstShtDsName());
			Msgs.WriteLineSpaced($"create sht ds name", ex.CreateShtDsName("AAAA"));

			Msgs.NewLine();
			Msgs.WriteLine($"schema names");
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"wbk schema name", ex.WbkSchemaName);
			Msgs.WriteLineSpaced($"sht schema name", ex.ShtSchemaName);

			Msgs.NewLine();
			Msgs.WriteLine($"search names");
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"wbk DS search name", ex.WbkSearchName);
			Msgs.WriteLineSpaced($"sht DS search name (general)", ex.ShtSearchName);
			// Msgs.WriteLineSpaced($"sht DS search name (specific)", ex.ShtSearchName);



		}

		public static void ShowExMgr()
		{
			ExStorMgr xMgr = ExStorMgr.Instance;

			Msgs.WriteLineSpaced("\n*** ExStorMgr properties ***\n");

			// Msgs.WriteLineSpaced($"model code", xMgr.xData.TempModelCode);
			// Msgs.WriteLineSpaced($"wbk ds name", xMgr.WorkBookDsName);
			Msgs.WriteLineSpaced($"got workbook", xMgr.xData.GotWorkBook.ToString());
			Msgs.WriteLineSpaced($"got sheets", xMgr.xData.GotAnySheets.ToString());
			Msgs.WriteLineSpaced($"got wbk ds", xMgr.xData.GotWbkDs.ToString());
			Msgs.WriteLineSpaced($"workbook is empty", xMgr.xData.IsWorkBookEmpty.ToString());


		}

		public static void ShowAWorkBook(WorkBook? wbk)
		{
			if (wbk == null) return;

			Msgs.WriteLineSpaced("\n*** workbook values ***\n");

			Msgs.Col1Width = 32;

			if (!ExStorMgr.Instance.xData.GotWbkDs)
			{
				Msgs.WriteLineSpaced("*** workbook is null or invalid ***\n");
				return;
			}		

			foreach ((WorkBookFieldKeys key, FieldData<WorkBookFieldKeys> value) in wbk)
			{
				Msgs.WriteSpaced($"*** {key}", $"{value.Field.FieldName,-20} | {(value.DyValue?.Value.ToString() ?? "null"),-34}");
				Msgs.WriteLine($" | chgd? {value.DyValue.IsChanged?.ToString() ?? "not enabled",-14} | clean? {value.DyValue.IsClean}");
				// Msgs.WriteLine($" | modified {value.DyValue.IsChanged?.ToString() ?? "not enabled",-14} | {value.DyValue.TypeIs.Name, -20}  [{value.DyValue.RevitTypeIs.Name}]");
			}

			showWorkBookInfo(wbk);
		}


		public static void ShowWorkBookFromMemory()
		{
			Msgs.Col1Width = 32;
			Msgs.WriteLineSpaced("*** workbook values ***\n");

			DynaValue? dv;

			WorkBook wbk = ExStorMgr.Instance!.xData.WorkBook;

			if (!wbk.GotDs)
			{
				Msgs.WriteLineSpaced("*** workbook ds is null or invalid ***\n");
				return;
			}

			foreach ((WorkBookFieldKeys key, FieldData<WorkBookFieldKeys> value) in wbk)
			{
				dv = ExStorLib.Instance.ReadFieldDyn(wbk.ExsEntity!, value.Field!);

				Msgs.WriteLineSpaced($"{key}", $"{value.Field.FieldName,-20} | {(dv?.ToString() ?? "null"),-40} | {dv.TypeIs, -20}  [{dv.RevitTypeIs}]");
			}

		}

		public static void ShowWorkBook()
		{
			Msgs.WriteLineSpaced("\n*** workbook values ***\n");

			WorkBook wbk = ExStorMgr.Instance.xData.WorkBook;

			Msgs.Col1Width = 32;

			if (!ExStorMgr.Instance.xData.GotWbkDs)
			{
				Msgs.WriteLineSpaced("*** workbook is null or invalid ***\n");
				return;
			}		

			foreach ((WorkBookFieldKeys key, FieldData<WorkBookFieldKeys> value) in wbk)
			{
				Msgs.WriteSpaced($"*** {key}", $"{value.Field.FieldName,-20} | {(value.DyValue?.Value.ToString() ?? "null"),-34}");
				Msgs.WriteLine($" | chgd? {value.DyValue.IsChanged?.ToString() ?? "not enabled",-14} | clean? {value.DyValue.IsClean}");
				// Msgs.WriteLine($" | modified {value.DyValue.IsChanged?.ToString() ?? "not enabled",-14} | {value.DyValue.TypeIs.Name, -20}  [{value.DyValue.RevitTypeIs.Name}]");
			}

			showWorkBookInfo(wbk);
		}

		public static void ShowSheetsFromMemory()
		{
			Msgs.Col1Width = 32;
			int count = 0;

			if (!ExStorMgr.Instance!.xData.GotAnySheets)
			{
				Msgs.WriteLineSpaced("\n*** NO sheets ***\n");
				return;
			}

			foreach ((string key, Sheet sht) in ExStorMgr.Instance.xData.Sheets)
			{
				//
				// foreach ((string? k, Sheet? sht) in ExStorMgr.Instance.xData.SheetsList)
				// {
				Msgs.WriteLineSpaced($"\n*** sheet {count} ***\n");
			
				if (ExStorMgr.Instance.xData.GotShtDs(sht.DsName))
				{
					ShowSheetFromMemory(sht);
				}
				else
				{
					Msgs.WriteLineSpaced("\n*** sheet is NULL or INVALID ***\n");
				}
			}
		}

		public static void ShowSheets()
		{
			Exid ex = ExStorMgr.Instance.Exid;
			
			Msgs.Col1Width = 32;
			
			Msgs.WriteLineSpaced("\n*** exid sheets list values ***\n");
			
			ShowSheets(ExStorMgr.Instance.xData.Sheets);


			// if (ExStorMgr.Instance.xData.Sheets.Count == 0)
			// {
			// 	Msgs.WriteLineSpaced("*** no sheets ***\n");
			// 	return;
			// }
			//
			// int count = 0;
			//
			// foreach ((string key, Sheet sht) in ExStorMgr.Instance.xData.Sheets)
			// {
			// 	// foreach ((string? k, Sheet? sht) in ExStorMgr.Instance.xData.SheetsList)
			// 	// {
			// 	Msgs.WriteLineSpaced($"*** sheet {count} ***\n");
			//
			// 	if (ExStorMgr.Instance.xData.GotShtDs(sht.DsName))
			// 	{
			// 		ShowSheet(sht);
			// 	}
			// 	else
			// 	{
			// 		Msgs.WriteLineSpaced("*** sheet is null or invalid ***\n");
			// 	}
			// }
		}

		public static void ShowSheets(ObservableDictionary<string, Sheet>? shts)
		{
			if (shts == null) return;

			if (shts.Count == 0)
			{
				Msgs.WriteLineSpaced("*** no sheets ***\n");
				return;
			}
			
			int count = 0;
			
			foreach ((string key, Sheet sht) in shts)
			{
				// foreach ((string? k, Sheet? sht) in ExStorMgr.Instance.xData.SheetsList)
				// {
				Msgs.WriteLineSpaced($"*** sheet {count} ***\n");
			
				if (ExStorMgr.Instance.xData.GotShtDs(sht.DsName))
				{
					ShowSheet(sht);
				}
				else
				{
					Msgs.WriteLineSpaced("*** sheet is null or invalid ***\n");
				}
			}
		}

		public static void ShowExData()
		{
			ExStorData xd = ExStorData.Instance;

			Msgs.WriteLineSpaced("\n*** start ExStorData properties ***\n");

			Msgs.WriteLineSpaced($"object id", xd.ObjectId);
			Msgs.WriteLineSpaced($"restart required", xd.RestartRequired);
			Msgs.NewLine();

			Msgs.WriteLineSpaced($"is wbk empty", xd.IsWorkBookEmpty);
			

			if (xd.GotWorkBook)
			{
				Msgs.WriteLineSpaced($"got workbook", xd.GotWorkBook);
				// Msgs.WriteLineSpaced($"workbook | ds name", $"{xd.WorkBook.DsName} | is empty {xd.WorkBook.IsEmpty} | is invalid {xd.WorkBook.IsInvalid}");
				Msgs.WriteLineSpaced($"workbook | ds name", $"{xd.WorkBook.DsName} | is empty {xd.WorkBook.IsEmpty}");
			}
			else
			{
				Msgs.WriteLineSpaced("don't got workbook");
			}
			
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"got any sheets", xd.GotAnySheets);
			
			if (xd.GotAnySheets)
			{
				Msgs.WriteLineSpaced($"sheets count", xd.Sheets.Count);

				foreach ((string key, Sheet sht) in xd.Sheets)
				{
					// foreach ((string? key, Sheet? sht) in xd.SheetsList)
					// {
					// Msgs.WriteLineSpaced($"\tsht | ds name", $"{sht.DsName} | is empty {sht.IsEmpty} | is invalid {sht.IsInvalid}");
					Msgs.WriteLineSpaced($"\tsht | ds name", $"{sht.DsName} | is empty {sht.IsEmpty}");
				}
			}

			Msgs.NewLine();

			ShowValues("", xd.GotWbkSchema, 
				"got sht schema?", xd.GotWbkSchema.ToString(),
				"sbk schema name", xd.WorkBookSchema.SchemaName,
				"don't got workbook schema"
				);

			ShowValues("", xd.GotShtSchema, 
				"got sht schema?", xd.GotShtSchema.ToString(),
				"sht schema name", xd.SheetSchema.SchemaName,
				"don't got sheet schema"
				);

			// Msgs.NewLine();
			// Msgs.WriteLineSpaced($"temp model code", xd.TempModelCode);

			Msgs.NewLine();

			Msgs.WriteLineSpaced($"got temp wbk ds"         , xd.GotTempWbkDs);
			Msgs.WriteLineSpaced($"got temp wbk dslist"     , xd.GotTempWbkDsList);
			Msgs.WriteLineSpaced($"got temp wbk schema"     , xd.GotTempWbkSchema);
			// Msgs.WriteLineSpaced($"got temp wbk schema list", xd.GotTempWbkSchemaList);
			Msgs.WriteLineSpaced($"got temp wbk entity"     , xd.GotTempWbkEntity);
			Msgs.WriteLineSpaced($"got temp sht ds"         , xd.GotTempShtDs);
			// Msgs.WriteLineSpaced($"got temp sht dslist"     , xd.GotTempAnySheets);
			Msgs.WriteLineSpaced($"got temp sht schema"     , xd.GotTempShtSchema);
			// Msgs.WriteLineSpaced($"got temp sht schema list", xd.GotTempShtSchemaList);
			// Msgs.WriteLineSpaced($"got temp sht entity"     , xd.GotTempShtEntity);

			Msgs.WriteLineSpaced("\n*** end ExStorData properties ***\n");
		}

		private static void ShowValues(string p1, bool b1, string s1a, string s1b, string s2a, string s2b, string s3, string p2 = null, string? s4 = null)
		{
			if (!s4.IsVoid())
			{
				Msgs.WriteLine($"{p2}{s4}");
			}

			if (b1)
			{
				Msgs.WriteLineSpaced($"{p1}{s1a}", $"{s1b}");
				Msgs.WriteLineSpaced($"{p1}{s2a}", $"{s2b}");


			}
			else
			{
				Msgs.WriteLine($"{p1}{s3}");
			}
		}

		public static void ShowParameterSet(ParameterSet? ps)
		{
			if (ps == null)
			{
				Msgs.WriteLine("\tparameter set is null");
			}

			Parameter? p;

			foreach (object o in ps)
			{
				p = o as Parameter;

				if (p != null)
				{
					Msgs.WriteLine($"\t{p.Definition.Name,-24} | {p.HasValue} | {p.AsValueString()} [ {p.AsString()} ]");
				}
			}
		}

		public static void FindAndShowExObjects()
		{
			ExStorMgr xMgr = ExStorMgr.Instance;

			string? name;
			string? mn;
			string? mc1;
			string? mc2;
			string temp;
			Guid? guid;

			Msgs.WriteLine("\n**** START - find and show all elements ****\n");

			Msgs.NewLine();
			Msgs.WriteLine($"\n\t** current model name {xMgr.Exid.ModelName} [{xMgr.Exid.ModelTitle}] **\n");

			Msgs.WriteLine("\tWBK Schema (local)");
			if (xMgr.xData.GotWbkSchema)
			{
				Msgs.WriteLine($"\t\t{xMgr.xData.WorkBookSchema!.SchemaName,-40} | valid {xMgr.xData.WorkBookSchema!.IsValidObject,-8} | {xMgr.xData.WorkBookSchema.GUID}");
				
			}
			else
			{
				Msgs.WriteLine("\t\t** got no WBK schema (local) **");
			}

			Msgs.NewLine();
			Msgs.WriteLine("\tWBK DataStorage (local)");
			if (xMgr.xData.GotWbkDs)
			{
				name = xMgr.xData.WorkBook!.ExsDataStorage!.Name;
				mn = xMgr.xData.WorkBook.ModelTitle;
				// mc1 = xMgr.xData.WorkBook.ModelCode;
				// mc2 = xMgr.xLib.ExtractModelCodeFromName(name, xMgr.Exid.WbkSearchName) ?? "is null";

				Msgs.WriteLine($"\t\t{xMgr.xData.WorkBook!.ExsDataStorage!.Name,-40} | valid {xMgr.xData.WorkBook!.ExsDataStorage!.IsValidObject,-8} | {mn,-12}");

				guid = xMgr.xData?.WorkBookSchema?.GUID ?? Guid.Empty;

				matchDsSchemaGuids(xMgr.xData.WorkBook!.ExsDataStorage, guid, ExStorConst.WbkSchemaGuid);

			}
			else
			{
				Msgs.WriteLine("\t\t** got no WBK datastorage (local) **");
			}

			Msgs.NewLine();
			Msgs.WriteLine("\tSHT Schema (local)");
			if (xMgr.xData.GotShtSchema)
			{
				Msgs.WriteLine($"\t\t{xMgr.xData.SheetSchema!.SchemaName,-40} | valid {xMgr.xData.SheetSchema!.IsValidObject,-8} | {xMgr.xData.SheetSchema.GUID}");
			}
			else
			{
				Msgs.WriteLine("\t\t** got no SHT schema (local) **");
			}

			Msgs.NewLine();
			Msgs.WriteLine("\tSHT DataStorage (local)");
			if (xMgr.xData.GotAnySheets)
			{
				// foreach ((string? key, Sheet? sht) in xMgr.xData.SheetsList)
				// {

				foreach ((string key, Sheet sht) in xMgr.xData.Sheets)
				{
					if (sht.ExsDataStorage != null && sht.ExsDataStorage.IsValidObject)
					{
						Msgs.NewLine();

						Msgs.WriteLine($"\t\t{sht.ExsDataStorage.Name,-40} | valid {xMgr.xData.WorkBook!.ExsDataStorage!.IsValidObject,-8}");

						guid = xMgr.xData?.SheetSchema?.GUID ?? Guid.Empty;

						matchDsSchemaGuids(sht.ExsDataStorage, guid, ExStorConst.ShtSchemaGuid);
					}
					else
					{
						Msgs.WriteLine($"\t\t** SHT datastorage {sht.DsName} is null (local) **");
					}
				}
			}
			else
			{
				Msgs.WriteLine("\t\t** got no SHT datastorage (local) **");
			}

			// above, what is currently saved on local data objects
			// below find actual information

			IList<Schema>? TempWbkSchemaList;

			Msgs.NewLine();
			Msgs.WriteLine("\tWBK Schema (live)");
			if (xMgr.FindAllWbkSchema(out TempWbkSchemaList))
			{
				foreach (Schema sc in TempWbkSchemaList)
				{

					Msgs.WriteLine($"\t\t{sc.SchemaName,-40} | valid {sc.IsValidObject,-8} | {sc.GUID}");
				}

				if (TempWbkSchemaList.Count == 1)
				{
					xMgr.xData.TempWbkSchemaEx = new ExListItem<Schema>( TempWbkSchemaList[0].SchemaName,  TempWbkSchemaList[0]);
				}
			}
			else
			{
				Msgs.WriteLine("\t\t** got no WBK schema **");
			}

			Msgs.NewLine();
			Msgs.WriteLine("\tWBK DataStorage (live)");

			IList<DataStorage>? TempWbkDsList;

			if (xMgr.FindAllWbkDs(out TempWbkDsList))
			{
				foreach (DataStorage ds in TempWbkDsList)
				{
					name = ds.Name;

					if (xMgr.xData.TempWbkSchemaEx != null)
					{
						mn = xMgr.ReadModelName(ds, xMgr.xData.TempWbkSchemaEx.Item) ?? "is null";
						// mc1 = xMgr.ReadModelCode(ds, xMgr.xData.TempWbkSchemaEx) ?? "is null";
						// mc2 =  xMgr.xLib.ExtractModelCodeFromName(name, xMgr.Exid.WbkSearchName) ?? "is null";
						temp = $"{mn,-12}";
					}
					else
					{
						temp = "cannot get info - no temp schema";
					}

					Msgs.WriteLine($"\t\t{ds.Name,-40} | valid {ds.IsValidObject,-8} | {temp}");

					guid = xMgr.xData?.TempWbkSchemaEx?.Item?.GUID ?? Guid.Empty;

					matchDsSchemaGuids(ds, guid, ExStorConst.WbkSchemaGuid);
				}
			}
			else
			{
				Msgs.WriteLine("\t\t** got no WBK datastorage **");
			}

			Msgs.NewLine();
			Msgs.WriteLine("\tSHT Schema (live)");

			IList<Schema>? TempShtSchemaList;

			if (xMgr.FindAllShtSchema(out TempShtSchemaList))
			{
				foreach (Schema sc in TempShtSchemaList)
				{
					Msgs.WriteLine($"\t\t{sc.SchemaName,-40} | valid {sc.IsValidObject,-8} | {sc.GUID}");
				}

				if (TempShtSchemaList.Count == 1)
				{
					xMgr.xData.TempShtSchemaEx = new ExListItem<Schema>(TempShtSchemaList[0].SchemaName, TempShtSchemaList[0]);
				}
			}
			else
			{
				Msgs.WriteLine("\t\t** got no SHT schema **");
			}

			Msgs.NewLine();
			Msgs.WriteLine("\tSHT DataStorage (live)");

			IList<DataStorage>? TempShtDsList;

			if (xMgr.FindAllShtDs(out TempShtDsList))
			{
				foreach (DataStorage ds in TempShtDsList)
				{
					Msgs.NewLine();
					Msgs.WriteLine($"\t\t{ds.Name,-40} | valid {ds.IsValidObject,-8}");

					guid = xMgr.xData?.TempShtSchemaEx?.Item.GUID ?? Guid.Empty;

					matchDsSchemaGuids(ds, guid, ExStorConst.ShtSchemaGuid);
				}
			}
			else
			{
				Msgs.WriteLine("\t\t** got no SHT datastorage **");
			}

			Msgs.NewLine();
			Msgs.WriteLine("\n**** END - find and show all elements ****\n");
		}

		/* private */

		private static void matchDsSchemaGuids(DataStorage? ds, Guid? tstGuid, Guid? constGuid)
		{
			bool match1;
			bool match2;

			if (ds == null || ds.GetEntitySchemaGuids().Count == 0)
			{
				Msgs.WriteLine($"\t\t\tno stored schema guids");
			}

			foreach (Guid g in ds.GetEntitySchemaGuids())
			{
				match1 = g.Equals(tstGuid);
				match2 = g.Equals(constGuid);

				Msgs.WriteLine($"\t\t\tmatch1? {match1} | match2? {match2} | {g}");
			}
		}

		public static void ShowSheet(Sheet? sht)
		{
			Msgs.Col1Width = 32;

			foreach ((SheetFieldKeys key, FieldData<SheetFieldKeys> value) in sht!)
			{
				if (key == SheetFieldKeys.RK_RD_FAMILY_LIST)
				{
					Msgs.WriteSpaced($"*** {key}", $"{value.Field.FieldName,-16} | {$"count| {(value.DyValue?.AsDictStringString().Count ?? -1)}",-34}");
					Msgs.WriteLine($" | modified {value.DyValue.IsChanged?.ToString() ?? "not enabled",-14} | clean? {value.DyValue.IsClean}");

					IDictionary<string, string>? famsAndTypes = (IDictionary<string, string>) value.DyValue.Value;

					showFamsAndTypes(famsAndTypes);
				}
				else
				{
					Msgs.WriteSpaced($"*** {key}", $"{value.Field.FieldName,-16} | {(value.DyValue?.Value?.ToString() ?? "null"),-34}");
					Msgs.WriteLine($" | modified {value.DyValue.IsChanged?.ToString() ?? "not enabled",-14} | clean? {value.DyValue.IsClean}");
					// Msgs.WriteLine($" | modified {value.DyValue.IsChanged?.ToString() ?? "not enabled",-14} | {value.DyValue?.TypeIs.Name, -30}  [{value.DyValue?.RevitTypeIs.Name}]");
				}
			}

			showSheetInfo(sht);
		}

		public static void ShowSheetFromMemory(Sheet? sht)
		{
			Msgs.Col1Width = 32;
			DynaValue? dv;

			foreach ((SheetFieldKeys key, FieldData<SheetFieldKeys> value) in sht!)
			{
				dv = ExStorLib.Instance.ReadFieldDyn(sht.ExsEntity!, value.Field!);

				if (key == SheetFieldKeys.RK_RD_FAMILY_LIST)
				{
					IDictionary<string, string>? famsAndTypes = (IDictionary<string, string>) dv.Value;

					showFamsAndTypes(famsAndTypes);
				}
				else
				{
					Msgs.WriteLineSpaced($"{key}", $"{value.Field.FieldName,-16} | {(dv?.Value?.ToString() ?? "null"),-34} | {dv?.TypeIs, -20}  [{dv?.RevitTypeIs}]");
				}
			}

			showSheetInfo(sht);
		}

		private static void showFamsAndTypes(IDictionary<string, string>? famsAndTypes)
		{
			if (famsAndTypes == null || famsAndTypes.Count == 0)
			{
				Msgs.WriteLineSpaced("\n*** got NO families / types");
				return;
			}

			Msgs.WriteLineSpaced($"\n** show fam types - START | list count > {famsAndTypes.Count.ToString()}");

			foreach ((string? key, string? value) in famsAndTypes)
			{
				Msgs.WriteLineSpaced($"\t>{key}<  >{value}<");
			}

			Msgs.WriteLineSpaced($"\n** show fam types - DONE\n");
		}

		private static void showWorkBookInfo(WorkBook wbk)
		{
			string dsName = wbk.ExsDataStorage!.Name;

			// Msgs.NewLine();
			// Msgs.WriteLineSpaced($"model code", wbk.ModelCode);
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"*** is empty", wbk.IsEmpty);
			Msgs.WriteLineSpaced($"*** got ds", wbk.GotDs.ToString());
			Msgs.WriteLineSpaced($"*** ex ds name", dsName);
			Msgs.WriteLineSpaced($"*** ds name", wbk.DsName);
			Msgs.WriteLineSpaced($"*** desc", wbk.Desc);
			Msgs.WriteLineSpaced($"*** ds search name", wbk.DsSearchName);
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"*** got entity", wbk.GotEntity.ToString());
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"*** schema name", wbk.SchemaName);
			Msgs.WriteLineSpaced($"*** schema desc", wbk.SchemaDesc);
			Msgs.WriteLineSpaced($"*** schema guid", wbk.SchemaGuid.ToString());
		}

		private static void showSheetInfo(Sheet sht)
		{
			Msgs.WriteLineSpaced($"\n** show sheet info - START\n");

			string dsName = sht.ExsDataStorage?.Name ?? "is null";

			Msgs.NewLine();

			Msgs.WriteLineSpaced($"*** is empty", sht.IsEmpty);
			Msgs.WriteLineSpaced($"*** ex data storage", dsName);
			Msgs.WriteLineSpaced($"*** ds name", sht.DsName);
			Msgs.WriteLineSpaced($"*** desc", sht.Desc);
			Msgs.WriteLineSpaced($"*** sht search name", sht.DsSearchName);
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"*** got entity", sht.GotEntity.ToString());
			Msgs.WriteLineSpaced($"*** got DS", sht.GotDs);
			Msgs.WriteLineSpaced($"*** is empty", sht.IsEmpty);
			// Msgs.WriteLineSpaced($"*** populated", sht.IsPopulated);

			Msgs.NewLine();
			Msgs.WriteLineSpaced($"*** schema name", sht.SchemaName);
			Msgs.WriteLineSpaced($"*** schema desc", sht.SchemaDesc);
			Msgs.WriteLineSpaced($"*** schema guid", sht.SchemaGuid.ToString());

			// Msgs.NewLine();
			// Msgs.WriteLineSpaced("text get values");
			// Msgs.WriteLineSpaced($"***  get vendor id", sht.GetVendorId);
			// Msgs.WriteLineSpaced($"***  get status", sht.GetStatus);
			// Msgs.WriteLineSpaced($"***  get sequence", sht.GetSequence);
			// Msgs.WriteLineSpaced($"***  get fam type row count", sht.GetFamilyTypeRowCount);

			Msgs.WriteLineSpaced($"\n** show sheet info - DONE\n");
		}

	}

}
