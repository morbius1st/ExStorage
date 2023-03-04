#region + Using Directives

#endregion

// user name: jeffs
// created:   10/29/2022 8:03:17 PM



namespace ShExStorageC.ShSchemaFields
{
	public class ScFieldDefMeta2 : IShScFieldMeta2
	{
		public KEY FieldKey { get; }
		public string FieldName { get; }
		public string FieldDesc { get; }
		public DynaValue DyValue { get; set; }
		public SchemaFieldDisplayLevel DisplayLevel { get; private set; }

		public ScFieldDefMeta2(
			KEY fieldKey, 
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

		public IShScFieldBase2 Clone()
		{
			return new ScFieldDefMeta2(
				FieldKey, FieldName, FieldDesc, new DynaValue(DyValue.Value),
				DisplayLevel);
		}

		public override string ToString()
		{
			return $"{nameof(ScFieldDefMeta2)} |   name| {$"\"{FieldName}\"",-20}| value| {DyValue.Value ?? "is null"}";
		}
	}
}
