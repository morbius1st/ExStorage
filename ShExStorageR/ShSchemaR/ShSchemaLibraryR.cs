#region using directives

using Autodesk.Revit.DB.ExtensibleStorage;

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using RevitSupport;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields;
using ShExStorageR.ShExStorage;

#endregion


// projname: $projectname$
// itemname: ShSchemaLibraryR
// username: jeffs
// created:  11/8/2022 8:57:52 PM

/*

process

* MakeSchema 
	> runs the process
	> needs a data storage
	1. start by making the table schema (via MakeTableSchema)
		a. makes table schema fields
		b. makes the sub schema field - one for each row
			i. save the Guid in a dictionary keyed by the subschema's name
			ii. each is added to the table schema (via schema builder)
		c. save the table schema into -> tableSchema
	2. convert the schema into an entity
		a. saves the entity -> tableEntity
	3. create the sub schemas and entities (configSubSchemaField) (and via makeSubSchema)
		a. each entity is set into the tableEntity
	4. the table entity is set into the datastoage



*/

namespace ShExStorageR.ShSchemaR
{
	public class ShSchemaLibraryR
	{
	#region private fields

		private static readonly Lazy<ShSchemaLibraryR> instance =
			new Lazy<ShSchemaLibraryR>(() => new ShSchemaLibraryR());


		private ExStorageAdmin da = ExStorageAdmin.Instance;



		private Dictionary<string, Guid> subSchema;

		private Entity tableEntity;
		private Schema tableSchema;
		private DataStorage dataStorage;

		private bool gotDataStorage;

	#endregion

	#region ctor

		private ShSchemaLibraryR() { }

	#endregion

	#region public properties

		public static ShSchemaLibraryR Instance => instance.Value;

	#endregion


	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ShSchemaLibraryR)}";
		}

	#endregion


		
	#region Schema

// begin
		public ExStoreRtnCode WriteTable<TTblKey, TTblFlds, TRowKey, TRowFlds, TRow>
			(ExId exid,
			AShScTable<TTblKey, TTblFlds, TRowKey, TRowFlds, TRow> tbld)
			where TTblKey : Enum
			where TTblFlds : IShScFieldData1<TTblKey>, new()
			where TRowKey : Enum
			where TRowFlds : IShScFieldData1<TRowKey>, new()
			where TRow : AShScRow<TRowKey, TRowFlds>, new()
		{
			ExStoreRtnCode result;

			Transaction T;

			using (T = new Transaction(exid.Document, "Save Cells Data to ExStorage"))
			{
				T.Start();

				DataStorage ds = da.CreateDataStorage(exid.Document, exid.ExsIdTable);

				result = writeTable(exid, tbld, ds);

				if (result == ExStoreRtnCode.XRC_GOOD)
				{
					T.Commit();
				}
				else
				{
					T.RollBack();
				}
			}

			return result;
		}

// A
		private ExStoreRtnCode writeTable<TTblKey, TTblFlds, TRowKey, TRowFlds, TRow>
			(ExId exid, 
			AShScTable<TTblKey, TTblFlds, TRowKey, TRowFlds, TRow> tbld,
			DataStorage ds)
			where TTblKey : Enum
			where TTblFlds : IShScFieldData1<TTblKey>, new()
			where TRowKey : Enum
			where TRowFlds : IShScFieldData1<TRowKey>, new()
			where TRow : AShScRow<TRowKey, TRowFlds>, new()
		{
			ExStoreRtnCode result = ExStoreRtnCode.XRC_GOOD;

			try
			{
				result = MakeTableSchema(exid, tbld);

				if (result != ExStoreRtnCode.XRC_GOOD) return ExStoreRtnCode.XRC_FAIL;

				tableEntity = new Entity(tableSchema);

				foreach (KeyValuePair<string, TRow> kvp in tbld.Rows)
				{
					configSubSchemaField(exid, kvp.Value);
				}

				WriteData(tableEntity, tableSchema, tbld);

				// WriteRows();

				dataStorage.SetEntity(tableEntity);

			}
			catch (InvalidOperationException ex)
			{
				result = ExStoreRtnCode.XRC_FAIL;

				if (ex.HResult == -2146233088)
				{
					result = ExStoreRtnCode.XRC_DUPLICATE;
				}
			}

			return result;
		}


// P
		public ExStoreRtnCode WriteData<TKey, TFlds>(
			Entity e, Schema s,
			AShScFields<TKey, TFlds> data)
			where TKey : Enum
			where TFlds : IShScFieldData1<TKey>, new()
		{
			ExStoreRtnCode result = ExStoreRtnCode.XRC_GOOD;

			foreach (KeyValuePair<TKey, TFlds> kvp in data.Fields)
			{
				Field f = s.GetField(kvp.Value.FieldName);

				if (f == null || !f.IsValidObject) continue;

				e.Set(f, kvp.Value.DyValue.Value);
			}

			return result;
		}

// Q1
		private ExStoreRtnCode WriteRows<TRowKey, TRowFlds, TRow>(
			Entity e, Schema s, Dictionary<string, TRow> rows)
			where TRowKey : Enum
			where TRowFlds : IShScFieldData1<TRowKey>, new()
			where TRow : AShScRow<TRowKey, TRowFlds>, new()
		{
			ExStoreRtnCode result = ExStoreRtnCode.XRC_GOOD;


			foreach (KeyValuePair<string, TRow> kvp in rows)
			{
				
			}


			return result;
		}


