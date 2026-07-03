using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UtilityLibrary;

// Solution:     ExStorage
// Project:       ExStoreTest2026
// File:             FieldData.cs
// Created:      2025-09-25 (19:09)

namespace ExStorSys;

public class FieldData<Te> : INotifyPropertyChanged
	where Te : Enum
{
	// private 

	/* ctor */

	private FieldData() { }

	public FieldData(FieldDef<Te>? field, DynaValue? dyValue)
	{
		Field = field;
		DyValue = dyValue;
	}

	public FieldData(FieldDef<Te>? field, dynamic? value)
	{
		Field = field;
		DyValue = new DynaValue(value);
	}

	/* properties */

	/// <summary>
	/// the dynamic value object
	/// </summary>
	public DynaValue? DyValue { get; private set; }

	public FieldDef<Te>? Field { get; }

	/* events */

	public event PropertyChangedEventHandler? PropertyChanged;

	[DebuggerStepThrough]
	private void OnPropertyChanged([CallerMemberName] string memberName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
	}

	/* methods */

	/// <summary>
	/// undo the last change and replace with the last saved value<br/>
	/// also undoes the chg src
	/// </summary>
	public void UndoChg()
	{
		if (DyValue!.IsClean) return;

		DyValue.UndoChange();
	}

	public static FieldData<Te> Empty()
	{
		return new FieldData<Te>(null, null);
	}


	/// <summary>
	/// applies the last change.  ChgSrc is set to none
	/// </summary>
	public void ApplyChg()
	{
		// R.AddRouteEnter(0, $"before {Field.FieldName} [ {Field.FieldChgSrcId} ]");

		if (DyValue!.IsClean)
		{
			// R.AddRouteExit(0, "is clean - early exit");
			return;
		}

		DyValue.ApplyChange();

		// R.AddRouteExit(0, $"before {Field.FieldName} [ {Field.FieldChgSrcId} ]");
	}

	public bool IsDirty()
	{
		return DyValue.IsDirty;
	}

	/// <summary>
	/// the source of the change to the dynaValue
	/// </summary>
	public ChgSrcId ChgSrc
	{
		get => DyValue!.ChgSrc;
		set => DyValue!.ChgSrc = value;
	}

	/// <summary>
	/// the source of the change to the dynaValue and set the IsDirtyFlag
	/// </summary>
	public ChgSrcId ChgSrcDirty
	{
		get => DyValue!.ChgSrc;
		set { DyValue!.ChgSrcDirty = value; }
	}

	/// <summary>
	/// the source of the change to the dynaValue
	/// </summary>
	public ChgSrcId ChgSrcStd => Field!.FieldChgSrcId[0];

	/// <summary>
	/// the source of the change to the dynaValue
	/// </summary>
	public ChgSrcId ChgSrcAlt => Field!.FieldChgSrcId[1];

	public void ApplyChgSrc()
	{
		DyValue!.ApplyChgSrcId();
	}

	public void UndoChgSrc()
	{
		DyValue!.UndoChgSrcId();
	}
}