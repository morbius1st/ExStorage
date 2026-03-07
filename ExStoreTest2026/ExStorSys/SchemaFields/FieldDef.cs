using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityLibrary;


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
	// 	public ItemUsage FieldUse { get; }
	//  public FieldStatus FieldStatus { get; }
	//  public FieldCopyType FieldCopyType { get; }
	// }

	// this is v2

	/// <summary>
	/// the definition of a single schema field.
	/// the schema field defines the information actual<br/>
	/// information stored in a DataStorage object<br/>
	/// this also contains field management and usage information<br/>
	/// v2 - added clean / dirty flag | added Duplicate Flag
	/// </summary>
	public class FieldDef<Te> // : IFieldDef<Te>
		where Te : Enum
	{
		public FieldDef(Te fieldKey, string fieldName, string fieldDesc,
			string? fieldPropName, DynaValue? fieldDefValue, ItemUsage fieldUse, 
			FieldEditLevel fieldEditLevel, FieldCopyType fieldCopyType)
		{
			FieldKey = fieldKey;
			FieldName = fieldName;
			FieldDesc = fieldDesc;
			FieldPropName = fieldPropName;
			FieldDefValue = fieldDefValue;
			FieldEditLevel = fieldEditLevel;
			FieldUse = fieldUse;
			FieldCopyType = fieldCopyType;
		}

		/// <summary>
		/// the access key
		/// </summary>
		public Te FieldKey { get; }
		
		/// <summary>
		/// human readable name for the field
		/// </summary>
		public string FieldName { get; }
		
		/// <summary>
		/// human readable description
		/// </summary>
		public string FieldDesc { get; }
		
		/// <summary>
		/// the name of the property in the class to allow undo prop change notifications
		/// </summary>
		public string? FieldPropName {get;}

		/// <summary>
		/// the type for the actual data
		/// </summary>
		public Type FieldType => FieldDefValue?.TypeIs ?? typeof(object);
		
		/// <summary>
		/// the actual value to be saved to the DS
		/// </summary>
		public DynaValue? FieldDefValue { get; }
		
		/// <summary>
		/// field editing security info
		/// </summary>
		public FieldEditLevel FieldEditLevel { get; }
		
		/// <summary>
		/// where used - schema or DS
		/// </summary>
		public ItemUsage FieldUse { get; }

		/// <summary>
		/// determines if the field should be duplicated when
		/// making a copy of the data
		/// </summary>
		public FieldCopyType FieldCopyType { get; }

		// public bool IsClean { get; set; } = true;

		public override string ToString()
		{
			return $"this is {FieldName}";
		}
	}
}