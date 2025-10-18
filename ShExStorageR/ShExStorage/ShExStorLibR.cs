#region + Using Directives

using ShExStorageC.ShSchemaFields;
using ShExStorageN.ShSchemaFields;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Autodesk.Revit.DB.ExtensibleStorage;
using ShExStorageN.ShExStorage;
using ShStudyN.ShEval;
using static ShExStorageC.ShSchemaFields.ShScSupport.ShExConstC;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExConstN;
using static ShExStorageN.ShExStorage.ExStoreRtnCode;
using Autodesk.Revit.DB;
using RevitSupport;

#endregion

// user name: jeffs
// created:   3/8/2023 9:05:12 PM

namespace ShExStorageR.ShExStorage
{
	public class ShExStorLibR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds> : INotifyPropertyChanged
		where TSht : AShScSheet<TShtKey, TShtFlds, TRowKey, TRowFlds, TSht, TRow>, new()
		where TRow : AShScRow<TRowKey, TRowFlds>, new()
		where TLok : AShScLock<TLokKey, TLokFlds>, new()
		where TShtKey : Enum
		where TShtFlds : ScFieldDefData<TShtKey>, new()
		where TRowKey : Enum
		where TRowFlds : ScFieldDefData<TRowKey>, new()
		where TLokKey : Enum
		where TLokFlds : ScFieldDefData<TLokKey>, new()
	{

	#region private fields

		private TSht shtd;
		private Entity shtEntity;
		private Schema shtSchema;

		private TLok lokd;
		private Entity lokEntity;
		private Schema lokSchema;
		private ShDebugMessages m;


		// public ShExStorageLibR StorLibR { get; set; }
		public ShExStorManagerR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>  smR { get; set; }

		private ExStoreRtnCode rtnCode;

	#endregion

	#region ctor

		public ShExStorLibR()
		{
			config();
		}

		public void config()
		{
			// StorLibR = new ShExStorageLibR();
		}

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ShExStorLibR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>)}";
		}

	#endregion

	#region events

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		[DebuggerStepThrough]
		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

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

	#region public properties

		public ShDebugMessages M
		{
			get => m;
			set
			{
				m = value;
				// StorLibR.M = value;
			}
		}

	#region private methods

		public TSht MakeInitSheet()
		{
			return new TSht();
		}

		public TLok MakeInitLock()
		{
			return new TLok();
		}

	#endregion

	#endregion

	#region write lock

	#region primary methods

		public ExStoreRtnCode WriteLock(LokExId lokExid, TLok lokd)
		{
			m.WriteLineStatus($"** write lock");

			if (lokd == null )
			{
				m.WriteLineStatus($"write lock| FAIL| data is null");
				return XRC_FAIL;
			}

			ExStoreRtnCode rtnCode;

			DataStorage ds;

			rtnCode = CreateDataStorage(lokExid, out ds);

			m.WriteLineStatus($"create ds status| {rtnCode}");

			if (rtnCode != XRC_GOOD) return XRC_FAIL;

			return saveLock(ds, lokd);
		}

		// L1
		private ExStoreRtnCode saveLock(DataStorage ds, TLok lokd)
		{
			M.WriteLineSteps("step: L1|", ">>> start | save lock");

			if (ds == null || !ds.IsValidObject
				|| lokd == null ) return ExStoreRtnCode.XRC_FAIL;

			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_GOOD;

			this.lokd = lokd;

			lokEntity = null;

			// step A1
			createLockSchema();

			writeLockData();

			ds.SetEntity(lokEntity);

			return lokEntity != null ? rtnCode : ExStoreRtnCode.XRC_ENTITY_NOT_FOUND;
		}

	#endregion

	#region private methods - write lock

// L3
		private void createLockSchema()
		{
			M.WriteLineSteps("step: L3|", ">>> start | create standard");

			lokSchema = null;

			SchemaBuilder sb = makePartialSchema(lokd);

			lokSchema = sb.Finish();

			lokEntity = new Entity(lokSchema);
		}

