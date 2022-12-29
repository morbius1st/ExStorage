#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using RevitSupport;
using ShExStorageN.ShExStorage;
using SharedApp.Windows.ShSupport;
using ShExStorageC.ShSchemaFields;
using System.Windows.Input;
using ExStorage.Windows;
using JetBrains.Annotations;
using ShExStorageC.ShExStorage;
using ShExStorageC.ShSchemaFields.ScSupport;
using static ShExStorageN.ShExStorage.ExStoreRtnCode;
using ShExStorageN.ShExStorage;
using ShStudy.ShEval;

#endregion


// username: jeffs
// created:  1/17/2022 6:35:18 AM


namespace ShExStorageR.ShExStorage
{
	public class ShExStorageLibR : INotifyPropertyChanged
	{
	#region base methods and fields

	#region private fields

		private Dictionary<string, Guid> subSchema;

		public ShDebugMessages M { get; set; }

		private ExStoreRtnCode rtnCode;

		public ShExStorManagerR<
			ScDataSheet,
			ScDataRow,
			ScDataLock,
			SchemaSheetKey,
			ScFieldDefData<SchemaSheetKey>,
			SchemaRowKey,
			ScFieldDefData<SchemaRowKey>,
			SchemaLockKey,
			ScFieldDefData<SchemaLockKey>
			> smR { get; set; }

	#endregion

	#region ctor

		// public ShExStorageLibRx() { }

	#endregion

	#region public properties

		public List<DataStorage> AllDs { get; set; }

		public ExStoreRtnCode ReturnCode
		{
			get => rtnCode;
			private set
			{
				rtnCode = value;
				OnPropertyChanged();
			}
		}

	#endregion

	#region private methods

		/// <summary>
		/// set the class return code 'rtnCode' and returns the supplied<br/>
		/// rtnCode if supplied rtnCode is not XRC_VOID<br/>
		/// and returns the last rtnCode is the supplied rtnCode<br/>
		/// is XRC_VOID (preset default)
		/// /// </summary>
		private ExStoreRtnCode SetRtnCodeE(ExStoreRtnCode rtnCode  = XRC_VOID)
		{
			if (rtnCode == XRC_VOID)
			{
				return ReturnCode;
			}

			ReturnCode = rtnCode;

			return rtnCode;
		}

		/// <summary>
		/// set the class return code 'rtnCode' and return<br/>
		/// true if rtnCode matches the testCode (which is XRC_GOOD by default)<br/>
		/// that is, by default, if the rtnCode is good, this returns true<br/>
		/// else, this returns false<br/>
		/// however, if no rtnCode is supplied, this compares the last<br/>
		/// rtnCode with the default (XRC_GOOD) or the supplied testCode
		/// </summary>
		private bool SetRtnCodeB(ExStoreRtnCode rtnCode = XRC_VOID, ExStoreRtnCode testCode = XRC_GOOD)
		{
			if (rtnCode != XRC_VOID)
			{
				ReturnCode = rtnCode;
			}

			return this.rtnCode == testCode;
		}

	#endregion

	#endregion


	#region event processing

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		[DebuggerStepThrough]
		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

	#endregion


	#region sheet

		/// <summary>
		/// Delete a DataStorage by its Exid
		/// </summary>
		/// <param name="name">name of DataStorage to delete</param>
		/// <returns>ExStoreRtnCode</returns>
		public ExStoreRtnCode DelSheetDs(ShtExId shtExid)
		{
			DataStorage ds;

			if (SetRtnCodeB(FindDs(shtExid, true, out ds)))
			{
				DelDs(ds);
			}

			return SetRtnCodeE();
		}

		/// <summary>
		/// Delete a DataStorage by its ElementId
		/// </summary>
		/// <param name="exid"></param>
		/// <param name="ds"></param>
		/// <returns></returns>
		public ExStoreRtnCode DelDs(DataStorage ds)
		{
			ExStoreRtnCode rtnCode = XRC_GOOD;

			try
			{
				AExId.Document.Delete(ds.Id);
			}
			catch
			{
				rtnCode = XRC_FAIL;
			}

			return SetRtnCodeE(rtnCode);
		}

