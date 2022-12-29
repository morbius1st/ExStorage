#region + Using Directives

#endregion

// user name: jeffs
// created:   10/23/2022 9:00:35 PM

namespace ShSchemaFields2
{
	public interface IShScInfoBase<TKey, Tdata> where TKey : Enum where Tdata : IShScFieldBase<TKey>, new()
	{
		Dictionary<TKey, Tdata> Fields { get; }

		string SchemaName { get; }

		string ValAsString(TKey key);

	}

	public interface IShScBaseData<TKey, Tdata, TCkey, TCdata> : IShScInfoBase<TKey, Tdata>
		where TKey : Enum 
		where TCkey : Enum 
		where Tdata : ShScFieldDefData<TKey>, new()
		where TCdata : AShScInfoBase<TCkey, ShScFieldDefData<TCkey>>, new()
	{
		Dictionary<string, TCdata> Rows { get; }
	}

	//public Dictionary<string, ScDataRow> Rows { get; protected set; }
}
