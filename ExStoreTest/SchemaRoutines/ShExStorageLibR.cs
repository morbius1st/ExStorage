#region using

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

using ShExStorageN.ShExStorage;

using ShExStorageC.ShSchemaFields;
using System.Windows.Input;

using ShExStorageC.ShSchemaFields.ShScSupport;
using ShExStorageN.ShSchemaFields;


#endregion


// username: jeffs
// created:  1/17/2022 6:35:18 AM


namespace ShExStorageR.ShExStorage
{
	public class ShExStorageLibR
	{
	
	#region base methods and fields
	
	#region private fields
	
		private Dictionary<string, Guid> subSchema;
	
		private Entity sheetEntity;
		private Schema sheetSchema;
		private DataStorage dataStorage;
	
		// private ExStorageLibraryC xlC;
	
		public ShExStorManagerR<
			ScDataSheet1,
			ScDataRow1,
			ScDataLock1,
			SchemaSheetKey,
			ScFieldDefData1<SchemaSheetKey>,
			SchemaRowKey,
			ScFieldDefData1<SchemaRowKey>,
			SchemaLockKey,
			ScFieldDefData1<SchemaLockKey>
			> smR { get; set; }
	
	#endregion
	
	#region ctor
	
		public ShExStorageLibR()
		{
			// xlC = new ExStorageLibraryC();
	
			// smR = ShExStorManagerR<
			// 	SchemaSheetKey,
			// 	ScFieldDefData1<SchemaSheetKey>,
			// 	SchemaRowKey,
			// 	ScFieldDefData1<SchemaRowKey>,
			// 	ScDataRow1,
			// 	SchemaLockKey,
			// 	ScFieldDefData1<SchemaLockKey>>.Instance;
		}
	
	#endregion
	
	#region public properties
	
		public DataStorage DataStorage => dataStorage;
	
		public Entity SheetEntity => sheetEntity;
	
		public Schema SheetSchema => sheetSchema;
	
		// public ExId ExId { get; private set;  }
		// public ExStorageAdmin Da { get; private set; }
	
		// public IList<Schema> AllSchemas { get; set; }
	
		public List<DataStorage> AllDs { get; set; }
	
	#endregion
	
	#region public methods
	
		// public ScDataSheet1 InitalizedSheet(ExId exid) => xlC.MakeInitSheet(exid);
	
		public bool HasDs()
		{
			return getDataStoreElements() != null;
		}
	
	#endregion
		
	
	#endregion
	
	
	
	#region DataStorage public
	
		// DataStorage
	
		/// <summary>
		/// Get all DataStorage's
		/// </summary>
		/// <returns>ExStoreRtnCode</returns>
		public ExStoreRtnCode GetAllDs()
		{
			AllDs = new List<DataStorage>(1);
	
			FilteredElementCollector dataStorages
				= getDataStoreElements();
	
			if (dataStorages == null) return ExStoreRtnCode.XRC_FAIL;
	
			foreach (Element ds in dataStorages)
			{
				AllDs.Add((DataStorage) ds);
			}
	
			return ExStoreRtnCode.XRC_GOOD;
		}
	
		/// <summary>
		/// Get a DataStorage by its Exid<br/>
		/// XRC_GOOD if found <br/>
		/// XRC_FAIL if not
		/// </summary>
		/// <param name="name">Name to find</param>
		/// <param name="ds">Out - the DS found</param>
		/// <returns>ExStoreRtnCode</returns>
		public ExStoreRtnCode FindSheetDs(ExId exid, out DataStorage ds)
		{
			ds = null;
			ExStoreRtnCode result = ExStoreRtnCode.XRC_FAIL;
	
			FilteredElementCollector dataStorages
				= getDataStoreElements();
	
			if (dataStorages != null)
			{
				foreach (Element el in dataStorages)
				{
					if (el.Name.Equals(exid.ExsIdSheetDsName))
					{
						ds = (DataStorage) el;
						result = ExStoreRtnCode.XRC_GOOD;
						break;
					}
				}
			}
	
			return result;
		}
	
