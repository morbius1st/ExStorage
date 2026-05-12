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

	private FieldData() { }

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

	/* events */

	public event PropertyChangedEventHandler? PropertyChanged;

	[DebuggerStepThrough]
	private void OnPropertyChanged([CallerMemberName] string memberName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
	}


	/* methods */

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void UndoChg()
	{
		// R.AddRouteEnter($"[{Field.FieldName}] | is clean? [{DyValue!.IsClean}] true => do NOT undo - exit");
		R.AddRoute($"{Field.FieldName} | is clean? [{DyValue!.IsClean}] true => do NOT undo - exit", 2, true);

		if (DyValue!.IsClean)
		{
			// R.AddRouteExit();
			return;
		}

		DyValue.UndoChange();
		// R.AddRouteExit();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void ApplyChg()
	{
		// R.AddRouteEnter($"[{Field.FieldName}]");
		R.AddRoute($"{Field.FieldName}", 2, true);

		if (DyValue!.IsClean)
		{
			// R.AddRouteExit();
			return;
		}

		DyValue.ApplyChange();

		// R.AddRouteExit();
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

	public SourceId ChgSrcId { get; set; }


}