	#endregion

	#region lock

		/// <summary>
		/// determine if the lock exists and return<br/>
		/// return the lock owner if it exists
		/// </summary>
		/// <returns>the lock owner if the lock exists, null otherwise</returns>
		public ExStoreRtnCode DoesLockExist(LokExId lokExid, out string userName)
		{
			DataStorage ds;
			Entity e;
			userName = null;

			if (SetRtnCodeB(FindEntity(lokExid, false, out ds, out e)))
			{
				userName = AExId.ParseReadUserName(e.Schema.SchemaName);
			}

			return SetRtnCodeE();
		}

		/// <summary>
		/// determine if the lock can be deleted<br/>
		/// that is, I am the lock owner
		/// </summary>
		/// <returns>
		/// XRC_GOOD if yes<br/>
		/// XRC_LOCK_NOT_EXIST if no<br/>
		/// </returns>
		public ExStoreRtnCode CanDeleteLock(LokExId lokExid)
		{
			ExStoreRtnCode rtnCode = XRC_LOCK_CANNOT_DEL;

			Entity e;
			DataStorage ds;

			if (FindEntity(lokExid, true, out ds, out e) == XRC_GOOD)
			{
				// lock exists - am I the owner
				if (lokExid.UserNameMatches(e.Schema.SchemaName))
				{
					rtnCode = XRC_GOOD;
				}
			}

			return SetRtnCodeE(rtnCode);
		}

	#endregion

	#region general public

		/// <summary>
		/// Find all associated elements - ds, schema, entity - if possible<br/>
		/// 1st, find the ds - return XRC_GOODn or XRC_DS_NOT_FOUND<br/>
		/// 2nd, find the schema - return XRC_GOOD or XRC_SCHEMA_NOT_FOUND<br/>
		/// 3rd, find the entity (straight Revit - no return code)<br/>
		/// return XRC_GOOD if all found
		/// </summary>
		public ExStoreRtnCode FindElements(AExId exid, bool matchUserName, 
			out DataStorage ds, out Schema s, out Entity e )
		{
			e = null;

			ExStoreRtnCode rtnCodeA = XRC_GOOD;
			ExStoreRtnCode rtnCodeB = XRC_GOOD;

			// note, need to process both
			rtnCodeA = FindDs(exid, matchUserName, out ds);

			rtnCodeB = FindSchema(exid, matchUserName, out s);

			if (rtnCodeA != XRC_GOOD) { return SetRtnCodeE(rtnCodeA); }

			if (rtnCodeB != XRC_GOOD) { return SetRtnCodeE(rtnCodeB); }

			rtnCodeB = GetDsEntity(ds, s, out e);

			return SetRtnCodeE(rtnCodeB);
		}

		/// <summary>
		/// Find a schema based on its name
		/// </summary>
		public ExStoreRtnCode FindSchema(AExId exid, bool matchUserName, out Schema schema)
		{
			schema = null;

			ExStoreRtnCode rtnCode = XRC_GOOD;

			IList<Schema> matchSchemas = new List<Schema>(1);

			IList<Schema>  schemas = Schema.ListSchemas();

			if (schemas != null && schemas.Count > 0)
			{
				rtnCode = XRC_SCHEMA_NOT_FOUND;

				foreach (Schema s in schemas)
				{
					if ((matchUserName && exid.SchNameMatches(s.SchemaName)) ||
						(!matchUserName && (exid.ReadSchNameMatches(s.SchemaName)))
						)
					{
						matchSchemas.Add(s);
					}
				}

				if (matchSchemas.Count == 1)
				{
					schema = matchSchemas[0];

					rtnCode = XRC_GOOD;
				}
			}

			return SetRtnCodeE(rtnCode);
		}