		/// <summary>
		/// Delete a DataStorage by its Exid
		/// </summary>
		/// <param name="name">name of DataStorage to delete</param>
		/// <returns>ExStoreRtnCode</returns>
		public ExStoreRtnCode DelDs(ExId exid)
		{
			ExStoreRtnCode result;
	
			DataStorage ds;
	
			result = FindSheetDs(exid, out ds);
	
			if (result == ExStoreRtnCode.XRC_GOOD)
			{
				DelDs(exid, ds);
				result = ExStoreRtnCode.XRC_GOOD;
			}
	
			return result;
		}
	
		/// <summary>
		/// Delete a DataStorage by its ElementId
		/// </summary>
		/// <param name="eid"></param>
		/// <returns></returns>
		public ExStoreRtnCode DelDs(ExId exid, DataStorage ds)
		{
			// using (Transaction t = new Transaction(RvtCommand.RvtDoc, "Delete DataStorage"))
			// {
			// 	t.Start();
			// 	t.Commit();
			// }
	
			exid.Document.Delete(ds.Id);
			return ExStoreRtnCode.XRC_GOOD;
		}
	
		/// <summary>
		/// Determine if the Ds, named in the Exid, exists
		/// </summary>
		/// <param name="exid">ExStorage Identifier</param>
		/// <returns>True if exists, false otherwise</returns>
		public bool DoesSheetDsExist(ExId exid)
		{
			DataStorage ds;
	
			return FindSheetDs(exid, out ds) == ExStoreRtnCode.XRC_GOOD;
		}
	
		// public ExStoreRtnCode MakeSheetDs(ExId exid)
		// {
		// 	ExStoreRtnCode result;
		//
		// 	Transaction T;
		//
		// 	using (T = new Transaction(exid.Document, "Setup Cells"))
		// 	{
		// 		result = CreateDataStorage(exid, out dataStorage);
		//
		// 		if (result == ExStoreRtnCode.XRC_GOOD)
		// 		{
		// 			T.Commit();
		// 		}
		// 		else
		// 		{
		// 			result = ExStoreRtnCode.XRC_FAIL;
		// 			T.RollBack();
		// 		}
		// 	}
		//
		// 	return result;
		// }
	
		/// <summary>
		/// create the datastorage object with name from an ExId<br/>
		/// XRC_GOOD = new ds created<br/>
		/// XRC_DS_EXISTS = existing ds found<br/>
		/// XRC_FAIL = no ds found and no ds created
		/// </summary>
		/// <param name="exid"></param>
		/// <returns></returns>
		public ExStoreRtnCode CreateDataStorage(ExId exid, out DataStorage ds)
		{
			ds = null;
	
			if (dataStorage != null) return ExStoreRtnCode.XRC_DS_EXISTS;
	
			ExStoreRtnCode result;
	
			try
			{
				ds = DataStorage.Create(exid.Document);
				ds.Name = exid.ExsIdSheetDsName;
				result = ExStoreRtnCode.XRC_GOOD;
			}
			catch
			{
				result = ExStoreRtnCode.XRC_FAIL;
			}
	
			return result;
		}
	
	#endregion
	
	#region DataStorage private
	
		/// <summary>
		/// collect all datastore elements
		/// </summary>
		/// <returns></returns>
		private FilteredElementCollector getDataStoreElements()
		{
			FilteredElementCollector collector
				= new FilteredElementCollector(smR.Exid.Document);
	
			FilteredElementCollector dataStorages =
				collector.OfClass(typeof(DataStorage));
	
			return dataStorages;
		}
	
	#endregion
	
	#region Schema general public
	
		/// <summary>
		/// Get all Schema's in memory (this crosses document boundaries)<br/>
		/// IList is saved in DataStorageAdmin -> AllSchemas
		/// </summary>
		/// <returns>ExStoreRtnCode</returns>
		public ExStoreRtnCode GetAllSchema(out IList<Schema> schemas)
		{
			schemas = Schema.ListSchemas();
		
			return schemas.Count > 0 ? ExStoreRtnCode.XRC_GOOD : ExStoreRtnCode.XRC_SCHEMA_NOT_FOUND;
		}
	
