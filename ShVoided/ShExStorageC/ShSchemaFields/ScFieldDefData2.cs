#region + Using Directives

#endregion

// user name: jeffs
// created:   10/29/2022 5:33:05 PM



namespace ShExStorageC.ShSchemaFields
{
	public class ScFieldDefData2 : IShScFieldData2
	{
		public ScFieldDefData2(){}
		public ScFieldDefData2(KEY fieldKey, 
			DynaValue dyValue, ScFieldDefMeta2 metaField)
		{
			FieldKey = fieldKey;
			DyValue = dyValue;
			Meta1Field = metaField;
		}


		public KEY FieldKey { get; }
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

		public IShScFieldMeta2 Meta1Field { get; }

		public override string ToString()
		{
			return $"name |   {$"\"{FieldName}\"",-20} | value| {Value}   ({Value.GetType().Name})";
		}

		public IShScFieldBase2 Clone()
		{
			ScFieldDefData2 copy = new ScFieldDefData2(
				FieldKey, new DynaValue(Value), (ScFieldDefMeta2) Meta1Field.Clone());
			return copy;

		}
	}
}
