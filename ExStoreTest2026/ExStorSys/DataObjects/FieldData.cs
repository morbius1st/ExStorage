// Solution:     ExStorage
// Project:       ExStoreTest2026
// File:             FieldData.cs
// Created:      2025-09-25 (19:09)

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB.ExtensibleStorage;
using JetBrains.Annotations;
using UtilityLibrary;

namespace ExStorSys;

// public class WbkFieldData : FieldData<WorkBookFieldKeys>
// {
// 	public WbkFieldData(FieldDef<WorkBookFieldKeys>? field, DynaValue? dyValue) 
// 		: base(field, dyValue) { }
// 	public WbkFieldData(FieldDef<WorkBookFieldKeys>? field, dynamic? dymValue) 
// 		: base(field, (object) dymValue) { }
// }

public class FieldData<Te> : INotifyPropertyChanged
	where Te : Enum
{
	// private 

	public const int USR_SRC_IDX = 0;
	public const int ALT_SRC_FIRST_IDX = 1;

	private int fieldSrcLen = 0;

	// list of sources that have caused this item's value to be changed
	private bool[] fieldSources;

	/* ctor */

	private FieldData() {}

	public FieldData(FieldDef<Te>? field, DynaValue? dyValue)
	{
		Field = field;
		DyValue = dyValue;

		fieldSources = null;
	}

	public FieldData(FieldDef<Te>? field, dynamic? value)
	{
		Field = field;
		DyValue = new DynaValue(value);

		fieldSources = null;
	}


	/* properties */


	/// <summary>
	/// the dynamic value object
	/// </summary>
	public DynaValue? DyValue { get; private set; }

	public FieldDef<Te>? Field { get; }

	// src array size (in fields)
	// src array (in here)
	// ctrl array (in fields)

	/// <summary>
	/// the number of elements in the field source array
	/// </summary>
	public int FieldSrcArraySize 
	{
		get => fieldSrcLen;
		set
		{
			if (fieldSrcLen != 0) return;
			fieldSrcLen = value;
			fieldSources = new bool[fieldSrcLen];
		}
}


	/* events */

	public event PropertyChangedEventHandler? PropertyChanged;

	[DebuggerStepThrough]
	[NotifyPropertyChangedInvocator]
	private void OnPropertyChanged([CallerMemberName] string memberName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
	}


	/* methods */

	public void UndoChg()
	{
		if (DyValue!.IsClean) return;

		DyValue.UndoChange();
	}

	public static FieldData<Te> Empty()
	{
		return new FieldData<Te>(null, null);
	}

	/* methods for revised / undo / apply processing */

	public bool SetChgSource(int srcIdx)
	{
		if (!Field!.CanSetSrcIdx(srcIdx)) return false; // setting allowed?
		if (fieldSources[srcIdx]) return false;			// was already set, cannot re-set

		fieldSources[srcIdx] = true;

		return true;
	}

	public bool UnSetChgSource(int srcIdx)
	{
		if (!Field!.CanSetSrcIdx(srcIdx)) return false; // setting allowed?
		if (!fieldSources[srcIdx]) return false;		// was not set, cannot unset

		fieldSources[srcIdx] = false;

		return true;
	}

	public bool IsChgSrcSet(int srcIdx)
	{
		if (!Field!.CanSetSrcIdx(srcIdx)) return false; // setting allowed?

		return fieldSources[srcIdx];
	}

	public void ClrChgSrc()
	{
		fieldSources = new bool[fieldSrcLen];
	}

	public bool SetUsrChgSrc()
	{
		return SetChgSource(USR_SRC_IDX);
	}

	public bool UnSetUsrChgSrc()
	{
		return UnSetChgSource(USR_SRC_IDX);
	}

	public bool IsUsrChgSrcSet()
	{
		return fieldSources[USR_SRC_IDX];
	}

	public bool IsAnyAltSourceSet()
	{
		for (var i = ALT_SRC_FIRST_IDX; i < fieldSources.Length; i++)
		{
			if (fieldSources[i]) return true;
		}

		return false;
	}

	public bool IsModified()
	{
		return DyValue.IsDirty;
	}


	// removed
	//
	// public bool IsCtrlField => Field!.FieldUse.HasFlag(ItemUsage.IU_IS_CTRL_FLD);
	// public bool IsAltSrcA   => Field!.FieldUse.HasFlag(ItemUsage.IU_IS_ALT_SRC_A);
	// public bool IsAltSrcB => Field!.FieldUse.HasFlag(ItemUsage.IU_IS_ALT_SRC_B);
	//
	// // possible shortcuts
	//
	// // set flag, by user
	// // set flag, by alt src A
	// // set flag, by alt src B
	//
	// public bool GotAltSrcA => Field!.TstFcFlag(FieldControl.FC_BY_ALT_SRC_A);
	// public bool GotAltSrcB => Field!.TstFcFlag(FieldControl.FC_BY_ALT_SRC_B);
	//
	// public void SetByUser()    => Field!.SetFcFlag(FieldControl.FC_BY_USER);
	// public void SetByAltSrcA() => Field!.SetFcFlag(FieldControl.FC_BY_ALT_SRC_A);
	// public void SetByAltSrcB() => Field!.SetFcFlag(FieldControl.FC_BY_ALT_SRC_B);
	//
	// public void ClrFc() => Field!.ClrFc();


	// public DynaValue DynValue
	// {
	// 	get => DyValue;
	// 	set
	// 	{
	// 		if (DyValue != null && value.Value.Equals(DyValue.Value)) return;
	//
	// 		// Type t1 = value.TypeIs;
	// 		// Type t2 = DyValue!.TypeIs;
	//
	// 		if (value.TypeIs != DyValue.TypeIs) return;
	//
	// 		DyValue = value;
	//
	// 		OnPropertyChanged();
	// 	}
	// }


	// public FieldData(FieldDef<Te>? field, dynamic? dymValue)
	// {
	// 	Field = field;
	// 	DyValue = new DynaValue(dymValue);
	// }

}