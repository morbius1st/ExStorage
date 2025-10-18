using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStoreTest2026;
using ExStoreTest2026.DebugAssist;
using RevitLibrary;
using static ExStorSys.WorkBookFieldKeys;

// username: jeffs
// created:  9/17/2025 9:22:50 PM

namespace ExStorSys
{
	public class ExStorLib
	{
	#region private fields

		public int ObjectId;

		private ExStorMgr xMgr => ExStorMgr.Instance;

	#endregion

	#region ctor

		private static readonly Lazy<ExStorLib> instance =
			new Lazy<ExStorLib>(() => new ExStorLib());

		private ExStorLib()
		{
			ObjectId = AppRibbon.ObjectIdx++;
		}

		public static ExStorLib Instance => instance.Value;

	#endregion

	#region public properties

		// public ExStorMgr ExMgr
		// {
		// 	set => xMgr = value;
		// }

	#endregion

	#region private properties

	#endregion

	#region methods

		/* system routines - workbook */

		public ExStoreRtnCode WriteWorkBook()
		{
			Msgs.WriteLine($"\t+++ L1 got ds? {ExStorMgr.Instance.GotWbkDs} | is empty {ExStorMgr.Instance.IsWorkBookEmpty}  [{ExStorMgr.Instance.WorkBook?.IsEmpty}]");
			WorkBook wbk = xMgr.WorkBook;

			Msgs.WriteLineSpaced("step: 01|", ">>> start | transaction| save workbook");
			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_FAIL;

			Msgs.WriteLine($"\t+++ L2 got ds? {ExStorMgr.Instance.GotWbkDs} | is empty {ExStorMgr.Instance.IsWorkBookEmpty}  [{ExStorMgr.Instance.WorkBook?.IsEmpty}]");

			if (xMgr.GotWbkDs) return ExStoreRtnCode.XRC_DS_EXISTS;

			Msgs.WriteLine($"\t+++ L3 got ds? {ExStorMgr.Instance.GotWbkDs} | is empty {ExStorMgr.Instance.IsWorkBookEmpty}  [{ExStorMgr.Instance.WorkBook?.IsEmpty}]");

			using (Transaction T = new Transaction(R.RvtDoc, "Store Cells Wbk Data"))
			{
				T.Start();
				{
					if (createDs(wbk) == ExStoreRtnCode.XRC_GOOD)
					{
						// make schema, set name and desc, save to workbook
						wbk.ExsSchema = makeSchema(wbk);
						wbk.ExsEntity = new Entity(wbk.ExsSchema!);

						if (wbk.GotEntity)
						{
							writeDataFields(wbk);
							rtnCode = ExStoreRtnCode.XRC_GOOD;
						}
					}

					if (rtnCode == ExStoreRtnCode.XRC_GOOD)
					{
						wbk.ExsDataStorage!.SetEntity(wbk.ExsEntity);
						T.Commit();
					}
					else
					{
						T.RollBack();
					}
				}
			}

			return rtnCode;
		}

		/* system routines - sheet */

		public ExStoreRtnCode WriteSheet(string dsName)
		{
			Sheet sheet = xMgr.Sheets[dsName];

			Msgs.WriteLineSpaced("step: 01|", ">>> start | transaction| save sheet");
			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_FAIL;

			if (xMgr.GotShtDs(dsName)) return ExStoreRtnCode.XRC_DS_EXISTS;

			using (Transaction T = new Transaction(R.RvtDoc, "Store Cells Sht Data"))
			{
				T.Start();
				{
					if (createDs(sheet) == ExStoreRtnCode.XRC_GOOD)
					{
						if (sheet.ExsSchema == null)
						{
							sheet.ExsSchema = makeSchema(sheet);
						}

						sheet.ExsEntity = new Entity(sheet.ExsSchema!);

						if (sheet.GotEntity)
						{
							writeDataFields(sheet);
							rtnCode = ExStoreRtnCode.XRC_GOOD;
						}
					}

					if (rtnCode == ExStoreRtnCode.XRC_GOOD)
					{
						sheet.ExsDataStorage!.SetEntity(sheet.ExsEntity);
						T.Commit();
					}
					else
					{
						T.RollBack();
					}
				}
			}

			return rtnCode;
		}