// WL
		private void writeLockData()
		{
			M.WriteLineSteps("step: WL|", ">>> start | write sheet data");

			writeData(lokEntity, lokSchema, lokd);
		}


// // B1
// 		private Schema makeLokSchema()
// 		{
// 			M.WriteLineSpaced("step: B1|", ">>> start | make the standard schema");
//
// 			SchemaBuilder sb = makeSheetPartialSchema(stdd);
//
// 			return sb.Finish();
// 		}

	#endregion

	#endregion

	#region Write Data

	#region primary methods

		// conditions
		// 1. datastorage exists, entity exists, and schema does exists.
		// +-> already configured - return with [XRC_DS_EXISTS]
		// 2. datastorage does not exist (and, therefore entity does not exist, schema may or may not exist)
		// proceed and check
		// is thre a lock
		// +-> yes - return with [XRC_LOCK_EXISTS]
		// not valid conditions
		// 3. datastorage does not exist but entity exists / schema may or may not exist - return with [XRC_ENTITY_EXISTS]
		//		not possible - the entity is stored in the datastorage element.  if the datastorage element is cone so is the entity
		// 4. datastorage exists, entity (and schema) do not exist - return with [XRC_ENTITY_NOT_FOUND] 
		//		this condition is not allowed.  that is, to update, the whole data set, schema, entities, datastorage element 
		//		gets removed and a completely new one is created
// 00

		/// <summary>
		/// save the sheet data and any row data to the model<br/>
		/// this routine must occur within a transaction<br/>
		/// return:<br/>
		/// XRC_GOOD = worked / saved<br/>
		/// XRC_FAIL = did not work (could not create a datastorage element
		/// </summary>
		/// <returns></returns>
		public ExStoreRtnCode WriteSheet(DataStorage ds, TSht shtd)
		{
			M.WriteLineSteps("step: 01|", ">>> start | transaction| save sheet");

			if (ds == null || !ds.IsValidObject
				|| shtd == null ) return ExStoreRtnCode.XRC_FAIL;

			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_GOOD;

			// this.exid = exid;
			shtEntity = null;

			// todo - fix this
			this.shtd = shtd;

			// at this point
			// sheet is not null
			// no existing ds
			// not locked

			// step A1

			saveSheet();

			if (shtEntity != null)
			{
				ds.SetEntity(shtEntity);
			}

			return rtnCode;
		}

	#endregion

	#region private methods - sheet schema

// A1
		private void saveSheet()
		{
			M.WriteLineSteps("step: A1|", ">>> start |primary sequence| save sheet");

			createSheetSchema();

			writeSheetData();
		}

// A3
		private void createSheetSchema()
		{
			M.WriteLineSteps("step: A3|", ">>> start | create sheet schema");

			shtSchema = null;

			// schema = makeSheetAndRowSchema();

			SchemaBuilder sb = makePartialSchema(shtd);

			addAllSubSchemaFields(sb);

			shtSchema = sb.Finish();

			shtEntity = new Entity(shtSchema);

			addAllRowSchemaFields();
		}

// B1
		// private Schema makeSheetAndRowSchema()
		// {
		// 	M.WriteLineSpaced("step: B1|", ">>> start | make the sheet and row schema");
		//
		// 	// B2
		// 	SchemaBuilder sb = makeSheetPartialSchema(smR.Sheet);
		//
		// 	// E1
		// 	addAllSubSchemaFields(sb);
		//
		// 	return sb.Finish();
		// }


// B2
		private SchemaBuilder makePartialSchema<TK, TF>(AShScFields<TK, TF> flds)
			where TK : Enum
			where TF : IShScFieldData<TK>, new()
		{
			M.WriteLineSteps("step: B2|", ">>> start | make the sheet portion of the schema");

			SchemaBuilder sb = new SchemaBuilder(flds.SchemaGuid);

			// C1
			configSchemaParams(sb, flds.SchemaName, flds.SchemaDesc, AExId.VendorId);

			// D1
			addSchemaFields(sb, flds);

			return sb;
		}

	#endregion

	#region private methods - common

