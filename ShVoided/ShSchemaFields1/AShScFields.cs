#region + Using Directives

#endregion

// user name: jeffs
// created:   10/23/2022 9:05:41 PM

namespace ShSchemaFields
{
	public abstract class AShScFields<TKey, Tdata> : IShScFieldsBase<TKey,Tdata>  where TKey : Enum where Tdata : IScBase<TKey>, new()
	{

		public abstract Dictionary<TKey, Tdata> Fields { get; }


		public override string ToString()
		{
			return $"this is {nameof(AShScFields<TKey, Tdata>)} | TKey| {typeof(TKey)} | Tdata| {typeof(Tdata)}";
		}
	}
}