		// public ExStoreRtnCode ReadSheet(DataStorage ds, Schema s, out Sheet sht)
		// {
		// 	ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_GOOD;
		//
		// 	Entity e = ds.GetEntity(s);
		//
		// 	sht = Sheet.CreateEmptySheet(ds);
		// 	sht.ExMgr = ExStorMgr.Instance;
		//
		// 	sht.SetReadStart();
		//
		// 	if (ReadFields(e, sht) != ExStoreRtnCode.XRC_GOOD) return ExStoreRtnCode.XRC_FAIL;
		//
		// 	sht.ExsEntity = e;
		// 	sht.ExsSchema = s;
		//
		// 	sht.SetReadComplete();
		//
		// 	return rtnCode;
		// }

		/* data storage */

		public ExStoreRtnCode FindSheetsDs(WorkBook wbk, out IList<DataStorage> dsList)
		{
			string searchName = xMgr.Exid.ShtDsSearchNameModelSpecific;

			dsList = FindAllDataStorageByNamePrefix(searchName);

			return ExStoreRtnCode.XRC_GOOD;
		}

		/// <summary>
		/// create a DataStorage object for the associated exo object.  the DS object
		/// must not already exist - check first
		/// </summary>
		private ExStoreRtnCode createDs<Te>(ExStorDataObj<Te> exo)
			where Te : Enum
		{
			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_GOOD;

			try
			{
				exo.ExsDataStorage = DataStorage.Create(R.RvtDoc);
				exo.ExsDataStorage.Name = exo.DsName;
			}
			catch
			{
				rtnCode = ExStoreRtnCode.XRC_FAIL;
			}

			return rtnCode;
		}


		/* schema */

		private Schema makeSheetSchema<Te>(ExStorDataObj<Te> exo)
			where Te : Enum
		{
			Schema s = makeSchema(exo);


			return s;
		}

		private Schema makeSchema<Te>(ExStorDataObj<Te> exo)
			where Te : Enum
		{
			SchemaBuilder sb = new (exo.SchemaGuid);

			configSchemaParams(sb, exo);

			addSchemaFields(sb, exo.Rows);

			return sb.Finish();
		}

		private void configSchemaParams<Te>(SchemaBuilder sb, ExStorDataObj<Te> exo)
			where Te : Enum
		{
			sb.SetReadAccessLevel(AccessLevel.Public);
			sb.SetWriteAccessLevel(AccessLevel.Public);

			sb.SetSchemaName(exo.SchemaName);
			sb.SetDocumentation(exo.SchemaDesc);
		}

		private void addSchemaFields<Te>(SchemaBuilder sb, Dictionary<Te, FieldData<Te>> rows)
			where Te : Enum
		{
			Msgs.WriteLineSpaced("step: S2|", "write schema fields");

			foreach ((Te key, FieldData<Te> wkbkField) in rows)
			{
				addSchemaField(sb, wkbkField);
			}

			Msgs.WriteLine(" done");
		}

		private void addSchemaField<Te>(SchemaBuilder sb, FieldData<Te> fd)
			where Te : Enum
		{
			Msgs.Write(".");

			FieldBuilder fb;

			FieldDef<Te> f = fd.Field;
			DynaValue d = fd.DyValue!;

			if (d.IsDictStringString)
			{
				fb = sb.AddMapField(f.FieldName, d.RevitGenericArg0TypeIs, d.RevitGenericArg1TypeIs);
			}
			else if (d.IsListString)
			{
				fb = sb.AddArrayField(f.FieldName, d.RevitGenericArg0TypeIs);
			}
			else
			{
				fb = sb.AddSimpleField(f.FieldName, d!.RevitTypeIs);
			}

			fb.SetDocumentation(f.FieldDesc);
		}

		/* delete */

