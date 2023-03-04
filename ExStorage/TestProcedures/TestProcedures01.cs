// Solution:     ExStorage
// Project:       ExStorage
// File:             TestProcedures01.cs
// Created:      2022-01-16 (9:41 PM)

using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStorage.Windows;
using RevitSupport;
using SettingsManager;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaRowKey;
using static ShExStorageN.ShSchemaFields.ShScSupport.CellUpdateRules;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ShScSupport;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields.ShScSupport;
using ShExStorageR.ShExStorage;
using ShStudyN.ShEval;

namespace ExStorage.TestProcedures
{
	public class TestProcedures01
	{
		public const string DS_ROWS_SHEET = "Rows│ Sheet Data Storage";
		public const string DS_ROWS_AP = "Rows│ App Data Storage";
		public const string DS_PRO_CS = "pro_cyberstudio_HasDataStorage";


		private int famIdx;

		public List<string[]> FauxFamNames { get; set; }  =
			new List<string[]>
			{
				new [] { "FamA1", "FamA2", "FamA3" },
				new [] { "FamB1", "FamB2", "FamB3" },
				new [] { "FamC1", "FamC2", "FamC3" },
				new [] { "FamD1", "FamD2", "FamD3" },
				new [] { "FamE1", "FamE2", "FamE3" },
			};

		private string[] a = new [] { "test" };

		private ShDebugMessages M { get; set; }

		// private ShDebugSupport D { get; set; }

		private ShowsProcedures01 sp01;

		public TestProcedures01()
		{
			// this.D = d;
			M = MainWindow.M;

			sp01 = new ShowsProcedures01();
		}

	#region general test routines

		// sub-tests

		// private void TestGetDsByName3(ShExStorageLibR shExLib, string name)
		// {
		// 	ExStoreRtnCode result;
		//
		// 	ExId exid = new ExId(true, name);
		//
		// 	exid.OverrideDsName(name);
		//
		// 	DataStorage ds;
		//
		// 	result = shExLib.FindDs(name, out ds);
		//
		// 	if (result != ExStoreRtnCode.XRC_GOOD)
		// 	{
		// 		M.WriteLine($"GetDsByName| not good| {result.ToString()}", $"looking for name| {name}");
		// 		return;
		// 	}
		//
		// 	M.WriteLine("\nGetDsByName| ds found");
		// 	sp01.ShowDsDetails(ds);
		// }

		// private bool TestDeleteDsByName4(ShExStorageLibR shExLib, string name)
		// {
		// 	ExId1 exid = ExId1.GetInstance(RvtCommand.RvtDoc);
		// 	exid.ExsId = name;
		//
		// 	ExStoreRtnCode result = shExLib.DelSheetDs(exid.Document, exid.ExsIdSheetDsName);
		//
		// 	return result == ExStoreRtnCode.XRC_GOOD;
		// }

		// private void TestGetDsEntityBySchema5(ShExStorageLibR shExLib, string name)
		// {
		// 	ExStoreRtnCode result;
		// 	ExId1 exid = ExId1.GetInstance(RvtCommand.RvtDoc);
		// 	exid.ExsId = name;
		//
		// 	Schema sc;
		//
		// 	result = shExLib.FindSchema(exid.ExsIdSheetDsName, out sc);
		//
		// 	if (result == ExStoreRtnCode.XRC_GOOD)
		// 	{
		// 		M.WriteLine("get schema| good");
		//
		// 		TestGetDsEntityBySchema5(shExLib, name, sc);
		// 	}
		// 	else
		// 	{
		// 		M.WriteLine("get schema| failed");
		// 	}
		// }

		// private void TestGetDsEntityBySchema5(ShExStorageLibR shExLib, string name, Schema sc)
		// {
		// 	ExStoreRtnCode result;
		// 	ExId1 exid = ExId1.GetInstance(RvtCommand.RvtDoc);
		// 	exid.ExsId = name;
		// 	Entity e = null;
		//
		// 	DataStorage ds;
		//
		// 	result = shExLib.FindDs(name, out ds);
		//
		// 	if (result == ExStoreRtnCode.XRC_GOOD)
		// 	{
		// 		M.WriteLine($"datastore found| {ds.Name}");
		//
		// 		result = shExLib.GetDsEntity(ds, sc, out e);
		// 	}
		// 	else
		// 	{
		// 		M.WriteLine($"datastore not found|");
		// 	}
		// }

