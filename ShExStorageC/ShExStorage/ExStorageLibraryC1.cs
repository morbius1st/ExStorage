#region using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;
using RevitSupport;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExConst;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaRowKey;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaSheetKey;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaLockKey;


using Autodesk.Revit.DB.ExtensibleStorage;
using ShExStorageC.ShExStorage;
using ShStudy;
using ShStudy.ShEval;

#endregion

// username: jeffs
// created:  10/22/2022 11:08:15 AM

namespace ShExStorageC.ShExStorage
{

	/// <summary>
	/// this has data storage test routines only
	/// </summary>
	public class ExStorageLibraryC1<TTbl, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLockKey, TLockFlds>
		where TTbl : AShScSheet<TShtKey, TShtFlds, TRowKey, TRowFlds, TRow>, new()
		where TLok : AShScFields<TLockKey, TLockFlds>, new()
		where TShtKey : Enum
		where TShtFlds : IShScFieldData1<TShtKey>, new()
		where TRowKey : Enum
		where TRowFlds : IShScFieldData1<TRowKey>, new()
		where TRow : AShScRow<TRowKey, TRowFlds>, new()
		where TLockKey : Enum
		where TLockFlds : IShScFieldData1<TLockKey>, new()
	{
	#region private fields

		// private ExStorageAdmin da = new ExStorageAdmin();

		private ShDebugMessages M { get; set; }

		private TTbl shtd;
		private TRow rowd;
		private TLok lokd;
		// private ExId1 exid;
		private Entity entity;
		private Schema schema;
		private DataStorage dataStorage;

		private string currSchema;

	#endregion

	#region ctor

		public ExStorageLibraryC1(ShDebugMessages msgs)
		{
			M = msgs;
		}

	#endregion

	#region public properties

		// public TT SheetData
		// {
		// 	get => shtd;
		// 	set
		// 	{
		// 		shtd = value;
		// 	}
		// }

		// public AShScFields<TKey, TFlds> FieldData
		// {
		// 	get => flds;
		// 	set => flds = value;
		// }


		// public ExId1 Exid
		// {
		// 	get => exid;
		// 	set => exid = value;
		// }

	#endregion

	#region private properties

	#endregion

	#region private methods 1

	#endregion

	#region public methods

			public ExStoreRtnCode CreateDataStorage(Document doc, out DataStorage ds)
			{
				ds = DataStorage.Create(doc);

				return ExStoreRtnCode.XRC_GOOD;
			}

	#endregion

	#region private methods

	#endregion

	#region event consuming

	#endregion

	#region event publishing

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ExStorageLibraryC1<TTbl, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLockKey, TLockFlds>)}";
		}

	#endregion


		public void SaveLock(DataStorage ds)
		{
			ExStoreRtnCode result;
			entity = null;
			
			// using
			{
				dataStorage = ds;

				saveLock();

				dataStorage.SetEntity(entity);
			}
		}

		private void saveLock()
		{
			createLock();

			writeLockData();
		}

		// A3 lock
		private void createLock()
		{
			schema = makeLockSchema();

			entity = new Entity(schema);
		}

		private void writeLockData()
		{
			writeData(entity, schema, lokd);
		}


// Begin
// 00
		// process the sheet data to create the schema, make the fields, and save the data
		public void SaveSheet()
		{
			M.WriteLineSteps("step: 01|", ">>> start | main process");
			M.MarginUp();

			ExStoreRtnCode result;
			entity = null;

			// using 
			{
				M.WriteLineSteps("step: 02|", "create data storage");

				result = CreateDataStorage(AExId.Document, out dataStorage);

				if (result == ExStoreRtnCode.XRC_GOOD)
				{
					M.WriteLineSteps("step: 03|", "write sheet");
					// A
					saveSheet();
				}

				M.WriteLineSteps("step: 04|", $"add / overwrite sheet entity to data storage");
				dataStorage.SetEntity(entity);
			}
			// end using

			M.MarginDn();
			M.WriteLineSteps("step: 01|", ">>> end | main process");
		}

// A1
		// create the sheet schema, then the entity, then the sub schemas, then write the data
		private void saveSheet()
		{
			M.WriteLineSteps("step: A1|", ">>> start |primary sequence");
			M.MarginUp();
			currSchema = "SHEET";

			createSheet();

			writeSheetData();

			M.MarginDn();
			M.WriteLineSteps("step: A1|", ">>> end | primary sequence");
		}

		/*
		// create schema and entities
		*/



