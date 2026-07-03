using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UtilityLibrary;

// Solution:     ExStorage
// Project:       ExStoreTest2026
// File:             ExStorDataObj.cs
// Created:      2025-09-25 (19:09)

namespace ExStorSys
{
	public abstract class ExStorDataObj<TE> : IEnumerable<KeyValuePair<TE, FieldData<TE>>>, INotifyPropertyChanged
		where TE : Enum
	{
		// ReSharper disable once InconsistentNaming
		protected Dictionary<TE, FieldData<TE>> rows;

		private bool isModExo;
		private bool undoBtnStatus;
		private bool applyBtnStatus;

		private readonly int _srcEnumLen;

		// ReSharper disable once ConvertConstructorToMemberInitializers
		protected ExStorDataObj()
		{
			rows = new ();

			_srcEnumLen = Enum.GetNames(typeof(ChgSrcId)).Length;
		}

		/* properties */

		protected Dictionary<TE, FieldData<TE>> Rows
		{
			get => rows;
			set => rows = value;
		}

		/// <summary>
		/// flags that the workbook has not been populated with data<br/>
		/// </summary>
		public bool IsEmpty { get; protected set; }

		/* Data storage */

		// public DataStorage? ExsDataStorage
		// {
		// 	get => exsDataStorage;
		// 	set
		// 	{
		// 		exsDataStorage = value;
		// 		// updatePopulate();
		// 	}
		// }

		// public Entity? ExsEntity
		// {
		// 	get => exsEntity;
		// 	set
		// 	{
		// 		exsEntity = value;
		// 		// updatePopulate();
		// 	}
		// }

		public abstract bool IsModifiedExo { get; set; }

		/* shortcuts */

		public abstract string DsName { get; }
		public abstract string Desc { get; set; }

		public abstract string DsSearchName { get; }
		public abstract string SchemaDesc { get; }
		public abstract Guid SchemaGuid { get; }

		/* rows */

		public IEnumerator<KeyValuePair<TE, FieldData<TE>>> GetEnumerator()
		{
			return Rows.GetEnumerator();
		}

		// Implementation of the non-generic IEnumerable (required for compatibility)
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/* required properties */

		public abstract string DateModified { get; set; }
		public abstract string NameModified { get; set; }

		public abstract void SetDateModifiedByInternal(string value, ChgSrcId cs);
		public abstract void SetNameModifiedInternal(string value, ChgSrcId cs);

		public abstract FieldData<TE> DateModifiedField { get; }
		public abstract FieldData<TE> NameModifiedField { get; }

		/* methods */

		public void SetTrackChanges()
		{
			foreach ((TE? key, FieldData<TE>? value) in Rows)
			{
				value.DyValue!.SetTrackChanges();
			}
		}

		public int RowCount => Rows.Count;

		protected void SetFieldCtrlByAlt(TE which)
		{
			FieldData<TE> fd = rows[which];
		}

		public bool SetInitValueDym(TE key, dynamic dv)
		{
			if (!(Rows?.ContainsKey(key) ?? false)) return false;

			FieldData<TE> field = Rows[key];

			if (field.DyValue.TrackChanges)
				throw new InvalidOperationException($"Use {nameof(SetNewValueDyn)}() to change the field's value");

			field.DyValue.SetValue(dv!, ChgSrcId.CI_NONE);

			Rows[key] = field;

			return true;
		}

		/// <summary>
		/// update the value of the DynaValue with the dynamic provided<br/>
		/// the ChgSrc is set to the value provided
		/// </summary>
		public bool SetNewValueDyn(FieldData<TE> field, dynamic dv, ChgSrcId cs) //, bool validate = true)
		{
			R.AddRouteEnter();

			if (!field.DyValue!.TrackChanges)
			{
				R.AddRouteExit("no track changes exit");
				return false;
			}

			field.DyValue.ChangeValue(dv!, cs);

			R.AddRouteExit();

			return true;
		}

		public FieldData<TE> GetField(TE key)
		{
			if (!(Rows?.ContainsKey(key) ?? false)) return FieldData<TE>.Empty();

			return Rows[key];
		}

		public DynaValue? GetValue(TE key)
		{
			FieldData<TE> row = GetField(key);

			return row.DyValue;
		}

		private void addValue(TE key, FieldDef<TE> field, dynamic? dy = null)
		{
			if (Rows.ContainsKey(key)) return;

			if (dy != null && !dy.GetType() != field.FieldType) return;

			FieldData<TE> f = new FieldData<TE>(field, dy ?? field.FieldDefValue!.Value);

			Rows.Add(key, f);
		}

		/* initialize row data */

		internal void init(Dictionary<TE, FieldDef<TE>> f)
		{
			IsEmpty = true;

			foreach ((TE key, FieldDef<TE> value) in f)
			{
				addValue(key, value);
			}
		}

		protected bool isModifiedExo
		{
			get => isModExo;
			set { isModExo = value; }
		}

		public bool UndoBtnStatus
		{
			get => undoBtnStatus;
			set
			{
				if (value == undoBtnStatus) return;

				R.AddRoute();

				undoBtnStatus = value;
				OnPropertyChanged();
			}
		}

		public bool ApplyBtnStatus
		{
			get => applyBtnStatus;
			set
			{
				if (value == applyBtnStatus) return;

				R.AddRoute();

				applyBtnStatus = value;
				OnPropertyChanged();
			}
		}

		/* modified date routines */

		/// <summary>
		/// update the modify date to a current value<br/>
		/// to be called only from validate - use _obj.ModDate field for UI changes
		/// </summary>
		public void ModDate_Update()
		{
			R.AddRoute();

			SetDateModifiedByInternal(ExStorConstFaux.FauxModDate, ChgSrcId.CI_SRC_T);
		}

		/// <summary>
		/// undo the date modified - to be called from validate and UI<br/>
		/// </summary>
		public void ModDate_Undo()
		{
			R.AddRoute();

			UndoChange(DateModifiedField, true);

			OnPropertyChanged(nameof(DateModified));
		}

		/// <summary>
		/// apply the modified date to be called by the UI<br/>
		/// </summary>
		public void ModDate_Apply()
		{
			R.AddRoute();

			// cannot use ApplyChange as that then updates
			// the mod dete and then does validate change status
			// in addition, chis applies the change and sets the prior value to null
			// so the current prior values must be saved in order to revert
			DateModifiedField.ApplyChg();
		}

		/* modified name routines */

		/// <summary>
		/// update the modified name to the current user<br/>
		/// to be called only from validate - use _obj.ModName field for UI changes
		/// </summary>
		public void ModName_Update()
		{
			R.AddRoute();

			if (NameModifiedField.ChgSrc != ChgSrcId.CI_NONE && NameModifiedField.ChgSrc != ChgSrcId.CI_SRC_T)
			{
				R.AddRoute("name mod - early exit - name not changed");
				return;
			}

			SetNameModifiedInternal(ExStorConstFaux.FauxUserName, ChgSrcId.CI_SRC_T);
		}

		/// <summary>
		/// undo the name modified - to be called from validate and the UI<br/>
		/// set the chgSrcId &lt;= srcIdIn
		/// </summary>
		public void ModName_Undo()
		{
			R.AddRoute();

			UndoChange(NameModifiedField, true);

			OnPropertyChanged(nameof(NameModifiedField));
		}

		/// <summary>
		/// apply the modified name - to be called from the UI<br/>
		/// </summary>
		public void ModName_Apply()
		{
			R.AddRoute();

			NameModifiedField.ApplyChg();
		}

		[DebuggerStepThrough]
		protected void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}
		public event PropertyChangedEventHandler? PropertyChanged;

