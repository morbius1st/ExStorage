using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows.Controls.Primitives;
using Autodesk.Revit.DB.ExtensibleStorage;
using JetBrains.Annotations;
using RevitLibrary;
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
		
		protected bool isModifiedExo;
		private bool canApplyResetBtn;

		protected ExStorDataObj()
		{
			Rows = new ();
		}

		/* properties */

		public int FieldSrcArraySize {get; protected set;}

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
		
		public bool CanApplyResetBtn
		{
			get => canApplyResetBtn;
			set
			{
				if (value == canApplyResetBtn) return;
				canApplyResetBtn = value;
				OnPropertyChanged();
			}
		}
		public abstract bool IsModifiedExo { get; set; }

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

		/* required properties */

		public abstract string DateModifiedByUser {get; set;}
		public abstract void SetDateModifiedByAltSrc(string value,int src);
		public abstract FieldData<Te> DateModifiedField {get;}

		/* methods */

		// protected abstract void UpdateModifiedDate(int state);
		/// <summary>
		/// update the modify date to a current value or to the original value depending on state<br/>
		/// state >= 0 -> set modified to state<br/>
		/// state &#60; 0 reset mod date and set modified by none.
		/// </summary>
		protected void UpdateModifiedDate(int state)
		{
			if (state >= 0)
			{
				SetDateModifiedByAltSrc(DateTime.Now.ToString("s"), state);
			}
			else
			{
				DateModifiedField.UndoChg();
				DateModifiedField.ClrChgSrc();

				OnPropertyChanged(nameof(DateModifiedByUser));
			}
		}



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


		protected void ValidateChangeStatus([CallerMemberName] string who = "")
		{
			bool isAnyAltSrcSet = false;
			bool hasMod = false;
			int[] isMod = new int[FieldSrcArraySize];

			foreach ((Te? key, FieldData<Te>? fd) in rows)
			{
				if (fd.DyValue!.IsClean) continue;

				hasMod = true;

				for (int i = 0; i < FieldSrcArraySize; i++)
				{
					if (fd.IsChgSrcSet(i))
					{
						isMod[i]++;
						if (i >= FieldData<Te>.ALT_SRC_FIRST_IDX) isAnyAltSrcSet=true;
					}
				}
			}

			IsModifiedExo = isMod[0] > 0;
			CanApplyResetBtn = isMod[0] > 0;;

			if (hasMod)
			{
				for (int i = FieldSrcArraySize - 1; i >= FieldData<Te>.ALT_SRC_FIRST_IDX; i--)
				{
					if (isMod[i] > 0)
					{
						UpdateModifiedDate(1);
						break;
					}
				}

				if (isMod[0] > 0)
				{
					UpdateModifiedDate(0);
				}
			}
			else
			{
				UpdateModifiedDate(-1);
			}
		}


		/// <summary>
		/// validate the state of field changes to determine the state of<br/>
		/// IsModifiedExo (basic modified flag)<br/>
		/// CanApplyResetBtn (whether these buttons are emabled)<br/>
		/// Whether to update the modified data field
		/// </summary>
		protected void ValidateChangeStatus2([CallerMemberName] string who = "")
		{
			int isMod = 0;

			int userFieldCount = 0;
			int altSrcCount = 0;
			int altSrcACount = 0;
			int altSrcBCount = 0;

			// UserSecutityLevel usl = SecurityMgr.Instance.UserSecurityLevel;

			foreach ((Te? key, FieldData<Te>? fd) in rows)
			{
				if (fd.DyValue!.IsClean) continue;
				// if (fd.IsCtrlField) continue;
				// todo - add logic?

				// canEditField = SecurityMgr.ValidateFieldEditing(fd.Field.FieldEditLevel, usl) == FieldEditStatus.FES_CAN_EDIT;


				// todo - add logic
				// if (fd.Field!.TstFcFlagViaIuFlag(ItemUsage.IU_IS_ALT_SRC_A) ||
				// 	fd.Field!.TstFcFlagViaIuFlag(ItemUsage.IU_IS_ALT_SRC_B))
				// {
				// 	altSrcCount++;
				// 	altSrcACount += fd.IsAltSrcA ? 1 : 0;
				// 	altSrcBCount += fd.IsAltSrcB ? 1 : 0;
				// }
				// else
				// 	userFieldCount++;

				isMod++;
			}

			if (isMod > 0)
			{
				isModifiedExo = userFieldCount > 0;
				CanApplyResetBtn = userFieldCount  > 0;

				if (altSrcCount == 0)
				{
					UpdateModifiedDate(0);
				}
				else
				{
					if (altSrcACount > 0) UpdateModifiedDate(1);
					else if (altSrcBCount > 0) UpdateModifiedDate(2);
				}
			}
			else
			{
				// no changes, clear the info
				// disallow buttons
				UpdateModifiedDate(-1);
				isModifiedExo = false;
				CanApplyResetBtn = false;;
			}

			OnPropertyChanged(nameof(IsModifiedExo));
		}

		/// <summary>
		/// apply or undo the change in the local copy to all fields
		/// </summary>
		public void ApplyOrUndoChanges(bool bypassAlt, bool applyChg)
		{
			UserSecutityLevel usl = SecurityMgr.Instance.UserSecurityLevel;
			bool canEditField;

			foreach ((Te? key, FieldData<Te>? fd) in rows)
			{
				if (fd.DyValue!.IsChanged == true)
				{
					canEditField = 
						SecurityMgr.ValidateFieldEditing(fd.Field!.FieldEditLevel, usl) == 
						FieldEditStatus.FES_CAN_EDIT;


					// todo - add logic
					// if (fd.Field!.IsAltSrcA)
					// {
					// 	if (applyChg)
					// 	{
					// 		if (bypassAlt && !canEditField) continue;
					// 	}
					// 	else
					// 	{
					// 		if (bypassAlt) continue;
					// 	}
					// 	
					// }
					// else if (!fd.Field.IsAltSrcB) if (!bypassAlt) continue;
					//
					// if (applyChg)
					// {
					// 	fd.DyValue.ApplyChange();
					// }
					// else
					// {
					// 	UndoChange(fd);
					// }
				}
			}

			// // flag no modified
			// IsModifiedExo = false;

			ValidateChangeStatus();

			// update the modified date to the current time
			// UpdateModifiedDate();
		}

		// /// <summary>
		// /// apply change in the local copy to all fields
		// /// </summary>
		// public void ApplyChanges(bool bypassAlt)
		// {
		// 	foreach ((Te? key, FieldData<Te>? fd) in rows)
		// 	{
		// 		if (fd.DyValue!.IsChanged == true)
		// 		{
		// 			if (fd.Field!.IsAltSrcA)
		// 			{
		// 				if (bypassAlt) continue;
		// 			}
		// 			else if (!fd.Field.IsAltSrcB) if (!bypassAlt) continue;
		//
		// 			fd.DyValue.ApplyChange();
		// 		}
		// 	}
		//
		// 	// // flag no modified
		// 	// IsModifiedExo = false;
		//
		// 	ValidateChangeStatus();
		//
		// 	// update the modified date to the current time
		// 	// UpdateModifiedDate();
		// }

		public void SetTrackChanges()
		{
			foreach ((Te? key, FieldData<Te>? value) in Rows)
			{
				value.DyValue!.SetTrackChanges();
			}
		}

		public int RowCount => Rows.Count;

		protected void SetFieldCtrlByAlt(Te which)
		{
			FieldData<Te> fd = rows[which];

			
		}

		public void UndoChange(FieldData<Te> fd)
		{
			UndoValueChange(fd);
			OnPropertyChanged(fd.Field.FieldPropName);
		}

		protected void UndoValueChange(FieldData<Te> fd)
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

		public bool SetNewValueDymx(Te key, dynamic dv) //, bool validate = true)
		{
			if (!(Rows?.ContainsKey(key) ?? false)) return false;

			FieldData<Te> field = Rows[key];

			if (!field.DyValue.TrackChanges) 
				R.ProcessMsg($"track? {field.DyValue.TrackChanges} | got key {key} & value {dv.ToString()}", -1);

			// if (!field.DyValue.TrackChanges) 
			// 	throw new InvalidOperationException($"Use {nameof(SetInitValueDym)}() to set the field's value");
			
			field.DyValue.ChangeValue(dv!);

			// Rows[key] = field;

			return true;
		}

		public bool SetNewValueDym(FieldData<Te> field, dynamic dv) //, bool validate = true)
		{
			if (!field.DyValue.TrackChanges)
			{
				R.ProcessMsg($"track? {field.DyValue.TrackChanges} | got field {field.Field!.FieldName} & value {dv.ToString()}", -1);
				return false;
			}

			field.DyValue.ChangeValue(dv!);

			return true;
		}

		public void UpdateUsrChgSrc(FieldData<Te> field)
		{
			if (field.IsModified())
			{
				field.SetUsrChgSrc();
			}
			else
			{
				field.UnSetUsrChgSrc();
			}
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

			FieldData<Te> f = new FieldData<Te>(field, dy ?? field.FieldDefValue!.Value);

			f.FieldSrcArraySize = FieldSrcArraySize;

			Rows.Add(key, f);
		}

		/* initialize row data */

		internal void init(Dictionary<Te, FieldDef<Te>> f)
		{
			IsEmpty = true;

			foreach ((Te key, FieldDef<Te> value) in f)
			{
				// addValue(key, f);

				addValue(key, value);
			}
		}

		[DebuggerStepThrough]
		[NotifyPropertyChangedInvocator]
		protected void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}
		public event PropertyChangedEventHandler PropertyChanged;

	}
}