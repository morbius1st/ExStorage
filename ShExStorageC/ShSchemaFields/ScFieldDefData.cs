#region + Using Directives
using System;
using System.Reflection.Emit;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShSchemaFields;

#endregion

// user name: jeffs
// created:   10/29/2022 5:33:05 PM



namespace ShExStorageC.ShSchemaFields
{
	public class ScFieldDefData<TKey> : IShScFieldData1<TKey>
	where TKey : Enum
	{
		public ScFieldDefData(){}
		public ScFieldDefData(TKey fieldKey, DynaValue dyValue, ScFieldDefMeta<TKey> metaField)
		{
			FieldKey = fieldKey;
			DyValue = dyValue;
			Meta1Field = metaField;
		}


		public TKey FieldKey { get; }
		public string FieldName => Meta1Field.FieldName;
		public string FieldDesc => Meta1Field.FieldDesc;
		public DynaValue DyValue { get; private set; }

		// public dynamic Value => DyValue.Value;
		public dynamic Value {
			get => DyValue.Value;
			set => DyValue = new DynaValue(value);
	}

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
			return $"name |   {$"\"{FieldName}\"",-20} | value| {Value}   ({Value.GetType().Name})";
		}

		public IShScFieldBase1<TKey> Clone()
		{
			ScFieldDefData<TKey> copy = new ScFieldDefData<TKey>(
				FieldKey, new DynaValue(Value), (ScFieldDefMeta<TKey>) Meta1Field.Clone());
			return copy;

		}
	}
}