		/// <summary>
		/// delete a datastorage object - when done' it is gone
		/// </summary>
		public ExStoreRtnCode DeleteDs(DataStorage ds)
		{
			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_GOOD;

			using (Transaction T = new Transaction(R.RvtDoc, "Delete a DataStorage"))
			{
				T.Start();
				{
					try
					{
						R.RvtDoc.Delete(ds.Id);
						T.Commit();
					}
					catch 
					{
						T.RollBack();
						rtnCode = ExStoreRtnCode.XRC_FAIL;
					}
				}
			}

			return rtnCode;
		}

		/// <summary>
		/// erase a schema and all its entities.  when it is deleted, a schema<br/>
		/// stays in memory until the document is closed.  it cannot be reused<br/>
		/// </summary>
		public ExStoreRtnCode EraseSc(Schema s)
		{
			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_GOOD;

			using (Transaction T = new Transaction(R.RvtDoc, "Erase a Schema"))
			{
				T.Start();
				{
					try
					{
						R.RvtDoc.EraseSchemaAndAllEntities(s);
						s.Dispose();
						T.Commit();
					}
					catch 
					{
						T.RollBack();
						rtnCode = ExStoreRtnCode.XRC_FAIL;
					}
				}
			}

			return rtnCode;
		}


		/* write data*/

		private void writeDataFields<Te>(ExStorDataObj<Te> exo)
			where Te : Enum
		{
			Msgs.WriteLineSpaced("step: WD|", ">>> start | write data");

			foreach ((Te? key, FieldData<Te> data) in exo.Rows)
			{
				writeDataField(exo, data);
			}

			Msgs.WriteLine(" done");
		}

		private void writeDataField<Te>(ExStorDataObj<Te> exo, FieldData<Te> fd)
			where Te : Enum
		{
			Msgs.Write(".");
			Field fx = exo.ExsSchema!.GetField(fd.Field.FieldName);

			if (fx == null || !fx.IsValidObject) return;

			DynaValue d = fd.DyValue!;

			if (d.IsDictStringString)
			{
				exo.ExsEntity!.Set<IDictionary<string, string>>(fx, d.RevitValue);
			}
			else if (d.IsListString)
			{
				exo.ExsEntity!.Set<IList<string>>(fx, d.RevitValue);
			}
			else
			{
				exo.ExsEntity!.Set(fx, d.RevitValue);
			}
		}


		/* update field */

		public void UpdateField<Te>(Te key, ExStorDataObj<Te> exo, DynaValue value)
			where Te : Enum
		{
			FieldData<Te> fd = exo.Rows[key];

			UpdateField(exo, fd, value);
		}

		public void UpdateField<Te, Tk>(ExStorDataObj<Te> exo, FieldData<Tk>  field, DynaValue value)
			where Te : Enum
			where Tk : Enum
		{
			Schema s = exo.ExsSchema!;
			Entity e = exo.ExsEntity!;
			DataStorage ds = exo.ExsDataStorage;

			Field f = s.GetField(field.Field.FieldName);

			if (f == null || !f.IsValidObject) return;

			if (field.DyValue.IsDictStringString)
			{
				e.Set<IDictionary<string, string>>(f, value.RevitValue);
			}
			else if (field.DyValue.IsListString)
			{
				e.Set<IList<string>>(f, value.RevitValue);
			}
			else
			{
				e.Set(f, value.RevitValue);
			}

			UpdateEntity(ds!, e);
		}
		
		public void UpdateEntity(DataStorage ds, Entity e)
		{
			using (Transaction T = new Transaction(R.RvtDoc, "Save Cells Sheet Data"))
			{
				T.Start();
				{
					try
					{
						ds.SetEntity(e);
						T.Commit();
					}
					catch (Exception exception)
					{
						T.RollBack();
					}
				}
			}
		}



		/* read */

		public ExStoreRtnCode ReadFields<Te>(Entity e, ExStorDataObj<Te> exo)
			where Te : Enum
		{
			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_GOOD;
			
			foreach ((Te? key, FieldData<Te> fd) in exo.Rows)
			{
				if (!exo.UpdateRow(key, ReadField(e, fd)))
				{
					rtnCode = ExStoreRtnCode.XRC_FAIL;
					break;
				}
			}

			return rtnCode;
		}