// A3
		private void createSheet()
		{
			M.WriteLineSteps("step: A3|", ">>> start | create sheet schema add entity");
			M.MarginUp();

			schema = makeSheetAndRowSchema();

			entity = new Entity(schema);

			addAllRowSchemaFields();

			M.MarginDn();
			M.WriteLineSteps("step: A3|", ">>> end | create sheet schema add entity");
		}



// B1
		// run the schema building process - create the sheetSchema
		private Schema makeSheetAndRowSchema()
		{
			M.WriteLineSteps("step: B1|", ">>> start | make the sheet schemas");
			M.MarginUp();

			schema = null;
			SchemaBuilder sb = makePartialSchema(shtd);

			// E1
			addAllSubSchemaFields(ref sb);

			M.MarginDn();
			M.WriteLineSteps("step: B1|", ">>> end | make the sheet schemas");
			
			return sb.Finish();
		}

// B1 lock
		private Schema makeLockSchema()
		{
			schema = null;
			SchemaBuilder sb = makePartialSchema(lokd);

			return sb.Finish();
		}


// B2
		private SchemaBuilder makePartialSchema<T1, T2>(AShScFields<T1, T2> f)
			where T1 : Enum
			where T2 : IShScFieldData1<T1>, new()
		{
			M.WriteLineSteps("step: B2|", ">>> start | make the sheet portion of the schema");
			M.MarginUp();

			schema = null;

			SchemaBuilder sb = new SchemaBuilder(f.SchemaGuid);

			// C1
			configSchemaParams(ref sb,  f.SchemaName, f.SchemaDesc, AExId.VendorId);

			// D1
			addSchemaFields(ref sb, f.Fields);

			M.MarginDn();
			M.WriteLineSteps("step: B2|", ">>> end | make the sheet portion of the schema");

			return sb;
		}

// C1
		// assign the schema settings
		private void configSchemaParams(ref SchemaBuilder sb, 
			string schemaName, string schemaDesc, string vendId)
		{
			M.WriteLineSteps("step: C1|", $"configure the schema| {currSchema}");
		}

// D1
		// create a schema field for each field
		private void addSchemaFields<TKey, TFields>
			(ref SchemaBuilder sb, Dictionary<TKey, TFields> fields)
			where TKey : Enum
			where TFields : IShScFieldData1<TKey>, new()
		{
			M.WriteLineSteps("step: D1|", $"create each schema fields| {currSchema}");


			foreach (KeyValuePair<TKey, TFields> kvp in fields)
			{
				FieldBuilder fb = sb.AddSimpleField(kvp.Value.FieldName, kvp.Value.DyValue.RevitTypeIs);
				// fb.SetDocumentation(kvp.Value.FieldDesc);
			}
		}

// E1
		private void addAllSubSchemaFields(ref SchemaBuilder sb)
		{
			M.WriteLineSteps("step: E1|", ">>> start | add all sub schema fields");
			M.MarginUp();
			foreach (KeyValuePair<string, TRow> kvp in shtd.Rows)
			{
				addSubSchemaField(ref sb, kvp.Value);
			}

			M.MarginDn();
			M.WriteLineSteps("step: E1|", ">>> end | add all sub-schema fields");

		}

// E2
		private void addSubSchemaField(ref SchemaBuilder sb, TRow row)
		{
			M.WriteLineSteps("step: E2|", "add sub-schema field");
			FieldBuilder fb = sb.AddSimpleField(row.SchemaName, typeof(Entity));
				
			// fb.SetDocumentation(kvp.Value.SchemaDesc);
			// fb.SetSubSchemaGUID(kvp.Value.SchemaGuid);
		}

// F1
		private void addAllRowSchemaFields()
		{
			M.WriteLineSteps("step: F1|", ">>> start | make all row schema");
			M.MarginUp();

			foreach (KeyValuePair<string, TRow> kvp in shtd.Rows)
			{
				// create the sub schema entity - J, K
				addRowSchema(kvp.Value);
			}

			M.MarginDn();
			M.WriteLineSteps("step: F1|", ">>> end | make all row schema");
		}

