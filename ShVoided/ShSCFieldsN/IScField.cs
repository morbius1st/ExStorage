#region + Using Directives

#endregion

// user name: jeffs
// created:   10/9/2022 10:09:56 PM

namespace ShSCFields
{
	public interface IScField<TE> where TE : Enum
	{
		TE Key { get; set; }
		string Name { get; set; }
		string Description { get; set; }
		DynaValue Value { get; set; }
		SchemaFieldDisplayLevel DisplayLevel { get; set; }
	}
}
