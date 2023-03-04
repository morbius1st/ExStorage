#region using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using JetBrains.Annotations;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ShScSupport;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields;
using ShExStorageN.ShSchemaFields.ShScSupport;
using static ShExStorageC.ShSchemaFields.ShScSupport.ShExConstC;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExConstN;
using static ShExStorageN.ShExStorage.ExStoreRtnCode;
using ShStudyN.ShEval;

#endregion


// projname: $projectname$
// itemname: ShSchemaLibraryR1
// username: jeffs
// created:  11/13/2022 5:34:29 PM


namespace ShExStorageR.ShExStorage
{
	public class ShExSchemaLibR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds> : INotifyPropertyChanged
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
	#region base methods and fields

	#region private fields

		private TSht shtd;
		private Entity shtEntity;
		private Schema shtSchema;

		private TLok lokd;
		private Entity lokEntity;
		private Schema lokSchema;
		private ShDebugMessages m;


		public ShExStorageLibR StorLibR { get; set; }
		public ShExStorManagerR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>  smR { get; set; }

	#endregion

	#region ctor

		public ShExSchemaLibR()
		{
			config();
		}

		public void config()
		{
			StorLibR = new ShExStorageLibR();
		}

	#endregion

	#region public properties

		public ShDebugMessages M
		{
			get => m;
			set
			{
				m = value;
				StorLibR.M = value;
			}
		}

/*
		public DataStorage ShtDs
		{
			[DebuggerStepThrough]
			get => shtDs;
			[DebuggerStepThrough]
			set
			{
				if (Equals(value, shtDs)) return;
				shtDs = value;
				OnPropertyChanged();
			}
		}

		public Entity ShtEntity
		{
			[DebuggerStepThrough]
			get => shtEntity;
			[DebuggerStepThrough]
			set
			{
				if (Equals(value, shtEntity)) return;
				shtEntity = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(Configured));
			}
		}

		public Schema ShtSchema
		{
			[DebuggerStepThrough]
			get => shtSchema;
			[DebuggerStepThrough]
			set
			{
				if (Equals(value, shtSchema)) return;
				shtSchema = value;
				OnPropertyChanged();
			}
		}


		public DataStorage LokDs
		{
			[DebuggerStepThrough]
			get => lokDs;
			[DebuggerStepThrough]
			set => lokDs = value;
		}

		public Entity LokEntity
		{
			[DebuggerStepThrough]
			get => lokEntity;
			[DebuggerStepThrough]
			set
			{
				if (Equals(value, lokEntity)) return;
				lokEntity = value;
				OnPropertyChanged();
			}
		}

		public Schema LokSchema
		{
			[DebuggerStepThrough]
			get => lokSchema;
			[DebuggerStepThrough]
			set
			{
				if (Equals(value, lokSchema)) return;
				lokSchema = value;
				OnPropertyChanged();
			}
		}
*/


		// private static readonly Lazy<ShExSchemaLibR<TShtKey, TShtFlds, TRowKey, TRowFlds, TRow>> instance =
		// 	new Lazy<ShExSchemaLibR<TShtKey, TShtFlds, TRowKey, TRowFlds, TRow>>(() => new ShExSchemaLibR<TShtKey, TShtFlds, TRowKey, TRowFlds, TRow>());
		//
		// public static ShExSchemaLibR<TShtKey, TShtFlds, TRowKey, TRowFlds, TRow> Instance => instance.Value;


		// public DataStorage Datastorage
		// {
		// 	get => datastorage;
		// 	set
		// 	{
		// 		datastorage = value;
		// 		OnPropertyChanged();
		// 	}

		// public TSht SheetData
		// {
		// 	get => smR.Sheet;
		// 	set { smR.Sheet = value; }
		// }

		// public TLok LockData
		// {
		// 	get => lokd;
		// 	set => lokd = value;
		// }

		// public smR.Exid smR.Exid
		// {
		// 	get => smR.Exid;
		// 	set => smR.Exid = value;
		// }

		// public bool Configured => (shtEntity != null);

	#endregion

	#region public methods

		// public void EraseDataStorage()
		// {
		// 	Datastorage = null;
		//
		// 	EraseSheetSchema();
		// }
		//
		// public void EraseSheetSchema()
		// {
		// 	shtEntity = null;
		// 	shtSchema = null;
		//
		// 	EraseLockSchema();
		// }
		//
		// public void EraseLockSchema()
		// {
		// 	lokEntity = null;
		// 	lokSchema = null;
		// }

	#endregion

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


	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ShExSchemaLibR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>)}";
		}

	#endregion

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

			rtnCode = StorLibR.CreateDataStorage(lokExid, out ds);

			m.WriteLineStatus($"create ds status| {rtnCode}");

			if (rtnCode != XRC_GOOD) return XRC_FAIL;

			return WriteLock(ds, lokd);
		}

		// 00
		private ExStoreRtnCode WriteLock(DataStorage ds, TLok lokd)
		{
			M.WriteLineSteps("step: 01|", ">>> start | transaction| save lock");

			if (ds == null || !ds.IsValidObject
				|| lokd == null ) return ExStoreRtnCode.XRC_FAIL;

			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_GOOD;

			this.lokd = lokd;

			lokEntity = null;

			// step A1
			writeLock();

			ds.SetEntity(lokEntity);

			return lokEntity != null ? rtnCode : ExStoreRtnCode.XRC_ENTITY_NOT_FOUND;
		}

	#endregion

	#region private methods - write lock

// A1
		private void writeLock()
		{
			M.WriteLineSteps("step: A1|", ">>> start |primary sequence| save sheet");

			createLockSchema();

			writeLockData();
		}

// A3
		private void createLockSchema()
		{
			M.WriteLineSteps("step: A3|", ">>> start | create standard");

			lokSchema = null;

			SchemaBuilder sb = makeSheetPartialSchema(lokd);

			lokSchema = sb.Finish();

			lokEntity = new Entity(lokSchema);
		}

// WS
		private void writeLockData()
		{
			M.WriteLineSteps("step: WS|", ">>> start | write sheet data");

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

			SchemaBuilder sb = makeSheetPartialSchema(shtd);

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
		private SchemaBuilder makeSheetPartialSchema<TK, TF>(AShScFields<TK, TF> flds)
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
			List<Entity> subSchema = smR.StorLibR.getSubEntities(e);

			foreach (Entity ee in subSchema)
			{
				TRow row = new TRow();

				ReadData(ee, row);

				sheet.AddRow(row);
			}
		}

		#endregion

		#endregion

	}
}