#region + Using Directives

#endregion

// user name: jeffs
// created:   10/24/2022 7:07:09 PM

namespace ShSchemaFields2
{
	public abstract class AShScInfoBase<TKey, Tdata> 
		: IShScInfoBase<TKey, Tdata> 
		where TKey : Enum 
		where Tdata : IShScFieldBase<TKey>, new()
	{

		protected AShScInfoBase()
		{
			Fields = new Dictionary<TKey, Tdata>();
		}

		public abstract string SchemaName { get; }

		public abstract Dictionary<TKey, Tdata> Fields { get; protected set; }

		public string ValAsString(TKey key)
		{
			return Fields[key].DyValue.AsString();
		}

		public void Add(Tdata info)
		{
			Fields.Add(info.Key, info);
		}


	}
}
