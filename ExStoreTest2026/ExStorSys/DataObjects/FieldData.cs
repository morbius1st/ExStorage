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
	public FieldDef<Te>? Field { get; }
	public DynaValue? DyValue { get; private set; }

	public FieldData() {}

	public FieldData(FieldDef<Te>? field, DynaValue? dyValue)
	{
		Field = field;
		DyValue = dyValue;
	}

	public FieldData(FieldDef<Te>? field, dynamic? dymValue)
	{
		Field = field;
		DyValue = new DynaValue(dymValue);
	}

	public DynaValue DynValue
	{
		get => DyValue;
		set
		{
			if (DyValue != null && value.Value.Equals(DyValue.Value)) return;
	
			// Type t1 = value.TypeIs;
			// Type t2 = DyValue!.TypeIs;
	
			if (value.TypeIs != DyValue.TypeIs) return;
	
			DyValue = value;
	
			OnPropertyChanged();
		}
	}

	// public dynamic DymValue
	// {
	// 	get => DyValue.Value;
	// 	set
	// 	{
	// 		if (DyValue != null && value.Equals(DyValue.Value)) return;
	//
	// 		if (!(value.GetType().Equals(DyValue.TypeIs))) return;
	//
	// 		DyValue.ChangeValue(value);
	//
	// 		OnPropertyChanged();
	// 	}
	// }
	//
	// public ActivateStatus ActStatus
	// {
	// 	get
	// 	{
	// 		if (Field.FieldType != typeof(ActivateStatus)) return ActivateStatus.AS_NA;
	//
	// 		return (ActivateStatus) DyValue.Value;
	// 	}
	// 	set
	// 	{
	// 		if (DyValue != null && value.Equals(DyValue.Value)) return;
	// 		if (!(value.GetType().Equals(DyValue.TypeIs))) return;
	// 		DyValue.ChangeValue(value);
	// 		OnPropertyChanged();
	// 	}
	// }

	public static FieldData<Te> Empty()
	{
		return new FieldData<Te>(null, null);
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	[DebuggerStepThrough]
	[NotifyPropertyChangedInvocator]
	private void OnPropertyChanged([CallerMemberName] string memberName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
	}
}