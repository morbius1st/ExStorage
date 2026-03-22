using System.Diagnostics;
using System.Windows.Markup;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStoreTest2026;
using ExStoreTest2026.DebugAssist;
using RevitLibrary;
using UtilityLibrary;
using static ExStorSys.ActivateStatus;
using static ExStorSys.WorkBookFieldKeys;

// username: jeffs
// created:  9/17/2025 9:22:50 PM

namespace ExStorSys
{
	public class ExStorLib
	{
	#region private fields

		public readonly int ObjectId;

		// private readonly ExStorMgr _xMgr;

	#endregion

	#region ctor

		// ReSharper disable once InconsistentNaming
		private static readonly Lazy<ExStorLib> instance =
			new (() => new ExStorLib());

		private ExStorLib()
		{
			ObjectId = ExStorStartMgr.Instance?.AddObjId(nameof(ExStorLib)) ?? -1;
		}

		public static ExStorLib Instance => instance.Value;

	#endregion

	#region public properties

	#endregion

	#region private properties

	#endregion

	#region methods

		/* system routines - workbook */

		/// <summary>
		/// takes the information stored in the workbook in xData and writes the
		/// information into the workbook's entity (and therefore into the models
		/// ds).  this creates a new DS so an existing DS must not be present. use
		/// UpdateWorkBook to update an existing DS
		/// </summary>
		/// <param name="wbk"></param>
		/// <param name="wbkSchema"></param>
		/// <returns></returns>
		public ExStoreRtnCode WriteWorkBook(WorkBook wbk, Schema? wbkSchema)
		{
			if (wbkSchema == null) return ExStoreRtnCode.XRC_SCHEMA_MISSING;

			// Msgs.WriteLineSpaced("step: 01|", ">>> start | transaction| save workbook");
			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_FAIL;

			if (wbk.GotDs) return ExStoreRtnCode.XRC_DS_EXISTS;

			using Transaction T = new (R.RvtDoc, "Store Cells Wbk Data");
			T.Start();
			{
				if (createDs(wbk) == ExStoreRtnCode.XRC_GOOD)
				{
					// make schema, set name and desc, save to workbook
					// wbkSchema = makeSchema(wbk);
					wbk.ExsEntity = new Entity(wbkSchema);

					if (wbk.GotEntity)
					{
						writeDataFields(wbkSchema, wbk);
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

			return rtnCode;
		}

		/* system routines - sheet */

		public ExStoreRtnCode WriteSheet(Sheet sheet, Schema? shtSchema)
		{
			if (shtSchema == null) return ExStoreRtnCode.XRC_SCHEMA_MISSING;

			// Msgs.WriteLineSpaced("step: 01|", ">>> start | transaction| save sheet");
			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_FAIL;

			if (sheet.GotDs) return ExStoreRtnCode.XRC_DS_EXISTS;

			using Transaction T = new (R.RvtDoc, "Store Cells Sht Data");
			T.Start();
			{
				if (createDs(sheet) == ExStoreRtnCode.XRC_GOOD)
				{
					sheet.ExsEntity = new Entity(shtSchema);

					if (sheet.GotEntity)
					{
						writeDataFields(shtSchema, sheet);
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
		// 	if (ReadEntityFields(e, sht) != ExStoreRtnCode.XRC_GOOD) return ExStoreRtnCode.XRC_FAIL;
		//
		// 	sht.ExsEntity = e;
		// 	sht.ExsSchema = s;
		//
		// 	sht.SetReadComplete();
		//
		// 	return rtnCode;
		// }


		/* data storage */

		public ExStoreRtnCode FindSheetsDs(string dsSearchName, out IList<DataStorage>? dsList)
		{
			return FindAllDataStorageByNamePrefix(dsSearchName, out dsList);
		}

		/// <summary>
		/// create a DataStorage object for the associated exo object.  the DS object
		/// must not already exist - check first
		/// </summary>
		private ExStoreRtnCode createDs<TE>(ExStorDataObj<TE> exo)
			where TE : Enum
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

		/* entity */

		public bool GetEntity(DataStorage ds, Schema? s, out Entity? e)
		{
			e = null;

			if (!ds.IsValidObject) return false;
			if (s == null || !s.IsValidObject) return false;

			e = ds.GetEntity(s);
			if (!e.IsValidObject) return false;

			return true;
		}

		/* schema */

		/// <summary>
		/// make, populate, and return a schema.  return null if fail (if already exists)
		/// </summary>
		public Schema? MakeSchema<TE>(ExStorDataObj<TE> exo)
			where TE : Enum
		{
			SchemaBuilder sb = new (exo.SchemaGuid);

			configSchemaParams(sb, exo);

			addSchemaFields(sb, exo);

			Schema? s = null;

			try
			{
				s = sb.Finish();
			}
			// ReSharper disable once EmptyGeneralCatchClause
			catch (Exception e) 
			{
				Debug.WriteLine($"exception | {e.Message}");
			}

			return s;
		}

		private void configSchemaParams<TE>(SchemaBuilder sb, ExStorDataObj<TE> exo)
			where TE : Enum
		{
			sb.SetReadAccessLevel(AccessLevel.Public);
			sb.SetWriteAccessLevel(AccessLevel.Public);

			sb.SetSchemaName(exo.SchemaName);
			sb.SetDocumentation(exo.SchemaDesc);
		}

		private void addSchemaFields<TE>(SchemaBuilder sb, ExStorDataObj<TE> exo)
			where TE : Enum
		{
			// Msgs.WriteLineSpaced("step: S2|", "write schema fields");

			// ReSharper disable once UnusedVariable
			foreach ((TE key, FieldData<TE> wkbkField) in exo)
			{
				addSchemaField(sb, wkbkField);
			}

			// Msgs.WriteLine(" done");
		}

		private void addSchemaField<TE>(SchemaBuilder sb, FieldData<TE> fd)
			where TE : Enum
		{
			// Msgs.Write(".");

			FieldBuilder fb;

			FieldDef<TE> f = fd.Field;
			DynaValue d = fd.DyValue;

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
				fb = sb.AddSimpleField(f.FieldName, d.RevitTypeIs);
			}

			fb.SetDocumentation(f.FieldDesc);
		}

		/* delete */

		/// <summary>
		/// delete a datastorage object - when done it is gone
		/// </summary>
		public ExStoreRtnCode DeleteDs(DataStorage ds)
		{
			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_GOOD;

			using Transaction T = new (R.RvtDoc, "Delete a DataStorage");
			T.Start();
			{
				try
				{
					R.RvtDoc?.Delete(ds.Id);
					T.Commit();
				}
				catch
				{
					T.RollBack();
					rtnCode = ExStoreRtnCode.XRC_FAIL;
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

			using Transaction T = new (R.RvtDoc, "Erase a Schema");
			T.Start();
			{
				try
				{
					R.RvtDoc?.EraseSchemaAndAllEntities(s);
					s.Dispose();
					T.Commit();
				}
				catch
				{
					T.RollBack();
					rtnCode = ExStoreRtnCode.XRC_FAIL;
				}
			}

			return rtnCode;
		}


		/* write data*/

		private void writeDataFields<TE>(Schema schema, ExStorDataObj<TE> exo)
			where TE : Enum
		{
			// Msgs.WriteLineSpaced("step: WD|", ">>> start | write data");

			// ReSharper disable once UnusedVariable
			foreach ((TE? key, FieldData<TE> data) in exo)
			{
				writeDataField(schema, exo, data);
			}

			// Msgs.WriteLine(" done");
		}

		private void writeDataField<TE>(Schema schema, ExStorDataObj<TE> exo, FieldData<TE> fd)
			where TE : Enum
		{
			// Msgs.Write(".");
			Field fx = schema.GetField(fd.Field.FieldName);

			if (fx == null || !fx.IsValidObject) return;

			DynaValue d = fd.DyValue;

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


		/* update entity field */

		/// <summary>
		/// update a field within the ds entity using a revit transaction
		/// </summary>
		public void UpdateEntityField<TE>(TE key, Schema? schema, ExStorDataObj<TE> exo, DynaValue value)
			where TE : Enum
		{
			FieldData<TE> fd = exo.GetField(key);

			updateEntityField(schema, exo, fd, value);
		}

		/// <summary>
		/// update a field within the ds entity using a revit transaction
		/// </summary>
		private void updateEntityField<TE, TK>(Schema? schema, ExStorDataObj<TE> exo, FieldData<TK>  field, DynaValue value)
			where TE : Enum
			where TK : Enum
		{
			Entity e = exo.ExsEntity!;
			DataStorage ds = exo.ExsDataStorage!;

			Field f = schema!.GetField(field.Field.FieldName);

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

			updateEntity(ds, e);
		}

		/// <summary>
		/// update a field within the ds entity using a revit transaction
		/// </summary>
		private void updateEntity(DataStorage ds, Entity e)
		{
			using Transaction T = new (R.RvtDoc, "Save Cells Data");
			T.Start();
			{
				try
				{
					ds.SetEntity(e);
					T.Commit();
				}
				catch
				{
					T.RollBack();
				}
			}
		}


		/* read */

		/* switched to use dynavalue - start */

		/* voided
		/// <summary>
		/// read a field from the entity using field data<br/>
		/// returns a DynaValue or null if not found
		/// </summary>
		public DynaValue? ReadField1a<TKeyType>(Entity e, FieldData<TKeyType>  data)
			where TKeyType : Enum
		{
			return readField<TKeyType>(e, data.Field.FieldName, data.DyValue.TypeIs);
		}

		/// <summary>
		/// read a field from the entity using the field definition<br/>
		/// returns a DynaValue or null if not found
		/// </summary>
		public DynaValue? ReadField1b<TKeyType>(Entity e, FieldDef<TKeyType>  data)
			where TKeyType : Enum
		{
			return readField<TKeyType>(e, data.FieldName, data.FieldType);
		}

		private DynaValue? readField<TKeyType>(Entity e, string? fieldName, Type t)
			where TKeyType : Enum
		{
			// data is the field definition and the DV within contains the definition 
			// information

			DynaValue? dv = null;

			if (fieldName.IsVoid()) return dv;

			if (t == typeof(string))
			{
				dv = new DynaValue(e.Get<string>(fieldName));
			}
			else if (t == typeof(bool))
			{
				dv = new DynaValue(e.Get<bool>(fieldName));
			}
			else if (t == typeof(Guid))
			{
				dv = new DynaValue(e.Get<Guid>(fieldName));
			}
			else if (t == typeof(Dictionary<string, string>))
			{
				dv = new DynaValue(e.Get<IDictionary<string, string>>(fieldName));
			}
			else if (t == typeof(List<string>))
			{
				dv = new DynaValue(e.Get<IList<string>>(fieldName));
			}
			else if (t.BaseType == typeof(Enum))
			{
				if (fieldName.Equals(Fields.KEY_DS_NAME)) return dv;

				string eName = e.Get<string>(fieldName);
				// dv = parseEnum(t, eName);

				if (t == typeof(UpdateRules)) dv = parseEnum(eName, ExStorConst.DEFAULT_UPDATE_RULE);
				else if (t == typeof(ActivateStatus)) dv = parseEnum(eName, ExStorConst.DEFAULT_ACTIVATE_STATUS);
				else if (t == typeof(SheetOpStatus)) dv = parseEnum(eName, ExStorConst.DEFAULT_SHEET_OP_STATUS);
			}

			return dv;
		}

		private DynaValue parseEnum<Te>(string enumName, Te def)
			where Te : struct, Enum
		{
			Te e = default(Te);

			bool ok = Enum.TryParse<Te>(enumName, out e);
			if (!ok) e = def;
			return new DynaValue(e);
		}
		*/

		public DynaValue? ReadFieldDyn<TKeyType>(Entity e, FieldDef<TKeyType>  data)
			where TKeyType : Enum
		{
			return new DynaValue(readField2<TKeyType>(e, data.FieldName, data.FieldType));
		}

		/* switched to use dynavalue - end */

		/* switched to use dynamic - start */

		/// <summary>
		/// read all fields from the entity into the exo object
		/// </summary>
		public ExStoreRtnCode ReadEntityFields<TE>(Entity? e, ExStorDataObj<TE> exo)
			where TE : Enum
		{
			if (e == null || !e.IsValid()) return ExStoreRtnCode.XRC_DATA_NOT_FOUND;

			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_GOOD;

			foreach ((TE? key, FieldData<TE> fd) in exo)
			{
				if (!exo.SetInitValueDym(key, ReadField2(e, fd.Field!)))
				{
					rtnCode = ExStoreRtnCode.XRC_FAIL;
					break;
				}
			}

			return rtnCode;
		}

		public dynamic? ReadField2<TKeyType>(Entity e, FieldDef<TKeyType>  data)
			where TKeyType : Enum
		{
			return readField2<TKeyType>(e, data.FieldName, data.FieldType);
		}

		private dynamic? readField2<TKeyType>(Entity e, string? fieldName, Type t)
			where TKeyType : Enum
		{
			// data is the field definition and the DV within contains the definition 
			// information

			dynamic? dv = null;

			if (fieldName.IsVoid()) return dv;

			if (t == typeof(string))
			{
				dv = e.Get<string>(fieldName);
			}
			else if (t == typeof(bool))
			{
				dv = e.Get<bool>(fieldName);
			}
			else if (t == typeof(Guid))
			{
				dv = e.Get<Guid>(fieldName);
			}
			else if (t == typeof(Dictionary<string, string>))
			{
				dv = e.Get<IDictionary<string, string>>(fieldName);
			}
			else if (t == typeof(List<string>))
			{
				dv = e.Get<IList<string>>(fieldName);
			}
			else if (t.BaseType == typeof(Enum))
			{
				if (fieldName!.Equals(Fields.KEY_DS_NAME)) return dv;

				string eName = e.Get<string>(fieldName);
				// dv = parseEnum(t, eName);

				if (t == typeof(UpdateRules)) dv = parseEnum2(eName, ExStorConst.DEFAULT_UPDATE_RULE);
				else if (t == typeof(ActivateStatus)) dv = parseEnum2(eName, ExStorConst.DEFAULT_ACTIVATE_STATUS);
				else if (t == typeof(SheetOpStatus)) dv = parseEnum2(eName, ExStorConst.DEFAULT_SHEET_OP_STATUS);
			}

			return dv;
		}

		private dynamic parseEnum2<Te>(string enumName, Te def)
			where Te : struct, Enum
		{
			Te e = default(Te);

			bool ok = Enum.TryParse<Te>(enumName, out e);
			if (!ok) e = def;
			return e;
		}

		/* switched to use dynamic - end */



		// private DynaValue? parseEnum(Type t, string enumName)
		// {
		// 	DynaValue? dv = null;
		//
		// 	if (t == typeof(UpdateRules))
		// 	{
		// 		UpdateRules k;
		// 		bool result = Enum.TryParse(enumName, out k);
		// 		if (!result) k = UpdateRules.UR_NEVER;
		// 		dv = new DynaValue(k);
		// 	}
		// 	else
		// 	if (t == typeof(ActivateStatus))
		// 	{
		// 		ActivateStatus k;
		// 		bool result = Enum.TryParse(enumName, out k);
		// 		if (!result) k = ActivateStatus.AS_INACTIVE;
		// 		dv = new DynaValue(k);
		// 	}
		// 	else
		// 	if (t == typeof(SheetOpStatus))
		// 	{
		// 		SheetOpStatus k;
		// 		bool result = Enum.TryParse(enumName, out k);
		// 		if (!result) k = SheetOpStatus.SOS_HOLD;
		// 		dv = new DynaValue(k);
		// 	}
		//
		// 	return dv;
		// }

		// public string? ReadModelName(DataStorage ds, Schema? s)
		// {
		// 	Entity e;
		//
		// 	if (!GetEntity(ds, s, out e)) return null;
		//
		// 	return ReadModelName(e);
		// }


		/// <summary>
		/// read the model name from the entity
		/// </summary>
		public string? ReadModelName(Entity? e)
		{
			if (e == null || !e.IsValid()) return null;
		
			FieldDef<WorkBookFieldKeys> f = Fields.GetWbkFieldDef(PK_MD_MODEL_TITLE);
		
			return CleanName(ReadField2(e, f!));
			// return CleanName(ReadField(e, f)?.Value);
		}

		// public ActivateStatus ReadActStatus(DataStorage ds, Schema? s)
		// {
		// 	Entity e;
		//
		// 	if (!GetEntity(ds, s, out e)) return AS_INACTIVE;
		//
		// 	return ReadActStatus(e);
		// }

		// public ActivateStatus ReadActStatus2(Entity e)
		// {
		// 	if (e == null || !e.IsValid()) return AS_INACTIVE;
		//
		// 	FieldData<WorkBookFieldKeys> f = Fields.GetWbkFieldData(PK_AD_STATUS);
		//
		// 	return (ActivateStatus) (ReadField(e, f)?.Value ?? AS_INACTIVE);
		// }


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
			rtnCode = findSchema(schemaName, out s);

			if (rtnCode != ExStoreRtnCode.XRC_GOOD) return rtnCode;

			// step 2, got schema / get remainder
			// when wbk - must match model name
			// when sht = must match model codes

			rtnCode = findExInfo(wbkOrSht, searchText, matchText, s, out ds, out e);

			return rtnCode;
		}


		private ExStoreRtnCode findExInfo(int wbkOrSht, string searchText, string matchText,
			Schema? s, out DataStorage? ds, out Entity? en)
		{
			ds = null;
			en = null;

			DataStorage? d;
			Entity? e;

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

						// bool b1 = e.IsValid();
						// bool b2 = (e.IsValid() && (wbkOrSht == 1 ||
						// 	(wbkOrSht == 0 && ValidateModelName(matchText, e, out string? _))));


						// wbkOrSht == 0 => getting workbook | wbkOrSht == 1 => getting sheet
						if (e.IsValid() && (wbkOrSht == 1 ||
							(wbkOrSht == 0 && ValidateModelName(matchText, e, out string? _)))
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

		private ExStoreRtnCode findSchema(string schemaName, out Schema? s)
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
		public ExStoreRtnCode FindAllSchemaByNamePrefix2(string searchName, out IList<Schema>? schemas)
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

		/// <summary>
		/// find all schema's that starts with the search name
		/// </summary>
		public ExStoreRtnCode FindAllSchemaByNamePrefix(string searchName, out IList<Schema> scList)
		{
			bool? result;
			string name;

			IList<Schema> scx = Schema.ListSchemas();

			scList = new List<Schema>();

			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_SCHEMA_NOT_FOUND;

			if (scx == null || scx.Count == 0) return rtnCode;

			foreach (Schema schema in scx)
			{
				name = schema.SchemaName;

				result = validateName(name, searchName);

				if (result == false) continue;

				scList.Add(schema);

				rtnCode = ExStoreRtnCode.XRC_GOOD;
			}

			return rtnCode;
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

		// public IList<DataStorage> FindAllDataStorageByNamePrefix(string searchName)
		// {
		// 	IList<DataStorage> dsList = new List<DataStorage>();
		//
		// 	FilteredElementCollector dataStorages = findDataStorages();
		//
		// 	foreach (Element el in dataStorages)
		// 	{
		// 		if (el.Name.StartsWith(searchName))
		// 		{
		// 			dsList.Add((DataStorage) el);
		// 		}
		// 	}
		//
		// 	return dsList;
		// }

		public ExStoreRtnCode FindAllDataStorageByNamePrefix(string searchName, out IList<DataStorage> dsList)
		{
			dsList = new List<DataStorage>();

			FilteredElementCollector dsx = findDataStorages();

			ExStoreRtnCode rtnCode = ExStoreRtnCode.XRC_SCHEMA_NOT_FOUND;

			if (dsx.GetElementCount() == 0) return rtnCode;

			foreach (Element el in dsx)
			{
				if (!validateName(el.Name, searchName)) continue;

				dsList.Add((DataStorage) el);

				rtnCode = ExStoreRtnCode.XRC_GOOD;
			}

			return rtnCode;
		}

		// /// <summary>
		// /// find all data storage elements based on the search name but not the version.<br/>
		// /// wbk - should find only one ds - with on without the current version<br/>
		// /// sht - may find zero or mord ds - all with on without the current version<br/>
		// /// return true if any found that match the search name else return false<br/>
		// /// flag if any found have the wrong version
		// /// </summary>
		// public bool FindAllDataStorageByNamePrefix(string searchName, out List<DataStorage> dsList)
		// {
		// 	DataStorage ds;
		// 	string name;
		// 	bool result;
		//
		// 	dsList = new ();
		//
		// 	FilteredElementCollector dataStorages = findDataStorages();
		//
		// 	if (dataStorages.GetElementCount() == 0) return false;
		//
		// 	foreach (Element el in dataStorages)
		// 	{
		// 		ds = (DataStorage) el;
		// 		name = ds.Name;
		//
		// 		result = validateName(name, searchName);
		//
		// 		// does not start with the search name
		// 		if (result == false) continue;
		//
		// 		dsList.Add(ds);
		// 	}
		//
		// 	return dsList.Count > 0;
		// }

		/* copy / duplicate */

		/// <summary>
		/// duplicate a sheet based on the copy type (does not apply to workbook)<br/>
		/// copy types: int 0 through 7 compared to the copy type in the field<br/>
		/// however, defined types (all will need a new name): <br/>
		/// 1 - copy as a template for a new item (needs creation / modification info)<br/>
		/// 2 - copy to update the version (modification info)<br/>
		/// 4 - copy to use in a new model (needs creation / modification info)
		/// </summary>
		public Sheet DuplicateSheet(int copyType, Sheet shtOldVer, 
			string name, Dictionary<SheetFieldKeys, DynaValue> sd)
		{
			bool doUpdate;
			Sheet shtNewVer;
			FieldCopyType cpyType = (FieldCopyType) copyType;

			shtNewVer = Sheet.CreateEmptySheet(name);

			foreach ((SheetFieldKeys key, FieldData<SheetFieldKeys> field) in shtOldVer)
			{
				if (field.Field.FieldCopyType == FieldCopyType.FC_IGNORE) continue;

				doUpdate = field.Field.FieldCopyType.HasFlag(cpyType);

				if (doUpdate)
				{
					if (sd.TryGetValue(key, out DynaValue? value))
						shtNewVer.SetInitValueDym(key, value.Value);
				}
				else
				{
					shtNewVer.SetInitValueDym(key, shtOldVer.GetValue(key)?.Value ?? null);
				}
			}

			// shtNewVer.IsPopulated = true;

			// shtNewVer.SchemaName - fixed value
			// shtNewVer.SchemaDesc - fixed value
			// shtNewVer.SchemaGuid - fixed value
			// shtNewVer.ExsDataStorage - set when written;
			// shtNewVer.ExsEntity - set when written;

			return shtNewVer;
		}

		
		/* misc */

		public bool ValidateModelName(string matchName, Entity e, out string? modelName)
		{
			modelName = ReadModelName(e);

			if (modelName == null) return false;

			return matchName.Equals(modelName);
		}


		/// <summary>
		/// validate the ex item's name<br/>
		/// true == name is good & correct version
		/// false == does not start with the search name<br/>
		/// null == has wrong version
		/// </summary>
		private bool? validateName(string name, string searchName, string tstVerStr)
		{
			if (!name.StartsWith(searchName)) return false;

			string? verStr = ExtractVersionFromName(name);

			return verStr.IsVoid() ? false : (verStr!.Equals(tstVerStr) ? true : null);
		}

		/// <summary>
		/// validate the ex item's name<br/>
		/// true == name is good & correct version
		/// false == does not start with the search name<br/>
		/// </summary>
		private bool validateName(string name, string searchName)
		{
			return name.StartsWith(searchName);
		}


		public string? ExtractVersionFromName(string? name)
		{
			// look for "_v"
			// wbk ds
			//           1         2         3
			// 0123456789012345678901234567890
			// v----------vv----e  
			// CsCells_WBK_v1_00
			// sht ds
			// CsCells_SHT_AAAA_v1_00
			// wbk schema
			// CsCells_WKB_Schema_v1_00
			// sht schema
			// CsCells_SHT_Schema_v1_00

			int pos1 = (name?.IndexOf("_v") + 1) ?? -1;
			int pos2 = (name?.Length ?? -2) - pos1;

			if (pos1 < 0 || pos2 < 3) return null;

			return name.Substring(pos1, pos2);
		}

		public string? ExtractIdFromShtName(string? name, string searchName)
		{
			// sht ds
			//           1         2         3
			// 0123456789012345678901234567890
			// v----------v---v------e  
			// CsCells_SHT_AAAA_v1_00
			// look for search name
			if (name.IsVoid()) return null;

			if (name!.Length != searchName.Length + 10) return null;

			return name.Substring(searchName.Length, 4);
		}

		public string CleanName(string? name)
		{
			return CsStringUtil.RegexCleanString(name ?? "", ExStorConst.DOC_NAME_REPL_STRING[0], ExStorConst.DOC_NAME_REPL_STRING[1]) ?? "";
		}

		public static string FormatFamAndType(string famName, string? typeName)
		{
			string key = $"{famName}|{typeName}";

			return key;
		}

		public static bool DivideFamAndType(string? famAndType, out string? family, out string? famType)
		{
			family = null;
			famType = null;
			int pos;

			if (famAndType!.IsVoid()) return false;

			pos = famAndType.IndexOf('|');

			// false if not there, or at begining, or at end
			if (pos == -1 || pos == 0 /*|| pos == famAndType.Length-1* - removed - empty type name is ok */) return false;

			// family is from start to the dividing charater but not including the dividing charagter
			family = famAndType.Substring(0, pos);
			famType = famAndType.Substring(pos+1, famAndType.Length - (pos+1));

			return true;
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
	}
}