using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows.Controls.Primitives;
using Autodesk.Revit.DB.ExtensibleStorage;


// Solution:     ExStorage
// Project:       ExStoreTest2026
// File:             ExStorDataObj.cs
// Created:      2025-09-25 (19:09)

namespace ExStorSys
{
	public abstract class ExStorDataObj<Te> : IEnumerable<KeyValuePair<Te, FieldData<Te>>>
		where Te : Enum
	{
		private DataStorage? exsDataStorage;
		private Entity? exsEntity;
		private bool populated;

		protected ExStorDataObj()
		{
			Rows = new ();
		}

		/* properties */

		public bool IsEmpty { get; protected set; }

		public bool Populated
		{
			get => populated;
			set
			{
				populated = value;
				updatePopulate();
			}
		}

		private void updatePopulate()
		{
			IsEmpty = !populated || !GotDs || !GotEntity;
		}

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
				updatePopulate();
			}
		}
		public Entity? ExsEntity
		{
			get => exsEntity;
			set
			{
				exsEntity = value;
				updatePopulate();
			}
		}

		/* shortcuts */

		public abstract string DsName { get; }
		public abstract string Desc { get; }

		public abstract string SchemaName { get; }
		public abstract string SchemaDesc { get; }
		public abstract Guid SchemaGuid { get; }

		public abstract string DsSearchName { get; }

		public bool Ready => GotDs && GotEntity;

		public bool GotDs => exsDataStorage != null && exsDataStorage.IsValidObject;
		public bool GotEntity => ExsEntity != null && ExsEntity.IsValid();

		/* rows */

		protected Dictionary<Te, FieldData<Te>> Rows { get; set; }

		public IEnumerator<KeyValuePair<Te, FieldData<Te>>> GetEnumerator()
		{
			return Rows.GetEnumerator();
		}

		// Implementation of the non-generic IEnumerable (required for compatibility)
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/* rows methods */

		public int RowCount => Rows.Count;

		private bool addRow(Te key, Dictionary<Te, FieldDef<Te>>? fields, DynaValue? dv = null)
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

		public FieldData<Te> getRow(Te key)
		{
			if (!(Rows?.ContainsKey(key) ?? false)) return FieldData<Te>.Empty();

			return Rows[key];
		}

		public DynaValue? GetValue(Te key)
		{
			FieldData<Te> row = getRow(key);

			return row.DyValue;
		}

		/* initialize row data */

		internal void init(Dictionary<Te, FieldDef<Te>> f)
		{
			IsEmpty = true;

			foreach ((Te? key, FieldDef<Te>? value) in f)
			{
				addRow(key, f);
			}
		}

	}
}