		/// <summary>
		/// Get a DataStorage by any name / <br/>
		/// XRC_GOOD if found <br/>
		/// XRC_FAIL if not
		/// </summary>
		public ExStoreRtnCode FindDs(AExId exid, bool matchUserName, out DataStorage ds)
		{
			ds = null;
			ExStoreRtnCode result = XRC_DS_NOT_FOUND;

			FilteredElementCollector dataStorages
				= getDataStoreElements();

			if (dataStorages != null)
			{
				foreach (Element el in dataStorages)
				{
					if ((matchUserName && exid.DsNameMatches(el.Name)) ||
						(!matchUserName && (exid.ReadDsNameMatches(el.Name)))
						)
					{
						ds = (DataStorage) el;
						result = XRC_GOOD;
						break;
					}
				}
			}

			return SetRtnCodeE(result);
		}

		/// <summary>
		/// find the sheet entity based on the Exid<br/>
		/// XRC_GOOD = retrived<br/>
		/// XRC_ENTITY_NOT_FOUND = not found<br/>
		/// returns the Entity when found
		/// </summary>
		public ExStoreRtnCode FindEntity(AExId exid, bool matchUserName, out DataStorage ds, out Entity e)
		{
			e = null;
			ExStoreRtnCode rtnCode = XRC_ENTITY_NOT_FOUND;

			if (!SetRtnCodeB(FindDs(exid, matchUserName, out ds))) return SetRtnCodeE(rtnCode);

			IList<Guid> guids = ds.GetEntitySchemaGuids();

			Schema s;

			if (guids == null || guids.Count == 0) return SetRtnCodeE(rtnCode);

			for (var i = 0; i < guids.Count; i++)
			{
				s = Schema.Lookup(guids[i]);

				if (s != null &&
					(matchUserName && exid.SchNameMatches(s.SchemaName)) ||
					(!matchUserName && exid.ReadSchNameMatches(s.SchemaName)))
				{
					rtnCode = GetDsEntity(ds, s, out e);
					break;
				}
			}

			return SetRtnCodeE(rtnCode);
		}



		/// <summary>
		/// get the ds group of elements and return bool whether each exists
		/// </summary>
		public void DoElementsExist(AExId exid, out bool dsExists, out bool schemaExists, out bool entityExists)
		{
			DataStorage ds;
			Schema s;
			Entity e;

			FindElements(exid, true, out ds, out s, out e);

			dsExists = ds != null && ds.IsValidObject;
			schemaExists = s != null && s.IsValidObject;
			entityExists = e != null && e.IsValidObject;
		}

		/// <summary>
		/// Determine if the Ds with the name provided exists
		/// </summary>
		/// <returns>True if exists, false otherwise</returns>
		public bool DoesDsExist(AExId exid)
		{
			DataStorage ds;

			return FindDs(exid, true, out ds) == XRC_GOOD;
		}


		/// <summary>
		/// create the datastorage object with name from an IExId<br/>
		/// this must occur within a transaction<br/>
		/// XRC_GOOD = new ds created<br/>
		/// XRC_FAIL = no ds created
		/// </summary>
		/// <param name="exid"></param>
		/// <returns></returns>
		public ExStoreRtnCode CreateDataStorage(AExId exid, out DataStorage ds)
		{
			ExStoreRtnCode result;

			M.WriteLine($"making Ds for {exid.DsName}");

			try
			{
				ds = DataStorage.Create(exid.Doc);
				ds.Name = exid.DsName;
				result = XRC_GOOD;
			}
			catch
			{
				result = XRC_FAIL;
				ds = null;
			}

			return SetRtnCodeE(result);
		}

		/// <summary>
		/// Get all DataStorage's
		/// </summary>
		/// <returns>ExStoreRtnCode</returns>
		public ExStoreRtnCode GetAllDs()
		{
			AllDs = new List<DataStorage>(1);

			FilteredElementCollector dataStorages
				= getDataStoreElements();

			if (dataStorages == null) return SetRtnCodeE(XRC_FAIL);

			foreach (Element ds in dataStorages)
			{
				AllDs.Add((DataStorage) ds);
			}

			return SetRtnCodeE(XRC_GOOD);
		}

