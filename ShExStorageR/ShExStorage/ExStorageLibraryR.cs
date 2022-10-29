#region using

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using RevitSupport;
using SharedApp.Windows.ShSupport;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields;
using ShExStorageC.ShSchemaFields;
using System.Windows.Input;

#endregion


// username: jeffs
// created:  1/17/2022 6:35:18 AM


namespace ShExStorageR.ShExStorage
{
	public class ExStorageLibraryR
	{
	#region private fields

		// private Document doc;

	#endregion

	#region ctor

		public ExStorageLibraryR()
		{
			// this.doc = doc;

			// ExId = new ExId(RvtCommand.RvtDoc.Title);
			Da = new ExStorageAdmin();
		}

	#endregion

	#region public properties

		public AWindow W { get; set; }

		// public ExId ExId { get; private set;  }
		public ExStorageAdmin Da { get; private set; }

		// public string EsId => ExId.ExsId;

	#endregion

	#region public methods

		public bool HasDs()
		{
			return getDataStoreElements() != null;
		}

	#endregion

	#region DataStorage public

		// DataStorage

		/// <summary>
		/// Get all DataStorage's
		/// </summary>
		/// <returns>ExStoreRtnCode</returns>
		public ExStoreRtnCode GetAllDs()
		{
			Da.AllDs = new List<DataStorage>(1);

			FilteredElementCollector dataStorages
				= getDataStoreElements();

			if (dataStorages == null) return ExStoreRtnCode.XRC_FAIL;

			foreach (Element ds in dataStorages)
			{
				Da.AllDs.Add((DataStorage) ds);
			}

			return ExStoreRtnCode.XRC_GOOD;
		}

		/// <summary>
		/// Get a DataStorage by its Exid
		/// </summary>
		/// <param name="name">Name to find</param>
		/// <param name="ds">Out - the DS found</param>
		/// <returns>ExStoreRtnCode</returns>
		public ExStoreRtnCode GetDs(ExId exid, out DataStorage ds)
		{
			ds = null;
			ExStoreRtnCode result = ExStoreRtnCode.XRC_FAIL;

			FilteredElementCollector dataStorages
				= getDataStoreElements();

			if (dataStorages != null)
			{
				foreach (Element el in dataStorages)
				{
					if (el.Name.Equals(exid.ExsId))
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

			result = GetDs(exid, out ds);

			if (result == ExStoreRtnCode.XRC_GOOD)
			{
				DelDs(ds);
				result = ExStoreRtnCode.XRC_GOOD;
			}

			return result;
		}

		/// <summary>
		/// Delete a DataStorage by its ElementId
		/// </summary>
		/// <param name="eid"></param>
		/// <returns></returns>
		public ExStoreRtnCode DelDs(DataStorage ds)
		{
			using (Transaction t = new Transaction(RvtCommand.RvtDoc, "Delete DataStorage"))
			{
				t.Start();
				RvtCommand.RvtDoc.Delete(ds.Id);
				t.Commit();
			}

			return ExStoreRtnCode.XRC_GOOD;
		}

		/// <summary>
		/// Determine if the Ds, named in the Exid, exists
		/// </summary>
		/// <param name="exid">ExStorage Identifier</param>
		/// <returns>True if exists, false otherwise</returns>
		public bool DoesDsExist(ExId exid)
		{
			DataStorage ds;

			return GetDs(exid, out ds) == ExStoreRtnCode.XRC_GOOD;
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
				= new FilteredElementCollector(RvtCommand.RvtDoc);

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
		public ExStoreRtnCode GetAllSchema()
		{
			Da.AllSchemas = Schema.ListSchemas();

			return Da.AllSchemas.Count > 0 ? ExStoreRtnCode.XRC_GOOD : ExStoreRtnCode.XRC_SCHEMA_NOT_FOUND;
		}

		/// <summary>
		/// Find a schema based on its Exid
		/// </summary>
		/// <param name="exid"></param>
		/// <param name="schema"></param>
		/// <returns>ExStoreRtnCode</returns>
		public ExStoreRtnCode GetSchema(ExId exid, out Schema schema)
		{
			ExStoreRtnCode result;

			IList<Schema> matchSchemas = new List<Schema>(1);

			schema = null;

			result = GetAllSchema();

			if (result == ExStoreRtnCode.XRC_GOOD)
			{
				result = ExStoreRtnCode.XRC_SCHEMA_NOT_FOUND;

				foreach (Schema s in Da.AllSchemas)
				{
					if (s.SchemaName.Equals(exid.ExsId))
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
		/// Determine if the schema, named in the Exid, exists
		/// </summary>
		/// <param name="exid">The Ex Storage Identifier</param>
		/// <returns>True if exists, false otherwise</returns>
		public bool DoesSchemaExist(ExId exid)
		{
			Schema schema;

			return GetSchema(exid, out schema) == ExStoreRtnCode.XRC_GOOD;
		}

	#endregion


	#region Schema creation public

		// public ExStoreRtnCode MakeSchema(ExId exid, )

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

	#endregion


	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ExStorageLibraryR)}";
		}





		#endregion
		#region Schema


		public ExStoreRtnCode MakeSchema<Tkey, Tdata, TCkey, TCdata>(

			IShScBaseData<Tkey, Tdata, TCkey, TCdata> BaseData) 
			where Tkey : Enum
			where TCkey : Enum
			where Tdata : ShScFieldDefData<Tkey>, new()
			where TCdata : AShScInfoBase<TCkey, ShScFieldDefData<TCkey>>, new()
		{

			if (BaseData == null || BaseData.SchemaName == null) return ExStoreRtnCode.XRC_FAIL;

			return ExStoreRtnCode.XRC_GOOD;
		}




		#endregion
	}
}