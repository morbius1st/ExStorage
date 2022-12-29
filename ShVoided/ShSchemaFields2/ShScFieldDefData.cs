#region + Using Directives

using static ShExStorageC.ShSchemaFields1.ScSupport1.SchemaLockKey;
using static ShExStorageN.ShSchemaFields.ShScSupport1.SchemaFieldDisplayLevel;

#endregion

// user name: jeffs
// created:   10/9/2022 10:26:53 PM


namespace ShSchemaFields2
{
	/// <summary>
	/// Schema Field Definition for Schema
	/// </summary>
	public class ShScFieldDefData<TKey> : IShScFieldData<TKey> where TKey : Enum
	{

		public TKey Key { get; private set; }

		public string Name => MetaField.Name;
		public string Description => MetaField.Description;

		public DynaValue DyValue { get; private set; }
		public dynamic Value => DyValue.Value;


		public IShScFieldMeta<TKey> MetaField { get; private set; }


		public ShScFieldDefData() { }

		public ShScFieldDefData(TKey key,
			DynaValue value
			,
			IShScFieldMeta<TKey> field
			)
		{
			Key = key;
			DyValue = value;
			MetaField = field;
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

		public string ValueAsString => DyValue.AsString();

		public IShScFieldData<TKey> Create(IShScFieldMeta<TKey> field)
		{
			return new ShScFieldDefData<TKey>(field.Key, field.DyValue, field);
		}

		public TD GetValue<TD>()
		{
			return DyValue.ConvertValueTo<TD>();
		}




		public override string ToString()
		{
			return $"this is {nameof(ShScFieldDefData<TKey>)} | value| {Value}";
		}
	}
}