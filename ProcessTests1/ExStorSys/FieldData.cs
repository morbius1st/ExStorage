using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UtilityLibrary;

// Solution:     ExStorage
// Project:       ExStoreTest2026
// File:             FieldData.cs
// Created:      2025-09-25 (19:09)

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

	/* ctor */

	private FieldData()
	{ }

	public FieldData(FieldDef<Te>? field, DynaValue? dyValue) 
	{
		Field = field;
		DyValue = dyValue;

		// fieldSources = null;
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

	public bool FieldCanEdit => ChgSrc <= ChgSrcId.CI_SRC_B;

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
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void UndoChg()
	{
		R.AddRoute(  $"{Field.FieldName} | is clean? [{DyValue!.IsClean}] true => do NOT undo - exit", 0, 2, true);

		if (DyValue!.IsClean) return;

		DyValue.UndoChange();
	}

	/// <summary>
	/// applies the last change.  ChgSrc is set to none
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	// public bool SourceIdOk(SourceId srcIdIn)
	// {
	// 	return srcIdIn > Field!.FieldSrcIdxMin && srcIdIn <= Field!.FieldSrcIdxMax;
	// }

	public static FieldData<Te> Empty()
	{
		return new FieldData<Te>(null, null);
	}

	/* methods for revised / undo / apply processing */

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
		set
		{
			DyValue!.ChgSrcDirty = value;
		}
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