// Q2 (combine / replace)
		public ExStoreRtnCode WriteRow<TRowKey, TRowFlds>(
			AShScRow<TRowKey, TRowFlds> row)
			where TRowKey : Enum
			where TRowFlds : IShScFieldData1<TRowKey>, new()
		{
			ExStoreRtnCode result = ExStoreRtnCode.XRC_GOOD;



			return result;
		}

// B
		public ExStoreRtnCode MakeTableSchema<TTblKey, TTblFlds, TRowKey, TRowFlds, TRow>
			(ExId exid, AShScTable<TTblKey, TTblFlds, TRowKey, TRowFlds, TRow> tbld)
			where TTblKey : Enum
			where TTblFlds : IShScFieldData1<TTblKey>, new()
			where TRowKey : Enum
			where TRowFlds : IShScFieldData1<TRowKey>, new()
			where TRow : AShScRow<TRowKey, TRowFlds>, new()
		{
			if (tbld == null || tbld.Fields == null || tbld.Fields.Count < 1)
				return ExStoreRtnCode.XRC_FAIL;

			tableSchema = null;

			try
			{
				SchemaBuilder sb = new SchemaBuilder(tbld.SchemaGuid);

				// assign the basic schema settings
				configSchema(ref sb, tbld.SchemaName, tbld.SchemaDesc, exid.VendorId);

				// make the top level schema fields
				makeSchemaFields(ref sb, tbld.Fields);

				// make the sub-schema fields
				makeSubSchemaFields(ref sb, exid, tbld);

				tableSchema = sb.Finish();
			}
			catch
			{
				return ExStoreRtnCode.XRC_FAIL;
			}

			return ExStoreRtnCode.XRC_GOOD;
		}


// C
		// assign the basic schema settings
		private void configSchema(ref SchemaBuilder sb,
			string schemaName, string schemaDesc, string vendId)
		{
			sb.SetReadAccessLevel(AccessLevel.Public);
			sb.SetWriteAccessLevel(AccessLevel.Public);

			sb.SetSchemaName(schemaName);
			sb.SetDocumentation(schemaDesc);
			sb.SetVendorId(vendId);
		}

// D1
		// run through the list and make a schema field for each one
		private void makeSchemaFields<TKey, TFields>
			(ref SchemaBuilder sb, Dictionary<TKey, TFields> fields)
			where TKey : Enum
			where TFields : IShScFieldData1<TKey>, new()
		{
			foreach (KeyValuePair<TKey, TFields> kvp in fields)
			{
				makeSchemaField(ref sb,  kvp.Value);
			}
		}

// D2  (combine)
		// create and assign one schema field
		private void makeSchemaField<TTblKey>(ref SchemaBuilder sb, 
			IShScFieldData1<TTblKey> field)
			where TTblKey : Enum
		{
			FieldBuilder fb = sb.AddSimpleField(field.FieldName, field.DyValue.TypeIs);
			fb.SetDocumentation(field.FieldDesc);

			// fb.SetSpec(field.DyValue.GetRevitSpecIdCustom());
		}


// E
		/// <summary>
		/// add a field in the root schema for each sub-schema<br/>
		/// this schema field will hold an entity which is a root<br/>
		/// holder for a sub-schema and placed an entry into<br/>
		/// a dictionary of sub-schema's and their associated Guid's
		/// </summary>
		/// <param name="sb">the root schema builder</param>
		/// <param name="exid">the ex store Id object</param>
		/// <param name="tbld">the table which holds all of the data</param>
		private void makeSubSchemaFields<TTblKey, TTblFlds, TRowKey, TRowFlds, TRow>
			(ref SchemaBuilder sb,
			ExId exid,
			AShScTable<TTblKey, TTblFlds, TRowKey, TRowFlds, TRow> tbld)
			where TTblKey : Enum
			where TTblFlds : IShScFieldData1<TTblKey>, new()
			where TRowKey : Enum
			where TRowFlds : IShScFieldData1<TRowKey>, new()
			where TRow : AShScRow<TRowKey, TRowFlds>, new()
		{
			subSchema = new Dictionary<string, Guid>(tbld.Rows.Count);
			

			foreach (KeyValuePair<string, TRow> kvp in tbld.Rows)
			{
				string subSchemaName =
					exid.ExIdRow(kvp.Value.RowSuffix);


				subSchema.Add(subSchemaName, kvp.Value.SchemaGuid);

				FieldBuilder fb = sb.AddSimpleField(subSchemaName, typeof(Entity));

				fb.SetDocumentation(kvp.Value.SchemaDesc);
				fb.SetSubSchemaGUID(kvp.Value.SchemaGuid);
			}
		}

// J
		private void configSubSchemaField<TRowKey, TRowFlds>
			(ExId exid,
			AShScRow<TRowKey, TRowFlds> row)

			where TRowKey : Enum
			where TRowFlds : IShScFieldData1<TRowKey>, new()
		{
			foreach (KeyValuePair<string, Guid> kvp in subSchema)
			{
				Field f = tableSchema.GetField(kvp.Key);

				Schema subSchema = makeSubSchema(exid, row);

				Entity subE = new Entity(subSchema);

				tableEntity.Set(f, subE);
			}
		}


// K
		private Schema makeSubSchema<TRowKey, TRowFlds>(
			ExId exid, 
			AShScRow<TRowKey, TRowFlds> row)
			where TRowKey : Enum
			where TRowFlds : IShScFieldData1<TRowKey>, new()
		{
			SchemaBuilder sb = new SchemaBuilder(row.SchemaGuid);

			configSchema(ref sb, row.SchemaName, row.SchemaDesc, exid.VendorId);
			makeSchemaFields(ref sb, row.Fields);

			return sb.Finish();
		}


	#endregion


	}
}