		/// <summary>
		/// get all schemas from the entity
		/// </summary>
		/// <param name="Ds"></param>
		/// <param name="guids"></param>
		/// <returns></returns>
		public bool GetDsSchemas(DataStorage Ds, out IList<Guid> guids)
		{
			guids = Ds.GetEntitySchemaGuids();
	
			return guids != null;
		}
	
		/// <summary>
		/// Find a schema based on its Exid
		/// </summary>
		/// <param name="exid"></param>
		/// <param name="schema"></param>
		/// <returns>ExStoreRtnCode</returns>
		public ExStoreRtnCode FindSchema(string name, out Schema schema)
		{
			ExStoreRtnCode result;
		
			IList<Schema> matchSchemas = new List<Schema>(1);
		
			schema = null;
			IList<Schema> schemas;
	
			result = GetAllSchema(out schemas);
		
			if (result == ExStoreRtnCode.XRC_GOOD)
			{
				result = ExStoreRtnCode.XRC_SCHEMA_NOT_FOUND;
		
				foreach (Schema s in schemas)
				{
					if (s.SchemaName.Equals(name))
					{
						matchSchemas.Add(s);
					}
				}
		
				if (matchSchemas.Count == 1)
				{
					schema = matchSchemas[0];
		
					result = ExStoreRtnCode.XRC_GOOD;
				}
			}
		
			return result;
		}
	
		/// <summary>
		/// find a schema using a Guid or return null
		/// </summary>
		/// <param name="guid"></param>
		/// <param name="s"></param>
		/// <returns></returns>
		public bool FindSchema(Guid guid, out Schema s)
		{
			s = Schema.Lookup(guid);
			return s != null;
		}
	
	
		public void DelSchema(ExId exid, Schema s)
		{
			exid.Document.EraseSchemaAndAllEntities(s);
		}
	
	
		/// <summary>
		/// Determine if the sheet schema exists
		/// </summary>
		/// <param name="exid">The Ex Storage Identifier</param>
		/// <returns>True if exists, false otherwise</returns>
		public bool DoesSheetSchemaExist(ExId exid)
		{
			Schema schema;
	
			return FindSchema(exid.ExsIdSheetSchemaName, out schema) == ExStoreRtnCode.XRC_GOOD;
		}
	
		/// <summary>
		/// Determine if the lock schema exists
		/// </summary>
		/// <param name="exid">The Ex Storage Identifier</param>
		/// <returns>True if exists, false otherwise</returns>
		public bool DoesLockSchemaExist(ExId exid)
		{
			Schema schema;
	
			return FindSchema(exid.ExsIdLockSchemaName, out schema) == ExStoreRtnCode.XRC_GOOD;
		}
	
	#endregion
	
	#region Schema creation public
	
	
	#endregion
	
	#region Entity public
	
		/// <summary>
		/// get the Entity stored in the DataStorage
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="sc"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public ExStoreRtnCode GetDsEntity(DataStorage ds, Schema sc, out Entity e)
		{
			ExStoreRtnCode result = ExStoreRtnCode.XRC_GOOD;
			e = null;
	
			try
			{
				e = ds.GetEntity(sc);
			}
			catch
			{
				result = ExStoreRtnCode.XRC_ENTITY_NOT_FOUND;
			}
	
			return result;
		}
	
		public void DelEntity(DataStorage ds, Schema s)
		{
			ds.DeleteEntity(s);
		}
	
		public List<Entity> getSubEntities(Entity e)
		{
			List<Entity> eList = new List<Entity>();
	
			// eList.Add(e);
	
			Schema s = e.Schema;
	
			foreach (Field f in s.ListFields())
			{
				if (f.SubSchema==null)	 continue;
	
				Field ff = s.GetField(f.FieldName);
				if (ff==null || !ff.IsValidObject) break;
	
				Entity ee = e.Get<Entity>(ff);
	
				if (ee == null || !ee.IsValidObject) break;
	
				eList.Add(ee);
			}
	
			return eList;
		}
	
	
		// entity data
	
