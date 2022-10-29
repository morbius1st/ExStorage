// Solution:     ExStorage
// Project:       ExStorage
// File:             TestProcedures01.cs
// Created:      2022-01-16 (9:41 PM)

using Autodesk.Revit.DB.ExtensibleStorage;
using ExStorage.Windows;
using SharedApp.Windows.ShSupport;
using ShExStorageN.ShExStorage;
using ShExStorageR.ShExStorage;
using ShStudy.ShEval;

namespace ExStorage.TestProcedures
{
	public class TestProcedures01
	{
		public const string DS_ROWS_TABLE = "Rows│ Table Data Storage";
		public const string DS_ROWS_AP = "Rows│ App Data Storage";
		public const string DS_PRO_CS = "pro_cyberstudio_HasDataStorage";

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

		private void TestGetDsByName3(ExStorageLibraryR exLib, string name)
		{
			ExStoreRtnCode result;

			ExId exid = new ExId("name");
			exid.ExsId = name;

			DataStorage ds;

			result = exLib.GetDs(exid, out ds);

			if (result != ExStoreRtnCode.XRC_GOOD)
			{
				M.WriteLine($"GetDsByName| not good| {result.ToString()}", $"looking for name| {name}");
				return;
			}

			M.WriteLine("\nGetDsByName| ds found");
			sp01.ShowDsDetails(ds);
		}

		private bool TestDeleteDsByName4(ExStorageLibraryR exLib, string name)
		{
			ExId exid = new ExId("name");
			exid.ExsId = name;

			ExStoreRtnCode result = exLib.DelDs(exid);

			return result == ExStoreRtnCode.XRC_GOOD;
		}

		private void TestGetDsEntityBySchema5(ExStorageLibraryR exLib, string name)
		{
			ExStoreRtnCode result;
			ExId exid = new ExId("name");
			exid.ExsId = name;

			Schema sc;

			result = exLib.GetSchema(exid, out sc);

			if (result == ExStoreRtnCode.XRC_GOOD)
			{
				M.WriteLine("get schema| good");

				TestGetDsEntityBySchema5(exLib, name, sc);
			}
			else
			{
				M.WriteLine("get schema| failed");
			}
		}

		private void TestGetDsEntityBySchema5(ExStorageLibraryR exLib, string name, Schema sc)
		{
			ExStoreRtnCode result;
			ExId exid = new ExId("name");
			exid.ExsId = name;
			Entity e = null;

			DataStorage ds;

			result = exLib.GetDs(exid, out ds);

			if (result == ExStoreRtnCode.XRC_GOOD)
			{
				M.WriteLine($"datastore found| {ds.Name}");

				result = exLib.GetDsEntity(ds, sc, out e);
			}
			else
			{
				M.WriteLine($"datastore not found|");
			}
		}

		private void TestGetDsEntityByName5(ExStorageLibraryR exLib, string name)
		{
			ExStoreRtnCode result;

			Schema sc;
			ExId exid = new ExId("name");
			exid.ExsId = name;
			Entity e = null;

			result = exLib.GetSchema(exid, out sc);

			if (result == ExStoreRtnCode.XRC_GOOD)
			{
				M.WriteLine($"schema found| {sc.GUID.ToString()}");

				DataStorage ds;

				result = exLib.GetDs(exid, out ds);

				if (result == ExStoreRtnCode.XRC_GOOD)
				{
					result = exLib.GetDsEntity(ds, sc, out e);
				}
			}

			if (result == ExStoreRtnCode.XRC_GOOD)
			{
				sp01.ShowEntityDetails(e);
			}
			else
			{
				M.WriteLine("get schema| failed");
				M.WriteLine("get entity| failed");
			}
		}


		// tests

		public void TestGetAllDs1(ExStorageLibraryR exLib)
		{
			ExStoreRtnCode result;

			result = exLib.GetAllDs();

			if (result != ExStoreRtnCode.XRC_GOOD)
			{
				M.WriteLineAligned($"GetAllDataStores returned|", result.ToString());
				// M.ShowMsg();
				return;
			}

			sp01.ShowAllDs1(exLib);

			// M.ShowMsg();
		}

