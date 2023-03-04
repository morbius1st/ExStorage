// Solution:     ExStorage
// Project:       ExStorage
// File:             AShScLock.cs
// Created:      2022-12-31 (3:51 PM)

using System;
using System.Collections.Generic;
using ShExStorageC.ShSchemaFields;
using ShExStorageN.ShExStorage;

namespace ShExStorageN.ShSchemaFields
{
	public abstract class AShScLock<TKey, TField> : 
		AShScFields<TKey, TField>
		where TKey : Enum
		where TField : ScFieldDefData<TKey>, new()
	{
		protected AShScLock()
		{
			// Fields = new Dictionary<TKey, TField>();

			init();
		}

		// public override Dictionary<TKey, TField> Fields { get; protected set; }

		protected abstract void init();

		public abstract void Configure(LokExId lokExId);

		public object Clone()
		{
			return new object();
		}
	}
}