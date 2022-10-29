#region + Using Directives
using ShExStorageC.ShSchemaFields;

using ShExStorageN.ShSchemaFields;

using System;
using System.Collections.Generic;
using System.Windows.Input;
using ShExStorageN.ShSchemaFields.ShScSupport;
// using ShExStorageC.ShSchemaData;
using static ShExStorageN.ShSchemaFields.ShScSupport.ScFieldColumns;

#endregion

// user name: jeffs
// created:   10/22/2022 9:00:09 AM

namespace ShStudy.ShEval
{
	public class ScValues<TKey> where TKey : Enum
	{

		public Dictionary<TKey, Dictionary<ScFieldColumns, string>> ScFieldValues { get; set; }
		public Dictionary<TKey, Dictionary<ScFieldColumns, string>> ScDataValues { get; set; }
		

		public void setFieldsValues(Dictionary<TKey, ShScFieldDefMeta<TKey>> fieldList)
		{
			ScFieldValues = new Dictionary<TKey, Dictionary<ScFieldColumns, string>>();

			foreach (KeyValuePair<TKey, ShScFieldDefMeta<TKey>> kvp in fieldList)
			{
				ShScFieldDefMeta<TKey> lf = kvp.Value;

				Dictionary<ScFieldColumns, string> f = new Dictionary<ScFieldColumns, string>();


				// native to ScFieldDef
				f.Add(SFC_KEY, lf.Key.ToString());
				f.Add(SFC_NAME, lf.Name);
				f.Add(SFC_DESC, lf.Description);
				f.Add(SFC_VALUE, lf.Value?.ToString());
				f.Add(SFC_VALUE_STR, lf.DyValue.ConvertValueTo<string>());
				f.Add(SFC_VALUE_TYPE, lf.DyValue.TypeIs.Name);
				f.Add(SFC_REVIT_TYPE, lf.DyValue.GetRevitSpecIdCustom().ToString());
				f.Add(SFC_FIELD, lf.Key.ToString());
				f.Add(SFC_DISPLAY_LEVEL, lf.DisplayLevel.ToString());

				ScFieldValues.Add(lf.Key, f);
			}
		}

		public void setDataValues(Dictionary<TKey, ShScFieldDefData<TKey>> fieldList)
		{
			ScDataValues = new Dictionary<TKey, Dictionary<ScFieldColumns, string>>();

			foreach (KeyValuePair<TKey, ShScFieldDefData<TKey>> kvp in fieldList)
			{
				ShScFieldDefData<TKey> lf = kvp.Value;

				IShScFieldMeta<TKey> fld = lf.Field;

				Dictionary<ScFieldColumns, string> f = new Dictionary<ScFieldColumns, string>();

				// native to ScDataDef
				f.Add(SFC_KEY, lf.Key.ToString());
				f.Add(SFC_VALUE, lf.DyValue.Value?.ToString());
				f.Add(SFC_VALUE_STR, lf.DyValue.ConvertValueTo<string>());
				f.Add(SFC_VALUE_TYPE, lf.DyValue.TypeIs?.Name);
				f.Add(SFC_REVIT_TYPE, lf.DyValue.GetRevitSpecIdCustom().TypeId);
				f.Add(SFC_FIELD, $"{lf.Field.Name} ({lf.Field.Key})");

				// derived
				f.Add(SFC_NAME, fld.Name);
				f.Add(SFC_DESC, fld.Description);

				ScDataValues.Add(lf.Key, f);
			}
		}

		public void setDataValues<Tdata>(Dictionary<TKey, Tdata> fieldList)
		where Tdata :  ShScFieldDefData<TKey>, new()
		{
			ScDataValues = new Dictionary<TKey, Dictionary<ScFieldColumns, string>>();

			foreach (KeyValuePair<TKey, Tdata> kvp in fieldList)
			{
				ShScFieldDefData<TKey> lf = kvp.Value;

				IShScFieldMeta<TKey> fld = lf.Field;

				Dictionary<ScFieldColumns, string> f = new Dictionary<ScFieldColumns, string>();

				// native to ScDataDef
				f.Add(SFC_KEY, lf.Key.ToString());
				f.Add(SFC_VALUE, lf.DyValue.Value?.ToString());
				f.Add(SFC_VALUE_STR, lf.DyValue.ConvertValueTo<string>());
				f.Add(SFC_VALUE_TYPE, lf.DyValue.TypeIs?.Name);
				f.Add(SFC_REVIT_TYPE, lf.DyValue.GetRevitSpecIdCustom().TypeId);
				f.Add(SFC_FIELD, $"{lf.Field.Name} ({lf.Field.Key})");

				// derived
				f.Add(SFC_NAME, fld.Name);
				f.Add(SFC_DESC, fld.Description);

				ScDataValues.Add(lf.Key, f);
			}
		}

	}
}
