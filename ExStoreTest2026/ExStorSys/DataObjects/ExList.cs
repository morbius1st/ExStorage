// Solution:     ExStorage
// Project:       ExStoreTest2026
// File:             ExList.cs
// Created:      2026-01-11 (18:01)

namespace ExStorSys;

/// <summary>
/// Ex Storage Element List Item of type (T) (schema or datastorage)<br/>
/// each item can have attributes:<br/>
/// version:   T == correct version (default) | F == wrong version<br/>
/// isvalid:   T == is valid (default) | F == is invalid<br/>
/// actoff:    T == activation is off | F activation is not off (default)<br/>
/// modelname: T == model name is correct (default) | F == model name is wrong
/// </summary>
/// <typeparam name="T"></typeparam>
public class ExListItem<T> 
	where T : class
{
	public ExListItem(string name, T item, bool version = true,
		bool isValid = true, bool actOff = false, 
		bool modelName = true)
	{
		Name = name;
		Item = item;
		Version = version;
		IsValid = isValid;
		ActOff = actOff;
		ModelName = modelName;
		// VerString = String.Empty;
	}

	public T Item { get; set; }

	public string Name { get; set; }

	// public string VerString { get; set; }

	/// <summary>
	/// applied to sht ds and wbk ds<br/>
	/// true, item has the correct version (default)<br/>
	/// false, item has the wrong version
	/// </summary>
	public bool Version { get; private set; }

	public bool SetWrongVersion() => Version = false;

	// sht only
	/// <summary>
	/// sht only
	/// true if valid (default)
	/// false if not valid
	/// </summary>
	public bool IsValid { get; private set; }

	public bool SetNotValid() => IsValid = false;
	public bool SetValid() => IsValid = true;

	/// <summary>
	/// wbk ds only
	/// true if activation is off (bad)
	/// false if activation is not off (good) (default)
	/// </summary>
	public bool ActOff { get; private set; }

	public bool SetActIsOff() => ActOff = true;

	/// <summary>
	/// wbk ds only
	/// true if model name is good (default)
	/// false if model name is not good 
	/// </summary>
	public bool ModelName { get; private set; }

	public bool SetWrongModelName() => ModelName = false;
}

public class ExList<T>
	where T : class
{
	public ExList()
	{
		GoodItems = new ();
		AllItems = new ();
		WrongVersionCount = 0;
		InvalidCount = 0;
	}

	public int AllItemsCount => AllItems.Count;
	public int GoodItemsCount => GoodItems.Count;

	public bool GotOneGoodItem => GoodItems.Count == 1;
	public bool GotGoodItems => GoodItems.Count > 0;

	public int WrongVersionCount { get; set; }
	public int InvalidCount { get; set; }

	// /// <summary>
	// /// true if wrong version count is greater than  0 or invalid count is greater than 0
	// /// </summary>
	// public bool GotNoGood => WrongVersionCount > 0 || InvalidCount > 0;

	public Dictionary<string, ExListItem<T>> GoodItems { get; set; }
	public Dictionary<string, ExListItem<T>> AllItems { get; set; }

	public void Add(ExListItem<T> item)
	{
		bool result = true;

		AllItems.Add(item.Name, item);

		if (!item.Version)
		{
			WrongVersionCount++;
		}

		if (!item.IsValid)
		{
			InvalidCount++;
			result = false;
		}

		if (item.ActOff) result = false;
		if (!item.ModelName) result = false;

		if (result) GoodItems.Add(item.Name, item);
	}

	public ExListItem<T>? GetGoodItem => GoodItems.Count > 0 ? GoodItems.First().Value : null;

	public static IList<T>? ToIList(Dictionary<string, ExListItem<T>>? list)
	{
		if (list == null) return null;

		IList<T> il = new List<T>();

		foreach ((string? key, ExListItem<T>? item) in list)
		{
			il.Add(item.Item);
		}

		return il;
	}
}