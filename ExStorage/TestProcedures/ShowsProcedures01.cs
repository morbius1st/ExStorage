

// Solution:     ExStorage
// Project:       ExStorage
// File:             ShowsProcedures01.cs
// Created:      2022-01-16 (9:47 PM)


using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStorage.Windows;
using SharedApp.Windows.ShSupport;
using ShExStorageN.ShExStorage;
using ShExStorageR.ShExStorage;
using ShStudy.ShEval;

namespace ExStorage.TestProcedures
{
	public class ShowsProcedures01
	{
		private ShDebugMessages M { get; set; }
		// private ShDebugSupport D { get; set; }

		public ShowsProcedures01()
		{
			// D = d;

			M = MainWindow.M;
		}




	#region general data show

		public void ShowEid(ExId exId)
		{
			// ExId exId = exLib.ExId;

			M.NewLine();
			M.WriteDebugMsgLine("getting", "data storage id");
			M.WriteDebugMsgLine("EsId", exId.ExsId);
			M.WriteDebugMsgLine("VendorId", exId.VendorId);
			M.WriteDebugMsgLine("DocumentName", exId.DocumentName);
			M.WriteDebugMsgLine("DocName", exId.DocName);
			// M.ShowMsg();
		}

		public void ShowAllDs1(ExStorageLibraryR exLib)
		{
			M.NewLine();

			if (exLib.Da.AllDs == null)
			{
				M.WriteDebugMsgLine("get all Ds", "*** failed ***");
				return;
			}

			if (exLib.Da.AllDs.Count < 1)
			{
				M.WriteDebugMsgLine("get all Ds", "found none");
				return;
			}

			M.WriteDebugMsgLine("get all Ds| count| ", exLib.Da.AllDs.Count.ToString());

			foreach (DataStorage ds in exLib.Da.AllDs)
			{
				M.WriteDebugMsgLine("name| ", $"{ds.Name} | guid count| {ds.GetEntitySchemaGuids().Count}");

			}
		}

		public void ShowDsDetails(DataStorage ds)
		{
			/*
			 get category info - n/a - none
			Parameter parameterData = ds.get_Parameter(BuiltInParameter.ELEM_CATEGORY_PARAM);

			ElementId eid =  ds.get_Parameter(BuiltInParameter.ELEM_CATEGORY_PARAM).AsElementId();
			Element e = RvtCommand.RvtDoc.GetElement(eid);
			*/

			IList<Guid> guids = ds.GetEntitySchemaGuids();

			string result = "no guid";

			if (guids.Count > 0)
			{
				result = guids[0].ToString();
			}

			M.WriteLine($"ds name| {ds.Name}", 
				$"is pinned?| {ds.Pinned} | schema guid| {result}");

			/*
			list parameters

			foreach (Parameter p in ds.GetOrderedParameters())
			{
				M.WriteLine("parameter info| ", $"{GetRevitParameterInfo(p) }");
			}

			foreach (object dsParameter in ds.Parameters)
			{
				Parameter p = (Parameter) dsParameter;

				M.WriteLine("parameter info| ", $"{GetRevitParameterInfo(p) }");
			}

			*/

			M.NewLine();
		}

		public void ShowSchema2(ExStoreRtnCode result, Schema schema)
		{
			M.NewLine();
			M.WriteDebugMsgLine("got schema| result|", $"{result.ToString()}| schema name| {schema?.SchemaName ?? "is null"}");

			if (schema != null)
			{
				ShowSchemaDetails(schema);
			}

			// M.ShowMsg();
		}

		public void ShowSchemaDetails(Schema schema)
		{
			string a = schema.SchemaName;
			string b = schema.Documentation;
			string c = schema.VendorId;
			AccessLevel d = schema.ReadAccessLevel;
			IList<Field> f = schema.ListFields();

			M.WriteLine("schema info| name|", $"{a}| doc| {b}| vId| {c}| access| {d.ToString()}");

			if (f != null)
			{
				ShowSchemaFields(f);
			}
		}

		public void ShowSchemaFields(IList<Field> fields)
		{
			for (var i = 0; i < fields.Count; i++)
			{
				Field f = fields[i];

				string a = f.Documentation;
				string b = f.FieldName;
				string c = f.ContainerType.ToString();
				string d = f.KeyType?.ToString() ?? "null";
				string e = f.ValueType.ToString();
				string g = f.UnitType.ToString();
				string h = f.GetSpecTypeId().ToString();

				M.WriteLineAligned($"field {i}| name|", $"{b}| doc| {a}| cType| {c}| kType| {d}| vType| {e}| uType| {g}| specType {h}");

				if (f.ValueType == typeof(Entity))
				{
					Schema sc = f.SubSchema;

					if (sc != null)
					{
						M.NewLine();
						M.MarginUp();
						M.WriteLineAligned($"got sub-schema| true", $"name| {sc.SchemaName}");
						ShowSchemaFields(sc.ListFields());
						M.MarginDn();
						M.NewLine();
					}
					else
					{
						M.WriteLineAligned($"\tgot sub-schema| false");
					}
					
				}
			}
		}

		public string GetRevitParameterInfo(Parameter p)
		{
			string result = "parameter is null";

			if (p != null)
			{
				result = p.Definition.Name;

				if (p.HasValue)
				{
					switch (p.StorageType)
					{
					case StorageType.Double:
						{
							result += $" is double| {p.AsDouble()}";
							break;
						}
					case StorageType.String:
						{
							result += $" is string| {p.AsString()}";
							break;
						}
					case StorageType.Integer:
						{
							result += $" is integer| {p.AsInteger()}";
							break;
						}
					case StorageType.None:
						{
							result += $" is none";
							break;
						}
					case StorageType.ElementId:
						{
							result += $" is elementId";
							break;
						}
					default:
						{
							result += " unknown storage type";
							break;
						}
					}
				}
				else
				{
					result += $" no value| storage type is| {p.StorageType}";
				}
			}

			return result;
		}
		
		public void ShowEntityDetails(Entity e)
		{
			if (e == null)
			{
				M.WriteLine($"got entity| false - is null");
				return;
			}

			M.WriteLine($"got entity| true");

			M.WriteLine($"can  read?| {e.ReadAccessGranted().ToString()}");
			M.WriteLine($"can write?| {e.WriteAccessGranted().ToString()}");

			if (e.ReadAccessGranted())
			{
				M.WriteLine($"schema name| {e.Schema.SchemaName}");
				M.WriteLine($"schema GUID| {e.Schema.GUID.ToString()}");
			}

			// M.ShowMsg();
		}

		public void ShowEntityData6(ExStorageLibraryR exLib,	Entity e, Schema sc)
		{
			M.WriteLineAligned("fields and values for|", $"{sc.SchemaName}");
			
			foreach (Field f in sc.ListFields())
			{
				try
				{
					if (f.ValueType == typeof(Entity))
					{
						Schema scx = f.SubSchema;
						Entity et = e.Get<Entity>(f);

						M.NewLine();
						M.MarginUp();
						ShowEntityData6(exLib, et, scx);
						M.NewLine();
						M.MarginDn();

					}
					else
					{
						string s = exLib.GetEntityDataAsString(e, f);
						M.WriteLineAligned($"value for| {f.FieldName}", $"{s}");
					}
				}
				catch 
				{
					M.WriteLineAligned($"value for| {f.FieldName}", "error");
				}
			}

		}
		

	#endregion


	}
}