		public DynaValue? ReadField<TKeyType>(Entity e, FieldData<TKeyType>  data)
			where TKeyType : Enum
		{
			// data is the field definition and the DV within contains the definition 
			// information

			DynaValue? dv = null;

			Type t = data.DyValue.TypeIs;

			if (t == typeof(string))
			{
				dv = new DynaValue(e.Get<string>(data.Field.FieldName));
			}
			else if (t == typeof(bool))
			{
				dv = new DynaValue(e.Get<bool>(data.Field.FieldName));
			}
			else if (t == typeof(Guid))
			{
				dv = new DynaValue(e.Get<Guid>(data.Field.FieldName));
			}
			else if (t == typeof(Dictionary<string, string>))
			{
				dv = new DynaValue(e.Get<IDictionary<string, string>>(data.Field.FieldName));
			}
			else if (t == typeof(List<string>))
			{
				dv = new DynaValue(e.Get<IList<string>>(data.Field.FieldName));
			}
			else if (t.BaseType == typeof(Enum))
			{
				if (data.Field.FieldName.Equals(Fields.KEY_DS_NAME)) return dv;

				string eName = e.Get<string>(data.Field.FieldName);
				dv = ParseEnum(t, eName);
			}

			return dv;
		}

		public DynaValue? ParseEnum(Type t, string enumName)
		{
			DynaValue? dv = null;

			if (t == typeof(UpdateRules))
			{
				UpdateRules k;
				bool result = Enum.TryParse(enumName, out k);
				if (!result) k = UpdateRules.UR_NEVER;
				dv = new DynaValue(k);
			}

			return dv;
		}


		/* find */

		public ExStoreRtnCode FindExStorInfo(int wbkOrSht, string schemaName, string searchText, string matchText,
			out DataStorage? ds, out Entity? e, out Schema? s)
		{
			// wbkOrSht - 0 = wbk / 1 = sht
			// match text - for wbk == model name / for sht == model code
			// search text - for wbk == ds search name / for sht = actual ds name

			// this searches for specific information - use different method to find
			// a list of sheets

			// to do this
			// (for wbk)
			// first, get the schema as we have its whole name - fail if not found
			// next, based on the schema and the model name, get the DS
			// next, from the DS, get the entity
			// (for sht)
			// first, get the schema as we have its whole name - fail if not found
			// next, get the ds based on the name provided

			ds = null;
			e = null;
			s = null;

			ExStoreRtnCode rtnCode;

			// step 1, get schema
			rtnCode = FindSchema(schemaName, out s);

			if (rtnCode != ExStoreRtnCode.XRC_GOOD) return rtnCode;

			// step 2, got schema / get remainder
			// when wbk - must match model name
			// when sht = must match model codes

			rtnCode = findExInfo(wbkOrSht, searchText, matchText, s, out ds, out e);

			return rtnCode;
		}


		private ExStoreRtnCode findExInfo(int wbkOrSht, string searchText, string matchText, Schema? s, out DataStorage? ds, out Entity? en)
		{
			ds = null;
			en = null;

			DataStorage? d;
			Entity? e;

			string? modelName;

			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_DS_NOT_FOUND;

			FilteredElementCollector dataStorages
				= findDataStorages();

			if (dataStorages.IsValidObject)
			{
				foreach (Element el in dataStorages)
				{
					d = el as DataStorage;

					if (d != null)
					{
						if (wbkOrSht == 0)
						{
							if (!d.Name.StartsWith(searchText)) continue;
						}
						else
						{
							if (!d.Name.Equals(searchText)) continue;
						}

						e = d.GetEntity(s);

						bool b1 = e.IsValid();
						bool b2 = (e.IsValid() && (wbkOrSht == 1 ||
							(wbkOrSht == 0 && ValidateModelName(matchText, e, out modelName))));


						// wbkOrSht == 0 => getting workbook | wbkOrSht == 1 => getting sheet
						if (e.IsValid() && (wbkOrSht == 1 ||
							(wbkOrSht == 0 && ValidateModelName(matchText, e, out modelName)))
							)
						{
							ds = d;
							en = e;
							rtnCode = ExStoreRtnCode.XRC_GOOD;
							break;
						}
					}
				}
			}

			return rtnCode;
		}

