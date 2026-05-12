using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UtilityLibrary;

// Solution:     ExStorage
// Project:       ExStoreTest2026
// File:             FamAndType.cs
// Created:      2026-03-26 (19:03)

namespace ExStorSys;

public class FamAndType : INotifyPropertyChanged
{
	private bool isNewItemFat;
	private bool isModifiedFat;
	private string key;
	private string famName;
	private string? typeName;
	private string? properties;

	public string Key
	{
		get => key;
		private set
		{
			if (value.Equals(key)) return;
			key = value;
			OnPropertyChanged();
		}
	}

	public string FamName
	{
		get => famName;
		set
		{
			if (value.Equals(famName)) return;

			famName = value;
			OnPropertyChanged();
			updateKey();
			IsModifiedFat = true;
		}
	}

	public string? TypeName
	{
		get => typeName;
		set
		{
			if (value != null && value.Equals(typeName)) return;

			typeName = value;
			OnPropertyChanged();
			updateKey();
			IsModifiedFat = true;
		}
	}

	public string? Properties
	{
		get => properties;

		// ReSharper disable once PropertyCanBeMadeInitOnly.Global
		// ReSharper disable once MemberCanBePrivate.Global
		set
		{
			if (value != null && value.Equals(properties)) return;

			properties = value;
			OnPropertyChanged();

			IsModifiedFat = true;
		}
	}

	public bool IsNewItemFat 
	{
		get => isNewItemFat;
		set
		{
			// if (!value) return;
			isNewItemFat = value;
			OnPropertyChanged();
		}
	}

	public bool IsModifiedFat
	{
		get => isModifiedFat;
		private set
		{
			if (value == isModifiedFat) return;
			isModifiedFat = value;
			OnPropertyChanged();
			raiseModifiedEvent();
		}
	}

	private FamAndType(string famName, string? typeName, string? properties)
	{
		this.famName = famName;
		this.typeName = typeName;
		this.properties = properties;
		isNewItemFat = false;
		isModifiedFat = false;
		key = "";
		updateKey();
	}

	public static FamAndType Invalid()
	{
		return new ("", null, null);
	}

	public static FamAndType GetNewItem(string fn, string? tn, string? pr)
	{
		FamAndType fat = new (fn, tn, pr);
		// {
		// 	IsNewItemFat = true
		// };

		fat.IsNewItemFat = true;

		return fat;
	}

	public static FamAndType GetExistItem(string fn, string? tn, string? pr)
	{
		if (fn.IsVoid()) return new ("", null, null);

		FamAndType fat = new (fn, tn, pr);
		return fat;
	}

	private void updateKey()
	{
		Key = ExStorLib.FormatFamAndType(famName, typeName);
	}

	public override string ToString()
	{
		return $"{FamName} / {TypeName} / {IsNewItemFat} / {IsModifiedFat}";
	}

	public event PropertyChangedEventHandler? PropertyChanged ;

	[DebuggerStepThrough]
	private void OnPropertyChanged([CallerMemberName] string memberName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
	}


	public delegate void ModifiedEventHandler(object sender);

	public static event ModifiedEventHandler? Modified;

	private void raiseModifiedEvent()
	{
		Modified?.Invoke(this);
	}
}