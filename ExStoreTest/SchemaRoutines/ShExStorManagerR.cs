#region + Using Directives

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
using ExStoreTest.Support;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ShScSupport;
using ShExStorageN.ShExStorage;
using static ShExStorageN.ShExStorage.ExStoreRtnCode;
using ShExStorageN.ShSchemaFields;
using ShExStorageR.ShExStorage;

#endregion

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


		public void DeleteSheet()
		{
			ExStoreRtnCode rtnCode;
			DataStorage ds;
			Entity e;

			List<Schema> schemas = new List<Schema>();

			rtnCode = StorLibR.FindEntity(Exid, Exid.ExsIdSheetSchemaName,
				out ds, out e);

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

			ExStoreRtnCode rtnCode;

			DataStorage ds;
			Entity e;

			if (SetRtnCode(StorLibR.FindEntity(Exid, Exid.ExsIdSheetSchemaName, out ds, out e))) return false;

			SchemaLibR.ReadSheet(e, ref sheet);

			OnPropertyChanged(nameof(Sheet));

			return SetRtnCode(ReturnCode);
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