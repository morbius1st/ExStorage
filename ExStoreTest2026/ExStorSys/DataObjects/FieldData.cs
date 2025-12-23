// Solution:     ExStorage
// Project:       ExStoreTest2026
// File:             FieldData.cs
// Created:      2025-09-25 (19:09)

using Autodesk.Revit.DB.ExtensibleStorage;

namespace ExStorSys;

public struct FieldData<Te> 
	where Te : Enum
{
	public FieldDef<Te> Field { get; }
	public DynaValue DyValue { get; set; }

	public FieldData(FieldDef<Te> field, DynaValue dyValue)
	{
		Field = field;
		DyValue = dyValue;
	}

	// note - must occur within a transaction
	// public DynaValue? GetValue(Entity e)
	// {
	// 	return ExStorLib.Instance.ReadField(e, this);
	// }

	// note - must occur within a transaction
	// public void SetValue(Entity e, Schema s, DynaValue value)
	// {
	// 	ExStorLib.Instance.WriteField(e, s, this, value);
	// }

	public static FieldData<Te> Empty()
	{
		return new FieldData<Te>(null, null);
	}
}