// C1
		private void configSchemaParams(SchemaBuilder sb, string schemaName, string schemaDesc, string vendId)
		{
			M.WriteLineSteps("step: C1|", $"configure the schema");
			sb.SetReadAccessLevel(AccessLevel.Public);
			sb.SetWriteAccessLevel(AccessLevel.Public);

			sb.SetSchemaName(schemaName);
			sb.SetDocumentation(schemaDesc);
			sb.SetVendorId(vendId);
		}

// D1
		private void addSchemaFields<TKey, TFields>
			(SchemaBuilder sb, AShScFields<TKey, TFields> fields)
			where TKey : Enum
			where TFields : IShScFieldData<TKey>, new()
		{
			M.WriteLineSteps("step: D1|", $"create each schema fields");
			foreach (KeyValuePair<TKey, TFields> kvp in fields)
			{
				FieldBuilder fb = sb.AddSimpleField(kvp.Value.FieldName, kvp.Value.DyValue.RevitTypeIs);
				fb.SetDocumentation(kvp.Value.FieldDesc);
			}
		}

	#endregion

	#region private methods - sub-schema

// E1
		private void addAllSubSchemaFields(SchemaBuilder sb)
		{
			M.WriteLineSteps("step: E1|", ">>> start | add all sub-schema fields");

			foreach (KeyValuePair<string, TRow> kvp in shtd.Rows)
			{
				addSubSchemaField(sb, kvp.Value);
			}
		}

// E2
		private void addSubSchemaField(SchemaBuilder sb, TRow row)
		{
			M.WriteLineSteps("step: E2|", "add sub-schema field");

			FieldBuilder fb = sb.AddSimpleField(row.SchemaName, typeof(Entity));
			fb.SetDocumentation(row.SchemaDesc);
			fb.SetSubSchemaGUID(row.SchemaGuid);
		}

	#endregion

	#region private methods - row schema

// F1
		private void addAllRowSchemaFields()
		{
			foreach (KeyValuePair<string, TRow> kvp in shtd.Rows)
			{
				addRowSchema(kvp.Value);
			}
		}

// J1
		private void addRowSchema(AShScRow<TRowKey, TRowFlds> row)
		{
			M.WriteLineSteps("step: J1|", "create and add row schema and entity");

			Field f = shtSchema.GetField(row.SchemaName);

			Schema rowSchema = makeRowSchema(row);

			Entity rowE = new Entity(rowSchema);

			shtEntity.Set(f, rowE);
		}

// K1
		private Schema makeRowSchema(AShScRow<TRowKey, TRowFlds> row)
		{
			M.WriteLineSteps("step: K1|", ">>> start | create, configure, and make row schema fields");

			SchemaBuilder sb = new SchemaBuilder(row.SchemaGuid);

			// C1
			configSchemaParams(sb, row.SchemaName, row.SchemaDesc, AExId.VendorId);

			// D1
			addSchemaFields(sb, row);

			return sb.Finish();
		}

	#endregion

	#region private methods - write data

// WT
		private void writeSheetData()
		{
			M.WriteLineSteps("step: WT|", ">>> start | write sheet data");

			writeData(shtEntity, shtSchema, shtd);

			writeRows(shtEntity, shtSchema, shtd.Rows);
		}

// WD
		private void writeData<TKey, TFld>(Entity e, Schema s, AShScFields<TKey, TFld> data)
			where TKey : Enum
			where TFld : IShScFieldData<TKey>, new()
		{
			M.WriteLineSteps("step: WD|", "write each field's data");

			foreach (KeyValuePair<TKey, TFld> kvp in data)
			{
				writeField<TKey, TFld>(e, s, kvp.Value);
			}
		}

