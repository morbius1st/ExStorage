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
using ShExStorageC.ShSchemaFields.ShScSupport;
using static ShExStorageN.ShExStorage.ExStoreRtnCode;
using ShExStorageN.ShExStorage;
using ShStudyN.ShEval;

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

		public ShDebugMessages M { get; set; }

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
		[DebuggerStepThrough]
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
		[DebuggerStepThrough]
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

		//***************
		// the below needs to be primitive routines
		//***************


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

		// primitive version - assumes that this is within a transaction and that the lock, if needed, exists
		public ExStoreRtnCode DelSheet(ShtExId shtExid)
		{
			ExStoreRtnCode rtnCode = XRC_GOOD;

			DataStorage ds;
			Schema sl;
			Entity e;

			rtnCode = FindEntity(shtExid, false, out ds, out e);

			if (rtnCode != XRC_GOOD)
			{
				return SetRtnCodeE(rtnCode);
			}

			List<Entity> subEntities = getSubEntities(e);
			subEntities.Add(e);

			for (var i = subEntities.Count - 1; i >= 0; i--)
			{
				// remove all schema & entities (this has a bug)
				shtExid.Doc.EraseSchemaAndAllEntities(subEntities[i].Schema);
			}

			DelDs(ds);

			return rtnCode;
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
			M.WriteLineStatus($"find elements| match name{matchUserName}| username| {AExId.ReadUserName}");

			e = null;

			ExStoreRtnCode rtnCodeA = XRC_GOOD;
			ExStoreRtnCode rtnCodeB = XRC_GOOD;

			// note, need to process both
			rtnCodeA = FindDs(exid, matchUserName, out ds);

			rtnCodeB = FindSchema(exid, matchUserName, out s);

			M.WriteLineStatus($"find ds return code (true)| {rtnCodeA}");
			M.WriteLineStatus($"find schema return code (true)| {rtnCodeB}");


			if (rtnCodeA != XRC_GOOD) { return SetRtnCodeE(rtnCodeA); }

			if (rtnCodeB != XRC_GOOD) { return SetRtnCodeE(rtnCodeB); }

			rtnCodeB = GetDsEntity(ds, s, out e);

			M.WriteLineStatus($"find entity return code (true)| {rtnCodeB}");

			return SetRtnCodeE(rtnCodeB);
		}

		/// <summary>
		/// Find a schema based on its name
		/// </summary>
		public ExStoreRtnCode FindSchema(AExId exid, bool matchUserName, out Schema schema)
		{
			M.WriteLineStatus($"findsc| matchusrname| {matchUserName}");
			M.WriteLineStatus($"findsc| sch name| {exid.SchemaName}");

			schema = null;

			ExStoreRtnCode rtnCode = XRC_GOOD;

			IList<Schema> matchSchemas = new List<Schema>(1);

			IList<Schema>  schemas = Schema.ListSchemas();

			if (schemas != null && schemas.Count > 0)
			{
				rtnCode = XRC_SCHEMA_NOT_FOUND;

				foreach (Schema s in schemas)
				{
					M.WriteLineStatus($"findsc| sc name| {s.SchemaName}");

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
			M.WriteLineStatus($"findds| matchusrname| {matchUserName}");
			M.WriteLineStatus($"findds| ds name| {exid.DsName}");


			ds = null;
			ExStoreRtnCode result = XRC_DS_NOT_FOUND;

			FilteredElementCollector dataStorages
				= getDataStoreElements();

			if (dataStorages != null)
			{
				foreach (Element el in dataStorages)
				{
					M.WriteLineStatus($"findds| test| {(matchUserName && exid.DsNameMatches(el.Name)) || (!matchUserName && exid.ReadDsNameMatches(el.Name))}");

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

			M.WriteLineStatus($"create Ds for {exid.DsName}");

			try
			{
				ds = DataStorage.Create(exid.Doc);
				ds.Name = exid.DsName;
				result = XRC_GOOD;
			}
			catch (Exception e)
			{
				M.WriteLineStatus($"Exception| {e.Message}");
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
		//
		// public ShExStorManagerR<object, object, object, object, object, object, object, object, object> ShExStorManagerR
		// {
		// 	get => default;
		// 	set
		// 	{
		// 	}
		// }

		#endregion

	}
}