		public void TestFindDsByName3(ExStorageLibraryR exLib)
		{
			TestGetDsByName3(exLib, DS_ROWS_TABLE);
			TestGetDsByName3(exLib, DS_ROWS_AP);
			TestGetDsByName3(exLib, DS_PRO_CS);

			// M.ShowMsg();
		}

		public void TestDeleteDsByName4(ExStorageLibraryR exLib)
		{
			bool result = TestDeleteDsByName4(exLib, DS_PRO_CS);

			if (result)
			{
				M.WriteLine("delete by name| worked");
			}
			else
			{
				M.WriteLine("delete by name| failed");
			}

			// M.ShowMsg();
		}

		public void TestFindExistSchema2(ExStorageLibraryR exLib)
		{
			ExStoreRtnCode result;

			Schema sc;
			ExId exid = new ExId("name");

			exid.ExsId = DS_ROWS_TABLE;
			result = exLib.GetSchema(exid, out sc);
			sp01.ShowSchema2(result, sc);

			exid.ExsId = DS_ROWS_AP;
			result = exLib.GetSchema(exid, out sc);
			sp01.ShowSchema2(result, sc);

			exid.ExsId = DS_PRO_CS;
			result = exLib.GetSchema(exid, out sc);
			sp01.ShowSchema2(result, sc);
		}

		public void TestGetDsEntity5(ExStorageLibraryR exLib)
		{
			M.WriteLine($"get entity by name");

			M.NewLine();

			M.WriteLine($"get entity for Ds named| {DS_PRO_CS}");
			TestGetDsEntityByName5(exLib, DS_PRO_CS);

			M.NewLine();

			M.WriteLine($"get entity for Ds named| {DS_ROWS_TABLE}");
			TestGetDsEntityByName5(exLib, DS_ROWS_TABLE);

			M.NewLine();

			M.WriteLine($"get entity by schema");

			M.NewLine();

			M.WriteLine($"get entity for Ds named| {DS_PRO_CS}");
			TestGetDsEntityBySchema5(exLib, DS_PRO_CS);

			M.NewLine();

			M.WriteLine($"get entity for Ds named| {DS_ROWS_TABLE}");
			TestGetDsEntityBySchema5(exLib, DS_ROWS_TABLE);


			// M.ShowMsg();
		}

		public void TestGetEntityData6(ExStorageLibraryR exLib)
		{
			ExStoreRtnCode result;

			ExId exid = new ExId("name");
			exid.ExsId = DS_PRO_CS;

			DataStorage ds;
			Schema sc;
			Entity e = null;

			result = exLib.GetDs(exid, out ds);
			result = exLib.GetSchema(exid, out sc);
			result = exLib.GetDsEntity(ds, sc, out e);

			sp01.ShowEntityData6(exLib, e, sc);

			// M.ShowMsg();
		}

		public void TestDoesDsExist7(ExStorageLibraryR exLib)
		{
			M.NewLine();

			ExStoreRtnCode result;

			DataStorage ds;

			ExId exid = new ExId("name");
			exid.ExsId = DS_PRO_CS;

			M.WriteLineAligned("does ds exist?|", $"checking| {exid.ExsId}");

			result = exLib.GetDs(exid, out ds);

			M.WriteLineAligned("does ds exist?|", $"{result == ExStoreRtnCode.XRC_GOOD}");
			M.NewLine();


			exid.ExsId = "Bogus_Name";

			M.WriteLineAligned("does ds exist?|", $"checking| {exid.ExsId}");

			result = exLib.GetDs(exid, out ds);

			M.WriteLineAligned("does ds exist?|", $"{result == ExStoreRtnCode.XRC_GOOD}");
			M.NewLine();

			// M.ShowMsg();
		}

	#endregion
	}
}