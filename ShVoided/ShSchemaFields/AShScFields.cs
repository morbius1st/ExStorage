#region + Using Directives

#endregion

// user name: jeffs
// created:   10/23/2022 9:05:41 PM

namespace ShSchemaFields
{
	public abstract class AShScFields<Tkey, Tdata> : IShScFieldsBase<Tkey,Tdata>  where Tkey : Enum where Tdata : IScBase<Tkey>, new()
	{

		public abstract Dictionary<Tkey, Tdata> Fields { get; }


		public override string ToString()
		{
			return $"this is {nameof(AShScFields<Tkey, Tdata>)} | Tkey| {typeof(Tkey)} | Tdata| {typeof(Tdata)}";
		}
	}
}