// J1
		// create a schema entity for each sub-schema - store in the sheet schema
		private void addRowSchema(AShScRow<TRowKey, TRowFlds> row)
		{
			currSchema = "ROW";
			M.WriteLineSteps("step: J1|", "create a row fields and entity");
			M.MarginUp();

			Field f = schema.GetField(row.SchemaName);

			// K
			Schema subSchema = makeRowSchema(row);

			M.WriteLineSteps("step: J2|", $"create row entity| {currSchema}");
			Entity subE = new Entity(subSchema);

			entity.Set(f, subE);

			M.MarginDn();
		}

// K1
		// create the sub-schema entity
		private Schema makeRowSchema(AShScRow<TRowKey, TRowFlds> row)

		{
			M.WriteLineSteps("step: K1|", ">>> start | create, configure, and make row fields");
			M.MarginUp();

			SchemaBuilder sb = new SchemaBuilder(row.SchemaGuid);

			// C
			configSchemaParams(ref sb, row.SchemaName, row.SchemaDesc, AExId.VendorId);

			// D
			addSchemaFields(ref sb, row.Fields);

			M.MarginDn();
			M.WriteLineSteps("step: K1|", ">>> end | create, configure, and make row fields");

			return sb.Finish();
		}


		//
		// writedata
		//

// WT
		private void writeSheetData()
		{
			M.WriteLineSteps("step: WT|", ">>> start | write all data");
			M.MarginUp();
			// P
			writeData(entity, schema, shtd);

			// Q
			writeRows(entity, schema, shtd.Rows);

			M.MarginDn();
			M.WriteLineSteps("step: WT|", ">>> end | write all data");
		}


// WD
		// write the actual value for each field into the entity
		private void writeData<TKey, TFlds>(Entity e, Schema s, AShScFields<TKey, TFlds> data)
			where TKey : Enum
			where TFlds : IShScFieldData1<TKey>, new()
		{
			M.WriteLineSteps("step: WD|", "write each field");
			M.MarginUp();

			foreach (KeyValuePair<TKey, TFlds> kvp in data.Fields)
			{
				writeField<TKey, TFlds>(e, s, kvp.Value);
			}

			M.MarginDn();
		}

// WF
		private void writeField<TKey, TFlds>(Entity e, Schema s, TFlds field)
			where TKey : Enum
			where TFlds : IShScFieldData1<TKey>, new()
		{
			Field f = s.GetField(field.FieldName);

			if (f == null || !f.IsValidObject) return;

			e.Set(f, field.DyValue.Value);
		}


// WR
		// create the entity and write the row data for all of the rows
		private void writeRows(Entity e, Schema s, Dictionary<string, TRow> rows)

		{
			M.WriteLineSteps("step: WR|", ">>> start | process each row to write the data");
			M.MarginUp();

			foreach (KeyValuePair<string, TRow> kvp in rows)
			{
				writeRow(e, s, kvp.Value);
			}

			M.MarginDn();
			M.WriteLineSteps("step: WR|", ">>> end | process each row to write the data");
		}


// Wr
		private void writeRow(Entity e, Schema s, TRow row)
		{
			M.WriteLineSteps("step: Wr|", ">>> start | process a row to write the data");
			M.MarginUp();

			Field f = s.GetField(row.SchemaName);

			Entity subE = e.Get<Entity>(row.SchemaName);

			// P
			writeData(subE, subE.Schema, row);

			e.Set(f, subE);

			M.MarginDn();
			M.WriteLineSteps("step: Wr|", ">>> end | process each row to write the data");
		}


// G, H, I:  reserved
// M, N, O: reserved


		//
		// read 
		//

		public ExStoreRtnCode ReadSheet(ShtExId exid, Entity e,
			out TTbl sheet)
		{
			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_GOOD;

			sheet = new TTbl();

			// foreach (KeyValuePair<TShtKey, TShtFlds> kvp in sheet.Fields)
			// {
			// 	Type t = kvp.Value.DyValue.TypeIs;
			//
			// 	if (t == typeof(string))
			// 	{
			// 		kvp.Value.SetValue = e.Get<string>(kvp.Value.FieldName);
			// 	}
			// 	else if (t == typeof(bool))
			// 	{
			// 		kvp.Value.SetValue = e.Get<bool>(kvp.Value.FieldName);
			// 	}
			// 	else if (t == typeof(Guid))
			// 	{
			// 		kvp.Value.SetValue = e.Get<Guid>(kvp.Value.FieldName);
			// 	}
			// }

			return rtnCode;


		} 





	}
}