		// private void TestGetDsEntityByName5(ShExStorageLibR shExLib, string name)
		// {
		// 	ExStoreRtnCode result;
		//
		// 	Schema sc;
		// 	ExId1 exid = ExId1.GetInstance(RvtCommand.RvtDoc);
		// 	exid.ExsId = name;
		// 	Entity e = null;
		//
		// 	result = shExLib.FindSchema(exid.ExsIdSheetDsName, out sc);
		//
		// 	if (result == ExStoreRtnCode.XRC_GOOD)
		// 	{
		// 		M.WriteLine($"schema found| {sc.GUID.ToString()}");
		//
		// 		DataStorage ds;
		//
		// 		result = shExLib.FindDs(name, out ds);
		//
		// 		if (result == ExStoreRtnCode.XRC_GOOD)
		// 		{
		// 			result = shExLib.GetDsEntity(ds, sc, out e);
		// 		}
		// 	}
		//
		// 	if (result == ExStoreRtnCode.XRC_GOOD)
		// 	{
		// 		sp01.ShowEntityDetails(e);
		// 	}
		// 	else
		// 	{
		// 		M.WriteLine("get schema| failed");
		// 		M.WriteLine("get entity| failed");
		// 	}
		// }


		// tests

		public void TestGetAllDs1(ShExStorageLibR shExLib)
		{
			ExStoreRtnCode result;

			result = shExLib.GetAllDs();

			if (result != ExStoreRtnCode.XRC_GOOD)
			{
				M.WriteLineAligned($"GetAllDataStores returned|", result.ToString());
				// M.ShowMsg();
				return;
			}

			sp01.ShowAllDs1(shExLib);

			// M.ShowMsg();
		}

		// public void TestFindDsByName3(ShExStorageLibR shExLib)
		// {
		// 	TestGetDsByName3(shExLib, DS_ROWS_SHEET);
		// 	TestGetDsByName3(shExLib, DS_ROWS_AP);
		// 	TestGetDsByName3(shExLib, DS_PRO_CS);
		//
		// 	// M.ShowMsg();
		// }

		// public void TestDeleteDsByName4(ShExStorageLibR shExLib)
		// {
		// 	bool result = TestDeleteDsByName4(shExLib, DS_PRO_CS);
		//
		// 	if (result)
		// 	{
		// 		M.WriteLine("delete by name| worked");
		// 	}
		// 	else
		// 	{
		// 		M.WriteLine("delete by name| failed");
		// 	}
		//
		// 	// M.ShowMsg();
		// }

		// public void TestFindExistSchema2(ShExStorageLibR shExLib)
		// {
		// 	ExStoreRtnCode result;
		//
		// 	Schema sc;
		// 	ExId1 exid = ExId1.GetInstance(RvtCommand.RvtDoc);
		//
		// 	result = shExLib.FindSchema(exid.ExsIdSheetSchemaName, out sc);
		// 	sp01.ShowSchema2(result, sc, exid.ExsIdSheetSchemaName);
		//
		// 	exid.ExsId = DS_ROWS_SHEET;
		// 	result = shExLib.FindSchema(exid.ExsId, out sc);
		// 	sp01.ShowSchema2(result, sc, DS_ROWS_SHEET);
		//
		// 	exid.ExsId = DS_ROWS_AP;
		// 	result = shExLib.FindSchema(exid.ExsId, out sc);
		// 	sp01.ShowSchema2(result, sc, DS_ROWS_AP);
		//
		// 	exid.ExsId = DS_PRO_CS;
		// 	result = shExLib.FindSchema(exid.ExsId, out sc);
		// 	sp01.ShowSchema2(result, sc, DS_PRO_CS);
		// }

		// public void TestGetDsEntity5(ShExStorageLibR shExLib)
		// {
		// 	M.WriteLine($"get entity by name");
		//
		// 	M.NewLine();
		//
		// 	M.WriteLine($"get entity for Ds named| {DS_PRO_CS}");
		// 	TestGetDsEntityByName5(shExLib, DS_PRO_CS);
		//
		// 	M.NewLine();
		//
		// 	M.WriteLine($"get entity for Ds named| {DS_ROWS_SHEET}");
		// 	TestGetDsEntityByName5(shExLib, DS_ROWS_SHEET);
		//
		// 	M.NewLine();
		//
		// 	M.WriteLine($"get entity by schema");
		//
		// 	M.NewLine();
		//
		// 	M.WriteLine($"get entity for Ds named| {DS_PRO_CS}");
		// 	TestGetDsEntityBySchema5(shExLib, DS_PRO_CS);
		//
		// 	M.NewLine();
		//
		// 	M.WriteLine($"get entity for Ds named| {DS_ROWS_SHEET}");
		// 	TestGetDsEntityBySchema5(shExLib, DS_ROWS_SHEET);
		//
		//
		// 	// M.ShowMsg();
		// }

