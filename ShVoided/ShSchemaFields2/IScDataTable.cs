#region + Using Directives

#endregion

// user name: jeffs
// created:   10/29/2022 8:35:18 AM

namespace ShSchemaFields
{
	public interface IScDataSheet<TKey, Tdata>
	where TKey : Enum
	// expected data is: IShScFieldData<TKey>
	where Tdata : IShScFieldData<TKey>, new()
	{
	}
}
