// Solution:     ExStorage
// Project:       ExStorage
// File:             AShScRow.cs
// Created:      2022-12-31 (3:50 PM)

using System;
using System.Collections.Generic;
using ShExStorageC.ShSchemaFields;

namespace ShExStorageN.ShSchemaFields
{
	public abstract class AShScRow<TKey, TField> : 
		AShScFields<TKey, TField>
		where TKey : Enum
		where TField : ScFieldDefData<TKey>, new()
	{
		protected AShScRow()
		{
			// Fields = new Dictionary<TKey, TField>();

			init();
		}

		// public override Dictionary<TKey, TField> Fields { get; protected set; }

		protected abstract void init();

		protected abstract  AShScRow<TKey, TField> NewForClone();


		public AShScRow<TKey, TField> Clone()
		{
			object o = new object();
			AShScRow<TKey, TField> a = NewForClone();

			a.Fields = CloneFields();

			return a;
		}

	}
}