		/// <summary>
		/// Get the raw value stored in the entity for the<br/>
		/// schema field 
		/// </summary>
		/// <param name="e">The entity with the date</param>
		/// <param name="f">The field for which to get the data</param>
		/// <returns>The raw value as a string</returns>
		public string GetEntityDataAsString(Entity e, Field f)
		{
			string value = null;
	
			if (f.ValueType == typeof(string))
			{
				value = e.Get<string>(f);
			}
			else
			{
				value = $"unknown";
			}
	
			return value;
		}
	
		/// <summary>
		/// find the sheet entity based on the Exid<br/>
		/// XRC_GOOD = retrived<br/>
		/// XRC_ENTITY_NOT_FOUND = not found<br/>
		/// returns the Entity when found
		/// </summary>
		/// <param name="exid"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		// public ExStoreRtnCode FindSheetEntity(ExId exid, out Entity e)
		// {
		// 	e = null;
		// 	bool result;
		// 	ExStoreRtnCode rtnCode;
		//
		// 	DataStorage ds;
		//
		// 	rtnCode = FindSheetDs(exid, out ds);
		//
		// 	if (rtnCode != ExStoreRtnCode.XRC_GOOD) return ExStoreRtnCode.XRC_ENTITY_NOT_FOUND;
		//
		// 	IList<Guid> guids;
		//
		// 	if (!GetDsSchemas(ds, out guids)) return ExStoreRtnCode.XRC_ENTITY_NOT_FOUND;
		//
		// 	Schema s;
		//
		// 	if (guids == null || guids.Count == 0) return ExStoreRtnCode.XRC_ENTITY_NOT_FOUND;
		//
		// 	if (!FindSchema(guids[0], out s)) return ExStoreRtnCode.XRC_ENTITY_NOT_FOUND;
		//
		// 	rtnCode = GetDsEntity(ds, s, out e);
		//
		// 	return rtnCode;
		// }
	
		/// <summary>
		/// check if the sheet entity exists<br/>
		/// true = found / false = not found
		/// </summary>
		/// <param name="exid"></param>
		/// <returns></returns>
		public bool DoesSheetEntityExist(ExId exid)
		{
			Entity e;
			DataStorage ds;
	
			ExStoreRtnCode rtnCode = FindEntity(exid, exid.ExsIdSheetSchemaName, 
				out ds, out e);
	
			return rtnCode == ExStoreRtnCode.XRC_GOOD;
	
		}
	
		public bool DoesLockEntityExist(ExId exid)
		{
			Entity e;
			DataStorage ds;
	
			ExStoreRtnCode rtnCode = FindEntity(exid, exid.ExsIdLockSchemaName, 
				out ds, out e);
	
			return rtnCode == ExStoreRtnCode.XRC_GOOD;
	
		}
	
		public ExStoreRtnCode FindEntity(ExId exid, string name, 
			out DataStorage ds, out Entity e)
		{
			e = null;
			bool result;
			ExStoreRtnCode rtnCode;
	
			rtnCode = FindSheetDs(exid, out ds);
	
			if (rtnCode != ExStoreRtnCode.XRC_GOOD) return ExStoreRtnCode.XRC_ENTITY_NOT_FOUND;
	
			rtnCode = ExStoreRtnCode.XRC_ENTITY_NOT_FOUND;
	
			IList<Guid> guids;
	
			if (!GetDsSchemas(ds, out guids)) return rtnCode;
	
			Schema s;
	
			if (guids == null || guids.Count == 0) return rtnCode;
	
			for (var i = 0; i < guids.Count; i++)
			{
				result = FindSchema(guids[i], out s);
	
				if (s.SchemaName.Equals(name))
				{
					rtnCode = GetDsEntity(ds, s, out e);
					break;
				}
			}
	
			return rtnCode;
		}
	
	#endregion
	
	#region system overrides
	
		public override string ToString()
		{
			return $"this is {nameof(ShExStorageLibR)}";
		}
	
	#endregion
	
	}
}