		public ExStoreRtnCode FindSchema(string schemaName, out Schema? s)
		{
			s = null;
		
			IList<Schema> schemas = Schema.ListSchemas();
		
			if (schemas == null || schemas.Count == 0) return ExStoreRtnCode.XRC_SCHEMA_NONE_FOUND;
		
			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_SCHEMA_NOT_FOUND;
		
			foreach (Schema schema in schemas)
			{
				// for schema, the name provided is the whole name
				if (schema.SchemaName.Equals(schemaName))
				{
					s = schema;
					rtnCode = ExStoreRtnCode.XRC_GOOD;
					break;
				}
			}
		
			return rtnCode;
		}

		/// <summary>
		/// find all schema's that starts with the search name
		/// </summary>
		public ExStoreRtnCode FindAllSchema(string searchName, out IList<Schema>? schemas)
		{
			IList<Schema> scx = Schema.ListSchemas();

			schemas = null;

			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_SCHEMA_NOT_FOUND;
		
			if (scx == null || scx.Count == 0) return rtnCode;

			schemas = new List<Schema>();
		
			foreach (Schema schema in scx)
			{
				// for schema, the name provided is the whole name
				if (schema.SchemaName.StartsWith(searchName))
				{
					schemas.Add(schema);
					rtnCode = ExStoreRtnCode.XRC_GOOD;
				}
			}
		
			return rtnCode;
		}


		public bool ValidateModelName(string matchName, Entity e, out string? modelName)
		{
			modelName = ReadField(e, Fields.GetWbkFieldData(PK_MD_MODEL_NAME))?.Value;

			if (modelName == null) return false;

			return matchName.Equals(modelName);
		}

		/// <summary>
		/// collect all datastore elements
		/// </summary>
		/// <returns></returns>
		private FilteredElementCollector findDataStorages()
		{
			FilteredElementCollector collector
				= new (R.RvtDoc);

			FilteredElementCollector dataStorages =
				collector.OfClass(typeof(DataStorage));

			return dataStorages;
		}

		public IList<DataStorage> FindAllDataStorageByNamePrefix(string searchName)
		{
			IList<DataStorage> dsList = new List<DataStorage>();

			FilteredElementCollector dataStorages = findDataStorages();

			foreach (Element el in dataStorages)
			{
				if (el.Name.StartsWith(searchName))
				{
					dsList.Add((DataStorage) el);
				}
			}

			return dsList;
		}

	#endregion

	#region event consuming

	#endregion