		// public void TestGetEntityData6(ShExStorageLibR shExLib)
		// {
		// 	ExStoreRtnCode result;
		//
		// 	ExId1 exid = ExId1.GetInstance(RvtCommand.RvtDoc);
		// 	exid.ExsId = DS_PRO_CS;
		//
		// 	DataStorage ds;
		// 	Schema sc;
		// 	Entity e = null;
		//
		// 	result = shExLib.FindDs(exid.ExsIdSheetDsName, out ds);
		// 	result = shExLib.FindSchema(exid.ExsIdSheetDsName, out sc);
		// 	result = shExLib.GetDsEntity(ds, sc, out e);
		//
		// 	sp01.ShowEntityData6(shExLib, e, sc);
		//
		// 	// M.ShowMsg();
		// }

		// public void TestDoesDsExist7(ShExStorageLibR shExLib)
		// {
		// 	M.NewLine();
		//
		// 	ExStoreRtnCode result;
		//
		// 	DataStorage ds;
		//
		// 	ExId1 exid = ExId1.GetInstance(RvtCommand.RvtDoc);
		// 	exid.ExsId = DS_PRO_CS;
		//
		// 	M.WriteLineAligned("does ds exist?|", $"checking| {exid.ExsId}");
		//
		// 	result = shExLib.FindDs(exid.ExsIdSheetDsName, out ds);
		//
		// 	M.WriteLineAligned("does ds exist?|", $"{result == ExStoreRtnCode.XRC_GOOD}");
		// 	M.NewLine();
		//
		//
		// 	exid.ExsId = "Bogus_Name";
		//
		// 	M.WriteLineAligned("does ds exist?|", $"checking| {exid.ExsId}");
		//
		// 	result = shExLib.FindDs(exid.ExsIdSheetDsName, out ds);
		//
		// 	M.WriteLineAligned("does ds exist?|", $"{result == ExStoreRtnCode.XRC_GOOD}");
		// 	M.NewLine();
		//
		// 	// M.ShowMsg();
		// }

		private static int fauxRowIdx = 0;

		public void MakeFauxRow(ShtExId exid, ScDataRow rowd)
		{
			famIdx = UserSettings.Data.UserSettingsValue;

			M.WriteLineStatus($"faux row data| fam idx| {famIdx}");

			if (fauxRowIdx > 2) fauxRowIdx = 0;
			// string fauxIdx = (fauxRowIdx++ * 11).ToString("000");
			string fauxWksPath = Path.GetTempPath();
			// string fauxWksName = $"{fauxIdx}_{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}";
			// string fauxFamName = $"{fauxIdx}_{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}";
			string fauxFamName = FauxFamNames[famIdx][fauxRowIdx++];
			string fauxSeq = $"x1";

			// row data initially created.  finish with bogus information
			rowd.SetValue(RK0_SCHEMA_NAME, exid.RowSchemaName(fauxFamName));
			rowd.SetValue(RK2_CELL_FAMILY_NAME , fauxFamName);
			rowd.SetValue(RK2_XL_FILE_PATH     , fauxWksPath);
			rowd.SetValue(RK2_XL_WORKSHEET_NAME, "ExcelFileName.xls");
			rowd.SetValue(RK2_SEQUENCE         , "A1");
			rowd.SetValue(RK2_SKIP             , false);
			rowd.SetValue(RK2_UPDATE_RULE      , UR_UPON_REQUEST);
		}


		// public void MakeFauxRow2(ShtExId exid, ScDataRow2 rowd)
		// {
		// 	famIdx = UserSettings.Data.UserSettingsValue;
		//
		// 	M.WriteLineStatus($"faux row data| fam idx| {famIdx}");
		//
		// 	if (fauxRowIdx > 2) fauxRowIdx = 0;
		// 	// string fauxIdx = (fauxRowIdx++ * 11).ToString("000");
		// 	string fauxWksPath = Path.GetTempPath();
		// 	// string fauxWksName = $"{fauxIdx}_{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}";
		// 	// string fauxFamName = $"{fauxIdx}_{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}";
		// 	string fauxFamName = FauxFamNames[famIdx][fauxRowIdx++];
		// 	string fauxSeq = $"x1";
		//
		// 	// row data initially created.  finish with bogus information
		// 	rowd.Fields[(rKey) ShExNTblKeys.TK_SCHEMA_NAME].SetValue = exid.RowSchemaName(fauxFamName);
		// 	rowd.Fields[ScRowKeys.RK_CELL_FAMILY_NAME].SetValue = fauxFamName;
		// 	rowd.Fields[ScRowKeys.RK_XL_FILE_PATH].SetValue = fauxWksPath;
		// 	rowd.Fields[ScRowKeys.RK_XL_WORKSHEET_NAME].SetValue = "ExcelFileName.xls";
		// 	rowd.Fields[ScRowKeys.RK_SEQUENCE].SetValue = "A1";
		// 	rowd.Fields[ScRowKeys.RK_SKIP].SetValue = false;
		// 	rowd.Fields[ScRowKeys.RK_UPDATE_RULE].SetValue = UR_UPON_REQUEST;
		// }


	#endregion
	}
}