using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows.Controls.Primitives;
using Autodesk.Revit.DB.ExtensibleStorage;
using JetBrains.Annotations;
using UtilityLibrary;


// Solution:     ExStorage
// Project:       ExStoreTest2026
// File:             ExStorDataObj.cs
// Created:      2025-09-25 (19:09)

namespace ExStorSys
{
	public abstract class ExStorDataObj<Te> : IEnumerable<KeyValuePair<Te, FieldData<Te>>>, INotifyPropertyChanged
		where Te : Enum
	{
		private DataStorage? exsDataStorage;
		private Entity? exsEntity;

		protected Dictionary<Te, FieldData<Te>> rows;
		// private bool isPopulated;

		protected ExStorDataObj()
		{
			Rows = new ();
		}

		/* properties */

		// use UpdateExsObjects to set this
		/// <summary>
		/// flags that the workbook has not been populated with data<br/>
		/// </summary>
		public bool IsEmpty { get; protected set; }

		/* Data storage */

		public DataStorage? ExsDataStorage
		{
			get => exsDataStorage;
			set
			{
				exsDataStorage = value;
				// updatePopulate();
			}
		}

		public Entity? ExsEntity
		{
			get => exsEntity;
			set
			{
				exsEntity = value;
				// updatePopulate();
			}
		}

		public abstract bool IsModified { get; protected set; }

		/* shortcuts */

		public abstract string DsName { get; }
		public abstract string Desc { get; set; }

		public abstract string DsSearchName { get; }
		public abstract string? SchemaName { get; }
		public abstract string SchemaDesc { get; }
		public abstract Guid SchemaGuid { get; }

		public bool GotDs => exsDataStorage != null && exsDataStorage.IsValidObject;
		public bool GotEntity => ExsEntity != null && ExsEntity.IsValid();

		/* rows */

		protected Dictionary<Te, FieldData<Te>> Rows 
		{
			get => rows;
			set => rows = value;
		}

		public IEnumerator<KeyValuePair<Te, FieldData<Te>>> GetEnumerator()
		{
			return Rows.GetEnumerator();
		}

		// Implementation of the non-generic IEnumerable (required for compatibility)
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}


		/* methods */

		/// <summary>
		/// update the DS & E objects (S to be removed)
		/// </summary>
		public bool UpdateExsObjects(DataStorage ds, Entity e, Schema s)
		{
			if (!IsEmpty) return false;

			ExsDataStorage = ds;
			ExsEntity = e;
			// ExsSchema = s;

			IsEmpty = false;

			return true;
		}

		protected void ValidateChangeStatus()
		{
			bool isMod = false;

			foreach ((Te? key, FieldData<Te>? fd) in rows)
			{
				if (fd.DyValue.IsChanged == true)
				{
					isMod = true;
				}
			}

			IsModified = isMod;
		}

		public void ApplyChanges()
		{
			foreach ((Te? key, FieldData<Te>? fd) in rows)
			{
				if (fd.DyValue.IsChanged == true)
				{
					fd.DyValue.ApplyChange();
				}
			}

			ValidateChangeStatus();
		}

		public void SetTrackChanges()
		{
			foreach ((Te? key, FieldData<Te>? value) in Rows)
			{
				value.DyValue!.SetTrackChanges();
			}
		}

		public int RowCount => Rows.Count;

		public void UndoValueChange(FieldData<Te> fd)
		{
			fd.DyValue.UndoChange();

			if (fd.DyValue.TrackChanges)
			{
				ValidateChangeStatus();
			}
		}

		public bool SetInitValueDym(Te key, dynamic dv)
		{
			if (!(Rows?.ContainsKey(key) ?? false)) return false;

			FieldData<Te> field = Rows[key];

			if (field.DyValue.TrackChanges) 
				throw new InvalidOperationException($"Use {nameof(SetNewValueDym)}() to change the field's value");
			
			field.DyValue.ChangeValue(dv!);

			Rows[key] = field;

			return true;
		}

		public bool SetNewValueDym(Te key, dynamic dv, bool validate = true)
		{
			if (!(Rows?.ContainsKey(key) ?? false)) return false;

			FieldData<Te> field = Rows[key];

			if (!field.DyValue.TrackChanges) 
				throw new InvalidOperationException($"Use {nameof(SetInitValueDym)}() to set the field's value");
			
			field.DyValue.ChangeValue(dv!);

			Rows[key] = field;

			if (validate) ValidateChangeStatus();

			return true;
		}

		public FieldData<Te> GetField(Te key)
		{
			if (!(Rows?.ContainsKey(key) ?? false)) return FieldData<Te>.Empty();

			return Rows[key];
		}

		public DynaValue? GetValue(Te key)
		{
			FieldData<Te> row = GetField(key);

			return row.DyValue;
		}

		private void addValue(Te key, FieldDef<Te> field, dynamic? dy = null)
		{
			if (Rows.ContainsKey(key)) return;

			if (dy != null && !dy.GetType() != field.FieldType) return;

			Rows.Add(key, new (field, dy ?? field.FieldDefValue.Value));
		}

		/* initialize row data */

		internal void init(Dictionary<Te, FieldDef<Te>> f)
		{
			IsEmpty = true;

			foreach ((Te? key, FieldDef<Te>? value) in f)
			{
				// addValue(key, f);

				addValue(key, value);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[DebuggerStepThrough]
		[NotifyPropertyChangedInvocator]
		protected void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}
	}
}