		/// <summary>
		/// undo a single field change<br/>
		/// undoes change soruce<br/>
		/// performs a validate unless suppressValidate is true
		/// </summary>
		public void UndoChange(FieldData<TE> fd, bool suppressValidate)
		{
			R.AddRoute();

			fd.UndoChg();

			if (!suppressValidate)
			{
				ValidateChanges(ChangeType.CT_UNDO);
			}

			OnPropertyChanged(fd.Field.FieldPropName);
		}

		/// <summary>
		/// apply a single field change<br/>
		/// applies change source<br/>
		/// performs a validate unless suppressValidate is true
		/// </summary>
		public void ApplyChange(FieldData<TE> fd, bool suppressValidate)
		{
			R.AddRoute();

			fd.ApplyChg();

			if (!suppressValidate)
			{
				ValidateChanges(ChangeType.CT_APPLY);
			}

			OnPropertyChanged(fd.Field.FieldPropName);
		}

		/// <summary>
		/// undo the change in the local copy to all fields
		/// this suppresses validate for all fields and runs
		/// validate only at the end
		/// </summary>
		public void UndoChangesAll()
		{
			R.AddRoute();

			if (!isModifiedExo) return;

			foreach ((TE? key, FieldData<TE>? fd) in rows)
			{
				if (fd.DyValue!.IsDirty)
				{
					if (fd.ChgSrc == ChgSrcId.CI_SRC_A
						|| fd.ChgSrc == ChgSrcId.CI_SRC_X
						|| fd.ChgSrc == ChgSrcId.CI_SRC_D
						)
					{
						UndoChange(fd, true);
					}
				}
			}

			ValidateChanges(ChangeType.CT_UNDO);
		}