	#region event publishing

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(ExStorLib)}";
		}

	#endregion


		/* saved */

		// private bool verifyDs(int wbkOrSht, string matchName, DataStorage? ds,  Entity e)
		// {
		// 	if (wbkOrSht == 1 /*sheet*/) return validateIdCode(matchName, ds.Name);
		// 		
		// 	return ValidateModelName(matchName, e);
		// }

		// private bool validateIdCode(string idCode, string dsName)
		// {
		// 	string? testIdCode = testIdCode = extractIdCode(dsName, ExStorConst.EXS_SHT_NAME_SEARCH);
		//
		// 	return idCode.Equals(testIdCode);
		// }
		//
		// private string? extractIdCode(string idCode, string preface)
		// {
		// 	string? result = null;
		// 	try
		// 	{
		// 		result = idCode.Substring(preface.Length, ExStorConst.ID_CODE_LENGTH);
		//
		// 	}
		// 	catch { }
		//
		// 	return result;
		// }



		// public ExStoreRtnCode FindAllDsByName(string searchName, out List<DataStorage> dsList)
		// {
		// 	dsList = new List<DataStorage>();
		// 	ExStoreRtnCode result = ExStoreRtnCode.XRC_FAIL;
		//
		// 	FilteredElementCollector dataStorages
		// 		= findDataStorages();
		//
		// 	if (dataStorages != null)
		// 	{
		// 		foreach (Element el in dataStorages)
		// 		{
		// 			if (el.Name.StartsWith(searchName))
		// 			{
		// 				dsList.Add((DataStorage) el);
		// 				result = ExStoreRtnCode.XRC_GOOD;
		// 			}
		// 		}
		// 	}
		//
		// 	return result;
		// }
		//
		// public ExStoreRtnCode FindDsBySchema(Schema s, out DataStorage ds)
		// {
		// 	ds = null;
		// 	Entity e;
		// 	DataStorage? d;
		//
		// 	ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_FAIL;
		//
		// 	FilteredElementCollector dataStorages
		// 		= findDataStorages();
		//
		// 	if (dataStorages != null)
		// 	{
		// 		foreach (Element el in dataStorages)
		// 		{
		// 			d = el as DataStorage;
		//
		// 			if (d != null)
		// 			{
		// 				e = d.GetEntity(s);
		//
		// 				if (e.IsValid())
		// 				{
		// 					ds = d;
		// 					rtnCode = ExStoreRtnCode.XRC_GOOD;
		// 					break;
		// 				}
		// 			}
		// 		}
		// 	}
		//
		// 	return rtnCode;
		// }

		// public ExStoreRtnCode FindSchema(string schemaName, out Schema s)
		// {
		// 	s = null;
		//
		// 	IList<Schema> schemas = Schema.ListSchemas();
		//
		// 	if (schemas == null || schemas.Count == 0) return ExStoreRtnCode.XRC_SCHEMA_NOT_FOUND;
		//
		// 	ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_SCHEMA_NOT_FOUND;
		//
		// 	foreach (Schema schema in schemas)
		// 	{
		// 		if (schema.SchemaName.Equals(schemaName))
		// 		{
		// 			s = schema;
		// 			return ExStoreRtnCode.XRC_GOOD;
		// 		}
		// 	}
		//
		// 	return rtnCode;
		// }


		// private ExStoreRtnCode findWorkBookDs(string searchName, string modelName, FieldData<WorkBookFieldKeys> fd, out DataStorage? ds)
		// {
		// 	ds = null;
		//
		// 	ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_DS_NOT_FOUND;
		//
		// 	List<DataStorage> dsList;
		// 	Entity e;
		// 	Schema s;
		// 	DynaValue? dv;
		//
		// 	if (FindAllDsByName(searchName, out dsList ) != 
		// 		ExStoreRtnCode.XRC_GOOD) return rtnCode;
		//
		// 	if (FindSchema(ExStorConst.WbkSchemaName(), out s)
		// 		!= ExStoreRtnCode.XRC_GOOD) return rtnCode;
		//
		//
		// 	// list has only DS that start with search name
		// 	// check for model name match
		// 	foreach (DataStorage dx in dsList)
		// 	{
		// 		e = dx.GetEntity(s);
		//
		// 		dv = ReadField(e, fd);
		//
		// 		if ((dv?.Value != null) && dv.Value.Equals(modelName))
		// 		{
		// 			ds = dx;
		// 			rtnCode = ExStoreRtnCode.XRC_GOOD;
		// 		} 
		// 	}
		//
		// 	return rtnCode;
		// }



		// public void FindStorageInfo(string name, out DataStorage ds, out Entity e, out Schema s)
		// {
		// 	e = null;
		// 	s = null;
		// 	ds = null;
		//
		// 	findWbkDs(ExStorConst.EXS_WBK_NAME_SEARCH, out ds);
		//
		// 	if (ds == null) return;
		//
		// 	IList<Guid> guids = ds.GetEntitySchemaGuids();
		//
		// 	if (guids == null || guids.Count == 0) return;
		//
		// 	for (int i = 0; i < guids.Count; i++)
		// 	{
		// 		s = Schema.Lookup(guids[i]);
		//
		// 		if (s.SchemaName.StartsWith(name))
		// 		{
		// 			e = ds.GetEntity(s);
		// 			break;
		// 		}
		// 	}
		//
		// }

		// private void findWbkDs(string search, out DataStorage ds)
		// {
		// 	ds = null;
		//
		// 	FilteredElementCollector dataStorages
		// 		= findDataStorages();
		//
		// 	if (dataStorages != null)
		// 	{
		// 		foreach (Element el in dataStorages)
		// 		{
		// 			if (el.Name.StartsWith(search))
		// 			{
		// 				ds = (DataStorage) el;
		// 				break;
		// 			}
		// 		}
		// 	}
		// }




		// public void WriteField2<Te, Tk>(ExStorDataObj<Te> exo, FieldData<Tk>  field, DynaValue value)
		// 	where Te : Enum
		// 	where Tk : Enum
		// {
		// 	Schema s;
		// 	Entity e;
		// 	DataStorage ds;
		//
		// 	findStorageInfo(exo.SchemaName, out ds, out e, out s);
		//
		// 	Field f = s.GetField(field.Field.FieldName);
		//
		// 	if (f == null || !f.IsValidObject) return;
		//
		// 	if (field.DyValue.IsDictStringString)
		// 	{
		// 		e.Set<IDictionary<string, string>>(f, value.RevitValue);
		// 	}
		// 	else if (field.DyValue.IsListString)
		// 	{
		// 		e.Set<IList<string>>(f, value.RevitValue);
		// 	}
		// 	else
		// 	{
		// 		e.Set(f, value.RevitValue);
		// 	}
		//
		// 	UpdateEntity(ds, e);
		// }

		// private void findStorageInfo(string name, out DataStorage ds, out Entity e, out Schema s)
		// {
		// 	e = null;
		// 	s = null;
		// 	ds = null;
		//
		// 	findWbkDs(ExStorConst.EXS_WBK_NAME_SEARCH, out ds);
		//
		// 	if (ds == null) return;
		//
		// 	IList<Guid> guids = ds.GetEntitySchemaGuids();
		//
		// 	if (guids == null || guids.Count == 0) return;
		//
		// 	for (int i = 0; i < guids.Count; i++)
		// 	{
		// 		s = Schema.Lookup(guids[i]);
		//
		// 		if (s.SchemaName.StartsWith(name))
		// 		{
		// 			e = ds.GetEntity(s);
		// 			break;
		// 		}
		// 	}
		//
		// }
		//
		// private void findWbkDs(string search, out DataStorage ds)
		// {
		// 	ds = null;
		//
		// 	FilteredElementCollector dataStorages
		// 		= findDataStorages();
		//
		// 	if (dataStorages != null)
		// 	{
		// 		foreach (Element el in dataStorages)
		// 		{
		// 			if (el.Name.StartsWith(search))
		// 			{
		// 				ds = (DataStorage) el;
		// 				break;
		// 			}
		// 		}
		// 	}
		// }



