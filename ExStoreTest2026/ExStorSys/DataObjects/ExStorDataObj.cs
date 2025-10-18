using System.Linq.Expressions;
using Autodesk.Revit.DB.ExtensibleStorage;
using static ExStorSys.ExoCreationStatus;

// Solution:     ExStorage
// Project:       ExStoreTest2026
// File:             ExStorDataObj.cs
// Created:      2025-09-25 (19:09)

namespace ExStorSys
{
	public interface IExStorDataObj
	{
		public string DsName { get; set; }
	}

	public abstract class ExStorDataObj<Te>
		where Te : Enum

	{
		private DataStorage? exsDataStorage;
		private Entity? exsEntity;
		public ExStorMgr ExMgr { get; set; }

		/* properties */

		public bool IsInvalid => CreationStatus == CS_INVALID;
		public bool IsEmpty { get; protected set; }
		public ExoCreationStatus CreationStatus { get; protected set; }

		public void SetReadStart() => CreationStatus = CS_INVALID;
		public void SetReadComplete() => CreationStatus = CS_GOOD;

		public abstract int WbkOrSht { get; }
		
		// to use as a generic object
		// saved in ds
		// ds name
		// wbk schema name
		// sht schema name
		// guid
		// datastorage
		// entity
		// schema
		// rows (got)

		/* Data storage */

		public DataStorage? ExsDataStorage
		{
			get => exsDataStorage;
			set
			{
				exsDataStorage = value;
				CreationStatus = CreationStatus == CS_INIT ? CS_GOOD : CS_INIT;
			}
		}

		public abstract Schema? ExsSchema { get; set; }

		public Entity? ExsEntity
		{
			get => exsEntity;
			set
			{ 
				exsEntity = value;
				CreationStatus = CreationStatus == CS_INIT ? CS_GOOD : CS_INIT;
			}
		}

		/* shortcuts */

		public abstract string DsName { get; }
		public abstract string Desc { get; }

		public abstract string SchemaName { get; }
		public abstract string SchemaDesc { get; }
		public abstract Guid SchemaGuid { get; }

		public abstract string DsSearchName { get; }

		public bool GotDs => exsDataStorage != null && exsDataStorage.IsValidObject;
		public bool GotSchema => ExsSchema != null && ExsSchema.IsValidObject;
		public bool GotEntity => ExsEntity != null && ExsEntity.IsValid();

		/* rows */

		public Dictionary<Te, FieldData<Te>> Rows { get; set; }

		/* rows methods */

		public bool AddRow(Te key, Dictionary<Te, FieldDef<Te>>? fields, DynaValue? dv = null)
		{
			if (Rows.ContainsKey(key)) return false;

			FieldDef<Te> field = fields![key];

			Rows.Add(key, new (field, dv ?? field.FieldDefValue));

			return true;
		}

		public bool UpdateRow(Te key, DynaValue? value)
		{
			if (!(Rows?.ContainsKey(key) ?? false)) return false;

			FieldData<Te> row = Rows[key];

			row.DyValue = value;

			Rows[key] = row;

			return true;
		}

		public FieldData<Te> GetRow(Te key)
		{
			if (!(Rows?.ContainsKey(key) ?? false)) return FieldData<Te>.Empty();

			return Rows[key];
		}

		public DynaValue? GetValue(Te key)
		{
			FieldData<Te> row = GetRow(key);

			return row.DyValue;
		}


		/* initialize row data */

		internal void init(Dictionary<Te, FieldDef<Te>> f)
		{
			IsEmpty = true;

			foreach ((Te? key, FieldDef<Te>? value) in f)
			{
				AddRow(key, f);
			}
		}
	}
}