// WF
		private void writeField<TKey, TFld>(Entity e, Schema s, TFld field)
			where TKey : Enum
			where TFld : IShScFieldData<TKey>, new()
		{
			Field f = s.GetField(field.FieldName);

			if (f == null || !f.IsValidObject) return;

			e.Set(f, field.DyValue.RevitValue);
		}

// WR
		private void writeRows(Entity e, Schema s, Dictionary<string, TRow> rows)
		{
			M.WriteLineSteps("step: WR|", ">>> start | process each row to write the data");

			foreach (KeyValuePair<string, TRow> kvp in rows)
			{
				writeRow(e, s, kvp.Value);
			}
		}

// Wr
		private void writeRow(Entity e, Schema S, TRow row)
		{
			M.WriteLineSteps("step: Wr|", ">>> start | process a row to write the data");

			Field f = S.GetField(row.SchemaName);

			Entity subE = e.Get<Entity>(row.SchemaName);

			writeData(subE, subE.Schema, row);

			e.Set(f, subE);
		}

	#endregion

	#endregion

	#region read data

	#region Primary Methods

		public TSht ReadSheet(Entity e)
		{
			TSht shtd = new TSht();

			ReadData(e, shtd);

			readRowData(e, shtd);

			shtd.HasData = true;

			return shtd;
		}

		public TLok ReadLock(Entity e)
		{
			TLok lokd = new TLok();

			ReadData(e, lokd);

			return lokd;
		}


		/// <summary>
		/// read the data stored in the entity
		/// </summary>
		public void ReadData<TKey, TFlds>(Entity e, AShScFields<TKey, TFlds> data)
			where TKey : Enum
			where TFlds :  IShScFieldData<TKey>, new()
		{
			foreach (KeyValuePair<TKey, TFlds> kvp in data)
			{
				int idx = 0;

				Type t = kvp.Value.DyValue.TypeIs;

				try
				{
					if (t == typeof(string))
					{
						idx = 1;
						kvp.Value.SetValue = e.Get<string>(kvp.Value.FieldName);
					}
					else if (t == typeof(bool))
					{
						idx = 2;
						kvp.Value.SetValue = e.Get<bool>(kvp.Value.FieldName);
					}
					else if (t == typeof(Guid))
					{
						idx = 3;
						kvp.Value.SetValue = e.Get<Guid>(kvp.Value.FieldName);
					}
					else if (t.BaseType == typeof(Enum))
					{
						if (kvp.Value.FieldName.Equals(KEY_FIELD_NAME)) continue;

						string eName = e.Get<string>(kvp.Value.FieldName);
						data.ParseEnum(t, eName);
					}
				}
				catch
				{
					switch (idx)
					{
					case 1:
						{
							kvp.Value.SetValue = K_NOT_DEFINED;
							break;
						}

					case 2:
						{
							kvp.Value.SetValue = false;
							break;
						}

					case 3:
						{
							kvp.Value.SetValue = Guid.Empty;
							break;
						}
					}
				}
			}
		}

	#endregion

	#region private methods

		/// <summary>
		/// read the data stored in each sub-entity
		/// </summary>
		private void readRowData(Entity e, TSht sheet)
		{
			List<Entity> subSchema = smR.StorLib.getSubEntities(e);

			foreach (Entity ee in subSchema)
			{
				TRow row = new TRow();

				ReadData(ee, row);

				sheet.AddRow(row);
			}
		}

	#endregion

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
		public ExStoreRtnCode FindElements(AExId exid, bool matchUserName, out DataStorage ds, out Schema s, out Entity e )
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
			M.WriteLineStatus($"findsc| finding| {exid.SchemaName}");

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
		/// Get a DataStorage by any name<br/>
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
		/// returns the Entity when found<br/>
		/// XRC_GOOD = retrived<br/>
		/// XRC_ENTITY_NOT_FOUND = not found
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


	}
}