#region + Using Directives
using ShExStorageC.ShSchemaFields;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using ShExStorageC.ShSchemaFields.ShScSupport;
using ShExStorageN.ShSchemaFields;
using ShExStorageN.ShSchemaFields.ShScSupport;
// using ShExStorageC.ShSchemaData;
using static ShExStorageN.ShSchemaFields.ShScSupport.ScFieldColumns;

#endregion

// user name: jeffs
// created:   10/22/2022 9:00:09 AM

namespace ShStudyN.ShEval
{
	/// <summary>
	/// translates the information in the data sheets into a format used to display
	/// the information using the show library
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	public class ScValues <TKey> where TKey : Enum
	{

		public Dictionary<TKey, Dictionary<ScFieldColumns, string>> ScFieldValues { get; set; }
		public Dictionary<TKey, Dictionary<ScFieldColumns, string>> ScDataValues { get; set; }
		

		public void setFieldsValues(Dictionary<TKey, ScFieldDefMeta<TKey>> fieldList)
		{
			ScFieldValues = new Dictionary<TKey, Dictionary<ScFieldColumns, string>>();

			foreach (KeyValuePair<TKey, ScFieldDefMeta<TKey>> kvp in fieldList)
			{
				ScFieldDefMeta<TKey> lf = kvp.Value;

				Dictionary<ScFieldColumns, string> f = new Dictionary<ScFieldColumns, string>();

				// native
				f.Add(SFC_KEY, lf.FieldKey.ToString());
				f.Add(SFC_NAME, lf.FieldName);
				f.Add(SFC_DESC, lf.FieldDesc);
				f.Add(SFC_VALUE, lf.DyValue.AsString() ?? "is null");
				f.Add(SFC_VALUE_STR, lf.DyValue.GetValueAs<string>());
				f.Add(SFC_VALUE_TYPE, lf.DyValue.TypeIs?.Name ?? "is null");
				f.Add(SFC_REVIT_TYPE, lf.DyValue.GetRevitSpecIdCustom()?.ToString() ?? "is null");
				f.Add(SFC_FIELD, lf.FieldKey?.ToString() ?? "is null");
				f.Add(SFC_DISPLAY_LEVEL, lf.DisplayLevel.ToString());

				ScFieldValues.Add(lf.FieldKey, f);
			}
		}

		// create a list of values to display
		public void setDataValues<Tdata>(Dictionary<TKey, Tdata> fieldList)
			where Tdata :  IShScFieldData<TKey>, new()
		{
			ScDataValues = new Dictionary<TKey, Dictionary<ScFieldColumns, string>>();

			foreach (KeyValuePair<TKey, Tdata> kvp in fieldList)
			{
				IShScFieldData<TKey> lf = kvp.Value;

				IShScFieldMeta<TKey> fld = lf.MetaField;

				Dictionary<ScFieldColumns, string> f = new Dictionary<ScFieldColumns, string>();

				// native
				f.Add(SFC_KEY, lf.FieldKey.ToString());
				f.Add(SFC_VALUE, lf.DyValue.Value?.ToString());
				f.Add(SFC_VALUE_STR, lf.DyValue.GetValueAs<string>());
				f.Add(SFC_VALUE_TYPE, lf.DyValue.TypeIs?.Name);
				f.Add(SFC_REVIT_TYPE, lf.DyValue.GetRevitSpecIdCustom().TypeId);
				f.Add(SFC_FIELD, $"{lf.MetaField.FieldName} ({lf.MetaField.FieldKey})");

				// derived
				f.Add(SFC_NAME,	fld.FieldName);
				f.Add(SFC_DESC, fld.FieldDesc);

				ScDataValues.Add(lf.FieldKey, f);
			}
		}

		// create a list of values to display
		public void setDataValues<Tdata>(AShScFields<TKey, Tdata> fieldList)
			where Tdata :  IShScFieldData<TKey>, new()
		{
			ScDataValues = new Dictionary<TKey, Dictionary<ScFieldColumns, string>>();

			foreach (KeyValuePair<TKey, Tdata> kvp in fieldList)
			{
				IShScFieldData<TKey> lf = kvp.Value;

				IShScFieldMeta<TKey> fld = lf.MetaField;

				Dictionary<ScFieldColumns, string> f = new Dictionary<ScFieldColumns, string>();

				// native
				f.Add(SFC_KEY, lf.FieldKey.ToString());
				f.Add(SFC_VALUE, lf.DyValue.Value?.ToString());
				f.Add(SFC_VALUE_STR, lf.DyValue.GetValueAs<string>());
				f.Add(SFC_VALUE_TYPE, lf.DyValue.TypeIs?.Name);
				f.Add(SFC_REVIT_TYPE, lf.DyValue.GetRevitSpecIdCustom().TypeId);
				f.Add(SFC_FIELD, $"{lf.MetaField.FieldName} ({lf.MetaField.FieldKey})");

				// derived
				f.Add(SFC_NAME,	fld.FieldName);
				f.Add(SFC_DESC, fld.FieldDesc);

				ScDataValues.Add(lf.FieldKey, f);
			}
		}


		// // create a list of values to display
		// public void setDataValues2<Tdata>(AShScFields<TKey, Tdata> shf)
		// 	where Tdata :  IShScFieldData<TKey>, new()
		// {
		// 	ScDataValues = new Dictionary<TKey, Dictionary<ScFieldColumns, string>>();
		//
		//
		// 	foreach (Tdata td in shf)
		// 	{
		// 		IShScFieldData<TKey> lf = td;
		// 		IShScFieldMeta<TKey> fld = td.MetaField;
		// 	// }
		// 	//
		// 	// foreach (KeyValuePair<TKey, Tdata> kvp in fieldList)
		// 	// {
		// 	// 	IShScFieldData<TKey> lf = kvp.Value;
		// 	//
		// 	// 	IShScFieldMeta<TKey> fld = lf.MetaField;
		//
		// 		Dictionary<ScFieldColumns, string> f = new Dictionary<ScFieldColumns, string>();
		//
		// 		// native
		// 		f.Add(SFC_KEY, lf.FieldKey.ToString());
		// 		f.Add(SFC_VALUE, lf.DyValue.Value?.ToString());
		// 		f.Add(SFC_VALUE_STR, lf.DyValue.GetValueAs<string>());
		// 		f.Add(SFC_VALUE_TYPE, lf.DyValue.TypeIs?.Name);
		// 		f.Add(SFC_REVIT_TYPE, lf.DyValue.GetRevitSpecIdCustom().TypeId);
		// 		f.Add(SFC_FIELD, $"{lf.MetaField.FieldName} ({lf.MetaField.FieldKey})");
		//
		// 		// derived
		// 		f.Add(SFC_NAME,	fld.FieldName);
		// 		f.Add(SFC_DESC, fld.FieldDesc);
		//
		// 		ScDataValues.Add(lf.FieldKey, f);
		// 	}
		// }

	}

}
