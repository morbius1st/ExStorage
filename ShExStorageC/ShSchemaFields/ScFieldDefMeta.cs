#region + Using Directives
using System;

using ShExStorageN.ShSchemaFields;
using ShExStorageN.ShSchemaFields.ShScSupport;
#endregion

// user name: jeffs
// created:   10/29/2022 8:03:17 PM



namespace ShExStorageC.ShSchemaFields
{
	public class ScFieldDefMeta<TKey> : IShScFieldMeta<TKey>
		where TKey : Enum
	{

		public TKey FieldKey { get; }
		public string FieldName { get; }
		public string FieldDesc { get; }
		public DynaValue DyValue { get; set; }
		// public dynamic Value => DyValue.Value;
		public SchemaFieldDisplayLevel DisplayLevel { get; private set; }

		public ScFieldDefMeta(){}

		public ScFieldDefMeta(
			TKey fieldKey, 
			string fieldName, 
			string fieldDesc, 
			DynaValue dyValue,
				SchemaFieldDisplayLevel dl
			)
		{
			FieldKey = fieldKey;
			FieldName = fieldName;
			FieldDesc = fieldDesc;
			DyValue = dyValue;
			DisplayLevel = dl;
		}


		public dynamic GetValueAs<T>()
		{
			return DyValue.GetValueAs<T>();
		}

		public dynamic SetValue
		{
			set
			{
				if (value.GetType() != DyValue.TypeIs)
				{
					throw new TypeAccessException();
				}

				DyValue = new DynaValue(value);
			}
		}

		public IShScFieldBase<TKey> Clone()
		{
			return new ScFieldDefMeta<TKey>(
				FieldKey, FieldName, FieldDesc, new DynaValue(DyValue.Value),
				DisplayLevel);
		}

		public override string ToString()
		{
			return $"{nameof(ScFieldDefMeta<TKey>)} |   name| {$"\"{FieldName}\"",-20}| value| {DyValue.Value ?? "is null"}";
		}
	}
}