		/// <summary>
		/// get the Entity stored in the DataStorage
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="sc"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public ExStoreRtnCode GetDsEntity(DataStorage ds, Schema sc, out Entity e)
		{
			ExStoreRtnCode rtnCode = XRC_GOOD;
			e = null;
		
			try
			{
				e = ds.GetEntity(sc);
			}
			catch
			{
				rtnCode = XRC_ENTITY_NOT_FOUND;
			}
		
			return SetRtnCodeE(rtnCode);
		}

		/// <summary>
		/// get the list of sub-entities from a DS entity, if any
		/// </summary>
		/// <param name="e"></param>
		public List<Entity> getSubEntities(Entity e)
		{
			List<Entity> eList = new List<Entity>();

			Schema s = e.Schema;

			foreach (Field f in s.ListFields())
			{
				if (f.SubSchema == null)	 continue;

				Field ff = s.GetField(f.FieldName);
				if (ff == null || !ff.IsValidObject) break;

				Entity ee = e.Get<Entity>(ff);

				if (ee == null || !ee.IsValidObject) break;

				eList.Add(ee);
			}

			return eList;
		}

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

	#region general private

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

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ShExStorageLibR)}";
		}

	#endregion


		// /// <summary>
		// /// check if the sheet entity exists<br/>
		// /// true = found / false = not found
		// /// </summary>
		// /// <param name="exid"></param>
		// /// <returns></returns>
		// public bool DoesSheetEntityExist(IExId exid)
		// {
		// 	Entity e;
		// 	DataStorage ds;
		//
		// 	ExStoreRtnCode rtnCode = FindEntity(
		// 		exid.ExsIdSheetDsName,
		// 		exid.ExsIdSheetSchemaName,
		// 		out ds, out e);
		//
		// 	return rtnCode == XRC_GOOD;
		// }


		// /// <summary>
		// /// Determine if the sheet schema exists
		// /// </summary>
		// /// <param name="exid">The Ex Storage Identifier</param>
		// /// <returns>True if exists, false otherwise</returns>
		// public bool DoesSheetSchemaExist(IExId exid)
		// {
		// 	Schema schema;
		//
		// 	return FindSchema(exid.ExsIdSheetSchemaName, out schema) == XRC_GOOD;
		// }


		// public void DelEntity(DataStorage ds, Schema s)
		// {
		// 	ds.DeleteEntity(s);
		// }


		// /// <summary>
		// /// Get all Schema's in memory (this crosses document boundaries)<br/>
		// /// IList is saved in DataStorageAdmin -> AllSchemas
		// /// </summary>
		// /// <returns>ExStoreRtnCode</returns>
		// public ExStoreRtnCode GetAllSchema(out IList<Schema> schemas)
		// {
		// 	schemas = Schema.ListSchemas();
		//
		// 	return schemas.Count > 0 ? XRC_GOOD : XRC_SCHEMA_NOT_FOUND;
		// }


		// /// <summary>
		// /// get all schemas from the entity
		// /// </summary>
		// /// <param name="Ds"></param>
		// /// <param name="guids"></param>
		// /// <returns></returns>
		// public bool GetAllDsGuids(DataStorage Ds, out IList<Guid> guids)
		// {
		// 	guids = Ds.GetEntitySchemaGuids();
		//
		// 	return guids != null;
		// }


		// /// <summary>
		// /// find a schema using a Guid or return null
		// /// </summary>
		// /// <param name="guid"></param>
		// /// <param name="s"></param>
		// /// <returns></returns>
		// public bool FindSchema(Guid guid, out Schema s)
		// {
		// 	s = Schema.Lookup(guid);
		// 	return s != null;
		// }
	}
}