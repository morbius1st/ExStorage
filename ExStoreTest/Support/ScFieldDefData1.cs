#region + Using Directives
using System;
using System.Reflection.Emit;
using ShExStorageC.ShSchemaFields;
using ShExStorageN.ShSchemaFields;

#endregion

// user name: jeffs
// created:   10/29/2022 5:33:05 PM



namespace ShExStorageC.ShSchemaFields
{
	public class ScFieldDefData1<TKey> : IShScFieldData1<TKey>
	where TKey : Enum
	{
		public ScFieldDefData1(){}
		public ScFieldDefData1(TKey fieldKey, DynaValue dyValue, ScFieldDefMeta1<TKey> metaField)
		{
			FieldKey = fieldKey;
			DyValue = dyValue;
			Meta1Field = metaField;
		}


		public TKey FieldKey { get; }
		public string FieldName => Meta1Field.FieldName;
		public string FieldDesc => Meta1Field.FieldDesc;
		public dynamic Value => DyValue.Value;
		public DynaValue DyValue { get; private set; }

		public dynamic GetValueAs<T>()
		{
			return DyValue.GetValueAs<T>();
		}

		public dynamic SetValue
		{
			set
			{
				if (value != null && DyValue.TypeIs != null &&
					value.GetType() != DyValue.TypeIs)
				{
					throw new TypeAccessException();
				}

				DyValue = new DynaValue(value);
			}
		}

		public IShScFieldMeta1<TKey> Meta1Field { get; }

		public override string ToString()
		{
			return $"name | {FieldName} | value| type| {Value.GetType().Name} | value| {Value}";
		}

		public IShScFieldBase1<TKey> Clone()
		{
			ScFieldDefData1<TKey> copy = new ScFieldDefData1<TKey>(
				FieldKey, new DynaValue(Value), (ScFieldDefMeta1<TKey>) Meta1Field.Clone());
			return copy;

		}
	}
}
