// Solution:     ExStorage
// Project:       ExStorage
// File:             AShScRow.cs
// Created:      2022-12-31 (3:50 PM)

namespace ShExStorageN.ShSchemaFields
{
	public abstract class AShScRow2<TField> : 
		AShScFields2<TField>
		where TField : IShScFieldBase2, new()
	{
		protected AShScRow2()
		{
			Fields = new Dictionary<KEY, TField>(kc);

			init();
		}

		public override Dictionary<KEY, TField> Fields { get; protected set; }

		protected abstract void init();
	}
}