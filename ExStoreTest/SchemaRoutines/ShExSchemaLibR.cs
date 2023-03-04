#region using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStoreTest.Support;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ShScSupport;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields;
using ShExStorageN.ShSchemaFields.ShScSupport;

#endregion


// projname: $projectname$
// itemname: ShSchemaLibraryR1
// username: jeffs
// created:  11/13/2022 5:34:29 PM


namespace ShExStorageR.ShExStorage
{
	public class ShExSchemaLibR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds> : INotifyPropertyChanged
		where TSht : AShScSheet<TShtKey, TShtFlds, TRowKey, TRowFlds, TRow>, new()
		where TRow : AShScRow<TRowKey, TRowFlds>, new()
		where TLok : AShScFields<TLokKey, TLokFlds>, new()
		where TShtKey : Enum
		where TShtFlds : IShScFieldData1<TShtKey>, new()
		where TRowKey : Enum
		where TRowFlds : IShScFieldData1<TRowKey>, new()
		where TLokKey : Enum
		where TLokFlds : IShScFieldData1<TLokKey>, new()
	{
	#region base methods and fields

	#region private fields

		// typical type: sheet
		// private TSht smR.Sheet;

		// special type: chart e.g. lock
		private TLok lokd;

		// private smR.Exid smR.Exid
		// {
		// 	get => smR.smR.Exid;
		// 	set => smR.smR.Exid = value;
		// }

		private DataStorage datastorage;

		private Entity shtEntity;
		private Schema shtSchema;

		private Entity lokEntity;
		private Schema lokSchema;

		// private ShExStorageLibR xlR;

	#endregion

	#region ctor

		public ShExSchemaLibR() { }

	#endregion

	#region public properties

		// private static readonly Lazy<ShExSchemaLibR<TShtKey, TShtFlds, TRowKey, TRowFlds, TRow>> instance =
		// 	new Lazy<ShExSchemaLibR<TShtKey, TShtFlds, TRowKey, TRowFlds, TRow>>(() => new ShExSchemaLibR<TShtKey, TShtFlds, TRowKey, TRowFlds, TRow>());
		//
		// public static ShExSchemaLibR<TShtKey, TShtFlds, TRowKey, TRowFlds, TRow> Instance => instance.Value;

		public ShExStorManagerR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>  smR { get; set; }

		public DataStorage Datastorage
		{
			get => datastorage;
			set
			{
				datastorage = value;
				OnPropertyChanged();
			}
		}


		// public TSht SheetData
		// {
		// 	get => smR.Sheet;
		// 	set { smR.Sheet = value; }
		// }

		public TLok LockData
		{
			get => lokd;
			set => lokd = value;
		}

		// public smR.Exid smR.Exid
		// {
		// 	get => smR.Exid;
		// 	set => smR.Exid = value;
		// }

		public bool Configured => (shtEntity != null);

	#endregion

	#region public methods

		public void EraseDataStorage()
		{
			Datastorage = null;

			EraseSheetSchema();
		}

		public void EraseSheetSchema()
		{
			shtEntity = null;
			shtSchema = null;

			EraseLockSchema();
		}

		public void EraseLockSchema()
		{
			lokEntity = null;
			lokSchema = null;
		}

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ShExSchemaLibR<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds>)}";
		}

	#endregion

	#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		[DebuggerStepThrough]
		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

	#region Save Data to Entity

	#region primary methods

// 00
		public ExStoreRtnCode SaveLock()
		{
			Msgs.WriteLineSpaced("step: 01|", ">>> start | transaction| save standard");

			if (lokd == null || smR.Exid == null) return ExStoreRtnCode.XRC_FAIL;

			ExStoreRtnCode result = ExStoreRtnCode.XRC_DS_NOT_FOUND;

			lokEntity = null;

			Transaction T;

			using (T = new Transaction(smR.Exid.Document, "Save Cells Lock Data"))
			{
				T.Start();
				{
					if (Datastorage != null)
					{
						// step A1
						saveLock();
						result = ExStoreRtnCode.XRC_GOOD;
					}
				}

				if (lokEntity == null)
				{
					T.RollBack();
				}
				else
				{
					Datastorage.SetEntity(lokEntity);
					T.Commit();
				}
			}
			return result;
		}

