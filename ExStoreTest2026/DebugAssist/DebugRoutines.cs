using System.Diagnostics;
using System.Windows.Media.Animation;
using Autodesk.Revit.DB;
using ExStorSys;
using RevitLibrary;
using static ExStorSys.ExStorConst;


namespace ExStoreTest2026.DebugAssist
{
	public static class DebugRoutines
	{
		public static void ShowObjectId(int cmdId, int mainWinId, 
			int winModelId, int mgrId, int libId, int dataId, int wbkId)
		{
			Msgs.WriteLineSpaced("\n*** object id's ***\n");

			Msgs.WriteLineSpaced($"command id", cmdId.ToString());
			Msgs.WriteLineSpaced($"window main id", mainWinId.ToString());
			Msgs.WriteLineSpaced($"window model id", winModelId.ToString());
			Msgs.WriteLineSpaced($"ex mgr id", mgrId.ToString());
			Msgs.WriteLineSpaced($"lib id", libId.ToString());
			Msgs.WriteLineSpaced($"data id", dataId.ToString());
			Msgs.WriteLineSpaced($"wbk id", wbkId.ToString());
		}

		public static void ShowR()
		{
			Msgs.Col1width = 32;

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
			Msgs.Col1width = 32;

			Msgs.WriteLineSpaced("\n*** storage and name constants ***\n");
			Msgs.WriteLineSpaced($"company id", ExStorConst.CompanyId);
			Msgs.WriteLineSpaced($"vendor id", ExStorConst.VendorId);
			Msgs.WriteLineSpaced($"user name", ExStorConst.UserName);
			
			Msgs.NewLine();

			Msgs.WriteLineSpaced($"app code", APP_CODE);
			Msgs.WriteLineSpaced($"stor ver", EXS_VERSION);
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
			
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"created model code", CreateModelCode());

		}

		public static void ShowWorkBookFields()
		{
			Msgs.Col1width = 32;

			Msgs.WriteLineSpaced("\n*** workbook fields ***\n");

			foreach ((WorkBookFieldKeys key, FieldDef<WorkBookFieldKeys>? value) in Fields.WorkBookFields)
			{
				Msgs.WriteLineSpaced($"field {key}", $"{value.FieldName, -15} | desc {value.FieldDesc}");
			}
		}

		public static void ShowSheetFields()
		{
			Msgs.Col1width = 32;

			Msgs.WriteLineSpaced("\n*** sheet fields ***\n");

			foreach ((SheetFieldKeys key, FieldDef<SheetFieldKeys>? value) in Fields.SheetFields)
			{
				Msgs.WriteLineSpaced($"field {key}", $"{value.FieldName, -15} | desc {value.FieldDesc}");
			}
		}

		public static void ShowExid()
		{
			Exid ex = ExStorMgr.Instance.Exid;

			Msgs.WriteLineSpaced("\n*** exid properties ***\n");

			Msgs.WriteLine($"model names");
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"*** document name ***", ex.ModelName);
			Msgs.WriteLineSpaced($"doc name", ex.Model_Name);

