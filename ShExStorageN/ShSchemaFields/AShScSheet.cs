using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ShScSupport;
using ShExStorageN.ShSchemaFields.ShScSupport;

// Solution:     ExStorage
// Project:       ExStorage
// File:             AShScSheet.cs
// Created:      2022-12-31 (3:50 PM)

namespace ShExStorageN.ShSchemaFields
{
	

	/// <summary>
	/// abstract class for a sheet - implements AShScFields & IShScRows1<br/>
	/// adds the collection of rows and the routine to add a row
	/// </summary>
	public abstract class AShScSheet<TShtKey, TShtFlds, TRowKey, TRowFlds, TSht, TRow> :
		AShScFields<TShtKey, TShtFlds>,
		// IShScRows<TRowKey, TRowFlds, TRow>,
		INotifyPropertyChanged
		where TShtKey : Enum
		where TRowKey : Enum
		where TShtFlds : ScFieldDefData<TShtKey>, new()
		where TRowFlds : ScFieldDefData<TRowKey>, new()
		where TSht : AShScSheet<TShtKey, TShtFlds, TRowKey, TRowFlds, TSht, TRow>, new()
		where TRow : AShScRow<TRowKey, TRowFlds>, new()
	{

		public AShScSheet()
		{
			// Fields = new Dictionary<TShtKey, TShtFlds>();

			// init();
		}

		// protected abstract void init();

		protected bool hasData;


		public bool  HasData
		{
			get => hasData;
			set
			{
				hasData = value;
				OnPropertyChanged();
			}
		}

		// private override Dictionary<TShtKey, TShtFlds> Fields { get; protected set; }

		public Dictionary<string, TRow> Rows { get; protected set; }

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

		public abstract void AddRow(TRow row);

		public object Clone()
		{
			TSht copy = new TSht();
		
			copy.Fields = CloneFields();
			copy.hasData = hasData;
		
			copy.Rows = new Dictionary<string, TRow>(Rows.Count);
		
			foreach (KeyValuePair<string, TRow> kvp in Rows)
			{
				copy.Rows.Add(kvp.Key, (TRow) kvp.Value.Clone());
			}
		
			return copy;
		}
	}



	// /// <summary>
	// /// abstract class for a sheet - implements AShScFields & IShScRows1<br/>
	// /// adds the collection of rows and the routine to add a row
	// /// </summary>
	// public abstract class AShScSheet<TShtKey, TShtFlds, TRowKey, TRowFlds, TRow> :
	// 	AShScFields<TShtKey, TShtFlds>,
	// 	// IShScRows<TRowKey, TRowFlds, TRow>,
	// 	INotifyPropertyChanged
	// 	where TShtKey : Enum
	// 	where TRowKey : Enum
	// 	where TShtFlds : ScFieldDefData<TShtKey>, new()
	// 	where TRowFlds : ScFieldDefData<TRowKey>, new()
	// 	where TRow : AShScRow<TRowKey, TRowFlds>, new()
	// {
	//
	// 	public AShScSheet()
	// 	{
	// 		// Fields = new Dictionary<TShtKey, TShtFlds>();
	//
	// 		init();
	// 	}
	//
	// 	protected abstract void init();
	//
	// 	protected bool hasData;
	//
	//
	// 	public bool  HasData
	// 	{
	// 		get => hasData;
	// 		set
	// 		{
	// 			hasData = value;
	// 			OnPropertyChanged();
	// 		}
	// 	}
	//
	// 	// private override Dictionary<TShtKey, TShtFlds> Fields { get; protected set; }
	//
	// 	public Dictionary<string, TRow> Rows { get; protected set; }
	//
	// 	public event PropertyChangedEventHandler PropertyChanged;
	//
	// 	private void OnPropertyChanged([CallerMemberName] string memberName = "")
	// 	{
	// 		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
	// 	}
	//
	// 	public abstract void AddRow(TRow row);
	//
	// 	protected abstract AShScSheet<TShtKey, TShtFlds, TRowKey, TRowFlds, TRow> CopyForClone();
	//
	// 	public object Clone()
	// 	{
	// 		AShScSheet<TShtKey, TShtFlds, TRowKey, TRowFlds, TRow> copy = CopyForClone();
	//
	// 		copy.Fields = CloneFields();
	// 		copy.hasData = hasData;
	//
	// 		copy.Rows = new Dictionary<string, TRow>(Rows.Count);
	//
	// 		foreach (KeyValuePair<string, TRow> kvp in Rows)
	// 		{
	// 			copy.Rows.Add(kvp.Key, (TRow) kvp.Value.Clone());
	// 		}
	//
	// 		return copy;
	// 	}
	// }


}