// 00

// todo: either use a provided datastore or create a new datastore


		/// <summary>
		/// save the sheet data and any row data to the model<br/>
		/// return:<br/>
		/// XRC_GOOD = worked / saved<br/>
		/// XRC_FAIL = did not work (could not create a datastorage element
		/// </summary>
		/// <returns></returns>

		public ExStoreRtnCode WriteSheet()
		{
			Msgs.WriteLineSpaced("step: 01|", ">>> start | transaction| save sheet");
			
			DataStorage ds;
			ExStoreRtnCode rtnCode;

			// if (smR.Sheet == null || smR.Exid == null) return ExStoreRtnCode.XRC_FAIL;
			//
			// rtnCode = smR.StorLibR.FindSheetDs(smR.Exid, out ds);
			//
			// if (rtnCode == ExStoreRtnCode.XRC_GOOD) return ExStoreRtnCode.XRC_DS_EXISTS;
			//
			// bool result = smR.StorLibR.DoesLockEntityExist(smR.Exid);
			//
			// if (result) return ExStoreRtnCode.XRC_LOCK_EXISTS;

			shtEntity = null;

			// at this point
			// sheet is not null
			// no existing ds
			// not locked

			using (Transaction T = new Transaction(smR.Exid.Document, "Save Cells Sheet Data"))
			{
				T.Start();
				{
					// conditions
					// 1. datastorage exists, entity exists, and schema does or does not exists.
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

					// DataStorage ds = null;

					rtnCode = smR.StorLibR.CreateDataStorage(smR.Exid, out ds);

					Datastorage = ds;
					
					if (rtnCode == ExStoreRtnCode.XRC_GOOD)
					{
						// step A1
						saveSheet();
					}
					else
					{
						rtnCode = ExStoreRtnCode.XRC_FAIL;
					}
				}

				if (shtEntity == null)
				{
					T.RollBack();
				}
				else
				{
					Datastorage.SetEntity(shtEntity);
					T.Commit();
				}
			}

			return rtnCode;
		}

	#endregion

	#region private methods - std schema

		// A1
		private void saveLock()
		{
			Msgs.WriteLineSpaced("step: A1|", ">>> start |primary sequence| save sheet");

			createLockSchema();

			writeLockData();
		}


// A3
		private void createLockSchema()
		{
			Msgs.WriteLineSpaced("step: A3|", ">>> start | create standard");

			lokSchema = null;

			// schema = makeLokSchema();

			SchemaBuilder sb = makeSheetPartialSchema(lokd);

			lokSchema = sb.Finish();

			lokEntity = new Entity(lokSchema);
		}

