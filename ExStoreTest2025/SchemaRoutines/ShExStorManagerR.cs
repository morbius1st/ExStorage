using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStoreTest2025.Support;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ShScSupport;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaSheetKey;
using static ShExStorageN.ShExStorage.ExStoreRtnCode;


// user name: jeffs
// created:   11/18/2022 9:38:37 PM

namespace ShExStorageR.ShExStorage
{
	[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
	[Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
	public class ShExStorManagerR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds> : INotifyPropertyChanged
		where TSht : AShScSheet<TShtKey, TShtFlds, TRowKey, TRowFlds, TRow>, new()
		where TLok : AShScFields<TLokKey, TLokFlds>, new()
		where TShtKey : Enum
		where TShtFlds : IShScFieldData1<TShtKey>, new()
		where TRowKey : Enum
		where TRowFlds : IShScFieldData1<TRowKey>, new()
		where TRow : AShScRow<TRowKey, TRowFlds>, new()
		where TLokKey : Enum
		where TLokFlds : IShScFieldData1<TLokKey>, new()
	{
	#region fields

		private static readonly Lazy<ShExStorManagerR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>> instance =
			new Lazy<ShExStorManagerR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>>(() =>
				new ShExStorManagerR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>());

		public static ShExStorManagerR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds> Instance => instance.Value;

		private DataStorage dsSheet;

		private ExId exid;

		private TSht sheet;

		// private TSht sheet2;
		private TRow row;

		private Entity sheetEntity;

		// private bool hasData;
		private ExStoreRtnCode rtnCode;

	#endregion

	#region config

		private ShExStorManagerR()
		{
			config();
		}

		private void config()
		{
			SchemaLibR = new ShExSchemaLibR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>();
			StorLibR = new ShExStorageLibR();
		}

		public void Init(ExId exid)
		{
			Exid = exid;
			// HasData = false;

		}

	#endregion

	#region base

		public event PropertyChangedEventHandler PropertyChanged;

		[DebuggerStepThrough]
		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

		public override string ToString()
		{
			return $"this is {nameof(ShExStorManagerR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>)}";
		}

	#endregion

	#region public properties

		// schema creation and data saving methods
		public ShExSchemaLibR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds> SchemaLibR { get; set; }

		// data storage methods
		public ShExStorageLibR StorLibR { get; set; }

		public ExId Exid
		{
			get => exid;
			set
			{
				exid = value;
				OnPropertyChanged();
			}
		}

		public TSht Sheet
		{
			get => sheet;
			set
			{
				sheet = value;
				// HasData = true;
				OnPropertyChanged();
			}
		}

		// public TSht Sheet2
		// {
		// 	get => sheet2;
		// 	set => sheet2 = value;
		// }

		public TRow Row
		{
			get => row;
			set => row = value;
		}

		public TLok Lock { get; set; }


		// public DataStorage SheetDs
		// {
		// 	get => dsSheet;
		// 	set
		// 	{
		// 		dsSheet = value;
		// 		OnPropertyChanged();
		// 	}
		// }
		//
		// public Entity SheetEntity
		// {
		// 	get => sheetEntity;
		// 	set => sheetEntity = value;
		// }
		//
		// public Schema SheetSchema { get; set; }
		//
		// public Entity LockEntity { get; set; }
		// public Schema LockSchema { get; set; }

		public ExStoreRtnCode ReturnCode
		{
			get => rtnCode;
			private set
			{
				rtnCode = value;
				OnPropertyChanged();
			}
		}

		// public bool HasData
		// {
		// 	get => hasData;
		// 	set
		// 	{
		// 		hasData = value;
		// 		OnPropertyChanged();
		// 	}
		// }

	#endregion

	#region public methods

		public bool SetRtnCode(ExStoreRtnCode rtnCode, ExStoreRtnCode testCode = XRC_GOOD)
		{
			ReturnCode = rtnCode;
			OnPropertyChanged(nameof(ReturnCode));
			return rtnCode == testCode;
		}

		public ExStoreRtnCode FindSheetSchema()
		{
			ExStoreRtnCode rtnCode = XRC_SCHEMA_NOT_FOUND;
			IList<Schema> schemas;

			rtnCode = StorLibR.GetAllSchema(out schemas);

			if (rtnCode != ExStoreRtnCode.XRC_GOOD)
			{
				Msgs.WriteLine("Did not find schemas| rtn code is ", rtnCode.ToString());
				SetRtnCode(rtnCode);
				return rtnCode;
			}

			rtnCode = XRC_GOOD;

			foreach (Schema sch in schemas)
			{
				if (sch.SchemaName.StartsWith(Exid.BaseExsIdSheetSchemaName) ||
					sch.SchemaName.StartsWith(Exid.BaseExsIdSheetDsName) ||
					sch.SchemaName.StartsWith(Exid.BaseExIdRowSchemaName))
				{
					Msgs.WriteLine("Found a schema| ", sch.SchemaName);
				}
			}

			return rtnCode;
		}

		public ExStoreRtnCode FindSchemaByStartsWith(string startWith, out List<Schema> found)
		{
			ExStoreRtnCode rtnCode = XRC_SCHEMA_NOT_FOUND;
			IList<Schema> schemas;
			found = new List<Schema>();

			rtnCode = StorLibR.GetAllSchema(out schemas);

			if (rtnCode != ExStoreRtnCode.XRC_GOOD)
			{
				Msgs.WriteLine("Did not find schemas| rtn code is ", rtnCode.ToString());
			}
			else
			{
				foreach (Schema sch in schemas)
				{
					if (sch.SchemaName.StartsWith(startWith))
					{
						found.Add(sch);
					}
				}

				Msgs.WriteLine("Found schema| count ", found.Count.ToString());
			}

			rtnCode = found.Count > 0 ? XRC_GOOD : XRC_SCHEMA_NOT_FOUND;

			SetRtnCode(rtnCode);

			return rtnCode;
		}

		public ExStoreRtnCode DeleteSchema(string startWith)
		{
			List<Schema> found = new List<Schema>();

			ElementId el;

			ExStoreRtnCode rtnCode = FindSchemaByStartsWith(startWith, out found);

			if (rtnCode != ExStoreRtnCode.XRC_GOOD)
			{
				Msgs.WriteLine("Nothing to delete");
				SetRtnCode(rtnCode);
				return rtnCode;
			}

			using (Transaction T = new Transaction(Exid.Document, "Erase Cells Data"))
			{
				try
				{
					T.Start();
					{
						foreach (Schema sch in found)
						{
							Msgs.Write($"Erasing Schema| {sch.SchemaName}");
							StorLibR.DelSchema(Exid, sch);
							sch.Dispose();
							Msgs.WriteLine(" | done");
						}
					}
					T.Commit();
				}
				catch
				{
					Msgs.WriteLine(" | failed");
					T.RollBack();
				}
			}

			return rtnCode;
		}

		public ExStoreRtnCode EraseSheetSchema()
		{
			ExStoreRtnCode rtnCode;

			rtnCode = DeleteSchema(exid.BaseExsIdSheetSchemaName);

			if (rtnCode != ExStoreRtnCode.XRC_GOOD)
			{
				Msgs.WriteLine("Nothing to delete");
				SetRtnCode(rtnCode);
				return rtnCode;
			}

			rtnCode = DeleteSchema(exid.BaseExIdRowSchemaName);

			if (rtnCode != ExStoreRtnCode.XRC_GOOD)
			{
				Msgs.WriteLine("Nothing to delete");
				SetRtnCode(rtnCode);
				return rtnCode;
			}

			Exid.Document.Application.PurgeReleasedAPIObjects();

			return rtnCode;
		}

		public ExStoreRtnCode FindSheetDs(out List<DataStorage> dataStorages)
		{
			ExStoreRtnCode rtnCode;

			dataStorages = new List<DataStorage>();

			rtnCode = StorLibR.FindDataStorages(Exid, Exid.ExsIdSheetSchemaName, out dataStorages);

			if (rtnCode != ExStoreRtnCode.XRC_GOOD)
			{
				Msgs.WriteLine("Did not find DataStorages| rtn code is ", rtnCode.ToString());
				SetRtnCode(rtnCode);
				return rtnCode;
			}

			foreach (DataStorage ds in dataStorages)
			{
				Msgs.WriteLine("Found a DataStorage| ", $"{ ds.Name}");
			}

			return XRC_GOOD;
		}

		public void EraseSheetDs()
		{
			List<DataStorage> dataStorages;

			ExStoreRtnCode rtnCode = FindSheetDs(out dataStorages);

			if (rtnCode != ExStoreRtnCode.XRC_GOOD)
			{
				SetRtnCode(rtnCode);
				return;
			}

			using (Transaction T = new Transaction(Exid.Document, "Erase Cells Data"))
			{
				try
				{
					T.Start();
					{
						foreach (DataStorage ds in dataStorages)
						{
							Msgs.Write($"Erasing DataStorage| {ds.Name}");
							StorLibR.DelDs(Exid, ds);

							Msgs.WriteLine(" | done");
						}
					}
					T.Commit();
				}
				catch
				{
					Msgs.WriteLine(" | failed");
					T.RollBack();
				}
			}
		}

		public void DeleteSheet()
		{
			ExStoreRtnCode rtnCode;
			DataStorage ds;
			Entity e;
			Schema s;

			List<Schema> schemas = new List<Schema>();

			rtnCode = StorLibR.FindExStorageInfo(Exid, Exid.ExsIdSheetSchemaName, out ds, out e, out s);

			if (rtnCode != ExStoreRtnCode.XRC_GOOD)
			{
				SetRtnCode(rtnCode);
				return;
			}

			schemas.Add(e.Schema);

			List<Entity> subEntities = StorLibR.getSubEntities(e);

			foreach (Entity subE in subEntities)
			{
				schemas.Add(subE.Schema);
			}

			using (Transaction T = new Transaction(Exid.Document, "Remove Cells Data"))
			{
				try
				{
					T.Start();
					{
						for (var i = schemas.Count - 1; i >= 0; i--)
						{
							Msgs.WriteLine("Removing schema|", schemas[i].SchemaName);

							Exid.Document.EraseSchemaAndAllEntities(schemas[i]);
							schemas[i].Dispose();
							schemas[i] = null;
						}
					}

					StorLibR.DelDs(Exid, ds);

					T.Commit();
				}
				catch
				{
					T.RollBack();
				}
			}
		}

		/// <summary>
		/// read the sheet data into a pre-initialized object<br/>
		/// expectation is to do this once at the begining
		/// </summary>
		/// <param name="initSheet"></param>
		/// <returns></returns>
		public bool ReadSheet()
		{
			// sheet must be populated.  it can have the current, used data set or
			// it can have a initialised data set.  this is needed as the data set
			// has the field information needed to read the data
			if (sheet == null) return SetRtnCode(XRC_FAIL);

			dsSheet = null;

			DataStorage ds;
			Entity e;
			Schema s;

			if (!SetRtnCode(StorLibR.FindExStorageInfo(Exid, Exid.ExsIdSheetSchemaName, out ds, out e, out s))) return false;

			SchemaLibR.ReadSheet(e, ref sheet);

			OnPropertyChanged(nameof(Sheet));

			return SetRtnCode(ReturnCode);
		}

		public bool ChangeSheetField(TShtKey keyName, DynaValue value)
		{
			ExStoreRtnCode rtnCode;
			DataStorage ds;
			Entity e;
			Schema s;

			if (sheet == null) return SetRtnCode(XRC_FAIL);
			if (!SetRtnCode(StorLibR.FindExStorageInfo(Exid, Exid.ExsIdSheetSchemaName, out ds, out e, out s))) return false;

			rtnCode = SchemaLibR.WriteField(keyName, value, sheet, ds, e, s);

			return rtnCode == XRC_GOOD;
		}

		// try to save the data to the DB 
		// fail if - ds exists or schema exists
		public bool WriteSheet()
		{
			// conditions
			// both ds and schema do not exist - new condition
			// ds exists and schema does not exist
			// schema exists and ds does not exist
			// both ds and schema exist

			if (sheet == null || exid == null) return SetRtnCode(XRC_FAIL);

			DataStorage ds;

			// check - does a ds exist
			SetRtnCode(StorLibR.FindSheetDs(Exid, out ds));

			if (ReturnCode == XRC_GOOD) return SetRtnCode(XRC_DS_EXISTS);

			if (StorLibR.DoesLockEntityExist(Exid)) return SetRtnCode(XRC_LOCK_EXISTS);

			SetRtnCode(SchemaLibR.WriteSheet());

			return SetRtnCode(ReturnCode);
		}

		public void ShowSheeFields()
		{
			Msgs.WriteLine("\n** Sheet Fields **");

			foreach ((TShtKey key, TShtFlds value) in sheet.Fields)
			{
				if (key.Equals(SK2_TEST_MAP))
				{
					Msgs.WriteLine("key ", $"{key,-22} | type {value.DyValue.TypeIs.Name}");

					Dictionary<string, string> map = value.DyValue.GetValueAs<Dictionary<string, string>>();
					foreach ((string mkey, string mvalue) in map)
					{
						Msgs.WriteLine("\t\tmap key ", $"{mkey,-18} | value {mvalue}");
					}
				}
				if (key.Equals(SK2_TEST_ARRAY))
				{
					Msgs.WriteLine("key ", $"{key,-22} | type {value.DyValue.TypeIs.Name}");

					List<string> map = value.DyValue.GetValueAs<List<string>>();

					foreach (string s in map)
					{
						Msgs.WriteLine("\t\tarray value ", $"{s,-18} ");
					}
				}
				else
				{
					Msgs.WriteLine("key ", $"{key,-22} | type {value.DyValue.TypeIs.Name,-12} | value {value.DyValue.Value}");
				}

			}
		}

		public void ShowRows()
		{
			Msgs.WriteLine("\n** Rows **");
			foreach ((string key, TRow value) in sheet.Rows)
			{
				Msgs.WriteLine("\nkey ", $"{key,-22} | {value.SchemaDesc}");
				showRowFields(value);
			}
		}

		private void showRowFields(TRow row)
		{
			Msgs.WriteLine("\n** Row Fields **");

			foreach ((TRowKey key, TRowFlds value) in row.Fields)
			{
				Msgs.WriteLine("key ", $"{key,-22} | type {value.DyValue.TypeIs.Name,-16} | value {value.DyValue.Value}");
			}
		}

	#endregion

	#region private methods

		private void reset()
		{
			Sheet = null;
			Lock = null;
			// SheetDs = null;
			// SheetEntity = null;
			// SheetSchema = null;
			// LockEntity = null;
			// LockSchema = null;
		}

		// private bool IsLocked()
		// {
		// 	return StorLibR.DoesLockEntityExist(exid);
		// }

	#endregion
	}
}