			Msgs.NewLine();
			Msgs.WriteLine($"DS names");
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"create - as in if making new");
			Msgs.WriteLineSpaced($"create wbk ds name", ex.CreateWbkDsName(ExStorConst.CreateModelCode()));
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
			Msgs.WriteLineSpaced($"sht DS search name (specific)", ex.ShtDsSearchNameModelSpecific);



		}

		public static void ShowExMgr()
		{
			ExStorMgr xMgr = ExStorMgr.Instance;

			Msgs.WriteLineSpaced("\n*** ExStorMgr properties ***\n");

			Msgs.WriteLineSpaced($"model code", xMgr.TempModelCode);
			// Msgs.WriteLineSpaced($"wbk ds name", xMgr.WorkBookDsName);
			Msgs.WriteLineSpaced($"got workbook", xMgr.GotWorkBook.ToString());
			Msgs.WriteLineSpaced($"got sheets", xMgr.GotSheets.ToString());
			Msgs.WriteLineSpaced($"got wbk ds", xMgr.GotWbkDs.ToString());
			Msgs.WriteLineSpaced($"workbook is empty", xMgr.IsWorkBookEmpty.ToString());


		}

		public static void ShowWorkBook()
		{
			Msgs.WriteLineSpaced("\n*** workbook values ***\n");

			WorkBook wbk = ExStorMgr.Instance.WorkBook;

			Msgs.Col1width = 32;

			if (!ExStorMgr.Instance.GotWbkDs)
			{
				Msgs.WriteLineSpaced("*** workbook is null or invalid ***\n");
				return;
			}

			foreach ((WorkBookFieldKeys key, FieldData<WorkBookFieldKeys> value) in wbk.Rows)
			{
				Msgs.WriteLineSpaced($"{key}", $"{value.Field.FieldName,-20} | {(value.DyValue?.Value.ToString() ?? "null"),-40} | {value.DyValue.TypeIs, -20}  [{value.DyValue.RevitTypeIs}]");
			}

			string dsName = wbk.ExsDataStorage.Name;
			string scName = wbk.GotSchema ? wbk.ExsSchema.SchemaName : "invalid";
			string scDoc = wbk.GotSchema ? wbk.ExsSchema.Documentation : "invalid";

			Msgs.NewLine();
			Msgs.WriteLineSpaced($"\tstatus", wbk.CreationStatus.ToString());
			Msgs.WriteLineSpaced($"\tmodel code", wbk.ModelCode);
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"\tgot ds", wbk.GotDs.ToString());
			Msgs.WriteLineSpaced($"\tex ds name", dsName);
			Msgs.WriteLineSpaced($"\tds name", wbk.DsName);
			Msgs.WriteLineSpaced($"\tdesc", wbk.Desc);
			Msgs.WriteLineSpaced($"\tds search name", wbk.DsSearchName);
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"\tgot entity", wbk.GotEntity.ToString());
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"\tgot schema", wbk.GotSchema.ToString());
			Msgs.WriteLineSpaced($"\tex schema name", scName);
			Msgs.WriteLineSpaced($"\tex documentation", scDoc);
			Msgs.WriteLineSpaced($"\tschema name", wbk.SchemaName);
			Msgs.WriteLineSpaced($"\tschema desc", wbk.SchemaDesc);
			Msgs.WriteLineSpaced($"\tschema guid", wbk.SchemaGuid.ToString());

		}

		public static void ShowSheets()
		{
			Exid ex = ExStorMgr.Instance.Exid;

			Msgs.Col1width = 32;

			Msgs.WriteLineSpaced("\n*** exid sheets list values ***\n");

			if (ExStorMgr.Instance.Sheets.Count == 0)
			{
				Msgs.WriteLineSpaced("*** no sheets ***\n");
				return;
			}

			int count = 0;

			foreach ((string? k, Sheet? sht) in ExStorMgr.Instance.Sheets)
			{
				Msgs.WriteLineSpaced($"*** sheet {count} ***\n");

				if (ExStorMgr.Instance.GotShtDs(k))
				{
					ShowSheet(sht);
				}
				else
				{
					Msgs.WriteLineSpaced("*** sheet is null or invalid ***\n");
				}
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


		/* private */

		public static void ShowSheet(Sheet? sht)
		{
			Msgs.Col1width = 32;

			foreach ((SheetFieldKeys key, FieldData<SheetFieldKeys> value) in sht.Rows)
			{
				Msgs.WriteLineSpaced($"{key}", $"{value.Field.FieldName,-20} | {(value.DyValue?.Value?.ToString() ?? "null"),-40} | {value.DyValue?.TypeIs, -20}  [{value.DyValue?.RevitTypeIs}]");

				if (key == SheetFieldKeys.RK_RD_FAMILY_LIST)
				{
					IList<string>? famsAndTypes = (IList<string>) value.DyValue.Value;

					if (famsAndTypes != null)
					{
						Msgs.WriteLineSpaced($"\tlist count > {famsAndTypes.Count.ToString()}");

						foreach (string ft in famsAndTypes)
						{
							int pos1 = ft.IndexOf('|');

							if (pos1 >= 0)
							{
								Msgs.WriteLineSpaced($"\t> {ft.Substring(0, pos1)}<  >{ft.Substring(pos1 + 1)}<");
							}
							else
							{
								Msgs.WriteLineSpaced($"\t> {ft}");
							}
						}
					}
					else
					{
						Msgs.WriteLineSpaced($"\tlist count > is null");
					}
				}
			}

			string dsName = sht.ExsDataStorage.Name;
			string scName = sht.GotSchema ? sht.ExsSchema.SchemaName : "invalid";
			string scDoc =  sht.GotSchema ? sht.ExsSchema.Documentation : "invalid";

			Msgs.NewLine();

			Msgs.WriteLineSpaced($"\tstatus", sht.CreationStatus.ToString());
			Msgs.NewLine();

			Msgs.WriteLineSpaced($"\tex data storage", dsName);
			Msgs.WriteLineSpaced($"\tds name", sht.DsName);
			Msgs.WriteLineSpaced($"\tdesc", sht.Desc);
			Msgs.WriteLineSpaced($"\tsht search name", sht.DsSearchName);
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"\tgot entity", sht.GotEntity.ToString());
			Msgs.NewLine();
			Msgs.WriteLineSpaced($"\tgot schema", sht.GotSchema.ToString());
			Msgs.WriteLineSpaced($"\tex schema name", scName);
			Msgs.WriteLineSpaced($"\tex documentation", scDoc);
			Msgs.WriteLineSpaced($"\tschema name", sht.SchemaName);
			Msgs.WriteLineSpaced($"\tschema desc", sht.SchemaDesc);
			Msgs.WriteLineSpaced($"\tschema guid", sht.SchemaGuid.ToString());
		}

	}

}