// WS
		private void writeLockData()
		{
			Msgs.WriteLineSpaced("step: WS|", ">>> start | write sheet data");

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


	#region private methods - shte schema

// A1
		private void saveSheet()
		{
			Msgs.WriteLineSpaced("step: A1|", ">>> start |primary sequence| save sheet");

			createSheetSchema();

			writeSheetData();
		}

// A3
		private void createSheetSchema()
		{
			Msgs.WriteLineSpaced("step: A3|", ">>> start | create sheet schema");

			shtSchema = null;

			// schema = makeSheetAndRowSchema();

			SchemaBuilder sb = makeSheetPartialSchema(smR.Sheet);

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
			where TF : IShScFieldData1<TK>, new()
		{
			Msgs.WriteLineSpaced("step: B2|", ">>> start | make the sheet portion of the schema");

			SchemaBuilder sb = new SchemaBuilder(flds.SchemaGuid);

			// C1
			configSchemaParams(sb, flds.SchemaName, flds.SchemaDesc, smR.Exid.VendorId);

			// D1
			addSchemaFields(sb, flds.Fields);

			return sb;
		}

	#endregion

	#region private methods - common

// C1
		private void configSchemaParams(SchemaBuilder sb, string schemaName, string schemaDesc, string vendId)
		{
			Msgs.WriteLineSpaced("step: C1|", $"configure the schema");
			sb.SetReadAccessLevel(AccessLevel.Public);
			sb.SetWriteAccessLevel(AccessLevel.Public);

			sb.SetSchemaName(schemaName);
			sb.SetDocumentation(schemaDesc);
			sb.SetVendorId(vendId);
		}

// D1
		private void addSchemaFields<TKey, TFields>
			(SchemaBuilder sb, Dictionary<TKey, TFields> fields)
			where TKey : Enum
			where TFields : IShScFieldData1<TKey>, new()
		{
			Msgs.WriteLineSpaced("step: D1|", $"create each schema fields");
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
			Msgs.WriteLineSpaced("step: E1|", ">>> start | add all sub-schema fields");

			foreach (KeyValuePair<string, TRow> kvp in smR.Sheet.Rows)
			{
				addSubSchemaField(sb, kvp.Value);
			}
		}

// E2
		private void addSubSchemaField(SchemaBuilder sb, TRow row)
		{
			Msgs.WriteLineSpaced("step: E2|", "add sub-schema field");

			FieldBuilder fb = sb.AddSimpleField(row.SchemaName, typeof(Entity));
			fb.SetDocumentation(row.SchemaDesc);
			fb.SetSubSchemaGUID(row.SchemaGuid);
		}

	#endregion

	#region private methods - row schema

// F1
		private void addAllRowSchemaFields()
		{
			foreach (KeyValuePair<string, TRow> kvp in smR.Sheet.Rows)
			{
				addRowSchema(kvp.Value);
			}
		}

// J1
		private void addRowSchema(AShScRow<TRowKey, TRowFlds> row)
		{
			Msgs.WriteLineSpaced("step: J1|", "create and add row schema and entity");

			Field f = shtSchema.GetField(row.SchemaName);

			Schema rowSchema = makeRowSchema(row);

			Entity rowE = new Entity(rowSchema);

			shtEntity.Set(f, rowE);
		}

// K1
		private Schema makeRowSchema(AShScRow<TRowKey, TRowFlds> row)
		{
			Msgs.WriteLineSpaced("step: K1|", ">>> start | create, configure, and make row schema fields");

			SchemaBuilder sb = new SchemaBuilder(row.SchemaGuid);

			// C1
			configSchemaParams(sb, row.SchemaName, row.SchemaDesc, smR.Exid.VendorId);

			// D1
			addSchemaFields(sb, row.Fields);

			return sb.Finish();
		}

	#endregion

	#region private methods - write data

// WT
		private void writeSheetData()
		{
			Msgs.WriteLineSpaced("step: WT|", ">>> start | write sheet data");

			writeData(shtEntity, shtSchema, smR.Sheet);

			writeRows(shtEntity, shtSchema, smR.Sheet.Rows);
		}

// WD
		private void writeData<TKey, TFld>(Entity e, Schema s, AShScFields<TKey, TFld> data)
			where TKey : Enum
			where TFld : IShScFieldData1<TKey>, new()
		{
			Msgs.WriteLineSpaced("step: WD|", "write each field's data");

			foreach (KeyValuePair<TKey, TFld> kvp in data.Fields)
			{
				writeField<TKey, TFld>(e, s, kvp.Value);
			}
		}

// WF
		private void writeField<TKey, TFld>(Entity e, Schema s, TFld field)
			where TKey : Enum
			where TFld : IShScFieldData1<TKey>, new()
		{
			Field f = s.GetField(field.FieldName);

			if (f == null || !f.IsValidObject) return;

			e.Set(f, field.DyValue.RevitValue);
		}

// WR
		private void writeRows(Entity e, Schema s, Dictionary<string, TRow> rows)
		{
			Msgs.WriteLineSpaced("step: WR|", ">>> start | process each row to write the data");

			foreach (KeyValuePair<string, TRow> kvp in rows)
			{
				writeRow(e, s, kvp.Value);
			}
		}

// Wr
		private void writeRow(Entity e, Schema S, TRow row)
		{
			Msgs.WriteLineSpaced("step: Wr|", ">>> start | process a row to write the data");

			Field f = S.GetField(row.SchemaName);

			Entity subE = e.Get<Entity>(row.SchemaName);

			writeData(subE, subE.Schema, row);

			e.Set(f, subE);
		}

	#endregion

	#endregion


	#region restore data

		public void ReadSheet(Entity e, ref TSht sheet)
		{
			// ExStoreRtnCode rtnCode;
			//
			// DataStorage ds;
			// Entity e;
			//
			// rtnCode = smR.StorLibR.FindEntity(smR.Exid, smR.Exid.ExsIdSheetSchemaName,
			// 	out ds, out e);
			//
			// if (rtnCode != ExStoreRtnCode.XRC_GOOD) return rtnCode;

			// smR.SheetDs = ds;
			// smR.SheetEntity = e;

			ReadData(e, sheet);

			readRowData(e, sheet);

			// return rtnCode;
		}

	#endregion



		private void readRowData(Entity e, TSht sheet)
		{
			List<Entity> subSchema = smR.StorLibR.getSubEntities(e);

			foreach (Entity ee in subSchema)
			{
				// M.WriteLine($"subschema name| {ee.Schema.SchemaName}");

				TRow row = new TRow();

				// ReadData(ee, row.Fields);
				ReadData(ee, row);


				sheet.AddRow(row);

				// M.NewLine();

			}
		}

		private void ReadData<TKey, TFlds>(Entity e, AShScFields<TKey, TFlds> data)
			where TKey : Enum
			where TFlds :  IShScFieldData1<TKey>, new()
		{
			foreach (KeyValuePair<TKey, TFlds> kvp in data.Fields)
			{
				Type t = kvp.Value.DyValue.TypeIs;

				if (t == typeof(string))
				{
					kvp.Value.SetValue = e.Get<string>(kvp.Value.FieldName);
				}
				else if (t == typeof(bool))
				{
					kvp.Value.SetValue = e.Get<bool>(kvp.Value.FieldName);
				}
				else if (t == typeof(Guid))
				{
					kvp.Value.SetValue = e.Get<Guid>(kvp.Value.FieldName);
				}
				else if (t.BaseType == typeof(Enum))
				{
					if (kvp.Value.FieldName.Equals(ScInfoMeta1.KEY_FIELD_NAME)) continue;

					string eName = e.Get<string>(kvp.Value.FieldName);
					data.ParseEnum(t, eName);
				}
				// else
				// {
				// 	M.WriteLine($"field not read|", $"{kvp.Value.FieldName} of type {t.Name}");
				// }
			}
		}

		
		// private void ReadData<TKey, TFlds>(Entity e, Dictionary<TKey, TFlds> fields)
		// 	where TKey : Enum
		// 	where TFlds :  IShScFieldData1<TKey>
		// {
		// 	foreach (KeyValuePair<TKey, TFlds> kvp in fields)
		// 	{
		// 		Type t = kvp.Value.DyValue.TypeIs;
		//
		// 		if (t == typeof(string))
		// 		{
		// 			kvp.Value.SetValue = e.Get<string>(kvp.Value.FieldName);
		// 		}
		// 		else if (t == typeof(bool))
		// 		{
		// 			kvp.Value.SetValue = e.Get<bool>(kvp.Value.FieldName);
		// 		}
		// 		else if (t == typeof(Guid))
		// 		{
		// 			kvp.Value.SetValue = e.Get<Guid>(kvp.Value.FieldName);
		// 		}
		// 		else if (t.BaseType == typeof(Enum))
		// 		{
		// 			string eName = (kvp.Value.FieldName);
		// 			M.WriteLine($"field not read|", $"enum| {kvp.Value.FieldName} of type| {t.Name} | and value of| {eName}");
		//
		// 		}
		// 		else
		// 		{
		// 			M.WriteLine($"field not read|", $"{kvp.Value.FieldName} of type {t.Name}");
		// 		}
		// 	}
		// }


	}
}