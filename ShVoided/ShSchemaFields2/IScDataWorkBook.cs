#region + Using Directives

#endregion

// user name: jeffs
// created:   10/29/2022 8:13:12 AM

namespace ShSchemaFields
{
	// example:
	// IScDataWorkBook<
	// SchemaSheetKey,
	// ScDataSheet1   (<SchemaSheetKey, ShScFieldDefData<SchemaSheetKey>>,)
	// ShScFieldDefData<SchemaSheetKey>


	public interface IScDataWorkBook<TKey, TSheetData, TFieldData>
	where TKey : Enum
	// expected actual is 
	where TSheetData : IShScInfoBase<TKey, TFieldData>, new()
	where TFieldData : IShScFieldData<TKey>, new()
	{

		Dictionary<string, TSheetData> Sheets { get; }


	}
}
