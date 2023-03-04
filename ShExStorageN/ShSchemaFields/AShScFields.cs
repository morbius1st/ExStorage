using System;
using System.Collections;
using System.Collections.Generic;
using Autodesk.Revit.DB.ExtensibleStorage;
using ShExStorageN.ShSchemaFields.ShScSupport;


// Solution:     ExStorage
// Project:       ExStoreDev
// File:             AShScFields.cs
// Created:      2023-01-15 (8:15 AM)

namespace ShExStorageN.ShSchemaFields
{

	public abstract class AShScFields2<TKey, TField, TData> : AShScFields<TKey, TField> 
		where TKey : Enum
		where TField : IShScFieldBase<TKey>, new()
	where TData : AShScFields2<TKey, TField, TData>, new()
	{
		// public object Clone()
		// {
		// 	TData copy = new TData();
		// 	copy.Fields = CloneFields();
		//
		// 	copy.UserName = "";
		// 	copy.Date = DateTime.Now;
		// 	;
		//
		// 	return copy;
		// }
	}



	/// <summary>
	/// basic abstract class for either sheet or rows<br/>
	///	has the basic collection of fields<br/>
	/// includes some pre-defined routines<br/>
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TField"></typeparam>
	public abstract class AShScFields<TKey, TField> : 
		IShScFieldsBase<TKey, TField> , IEnumerable<KeyValuePair<TKey, TField>>
		where TKey : Enum
		where TField : IShScFieldBase<TKey>, new()
	{
		protected AShScFields()
		{
			Fields = new Dictionary<TKey, TField>();
		}

		public string SchemaName
		{
			get { return GetValue<string>(ShExConstN.K_SCHEMA_NAME); }
		protected set  => SetValue((ShExConstN.K_SCHEMA_NAME), value);
		}

		public string SchemaDesc
		{
			get { return GetValue<string>(ShExConstN.K_DESCRIPTION); }
		}

		// protected set  => SetValue((ShExConstN.K_DESCRIPTION), value);
		public Guid SchemaGuid
		{
			get { return GetValue<Guid>(ShExConstN.K_GUID); }
		}

		// protected set  => SetValue((ShExConstN.K_GUID), value);
		public string UserName
		{
			get { return GetValue<string>(ShExConstN.K_USERNAME); }
		}

		// protected set  => SetValue((ShExConstN.K_USERNAME), value);
		public string SchemaKey
		{
			get { return GetValue<string>(ShExConstN.K_KEY); }
		}

		// protected set  => SetValue((ShExConstN.K_KEY), value);
		public string SchemaVersion
		{
			get { return GetValue<string>(ShExConstN.K_VERSION); }
		}

		// protected set  => SetValue((ShExConstN.K_VERSION), value);
		public string ModelName
		{
			get { return GetValue<string>(ShExConstN.K_MODEL_NAME); }
		}

		// protected set  => SetValue((ShExConstN.K_MODEL_NAME), value);
		public string ModelPath
		{
			get { return GetValue<string>(ShExConstN.K_MODEL_PATH); }
		}

		// protected set => SetValue((ShExConstN.K_MODEL_PATH), value);
		public string Date
		{
			get { return GetValue<string>(ShExConstN.K_DATE); }
		}

		// protected set  => SetValue((ShExConstN.K_DATE), value);
		public virtual dynamic GetValueAs<T>(Enum fieldKey)
		{
			// return Fields[(TKey) fieldKey].DyValue.Value;
			return Fields[(TKey) fieldKey].DyValue.GetValueAs<T>();
		}

		public virtual TField GetField(Enum fieldKey)
		{
			TField f = Fields[(TKey) fieldKey];

			return f;
		}

		public virtual void SetValue(TKey FieldKey, dynamic value)
		{
			Fields[FieldKey].SetValue = value;
		}

		public virtual void Add(TField field)
		{
			Fields.Add(field.FieldKey, field);
		}

		protected Dictionary<TKey, TField> CloneFields()
		{
			Dictionary<TKey, TField> copy = new Dictionary<TKey, TField>();

			foreach (KeyValuePair<TKey, TField> kvp in Fields)
			{
				TField tf = (TField) kvp.Value.Clone();


				copy.Add(kvp.Key, (TField) kvp.Value.Clone());
			}

			return copy;
		}


		protected Dictionary<TKey, TField> Fields { get; set; }

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		public IEnumerator<KeyValuePair<TKey, TField>> GetEnumerator()
		{
			return Fields.GetEnumerator();
		}


		// abstract

		public abstract void SetValue(int FieldKey, dynamic value);

		public abstract T GetValue<T>(int key);
		public abstract T GetValue<T>(TKey key);

		public abstract void ParseEnum(Type t, string enumName);

	}
}