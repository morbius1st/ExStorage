using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// user name: jeffs
// created:   9/15/2025 8:16:18 PM

namespace ExStorSys
{
	// public interface IFieldDef<Te> 
	// 	where Te : Enum
	// {
	// 	public Te FieldKey { get; }
	//
	// 	public string FieldName { get; }
	// 	public string FieldDesc { get; }
	// 	public Type FieldType { get; }
	// 	public DynaValue FieldDefValue { get; }
	// 	public FieldEditLevel FieldEditLevel { get; }
	// 	public FieldUsage FieldUse { get; }
	// }

	/// <summary>
	/// the definition of a single schema field.  a field is the information which describes a schema field<br/>
	/// and is used to link the schema fields value
	/// </summary>
	public class FieldDef<Te> // : IFieldDef<Te>
		where Te : Enum
	{
		public FieldDef(  Te fieldKey, string fieldName, string fieldDesc,
			DynaValue? fieldDefValue, FieldUsage fieldUse, FieldEditLevel fieldEditLevel)
		{
			FieldKey = fieldKey;
			FieldName = fieldName;
			FieldDesc = fieldDesc;
			FieldDefValue = fieldDefValue;
			FieldEditLevel = fieldEditLevel;
			FieldUse = fieldUse;
		}

		public Te FieldKey { get; }
		public string FieldName { get; }
		public string FieldDesc { get; }
		public Type FieldType => FieldDefValue?.TypeIs ?? typeof(object);
		public DynaValue? FieldDefValue { get; }
		public FieldEditLevel FieldEditLevel { get; }
		public FieldUsage FieldUse { get; }


		public override string ToString()
		{
			return $"this is {nameof(FieldDef<Te>)}";
		}
	}
}