		/// <summary>
		/// apply the change in the local copy to all fields</br>
		/// this suppresses validate for all fields and runs
		/// validate only at the end
		/// </summary>
		public void ApplyChangesAll()
		{
			R.AddRoute();

			if (!isModifiedExo) return;

			foreach ((TE? key, FieldData<TE>? fd) in rows)
			{
				if (fd.IsDirty())
				{
					if (fd.ChgSrc == ChgSrcId.CI_SRC_A
						|| fd.ChgSrc == ChgSrcId.CI_SRC_X
						|| fd.ChgSrc == ChgSrcId.CI_SRC_D
						)
					{
						ApplyChange(fd, true);
					}
				}
			}

			ValidateChanges(ChangeType.CT_APPLY);
		}

		/// <summary>
		/// validate the status of all of the fields<br/>
		/// got change | true = a field has changed | null = doing an apply | false = doing an undo
		/// </summary>
		public void ValidateChanges(ChangeType gotChgType)
		{
			R.AddRouteEnter();

			string s = gotChgType.ToString();

			int[] chgSrcs = new int[_srcEnumLen];
			int count = 0;

			foreach ((TE? key, FieldData<TE>? fd) in rows)
			{
				if (!fd.DyValue!.IsDirty) continue;

				chgSrcs[(int) fd.ChgSrc]++;
				count++;
			}

			if (chgSrcs[(int) ChgSrcId.CI_SRC_E] > 0 && gotChgType == ChangeType.CT_CHANGE)
			{
				R.AddRoute($"got chgsrc E | chg type {gotChgType}");

				R.AddRoute("update mod date and name");
				ModDate_Update();
				ModName_Update();

				IsModifiedExo = true;

				ApplyBtnStatus = false;
				UndoBtnStatus = false;

				// no further processing
			}
			else if (chgSrcs[(int) ChgSrcId.CI_SRC_A] > 0
					|| (chgSrcs[(int) ChgSrcId.CI_SRC_X] > 0 &&
					gotChgType == ChangeType.CT_CHANGE)
					)
			{
				R.AddRoute($"got chgsrc A | chg type {gotChgType}");

				if (gotChgType == ChangeType.CT_CHANGE)
				{
					ModDate_Update();
					ModName_Update();
				}
				else if (gotChgType == ChangeType.CT_SHT_UNDO)
				{
					ModDate_Undo();
					ModName_Undo();
				}
				else if (gotChgType == ChangeType.CT_SHT_APPLY)
				{
					ModDate_Apply();
					ModName_Apply();
				}

				IsModifiedExo = true;
				ApplyBtnStatus = true;
				UndoBtnStatus = true;

				// no further processing
			}
			else if (chgSrcs[(int) ChgSrcId.CI_SRC_D] > 0)
			{
				R.AddRoute($"got chgsrc A | chg type {gotChgType}");

				if (gotChgType == ChangeType.CT_CHANGE)
				{
					ModDate_Update();
					ModName_Update();

					IsModifiedExo = true;
					ApplyBtnStatus = true;
					UndoBtnStatus = true;
				}
				else if (gotChgType == ChangeType.CT_APPLY)
				{
					ModDate_Apply();
					ModName_Apply();

					IsModifiedExo = false;
					ApplyBtnStatus = false;
					UndoBtnStatus = false;
				}
				else
				{
					ModDate_Undo();
					ModName_Undo();

					IsModifiedExo = false;
					ApplyBtnStatus = false;
					UndoBtnStatus = false;
				}

				// no further processing
			}
			else if (chgSrcs[(int) ChgSrcId.CI_SRC_T] > 0 || chgSrcs[(int) ChgSrcId.CI_SRC_X] > 0)
			{
				R.AddRoute($"got chgsrc T or X | chg type {gotChgType}");

				if (gotChgType == ChangeType.CT_UNDO || gotChgType == ChangeType.CT_SHT_UNDO)
				{
					ModDate_Undo();
					ModName_Undo();
				}
				else
				{
					ModDate_Apply();
					ModName_Apply();
				}

				IsModifiedExo = false;
				ApplyBtnStatus = false;
				UndoBtnStatus = false;

				// no further processing
			}
			else
			{
				IsModifiedExo = false;
				ApplyBtnStatus = false;
				UndoBtnStatus = false;

				// no further processing
			}

			R.AddRouteExit();
		}
	}
}