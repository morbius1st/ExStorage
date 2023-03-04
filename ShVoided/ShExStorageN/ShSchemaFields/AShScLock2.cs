// Solution:     ExStorage
// Project:       ExStorage
// File:             AShScLock.cs
// Created:      2022-12-31 (3:51 PM)

namespace ShExStorageN.ShSchemaFields
{
	public abstract class AShScLock2<TField> : 
		AShScFields2<TField>
		where TField : IShScFieldBase2, new()
	{
		protected AShScLock2()
		{
			Fields = new Dictionary<KEY, TField>(kc);

			init();
		}

		public override Dictionary<KEY, TField> Fields { get; protected set; }

		protected abstract void init();

		public abstract void Configure(LokExId lokExId);

		// public abstract string MachineName { get; }
	}
}