// 		/// <summary>
// 		/// write the workbook live data to the model<br/>
// 		/// creates the ds, schema, and entity
// 		/// </summary>
// 		public ExStoreRtnCode WriteWorkBook()
// 		{
// 			Msgs.WriteLineSpaced("step: 01|", ">>> start | transaction| save workbook");
//
// 			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_GOOD;
// 			DataStorage ds;
//
// 			using (Transaction T = new Transaction(R.RvtDoc, "Store Cells Data"))
// 			{
// 				T.Start();
// 				{
// 					if (CreateDs(out ds) != ExStoreRtnCode.XRC_GOOD) return ExStoreRtnCode.XRC_FAIL;
// //W1
// 					Msgs.WriteLineSpaced("step: W1|", ">>> start |primary write sequence| write workbook");
//
// 					xMgr.WorkBook.ExsSchema = makeSchema(xMgr.WorkBook.SchemaGuid, xMgr.WorkBook.Rows);
//
// 					xMgr.AddWbkDataStorage(ds);
//
// // eq
// 					Msgs.WriteLineSpaced("step: E1|", "create workbook entity");
//
// 					xMgr.WorkBook.ExsEntity = new Entity(xMgr.WorkBook.ExsSchema!);
//
// 					writeDataFields(xMgr.WorkBook.Rows);
//
// 					if (!xMgr.WorkBook.GotEntity)
// 					{
// 						T.RollBack();
// 						rtnCode = ExStoreRtnCode.XRC_FAIL;
// 					}
// 					else
// 					{
// 						xMgr.WorkBook.ExsDataStorage!.SetEntity(xMgr.WorkBook.ExsEntity);
// 						T.Commit();
// 						rtnCode = ExStoreRtnCode.XRC_GOOD;
// 					}
// 				}
// 			}
//
// 			return rtnCode;
// 		}
//
// 		private ExStoreRtnCode CreateDs(out  DataStorage ds)
// 		{
// 			Msgs.WriteLineSpaced("step: D1|", " create ds");
// 			ExStoreRtnCode rtnCode;
//
// 			ds = null;
// 			if (xMgr.WorkBook.GotDs) return ExStoreRtnCode.XRC_DS_EXISTS;
//
// 			try
// 			{
// 				ds = DataStorage.Create(R.RvtDoc);
//
// 				rtnCode = ExStoreRtnCode.XRC_GOOD;
// 			}
// 			catch
// 			{
// 				rtnCode = ExStoreRtnCode.XRC_FAIL;
// 			}
//
// 			return rtnCode;
//
// 		}
//
// 		private Schema makeSchema<Te>(Guid guid, Dictionary<Te, FieldData<Te>> fields)
// 			where Te : Enum
// 		{
// 			Msgs.WriteLineSpaced("step: W2|", ">>> start | create workbook schema");
//
// 			SchemaBuilder sb = new SchemaBuilder(guid);
//
// 			configWbkSchemaAdminParams(sb);
//
// 			addSchemaFields(sb, fields);
//
// 			Msgs.WriteLineSpaced("step: W2|", "<<< complete | create workbook schema");
// 			return sb.Finish();
// 		}
// 		
// 		// private void createWorkBookSchema()
// 		// {
// 		// 	Msgs.WriteLineSpaced("step: W2|", ">>> start | create workbook schema");
// 		//
// 		// 	SchemaBuilder sb = makeSchema(xMgr.WorkBook.SchemaGuid, xMgr.WorkBook.Rows);
// 		//
// 		// 	xMgr.ExStorWorkBookSchema = sb.Finish();
// 		//
// 		// 	Msgs.WriteLineSpaced("step: W2|", "<<< complete | create workbook schema");
// 		// }
//
// // 		private Schema createSchema<Te>(Guid guid, Dictionary<Te, FieldData<Te>> fields)
// // 			where Te : Enum
// // 		{
// // 			Msgs.WriteLineSpaced("step: W2|", ">>> start | create workbook schema");
// // 			SchemaBuilder sb = makeSchema(guid,fields);
// //
// // 			Msgs.WriteLineSpaced("step: W2|", "<<< complete | create workbook schema");
// // 			return sb.Finish();
// // 		}
// //
// // //W3
// // 		private SchemaBuilder makeSchema<Te>(Guid guid, Dictionary<Te, FieldData<Te>> fields)
// // 			where Te : Enum
// // 		{
// // 			Msgs.WriteLineSpaced("step: W3|", ">>> start | start add workbook schema fields");
// //
// // 			SchemaBuilder sb = new SchemaBuilder(guid);
// //
// // 			configWbkSchemaAdminParams(sb);
// //
// // 			addSchemaFields(sb, fields);
// //
// // 			Msgs.WriteLineSpaced("step: W3|", "<<< complete | start add workbook schema fields");
// //
// // 			return sb;
// // 		}
//
// //S1
// 		private void configWbkSchemaAdminParams(SchemaBuilder sb)
// 		{
// 			Msgs.WriteLineSpaced("step: S1|", "config schema admin properties");
//
// 			sb.SetReadAccessLevel(AccessLevel.Public);
// 			sb.SetWriteAccessLevel(AccessLevel.Public);
//
// 			sb.SetSchemaName(xMgr.WorkBook.DsName);
// 			sb.SetDocumentation(xMgr.WorkBook.Desc);
// 		}

	}
}